global using static Dassie.Preprocessor.Errors.ErrorWriter;
using System;

namespace Dassie.Preprocessor.Errors;

internal static class ErrorWriter
{
    public static void EmitError(string message, (int Line, int Column) pos = default, string file = null)
    {
        if (!string.IsNullOrEmpty(file))
            Console.Error.Write($"{file}");

        if (pos != default)
            Console.Error.Write($"({pos.Line},{pos.Column}): ");

        Console.WriteLine($"Error: {message}");
    }
}