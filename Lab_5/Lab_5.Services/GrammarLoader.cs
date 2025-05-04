using System.Text.RegularExpressions;
using Lab_5.Domain;

namespace Lab_5.Services;

public static class GrammarLoader
{
    public static Grammar LoadFromFile(string path, string startSymbolName)
    {
        var lines = File.ReadAllLines(path)
            .Where(line => !string.IsNullOrWhiteSpace(line) && !line.Trim().StartsWith("#"))
            .ToList();

        var nonTerminals = new HashSet<string>();
        var terminals = new HashSet<string>();
        int currentLine = 0;

        if (lines[currentLine].StartsWith("VN:"))
        {
            var vnLine = lines[currentLine++].Substring(3).Trim();
            foreach (var nt in vnLine.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            {
                nonTerminals.Add(nt);
            }
        }

        if (lines[currentLine].StartsWith("VT:"))
        {
            var vtLine = lines[currentLine++].Substring(3).Trim();
            foreach (var t in vtLine.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            {
                terminals.Add(t);
            }
        }

        var symbols = new Dictionary<string, Symbol>();

        Symbol GetSymbol(string name)
        {
            if (!symbols.ContainsKey(name))
            {
                bool isTerminal = terminals.Contains(name);
                symbols[name] = new Symbol(name, isTerminal);
            }

            return symbols[name];
        }

        var startSymbol = GetSymbol(startSymbolName);
        var grammar = new Grammar(startSymbol);

        var arrowPattern = new Regex(@"\s*->\s*");
        for (int i = currentLine; i < lines.Count; i++)
        {
            var parts = arrowPattern.Split(lines[i]);
            if (parts.Length != 2) continue;

            var left = parts[0].Trim();
            var rightSymbols = parts[1].Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var lhs = GetSymbol(left);
            var rhs = new List<Symbol>();

            foreach (var sym in rightSymbols)
            {
                if (sym == "ε") continue; // epsilon production will be handled as empty RHS
                rhs.Add(GetSymbol(sym));
            }

            grammar.AddProduction(lhs, rhs);
        }

        return grammar;
    }
}