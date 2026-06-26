namespace FpaaLcs.Core.Services;

/// <summary>
/// Monta a tabela de programacao dinamica da LCS.
/// Modulo base compartilhado com o integrante 3 (DP).
/// </summary>
public static class LcsDpTable
{
    public static int[,] Montar(string helena, string marcus)
    {
        int m = helena.Length;
        int n = marcus.Length;
        var tabela = new int[m + 1, n + 1];

        for (int i = 1; i <= m; i++)
        {
            for (int j = 1; j <= n; j++)
            {
                if (helena[i - 1] == marcus[j - 1])
                {
                    tabela[i, j] = tabela[i - 1, j - 1] + 1;
                }
                else
                {
                    tabela[i, j] = Math.Max(tabela[i - 1, j], tabela[i, j - 1]);
                }
            }
        }

        return tabela;
    }

    public static int ComprimentoMaximo(int[,] tabela, int m, int n) => tabela[m, n];

    public static int[][] Serializar(int[,] tabela, int linhas, int colunas)
    {
        var resultado = new int[linhas + 1][];

        for (int i = 0; i <= linhas; i++)
        {
            resultado[i] = new int[colunas + 1];
            for (int j = 0; j <= colunas; j++)
            {
                resultado[i][j] = tabela[i, j];
            }
        }

        return resultado;
    }
}
