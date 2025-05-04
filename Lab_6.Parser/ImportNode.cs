namespace Lab_6.Parser;

public class ImportNode : AstNode
{
    public string FilePath { get; set; } = "";
    public string Alias { get; set; } = "";

    public override void Print(string indent = "")
    {
        Console.WriteLine($"{indent}Import: \"{FilePath}\" as {Alias}");
    }
}