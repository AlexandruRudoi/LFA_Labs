namespace Lab_6.Parser;

public class ProgramNode : AstNode
{
    public List<AstNode> Statements { get; } = new();

    public override void Print(string indent = "")
    {
        Console.WriteLine($"{indent}Program");
        foreach (var stmt in Statements)
            stmt.Print(indent + "  ");
    }
}