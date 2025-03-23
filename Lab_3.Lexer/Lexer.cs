using System.Text;

namespace Lab_3.Lexer;

public class Lexer
{
    private readonly string _source;
    private int _position = 0;
    private int _line = 1;
    private int _column = 1;

    private readonly List<Token> _tokens = new();

    private static readonly HashSet<string> Keywords = new()
    {
        "import", "as", "event", "task", "pomodoro", "new", "on", "from", "to",
        "at", "each", "with", "alarm", "repeat", "times", "break", "weeknumber",
        "if", "else", "else if", "filter", "merge", "include", "in", "export",
        "default", "all", "named", "find", "between", "using", "count", "month",
        "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday",
        "January", "February", "March", "April", "May", "June",
        "July", "August", "September", "October", "November", "December"
    };

    public Lexer(string source)
    {
        _source = source;
    }

    public List<Token> Tokenize()
    {
        while (!IsAtEnd())
        {
            SkipWhitespace();
            int startCol = _column;
            char current = Peek();

            if (char.IsLetter(current) || current == '_')
                _tokens.Add(ReadIdentifierOrKeyword());
            else if (char.IsDigit(current))
                _tokens.Add(ReadNumberOrDuration());
            else if (current == '"')
                _tokens.Add(ReadString());
            else
                _tokens.Add(ReadSymbol());

            // EOF is added explicitly at the end
        }

        _tokens.Add(new Token(TokenType.EndOfFile, "", _line, _column));
        return _tokens;
    }

    // Utility methods (to be implemented next)
    private bool IsAtEnd() => _position >= _source.Length;
    private char Peek() => IsAtEnd() ? '\0' : _source[_position];

    private char Advance()
    {
        char c = _source[_position++];
        _column++;
        return c;
    }

    private void SkipWhitespace()
    {
        while (!IsAtEnd())
        {
            char c = Peek();
            if (c == ' ' || c == '\t' || c == '\r')
            {
                Advance();
            }
            else if (c == '\n')
            {
                _line++;
                _column = 1;
                Advance();
            }
            else
            {
                break;
            }
        }
    }


    private Token ReadIdentifierOrKeyword()
    {
        int start = _position;
        int col = _column;

        while (!IsAtEnd() && (char.IsLetterOrDigit(Peek()) || Peek() == '_'))
            Advance();

        string lexeme = _source.Substring(start, _position - start);

        TokenType type = Keywords.Contains(lexeme)
            ? TokenType.Keyword
            : TokenType.Identifier;

        return new Token(type, lexeme, _line, col);
    }

    private Token ReadNumberOrDuration()
    {
        int start = _position;
        int col = _column;
        bool hasDot = false;

        while (!IsAtEnd() && (char.IsDigit(Peek()) || Peek() == '.'))
        {
            if (Peek() == '.')
            {
                if (hasDot)
                    break; // second dot, break
                hasDot = true;
            }

            Advance();
        }

        string number = _source.Substring(start, _position - start);

        // Check for duration suffix: "m" or "h"
        if (!IsAtEnd() && (Peek() == 'm' || Peek() == 'h'))
        {
            char unit = Advance();
            return new Token(TokenType.Duration, number + unit, _line, col);
        }

        return new Token(TokenType.Number, number, _line, col);
    }

    private Token ReadString()
    {
        int col = _column;
        Advance(); // Skip opening quote

        StringBuilder sb = new();

        while (!IsAtEnd() && Peek() != '"')
        {
            if (Peek() == '\n')
            {
                _line++;
                _column = 1;
            }

            sb.Append(Advance());
        }

        if (IsAtEnd())
            return new Token(TokenType.Unknown, sb.ToString(), _line, col); // unterminated

        Advance(); // Skip closing quote
        return new Token(TokenType.String, sb.ToString(), _line, col);
    }


    private Token ReadSymbol()
    {
        if (IsAtEnd()) return new Token(TokenType.EndOfFile, "", _line, _column);

        int col = _column;
        char current = Advance();

        switch (current)
        {
            case '=':
                if (Match('=')) return new Token(TokenType.Equal, "==", _line, col);
                return new Token(TokenType.Assign, "=", _line, col);
            case '!':
                if (Match('=')) return new Token(TokenType.NotEqual, "!=", _line, col);
                break;
            case '<':
                if (Match('=')) return new Token(TokenType.LessEqual, "<=", _line, col);
                return new Token(TokenType.Less, "<", _line, col);
            case '>':
                if (Match('=')) return new Token(TokenType.GreaterEqual, ">=", _line, col);
                return new Token(TokenType.Greater, ">", _line, col);
            case '{': return new Token(TokenType.LeftBrace, "{", _line, col);
            case '}': return new Token(TokenType.RightBrace, "}", _line, col);
            case '(': return new Token(TokenType.LeftParen, "(", _line, col);
            case ')': return new Token(TokenType.RightParen, ")", _line, col);
            case ',': return new Token(TokenType.Comma, ",", _line, col);
            case ';': return new Token(TokenType.Semicolon, ";", _line, col);
            case ':': return new Token(TokenType.Colon, ":", _line, col);
            case '.': return new Token(TokenType.Dot, ".", _line, col);
        }

        return new Token(TokenType.Unknown, current.ToString(), _line, col);
    }

    private bool Match(char expected)
    {
        if (IsAtEnd()) return false;
        if (_source[_position] != expected) return false;

        _position++;
        _column++;
        return true;
    }
}