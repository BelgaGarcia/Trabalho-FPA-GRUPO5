namespace LcsEtapa1;

public class Program
{
    public static void Main()
    {
        try
        {
            bool entradaManual = !Console.IsInputRedirected;

            if (entradaManual)
            {
                ExibirDescricaoEntrada();
            }

            int quantidadeConjuntos = LeitorEntrada.LerQuantidadeConjuntos();

            for (int numeroConjunto = 1; numeroConjunto <= quantidadeConjuntos; numeroConjunto++)
            {
                if (entradaManual)
                {
                    Console.WriteLine();
                    Console.WriteLine($"Conjunto {numeroConjunto}");
                    Console.Write("Sequencia de Helena: ");
                }

                string sequenciaHelena = LeitorEntrada.LerSequencia("Helena", numeroConjunto);

                if (entradaManual)
                {
                    Console.Write("Sequencia de Marcus: ");
                }

                string sequenciaMarcus = LeitorEntrada.LerSequencia("Marcus", numeroConjunto);

                ResultadoMaiorSubsequenciaComum resultado =
                    CalculadoraMaiorSubsequenciaComum.Calcular(sequenciaHelena, sequenciaMarcus);

                if (entradaManual)
                {
                    Console.WriteLine($"Comprimento da LCS: {resultado.ComprimentoMaiorSubsequenciaComum}");
                    Console.WriteLine("Subsequencias comuns mais longas:");
                }

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
}
