namespace Lab_3.Domain;

/// <summary>
///     Utility class with helper methods for character classification.
/// </summary>
public static class CharUtils
{
    public static bool IsIdentifierStart(char c) => char.IsLetter(c) || c == '_';

    public static bool IsIdentifierPart(char c) => char.IsLetterOrDigit(c) || c == '_';

    public static bool IsDigitOrDot(char c) => char.IsDigit(c) || c == '.';
}