namespace Lab_4.Domain;

public class RegexNode
{
    public RegexNodeType Type { get; set; }

    public string? Value { get; set; } // For literals
    public List<RegexNode> Children { get; set; } = new();

    public int MinRepeat { get; set; } = 1;
    public int MaxRepeat { get; set; } = 1;

    public RegexNode(RegexNodeType type, string? value = null)
    {
        Type = type;
        Value = value;
    }
}