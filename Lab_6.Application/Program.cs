using Lab_6.Lexer;
using Lab_6.Parser;

namespace Lab_6.Application;

class Program
{
    static void Main(string[] args)
    {
        string path = "D:\\Projects\\University\\LFA_Labs\\Lab_6\\Lab_6.Application\\resources\\aion_examples\\example.aion"; // Replace with your real test file
        if (!File.Exists(path))
        {
            Console.WriteLine("File not found.");
            return;
        }

        string source = File.ReadAllText(path);

        // 1. Lexical Analysis
        var lexer = new Lexer.Lexer(source);
        var tokens = lexer.Tokenize();

        // (Optional) Print tokens
        Console.WriteLine("=== Tokens ===");
        foreach (var token in tokens)
            Console.WriteLine(token);

        // 2. Parsing
        var parser = new Parser.Parser(tokens);
        ProgramNode ast = parser.ParseProgram();

        // 3. AST Output
        Console.WriteLine("\n=== AST ===");
        ast.Print();
    }
}