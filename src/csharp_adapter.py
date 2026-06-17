import json
import subprocess
from pathlib import Path

BASE_DIR = Path(__file__).resolve().parent.parent
C_SHARP_EXE = (
    BASE_DIR / "csharp" / "FpaaLcs.Core" / "bin" / "Debug" / "net8.0" / "FpaaLcs.Core.exe"
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
    return {
        "status": "ok" if resultado.returncode == 0 else "erro",
        "stdout": resultado.stdout,
        "stderr": resultado.stderr,
    }

