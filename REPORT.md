# **Regex String Generator: Dynamic Interpretation of Regular Expressions**

### **Course**: Formal Languages & Finite Automata

### **Author**: Alexandru Rudoi

---

## **Theory**

Regular expressions (regex) are symbolic notations used to define sets of strings over a given alphabet. In the theory of formal languages, regular expressions describe regular languages, which can be recognized by finite automata. They are foundational in lexical analysis, search engines, data validation, and many text-processing tasks.

A regular expression specifies a pattern through operators like alternation (`|`), repetition (`*`, `+`, `{n}`), and grouping (`(...)`). Rather than testing strings against a pattern (the common use case), this project inverts the perspective: it **interprets** a regular expression dynamically and **generates** strings that conform to it.

This inversion has theoretical relevance, as it shows the constructive side of regular languages—building examples rather than validating them. Practically, it can aid in unit testing, fuzzing, grammar generation, and understanding abstract regex behaviors.

---

## **Objectives**

The primary objective of this lab was to implement a system that reads regular expressions and dynamically produces valid strings matching the specified patterns. The system needed to:

- Parse and represent regular expressions as structured trees.
- Generate valid strings based on their structure, applying constraints like repetition limits.
- Avoid hardcoding expression logic, instead dynamically interpreting the given regex.
- Bonus: provide a detailed step-by-step explanation of how a specific string was generated from a regex.

The implementation had to support repetition operators, alternation, grouping, and literal characters, while preserving clarity and maintainability in the codebase.

---

## **Implementation Description**

The solution was developed using C# (.NET 9) and designed around a minimal, modular architecture. The system is divided into three conceptual layers: **Domain**, **Services**, and **Application**.

### **RegexNode and Tree Representation**

Regex expressions are first parsed into a tree of `RegexNode` elements, forming an abstract syntax tree (AST) representation of the expression. Each node is categorized by type: `Literal`, `Concat`, `Alternation`, `Repetition`, or `Group`. This tree structure allows recursive traversal and generation.

```csharp
RegexNodeType.Concat
 ├── Literal('O')
 ├── Repetition (1–5)
 │    └── Alternation: P | Q | R
 ├── Literal('2')
 └── Alternation: 3 | 4
```

### **Parser Design**

The parser operates recursively. It reads characters one-by-one, recognizes literals and operators, and constructs the AST accordingly. Grouping via parentheses and alternation (`|`) are handled with special care to preserve associativity and precedence. Repetition markers such as `*`, `+`, `?`, and `{n}` or `{n,m}` are translated into `Repetition` nodes with bounded limits.

To prevent infinite generation loops, all unbounded repetitions (`*`, `+`) are capped at **5 repetitions**.

### **String Generation Logic**

A service class called `RegexGenerator` traverses the AST and constructs strings. For each node, it makes random but valid decisions: selecting one option in alternations, choosing a repetition count within the allowed range, and recursively generating subcomponents. The process is fully dynamic and supports any valid regex constructed from the accepted grammar.

```csharp
Regex: O(P|Q|R)+2(3|4)

→ Might generate: OQQPP24, ORRPQ23, OPPP24
```

---

## **Results and Analysis**

The program was tested against the three regular expressions from **Variant 3**, assigned by index number modulo 4 (27 % 4 = 3). Each expression was parsed and used to generate 10 valid strings.

### **Input Expressions**

1. `O(P|Q|R)+2(3|4)`
2. `A*B(C|D|E)F(G|H|I){2}`
3. `J+K(L|M|N)*0?(P|Q){3}`

### **Example Output**

```
=== Regex 1 ===
1.1: ORRPP24
1.2: OPPQPP24
1.3: OP23
...

=== Regex 2 ===
2.1: AAABDFHG
2.2: ABEFIH
2.3: BDFGG
...

=== Regex 3 ===
3.1: JJJKLLQPQ
3.2: JJKLNLLPQP
3.3: JJJK0QQQ
...
```

All outputs were validated manually and via code inspection, confirming full conformance to the patterns described by each regex.

### **Step-by-Step Explanation**

A bonus feature allows users to request a generation trace for any output string. When enabled, the system replays the recursive decision path used to construct a string, detailing all branches, repetitions, and alternations.

#### **Example: Trace for `2.5: AABEFIH`**

```
Begin concatenation:
Repetition: repeating 2 times between 0 and 5
  → Repetition #1: 'A'
  → Repetition #2: 'A'
Matched literal 'B'
Alternation: chose 'E'
Matched literal 'F'
Repetition: repeating 2 times between 2 and 2
  → Repetition #1: 'I'
  → Repetition #2: 'H'
```

This feature supports educational understanding of regex mechanics and internal decision logic.

---

## **Conclusion**

This lab successfully demonstrates the dynamic interpretation of regular expressions through recursive parsing and string generation. Unlike traditional regex matchers, the system approaches the problem constructively—producing concrete instances of the abstract language described by a pattern.

All core objectives were met, including the parsing of grouped expressions, bounded repetition handling, and the generation of fully valid output strings. The bonus requirement was also fulfilled, with a clear trace mechanism explaining how strings are derived from regex input.

The system is modular, extensible, and well-suited for integration into fuzzing tools, test data generation pipelines, or even DSL prototypes that rely on rule-based string construction.

---

## **References**

- **Michael Sipser** – Introduction to the Theory of Computation
- **.NET Regular Expression Documentation** – [docs.microsoft.com](https://learn.microsoft.com/en-us/dotnet/standard/base-types/regular-expression-language-quick-reference)
- Class Notes – Formal Languages & Finite Automata, UTM
