namespace Dassie.Preprocessor.Syntax;

internal abstract record TextFragment() { }

internal record Section((int Line, int Column) Start, string Name, string Content) : TextFragment;
internal record Expression((int Line, int Column) Start, string Text) : TextFragment;
internal record LiteralText((int Line, int Column) Start, string Text) : TextFragment;