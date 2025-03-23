using Lab_3.Lexer;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("AION Lexer - Token Stream");

        string filePath = "D:\\Projects\\University\\LFA_Labs\\Lab_3\\Lab_3.Application\\aion_examples\\template1.aion"; // you can change this path

        if (!File.Exists(filePath))
        {
            Console.WriteLine($"File not found: {filePath}");
            return;
        }

        string source = File.ReadAllText(filePath);
        var lexer = new Lexer(source);
        var tokens = lexer.Tokenize();

        foreach (var token in tokens)
        {
            Console.WriteLine(token);
        }
    }
}