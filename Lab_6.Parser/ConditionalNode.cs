namespace Lab_6.Parser;

public class ConditionalNode : AstNode
{
    public List<(ConditionNode Condition, List<AstNode> Body)> IfElseChain { get; set; } = new();
    public List<AstNode>? ElseBody { get; set; }

    public override void Print(string indent = "")
    {
        foreach (var (condition, body) in IfElseChain)
        {
            Console.WriteLine($"{indent}If: {condition.Left} {condition.Operator} {condition.Right}");
            foreach (var stmt in body)
                stmt.Print(indent + "  ");
        }

        if (ElseBody != null)
        {
            Console.WriteLine($"{indent}else:");
            foreach (var stmt in ElseBody)
                stmt.Print(indent + "  ");
        }
    }
}