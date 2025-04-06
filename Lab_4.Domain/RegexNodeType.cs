namespace Lab_4.Domain;

public enum RegexNodeType
{
    Literal, // Single characters like 'A', 'B', '3'
    Concat, // Sequence: AB(C|D)E
    Alternation, // A|B|C
    Repetition, // *, +, ?, {min,max}
    Group // Parentheses
}