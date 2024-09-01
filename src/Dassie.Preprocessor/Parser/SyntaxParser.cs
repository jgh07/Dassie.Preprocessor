using Dassie.Preprocessor.Syntax;
using System.Collections.Generic;

namespace Dassie.Preprocessor.Parser;

#pragma warning disable IDE0305

// Not much here since the Lexer has done all the heavy lifting
internal class SyntaxParser
{
    /*
     * file := fragment* eof
     * fragment := default | expression | section
     * default := <Default>
     * expression := <Dollar> <OpenBrace> <DassieExpression> <CloseBrace> 
     * section := <Hash> <OpenBrace> <DassieDirectives> <CloseBrace>
     */

    private readonly Lexer _lexer;

    public SyntaxParser(Lexer lexer)
    {
        _lexer = lexer;
        _lexer.Tokenize();
    }

    public CompilationUnit Parse()
    {
        List<TextFragment> fragments = [];

        while (_lexer.Peek().Kind != TokenKind.Eof)
        {
            Token tok = _lexer.Next();

            switch (tok.Kind)
            {
                case TokenKind.Default:
                    fragments.Add(new LiteralText(tok.Start, tok.Value));
                    break;
                case TokenKind.Hash:
                    string name = _lexer.Next().Value;
                    _lexer.Next();
                    fragments.Add(new Section(tok.Start, name, _lexer.Next().Value));
                    _lexer.Next();
                    break;
                case TokenKind.Dollar:
                    _lexer.Next();
                    fragments.Add(new Expression(tok.Start, _lexer.Next().Value));
                    _lexer.Next();
                    break;
            }
        }

        return new(fragments.ToArray());
    }
}