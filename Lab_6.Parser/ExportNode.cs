namespace Lab_6.Parser;

public class ExportNode : AstNode
{
    public string Source { get; set; } // e.g. meetings_only or default
    public string? OutputFile { get; set; } // e.g. "main.ics" or null

    public override void Print(string indent = "")
    {
        var target = OutputFile != null ? $" as {OutputFile}" : "";
        Console.WriteLine($"{indent}Export: {Source}{target}");
    }
}