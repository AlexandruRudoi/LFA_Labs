using Lab_1.Domain;

namespace Lab_1;

class Program
{
    static void Main()
    {
        var rules = new Dictionary<string, List<string>>
        {
            { "S", new List<string> { "aA", "bB" } },
            { "A", new List<string> { "bS", "cA", "aB" } },
            { "B", new List<string> { "aB", "b" } }
        };

        Grammar grammar = new(
            new HashSet<string> { "S", "A", "B" },
            new HashSet<string> { "a", "b", "c" },
            rules,
            "S"
        );

        Console.WriteLine("Generated Strings:");
        List<string> generatedWords = grammar.GenerateStrings();

        FiniteAutomaton fa = grammar.ToFiniteAutomaton();

        Console.WriteLine("\nControl if generated words are valid:");
        foreach (string word in generatedWords)
        {
            Console.WriteLine($"{word} belongs to the language? {fa.StringBelongsToLanguage(word)}");
        }

        List<string> incorrectWords = new() { "xyz", "abc", "da", "ae", "cc", "fS", "rL", "aee" };
        Console.WriteLine("\nChecking invalid words:");
        foreach (string word in incorrectWords)
        {
            Console.WriteLine($"{word} belongs to the language? {fa.StringBelongsToLanguage(word)}");
        }
    }
}