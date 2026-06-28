"""
Leitura de stdin e formatação de saída conforme o roteiro.

Autores: Grupo FPAA — PUC Minas Contagem (preencher nomes dos 7–8 integrantes)
Versão: 1.0.0
Data: 2026-05-25
"""

import sys
from typing import Callable, List, Tuple

from .validacao import ValidacaoErro, validar_caso, validar_d


def ler_casos_stdin() -> Tuple[int, List[Tuple[str, str]]]:
    """Lê D e os pares Helena/Marcus da entrada padrão."""
    linhas = [ln.rstrip("\n") for ln in sys.stdin.readlines()]
    if not linhas:
        raise ValidacaoErro("Entrada vazia: informe D na primeira linha.")

    d = validar_d(linhas[0])
    esperadas = 1 + 2 * d
    if len(linhas) != esperadas:
        raise ValidacaoErro(
            f"Esperadas {esperadas} linhas (D + 2 por caso); recebidas {len(linhas)}."
        )

    casos: List[Tuple[str, str]] = []
    idx = 1
    for caso in range(1, d + 1):
        helena = linhas[idx]
        marcus = linhas[idx + 1]
        casos.append(validar_caso(helena, marcus))
        idx += 2

    return d, casos


def formatar_saida(blocos: List[List[str]]) -> str:
    """Junta blocos de subsequências separados por linha em branco."""
    partes = ["\n".join(bloco) for bloco in blocos if bloco]
    return "\n\n".join(partes) + ("\n" if partes else "")


def processar_stdin(resolver: Callable[[str, str], List[str]]) -> None:
    """Pipeline completo: ler, validar, resolver e imprimir."""
    try:
        _, casos = ler_casos_stdin()
        blocos = [resolver(h, m) for h, m in casos]
        sys.stdout.write(formatar_saida(blocos))
    except ValidacaoErro as erro:
        sys.stderr.write(f"Erro de validação: {erro}\n")
        sys.exit(1)
