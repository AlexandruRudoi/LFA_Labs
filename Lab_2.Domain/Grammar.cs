namespace Lab_2.Domain;

public class Grammar
{
    public HashSet<string> NonTerminals { get; }
    public HashSet<string> Terminals { get; }
    public Dictionary<string, List<string>> Productions { get; }
    public string StartSymbol { get; }
    private static Random random = new Random();

    public Grammar(HashSet<string> nonTerminals, HashSet<string> terminals, Dictionary<string, List<string>> productions,
        string startSymbol)
    {
        NonTerminals = nonTerminals;
        Terminals = terminals;
        Productions = productions;
        StartSymbol = startSymbol;
    }

    public List<string> GenerateStrings(int count = 5)
    {
        List<string> generatedStrings = new List<string>();

        for (int i = 0; i < count; i++)
        {
            string generatedWord = GenerateWord(StartSymbol);
            Console.WriteLine(generatedWord);
            generatedStrings.Add(generatedWord);
        }

        return generatedStrings;
    }

    private string GenerateWord(string symbol)
    {
        if (!Productions.ContainsKey(symbol)) return symbol; // Terminal symbol, return it

        string rule = Productions[symbol][random.Next(Productions[symbol].Count)];
        string result = string.Concat(rule.Select(c => GenerateWord(c.ToString())));

        return result;
    }

    public FiniteAutomaton ToFiniteAutomaton()
    {
        HashSet<string> states = new HashSet<string>(NonTerminals);
        HashSet<string> alphabet = new HashSet<string>(Terminals);
        Dictionary<string, Dictionary<string, HashSet<string>>> transitions = new();
        HashSet<string> finalStates = new HashSet<string>();

        foreach (var kvp in Productions)
        {
            string nonTerminal = kvp.Key;
            transitions[nonTerminal] = new Dictionary<string, HashSet<string>>();

            foreach (string rule in kvp.Value)
            {
                string firstSymbol = rule[0].ToString();
                string rest = rule.Length > 1 ? rule.Substring(1) : "";

                if (!transitions[nonTerminal].ContainsKey(firstSymbol))
                    transitions[nonTerminal][firstSymbol] = new HashSet<string>();

                transitions[nonTerminal][firstSymbol].Add(string.IsNullOrEmpty(rest) ? firstSymbol : rest);

                if (Terminals.Contains(firstSymbol))
                    finalStates.Add(firstSymbol);
            }
        }

        return new FiniteAutomaton(states, alphabet, transitions, StartSymbol, finalStates);
    }

    public string ClassifyGrammar()
    {
        bool type3 = true, type2 = true, type1 = true;

        foreach (var rule in Productions)
        {
            string left = rule.Key;
            foreach (var right in rule.Value)
            {
                if (left.Length > 1) type3 = type2 = false;
                if (right.Length < left.Length) type1 = false;

                bool startsWithVN = NonTerminals.Contains(right[0].ToString());
                bool endsWithVN = NonTerminals.Contains(right[^1].ToString());
                int vnCount = right.Count(c => NonTerminals.Contains(c.ToString()));

                if (vnCount > 1 || (startsWithVN && endsWithVN)) type3 = false;
            }
        }

        return type3 ? "Type 3: Regular Grammar"
            : type2 ? "Type 2: Context-Free Grammar"
            : type1 ? "Type 1: Context-Sensitive Grammar"
            : "Type 0: Unrestricted Grammar";
    }
}