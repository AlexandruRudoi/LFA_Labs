namespace Lab_1.Domain;

public class FiniteAutomaton
{
    public HashSet<string> States { get; }
    public HashSet<string> Alphabet { get; }
    public Dictionary<string, Dictionary<string, HashSet<string>>> Transitions { get; }
    public string StartState { get; }
    public HashSet<string> FinalStates { get; }

    public FiniteAutomaton(HashSet<string> states, HashSet<string> alphabet,
        Dictionary<string, Dictionary<string, HashSet<string>>> transitions, string startState,
        HashSet<string> finalStates)
    {
        States = states;
        Alphabet = alphabet;
        Transitions = transitions;
        StartState = startState;
        FinalStates = finalStates;
    }

    public bool StringBelongsToLanguage(string inputString)
    {
        HashSet<string> currentStates = new() { StartState };

        foreach (char c in inputString)
        {
            HashSet<string> nextStates = new();
            foreach (string state in currentStates)
            {
                if (Transitions.ContainsKey(state) && Transitions[state].ContainsKey(c.ToString()))
                    nextStates.UnionWith(Transitions[state][c.ToString()]);
            }

            if (!nextStates.Any())
                return false; // If there are no valid transitions, reject the string

            currentStates = nextStates;
        }

        return currentStates.Any(state => FinalStates.Contains(state));
    }
}