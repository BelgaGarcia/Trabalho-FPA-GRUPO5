using FpaaLcs.Core.Models;

namespace FpaaLcs.Core.Services;

public static class CalculadoraMaiorSubsequenciaComum
{
    public static ResultadoMaiorSubsequenciaComum Calcular(string sequenciaHelena, string sequenciaMarcus)
    {
        int[,] tabelaProgramacaoDinamica = MontarTabelaProgramacaoDinamica(sequenciaHelena, sequenciaMarcus);

        // A resposta fica na ultima celula, pois ali as duas sequencias ja foram lidas por completo.
        int comprimentoMaiorSubsequenciaComum =
            tabelaProgramacaoDinamica[sequenciaHelena.Length, sequenciaMarcus.Length];

        IReadOnlyList<string> subsequenciasComunsMaisLongas =
            BuscarSubsequenciasComunsMaisLongas(
                sequenciaHelena,
                sequenciaMarcus,
                tabelaProgramacaoDinamica,
                comprimentoMaiorSubsequenciaComum);

        return new ResultadoMaiorSubsequenciaComum(
            sequenciaHelena,
            sequenciaMarcus,
            tabelaProgramacaoDinamica,
            comprimentoMaiorSubsequenciaComum,
            subsequenciasComunsMaisLongas);
    }

    public static int[,] MontarTabelaProgramacaoDinamica(string sequenciaHelena, string sequenciaMarcus)
    {
        int quantidadeLinhas = sequenciaHelena.Length + 1;
        int quantidadeColunas = sequenciaMarcus.Length + 1;
        int[,] tabelaProgramacaoDinamica = new int[quantidadeLinhas, quantidadeColunas];

        // A linha 0 e a coluna 0 representam comparacao com uma sequencia vazia.
        // Como o valor inicial de inteiro em C# e zero, nao precisamos preencher essa borda.
        for (int indiceHelena = 1; indiceHelena < quantidadeLinhas; indiceHelena++)
        {
            for (int indiceMarcus = 1; indiceMarcus < quantidadeColunas; indiceMarcus++)
            {
                char eventoHelena = sequenciaHelena[indiceHelena - 1];
                char eventoMarcus = sequenciaMarcus[indiceMarcus - 1];

                if (eventoHelena == eventoMarcus)
                {
                    // Se os eventos sao iguais, pegamos o resultado da diagonal e somamos esse evento.
                    tabelaProgramacaoDinamica[indiceHelena, indiceMarcus] =
                        tabelaProgramacaoDinamica[indiceHelena - 1, indiceMarcus - 1] + 1;
                }
                else
                {
                    // Se os eventos sao diferentes, mantemos o melhor resultado entre as duas opcoes:
                    // ignorar o evento atual de Helena ou ignorar o evento atual de Marcus.
                    int resultadoIgnorandoEventoHelena =
                        tabelaProgramacaoDinamica[indiceHelena - 1, indiceMarcus];

                    int resultadoIgnorandoEventoMarcus =
                        tabelaProgramacaoDinamica[indiceHelena, indiceMarcus - 1];

                    tabelaProgramacaoDinamica[indiceHelena, indiceMarcus] =
                        Math.Max(resultadoIgnorandoEventoHelena, resultadoIgnorandoEventoMarcus);
                }
            }
        }

        return tabelaProgramacaoDinamica;
    }

    public static int[][] ConverterTabelaParaMatrizSimples(
        int[,] tabelaProgramacaoDinamica,
        int tamanhoSequenciaHelena,
        int tamanhoSequenciaMarcus)
    {
        int[][] tabelaConvertida = new int[tamanhoSequenciaHelena + 1][];

        for (int linha = 0; linha <= tamanhoSequenciaHelena; linha++)
        {
            tabelaConvertida[linha] = new int[tamanhoSequenciaMarcus + 1];

            for (int coluna = 0; coluna <= tamanhoSequenciaMarcus; coluna++)
            {
                tabelaConvertida[linha][coluna] = tabelaProgramacaoDinamica[linha, coluna];
            }
        }

        return tabelaConvertida;
    }

    private static IReadOnlyList<string> BuscarSubsequenciasComunsMaisLongas(
        string sequenciaHelena,
        string sequenciaMarcus,
        int[,] tabelaProgramacaoDinamica,
        int comprimentoMaiorSubsequenciaComum)
    {
        Dictionary<(int IndiceHelena, int IndiceMarcus), SortedSet<string>> memoria = new();

        SortedSet<string> subsequenciasEncontradas = VoltarNaTabela(
            sequenciaHelena,
            sequenciaMarcus,
            tabelaProgramacaoDinamica,
            sequenciaHelena.Length,
            sequenciaMarcus.Length,
            memoria);

        return subsequenciasEncontradas
            .Where(subsequencia => subsequencia.Length == comprimentoMaiorSubsequenciaComum)
            .OrderBy(subsequencia => subsequencia, StringComparer.Ordinal)
            .ToList();
    }

    private static SortedSet<string> VoltarNaTabela(
        string sequenciaHelena,
        string sequenciaMarcus,
        int[,] tabelaProgramacaoDinamica,
        int indiceHelena,
        int indiceMarcus,
        Dictionary<(int IndiceHelena, int IndiceMarcus), SortedSet<string>> memoria)
    {
        if (indiceHelena == 0 || indiceMarcus == 0)
        {
            return new SortedSet<string>(StringComparer.Ordinal) { string.Empty };
        }

        var chave = (indiceHelena, indiceMarcus);
        if (memoria.TryGetValue(chave, out SortedSet<string>? resultadoSalvo))
        {
            return resultadoSalvo;
        }

        SortedSet<string> resultadoAtual = new(StringComparer.Ordinal);
        char eventoHelena = sequenciaHelena[indiceHelena - 1];
        char eventoMarcus = sequenciaMarcus[indiceMarcus - 1];

        if (eventoHelena == eventoMarcus)
        {
            // Se os eventos sao iguais, esse caractere faz parte das subsequencias desse caminho.
            foreach (string prefixo in VoltarNaTabela(
                         sequenciaHelena,
                         sequenciaMarcus,
                         tabelaProgramacaoDinamica,
                         indiceHelena - 1,
                         indiceMarcus - 1,
                         memoria))
            {
                resultadoAtual.Add(prefixo + eventoHelena);
            }
        }
        else
        {
            int valorAtual = tabelaProgramacaoDinamica[indiceHelena, indiceMarcus];

            // Seguimos apenas pelos vizinhos que mantem o mesmo valor da celula atual.
            // Assim o backtracking fica preso aos caminhos que ainda podem formar uma LCS.
            if (tabelaProgramacaoDinamica[indiceHelena - 1, indiceMarcus] == valorAtual)
            {
                resultadoAtual.UnionWith(VoltarNaTabela(
                    sequenciaHelena,
                    sequenciaMarcus,
                    tabelaProgramacaoDinamica,
                    indiceHelena - 1,
                    indiceMarcus,
                    memoria));
            }

            if (tabelaProgramacaoDinamica[indiceHelena, indiceMarcus - 1] == valorAtual)
            {
                resultadoAtual.UnionWith(VoltarNaTabela(
                    sequenciaHelena,
                    sequenciaMarcus,
                    tabelaProgramacaoDinamica,
                    indiceHelena,
                    indiceMarcus - 1,
                    memoria));
            }
        }

        memoria[chave] = resultadoAtual;
        return resultadoAtual;
    }
}
