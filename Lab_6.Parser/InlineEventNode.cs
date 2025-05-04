namespace Lab_6.Parser;

public class InlineEventNode : AstNode
{
    public string Identifier { get; set; } = "";
    public string Title { get; set; } = "";
    public string? Date { get; set; }
    public string? DayOfWeek { get; set; }
    public string? FromTime { get; set; }
    public string? ToTime { get; set; }
    public string? Location { get; set; }
    public string? Ordinal { get; set; }  // e.g., "2nd", "last"
    public bool IsRecurring { get; set; } = false;

    public override void Print(string indent = "")
    {
        Console.WriteLine($"{indent}InlineEvent: {Identifier}");
        Console.WriteLine($"{indent}  Title: {Title}");
        if (Date != null) Console.WriteLine($"{indent}  Date: {Date}");
        if (DayOfWeek != null) Console.WriteLine($"{indent}  Every: {DayOfWeek}");
        if (FromTime != null && ToTime != null)
            Console.WriteLine($"{indent}  Time: {FromTime} -> {ToTime}");
        if (Location != null) Console.WriteLine($"{indent}  Location: {Location}");
    }
}