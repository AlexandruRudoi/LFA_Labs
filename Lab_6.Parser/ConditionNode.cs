namespace Lab_6.Parser;

public class ConditionNode : AstNode
{
    public string Left { get; set; } = null!;
    public string Operator { get; set; } = null!;
    public string Right { get; set; } = null!;

    public override void Print(string indent = "")
    {
        Console.WriteLine($"{indent}Condition: {Left} {Operator} {Right}");
    }
}