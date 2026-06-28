"""
Enumeração de todas as LCS com backtracking recursivo sobre a tabela DP.

Autores: Grupo FPAA — PUC Minas Contagem (preencher nomes dos 7–8 integrantes)
Versão: 1.0.0
Data: 2026-05-25
"""

from typing import List, Set

from .lcs_dp import construir_tabela_dp


def todas_lcs_backtracking(helena: str, marcus: str) -> List[str]:
    """
    Após construir a tabela DP, usa backtracking recursivo para listar todas
    as subsequências comuns de comprimento máximo.
    """
    tabela = construir_tabela_dp(helena, marcus)
    n, m = len(helena), len(marcus)
    encontradas: Set[str] = set()

    def backtrack(i: int, j: int, caminho: List[str]) -> None:
        if i == 0 and j == 0:
            encontradas.add("".join(reversed(caminho)))
            return

        if (
            i > 0
            and j > 0
            and helena[i - 1] == marcus[j - 1]
            and tabela[i][j] == tabela[i - 1][j - 1] + 1
        ):
            backtrack(i - 1, j - 1, caminho + [helena[i - 1]])
            return

        if i > 0 and tabela[i][j] == tabela[i - 1][j]:
            backtrack(i - 1, j, caminho)
        if j > 0 and tabela[i][j] == tabela[i][j - 1]:
            backtrack(i, j - 1, caminho)

    backtrack(n, m, [])
    return sorted(encontradas)
