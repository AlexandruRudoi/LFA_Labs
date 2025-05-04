namespace Lab_6.Domain;

public enum TokenType
{
    // Single-character tokens
    LeftParen, RightParen,
    LeftBrace, RightBrace,
    Comma, Dot, Semicolon, Colon,

    // Operators
    Assign, Equal, NotEqual,
    Greater, GreaterEqual,
    Less, LessEqual,

    // Literals
    Identifier, String, Number, Duration,

    // Keywords
    Import, As, Event, Task, Pomodoro, New, On, From, To, Into,
    At, Each, With, Alarm, Repeat, Times, Break, WeekNumber,
    If, Else, ElseIf, Filter, Merge, Include, In, Export,
    Default, All, Named, Find, Between, Using, Count, Month, Day, Where, Every,

    // Days of the week
    Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday,

    // Months
    January, February, March, April, May, June,
    July, August, September, October, November, December,

    // Special
    EndOfFile,
    Unknown
}