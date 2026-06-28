namespace FpaaLcs.Core.Models;

public sealed class LcsRequest
{
    public string Helena { get; init; } = string.Empty;

    public string Marcus { get; init; } = string.Empty;

    public string Metodo { get; init; } = "dp";

    public string? ValorD { get; init; }
}
