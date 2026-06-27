"""
Adapter para obter validacao, tabela DP, comprimento da LCS e padroes via executavel C#.

Se o executavel nao existir ou falhar operacionalmente, usa a implementacao
Python atual, incluindo src/validacao.py como fallback.
"""

from __future__ import annotations

import json
import subprocess
from pathlib import Path
from typing import Any

from .lcs_backtracking import todas_lcs_backtracking
from .lcs_dp import comprimento_lcs, construir_tabela_dp
from .lcs_iterativo import todas_lcs_iterativo
from .validacao import ValidacaoErro, validar_caso, validar_d

BASE_DIR = Path(__file__).resolve().parent.parent
C_SHARP_EXE = (
    BASE_DIR / "csharp" / "FpaaLcs.Core" / "bin" / "Debug" / "net8.0" / "FpaaLcs.Core.exe"
)


def _fallback_python(helena: str, marcus: str, metodo: str) -> dict[str, Any]:
    helena, marcus = validar_caso(helena, marcus)
    tabela = construir_tabela_dp(helena, marcus)
    if metodo == "backtracking":
        padroes = todas_lcs_backtracking(helena, marcus)
    else:
        padroes = todas_lcs_iterativo(helena, marcus)
    return {
        "comprimentoMaximo": comprimento_lcs(helena, marcus),
        "padroes": padroes,
        "tabelaDp": tabela,
    }


def _extrair_erro_validacao(stdout: str) -> str | None:
    try:
        data = json.loads(stdout)
    except json.JSONDecodeError:
        return None

    erro = data.get("erro") if isinstance(data, dict) else None
    return erro if isinstance(erro, str) else None


def _normalizar_resposta(data: Any) -> dict[str, Any]:
    if not isinstance(data, dict):
        raise ValueError("Resposta do C# nao eh um objeto JSON.")

    comprimento = data.get("comprimentoMaximo")
    padroes = data.get("padroes")
    tabela = data.get("tabelaDp")

    if not isinstance(comprimento, int):
        raise ValueError("Campo comprimentoMaximo ausente ou invalido.")
    if not isinstance(padroes, list) or not all(isinstance(item, str) for item in padroes):
        raise ValueError("Campo padroes ausente ou invalido.")
    if not isinstance(tabela, list):
        raise ValueError("Campo tabelaDp ausente ou invalido.")

    return {
        "comprimentoMaximo": comprimento,
        "padroes": padroes,
        "tabelaDp": tabela,
    }


def calcular_dp_via_csharp(helena: str, marcus: str, metodo: str = "dp") -> dict[str, Any]:
    if not C_SHARP_EXE.exists():
        return _fallback_python(helena, marcus, metodo)

    payload = json.dumps({"helena": helena, "marcus": marcus, "metodo": metodo})

    try:
        resultado = subprocess.run(
            [str(C_SHARP_EXE)],
            input=payload,
            capture_output=True,
            text=True,
            cwd=BASE_DIR,
        )
        if resultado.returncode == 2:
            erro = _extrair_erro_validacao(resultado.stdout)
            if erro is None:
                raise ValueError("Resposta de erro de validacao invalida.")
            raise ValidacaoErro(erro)
        if resultado.returncode != 0:
            raise subprocess.SubprocessError(resultado.stderr or "Falha ao executar o C#.")

        data = json.loads(resultado.stdout)
        return _normalizar_resposta(data)
    except ValidacaoErro:
        raise
    except (OSError, subprocess.SubprocessError, json.JSONDecodeError, ValueError):
        return _fallback_python(helena, marcus, metodo)


def validar_d_via_csharp(valor: str) -> int:
    if not C_SHARP_EXE.exists():
        return validar_d(valor)

    payload = json.dumps({"valorD": valor})

    try:
        resultado = subprocess.run(
            [str(C_SHARP_EXE)],
            input=payload,
            capture_output=True,
            text=True,
            cwd=BASE_DIR,
        )
        if resultado.returncode == 2:
            erro = _extrair_erro_validacao(resultado.stdout)
            if erro is None:
                raise ValueError("Resposta de erro de validacao invalida.")
            raise ValidacaoErro(erro)
        if resultado.returncode != 0:
            raise subprocess.SubprocessError(resultado.stderr or "Falha ao executar o C#.")

        data = json.loads(resultado.stdout)
        d = data.get("valorD") if isinstance(data, dict) else None
        if not isinstance(d, int):
            raise ValueError("Campo valorD ausente ou invalido.")
        return d
    except ValidacaoErro:
        raise
    except (OSError, subprocess.SubprocessError, json.JSONDecodeError, ValueError):
        return validar_d(valor)
