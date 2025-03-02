# Finite Automata

### Course: Formal Languages & Finite Automata
### Author: Titerez Vladislav

----

## Theory
A finite automaton is a mechanism used to represent processes of different kinds. It can be compared to a state machine as they both have similar structures and purpose as well. The word finite signifies the fact that an automaton comes with a starting and a set of final states. In other words, for process modeled by an automaton has a beginning and an ending.
- Based on the structure of an automaton, there are cases in which with one transition multiple states can be reached which causes non determinism to appear. In general, when talking about systems theory the word determinism characterizes how predictable a system is. If there are random variables involved, the system becomes stochastic or non deterministic.
- That being said, the automata can be classified as non-/deterministic, and there is in fact a possibility to reach determinism by following algorithms which modify the structure of the automaton.

A **Deterministic Finite Automaton (DFA)** and a **Nondeterministic Finite Automaton (NDFA or NFA)** are both finite state machines used in automata theory to recognize patterns in strings. A DFA has exactly one transition for each symbol from a given state, making its execution deterministic. In contrast, an NDFA allows multiple transitions for the same input or even ε-transitions (moves without consuming input), introducing nondeterminism. Despite these differences, both have equivalent computational power since any NDFA can be converted into a DFA. However, DFAs may require exponentially more states than their NFA counterparts, making NFAs more space-efficient in certain scenarios.
    
## Objectives:
Understand what an automaton is and what it can be used for.

Continuing the work in the same repository and the same project, the following need to be added: a. Provide a function in your grammar type/class that could classify the grammar based on Chomsky hierarchy.

b. For this you can use the variant from the previous lab.

According to your variant number (by universal convention it is register ID), get the finite automaton definition and do the following tasks:

a. Implement conversion of a finite automaton to a regular grammar.

b. Determine whether your FA is deterministic or non-deterministic.

c. Implement some functionality that would convert an NDFA to a DFA.

d. Represent the finite automaton graphically (Optional, and can be considered as a bonus point):

You can use external libraries, tools or APIs to generate the figures/diagrams.

Your program needs to gather and send the data about the automaton and the lib/tool/API return the visual representation.

Please consider that all elements of the task 3 can be done manually, writing a detailed report about how you've done the conversion and what changes have you introduced. In case if you'll be able to write a complete program that will take some finite automata and then convert it to the regular grammar - this will be a good bonus point.

## Implementation description

My implementation defines a Non-Deterministic Finite Automaton (NDFA) and converts it into a Deterministic Finite Automaton (DFA) while also classifying grammars based on the Chomsky hierarchy.

NDFA Definition
I define an NDFA using:

A set of states: Q = ["A", "B", "C"]
An input alphabet: sigma = ["a", "b"]
A transition function delta, which allows multiple possible next states for a given state and input
An initial state: "A"
A set of final states: F = ["C"]
I create an instance of FiniteAutomaton with these parameters and log it to the console.

NDFA to DFA Conversion
I implement the NFAtoDFAConverter class to convert an NDFA into a DFA:

It initializes with the NDFA object and extracts the alphabet.
The convert() method:
Computes ε-closures (even though this NDFA does not have ε-transitions).
Constructs DFA states iteratively by processing new state combinations.
Uses a queue (unprocessedStates) to keep track of newly discovered DFA states.
Defines DFA transitions using a state-to-string representation.
Determines final states by checking if any state in the DFA combination is a final state in the original NDFA.
I log the resulting DFA to the console.

Automaton Visualization
I visualize both the NDFA and the DFA:

drawAutomaton(ndfa, "nfaCanvas") renders the NDFA.
drawAutomaton2(dfa, "dfaCanvas") renders the DFA.
These functions likely draw the respective automata on HTML <canvas> elements.

Grammar Classification
I implement the classifyGrammar() function to categorize a given grammar according to the Chomsky hierarchy (Type 0 - Type 3):

Type 3 (Regular Grammar): Right-linear or left-linear productions.
Type 2 (Context-Free Grammar): Productions have a single non-terminal on the left side.
Type 1 (Context-Sensitive Grammar): The right side is at least as long as the left side.
Type 0 (Unrestricted Grammar): The most general form of a grammar.
The function iterates through the production rules and checks for violations of higher constraints, gradually eliminating possibilities:

If a left-hand side has multiple symbols, it cannot be Type 3 or Type 2.
If a right-hand side is shorter than the left, it violates Type 1.
The function logs violations for debugging.
Finally, it returns the strictest type that applies.


```js

let Q = ["A", "B", "C"];
let sigma = ["a", "b"];
let F = ["C"];
let delta = {
    "A": { "a": ["B", "A"] },
    "B": { "b": ["B"], "a": ["C"] },
    "C": { "b": ["C"], "a": ["A"] }
};

let ndfa = new FiniteAutomaton(Q, sigma, delta, "A", F);
console.log(ndfa);
let dfa = new NFAtoDFAConverter(ndfa);
console.log(dfa);


drawAutomaton(ndfa, "nfaCanvas");
drawAutomaton2(dfa, "dfaCanvas");

 classifyGrammar() {
        let type1, type3, type2, type0;
        type1 = type3 = type2 = type0 = true;
        for(let left in this.productions){
            if(left.length > 1){
                type3 = false; //For structure like aAB -> a
                type2 = false; //For structure like AB -> a
            }
            let startedWithNonTerminal = false, endedWithNonTerminal = false;
            for(let right of this.productions[left]){
                // If the production is non-empty and the right side is shorter than the left side, it's invalid.
                if (right !== '' && right.length < left.length) {
                    type1 = false;
                    console.log("Type 1 violation (shortened production): " + left + " -> " + right);
                }
                // If an empty production is found, it is only allowed for the start symbol (if defined).
                if (right === '' && left !== this.startSymbol) {
                    type1 = false;
                    console.log("Empty production is only allowed for the start symbol: " + left + " -> " + right);
                }

                let startsWithNonTerminal = this.nonTerminals.has(right[0]);
                let endsWithNonTerminal = this.nonTerminals.has(right[right.length-1]);
                const nonTerminalCount = [...right].filter(symbol => this.nonTerminals.has(symbol)).length;
                if(right === ''){
                    continue;
                };
                if(right.length === 1){
                    if(!this.terminals.has(right)){
                        type3 = false;    //For structure like A -> B
                        console.log("Vn -> Vn");
                    } 
                } else if(!((startsWithNonTerminal || endsWithNonTerminal) && nonTerminalCount === 1)){
                    type3 = false;     //For structure like A -> aBa or A -> BaB
                    console.log("Vn -> VnVn");
                }
                if(startsWithNonTerminal){
                    startedWithNonTerminal = true;
                } else if(endsWithNonTerminal){
                    endedWithNonTerminal = true;
                }
                if(startedWithNonTerminal && endedWithNonTerminal){
                    type3 = false;              // For situations when A -> aB and A -> Ba
                    console.log("Vn -> VnVt/VtVn");
                }
            }
        }

        console.log("Type 3: " + type3 + ", Type 2: " + type2 + ", Type 1: " + type1 + ", Type 0: " + type0);
        switch(true){
            case type3:
                return "Type 3";
            case type2:
                return "Type 2";
            case type1:
                return "Type 1";
            default:
                return "Type 0";
        }
    }

export default class NFAtoDFAConverter {
    constructor(nfa) {
        this.nfa = nfa;
        this.alphabet = [...nfa.alphabet]; 
        this.states = [];
        this.transitions = {};
        this.finalStates = new Set();
        this.convert();
        this.initialState = nfa.initialState;
    }

    convert() {
        const initialDFAState = this.epsilonClosure(new Set([this.nfa.initialState]));
        this.states.push(initialDFAState);
        let unprocessedStates = [initialDFAState];

        while (unprocessedStates.length > 0) {
            const currentState = unprocessedStates.pop();
            const stateKey = this.stateToString(currentState);
            this.transitions[stateKey] = {};

            for (let symbol of this.alphabet) {
                const nextState = this.epsilonClosure(this.move(currentState, symbol));
                if (nextState.size > 0) {
                    const nextStateKey = this.stateToString(nextState);
                    if (!this.states.some(s => this.stateToString(s) === nextStateKey)) {
                        this.states.push(nextState);
                        unprocessedStates.push(nextState);
                    }
            
                    if (!this.transitions[stateKey][symbol]) {
                        this.transitions[stateKey][symbol] = [];
                    }
                    this.transitions[stateKey][symbol].push(nextStateKey);
                }
            }
            

            if ([...currentState].some(state => this.nfa.finalStates.has(state))) {
                this.finalStates.add(stateKey);
            }
        }
    }

    epsilonClosure(states) {
        return new Set(states); 
    }

    move(states, symbol) {
        let result = new Set();
        for (let state of states) {
            if (this.nfa.transitions[state] && this.nfa.transitions[state][symbol]) {
                this.nfa.transitions[state][symbol].forEach(s => result.add(s));
            }
        }
        return result;
    }

    stateToString(stateSet) {
        return [...stateSet].sort().join(',');
    }
}

```
## Conclusions / Screenshots / Results
This project provided a valuable opportunity to bridge theory with practical implementation in the realm of finite automata. Converting an NFA to a DFA proved challenging, particularly in managing state representations and correctly mapping transitions, yet these hurdles spurred innovative solutions and deeper insights. The successful transformation process not only reinforced the theoretical equivalence between nondeterministic and deterministic models but also underscored the complexities hidden within seemingly simple automata.

Furthermore, the additional features—such as the conversion of finite automata to regular grammar and the graphical representation—enhanced the overall understanding of formal languages. The visual output, in particular, helped to verify the correctness of the conversion algorithm, making abstract concepts more tangible.

In summary, despite initial difficulties, persistent debugging and iterative improvements led to a robust and functional system. This accomplishment has significantly deepened my understanding of automata theory, formal grammars, and computational models, laying a strong foundation for further exploration in the field.

Developing the DFA converter was a challenging yet rewarding experience. I encountered several obstacles, particularly in managing state representations and ensuring the transitions were accurately mapped. Converting the nondeterministic elements into a deterministic structure required careful debugging and innovative solutions. Despite the difficulties, persistence and iterative improvements led to a successful implementation. In the end, overcoming these hurdles provided valuable insights into automata theory and system design, making the entire effort worthwhile.

![Снимок экрана 2025-02-23 202809](https://github.com/user-attachments/assets/b9aebba8-693d-47b6-809d-774c97cc702f)


## References
- [https://github.com/filpatterson/DSL_laboratory_works/blob/master/1_RegularGrammars/task.md](https://en.wikipedia.org/wiki/Deterministic_finite_automaton)
- [https://en.wikipedia.org/wiki/Finite-state_machine](https://www.nfa.futures.org/)