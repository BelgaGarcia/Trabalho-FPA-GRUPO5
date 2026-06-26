namespace LcsEtapa1;

public static class CalculadoraMaiorSubsequenciaComum
{
    public static ResultadoMaiorSubsequenciaComum Calcular(string sequenciaHelena, string sequenciaMarcus)
    {
        int[,] tabelaProgramacaoDinamica = MontarTabelaProgramacaoDinamica(sequenciaHelena, sequenciaMarcus);

        // A resposta fica no canto inferior direito da tabela, pois ali as duas sequencias
        // foram consideradas por completo.
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

        // A linha 0 e a coluna 0 representam uma sequencia vazia.
        // Por isso elas continuam com zero, que ja e o valor inicial de um vetor de inteiros em C#.
        for (int indiceHelena = 1; indiceHelena < quantidadeLinhas; indiceHelena++)
        {
            for (int indiceMarcus = 1; indiceMarcus < quantidadeColunas; indiceMarcus++)
            {
                char eventoHelena = sequenciaHelena[indiceHelena - 1];
                char eventoMarcus = sequenciaMarcus[indiceMarcus - 1];

                if (eventoHelena == eventoMarcus)
                {
                    // Quando os eventos sao iguais, aproveitamos o melhor resultado da diagonal
                    // e somamos 1, pois encontramos mais um evento comum.
                    tabelaProgramacaoDinamica[indiceHelena, indiceMarcus] =
                        tabelaProgramacaoDinamica[indiceHelena - 1, indiceMarcus - 1] + 1;
                }
                else
                {
                    // Quando os eventos sao diferentes, ficamos com o melhor resultado ja encontrado
                    // ao ignorar um evento de Helena ou um evento de Marcus.
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

        // O filtro final deixa a resposta protegida contra caminhos que nao tenham tamanho maximo.
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
