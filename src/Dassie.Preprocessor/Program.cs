using Dassie.Preprocessor;
using System;
using System.Linq;

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
    return;
}

PreprocessCommand cmd = new();
cmd.Invoke(args);