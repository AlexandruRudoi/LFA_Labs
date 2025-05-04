namespace Lab_6.Parser;

public class AssignmentNode : AstNode
{
    public string Identifier { get; set; }
    public string Value { get; set; }

    public override void Print(string indent = "")
    {
        Console.WriteLine($"{indent}Assignment: {Identifier} = {Value}");
    }
}