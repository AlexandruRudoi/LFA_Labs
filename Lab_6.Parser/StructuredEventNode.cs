namespace Lab_6.Parser;

public class StructuredEventNode : AstNode
{
    public string Identifier { get; set; } = "";
    public string? Name { get; set; }
    public string? StartTime { get; set; }
    public string? Duration { get; set; }
    public string? Location { get; set; }

    public override void Print(string indent = "")
    {
        Console.WriteLine($"{indent}Structured Event: {Identifier}");
        if (Name != null)
            Console.WriteLine($"{indent}  Name: {Name}");
        if (StartTime != null)
            Console.WriteLine($"{indent}  Start: {StartTime}");
        if (Duration != null)
            Console.WriteLine($"{indent}  Duration: {Duration}");
        if (Location != null)
            Console.WriteLine($"{indent}  Location: {Location}");
    }
}