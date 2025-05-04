namespace Lab_6.Parser;

public class IncludeNode : AstNode
{
    public string Target { get; set; } // e.g. find_gift
    public string? Calendar { get; set; } // e.g. personal

    public override void Print(string indent = "")
    {
        Console.WriteLine($"{indent}Include: {Target} in {Calendar}");
    }
}