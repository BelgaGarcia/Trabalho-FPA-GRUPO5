using System.Text.Json;
using FpaaLcs.Core.Models;
using FpaaLcs.Core.Services;

var input = Console.In.ReadToEnd().TrimStart('\uFEFF');

if (string.IsNullOrWhiteSpace(input))
{
    Console.Error.WriteLine("Entrada JSON vazia.");
    Environment.Exit(1);
    return;
}

var inputOptions = new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true,
};

var outputOptions = new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
};

try
{
    var request = JsonSerializer.Deserialize<AppRequest>(input, inputOptions);

    if (request is null)
    {
        Console.Error.WriteLine("Entrada JSON invalida.");
        Environment.Exit(1);
        return;
    }

    var helena = request.Helena ?? string.Empty;
    var marcus = request.Marcus ?? string.Empty;
    var metodo = string.IsNullOrWhiteSpace(request.Metodo)
        ? "dp"
        : request.Metodo.Trim().ToLowerInvariant();

    var tabela = LcsDpTable.Montar(helena, marcus);
    var padroes = metodo == "backtracking"
        ? LcsBacktracker.EncontrarTodas(tabela, helena, marcus)
        : LcsDpEnumerator.EncontrarTodas(tabela, helena, marcus);
    var comprimento = LcsDpTable.ComprimentoMaximo(tabela, helena.Length, marcus.Length);
    var tabelaSerializada = LcsDpTable.Serializar(tabela, helena.Length, marcus.Length);

    var resposta = new
    {
        Helena = helena,
        Marcus = marcus,
        ComprimentoMaximo = comprimento,
        Quantidade = padroes.Count,
        Padroes = padroes,
        Algoritmo = metodo == "backtracking" ? "DP + Backtracking" : "Enumeracao DP",
        TabelaDp = tabelaSerializada,
    };

    Console.WriteLine(JsonSerializer.Serialize(resposta, outputOptions));
}
catch (JsonException erro)
{
    Console.Error.WriteLine($"Entrada JSON invalida: {erro.Message}");
    Environment.Exit(1);
}
