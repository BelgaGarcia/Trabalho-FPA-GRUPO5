"""Validação de entradas conforme o roteiro do trabalho FPAA."""

MAX_CASOS = 10
MIN_LEN = 1
MAX_LEN = 80

class ValidacaoErro(ValueError):
    """Entrada fora das regras do enunciado."""

def _validar_sequencia(nome: str, sequencia: str) -> None:
    if not sequencia:
        raise ValidacaoErro(f"{nome}: sequência vazia (mínimo {MIN_LEN} caractere).")
    if len(sequencia) > MAX_LEN:
        raise ValidacaoErro(
            f"{nome}: sequência com {len(sequencia)} caracteres (máximo {MAX_LEN})."
        )
    for pos, ch in enumerate(sequencia, start=1):
        if not "a" <= ch <= "z":
            raise ValidacaoErro(
                f"{nome}: caractere inválido '{ch}' na posição {pos} "
                "(use apenas letras minúsculas de a a z)."
            )

def validar_d(valor: str) -> int:
    """Valida o número D de conjuntos de dados (1 ≤ D ≤ 10)."""
    valor = valor.strip()
    if not valor.isdigit():
        raise ValidacaoErro(f"D deve ser um inteiro positivo; recebido: '{valor}'.")
    d = int(valor)
    if d < 1:
        raise ValidacaoErro(f"D deve ser pelo menos 1; recebido: {d}.")
    if d > MAX_CASOS:
        raise ValidacaoErro(f"D não pode exceder {MAX_CASOS}; recebido: {d}.")
    return d

def validar_caso(helena: str, marcus: str) -> tuple[str, str]:
    """Valida um par de sequências (Helena e Marcus)."""
    helena = helena.strip()
    marcus = marcus.strip()
    _validar_sequencia("Helena", helena)
    _validar_sequencia("Marcus", marcus)
    return helena, marcus
