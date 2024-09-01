using Dassie.Preprocessor.Syntax;
using System;
using System.Linq;
using System.Text;

namespace Dassie.Preprocessor.Generator;

internal static class ProgramGenerator
{
    public static string Generate(CompilationUnit unit, string inPath, string outPath)
    {
        StringBuilder memberSectionBuilder = new();
        StringBuilder sb = new();

        foreach (Section sec in unit.Fragments.Where(f => f is Section).Select(f => (Section)f))
        {
            if (sec.Name == "head")
                sb.AppendLine(sec.Content);

            else if (sec.Name == "members")
                memberSectionBuilder.AppendLine(sec.Content);

            else
                EmitError($"Invalid section '{sec.Name}'.", sec.Start, inPath);
        }

        int exprCount = 0;

        sb.AppendLine("import System.IO");

        sb.AppendLine("module Generator = {");
        sb.AppendLine(memberSectionBuilder.ToString());

        sb.AppendLine("<EntryPoint>");
        sb.AppendLine("Main (): int32 = {");
        sb.AppendLine($"sw = StreamWriter \"{outPath}\"");

        foreach (var fragment in unit.Fragments)
        {
            if (fragment is LiteralText lt)
                sb.AppendLine($"sw.Write ^\"{lt.Text.Replace("\"", "\"\"")}\"");

            else if (fragment is Expression e)
            {
                sb.AppendLine($"expression{exprCount} = {{{e.Text}}}");
                sb.AppendLine($"sw.Write expression{exprCount}.ToString");
                exprCount++;
            }
        }

        sb.AppendLine("sw.Dispose");
        sb.AppendLine("0");
        sb.AppendLine("}");
        sb.AppendLine("}");
        return sb.ToString();
    }
}