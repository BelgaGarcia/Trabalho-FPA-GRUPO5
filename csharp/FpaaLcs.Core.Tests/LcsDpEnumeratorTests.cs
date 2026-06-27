using FpaaLcs.Core.Services;
using Xunit;

namespace FpaaLcs.Core.Tests;

public class LcsDpEnumeratorTests
{
    [Fact]
    public void EncontrarTodas_ComSubsequenciaUnica_EnumeraIterativamente()
    {
        const string helena = "abc";
        const string marcus = "abc";

        var tabela = LcsDpTable.Montar(helena, marcus);
        var subsequencias = LcsDpEnumerator.EncontrarTodas(tabela, helena, marcus);

        Assert.Equal(3, LcsDpTable.ComprimentoMaximo(tabela, helena.Length, marcus.Length));
        Assert.Equal(new[] { "abc" }, subsequencias);
    }

    [Fact]
    public void EncontrarTodas_ComEmpateNaTabela_ExploraCaminhosOtimos()
    {
        const string helena = "bd";
        const string marcus = "db";

        var tabela = LcsDpTable.Montar(helena, marcus);
        var subsequencias = LcsDpEnumerator.EncontrarTodas(tabela, helena, marcus);

        Assert.Equal(2, subsequencias.Count);
        Assert.Contains("b", subsequencias);
        Assert.Contains("d", subsequencias);
    }

    [Fact]
    public void EncontrarTodas_SemCaracteresComuns_RemoveDuplicatas()
    {
        const string helena = "a";
        const string marcus = "b";

        var tabela = LcsDpTable.Montar(helena, marcus);
        var subsequencias = LcsDpEnumerator.EncontrarTodas(tabela, helena, marcus);

        Assert.Single(subsequencias);
        Assert.Equal(string.Empty, subsequencias[0]);
    }
}
