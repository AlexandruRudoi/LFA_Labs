namespace Lab_6.Parser;

public class InlineTaskNode : AstNode
{
    public string Identifier { get; set; } = "";
    public string Title { get; set; } = "";
    public string? Time { get; set; }
    public string? DayOfWeek { get; set; }
    public bool HasAlarm { get; set; } = false;

    public override void Print(string indent = "")
    {
        Console.WriteLine($"{indent}InlineTask: {Identifier}");
        Console.WriteLine($"{indent}  Title: \"{Title}\"");
        if (Time != null) Console.WriteLine($"{indent}  Time: {Time}");
        if (DayOfWeek != null) Console.WriteLine($"{indent}  Day: {DayOfWeek}");
        if (HasAlarm) Console.WriteLine($"{indent}  Alarm: true");
    }
}