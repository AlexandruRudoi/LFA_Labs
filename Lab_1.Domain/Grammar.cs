namespace Lab_1.Domain;

public class Grammar
{
    public HashSet<string> VN { get; }
    public HashSet<string> VT { get; }
    public Dictionary<string, List<string>> Productions { get; }
    public string StartSymbol { get; }
    private static Random random = new Random();

    public Grammar(HashSet<string> vn, HashSet<string> vt, Dictionary<string, List<string>> productions,
        string startSymbol)
    {
        VN = vn;
        VT = vt;
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
        HashSet<string> states = new HashSet<string>(VN);
        HashSet<string> alphabet = new HashSet<string>(VT);
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

                if (VT.Contains(firstSymbol))
                    finalStates.Add(firstSymbol);
            }
        }

        return new FiniteAutomaton(states, alphabet, transitions, StartSymbol, finalStates);
    }
}