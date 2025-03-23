# **AION Lexer and Tokenizer System**

### **Course**: Formal Languages & Finite Automata  
### **Author**: Alexandru Rudoi

📑 **[👉 Read the Full Report](https://github.com/AlexandruRudoi/LFA_Labs/blob/Lab_3/REPORT.md)**

---

## **Overview**

This project introduces a lexer for the **AION language**, a domain-specific language designed for managing tasks, events, pomodoros, and calendar-based logic. The lexer converts `.aion` source files into a structured stream of tokens used for further parsing and analysis.

The lexer was developed using **C# (.NET 9)** with a strong focus on clean architecture and maintainability. Core components are modularized into packages for lexing logic and domain utilities.

---

## **Objectives**

- Develop a lexer that tokenizes **AION DSL** syntax into categorized tokens.
- Support various token types such as **identifiers**, **keywords**, **durations**, **strings**, **symbols**, and **numbers**.
- Handle **single-line** and **multi-line comments** cleanly.
- Implement a clear separation of concerns using `Lexer`, `SourceReader`, and `Keywords` classes.
- Ensure token stream is **accurate**, **resilient**, and **position-aware** (with line/column tracking).

---

## **Implementation Details**

### **Architecture**

- **Lab_3.Lexer**: Contains the `Lexer` class, which reads source code and emits a stream of `Token` objects.
- **Lab_3.Domain**: Holds reusable utilities like `SourceReader` (tracks positions) and `Keywords` (shared constants).

### **Lexer Design**

The `Lexer` scans input character-by-character and applies rules based on the detected pattern:

- **Keywords**: Recognizes AION-specific language constructs like `event`, `task`, `each`, `repeat`, `export`, etc.
- **Identifiers**: Custom variable names or labels not reserved as keywords.
- **Strings**: Quoted content (e.g., `"Office"` or `"Buy Gift"`).
- **Durations**: Special postfix numeric types like `25m`, `2h`.
- **Numbers**: Standard decimals and time representations (e.g., `09`, `12.03`, `2025.03.25`).
- **Symbols**: Includes operators (`=`, `!=`, `==`, `>=`, etc.) and punctuation (`{`, `}`, `;`, `:`).
- **Comments**: Skips both `// line comments` and `/* block comments */`.

All tokens include metadata: `type`, `lexeme`, `line`, and `column`.

### **Sample Token Stream**

Given the AION input:

```aion
event "Team Sync" on 12 March from 11:00 to 12:00 at "Office";
```

The lexer will emit:

```
Keyword 'event' at (1, 1)
String 'Team Sync' at (1, 7)
Keyword 'on' at (1, 19)
Number '12' at (1, 22)
Keyword 'March' at (1, 25)
Keyword 'from' at (1, 31)
Number '11' at (1, 36)
Colon ':' at (1, 38)
Number '00' at (1, 39)
Keyword 'to' at (1, 42)
Number '12' at (1, 45)
Colon ':' at (1, 47)
Number '00' at (1, 48)
Keyword 'at' at (1, 51)
String 'Office' at (1, 54)
Semicolon ';' at (1, 62)
```

---

## **Execution Results**

The lexer correctly tokenizes a variety of AION features, including:

- Structured events with `event { ... }` blocks
- Loops using `each day from ... to ... { ... }`
- Conditional logic like:
  ```aion
  if (count(Friday) in month == 4) { ... } else { ... }
  ```
- Export commands and merging calendar sources
- Inline time and date arithmetic

✔ Handles time expressions like `21:00`  
✔ Supports block comments and nested structures  
✔ Accurate line/column output for debugging

---

## **How to Run the Project**

### **Prerequisites**

- .NET 9 SDK
- Optional: JetBrains Rider or Visual Studio 2022+

### **Build and Run**

```bash
dotnet build
dotnet run --project Lab_3/Lab_3.Application
```

### **Edit AION Source File**

Edit the file at:

```
Lab_3.Application/resources/aion_examples/main.aion
```

Then re-run the lexer to view updated tokens.

---

## **Conclusion**

This lexer lays a solid foundation for parsing the AION DSL, enabling structured task and event planning logic. With support for advanced date/time patterns, loops, and comments, it serves as a reliable front-end for future parser or interpreter integration.

---

## **References**

- **Michael Sipser** – Introduction to the Theory of Computation  
- **C# .NET 9 Documentation** – [docs.microsoft.com](https://docs.microsoft.com/en-us/dotnet/)  
- AION DSL Syntax Draft – Internal Specification  