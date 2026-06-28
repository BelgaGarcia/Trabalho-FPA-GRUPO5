"""
Enumeração de todas as LCS usando apenas a tabela DP e pilha iterativa.

Não utiliza recursão de backtracking — explora caminhos ótimos com estrutura
de pilha explícita (reconstrução guiada pela tabela L).

Autores: Grupo FPAA — PUC Minas Contagem (preencher nomes dos 7–8 integrantes)
Versão: 1.0.0
Data: 2026-05-25
"""

from typing import List, Set, Tuple

from .lcs_dp import construir_tabela_dp


def todas_lcs_iterativo(helena: str, marcus: str) -> List[str]:
    """
    Retorna todas as subsequências comuns de comprimento máximo, sem duplicatas,
    em ordem alfabética, via enumeração iterativa baseada na tabela DP.
    """
    tabela = construir_tabela_dp(helena, marcus)
    n, m = len(helena), len(marcus)
    encontradas: Set[str] = set()

    # Estado: (i, j, caminho construído de trás para frente)
    pilha: List[Tuple[int, int, List[str]]] = [(n, m, [])]

    while pilha:
        i, j, caminho = pilha.pop()

        if i == 0 and j == 0:
            encontradas.add("".join(reversed(caminho)))
            continue

        # Caso de match: único caminho quando L[i][j] veio do diagonal
        if (
            i > 0
            and j > 0
            and helena[i - 1] == marcus[j - 1]
            and tabela[i][j] == tabela[i - 1][j - 1] + 1
        ):
            pilha.append((i - 1, j - 1, caminho + [helena[i - 1]]))
            continue

        # Empate: pode vir de cima ou da esquerda — empilha ambos os caminhos
        if i > 0 and tabela[i][j] == tabela[i - 1][j]:
            pilha.append((i - 1, j, caminho))
        if j > 0 and tabela[i][j] == tabela[i][j - 1]:
            pilha.append((i, j - 1, caminho))

    return sorted(encontradas)
