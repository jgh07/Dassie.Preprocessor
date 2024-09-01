using Dassie.CodeGeneration;
using Dassie.Preprocessor.Generator;
using Dassie.Preprocessor.Parser;
using Dassie.Preprocessor.Syntax;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Dassie.Preprocessor;

public static class Preprocessor
{
    public static void Preprocess(string inFile, string outFile)
    {
        string name = Path.GetFileName(inFile);
        string source = File.ReadAllText(inFile);

        Lexer lexer = new(source, name);
        SyntaxParser parser = new(lexer);
        CompilationUnit cu = parser.Parse();

        string tempPath = Directory.CreateDirectory(".temp").FullName;
        string srcFile = Path.Combine(tempPath, "generator.ds");
        File.WriteAllText(srcFile, ProgramGenerator.Generate(cu, inFile, outFile));

        string workingDir = Directory.GetCurrentDirectory();
        Directory.SetCurrentDirectory(tempPath);

        Type cliHelpersType = Assembly.GetAssembly(typeof(Compiler)).GetType("Dassie.CLI.CliHelpers");
        cliHelpersType.GetMethod("HandleArgs").Invoke(null, [(string[])[srcFile], null]);

        string outputAssembly = Path.Combine(tempPath, "build", "generator.dll");

        ProcessStartInfo psi = new()
        {
            FileName = "dotnet",
            Arguments = outputAssembly
        };

        Process.Start(psi).WaitForExit();
        Directory.SetCurrentDirectory(workingDir);
        Directory.Delete(tempPath, true);
    }
}