import json
import subprocess
from pathlib import Path

BASE_DIR = Path(__file__).resolve().parent.parent
C_SHARP_EXE = (
    BASE_DIR / "csharp" / "FpaaLcs.Core" / "bin" / "Debug" / "net9.0" / "FpaaLcs.Core.exe"
)


def executar_nucleo(payload: dict) -> dict:
    if not C_SHARP_EXE.exists():
        return {"status": "indisponivel", "motivo": "executavel ainda nao compilado"}

    resultado = subprocess.run(
        [str(C_SHARP_EXE)],
        input=json.dumps(payload),
        capture_output=True,
        text=True,
        cwd=BASE_DIR,
    )

    if resultado.returncode != 0:
        return {
            "status": "erro",
            "motivo": "falha ao executar nucleo csharp",
            "stderr": resultado.stderr.strip(),
        }

    try:
        return json.loads(resultado.stdout)
    except json.JSONDecodeError:
        return {
            "status": "erro",
            "motivo": "resposta invalida do nucleo csharp",
            "stdout": resultado.stdout.strip(),
        }
