# **Parser & Abstract Syntax Tree Construction**

### **Course**: Formal Languages & Finite Automata

### **Author**: Alexandru Rudoi

📑 **[👉 Read the Full Report](https://github.com/AlexandruRudoi/LFA_Labs/blob/Lab_6/REPORT.md)**

---

## **Overview**

This project implements a parser that processes a custom calendar Domain-Specific Language (DSL) and generates an Abstract Syntax Tree (AST) to represent the structure of the input. The AST is a hierarchical representation of the syntactic constructs found in the DSL, which can later be used for further processing such as scheduling or generating calendar events in formats like ICS (iCalendar). This parser is designed to handle constructs such as event definitions, task assignments, inline tasks, conditionals, and loops.

The system follows a clean architecture, with separate layers for the lexer, parser, and the AST data structures. The implementation was built using **C# (.NET 9)**, following a modular and extensible design.

---

## **Objectives**

- Implement a tokenizer (lexer) that categorizes tokens in the calendar DSL.
- Build a parser that constructs an Abstract Syntax Tree (AST) from the tokenized input.
- Implement various constructs such as events, tasks, pomodoros, loops, and conditionals in the AST.
- Use recursive-descent parsing techniques to process the input text and generate the AST.

---

## **Implementation Details**

### **Architecture**

```
Lab_6/
│
├── Domain/               // Core AST structures and token types
│   ├── TokenType.cs      // Defines token types for lexical 
│   ├── ASTNode.cs        // Defines AST node structure
│   ├── Keywords.cs       // Defines keywords
│   └── Token.cs          // Represents individual tokens
│
├── Lexer/             // Lexer logic
│   └── Lexer.cs          // Tokenization and lexical analysis
│
├── Parser/             // Parser logic
│   ├── Parser.cs         // Parsing and AST construction
│   └── ...
│
├── Application/          // Main entry point and execution logic
│   ├── Program.cs        // Executes the parsing and AST 
│   └── resources/       // Executes the parsing and AST 
│       ├── task.md
│       ├── grammar_BNF.md
│       ├── grammar_table.md
│       └── aion_examples/
│            └── example.aion
│ 
```

### **Parser Implementation**

The parser utilizes a recursive-descent approach, where each function corresponds to a non-terminal in the grammar. The parser functions consume tokens and recursively build AST nodes. For instance, parsing an event would look for the `event` keyword, followed by event details such as name, start time, and location.

Each token type is categorized using a `TokenType` enum, which helps in the identification and classification of tokens. These include identifiers, strings, durations, numbers, keywords (such as `event`, `task`, `import`, etc.), and symbols (like `=`, `;`, `(`, `)`, etc.).

---

## **Example Input**

Input for a sample calendar DSL file:

```text
import "personal.ics" as personal;
import "work.ics" as work;

date1 = "12.03";
time1 = "09:00";
duration1 = "2h";
loc = "Office";

event find_gift {
  name: "Buy Gift";
  start: 10:30;
  duration: 2h;
  location: "Mall";
}

task Daily Summary at 21:00 on Monday with alarm;
```

---

## **Sample AST Output**

After parsing the input, the AST generated for the example would look as follows:

```text
Program
  Import: "personal.ics" as personal
  Import: "work.ics" as work
  Assignment: date1 = "12.03"
  Assignment: time1 = "09:00"
  Assignment: duration1 = "2h"
  Assignment: loc = "Office"
  Structured Event: find_gift
    Name: "Buy Gift"
    Start: 10:30
    Duration: 2h
    Location: "Mall"
  Inline Task: "Daily Summary"
    Time: 21:00
    Day: Monday
    Alarm: true
```

This AST captures the entire structure of the calendar DSL, representing imports, assignments, events, and tasks. It follows a hierarchical structure where each element is associated with its corresponding details such as event names, times, and locations.

---

## **Graph Visualization**

The AST is also represented as a **Graphviz** diagram to provide a visual understanding of the abstract structure. The final AST is exported to a `.dot` file, which can be rendered using Graphviz into a graphical representation.

```bash
dot -Tpng grammar.dot -o grammar.png
```

### ✅ Example

![AST Graph](./resources/grammar.png)

This graph visually represents the relationships between the different elements of the DSL, such as events, tasks, and assignments. Each node corresponds to an element in the AST, and edges represent the syntactic relationships between them.

---

## **How to Run the Project**

### **Prerequisites**

- .NET 9 SDK installed
- Optional: Rider or Visual Studio for development

### **Build and Execute**

```bash
dotnet build
dotnet run --project Lab_6/Lab_6.Application
```

This will read the input DSL file, parse it, and output the AST both in terminal format and as a graphical representation.

---

## **Conclusion**

This project demonstrates the construction of a parser that extracts syntactic information from a custom calendar DSL and generates an Abstract Syntax Tree (AST). By implementing a recursive-descent parser and categorizing tokens using a `TokenType` enum, the system successfully processes various constructs, including events, tasks, and pomodoros, while generating a hierarchical AST representation.

The modular architecture of the system ensures extensibility, allowing the addition of more complex constructs or enhancements to the grammar in the future. The integration of Graphviz for visual representation further enhances the understanding of the grammar structure and parsing process.

---

## **References**

- **John E. Hopcroft & Jeffrey D. Ullman** – _Introduction to Automata Theory, Languages, and Computation_
- **Michael Sipser** – _Introduction to the Theory of Computation_
- Formal Languages & Finite Automata Course Materials, UTM
- Graphviz – [graphviz.org](https://graphviz.org/)

