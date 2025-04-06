using Lab_4.Domain;

namespace Lab_4.Services;

public class RegexGenerator
{
    public string Generate(RegexNode node)
    {
        return node.Type switch
        {
            RegexNodeType.Literal => node.Value!,
            RegexNodeType.Concat => string.Concat(node.Children.Select(Generate)),
            RegexNodeType.Alternation => Generate(node.Children.PickRandom()),
            RegexNodeType.Group => Generate(node.Children[0]),
            RegexNodeType.Repetition => GenerateRepetition(node),
            _ => throw new NotImplementedException($"Unhandled node type: {node.Type}")
        };
    }

    private string GenerateRepetition(RegexNode node)
    {
        int count = Utils.RandomInt(node.MinRepeat, node.MaxRepeat);
        var inner = Generate(node.Children[0]);
        return Utils.Repeat(inner, count);
    }

    public IEnumerable<string> GenerateMany(RegexNode node, int count)
    {
        for (int i = 0; i < count; i++)
            yield return Generate(node);
    }
}