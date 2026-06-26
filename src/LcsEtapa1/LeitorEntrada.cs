namespace LcsEtapa1;

public static class LeitorEntrada
{
    public static int LerQuantidadeConjuntos()
    {
        string? linhaQuantidade = Console.ReadLine();

        if (!int.TryParse(linhaQuantidade, out int quantidadeConjuntos))
        {
            throw new ArgumentException("a primeira linha precisa ser um numero inteiro.");
        }

        if (quantidadeConjuntos < 1 || quantidadeConjuntos > 10)
        {
            throw new ArgumentException("a quantidade de conjuntos deve ficar entre 1 e 10.");
        }

        return quantidadeConjuntos;
    }

    public static string LerSequencia(string nomeAnalista, int numeroConjunto)
    {
        string? sequenciaLida = Console.ReadLine();

        if (sequenciaLida is null)
        {
            throw new ArgumentException($"faltou informar a sequencia de {nomeAnalista} no conjunto {numeroConjunto}.");
        }

        string sequenciaEventos = sequenciaLida.Trim();
        ValidarSequencia(sequenciaEventos, nomeAnalista, numeroConjunto);

        return sequenciaEventos;
    }

    private static void ValidarSequencia(string sequenciaEventos, string nomeAnalista, int numeroConjunto)
    {
        if (sequenciaEventos.Length < 1 || sequenciaEventos.Length > 80)
        {
            throw new ArgumentException(
                $"a sequencia de {nomeAnalista} no conjunto {numeroConjunto} deve ter entre 1 e 80 caracteres.");
        }

        foreach (char evento in sequenciaEventos)
        {
            if (evento < 'a' || evento > 'z')
            {
                throw new ArgumentException(
                    $"a sequencia de {nomeAnalista} no conjunto {numeroConjunto} deve conter apenas letras minusculas de a ate z.");
            }
        }
    }
}
