using Lab_5.Domain;

namespace Lab_5.Services;

public class CNFConverter
{
    private readonly Grammar _grammar;
    private int _intermediateCounter = 1;
    private readonly Dictionary<string, Symbol> _terminalMap = new();
    private readonly List<Production> _terminalRules = new();

    public CNFConverter(Grammar grammar)
    {
        _grammar = grammar;
    }

    public void EliminateEpsilonProductions()
    {
        var nullable = new HashSet<Symbol>();

        bool changed;
        do
        {
            changed = false;
            foreach (var production in _grammar.Productions.ToList())
            {
                if (!nullable.Contains(production.Left))
                {
                    if (production.Right.Count == 0 || production.Right.All(s => nullable.Contains(s)))
                    {
                        nullable.Add(production.Left);
                        changed = true;
                    }
                }
            }
        } while (changed);

        var newProductions = new List<Production>();

        foreach (var production in _grammar.Productions)
        {
            var right = production.Right;
            var combinations = GenerateNullableCombinations(right, nullable);

            foreach (var combo in combinations)
            {
                if (combo.Count > 0 || production.Left.Equals(_grammar.StartSymbol))
                    newProductions.Add(new Production(production.Left, combo));
            }
        }

        _grammar.Productions = newProductions;
    }

    private List<List<Symbol>> GenerateNullableCombinations(List<Symbol> symbols, HashSet<Symbol> nullable)
    {
        var results = new List<List<Symbol>> { new List<Symbol>() };

        foreach (var symbol in symbols)
        {
            if (nullable.Contains(symbol))
            {
                var newResults = new List<List<Symbol>>();
                foreach (var list in results)
                {
                    var with = new List<Symbol>(list) { symbol };
                    newResults.Add(with);
                    newResults.Add(new List<Symbol>(list));
                }

                results = newResults;
            }
            else
            {
                foreach (var list in results)
                    list.Add(symbol);
            }
        }

        return results;
    }

    public void EliminateRenaming()
    {
        var unitPairs = new HashSet<(Symbol, Symbol)>();

        foreach (var p in _grammar.Productions)
        {
            if (p.Right.Count == 1 && !p.Right[0].IsTerminal)
                unitPairs.Add((p.Left, p.Right[0]));
        }

        bool changed;
        do
        {
            changed = false;
            var newPairs = new HashSet<(Symbol, Symbol)>(unitPairs);

            foreach (var (a, b) in unitPairs)
            {
                foreach (var (c, d) in unitPairs)
                {
                    if (b.Equals(c) && !unitPairs.Contains((a, d)))
                    {
                        newPairs.Add((a, d));
                        changed = true;
                    }
                }
            }

            unitPairs = newPairs;
        } while (changed);

        var newProductions = new List<Production>();
        foreach (var (a, b) in unitPairs)
        {
            var bProductions = _grammar.Productions
                .Where(p => p.Left.Equals(b) && !(p.Right.Count == 1 && !p.Right[0].IsTerminal));

            foreach (var prod in bProductions)
            {
                newProductions.Add(new Production(a, prod.Right));
            }
        }

        _grammar.Productions = _grammar.Productions
            .Where(p => !(p.Right.Count == 1 && !p.Right[0].IsTerminal)).ToList();
        _grammar.Productions.AddRange(newProductions);
    }

    public void EliminateInaccessibleSymbols()
    {
        var accessible = new HashSet<Symbol> { _grammar.StartSymbol };
        bool changed;

        do
        {
            changed = false;
            foreach (var production in _grammar.Productions)
            {
                if (accessible.Contains(production.Left))
                {
                    foreach (var symbol in production.Right)
                    {
                        if (!symbol.IsTerminal && accessible.Add(symbol))
                        {
                            changed = true;
                        }
                    }
                }
            }
        } while (changed);

        // ONLY remove truly unreachable productions, not declared VN
        _grammar.Productions = _grammar.Productions
            .Where(p => accessible.Contains(p.Left))
            .ToList();

        // But don't change the declared non-terminals
    }


    public void EliminateNonProductiveSymbols()
    {
        var productive = new HashSet<Symbol>();
        bool changed;

        do
        {
            changed = false;
            foreach (var production in _grammar.Productions)
            {
                if (!productive.Contains(production.Left))
                {
                    if (production.Right.All(s => s.IsTerminal || productive.Contains(s)))
                    {
                        productive.Add(production.Left);
                        changed = true;
                    }
                }
            }
        } while (changed);

        _grammar.Productions = _grammar.Productions
            .Where(p => productive.Contains(p.Left) && p.Right.All(s => s.IsTerminal || productive.Contains(s)))
            .ToList();

        _grammar.NonTerminals = _grammar.NonTerminals.Where(nt => productive.Contains(nt)).ToHashSet();
    }

    public void ConvertToChomskyNormalForm()
    {
        var newProductions = new List<Production>();

        foreach (var production in _grammar.Productions)
        {
            var right = production.Right;
            var updatedRight = new List<Symbol>();

            foreach (var symbol in right)
            {
                if (symbol.IsTerminal)
                {
                    var wrapper = GetOrCreateTerminalWrapper(symbol);
                    updatedRight.Add(wrapper);
                }
                else
                {
                    updatedRight.Add(symbol);
                }
            }

            if (updatedRight.Count <= 2)
            {
                newProductions.Add(new Production(production.Left, updatedRight));
            }
            else
            {
                var currentLeft = production.Left;
                for (int i = 0; i < updatedRight.Count - 2; i++)
                {
                    var nextSymbol = GetNextIntermediateSymbol();
                    newProductions.Add(new Production(currentLeft, new List<Symbol> { updatedRight[i], nextSymbol }));
                    currentLeft = nextSymbol;
                }

                newProductions.Add(new Production(currentLeft, updatedRight.GetRange(updatedRight.Count - 2, 2)));
            }
        }

        _grammar.Productions = newProductions;
        _grammar.Productions.AddRange(_terminalRules);
    }

    private Symbol GetOrCreateTerminalWrapper(Symbol terminal)
    {
        if (_terminalMap.ContainsKey(terminal.Name))
            return _terminalMap[terminal.Name];

        var wrapperName = $"T_{terminal.Name}";
        var wrapper = new Symbol(wrapperName, false);

        if (_grammar.NonTerminals.Contains(wrapper))
        {
            int suffix = 1;
            while (_grammar.NonTerminals.Contains(new Symbol(wrapperName + suffix, false)))
            {
                suffix++;
            }

            wrapper = new Symbol(wrapperName + suffix, false);
        }

        _grammar.NonTerminals.Add(wrapper);
        _terminalMap[terminal.Name] = wrapper;
        _terminalRules.Add(new Production(wrapper, new List<Symbol> { terminal }));

        return wrapper;
    }

    private Symbol GetNextIntermediateSymbol()
    {
        Symbol symbol;
        do
        {
            symbol = new Symbol($"X{_intermediateCounter++}", false);
        } while (_grammar.NonTerminals.Contains(symbol));

        _grammar.NonTerminals.Add(symbol);
        return symbol;
    }

    public void Normalize()
    {
        EliminateEpsilonProductions();
        EliminateRenaming();
        // EliminateInaccessibleSymbols(); // Not required for CNF, also may delete D → A B
        EliminateNonProductiveSymbols();
        ConvertToChomskyNormalForm();
    }
}