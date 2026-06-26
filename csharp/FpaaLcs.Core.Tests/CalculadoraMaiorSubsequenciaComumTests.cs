using FpaaLcs.Core.Services;
using Xunit;

namespace FpaaLcs.Core.Tests;

public class CalculadoraMaiorSubsequenciaComumTests
{
    [Fact]
    public void Calcular_RoteiroDoTrabalho_RetornaSubsequenciasEmOrdem()
    {
        const string sequenciaHelena = "ijkijkii";
        const string sequenciaMarcus = "ikjikji";

        var resultado = CalculadoraMaiorSubsequenciaComum.Calcular(sequenciaHelena, sequenciaMarcus);

        Assert.Equal(5, resultado.ComprimentoMaiorSubsequenciaComum);
        Assert.Equal(
            new[]
            {
                "ijiji",
                "ijiki",
                "ijkji",
                "ikiji",
                "ikiki",
                "ikjii",
                "ikjki",
            },
            resultado.SubsequenciasComunsMaisLongas);
    }

    [Fact]
    public void Calcular_SemEventosIguais_RetornaSubsequenciaVazia()
    {
        const string sequenciaHelena = "abc";
        const string sequenciaMarcus = "xyz";

        var resultado = CalculadoraMaiorSubsequenciaComum.Calcular(sequenciaHelena, sequenciaMarcus);

        Assert.Equal(0, resultado.ComprimentoMaiorSubsequenciaComum);
        Assert.Equal(new[] { string.Empty }, resultado.SubsequenciasComunsMaisLongas);
    }

    [Fact]
    public void Calcular_ComEmpateNaTabela_RetornaSemRepetir()
    {
        const string sequenciaHelena = "bd";
        const string sequenciaMarcus = "db";

        var resultado = CalculadoraMaiorSubsequenciaComum.Calcular(sequenciaHelena, sequenciaMarcus);

        Assert.Equal(1, resultado.ComprimentoMaiorSubsequenciaComum);
        Assert.Equal(new[] { "b", "d" }, resultado.SubsequenciasComunsMaisLongas);
    }
}
