namespace Lab_3.Domain;

/// <summary>
///     Centralized set of reserved keywords in the AION language.
/// </summary>
public static class Keywords
{
    public static readonly HashSet<string> All = new()
    {
        "import", "as", "event", "task", "pomodoro", "new", "on", "from", "to",
        "at", "each", "with", "alarm", "repeat", "times", "break", "weeknumber",
        "if", "else", "else if", "filter", "merge", "include", "in", "export",
        "default", "all", "named", "find", "between", "using", "count", "month",
        "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday",
        "January", "February", "March", "April", "May", "June",
        "July", "August", "September", "October", "November", "December"
    };
}