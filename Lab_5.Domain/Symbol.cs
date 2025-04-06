namespace Lab_5.Domain;

public class Symbol
{
    public string Name { get; set; }
    public bool IsTerminal { get; set; }

    public Symbol(string name, bool isTerminal)
    {
        Name = name;
        IsTerminal = isTerminal;
    }

    public override string ToString() => Name;
    public override bool Equals(object obj) => obj is Symbol s && s.Name == Name && s.IsTerminal == IsTerminal;
    public override int GetHashCode() => (Name, IsTerminal).GetHashCode();
}