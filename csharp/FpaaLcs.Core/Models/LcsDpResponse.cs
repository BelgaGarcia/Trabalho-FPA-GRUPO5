namespace FpaaLcs.Core.Models;

public sealed class LcsDpResponse
{
    public string Helena { get; init; } = string.Empty;

    public string Marcus { get; init; } = string.Empty;

    public int ComprimentoMaximo { get; init; }

    public int[][] TabelaDp { get; init; } = [];

    public string[] Padroes { get; init; } = [];
}
