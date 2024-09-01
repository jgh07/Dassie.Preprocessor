namespace Dassie.Preprocessor.Syntax;

internal enum TokenKind
{
    Dollar,
    Hash,
    Identifier,
    OpenBrace,
    CloseBrace,
    Default,
    DassieExpression,
    DassieDirectives,
    Eof
}