# **Lexical Analysis in AION DSL: Tokenization for Calendar-based Language**

### **Course**: Formal Languages & Finite Automata  
### **Author**: Alexandru Rudoi  

---

## **Theory**

Lexical analysis is the first phase of the compilation process, responsible for breaking down source code into meaningful sequences of characters known as tokens. This process forms the bridge between raw text input and syntactic parsing, enabling the compiler to understand the structure and semantics of a programming language.

In the context of domain-specific languages (DSLs), a lexer is especially important for interpreting high-level instructions crafted to model real-world scenarios—in this case, a calendar-focused DSL named **AION**. AION is designed to express events, tasks, conditions, and schedules in a human-readable and programmable way.

The lexer identifies keywords, identifiers, numbers, strings, punctuation, and symbolic operators. It also handles more complex constructs such as time, durations, and dates, enabling features like scheduling rules, logical conditions, and calendar exports. Importantly, it filters out whitespace and comments to ensure that only meaningful tokens are processed in subsequent stages.

---

## **Objectives**

The main objective of this lab was to implement a robust lexical analyzer for the AION DSL. The system needed to recognize a wide variety of tokens defined in the language's BNF grammar, including structured statements, conditions, loop constructs, and custom time formats.

Beyond token recognition, the lexer also aimed to:

- Accurately track token positions (line and column) for error reporting.
- Handle both line and block comments (`//` and `/* */`).
- Allow clean and modular code through architectural separation of concerns.
- Enable easy extensibility for future DSL enhancements.

---

## **Implementation Description**

The implementation consists of two main layers: the **Lexer** and a reusable **Domain utility layer**, which abstracted the input reading and keyword handling.

### **Lexer Class**

The core logic is encapsulated within the `Lexer` class, which reads characters from the source and categorizes them into typed tokens. Each token includes metadata like its type, raw value, and source position.

The tokenization is driven by a character stream, handled through a `SourceReader` utility. The main loop scans each character and delegates to specialized methods like:

- `ReadIdentifierOrKeyword()`
- `ReadNumberOrDuration()`
- `ReadString()`
- `ReadSymbol()`

These methods build tokens from valid sequences of characters and return appropriate token types (e.g., `Keyword`, `Identifier`, `Number`, `Duration`, `String`, `Assign`, etc.).

### **SourceReader Utility**

The `SourceReader` class, placed in the `Lab_3.Domain` namespace, is responsible for managing the cursor over the source string. It exposes properties like `Current`, `Line`, `Column`, and utility methods such as `Advance()`, `Peek()`, and `Match()`.

By decoupling character stream logic from the lexer, the code remains easier to maintain, test, and reason about.

### **Keyword Extraction**

All language keywords were moved into a centralized `Keywords` class in the `Domain` layer. This not only improved the organization of the code but also makes it trivial to extend the keyword set in the future without polluting the core lexer.

### **Comment Support**

AION supports both single-line (`// ...`) and multi-line (`/* ... */`) comments. These are ignored during lexical analysis via the `SkipWhitespace()` method, which includes logic to detect and skip comment blocks, along with line tracking for error reporting.

---

## **Results and Analysis**

The final lexer correctly tokenized AION source files, handling all language constructs defined in the grammar. Input files containing a wide range of syntax—such as structured events, conditional logic, loop constructs, time and duration declarations, and calendar exports—were parsed without errors.

**Example Input:**

```aion
import "calendar.ai" as mycal;

event "Team Sync" on 12 March from 11:00 to 12:00 at "Office";

task named "Write Report" at 14:00 on each Monday with alarm;

if (count(Monday) in month >= 4) {
    export mycal as "final.ical";
} else {
    export default as "fallback.ical";
}
```

**Lexer Output:**

```
Keyword 'import' at (1, 1)
String 'calendar.ai' at (1, 8)
Keyword 'as' at (1, 22)
Identifier 'mycal' at (1, 25)
Semicolon ';' at (1, 30)
Keyword 'event' at (3, 1)
String 'Team Sync' at (3, 7)
Keyword 'on' at (3, 19)
Number '12' at (3, 22)
Keyword 'March' at (3, 25)
Keyword 'from' at (3, 31)
Number '11' at (3, 36)
Colon ':' at (3, 38)
Number '00' at (3, 39)
Keyword 'to' at (3, 42)
Number '12' at (3, 45)
Colon ':' at (3, 47)
Number '00' at (3, 48)
Keyword 'at' at (3, 51)
String 'Office' at (3, 54)
Semicolon ';' at (3, 62)
Keyword 'task' at (5, 1)
Keyword 'named' at (5, 6)
String 'Write Report' at (5, 12)
Keyword 'at' at (5, 27)
Number '14' at (5, 30)
Colon ':' at (5, 32)
Number '00' at (5, 33)
Keyword 'on' at (5, 36)
Keyword 'each' at (5, 39)
Keyword 'Monday' at (5, 44)
Keyword 'with' at (5, 51)
Keyword 'alarm' at (5, 56)
Semicolon ';' at (5, 61)
Keyword 'if' at (7, 1)
LeftParen '(' at (7, 4)
Keyword 'count' at (7, 5)
LeftParen '(' at (7, 10)
Keyword 'Monday' at (7, 11)
RightParen ')' at (7, 17)
Keyword 'in' at (7, 19)
Keyword 'month' at (7, 22)
GreaterEqual '>=' at (7, 28)
Number '4' at (7, 31)
RightParen ')' at (7, 32)
LeftBrace '{' at (7, 34)
Keyword 'export' at (8, 5)
Identifier 'mycal' at (8, 12)
Keyword 'as' at (8, 18)
String 'final.ical' at (8, 21)
Semicolon ';' at (8, 33)
RightBrace '}' at (9, 1)
Keyword 'else' at (9, 3)
LeftBrace '{' at (9, 8)
Keyword 'export' at (10, 5)
Keyword 'default' at (10, 12)
Keyword 'as' at (10, 20)
String 'fallback.ical' at (10, 23)
Semicolon ';' at (10, 38)
RightBrace '}' at (11, 1)
EndOfFile '' at (12, 1)
```

Each token was reported with its line and column location, which enables better debugging and error tracing during parsing. Even more complex DSL constructs like `repeat`, `pomodoro`, and `filter` were handled successfully.

---

## **Conclusions**

This lab served as an excellent practical exercise in constructing a lexer for a real-world DSL. The architecture was designed with extensibility and clarity in mind, using helper classes and separation of concerns to keep responsibilities clean.

The lexer now forms a strong foundation for the next stages of the AION language pipeline—namely parsing, semantic analysis, and code generation for iCal or other formats. Thanks to the modular design, features such as enhanced diagnostics or additional token types can be added with minimal disruption to existing logic.

This implementation successfully demonstrates how fundamental concepts from compiler theory apply even to high-level, user-friendly domain-specific languages like AION.

---

## **References**

- **Michael Sipser** – Introduction to the Theory of Computation  
- **C# .NET 9 Documentation** – [docs.microsoft.com](https://docs.microsoft.com/en-us/dotnet/)  
- AION DSL Syntax Draft – Internal Specification  