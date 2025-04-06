namespace Lab_5.Domain;

public class Grammar
{
    public HashSet<Symbol> NonTerminals { get; set; } = new();
    public HashSet<Symbol> Terminals { get; set; } = new();
    public List<Production> Productions { get; set; } = new();
    public Symbol StartSymbol { get; set; }

    public Grammar(Symbol startSymbol)
    {
        StartSymbol = startSymbol;
    }

    public void AddProduction(Symbol left, IEnumerable<Symbol> right)
    {
        Productions.Add(new Production(left, right));
        NonTerminals.Add(left);
        foreach (var sym in right)
        {
            if (sym.IsTerminal) Terminals.Add(sym);
            else NonTerminals.Add(sym);
        }
    }
}