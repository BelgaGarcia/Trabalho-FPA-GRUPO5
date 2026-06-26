namespace FpaaLcs.Core.Services;

/// <summary>
/// Enumera todas as maiores subsequencias comuns (LCS) via backtracking recursivo
/// sobre a tabela DP, explorando empates entre celulas vizinhas.
/// </summary>
public static class LcsBacktracker
{
    public static IReadOnlyList<string> EncontrarTodas(int[,] tabela, string helena, string marcus)
    {
        var memo = new Dictionary<(int I, int J), HashSet<string>>();
        var subsequencias = Coletar(tabela, helena, marcus, helena.Length, marcus.Length, memo);

        return subsequencias
            .OrderBy(subsequencia => subsequencia, StringComparer.Ordinal)
            .ToList();
    }

    private static HashSet<string> Coletar(
        int[,] tabela,
        string helena,
        string marcus,
        int i,
        int j,
        Dictionary<(int I, int J), HashSet<string>> memo)
    {
        if (i == 0 || j == 0)
        {
            return new HashSet<string> { string.Empty };
        }

        var chave = (i, j);
        if (memo.TryGetValue(chave, out var emCache))
        {
            return emCache;
        }

        HashSet<string> resultado;

        if (helena[i - 1] == marcus[j - 1])
        {
            resultado = new HashSet<string>();
            foreach (var prefixo in Coletar(tabela, helena, marcus, i - 1, j - 1, memo))
            {
                resultado.Add(prefixo + helena[i - 1]);
            }
        }
        else
        {
            resultado = new HashSet<string>();

            if (tabela[i - 1, j] >= tabela[i, j - 1])
            {
                foreach (var subsequencia in Coletar(tabela, helena, marcus, i - 1, j, memo))
                {
                    resultado.Add(subsequencia);
                }
            }

            if (tabela[i, j - 1] >= tabela[i - 1, j])
            {
                foreach (var subsequencia in Coletar(tabela, helena, marcus, i, j - 1, memo))
                {
                    resultado.Add(subsequencia);
                }
            }
        }

        memo[chave] = resultado;
        return resultado;
    }
}
