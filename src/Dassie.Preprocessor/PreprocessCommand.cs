using Dassie.Extensions;
using System;
using System.IO;

namespace Dassie.Preprocessor;

public class PreprocessCommand : ICompilerCommand
{
    public string Command { get; } = "pp";

    public string UsageString { get; } = "pp <InputFile> [OutputFile]";

    public string Description { get; } = "Preprocesses the specified file.";

    public int Invoke(string[] args)
    {
        if (args == null || args.Length == 0)
        {
            Console.WriteLine("Error: No input file specified.");
            return -1;
        }

        string inFile = Path.GetFullPath(args[0]);
        string outFile = Path.Combine(Path.GetDirectoryName(inFile), $"{Path.GetFileNameWithoutExtension(inFile)}.out{Path.GetExtension(inFile)}");

        if (args.Length > 1)
            outFile = args[1];

        Preprocessor.Preprocess(inFile, outFile);
        return 0;
    }
}