using System.Text;
using Lab_6.Domain;

namespace Lab_6.Lexer;

/// <summary>
///     Tokenizes AION source code into a stream of typed tokens.
/// </summary>
public class Lexer
{
    private readonly SourceReader _reader;
    private readonly List<Token> _tokens = new();

    public Lexer(string source)
    {
        _reader = new SourceReader(source);
    }

    /// <summary>
    ///     Entry point for lexical analysis. Converts the source into a token list.
    /// </summary>
    public List<Token> Tokenize()
    {
        while (!_reader.IsAtEnd)
        {
            SkipWhitespace();
            int col = _reader.Column;
            char current = _reader.Peek();

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

        _tokens.Add(new Token(TokenType.EndOfFile, "", _reader.Line, _reader.Column));
        return _tokens;
    }

    private void SkipWhitespace()
    {
        while (!_reader.IsAtEnd)
        {
            char c = _reader.Peek();

            if (c == ' ' || c == '\t' || c == '\r')
                _reader.Advance();
            else if (c == '\n')
                _reader.Advance();
            else if (c == '/' && _reader.PeekNext() == '/')
                SkipLineComment();
            else if (c == '/' && _reader.PeekNext() == '*')
                SkipBlockComment();
            else
                break;
        }
    }

    private void SkipLineComment()
    {
        while (!_reader.IsAtEnd && _reader.Peek() != '\n')
            _reader.Advance();
    }

    private void SkipBlockComment()
    {
        _reader.Advance(); // /
        _reader.Advance(); // *

        while (!_reader.IsAtEnd)
        {
            if (_reader.Peek() == '*' && _reader.PeekNext() == '/')
            {
                _reader.Advance(); // *
                _reader.Advance(); // /
                break;
            }

            _reader.Advance();
        }
    }

    private Token ReadIdentifierOrKeyword()
    {
        int startCol = _reader.Column;
        int startPos = _reader.Position;

        while (!_reader.IsAtEnd && CharUtils.IsIdentifierPart(_reader.Peek()))
            _reader.Advance();

        string lexeme = _reader.Substring(startPos, _reader.Position - startPos);
        string lower = lexeme.ToLower();

        TokenType type = Keywords.Map.TryGetValue(lower, out var keywordType)
            ? keywordType
            : TokenType.Identifier;

        return new Token(type, lexeme, _reader.Line, startCol);
    }

    private Token ReadNumberOrDuration()
    {
        int startCol = _reader.Column;
        int startPos = _reader.Position;

        while (!_reader.IsAtEnd && (char.IsDigit(_reader.Peek()) || _reader.Peek() == '.'))
            _reader.Advance();

        string number = _reader.Substring(startPos, _reader.Position - startPos);

        // Extra regex validation
        if (!System.Text.RegularExpressions.Regex.IsMatch(number, @"^\d+(\.\d+)?$"))
        {
            return new Token(TokenType.Unknown, number, _reader.Line, startCol);
        }

        if (!_reader.IsAtEnd && (_reader.Peek() == 'h' || _reader.Peek() == 'm'))
        {
            char unit = _reader.Advance();
            return new Token(TokenType.Duration, number + unit, _reader.Line, startCol);
        }

        return new Token(TokenType.Number, number, _reader.Line, startCol);
    }


    private Token ReadString()
    {
        int col = _reader.Column;
        _reader.Advance(); // skip opening "

        StringBuilder sb = new();

        while (!_reader.IsAtEnd && _reader.Peek() != '"')
        {
            sb.Append(_reader.Advance());
        }

        if (_reader.IsAtEnd)
            return new Token(TokenType.Unknown, sb.ToString(), _reader.Line, col);

        _reader.Advance(); // skip closing "
        return new Token(TokenType.String, sb.ToString(), _reader.Line, col);
    }

    private Token? ReadSymbol()
    {
        if (_reader.IsAtEnd)
            return null;

        char peek = _reader.Peek();
        if (peek == '\0')
        {
            _reader.Advance(); // consume it so we move forward
            return null; // skip meaningless null token
        }

        int col = _reader.Column;
        char current = _reader.Advance();

        switch (current)
        {
            case '=':
                return _reader.Match('=')
                    ? new Token(TokenType.Equal, "==", _reader.Line, col)
                    : new Token(TokenType.Assign, "=", _reader.Line, col);
            case '!':
                return _reader.Match('=')
                    ? new Token(TokenType.NotEqual, "!=", _reader.Line, col)
                    : new Token(TokenType.Unknown, "!", _reader.Line, col);
            case '<':
                return _reader.Match('=')
                    ? new Token(TokenType.LessEqual, "<=", _reader.Line, col)
                    : new Token(TokenType.Less, "<", _reader.Line, col);
            case '>':
                return _reader.Match('=')
                    ? new Token(TokenType.GreaterEqual, ">=", _reader.Line, col)
                    : new Token(TokenType.Greater, ">", _reader.Line, col);
            case '{': return new Token(TokenType.LeftBrace, "{", _reader.Line, col);
            case '}': return new Token(TokenType.RightBrace, "}", _reader.Line, col);
            case '(': return new Token(TokenType.LeftParen, "(", _reader.Line, col);
            case ')': return new Token(TokenType.RightParen, ")", _reader.Line, col);
            case ',': return new Token(TokenType.Comma, ",", _reader.Line, col);
            case ';': return new Token(TokenType.Semicolon, ";", _reader.Line, col);
            case ':': return new Token(TokenType.Colon, ":", _reader.Line, col);
            case '.': return new Token(TokenType.Dot, ".", _reader.Line, col);
        }

        return new Token(TokenType.Unknown, current.ToString(), _reader.Line, col);
    }
}