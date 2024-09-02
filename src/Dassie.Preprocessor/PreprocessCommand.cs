using Dassie.Extensions;
using System;
using System.IO;
using System.Linq;

namespace Dassie.Preprocessor;

public class PreprocessCommand : ICompilerCommand
{
    public string Command { get; } = "pp";

    public string UsageString { get; } = "pp <InputFile> [OutputFile]";

    public string Description { get; } = "Preprocesses the specified file.";

    public int Invoke(string[] args)
    {
        static void PrintUsage()
        {
            Console.WriteLine("Dassie Preprocessor");
            Console.WriteLine("Version 1.0, (C) 2024 Jonas H.");
            Console.WriteLine();

            Console.WriteLine("Usage: pp <InputFile> [OutputFile]");
        }

        if (args == null || args.Length == 0 || ((string[])["help", "?", "--help", "-h", "-?", "/?", "/help", "/h"]).Contains(args[0]))
        {
            PrintUsage();
            return 0;
        }

        string inFile = Path.GetFullPath(args[0]);
        string outFile = Path.Combine(Path.GetDirectoryName(inFile), $"{Path.GetFileNameWithoutExtension(inFile)}.out{Path.GetExtension(inFile)}");

        if (args.Length > 1)
            outFile = Path.GetFullPath(args[1]);

        Preprocessor.Preprocess(inFile, outFile);
        return 0;
    }
}