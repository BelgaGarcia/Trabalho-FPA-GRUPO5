using System.Text.Json;
using FpaaLcs.Core.Models;
using FpaaLcs.Core.Services;

var input = Console.In.ReadToEnd();

if (string.IsNullOrWhiteSpace(input))
{
    Console.WriteLine("{\"status\":\"aguardando_entrada\"}");
    return;
}

var options = new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true,
};

var request = JsonSerializer.Deserialize<AppRequest>(input, options);
var helena = request?.Helena ?? string.Empty;
var marcus = request?.Marcus ?? string.Empty;
var metodo = request?.Metodo ?? "dp";

if (metodo == "backtracking")
{
    var tabela = LcsDpTable.Montar(helena, marcus);
    var comprimentoMaximo = LcsDpTable.ComprimentoMaximo(tabela, helena.Length, marcus.Length);
    var subsequencias = LcsBacktracker.EncontrarTodas(tabela, helena, marcus);

    Console.WriteLine(
        JsonSerializer.Serialize(
            new
            {
                status = "ok",
                metodo = "backtracking",
                helena,
                marcus,
                comprimento_maximo = comprimentoMaximo,
                quantidade = subsequencias.Count,
                subsequencias,
                tabela = LcsDpTable.Serializar(tabela, helena.Length, marcus.Length),
            }
        )
    );
    return;
}

Console.WriteLine(
    JsonSerializer.Serialize(
        new
        {
            status = "estrutura_inicial",
            helena = request?.Helena,
            marcus = request?.Marcus,
            metodo,
        }
    )
);
