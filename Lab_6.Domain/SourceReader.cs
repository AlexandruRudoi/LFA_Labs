namespace Lab_6.Domain;

/// <summary>
///     Utility class for reading characters from a source string with line and column tracking for better error reporting.
/// </summary>
public class SourceReader
{
    private readonly string _source;
    private int _position = 0;
    private int _line = 1;
    private int _column = 1;

    /// <summary>
    ///     The current line, column and position in the source.
    /// </summary>
    public int Line => _line;

    /// <summary>
    ///     The current column and position in the source.
    /// </summary>
    public int Column => _column;

    /// <summary>
    ///     The current position in the source.
    /// </summary>
    public int Position => _position;

    /// <summary>
    ///     Indicates if the reader has reached the end of the source.
    /// </summary>
    public bool IsAtEnd => _position >= _source.Length;

    public SourceReader(string source)
    {
        _source = source;
    }

    /// <summary>
    ///     Returns the current character without advancing the position.
    /// </summary>
    public char Peek() => IsAtEnd ? '\0' : _source[_position];

    /// <summary>
    ///     Returns the character after the current one without advancing.
    /// </summary>
    public char PeekNext() => (_position + 1 >= _source.Length) ? '\0' : _source[_position + 1];

    /// <summary>
    ///     Advances the current position and returns the character. Updates line and column numbers accordingly.
    /// </summary>
    public char Advance()
    {
        if (IsAtEnd)
            return '\0'; // Prevent out-of-bounds

        char c = _source[_position++];

        if (c == '\n')
        {
            _line++;
            _column = 1;
        }
        else
        {
            _column++;
        }

        return c;
    }

    /// <summary>
    ///     Advances the current position by the specified amount.
    /// </summary>
    /// <param name="start"> The starting position. </param>
    /// <param name="length"> The length of the substring. </param>
    /// <returns> The substring. </returns>
    public string Substring(int start, int length)
    {
        return _source.Substring(start, length);
    }

    /// <summary>
    ///     Advances the current position by the specified amount.
    /// </summary>
    /// <param name="expected"> The expected character. </param>
    /// <returns> True if the character matches the expected one. </returns>
    public bool Match(char expected)
    {
        if (IsAtEnd) return false;
        if (_source[_position] != expected) return false;

        _position++;
        _column++;
        return true;
    }
}