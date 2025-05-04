namespace Lab_6.Parser;

public class InlinePomodoroNode : AstNode
{
    public string Identifier { get; set; } = "";
    public string Title { get; set; } = "";
    public string? StartTime { get; set; }
    public int RepeatCount { get; set; } = 1;
    public string? BreakDuration { get; set; }

    public override void Print(string indent = "")
    {
        Console.WriteLine($"{indent}InlinePomodoro: {Identifier}");
        Console.WriteLine($"{indent}  Title: \"{Title}\"");
        if (StartTime != null)
            Console.WriteLine($"{indent}  Start Time: {StartTime}");
        Console.WriteLine($"{indent}  Repeat: {RepeatCount} times");
        if (BreakDuration != null)
            Console.WriteLine($"{indent}  Break: {BreakDuration}");
    }
}