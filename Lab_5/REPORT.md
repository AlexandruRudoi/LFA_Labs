# **Chomsky Normal Form: Grammar Normalization and Simplification**

### **Course**: Formal Languages & Finite Automata  
### **Author**: Alexandru Rudoi  

---

## **Theory**

Context-free grammars (CFGs) are formal systems used to define languages beyond the capabilities of regular expressions. They consist of a finite set of non-terminal symbols, terminal symbols, a start symbol, and a set of production rules. While CFGs are expressive and intuitive, their flexibility can make parsing, optimization, and analysis more difficult.

To address this, grammars are often transformed into standardized formats. One such form is the **Chomsky Normal Form (CNF)**, where each production rule must conform to one of two strict patterns: either a non-terminal producing exactly two non-terminals (`A → BC`), or a non-terminal producing a single terminal (`A → a`). Optionally, the start symbol may produce the empty string (`S → ε`) if the language includes the empty word.

The CNF transformation simplifies parsing algorithms such as the CYK (Cocke-Younger-Kasami) algorithm and facilitates automated analysis. Converting an arbitrary CFG to CNF involves several steps: eliminating ε-productions, removing unit productions (renaming rules), discarding non-productive or inaccessible symbols, and finally reformatting the grammar to fit the CNF constraints.

---

## **Objectives**

The purpose of this lab was to implement a complete and automatic converter that transforms any input context-free grammar into its Chomsky Normal Form equivalent. The system was required to:

- Accept any valid CFG input in a structured text format.
- Eliminate nullable symbols, unit rules, non-productive, and unreachable non-terminals as needed.
- Convert all productions into the CNF-compliant format: either `A → BC` or `A → a`.
- Preserve all semantics of the original grammar in the transformed form.
- Provide a clean terminal output and optionally export a visual graph of the grammar structure.
- Bonus: support generalized input from file rather than hardcoded productions, allowing any CFG to be tested through external resources.

---

## **Implementation Description**

The system was developed using C# under the .NET 9 platform, structured around three logical layers: **Domain**, **Services**, and **Application**. The `Grammar` model encapsulates non-terminals, terminals, productions, and the start symbol. The `CNFConverter` service operates directly on this model and applies the series of transformations required for CNF compliance.

### **Input Format and Loader**

The grammar is read from a structured text file. A sample input file for **Variant 27** looks like this:

```
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

This format includes the set of non-terminals (`VN`), terminals (`VT`), and a list of production rules. The loader parses the file, splits the rules, and constructs the `Grammar` object accordingly.

### **Conversion Pipeline**

The conversion is done step by step within the `CNFConverter.Normalize()` method:

1. **Epsilon Elimination**: Detects nullable non-terminals (like `C → ε`) and creates new productions by omitting such symbols in all contexts except where their presence is mandatory.
2. **Renaming Removal**: Eliminates unit rules like `A → B` by transitive closure and replacement with the derived rules of the target symbol.
3. **Non-productive Elimination**: Removes non-terminals that cannot derive terminal strings.
4. **CNF Restructuring**: Transforms long right-hand sides into binary sequences using new intermediate symbols (`X1`, `X2`, etc.), and replaces terminal appearances inside longer RHS with wrapper non-terminals (`T_a → a`, `T_b → b`).

### **Output Representation**

After normalization, the productions are grouped by left-hand side and printed in a concise form. Additionally, a method was implemented to export the resulting grammar as a Graphviz `.dot` file, which can be rendered as a visual diagram using external tools.

---

## **Results and Analysis**

The original grammar for Variant 27 features a variety of context-free structures, including ε-productions, long right-hand sides, and terminal symbols interleaved with non-terminals. The grammar is defined over non-terminals `S`, `A`, `B`, `C`, `D` and terminals `a`, `b`. Its production rules include the following:

```
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

This grammar presents several challenges for Chomsky Normal Form transformation. First, the production `C → ε` introduces nullability, requiring all productions where `C` appears to be duplicated with and without `C`. Several rules, such as `A → A b C` and `B → C b a C`, contain more than two symbols on the right-hand side, which violates CNF restrictions and must be decomposed. Additionally, terminal symbols like `a` and `b` appear within longer productions and must be replaced with new non-terminals that represent these terminals.

The CNF conversion proceeds in several systematic steps. Nullability is eliminated by computing all nullable non-terminals and regenerating production rules by selectively omitting nullable symbols. Unit productions are removed by computing the transitive closure of renaming chains and substituting all reachable derivations directly. To comply with CNF's requirement that each right-hand side must contain at most two non-terminals or exactly one terminal, long rules are decomposed using newly generated intermediate symbols such as `X1`, `X2`, and `X3`. For example, the rule `B → C b a C` leads to a chain of binary productions where `b` and `a` are replaced by new non-terminals `T_b` and `T_a`, and intermediate non-terminals handle grouping.

Terminal symbols that appear within long productions are replaced by corresponding wrapper variables. For instance, `T_a → a` and `T_b → b` are introduced, and any longer production involving `a` or `b` uses `T_a` or `T_b` instead. This ensures that all non-terminal productions conform to the `A → BC` form. Finally, rules such as `D → A B`, which already match the CNF pattern, are preserved without modification.

The final CNF grammar derived from Variant 27 is:

```
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

Each production strictly adheres to the Chomsky Normal Form structure. The transformation maintains the language generated by the original grammar while producing a more regular and analyzable form. All intermediate variables and wrapper non-terminals were introduced systematically and only as required. The presence of visually similar productions (e.g., `X1`, `X2`, `X3`) reflects the recursive nature of certain rules and the necessity of unique binary decompositions. The correctness of the resulting grammar was verified both through theoretical conformance and manual inspection.

---

## **Graph Visualization**

As a final enhancement, the normalized grammar was exported into a `.dot` file and converted to a `.png` image using Graphviz. Each non-terminal is represented as a circular node, while terminal symbols `"a"` and `"b"` are styled as double circles with light gray backgrounds.

This visual representation offers a structural overview of how the grammar’s non-terminals expand and interact. It highlights recursive paths, intermediate breakdowns, and terminal-producing nodes—offering deeper insights into the shape and flow of the grammar.

### **Chomsky Normal Form Grammar Graph**

![CNF Grammar Graph](/Lab_5.Application/resources/grammar.png)

The graph illustrates how terminal wrappers like `T_a` and `T_b` connect to terminal outputs, while rules involving intermediate nodes such as `X1`, `X2`, and `X3` showcase decompositions of longer productions. The paths from the start symbol `S` demonstrate recursive structures and branching options clearly, aiding in both debugging and theoretical analysis.

---

## **Conclusion**

This lab achieved a complete and systematic conversion of a context-free grammar into Chomsky Normal Form, satisfying all theoretical requirements. The transformation pipeline was modular, extensible, and followed the standard algorithmic steps used in formal language theory. The resulting CNF grammar was validated for structure and correctness.

The optional visualization feature offered an intuitive perspective on the output, showcasing not just the correctness but the structure and elegance of CNF grammars. The implementation supports any CFG and provides a reusable toolset for analysis, teaching, or further formal language experiments.

---

## **References**

- **John E. Hopcroft & Jeffrey D. Ullman** – Introduction to Automata Theory, Languages, and Computation  
- **Michael Sipser** – Introduction to the Theory of Computation  
- Formal Languages & Finite Automata Course Materials, UTM  
- Graphviz – [graphviz.org](https://graphviz.org/)  