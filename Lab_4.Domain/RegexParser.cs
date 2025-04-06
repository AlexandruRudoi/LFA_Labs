using System.Text.RegularExpressions;

namespace Lab_4.Domain;

public class RegexParser
{
    private string _input = "";
    private int _pos = 0;

    public RegexNode Parse(string input)
    {
        _input = input;
        _pos = 0;
        return ParseExpression();
    }

    private RegexNode ParseExpression()
    {
        var nodes = new List<RegexNode>();

        while (_pos < _input.Length && _input[_pos] != ')')
        {
            if (_input[_pos] == '(')
            {
                _pos++; // skip '('
                var group = ParseExpression();
                Expect(')');
                _pos++; // skip ')'
                nodes.Add(ApplyQuantifier(new RegexNode(RegexNodeType.Group)
                {
                    Children = { group }
                }));
            }
            else if (_input[_pos] == '|')
            {
                _pos++; // skip '|'
                var right = ParseExpression();
                var alternation = new RegexNode(RegexNodeType.Alternation);
                alternation.Children.AddRange(nodes);
                alternation.Children.Add(right);
                return alternation;
            }
            else
            {
                nodes.Add(ApplyQuantifier(ParseLiteral()));
            }
        }

        if (nodes.Count == 1) return nodes[0];

        var concat = new RegexNode(RegexNodeType.Concat);
        concat.Children.AddRange(nodes);
        return concat;
    }

    private RegexNode ParseLiteral()
    {
        char current = _input[_pos];
        _pos++;

        return new RegexNode(RegexNodeType.Literal, current.ToString());
    }

    private RegexNode ApplyQuantifier(RegexNode node)
    {
        if (_pos >= _input.Length) return node;

        switch (_input[_pos])
        {
            case '*':
                _pos++;
                return new RegexNode(RegexNodeType.Repetition)
                {
                    MinRepeat = 0,
                    MaxRepeat = 5,
                    Children = { node }
                };
            case '+':
                _pos++;
                return new RegexNode(RegexNodeType.Repetition)
                {
                    MinRepeat = 1,
                    MaxRepeat = 5,
                    Children = { node }
                };
            case '?':
                _pos++;
                return new RegexNode(RegexNodeType.Repetition)
                {
                    MinRepeat = 0,
                    MaxRepeat = 1,
                    Children = { node }
                };
            case '{':
                var match = Regex.Match(_input.Substring(_pos), @"^\{(\d+)(,(\d+))?\}");
                if (match.Success)
                {
                    int min = int.Parse(match.Groups[1].Value);
                    int max = match.Groups[3].Success ? int.Parse(match.Groups[3].Value) : min;

                    _pos += match.Length;
                    return new RegexNode(RegexNodeType.Repetition)
                    {
                        MinRepeat = min,
                        MaxRepeat = max,
                        Children = { node }
                    };
                }

                break;
        }

        return node;
    }

    private void Expect(char expected)
    {
        if (_pos >= _input.Length || _input[_pos] != expected)
            throw new Exception($"Expected '{expected}' at position {_pos}");
    }
}