using Lab_6.Domain;

namespace Lab_6.Parser;

public class Parser
{
    private readonly List<Token> _tokens;
    private int _current = 0;

    public Parser(List<Token> tokens)
    {
        _tokens = tokens;
    }

    public ProgramNode ParseProgram()
    {
        var program = new ProgramNode();

        while (!IsAtEnd())
        {
            var stmt = ParseStatement();
            if (stmt != null)
                program.Statements.Add(stmt);
        }

        return program;
    }

    private AstNode? ParseStatement()
    {
        if (Match(TokenType.Event))
            return ParseEvent();
        // Add support for more statements later (Task, If, etc.)

        // Skip unknown
        Advance();
        return null;
    }

    private EventNode ParseEvent()
    {
        var nameToken = Consume(TokenType.Identifier, "Expected identifier after 'event'");
        Consume(TokenType.LeftBrace, "Expected '{' after event name");

        var body = new List<AstNode>();
        while (!Check(TokenType.RightBrace) && !IsAtEnd())
        {
            var stmt = ParseStatement();
            if (stmt != null)
                body.Add(stmt);
        }

        Consume(TokenType.RightBrace, "Expected '}' after event body");

        return new EventNode
        {
            Name = nameToken.Lexeme,
            Body = body
        };
    }

    // === Helpers ===
    private bool Match(TokenType type)
    {
        if (Check(type))
        {
            Advance();
            return true;
        }

        return false;
    }

    private bool Check(TokenType type)
    {
        if (IsAtEnd()) return false;
        return Peek().Type == type;
    }

    private Token Advance()
    {
        if (!IsAtEnd()) _current++;
        return Previous();
    }

    private bool IsAtEnd() => Peek().Type == TokenType.EndOfFile;

    private Token Peek() => _tokens[_current];
    private Token Previous() => _tokens[_current - 1];

    private Token Consume(TokenType type, string errorMessage)
    {
        if (Check(type)) return Advance();
        throw new Exception($"[Line {Peek().Line}] Error: {errorMessage}");
    }
}