namespace LcsEtapa1;

public class ResultadoMaiorSubsequenciaComum
{
    public ResultadoMaiorSubsequenciaComum(
        string sequenciaHelena,
        string sequenciaMarcus,
        int[,] tabelaProgramacaoDinamica,
        int comprimentoMaiorSubsequenciaComum,
        IReadOnlyList<string> subsequenciasComunsMaisLongas)
    {
        SequenciaHelena = sequenciaHelena;
        SequenciaMarcus = sequenciaMarcus;
        TabelaProgramacaoDinamica = tabelaProgramacaoDinamica;
        ComprimentoMaiorSubsequenciaComum = comprimentoMaiorSubsequenciaComum;
        SubsequenciasComunsMaisLongas = subsequenciasComunsMaisLongas;
    }

    public string SequenciaHelena { get; }

    public string SequenciaMarcus { get; }

    public int[,] TabelaProgramacaoDinamica { get; }

    public int ComprimentoMaiorSubsequenciaComum { get; }

    public IReadOnlyList<string> SubsequenciasComunsMaisLongas { get; }
}
