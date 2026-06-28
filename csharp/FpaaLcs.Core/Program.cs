using System.Text.Json;
using FpaaLcs.Core.Exceptions;
using FpaaLcs.Core.Models;
using FpaaLcs.Core.Services;

var json = Console.In.ReadToEnd();

if (string.IsNullOrWhiteSpace(json))
{
    Console.Error.WriteLine("Entrada JSON vazia.");
    Environment.Exit(1);
    return;
}

var options = new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true,
};

var request = JsonSerializer.Deserialize<LcsRequest>(json, options);

if (request is null)
{
    Console.Error.WriteLine("Nao foi possivel desserializar a entrada JSON.");
    Environment.Exit(1);
    return;
}

var outputOptions = new JsonSerializerOptions
{
    WriteIndented = true,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
};

try
{
    if (request.ValorD is not null)
    {
        var d = ValidationService.ValidateD(request.ValorD);
        Console.Out.WriteLine(JsonSerializer.Serialize(new { valorD = d }, outputOptions));
        return;
    }

    var metodo = string.IsNullOrWhiteSpace(request.Metodo) ? "dp" : request.Metodo;
    var response = LcsDpService.Calculate(request.Helena, request.Marcus, metodo);
    Console.Out.WriteLine(JsonSerializer.Serialize(response, outputOptions));
}
catch (ValidationException ex)
{
    var error = new LcsErrorResponse
    {
        Erro = ex.Message,
    };
    Console.Out.WriteLine(JsonSerializer.Serialize(error, outputOptions));
    Environment.Exit(2);
}
