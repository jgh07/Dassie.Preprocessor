global using static Dassie.Preprocessor.Errors.ErrorWriter;
using DSErrorWriter = Dassie.Errors.ErrorWriter;
using System;
using Dassie.Errors;

namespace Dassie.Preprocessor.Errors;

internal static class ErrorWriter
{
    public static void EmitError(string message, (int Line, int Column) pos = default, string file = null)
    {
        DSErrorWriter.EmitErrorMessage(new ErrorInfo()
        {
            CodePosition = pos,
            ErrorMessage = message,
            ErrorCode = ErrorKind.CustomError,
            CustomErrorCode = "DSTT01",
            Severity = Severity.Error,
            File = file,
            Length = 1
        });
    }
}