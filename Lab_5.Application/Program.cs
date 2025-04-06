using Lab_5.Domain;
using Lab_5.Services;

namespace Lab_5.Application;

class Program
{
    static void Main(string[] args)
    {
        var filePath = "D:\\Projects\\University\\LFA_Labs\\Lab_5\\Lab_5.Application\\resources\\variant27.txt";
        var startSymbol = "S";

        if (!File.Exists(filePath))
        {
            Console.WriteLine($"Error: File '{filePath}' not found.");
            return;
        }

        var grammar = GrammarLoader.LoadFromFile(filePath, startSymbol);
        var converter = new CNFConverter(grammar);
        converter.Normalize();

        PrintPrettyProductions(grammar);
    }

    static void PrintPrettyProductions(Grammar grammar)
    {
        var grouped = grammar.Productions
            .GroupBy(p => p.Left)
            .OrderBy(g => g.Key.Name);

        Console.WriteLine("\nCNF Productions:\n");

        foreach (var group in grouped)
        {
            var left = group.Key.Name.PadRight(4);
            var alternatives = group.Select(p => string.Join(" ", p.Right.Select(s => s.Name))).ToList();

            Console.WriteLine($"{left} -> {alternatives[0]}");
            for (int i = 1; i < alternatives.Count; i++)
            {
                Console.WriteLine("     | " + alternatives[i]);
            }

            Console.WriteLine();
        }
    }
}