namespace FpaaLcs.Core.Services;

/// <summary>
/// Enumera todas as maiores subsequencias comuns usando a tabela DP e uma pilha explicita.
/// </summary>
public static class LcsDpEnumerator
{
    public static IReadOnlyList<string> EncontrarTodas(int[,] tabela, string helena, string marcus)
    {
        var encontradas = new HashSet<string>();
        var pilha = new Stack<(int I, int J, List<char> Caminho)>();

        pilha.Push((helena.Length, marcus.Length, new List<char>()));

        while (pilha.Count > 0)
        {
            var (i, j, caminho) = pilha.Pop();

            if (i == 0 && j == 0)
            {
                var caracteres = caminho.ToArray();
                Array.Reverse(caracteres);
                encontradas.Add(new string(caracteres));
                continue;
            }

            if (
                i > 0
                && j > 0
                && helena[i - 1] == marcus[j - 1]
                && tabela[i, j] == tabela[i - 1, j - 1] + 1
            )
            {
                var proximoCaminho = new List<char>(caminho) { helena[i - 1] };
                pilha.Push((i - 1, j - 1, proximoCaminho));
                continue;
            }

            if (i > 0 && tabela[i, j] == tabela[i - 1, j])
            {
                pilha.Push((i - 1, j, new List<char>(caminho)));
            }

            if (j > 0 && tabela[i, j] == tabela[i, j - 1])
            {
                pilha.Push((i, j - 1, new List<char>(caminho)));
            }
        }

        return encontradas
            .OrderBy(subsequencia => subsequencia, StringComparer.Ordinal)
            .ToList();
    }
}
