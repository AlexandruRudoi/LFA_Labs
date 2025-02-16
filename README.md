# 🚀 Grammar to Finite Automaton Conversion

### **Course**: Formal Languages & Finite Automata  
### **Author**: Alexandru Rudoi  

📑 **[👉 Read the Full Report](https://github.com/AlexandruRudoi/LFA_Labs/blob/Lab_1/REPORT.md)**  

---

## 📖 **Overview**
This project demonstrates the conversion of a **formal grammar** into a **finite automaton**. It includes:
- **Grammar Definition**: Defines non-terminals, terminals, production rules, and a start symbol.
- **Finite Automaton Implementation**: Converts the grammar into an automaton with states and transitions.
- **Validation**: Generates valid strings and checks whether words belong to the language.

---

## 🎯 **Objectives**
- Implement a **Grammar class** to define and generate valid words.
- Implement a **FiniteAutomaton class** that validates input strings.
- Convert a **grammar into a finite automaton**.
- Verify **correct & incorrect words** using the automaton.

---

## 🔧 **Implementation Details**
### 🏗 **Grammar Definition**
The grammar follows **right-linear production rules**, allowing easy conversion to a **finite automaton**.  
Example:
```
S → aA
S → bB
A → bS
A → cA
A → aB
B → aB
B → b
```

### 🔄 **Conversion to Finite Automaton**
- **States (Q)** → `{S, A, B}`
- **Alphabet (Σ)** → `{a, b, c}`
- **Transitions (δ)** → Defined based on production rules.
- **Start State (q₀)** → `S`
- **Final States (F)** → Determined dynamically.

### ✅ **Code Snippet (Checking Word Validity)**
```csharp
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
            return false; // No valid transitions

        currentStates = nextStates;
    }

    return currentStates.Any(state => FinalStates.Contains(state));
}
```

---

## 🖼 **Execution Results**
Below is a screenshot of the program's output:

![Program Output](Lab_1.Application/resources/Result.png)

---

## 🏁 **How to Run the Project**
1. **Clone the repository**:
   ```sh
   git clone https://github.com/AlexandruRudoi/LFA_Labs.git
   cd LFA_Labs
   ```
2. **Run the project**:
   ```sh
   dotnet run --project Lab_1/Lab_1.Application
   ```
3. **Run the tests**:
   ```sh
   dotnet test
   ```

---

## 🎓 **Conclusion**
This lab helped me develop a deeper understanding of **formal grammars** and **finite automata** by implementing the conversion from theoretical rules to an actual working model. Initially, I experimented with Python but later switched to C# as it provided a more structured and intuitive way to manage state transitions. The hands-on approach of testing valid and invalid words strengthened my grasp of automata theory and language processing.

---

## 📚 **References**
- **Michael Sipser** – Introduction to the Theory of Computation.  
- **Formal Language & Automata Theory** – Course Materials.  
- **C# .NET 9 Documentation** – [docs.microsoft.com](https://docs.microsoft.com/en-us/dotnet/)  
