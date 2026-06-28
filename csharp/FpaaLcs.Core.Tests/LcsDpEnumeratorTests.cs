using FpaaLcs.Core.Services;
using Xunit;

namespace FpaaLcs.Core.Tests;

public class LcsDpEnumeratorTests
{
    [Fact]
    public void EncontrarTodas_RoteiroDoTrabalho_RetornaSeteSubsequencias()
    {
        const string helena = "ijkijkii";
        const string marcus = "ikjikji";

        var tabela = LcsDpTable.Montar(helena, marcus);
        var subsequencias = LcsDpEnumerator.EncontrarTodas(tabela, helena, marcus);

        Assert.Equal(5, LcsDpTable.ComprimentoMaximo(tabela, helena.Length, marcus.Length));
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
    public void EncontrarTodas_ComEmpateNaTabela_RetornaEmOrdemAlfabetica()
    {
        const string helena = "bd";
        const string marcus = "db";

        var tabela = LcsDpTable.Montar(helena, marcus);
        var subsequencias = LcsDpEnumerator.EncontrarTodas(tabela, helena, marcus);

        Assert.Equal(new[] { "b", "d" }, subsequencias);
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

    [Fact]
    public void EncontrarTodas_CasoComRepeticoes_EquivaleAoBacktracking()
    {
        const string helena = "abcabcaa";
        const string marcus = "acbacba";

        var tabela = LcsDpTable.Montar(helena, marcus);
        var enumeracao = LcsDpEnumerator.EncontrarTodas(tabela, helena, marcus);
        var backtracking = LcsBacktracker.EncontrarTodas(tabela, helena, marcus);

        Assert.Equal(backtracking, enumeracao);
    }
}
