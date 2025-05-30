# **Chomsky Normal Form Converter**

### **Course**: Formal Languages & Finite Automata  
### **Author**: Alexandru Rudoi  

📑 **[👉 Read the Full Report](https://github.com/AlexandruRudoi/LFA_Labs/blob/Lab_5/REPORT.md)**  

---

## **Overview**

This project implements a system that transforms any context-free grammar (CFG) into its equivalent **Chomsky Normal Form (CNF)**. CNF is a normalized structure for grammars in which each production rule conforms strictly to one of two patterns: a non-terminal producing exactly two non-terminals, or a non-terminal producing a single terminal.

Unlike CFGs that can contain nullable rules, unit rules, and long right-hand sides, CNF simplifies parsing and analysis by enforcing a binary format. This project reads any CFG from file, applies a multi-stage normalization pipeline, and outputs the equivalent CNF grammar in both terminal format and graphical representation.

The implementation was built using **C# (.NET 9)** following a clean architecture that separates the domain logic, transformation pipeline, and application entry point.

---

## **Objectives**

- Parse CFGs from structured `.txt` files, including non-terminals, terminals, and production rules.
- Eliminate ε-productions, unit productions, inaccessible and non-productive symbols.
- Decompose long right-hand sides into binary rules using intermediate symbols.
- Convert all terminals inside long productions to terminal wrappers (`T_a → a`).
- BONUS: Export the final CNF grammar as a `.dot` graph that can be rendered with Graphviz.

---

## **Implementation Details**

### **Architecture**

```
Lab_5/
│
├── Domain/               // Core grammar structure
│   ├── Grammar.cs
│   ├── Symbol.cs
│   └── Production.cs
│
├── Services/             // CNF conversion pipeline and DOT exporter
│   ├── CNFConverter.cs
│   └── GrammarLoader.cs
│
├── Application/          // Main entry point
│   ├── Program.cs
│   ├── Graphviz.cs
│   ├── resources/ 
│       ├── variant27.txt
│       ├── task.md
│       └── grammar.png
│

```

### **Conversion Pipeline**

The converter performs the following transformation stages in order:

1. **Epsilon Removal** – Eliminates nullable productions like `C → ε`.
2. **Renaming Removal** – Removes unit rules such as `A → B`.
3. **Inaccessible & Non-Productive Pruning** – Removes symbols that cannot derive strings or are not reachable.
4. **CNF Formatting** – Ensures each production is either `A → a` or `A → BC` using wrapper and intermediate symbols.

All transformations preserve the language generated by the original grammar.

---

## **Example Input**

Input grammar for **Variant 27** is stored in `variant27.txt`:

```plaintext
VN: S A B C D
VT: a b
S -> b A
S -> A C
A -> b S
A -> B C
A -> A b C
B -> C b a C
B -> a
B -> b S a
C -> ε
D -> A B
```

---

## **Sample CNF Output**

```plaintext
A    -> T_b S
     | A T_b
     | T_b T_a
     | T_a
     | T_b X2

B    -> T_b T_a
     | T_a
     | T_b X1

D    -> A B

S    -> T_b A
     | T_b S
     | A T_b
     | T_b T_a
     | T_a
     | T_b X3

T_a  -> a
T_b  -> b

X1   -> S T_a
X2   -> S T_a
X3   -> S T_a
```

All rules now satisfy CNF constraints: each rule has exactly two non-terminals or one terminal.

---

## **Graph Visualization**

The final CNF grammar is exported to `grammar.dot` and can be visualized using [Graphviz](https://graphviz.org/):

```bash
dot -Tpng grammar.dot -o grammar.png
```

### ✅ Example

![CNF Grammar Graph](/Lab_5.Application/resources/grammar.png)

This graph offers an overview of recursive expansions, intermediate decompositions, and terminal derivations.

---

## **How to Run the Project**

### **Prerequisites**

- .NET 9 SDK installed
- Optional: Rider or Visual Studio for development

### **Build and Execute**

```bash
dotnet build
dotnet run --project Lab_5/Lab_5.Application
```

This will read the grammar from `variant27.txt`, normalize it, and print the CNF result to the terminal.

---

## **Conclusion**

This project demonstrates the full transformation of arbitrary CFGs into Chomsky Normal Form using a clean, modular C# architecture. It supports any grammar format via text input and is suitable for theoretical exploration, automated analysis, and education.

By visualizing the CNF as a graph, it bridges the gap between abstract rules and their structural representation, enabling clearer understanding of derivation paths and simplification logic.

---

## **References**

- **John E. Hopcroft & Jeffrey D. Ullman** – Introduction to Automata Theory, Languages, and Computation  
- **Michael Sipser** – Introduction to the Theory of Computation  
- Graphviz – [graphviz.org](https://graphviz.org/)  
- Variant Task Sheet – Formal Languages & Finite Automata, UTM
