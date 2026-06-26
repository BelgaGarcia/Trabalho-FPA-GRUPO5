/*
 * Trabalho pratico - Fundamentos de Projeto e Analise de Algoritmos
 * Tema: Maior Subsequencia Comum
 * Autores: preencher com os nomes do grupo
 * Versao: 2.0
 * Data: 26/06/2026
 *
 * Esta versao resolve o problema completo:
 * - monta a tabela de programacao dinamica;
 * - calcula o comprimento da LCS;
 * - usa backtracking para listar todas as LCS sem repeticao e em ordem alfabetica.
 */

using System.Text.Json;
using FpaaLcs.Core.Models;
using FpaaLcs.Core.Services;

namespace FpaaLcs.Core;

public class Program
{
    public static void Main()
    {
        if (!Console.IsInputRedirected)
        {
            ProcessarEntradaInterativa();
            return;
        }

        string entradaCompleta = Console.In.ReadToEnd();

        if (string.IsNullOrWhiteSpace(entradaCompleta))
        {
            Console.WriteLine("{\"status\":\"aguardando_entrada\"}");
            return;
        }

        if (entradaCompleta.TrimStart().StartsWith("{"))
        {
            ProcessarEntradaJson(entradaCompleta);
            return;
        }

        ProcessarEntradaDoRoteiro(entradaCompleta);
    }

    private static void ProcessarEntradaInterativa()
    {
        try
        {
            ExibirDescricaoEntrada();

            string? primeiraLinha = Console.ReadLine();
            if (!int.TryParse(primeiraLinha, out int quantidadeConjuntos))
            {
                throw new ArgumentException("a primeira linha deve informar a quantidade de conjuntos.");
            }

            if (quantidadeConjuntos < 1 || quantidadeConjuntos > 10)
            {
                throw new ArgumentException("a quantidade de conjuntos deve ficar entre 1 e 10.");
            }

            for (int numeroConjunto = 1; numeroConjunto <= quantidadeConjuntos; numeroConjunto++)
            {
                Console.WriteLine();
                Console.WriteLine($"Conjunto {numeroConjunto}");
                Console.Write("Sequencia de Helena: ");
                string sequenciaHelena = (Console.ReadLine() ?? string.Empty).Trim();

                Console.Write("Sequencia de Marcus: ");
                string sequenciaMarcus = (Console.ReadLine() ?? string.Empty).Trim();

                ResultadoMaiorSubsequenciaComum resultado = CalcularComValidacao(sequenciaHelena, sequenciaMarcus);

                Console.WriteLine($"Comprimento da LCS: {resultado.ComprimentoMaiorSubsequenciaComum}");
                Console.WriteLine("Subsequencias comuns mais longas:");
                foreach (string subsequencia in resultado.SubsequenciasComunsMaisLongas)
                {
                    Console.WriteLine(subsequencia);
                }
            }
        }
        catch (ArgumentException erroEntrada)
        {
            Console.WriteLine($"Erro na entrada: {erroEntrada.Message}");
        }
    }

    private static void ExibirDescricaoEntrada()
    {
        Console.WriteLine("Maior subsequencia comum (LCS)");
        Console.WriteLine();
        Console.WriteLine("Como preencher a entrada:");
        Console.WriteLine("1. Digite a quantidade de conjuntos de dados, de 1 ate 10.");
        Console.WriteLine("2. Para cada conjunto, digite uma linha com a sequencia de Helena.");
        Console.WriteLine("3. Em seguida, digite uma linha com a sequencia de Marcus.");
        Console.WriteLine("4. Use apenas letras minusculas de a ate z, com ate 80 caracteres por sequencia.");
        Console.WriteLine();
        Console.WriteLine("Exemplo:");
        Console.WriteLine("2");
        Console.WriteLine("abcbdab");
        Console.WriteLine("bdcaba");
        Console.WriteLine("abc");
        Console.WriteLine("def");
        Console.WriteLine();
        Console.Write("Quantidade de conjuntos: ");
    }

    private static void ProcessarEntradaJson(string entradaCompleta)
    {
        try
        {
            JsonSerializerOptions opcoesJson = new()
            {
                PropertyNameCaseInsensitive = true,
            };

            AppRequest? requisicao = JsonSerializer.Deserialize<AppRequest>(entradaCompleta, opcoesJson);
            string sequenciaHelena = (requisicao?.Helena ?? string.Empty).Trim();
            string sequenciaMarcus = (requisicao?.Marcus ?? string.Empty).Trim();

            ResultadoMaiorSubsequenciaComum resultado = CalcularComValidacao(sequenciaHelena, sequenciaMarcus);

            Console.WriteLine(
                JsonSerializer.Serialize(
                    new
                    {
                        status = "ok",
                        metodo = "programacao_dinamica_com_backtracking",
                        helena = resultado.SequenciaHelena,
                        marcus = resultado.SequenciaMarcus,
                        comprimento_maximo = resultado.ComprimentoMaiorSubsequenciaComum,
                        quantidade = resultado.SubsequenciasComunsMaisLongas.Count,
                        subsequencias = resultado.SubsequenciasComunsMaisLongas,
                        tabela = CalculadoraMaiorSubsequenciaComum.ConverterTabelaParaMatrizSimples(
                            resultado.TabelaProgramacaoDinamica,
                            resultado.SequenciaHelena.Length,
                            resultado.SequenciaMarcus.Length),
                    }));
        }
        catch (Exception erro) when (erro is ArgumentException or JsonException)
        {
            Console.WriteLine(
                JsonSerializer.Serialize(
                    new
                    {
                        status = "erro",
                        motivo = erro.Message,
                    }));
        }
    }

    private static void ProcessarEntradaDoRoteiro(string entradaCompleta)
    {
        try
        {
            using StringReader leitor = new(entradaCompleta);
            string? primeiraLinha = leitor.ReadLine();

            if (!int.TryParse(primeiraLinha, out int quantidadeConjuntos))
            {
                throw new ArgumentException("a primeira linha deve informar a quantidade de conjuntos.");
            }

            if (quantidadeConjuntos < 1 || quantidadeConjuntos > 10)
            {
                throw new ArgumentException("a quantidade de conjuntos deve ficar entre 1 e 10.");
            }

            for (int numeroConjunto = 1; numeroConjunto <= quantidadeConjuntos; numeroConjunto++)
            {
                string sequenciaHelena = (leitor.ReadLine() ?? string.Empty).Trim();
                string sequenciaMarcus = (leitor.ReadLine() ?? string.Empty).Trim();

                ResultadoMaiorSubsequenciaComum resultado = CalcularComValidacao(sequenciaHelena, sequenciaMarcus);

                foreach (string subsequencia in resultado.SubsequenciasComunsMaisLongas)
                {
                    Console.WriteLine(subsequencia);
                }

                if (numeroConjunto < quantidadeConjuntos)
                {
                    Console.WriteLine();
                }
            }
        }
        catch (ArgumentException erroEntrada)
        {
            Console.WriteLine($"Erro na entrada: {erroEntrada.Message}");
        }
    }

    private static ResultadoMaiorSubsequenciaComum CalcularComValidacao(
        string sequenciaHelena,
        string sequenciaMarcus)
    {
        ValidadorSequencia.Validar(sequenciaHelena, "Helena");
        ValidadorSequencia.Validar(sequenciaMarcus, "Marcus");

        return CalculadoraMaiorSubsequenciaComum.Calcular(sequenciaHelena, sequenciaMarcus);
    }
}
