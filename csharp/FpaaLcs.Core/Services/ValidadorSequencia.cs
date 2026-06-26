namespace FpaaLcs.Core.Services;

public static class ValidadorSequencia
{
    public static void Validar(string sequenciaEventos, string nomeAnalista)
    {
        if (sequenciaEventos.Length < 1 || sequenciaEventos.Length > 80)
        {
            throw new ArgumentException($"a sequencia de {nomeAnalista} deve ter entre 1 e 80 caracteres.");
        }

        foreach (char evento in sequenciaEventos)
        {
            if (evento < 'a' || evento > 'z')
            {
                throw new ArgumentException(
                    $"a sequencia de {nomeAnalista} deve conter apenas letras minusculas de a ate z.");
            }
        }
    }
}
