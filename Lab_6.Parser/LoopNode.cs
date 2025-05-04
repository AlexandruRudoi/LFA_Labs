namespace Lab_6.Parser;

public class LoopNode : AstNode
{
    public string Unit { get; set; } = ""; // "day" or "month"
    public string From { get; set; } = "";
    public string To { get; set; } = "";
    public List<AstNode> Body { get; set; } = new();

    public override void Print(string indent = "")
    {
        Console.WriteLine($"{indent}Loop: each {Unit} from {From} to {To}");
        foreach (var stmt in Body)
            stmt.Print(indent + "  ");
    }
}