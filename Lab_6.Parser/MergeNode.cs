namespace Lab_6.Parser;

public class MergeNode : AstNode
{
    public List<string> Sources { get; set; } = new();
    public string Target { get; set; } = "";

    public override void Print(string indent = "")
    {
        Console.WriteLine($"{indent}Merge: [{string.Join(", ", Sources)}] → {Target}");
    }
}