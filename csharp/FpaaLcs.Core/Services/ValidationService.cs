using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FpaaLcs.Core.Exceptions;

namespace FpaaLcs.Core.Services;

public static class ValidationService
{
    public const int MaxCasos = 10;
    public const int MinLen = 1;
    public const int MaxLen = 80;

    public static (string Helena, string Marcus) ValidateCase(string helena, string marcus)
    {
        helena = (helena ?? string.Empty).Trim();
        marcus = (marcus ?? string.Empty).Trim();

        ValidateSequence("Helena", helena);
        ValidateSequence("Marcus", marcus);

        return (helena, marcus);
    }

    public static int ValidateD(string valor)
    {
        valor = (valor ?? string.Empty).Trim();

        if (string.IsNullOrEmpty(valor) || !valor.All(char.IsDigit))
        {
            throw new ValidationException($"D deve ser um inteiro positivo; recebido: '{valor}'.");
        }

        var d = int.Parse(valor);

        if (d < 1)
        {
            throw new ValidationException($"D deve ser pelo menos 1; recebido: {d}.");
        }

        if (d > MaxCasos)
        {
            throw new ValidationException($"D não pode exceder {MaxCasos}; recebido: {d}.");
        }

        return d;
    }

    private static void ValidateSequence(string nome, string sequencia)
    {
        if (string.IsNullOrEmpty(sequencia))
        {
            throw new ValidationException($"{nome}: sequência vazia (mínimo {MinLen} caractere).");
        }

        if (sequencia.Length > MaxLen)
        {
            throw new ValidationException(
                $"{nome}: sequência com {sequencia.Length} caracteres (máximo {MaxLen})."
            );
        }

        for (var pos = 0; pos < sequencia.Length; pos++)
        {
            var ch = sequencia[pos];
            if (ch < 'a' || ch > 'z')
            {
                throw new ValidationException(
                    $"{nome}: caractere inválido '{ch}' na posição {pos + 1} "
                    + "(use apenas letras minúsculas de a a z)."
                );
            }
        }
    }
}
