using System.Text.Json;
using FpaaLcs.Core.Models;

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

Console.WriteLine(
    JsonSerializer.Serialize(
        new
        {
            status = "estrutura_inicial",
            helena = request?.Helena,
            marcus = request?.Marcus,
            metodo = request?.Metodo ?? "dp",
        }
    )
);
