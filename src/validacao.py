"""
Validação de entradas conforme o roteiro do trabalho FPAA.

Autores: Leni Rocha Bento
Versão: 1.0.0
Data: 2026-05-25
Disciplina: Fundamentos de Projeto e Análise de Algoritmos
"""

MAX_CASOS = 10
MIN_LEN = 1
MAX_LEN = 80


class ValidacaoErro(ValueError):
    """Entrada fora das regras do enunciado."""


def _validar_sequencia(nome: str, sequencia: str) -> None:
    if not sequencia:
        raise ValidacaoErro(f"{nome}: sequencia vazia (minimo {MIN_LEN} caractere).")

    if len(sequencia) > MAX_LEN:
        raise ValidacaoErro(
            f"{nome}: sequencia com {len(sequencia)} caracteres (maximo {MAX_LEN})."
        )

    for pos, ch in enumerate(sequencia, start=1):
        if not "a" <= ch <= "z":
            raise ValidacaoErro(
                f"{nome}: caractere invalido '{ch}' na posicao {pos} "
                "(use apenas letras minusculas de a a z)."
            )


def validar_d(valor: str) -> int:
    """Valida o numero D de conjuntos de dados (1 <= D <= 10)."""
    valor = valor.strip()

    if not valor.isdigit():
        raise ValidacaoErro(f"D deve ser um inteiro positivo; recebido: '{valor}'.")

    d = int(valor)

    if d < 1:
        raise ValidacaoErro(f"D deve ser pelo menos 1; recebido: {d}.")

    if d > MAX_CASOS:
        raise ValidacaoErro(f"D nao pode exceder {MAX_CASOS}; recebido: {d}.")

    return d


def validar_caso(helena: str, marcus: str) -> tuple[str, str]:
    """Valida um par de sequencias (Helena e Marcus)."""
    helena = helena.strip()
    marcus = marcus.strip()

    _validar_sequencia("Helena", helena)
    _validar_sequencia("Marcus", marcus)

    return helena, marcus
