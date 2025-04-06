namespace Lab_5.Domain;

public class Production
{
    public Symbol Left { get; set; }
    public List<Symbol> Right { get; set; }

    public Production(Symbol left, IEnumerable<Symbol> right)
    {
        Left = left;
        Right = right.ToList();
    }

    public override string ToString() => $"{Left} → {string.Join("", Right)}";
}