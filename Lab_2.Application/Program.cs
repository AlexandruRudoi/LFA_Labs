using System.Diagnostics;
using Lab_2.Domain;

namespace Lab_2.Application;

class Program
{
    static void Main()
    {
        // Corrected Grammar for Variant 27
        var rules = new Dictionary<string, List<string>>
        {
            { "S", new List<string> { "aA", "bB" } },
            { "A", new List<string> { "bS", "cA", "aB" } },
            { "B", new List<string> { "aB", "b" } }
        };

        var nonTerminals = new HashSet<string> { "S", "A", "B" };
        var terminals = new HashSet<string> { "a", "b", "c" };
        string startSymbol = "S";

        Grammar grammar = new(nonTerminals, terminals, rules, startSymbol);

        Console.WriteLine("_________________");
        Console.WriteLine("NonTerminals: " + string.Join(", ", grammar.NonTerminals));
        Console.WriteLine("Terminals: " + string.Join(", ", grammar.Terminals));
        Console.WriteLine("Productions:");
        foreach (var rule in grammar.Productions)
            Console.WriteLine($"{rule.Key} -> {string.Join(" | ", rule.Value)}");
        Console.WriteLine("StartSymbol: " + grammar.StartSymbol);
        Console.WriteLine("_________________");

        // Generate words
        List<string> generatedWords = grammar.GenerateStrings();
        Console.WriteLine("\nGenerated Strings:");
        generatedWords.ForEach(Console.WriteLine);

        FiniteAutomaton fa = grammar.ToFiniteAutomaton();

        Console.WriteLine("\nControl if generated words are valid:");
        foreach (string word in generatedWords)
            Console.WriteLine($"{word} belongs to the language? {fa.StringBelongsToLanguage(word)}");

        // Incorrect words check
        List<string> incorrectWords = new() { "xyz", "abc", "da", "ae", "cc", "fS", "rL", "aee" };
        Console.WriteLine("\nChecking invalid words:");
        foreach (string word in incorrectWords)
            Console.WriteLine($"{word} belongs to the language? {fa.StringBelongsToLanguage(word)}");

        string grammarType = grammar.ClassifyGrammar();
        Console.WriteLine("\n---------------------------------------");
        Console.WriteLine($"Grammar classification: {grammarType}");

        Console.WriteLine("\n_________________________________________");

        // Corrected Finite Automaton for Variant 27
        var states = new HashSet<string> { "q0", "q1", "q2", "q3" };
        var alphabet = new HashSet<string> { "a", "b" };
        var finalStates = new HashSet<string> { "q3" };
        var transitions = new Dictionary<string, Dictionary<string, HashSet<string>>>
        {
            { "q0", new() { { "a", new() { "q1" } }, { "b", new() { "q2" } } } },
            { "q1", new() { { "b", new() { "q2" } }, { "a", new() { "q3", "q1" } } } },
            { "q2", new() { { "b", new() { "q3" } } } }
        };

        FiniteAutomaton fa1 = new(states, alphabet, transitions, "q0", finalStates);
        Grammar grammar1 = fa1.ToGrammar();

        Console.WriteLine("_________________");
        Console.WriteLine("Grammar from FA:");
        Console.WriteLine("NonTerminals: " + string.Join(", ", grammar1.NonTerminals));
        Console.WriteLine("Terminals: " + string.Join(", ", grammar1.Terminals));
        Console.WriteLine("Productions:");
        foreach (var rule in grammar1.Productions)
            Console.WriteLine($"{rule.Key} -> {string.Join(" | ", rule.Value)}");
        Console.WriteLine("StartSymbol: " + grammar1.StartSymbol);
        Console.WriteLine("_________________");

        // Generate words from FA grammar
        Console.WriteLine("Generated Words from FA Grammar:");
        List<string> generatedWords1 = grammar1.GenerateStrings();

        Console.WriteLine("\nIs Deterministic? " + fa1.IsDeterministic());

        // Convert to DFA
        FiniteAutomaton dfa = fa1.ConvertToDFA();

        Console.WriteLine("\nDFA States: " + string.Join(", ", dfa.States));
        Console.WriteLine("DFA Transitions:");
        foreach (var state in dfa.Transitions)
        {
            foreach (var transition in state.Value)
            {
                Console.WriteLine(
                    $"δ({state.Key}, {transition.Key}) -> {{ {string.Join(", ", transition.Value)} }}");
            }
        }

        Console.WriteLine("DFA Start State: " + dfa.StartState);
        Console.WriteLine("DFA Final States: " + string.Join(", ", dfa.FinalStates));

        // **Generate DOT representation**
        string dotRepresentation = dfa.ToDot();

        // **Save the DOT file**
        string dotFilePath = "DFA.dot";
        File.WriteAllText(dotFilePath, dotRepresentation);
        Console.WriteLine("DFA saved as DFA.dot");

        // **Generate PNG image**
        GenerateGraphImage(dotFilePath, "DFA.png");
    }

    static void GenerateGraphImage(string dotFilePath, string outputImagePath)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = @"C:\Program Files\Graphviz\bin\dot.exe",
            Arguments = $"-Tpng {dotFilePath} -o {outputImagePath}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(startInfo);
        process.WaitForExit();

        Console.WriteLine($"DFA image saved as {outputImagePath}");
    }
}