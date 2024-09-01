namespace Dassie.Preprocessor.Syntax;

internal class CompilationUnit(TextFragment[] fragments)
{
    public TextFragment[] Fragments { get; } = fragments;
}