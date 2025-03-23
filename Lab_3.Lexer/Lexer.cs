using System.Text;
using Lab_3.Domain;

namespace Lab_3.Lexer;

/// <summary>
///     Tokenizes AION source code into a stream of typed tokens.
/// </summary>
public class Lexer
{
    private readonly SourceReader reader;
    private readonly List<Token> _tokens = new();

    public Lexer(string source)
    {
        reader = new SourceReader(source);
    }

    /// <summary>
    ///     Entry point for lexical analysis. Converts the source into a token list.
    /// </summary>
    public List<Token> Tokenize()
    {
        while (!reader.IsAtEnd)
        {
            SkipWhitespace();
            int col = reader.Column;
            char current = reader.Peek();

            if (char.IsLetter(current) || current == '_')
                _tokens.Add(ReadIdentifierOrKeyword());
            else if (char.IsDigit(current))
                _tokens.Add(ReadNumberOrDuration());
            else if (current == '"')
                _tokens.Add(ReadString());
            else
            {
                var token = ReadSymbol();
                if (token != null)
                    _tokens.Add(token);
            }
        }

        _tokens.Add(new Token(TokenType.EndOfFile, "", reader.Line, reader.Column));
        return _tokens;
    }

    private void SkipWhitespace()
    {
        while (!reader.IsAtEnd)
        {
            char c = reader.Peek();

            if (c == ' ' || c == '\t' || c == '\r')
                reader.Advance();
            else if (c == '\n')
                reader.Advance();
            else if (c == '/' && reader.PeekNext() == '/')
                SkipLineComment();
            else if (c == '/' && reader.PeekNext() == '*')
                SkipBlockComment();
            else
                break;
        }
    }

    private void SkipLineComment()
    {
        while (!reader.IsAtEnd && reader.Peek() != '\n')
            reader.Advance();
    }

    private void SkipBlockComment()
    {
        reader.Advance(); // /
        reader.Advance(); // *

        while (!reader.IsAtEnd)
        {
            if (reader.Peek() == '*' && reader.PeekNext() == '/')
            {
                reader.Advance(); // *
                reader.Advance(); // /
                break;
            }

            reader.Advance();
        }
    }

    private Token ReadIdentifierOrKeyword()
    {
        int startCol = reader.Column;
        int startPos = reader.Position;

        while (!reader.IsAtEnd && (char.IsLetterOrDigit(reader.Peek()) || reader.Peek() == '_'))
            reader.Advance();

        string lexeme = reader.Substring(startPos, reader.Position - startPos);
        TokenType type = Keywords.All.Contains(lexeme) ? TokenType.Keyword : TokenType.Identifier;

        return new Token(type, lexeme, reader.Line, startCol);
    }

    private Token ReadNumberOrDuration()
    {
        int startCol = reader.Column;
        int startPos = reader.Position;
        bool hasDot = false;

        while (!reader.IsAtEnd && (char.IsDigit(reader.Peek()) || reader.Peek() == '.'))
        {
            if (reader.Peek() == '.')
            {
                if (hasDot) break;
                hasDot = true;
            }

            reader.Advance();
        }

        string number = reader.Substring(startPos, reader.Position - startPos);

        if (!reader.IsAtEnd && (reader.Peek() == 'h' || reader.Peek() == 'm'))
        {
            char unit = reader.Advance();
            return new Token(TokenType.Duration, number + unit, reader.Line, startCol);
        }

        return new Token(TokenType.Number, number, reader.Line, startCol);
    }

    private Token ReadString()
    {
        int col = reader.Column;
        reader.Advance(); // skip opening "

        StringBuilder sb = new();

        while (!reader.IsAtEnd && reader.Peek() != '"')
        {
            sb.Append(reader.Advance());
        }

        if (reader.IsAtEnd)
            return new Token(TokenType.Unknown, sb.ToString(), reader.Line, col);

        reader.Advance(); // skip closing "
        return new Token(TokenType.String, sb.ToString(), reader.Line, col);
    }

    private Token? ReadSymbol()
    {
        if (reader.IsAtEnd)
            return null;

        char peek = reader.Peek();
        if (peek == '\0')
        {
            reader.Advance(); // consume it so we move forward
            return null; // skip meaningless null token
        }

        int col = reader.Column;
        char current = reader.Advance();

        switch (current)
        {
            case '=':
                return reader.Match('=')
                    ? new Token(TokenType.Equal, "==", reader.Line, col)
                    : new Token(TokenType.Assign, "=", reader.Line, col);
            case '!':
                return reader.Match('=')
                    ? new Token(TokenType.NotEqual, "!=", reader.Line, col)
                    : new Token(TokenType.Unknown, "!", reader.Line, col);
            case '<':
                return reader.Match('=')
                    ? new Token(TokenType.LessEqual, "<=", reader.Line, col)
                    : new Token(TokenType.Less, "<", reader.Line, col);
            case '>':
                return reader.Match('=')
                    ? new Token(TokenType.GreaterEqual, ">=", reader.Line, col)
                    : new Token(TokenType.Greater, ">", reader.Line, col);
            case '{': return new Token(TokenType.LeftBrace, "{", reader.Line, col);
            case '}': return new Token(TokenType.RightBrace, "}", reader.Line, col);
            case '(': return new Token(TokenType.LeftParen, "(", reader.Line, col);
            case ')': return new Token(TokenType.RightParen, ")", reader.Line, col);
            case ',': return new Token(TokenType.Comma, ",", reader.Line, col);
            case ';': return new Token(TokenType.Semicolon, ";", reader.Line, col);
            case ':': return new Token(TokenType.Colon, ":", reader.Line, col);
            case '.': return new Token(TokenType.Dot, ".", reader.Line, col);
        }

        return new Token(TokenType.Unknown, current.ToString(), reader.Line, col);
    }
}