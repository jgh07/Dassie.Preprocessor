using Dassie.Preprocessor.Parser.Helpers;
using Dassie.Preprocessor.Syntax;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Dassie.Preprocessor.Parser;

// This is not actually a lexer, but a lexer and half a parser...
internal class Lexer
{
    private int _strIndex = 0;
    private readonly string _fileName;
    private readonly StringReader _sr;

    private int _tokenIndex = 0;
    private readonly List<Token> _tokens = [];
    private readonly CodePositionHelper _cpHelper;

    public Lexer(string source, string fileName)
    {
        _fileName = fileName;
        _sr = new(source);
        _cpHelper = new(source);
        Tokenize();
    }

    public Token Peek()
    {
        return _tokens[_tokenIndex];
    }

    public Token Next()
    {
        return _tokens[_tokenIndex++];
    }

    private char Advance()
    {
        _strIndex++;
        return (char)_sr.Read();
    }

    public void Tokenize()
    {
        while (_sr.Peek() != -1)
        {
            char c = Advance();

            if (c == '#')
            {
                if ((char)_sr.Peek() == '#')
                {
                    Advance();
                    _tokens.Add(new(TokenKind.Default, _cpHelper.FromIndex(_strIndex - 1), "#"));
                    continue;
                }

                TokenizeSection();
                continue;
            }

            if (c == '$')
            {
                if ((char)_sr.Peek() == '$')
                {
                    Advance();
                    _tokens.Add(new(TokenKind.Default, _cpHelper.FromIndex(_strIndex - 1), "$"));
                    continue;
                }

                TokenizeExpression();
                continue;
            }

            StringBuilder sb = new();
            sb.Append(c);

            while (_sr.Peek() != -1 && !((char[])['$', '#']).Contains((char)_sr.Peek()))
            {
                c = Advance();
                sb.Append(c);
            }

            _tokens.Add(new(TokenKind.Default, _cpHelper.FromIndex(_strIndex - sb.Length), sb.ToString()));
        }

        var (line, col) = _cpHelper.FromIndex(_strIndex - 1);
        _tokens.Add(new(TokenKind.Eof, (line, col + 1), ""));
    }

    private void TokenizeBlock(TokenKind contentKind)
    {
        _tokens.Add(new(TokenKind.OpenBrace, _cpHelper.FromIndex(_strIndex - 1), "{"));

        bool closed = false;
        StringBuilder contentBuilder = new();

        while (_sr.Peek() != -1)
        {
            char c = Advance();

            if (c == '}')
            {
                if (_sr.Peek() == '}')
                {
                    Advance();
                    contentBuilder.Append('}');
                    continue;
                }

                closed = true;
                break;
            }

            contentBuilder.Append(c);
        }

        _tokens.Add(new(contentKind, _cpHelper.FromIndex(_strIndex - (contentBuilder.Length + 1)), contentBuilder.ToString()));
        _tokens.Add(new(TokenKind.CloseBrace, _cpHelper.FromIndex(_strIndex - 1), "}"));

        if (!closed)
            EmitError("Missing closing brace for block.", _cpHelper.FromIndex(_strIndex - 1), _fileName);

        while (_sr.Peek() == '\r' || _sr.Peek() == '\n')
            Advance();
    }

    private void TokenizeSection()
    {
        _tokens.Add(new(TokenKind.Hash, _cpHelper.FromIndex(_strIndex - 1), "#"));
        bool hasBlock = false;
        StringBuilder identifierBuilder = new();
        int wsCount = 0;

        while (_sr.Peek() != -1)
        {
            char c = Advance();

            if (c == '{')
            {
                if (identifierBuilder.Length == 0)
                    EmitError("Expected identifier.", _cpHelper.FromIndex(_strIndex - 1), _fileName);

                hasBlock = true;
                break;
            }

            if (char.IsWhiteSpace(c))
            {
                wsCount++;
                continue;
            }

            if (char.IsDigit(c))
            {
                if (identifierBuilder.Length == 0)
                    EmitError("Identifier cannot start with a digit, only letters and underscores are allowed.", _cpHelper.FromIndex(_strIndex - 1), _fileName);

                identifierBuilder.Append(c);
                continue;
            }

            if (char.IsLetter(c) || c == '_')
            {
                identifierBuilder.Append(c);
                continue;
            }

            EmitError($"Invalid character '{c}' in identifier name, only letters, digits and underscores are allowed.", _cpHelper.FromIndex(_strIndex - 1), _fileName);
            identifierBuilder.Append(c);
        }

        _tokens.Add(new(TokenKind.Identifier, _cpHelper.FromIndex(_strIndex - (identifierBuilder.Length + wsCount + 1)), identifierBuilder.ToString()));
        TokenizeBlock(TokenKind.DassieDirectives);

        if (!hasBlock)
            EmitError("Expected '{'.", _cpHelper.FromIndex(_strIndex - 1), _fileName);
    }

    private void TokenizeExpression()
    {
        _tokens.Add(new(TokenKind.Dollar, _cpHelper.FromIndex(_strIndex - 1), "$"));

        char brace = Advance();
        int wsCount = 0;

        while (char.IsWhiteSpace(brace))
        {
            wsCount++;
            brace = Advance();
        }

        if (brace != '{')
        {
            EmitError($"Unexpected token '{brace}'. Expected '{{'.", _cpHelper.FromIndex(_strIndex - 1), _fileName);
            return;
        }

        TokenizeBlock(TokenKind.DassieExpression);
    }
}