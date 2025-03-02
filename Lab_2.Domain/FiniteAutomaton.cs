namespace Lab_2.Domain;

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

    public bool IsDeterministic()
    {
        foreach (var state in Transitions)
        {
            foreach (var transition in state.Value)
            {
                if (transition.Value.Count > 1)
                    return false; // Multiple transitions for same symbol = NDFA
            }
        }

        return true;
    }

    public FiniteAutomaton ConvertToDFA()
    {
        if (IsDeterministic()) return this;

        var newStates = new HashSet<string>();
        var newTransitions = new Dictionary<string, Dictionary<string, HashSet<string>>>();
        var newFinalStates = new HashSet<string>();
        var queue = new Queue<HashSet<string>>();
        var stateMapping = new Dictionary<HashSet<string>, string>(new HashSetComparer());

        HashSet<string> startSet = new() { StartState };
        queue.Enqueue(startSet);
        stateMapping[startSet] = string.Join("", startSet);
        newStates.Add(stateMapping[startSet]);

        // If any state in the start set is final, the DFA start state should also be final
        if (startSet.Any(s => FinalStates.Contains(s)))
            newFinalStates.Add(stateMapping[startSet]);

        while (queue.Count > 0)
        {
            var currentSet = queue.Dequeue();
            string currentState = stateMapping[currentSet];

            foreach (string symbol in Alphabet)
            {
                HashSet<string> nextSet = new();
                foreach (string state in currentSet)
                {
                    if (Transitions.ContainsKey(state) && Transitions[state].ContainsKey(symbol))
                        nextSet.UnionWith(Transitions[state][symbol]);
                }

                if (nextSet.Count > 0)
                {
                    if (!stateMapping.ContainsKey(nextSet))
                    {
                        stateMapping[nextSet] = string.Join("", nextSet);
                        queue.Enqueue(nextSet);
                        newStates.Add(stateMapping[nextSet]);

                        // If any state in the new set is final, mark the entire DFA state as final
                        if (nextSet.Any(s => FinalStates.Contains(s)))
                            newFinalStates.Add(stateMapping[nextSet]);
                    }

                    if (!newTransitions.ContainsKey(currentState))
                        newTransitions[currentState] = new Dictionary<string, HashSet<string>>();

                    newTransitions[currentState][symbol] = new HashSet<string> { stateMapping[nextSet] };
                }
            }
        }

        // return RenameStates(new FiniteAutomaton(newStates, Alphabet, newTransitions, stateMapping[startSet], newFinalStates));
        return new FiniteAutomaton(newStates, Alphabet, newTransitions, stateMapping[startSet], newFinalStates);
    }

    public Grammar ToGrammar()
    {
        HashSet<string> nonTerminals = new(States);
        HashSet<string> terminals = new(Alphabet);
        Dictionary<string, List<string>> productions = new();

        foreach (var state in Transitions)
        {
            foreach (var transition in state.Value)
            {
                foreach (var target in transition.Value)
                {
                    if (!productions.ContainsKey(state.Key))
                        productions[state.Key] = new List<string>();

                    productions[state.Key].Add(transition.Key + target);

                    if (FinalStates.Contains(target))
                        productions[state.Key].Add(transition.Key);
                }
            }
        }

        return new Grammar(nonTerminals, terminals, productions, StartState);
    }

    private FiniteAutomaton RenameStates(FiniteAutomaton dfa)
    {
        var stateNames = dfa.States.ToList();
        var renamedStates = new Dictionary<string, string>();
        char newStateChar = 'A';

        // Assign new names (A, B, C, ...) to each state
        foreach (var state in stateNames)
        {
            renamedStates[state] = newStateChar.ToString();
            newStateChar++;
        }

        // Rename transitions
        var renamedTransitions = new Dictionary<string, Dictionary<string, HashSet<string>>>();
        foreach (var (oldState, transitions) in dfa.Transitions)
        {
            string newState = renamedStates[oldState];
            renamedTransitions[newState] = new Dictionary<string, HashSet<string>>();

            foreach (var (symbol, targets) in transitions)
            {
                renamedTransitions[newState][symbol] =
                    new HashSet<string>(targets.Select(target => renamedStates[target]));
            }
        }

        // Rename final states and start state
        var renamedFinalStates = new HashSet<string>(dfa.FinalStates.Select(state => renamedStates[state]));
        string renamedStartState = renamedStates[dfa.StartState];

        return new FiniteAutomaton(
            new HashSet<string>(renamedStates.Values),
            dfa.Alphabet,
            renamedTransitions,
            renamedStartState,
            renamedFinalStates
        );
    }
}