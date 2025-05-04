namespace Lab_6.Domain;

public static class Keywords
{
    public static readonly Dictionary<string, TokenType> Map = new()
    {
        // Keywords
        ["import"] = TokenType.Import,
        ["as"] = TokenType.As,
        ["event"] = TokenType.Event,
        ["task"] = TokenType.Task,
        ["pomodoro"] = TokenType.Pomodoro,
        ["new"] = TokenType.New,
        ["on"] = TokenType.On,
        ["from"] = TokenType.From,
        ["to"] = TokenType.To,
        ["at"] = TokenType.At,
        ["each"] = TokenType.Each,
        ["with"] = TokenType.With,
        ["alarm"] = TokenType.Alarm,
        ["repeat"] = TokenType.Repeat,
        ["times"] = TokenType.Times,
        ["break"] = TokenType.Break,
        ["weeknumber"] = TokenType.WeekNumber,
        ["if"] = TokenType.If,
        ["else"] = TokenType.Else,
        ["else if"] = TokenType.ElseIf,
        ["filter"] = TokenType.Filter,
        ["merge"] = TokenType.Merge,
        ["include"] = TokenType.Include,
        ["in"] = TokenType.In,
        ["export"] = TokenType.Export,
        ["default"] = TokenType.Default,
        ["all"] = TokenType.All,
        ["named"] = TokenType.Named,
        ["find"] = TokenType.Find,
        ["between"] = TokenType.Between,
        ["using"] = TokenType.Using,
        ["count"] = TokenType.Count,
        ["month"] = TokenType.Month,

        // Days of the week
        ["monday"] = TokenType.Monday,
        ["tuesday"] = TokenType.Tuesday,
        ["wednesday"] = TokenType.Wednesday,
        ["thursday"] = TokenType.Thursday,
        ["friday"] = TokenType.Friday,
        ["saturday"] = TokenType.Saturday,
        ["sunday"] = TokenType.Sunday,

        // Months
        ["january"] = TokenType.January,
        ["february"] = TokenType.February,
        ["march"] = TokenType.March,
        ["april"] = TokenType.April,
        ["may"] = TokenType.May,
        ["june"] = TokenType.June,
        ["july"] = TokenType.July,
        ["august"] = TokenType.August,
        ["september"] = TokenType.September,
        ["october"] = TokenType.October,
        ["november"] = TokenType.November,
        ["december"] = TokenType.December
    };
}