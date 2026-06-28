"""
Tabela de programação dinâmica para comprimento da LCS.

Autores: Grupo FPAA — PUC Minas Contagem (preencher nomes dos 7–8 integrantes)
Versão: 1.0.0
Data: 2026-05-25
"""

from typing import List


def construir_tabela_dp(helena: str, marcus: str) -> List[List[int]]:
    """
    Preenche L[i][j] = comprimento da maior subsequência comum dos prefixos
    helena[0:i] e marcus[0:j], usando programação dinâmica bottom-up.
    """
    n, m = len(helena), len(marcus)
    tabela = [[0] * (m + 1) for _ in range(n + 1)]

    for i in range(1, n + 1):
        for j in range(1, m + 1):
            if helena[i - 1] == marcus[j - 1]:
                tabela[i][j] = tabela[i - 1][j - 1] + 1
            else:
                tabela[i][j] = max(tabela[i - 1][j], tabela[i][j - 1])

    return tabela


def comprimento_lcs(helena: str, marcus: str) -> int:
    """Retorna apenas o comprimento máximo da subsequência comum."""
    tabela = construir_tabela_dp(helena, marcus)
    return tabela[len(helena)][len(marcus)]
