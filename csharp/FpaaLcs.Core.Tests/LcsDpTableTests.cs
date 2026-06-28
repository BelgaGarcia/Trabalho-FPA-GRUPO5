using FpaaLcs.Core.Services;
using Xunit;

namespace FpaaLcs.Core.Tests;

public class LcsDpTableTests
{
    [Fact]
    public void Montar_RoteiroDoTrabalho_CalculaComprimentoMaximoCinco()
    {
        const string helena = "ijkijkii";
        const string marcus = "ikjikji";

        var tabela = LcsDpTable.Montar(helena, marcus);

        Assert.Equal(helena.Length + 1, tabela.GetLength(0));
        Assert.Equal(marcus.Length + 1, tabela.GetLength(1));
        Assert.Equal(5, LcsDpTable.ComprimentoMaximo(tabela, helena.Length, marcus.Length));
    }

    [Fact]
    public void Montar_PrimeiraLinhaEPrimeiraColuna_PermanecemZeradas()
    {
        const string helena = "abcbdab";
        const string marcus = "bdcaba";

        var tabela = LcsDpTable.Montar(helena, marcus);

        for (int i = 0; i <= helena.Length; i++)
        {
            Assert.Equal(0, tabela[i, 0]);
        }

        for (int j = 0; j <= marcus.Length; j++)
        {
            Assert.Equal(0, tabela[0, j]);
        }
    }

    [Fact]
    public void Montar_SemCaracteresComuns_RetornaComprimentoZero()
    {
        const string helena = "abc";
        const string marcus = "xyz";

        var tabela = LcsDpTable.Montar(helena, marcus);

        Assert.Equal(0, LcsDpTable.ComprimentoMaximo(tabela, helena.Length, marcus.Length));
    }

    [Fact]
    public void Serializar_ConverteTabelaParaMatrizSimples()
    {
        const string helena = "abc";
        const string marcus = "ac";

        var tabela = LcsDpTable.Montar(helena, marcus);
        var serializada = LcsDpTable.Serializar(tabela, helena.Length, marcus.Length);

        Assert.Equal(helena.Length + 1, serializada.Length);
        Assert.All(serializada, linha => Assert.Equal(marcus.Length + 1, linha.Length));
        Assert.Equal(2, serializada[helena.Length][marcus.Length]);
    }
}
