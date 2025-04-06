# **Regex String Generator**

### **Course**: Formal Languages & Finite Automata

### **Author**: Alexandru Rudoi

📑 **[👉 Read the Full Report](https://github.com/AlexandruRudoi/LFA_Labs/blob/Lab_4/REPORT.md)**  

---

## **Overview**

This project implements a system that dynamically interprets regular expressions and generates strings that conform to their structure. Unlike traditional regex matchers that test if a string fits a pattern, this generator does the reverse: it takes a regular expression as input and produces valid output strings that match it.

The solution was built using **C# (.NET 9)** with a clean architecture that separates concerns across parsing logic, node representation, generation logic, and user interaction.

---

## **Objectives**

- Dynamically parse complex regular expressions and build an abstract syntax tree (AST).
- Generate valid strings that match each regex without hardcoding logic.
- Handle common regex constructs like `*`, `+`, `?`, `{n}`, `{n,m}`, grouping, alternation, and concatenation.
- Impose a configurable upper limit (default 5) on unbounded repetitions for safe generation.
- BONUS: Provide a trace feature that explains how a specific string was generated step by step.

---

## **Implementation Details**

### **Architecture**

```
RegexGenerator/
│
├── Domain/             // RegexNode structure, parser, and utilities
│   ├── RegexNode.cs
│   ├── RegexParser.cs
│   └── Utils.cs
│
├── Services/           // Core logic for generating and tracing strings
│   └── RegexGenerator.cs
│
├── Application/        // Entry point and user interaction
│   └── Program.cs
```

### **Supported Features**

- **Regex Operators**: `*`, `+`, `?`, `{n}`, `{n,m}`, `|`, grouping via `(...)`
- **Repetition Limit**: Unbounded operators (`*`, `+`) are limited to max 5 repetitions
- **Tree-Based Generation**: Recursively builds strings from a regex tree
- **Trace Explanation**: Logs each step used to construct a generated string

---

## **Example Input**

The program uses the three regexes from **Variant 3** of the assignment:

```plaintext
1. O(P|Q|R)+2(3|4)
2. A*B(C|D|E)F(G|H|I){2}
3. J+K(L|M|N)*0?(P|Q){3}
```

---

## **Sample Output**

```
=== Regex 1 ===
1.1: OPQQP24
1.2: OQQQR23
...

=== Regex 2 ===
2.1: AAABCFII
2.2: AABDFGH
...

=== Regex 3 ===
3.1: JJJKLLQPQ
3.2: JJKLNLLPQP
...
```

### ✅ Example Trace (for 2.2)

```
--- Generation trace for: AABDFGH ---
Begin concatenation:
Repetition: 2 A's
Matched literal 'B'
Chose 'D' from (C|D|E)
Matched literal 'F'
Repetition: 2
→ G
→ H
End
```

---

## **How to Run the Project**

### **Prerequisites**

- .NET 9 SDK installed
- Optional: Visual Studio or JetBrains Rider

### **Build and Run**

```bash
dotnet build
dotnet run --project Lab_4/Lab_4.Application
```

---

## **How to Use**

When you run the project, you will see:

1. The program outputs 10 valid strings for each regex.
2. You can enter an index like `2.3` to view the **generation trace** for string #3 of regex #2.
3. Enter `0` or `exit` to close the program.

---

## **Conclusion**

This project demonstrates how regular expressions can be interpreted and used to construct valid output strings rather than just evaluate them. The system is modular, extensible, and useful both for practical purposes like test case generation and for educational goals, providing insight into how regex patterns actually behave when executed in reverse.

---

## **References**

- **Michael Sipser** – Introduction to the Theory of Computation
- [.NET Regex Documentation](https://learn.microsoft.com/en-us/dotnet/standard/base-types/regular-expression-language-quick-reference)
- Variant Task Sheet – Formal Languages & Finite Automata, UTM