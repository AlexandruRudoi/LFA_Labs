using Lab_5.Domain;

namespace Lab_5.Application;

public static class Graphviz
{
    public static void ExportGrammarToDot(Grammar grammar, string dotFilePath)
    {
        using var writer = new StreamWriter(dotFilePath);
        writer.WriteLine("digraph Grammar {");
        writer.WriteLine("    rankdir=LR;");
        writer.WriteLine("    node [shape=circle];");

        // Style actual terminal leaf nodes
        writer.WriteLine("    \"a\" [shape=doublecircle, fillcolor=lightgray, style=filled];");
        writer.WriteLine("    \"b\" [shape=doublecircle, fillcolor=lightgray, style=filled];");

        foreach (var production in grammar.Productions)
        {
            var from = production.Left.Name;

            if (production.Right.Count == 1 && production.Right[0].IsTerminal)
            {
                var to = $"\"{production.Right[0].Name}\"";
                writer.WriteLine($"    {from} -> {to} [label=\"{from} → {production.Right[0].Name}\"];");
            }
            else
            {
                var rhs = string.Join(" ", production.Right.Select(s => s.Name));
                foreach (var symbol in production.Right)
                {
                    writer.WriteLine($"    {from} -> {symbol.Name} [label=\"{from} → {rhs}\"];");
                }
            }
        }

        writer.WriteLine("}");
    }
}