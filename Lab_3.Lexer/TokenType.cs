namespace Lab_3.Lexer;

public enum TokenType
{
    // Single-character tokens
    LeftParen,
    RightParen,
    LeftBrace,
    RightBrace,
    Comma,
    Dot,
    Semicolon,
    Colon,

    // Operators
    Assign, // =
    Equal, // ==
    NotEqual, // !=
    Greater, // >
    GreaterEqual, // >=
    Less, // <
    LessEqual, // <=

    // Literals
    Identifier,
    String,
    Number,
    Duration,

    // Keywords
    Keyword,

    // Special
    EndOfFile,
    Unknown
}