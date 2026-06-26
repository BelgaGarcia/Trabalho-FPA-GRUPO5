using FpaaLcs.Core.Services;
using Xunit;

namespace FpaaLcs.Core.Tests;

public class LcsBacktrackerTests
{
    [Fact]
    public void EncontrarTodas_RoteiroDoTrabalho_RetornaSeteSubsequencias()
    {
        const string helena = "ijkijkii";
        const string marcus = "ikjikji";

        var tabela = LcsDpTable.Montar(helena, marcus);
        var subsequencias = LcsBacktracker.EncontrarTodas(tabela, helena, marcus);

        Assert.Equal(5, LcsDpTable.ComprimentoMaximo(tabela, helena.Length, marcus.Length));
        Assert.Equal(7, subsequencias.Count);
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
            subsequencias
        );
    }

    [Fact]
    public void EncontrarTodas_SemCaracteresComuns_RetornaSubsequenciaVazia()
    {
        const string helena = "abc";
        const string marcus = "xyz";

        var tabela = LcsDpTable.Montar(helena, marcus);
        var subsequencias = LcsBacktracker.EncontrarTodas(tabela, helena, marcus);

        Assert.Single(subsequencias);
        Assert.Equal(string.Empty, subsequencias[0]);
    }

    [Fact]
    public void EncontrarTodas_ComEmpateNaTabela_ExploraTodosOsCaminhos()
    {
        const string helena = "bd";
        const string marcus = "db";

        var tabela = LcsDpTable.Montar(helena, marcus);
        var subsequencias = LcsBacktracker.EncontrarTodas(tabela, helena, marcus);

        Assert.Equal(new[] { "b", "d" }, subsequencias);
    }
}
