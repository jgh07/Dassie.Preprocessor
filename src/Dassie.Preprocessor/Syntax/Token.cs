namespace Dassie.Preprocessor.Syntax;

internal record struct Token(TokenKind Kind, (int Line, int Column) Start, string Value);