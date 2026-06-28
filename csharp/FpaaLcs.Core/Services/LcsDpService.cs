using FpaaLcs.Core.Models;

namespace FpaaLcs.Core.Services;

public static class LcsDpService
{
    public static LcsDpResponse Calculate(string helena, string marcus, string metodo = "dp")
    {
        (helena, marcus) = ValidationService.ValidateCase(helena, marcus);

        var tabela = BuildTable(helena, marcus);
        var padroes = string.Equals(metodo, "backtracking", StringComparison.OrdinalIgnoreCase)
            ? BuildPatternsBacktracking(helena, marcus, tabela)
            : BuildPatterns(helena, marcus, tabela);

        return new LcsDpResponse
        {
            Helena = helena,
            Marcus = marcus,
            ComprimentoMaximo = tabela[helena.Length][marcus.Length],
            TabelaDp = tabela,
            Padroes = padroes,
        };
    }

    public static int[][] BuildTable(string helena, string marcus)
    {
        var n = helena.Length;
        var m = marcus.Length;
        var tabela = new int[n + 1][];

        for (var i = 0; i <= n; i++)
        {
            tabela[i] = new int[m + 1];
        }

        for (var i = 1; i <= n; i++)
        {
            for (var j = 1; j <= m; j++)
            {
                if (helena[i - 1] == marcus[j - 1])
                {
                    tabela[i][j] = tabela[i - 1][j - 1] + 1;
                }
                else
                {
                    tabela[i][j] = Math.Max(tabela[i - 1][j], tabela[i][j - 1]);
                }
            }
        }

        return tabela;
    }

    public static string[] BuildPatterns(string helena, string marcus, int[][] tabela)
    {
        var encontradas = new HashSet<string>();
        var pilha = new Stack<(int i, int j, List<char> caminho)>();

        pilha.Push((helena.Length, marcus.Length, []));

        while (pilha.Count > 0)
        {
            var (i, j, caminho) = pilha.Pop();

            if (i == 0 && j == 0)
            {
                var chars = caminho.ToArray();
                Array.Reverse(chars);
                encontradas.Add(new string(chars));
                continue;
            }

            if (
                i > 0
                && j > 0
                && helena[i - 1] == marcus[j - 1]
                && tabela[i][j] == tabela[i - 1][j - 1] + 1
            )
            {
                var proximoCaminho = new List<char>(caminho) { helena[i - 1] };
                pilha.Push((i - 1, j - 1, proximoCaminho));
                continue;
            }

            if (i > 0 && tabela[i][j] == tabela[i - 1][j])
            {
                pilha.Push((i - 1, j, new List<char>(caminho)));
            }

            if (j > 0 && tabela[i][j] == tabela[i][j - 1])
            {
                pilha.Push((i, j - 1, new List<char>(caminho)));
            }
        }

        return [.. encontradas.OrderBy(valor => valor)];
    }

    public static string[] BuildPatternsBacktracking(string helena, string marcus, int[][] tabela)
    {
        var encontradas = new HashSet<string>();

        void Backtrack(int i, int j, List<char> caminho)
        {
            if (i == 0 && j == 0)
            {
                var chars = caminho.ToArray();
                Array.Reverse(chars);
                encontradas.Add(new string(chars));
                return;
            }

            if (
                i > 0
                && j > 0
                && helena[i - 1] == marcus[j - 1]
                && tabela[i][j] == tabela[i - 1][j - 1] + 1
            )
            {
                var proximoCaminho = new List<char>(caminho) { helena[i - 1] };
                Backtrack(i - 1, j - 1, proximoCaminho);
                return;
            }

            if (i > 0 && tabela[i][j] == tabela[i - 1][j])
            {
                Backtrack(i - 1, j, new List<char>(caminho));
            }

            if (j > 0 && tabela[i][j] == tabela[i][j - 1])
            {
                Backtrack(i, j - 1, new List<char>(caminho));
            }
        }

        Backtrack(helena.Length, marcus.Length, []);
        return [.. encontradas.OrderBy(valor => valor)];
    }
}
