namespace Lab_6.Parser;

public class FilterNode : AstNode
{
    public string Source { get; set; } = "";
    public string Field { get; set; } = "";
    public string Operator { get; set; } = "";
    public string Value { get; set; } = "";
    public string Target { get; set; } = "";

    public override void Print(string indent = "")
    {
        Console.WriteLine($"{indent}Filter: {Source} where {Field} {Operator} {Value} into {Target}");
    }
}