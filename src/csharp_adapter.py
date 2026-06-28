import json
import subprocess
from pathlib import Path

BASE_DIR = Path(__file__).resolve().parent.parent
C_SHARP_EXE = (
    BASE_DIR / "csharp" / "FpaaLcs.Core" / "bin" / "Debug" / "net8.0" / "FpaaLcs.Core.exe"
)


class CSharpAdapterErro(RuntimeError):
    """Falha operacional ao executar ou interpretar o nucleo C#."""


def _executar(payload: dict) -> dict:
    if not C_SHARP_EXE.exists():
        raise CSharpAdapterErro(
            "Executavel C# nao encontrado. Rode dotnet build csharp/FpaaLcs.Core primeiro."
        )

    try:
        resultado = subprocess.run(
            [str(C_SHARP_EXE)],
            input=json.dumps(payload),
            capture_output=True,
            text=True,
            cwd=BASE_DIR,
        )
    except OSError as erro:
        raise CSharpAdapterErro(f"Falha ao iniciar o nucleo C#: {erro}") from erro

    if resultado.returncode != 0:
        detalhe = resultado.stderr.strip() or resultado.stdout.strip()
        raise CSharpAdapterErro(detalhe or "Nucleo C# retornou erro.")

    try:
        data = json.loads(resultado.stdout)
    except json.JSONDecodeError as erro:
        raise CSharpAdapterErro("Nucleo C# retornou JSON invalido.") from erro

    if not isinstance(data, dict):
        raise CSharpAdapterErro("Nucleo C# retornou uma resposta invalida.")

    return data


def calcular_lcs(helena: str, marcus: str, metodo: str) -> dict:
    data = _executar({"helena": helena, "marcus": marcus, "metodo": metodo})

    campos_obrigatorios = ("comprimentoMaximo", "quantidade", "padroes", "tabelaDp")
    if any(campo not in data for campo in campos_obrigatorios):
        raise CSharpAdapterErro("Resposta do nucleo C# esta incompleta.")

    return data


def executar_nucleo(payload: dict) -> dict:
    """Compatibilidade com o adapter inicial usado nas primeiras etapas."""
    return _executar(payload)
