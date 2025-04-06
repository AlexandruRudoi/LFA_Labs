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

    public string GenerateWithTrace(RegexNode node, List<string> trace)
    {
        return node.Type switch
        {
            RegexNodeType.Literal => TraceLiteral(node, trace),
            RegexNodeType.Concat => TraceConcat(node, trace),
            RegexNodeType.Alternation => TraceAlternation(node, trace),
            RegexNodeType.Group => GenerateWithTrace(node.Children[0], trace),
            RegexNodeType.Repetition => TraceRepetition(node, trace),
            _ => throw new NotImplementedException()
        };
    }

    private string TraceLiteral(RegexNode node, List<string> trace)
    {
        trace.Add($"Matched literal '{node.Value}'");
        return node.Value!;
    }

    private string TraceConcat(RegexNode node, List<string> trace)
    {
        string result = "";
        trace.Add("Begin concatenation:");
        foreach (var child in node.Children)
            result += GenerateWithTrace(child, trace);
        trace.Add("End concatenation");
        return result;
    }

    private string TraceAlternation(RegexNode node, List<string> trace)
    {
        var chosen = node.Children.PickRandom();
        trace.Add("Alternation: choosing one option...");
        return GenerateWithTrace(chosen, trace);
    }

    private string TraceRepetition(RegexNode node, List<string> trace)
    {
        int count = Utils.RandomInt(node.MinRepeat, node.MaxRepeat);
        trace.Add($"Repetition: repeating {count} time(s) between {node.MinRepeat} and {node.MaxRepeat}");
        string result = "";
        for (int i = 1; i <= count; i++)
        {
            trace.Add($"  -> Repetition #{i}:");
            result += GenerateWithTrace(node.Children[0], trace);
        }

        return result;
    }


    public IEnumerable<string> GenerateMany(RegexNode node, int count)
    {
        for (int i = 0; i < count; i++)
            yield return Generate(node);
    }

    public void ExplainSteps(RegexNode node, int indent = 0)
    {
        string prefix = new string(' ', indent * 2);
        switch (node.Type)
        {
            case RegexNodeType.Literal:
                Console.WriteLine($"{prefix}- Match literal '{node.Value}'");
                break;
            case RegexNodeType.Concat:
                Console.WriteLine($"{prefix}- Concatenation:");
                foreach (var child in node.Children)
                    ExplainSteps(child, indent + 1);
                break;
            case RegexNodeType.Alternation:
                Console.WriteLine($"{prefix}- Choose one of the alternatives:");
                foreach (var child in node.Children)
                    ExplainSteps(child, indent + 1);
                break;
            case RegexNodeType.Repetition:
                Console.WriteLine(
                    $"{prefix}- Repeat the following between {node.MinRepeat} and {node.MaxRepeat} times:");
                ExplainSteps(node.Children[0], indent + 1);
                break;
            case RegexNodeType.Group:
                Console.WriteLine($"{prefix}- Group:");
                ExplainSteps(node.Children[0], indent + 1);
                break;
        }
    }

    private string GenerateRepetition(RegexNode node)
    {
        int count = Utils.RandomInt(node.MinRepeat, node.MaxRepeat);
        var inner = Generate(node.Children[0]);
        return Utils.Repeat(inner, count);
    }
}