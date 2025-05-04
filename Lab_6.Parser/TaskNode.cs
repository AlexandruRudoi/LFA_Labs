namespace Lab_6.Parser;

public class TaskNode : AstNode
{
    public string Name { get; set; }
    public List<AstNode> Body { get; set; } = new();

    public override void Print(string indent = "")
    {
        Console.WriteLine($"{indent}Task: {Name}");
        foreach (var node in Body)
            node.Print(indent + "  ");
    }
}