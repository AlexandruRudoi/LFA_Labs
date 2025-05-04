using Lab_6.Domain;

namespace Lab_6.Parser;

public class Parser
{
    private readonly List<Token> _tokens;
    private int _current = 0;

    public Parser(List<Token> tokens)
    {
        _tokens = tokens;
    }

    public ProgramNode ParseProgram()
    {
        var program = new ProgramNode();

        while (!IsAtEnd())
        {
            var stmt = ParseStatement();
            if (stmt != null)
                program.Statements.Add(stmt);
        }

        return program;
    }

    private AstNode? ParseStatement()
    {
        if (Match(TokenType.Each))
            return ParseLoop();

        if (Match(TokenType.Import))
            return ParseImport();

        if (Match(TokenType.Include))
            return ParseInclude();

        if (Match(TokenType.Export))
            return ParseExport();

        if (Match(TokenType.Merge))
            return ParseMerge();

        if (Match(TokenType.Filter))
            return ParseFilter();

        if (Match(TokenType.If))
            return ParseConditional();

        if (Check(TokenType.Event))
        {
            // Consume the 'event' keyword first
            Advance();

            if (Check(TokenType.String))
                return ParseInlineEventWithoutId();

            if (Check(TokenType.Identifier) && PeekNext().Type == TokenType.LeftBrace)
                return ParseStructuredEvent();

            return ParseEvent(); // fallback for other variants
        }

        if (Check(TokenType.Task))
        {
            var lookahead = PeekAhead(1);
            if (lookahead.Type == TokenType.Named)
                return ParseInlineTask();
            return ParseTask();
        }

        if (Check(TokenType.Identifier) && PeekNext().Type == TokenType.Assign)
        {
            var third = PeekAhead(2);

            if (third.Type == TokenType.Event)
                return ParseInlineEvent();

            if (third.Type == TokenType.Task)
                return ParseInlineTask();

            if (third.Type == TokenType.Pomodoro)
                return ParseInlinePomodoro();

            return ParseAssignment();
        }

        Advance(); // skip unknown
        return null;
    }

    private EventNode ParseEvent()
    {
        var nameToken = Consume(TokenType.Identifier, "Expected identifier after 'event'");
        Consume(TokenType.LeftBrace, "Expected '{' after event name");

        var body = new List<AstNode>();
        while (!Check(TokenType.RightBrace) && !IsAtEnd())
        {
            var stmt = ParseStatement();
            if (stmt != null)
                body.Add(stmt);
        }

        Consume(TokenType.RightBrace, "Expected '}' after event body");

        return new EventNode
        {
            Name = nameToken.Lexeme,
            Body = body
        };
    }

    private TaskNode ParseTask()
    {
        Consume(TokenType.Task, "Expected 'task' keyword");

        Token nameToken;
        if (Match(TokenType.Identifier) || Match(TokenType.String))
        {
            nameToken = Previous();
        }
        else
        {
            throw new Exception($"[Line {Peek().Line}] Error: Expected task name after 'task'");
        }

        Consume(TokenType.LeftBrace, "Expected '{' after task name");

        var body = new List<AstNode>();
        while (!Check(TokenType.RightBrace) && !IsAtEnd())
        {
            var stmt = ParseStatement();
            if (stmt != null)
                body.Add(stmt);
        }

        Consume(TokenType.RightBrace, "Expected '}' after task body");

        return new TaskNode
        {
            Name = nameToken.Lexeme,
            Body = body
        };
    }

    private AssignmentNode ParseAssignment()
    {
        var idToken = Consume(TokenType.Identifier, "Expected variable name");
        Consume(TokenType.Assign, "Expected '='");

        var valueToken = ConsumeAny(
            TokenType.String,
            TokenType.Number,
            TokenType.Duration,
            TokenType.Identifier
        );

        Consume(TokenType.Semicolon, "Expected ';' after assignment");

        return new AssignmentNode
        {
            Identifier = idToken.Lexeme,
            Value = valueToken.Lexeme
        };
    }

    private InlineEventNode ParseInlineEvent()
    {
        var id = Consume(TokenType.Identifier, "Expected identifier");
        Consume(TokenType.Assign, "Expected '='");
        Consume(TokenType.Event, "Expected 'event' keyword");

        var title = Consume(TokenType.String, "Expected event title string");

        var node = new InlineEventNode
        {
            Identifier = id.Lexeme,
            Title = title.Lexeme
        };

        // === DATE SPECIFICATION ===
        if (Match(TokenType.On))
        {
            if (Check(TokenType.Number) && IsMonthToken(PeekNext().Type))
            {
                // on 12 March
                var day = Consume(TokenType.Number, "Expected day number").Lexeme;
                var monthToken = ConsumeAny(
                    TokenType.January, TokenType.February, TokenType.March, TokenType.April,
                    TokenType.May, TokenType.June, TokenType.July, TokenType.August,
                    TokenType.September, TokenType.October, TokenType.November, TokenType.December
                );
                node.Date = $"{day} {monthToken.Type}";
            }
            else if (Check(TokenType.Number) && PeekNext().Type == TokenType.Identifier)
            {
                // on 2nd Friday
                var ordinalNumber = Consume(TokenType.Number, "Expected ordinal number").Lexeme;
                var ordinalSuffix = Consume(TokenType.Identifier, "Expected ordinal suffix").Lexeme;
                var weekday = ConsumeAny(
                    TokenType.Monday, TokenType.Tuesday, TokenType.Wednesday,
                    TokenType.Thursday, TokenType.Friday, TokenType.Saturday, TokenType.Sunday
                );
                node.Ordinal = $"{ordinalNumber}{ordinalSuffix}";
                node.DayOfWeek = weekday.Type.ToString();
            }
            else if (Check(TokenType.Identifier) && Peek().Lexeme.ToLower() == "last")
            {
                // on last Friday
                Advance(); // consume 'last'
                var weekday = ConsumeAny(
                    TokenType.Monday, TokenType.Tuesday, TokenType.Wednesday,
                    TokenType.Thursday, TokenType.Friday, TokenType.Saturday, TokenType.Sunday
                );
                node.Ordinal = "last";
                node.DayOfWeek = weekday.Type.ToString();
            }
            else
            {
                // on Friday
                var weekday = ConsumeAny(
                    TokenType.Monday, TokenType.Tuesday, TokenType.Wednesday,
                    TokenType.Thursday, TokenType.Friday, TokenType.Saturday, TokenType.Sunday
                );
                node.DayOfWeek = weekday.Type.ToString();
            }
        }
        else if (Match(TokenType.Every))
        {
            // every Monday
            var weekday = ConsumeAny(
                TokenType.Monday, TokenType.Tuesday, TokenType.Wednesday,
                TokenType.Thursday, TokenType.Friday, TokenType.Saturday, TokenType.Sunday
            );
            node.DayOfWeek = weekday.Type.ToString();
            node.IsRecurring = true;
        }

        // === TIME RANGE ===
        if (Match(TokenType.From))
        {
            var startHour = Consume(TokenType.Number, "Expected start hour").Lexeme;
            Consume(TokenType.Colon, "Expected ':'");
            var startMin = Consume(TokenType.Number, "Expected start minutes").Lexeme;
            node.FromTime = $"{startHour}:{startMin}";

            Consume(TokenType.To, "Expected 'to'");

            var endHour = Consume(TokenType.Number, "Expected end hour").Lexeme;
            Consume(TokenType.Colon, "Expected ':'");
            var endMin = Consume(TokenType.Number, "Expected end minutes").Lexeme;
            node.ToTime = $"{endHour}:{endMin}";
        }

        // === LOCATION ===
        if (Match(TokenType.At))
        {
            var location = Consume(TokenType.String, "Expected location string");
            node.Location = location.Lexeme;
        }

        Consume(TokenType.Semicolon, "Expected ';' after inline event");
        return node;
    }

    private InlineTaskNode ParseInlineTask()
    {
        string identifier = "";

        // Optional assignment form: id = task named ...
        if (Check(TokenType.Identifier) && PeekNext().Type == TokenType.Assign)
        {
            var id = Consume(TokenType.Identifier, "Expected identifier");
            Consume(TokenType.Assign, "Expected '='");
            identifier = id.Lexeme;
        }

        Consume(TokenType.Task, "Expected 'task'");
        Consume(TokenType.Named, "Expected 'named'");
        var title = Consume(TokenType.String, "Expected task title");

        var node = new InlineTaskNode
        {
            Identifier = identifier,
            Title = title.Lexeme
        };

        if (Match(TokenType.At))
        {
            var hour = Consume(TokenType.Number, "Expected hour").Lexeme;
            Consume(TokenType.Colon, "Expected ':'");
            var min = Consume(TokenType.Number, "Expected minutes").Lexeme;
            node.Time = $"{hour}:{min}";
        }

        if (Match(TokenType.On))
        {
            Consume(TokenType.Each, "Expected 'each'");
            var dayToken = ConsumeAny(
                TokenType.Monday, TokenType.Tuesday, TokenType.Wednesday,
                TokenType.Thursday, TokenType.Friday, TokenType.Saturday, TokenType.Sunday
            );
            node.DayOfWeek = dayToken.Type.ToString();
        }

        if (Match(TokenType.With))
        {
            Consume(TokenType.Alarm, "Expected 'alarm'");
            node.HasAlarm = true;
        }

        Consume(TokenType.Semicolon, "Expected ';' after task");
        return node;
    }

    private InlinePomodoroNode ParseInlinePomodoro()
    {
        var id = Consume(TokenType.Identifier, "Expected identifier");
        Consume(TokenType.Assign, "Expected '='");
        Consume(TokenType.Pomodoro, "Expected 'pomodoro' keyword");

        var title = Consume(TokenType.String, "Expected pomodoro title");

        var node = new InlinePomodoroNode
        {
            Identifier = id.Lexeme,
            Title = title.Lexeme
        };

        if (Match(TokenType.At))
        {
            var hour = Consume(TokenType.Number, "Expected hour").Lexeme;
            Consume(TokenType.Colon, "Expected ':'");
            var min = Consume(TokenType.Number, "Expected minutes").Lexeme;
            node.StartTime = $"{hour}:{min}";
        }

        if (Match(TokenType.Repeat))
        {
            var count = Consume(TokenType.Number, "Expected repeat count").Lexeme;
            node.RepeatCount = int.Parse(count);
            Consume(TokenType.Times, "Expected 'times'");
        }

        if (Match(TokenType.With))
        {
            var duration = Consume(TokenType.Duration, "Expected break duration (e.g. 5m)");
            Consume(TokenType.Break, "Expected 'break'");
            node.BreakDuration = duration.Lexeme;
        }

        Consume(TokenType.Semicolon, "Expected ';' after pomodoro");
        return node;
    }

    private LoopNode ParseLoop()
    {
        var unitToken = ConsumeAny(TokenType.Identifier, TokenType.Day, TokenType.Month);
        Consume(TokenType.From, "Expected 'from'");

        var fromToken = ConsumeAny(TokenType.String, TokenType.Number);
        Consume(TokenType.To, "Expected 'to'");
        var toToken = ConsumeAny(TokenType.String, TokenType.Number);

        Consume(TokenType.LeftBrace, "Expected '{' to start loop body");

        var body = new List<AstNode>();
        while (!Check(TokenType.RightBrace) && !IsAtEnd())
        {
            var stmt = ParseStatement();
            if (stmt != null)
                body.Add(stmt);
        }

        Consume(TokenType.RightBrace, "Expected '}' after loop body");

        return new LoopNode
        {
            Unit = unitToken.Lexeme,
            From = fromToken.Lexeme,
            To = toToken.Lexeme,
            Body = body
        };
    }

    private InlineEventNode ParseInlineEventWithoutId()
    {
        var title = Consume(TokenType.String, "Expected event title string");

        var node = new InlineEventNode
        {
            Title = title.Lexeme
        };

        // === DATE SPECIFICATION ===
        if (Match(TokenType.On))
        {
            if (Check(TokenType.Number) && IsMonthToken(PeekNext().Type))
            {
                // on 12 March
                var day = Consume(TokenType.Number, "Expected day number").Lexeme;
                var monthToken = ConsumeAny(
                    TokenType.January, TokenType.February, TokenType.March, TokenType.April,
                    TokenType.May, TokenType.June, TokenType.July, TokenType.August,
                    TokenType.September, TokenType.October, TokenType.November, TokenType.December
                );
                node.Date = $"{day} {monthToken.Type}";
            }
            else if (Check(TokenType.Number) && PeekNext().Type == TokenType.Identifier)
            {
                // on 2nd Friday
                var ordinalNumber = Consume(TokenType.Number, "Expected ordinal number").Lexeme;
                var ordinalSuffix = Consume(TokenType.Identifier, "Expected ordinal suffix").Lexeme;
                var weekday = ConsumeAny(
                    TokenType.Monday, TokenType.Tuesday, TokenType.Wednesday,
                    TokenType.Thursday, TokenType.Friday, TokenType.Saturday, TokenType.Sunday
                );
                node.Ordinal = $"{ordinalNumber}{ordinalSuffix}";
                node.DayOfWeek = weekday.Type.ToString();
            }
            else if (Check(TokenType.Identifier) && Peek().Lexeme.ToLower() == "last")
            {
                // on last Friday
                Advance(); // consume 'last'
                var weekday = ConsumeAny(
                    TokenType.Monday, TokenType.Tuesday, TokenType.Wednesday,
                    TokenType.Thursday, TokenType.Friday, TokenType.Saturday, TokenType.Sunday
                );
                node.Ordinal = "last";
                node.DayOfWeek = weekday.Type.ToString();
            }
            else
            {
                // on Friday
                var weekday = ConsumeAny(
                    TokenType.Monday, TokenType.Tuesday, TokenType.Wednesday,
                    TokenType.Thursday, TokenType.Friday, TokenType.Saturday, TokenType.Sunday
                );
                node.DayOfWeek = weekday.Type.ToString();
            }
        }
        else if (Match(TokenType.Every))
        {
            // every Monday
            var weekday = ConsumeAny(
                TokenType.Monday, TokenType.Tuesday, TokenType.Wednesday,
                TokenType.Thursday, TokenType.Friday, TokenType.Saturday, TokenType.Sunday
            );
            node.DayOfWeek = weekday.Type.ToString();
            node.IsRecurring = true;
        }

        // === TIME RANGE ===
        if (Match(TokenType.From))
        {
            var startHour = Consume(TokenType.Number, "Expected start hour").Lexeme;
            Consume(TokenType.Colon, "Expected ':'");
            var startMin = Consume(TokenType.Number, "Expected start minutes").Lexeme;
            node.FromTime = $"{startHour}:{startMin}";

            Consume(TokenType.To, "Expected 'to'");

            var endHour = Consume(TokenType.Number, "Expected end hour").Lexeme;
            Consume(TokenType.Colon, "Expected ':'");
            var endMin = Consume(TokenType.Number, "Expected end minutes").Lexeme;
            node.ToTime = $"{endHour}:{endMin}";
        }

        // === LOCATION ===
        if (Match(TokenType.At))
        {
            var location = Consume(TokenType.String, "Expected location string");
            node.Location = location.Lexeme;
        }

        Consume(TokenType.Semicolon, "Expected ';' after inline event");
        return node;
    }

    private ImportNode ParseImport()
    {
        var fileToken = Consume(TokenType.String, "Expected file path after 'import'");
        Consume(TokenType.As, "Expected 'as' after file path");
        var aliasToken = Consume(TokenType.Identifier, "Expected alias identifier after 'as'");
        Consume(TokenType.Semicolon, "Expected ';' after import statement");

        return new ImportNode
        {
            FilePath = fileToken.Lexeme,
            Alias = aliasToken.Lexeme
        };
    }

    private IncludeNode ParseInclude()
    {
        var target = Consume(TokenType.Identifier, "Expected identifier after 'include'");

        string? calendar = null;
        if (Match(TokenType.In))
        {
            calendar = Consume(TokenType.Identifier, "Expected calendar identifier").Lexeme;
        }

        Consume(TokenType.Semicolon, "Expected ';' after include");

        return new IncludeNode
        {
            Target = target.Lexeme,
            Calendar = calendar
        };
    }
    
    private ExportNode ParseExport()
    {
        string source;
        if (Match(TokenType.Default))
        {
            source = "default";
        }
        else
        {
            source = Consume(TokenType.Identifier, "Expected source to export").Lexeme;
        }

        string? outputFile = null;
        if (Match(TokenType.As))
        {
            var output = Consume(TokenType.String, "Expected filename string");
            outputFile = output.Lexeme;
        }

        Consume(TokenType.Semicolon, "Expected ';' after export");

        return new ExportNode
        {
            Source = source,
            OutputFile = outputFile
        };
    }

    private MergeNode ParseMerge()
    {
        var sources = new List<string>();
        do
        {
            var id = Consume(TokenType.Identifier, "Expected source identifier");
            sources.Add(id.Lexeme);
        } while (Match(TokenType.Comma));

        Consume(TokenType.Into, "Expected 'into'");
        var target = Consume(TokenType.Identifier, "Expected target identifier");
        Consume(TokenType.Semicolon, "Expected ';' after merge");

        return new MergeNode
        {
            Sources = sources,
            Target = target.Lexeme
        };
    }

    private FilterNode ParseFilter()
    {
        var source = Consume(TokenType.Identifier, "Expected source to filter").Lexeme;
        Consume(TokenType.Where, "Expected 'where'");

        var field = Consume(TokenType.Identifier, "Expected field name").Lexeme;

        string op;
        if (Match(TokenType.Equal)) op = "==";
        else if (Match(TokenType.NotEqual)) op = "!=";
        else throw new Exception($"[Line {Peek().Line}] Expected '==' or '!='");

        var valueToken = ConsumeAny(TokenType.String, TokenType.Identifier);
        var value = valueToken.Lexeme;

        Consume(TokenType.Into, "Expected 'into'");
        var target = Consume(TokenType.Identifier, "Expected target identifier").Lexeme;
        Consume(TokenType.Semicolon, "Expected ';' after filter");

        return new FilterNode
        {
            Source = source,
            Field = field,
            Operator = op,
            Value = value,
            Target = target
        };
    }

    private StructuredEventNode ParseStructuredEvent()
    {
        var idToken = Consume(TokenType.Identifier, "Expected identifier after 'event'");
        var node = new StructuredEventNode { Identifier = idToken.Lexeme };

        Consume(TokenType.LeftBrace, "Expected '{' after structured event identifier");

        while (!Check(TokenType.RightBrace) && !IsAtEnd())
        {
            var property = Consume(TokenType.Identifier, "Expected property name").Lexeme;
            Consume(TokenType.Colon, "Expected ':' after property name");

            switch (property)
            {
                case "name":
                    node.Name = Consume(TokenType.String, "Expected string for 'name'").Lexeme;
                    break;
                case "start":
                    var hour = Consume(TokenType.Number, "Expected start hour").Lexeme;
                    Consume(TokenType.Colon, "Expected ':'");
                    var min = Consume(TokenType.Number, "Expected minutes").Lexeme;
                    node.StartTime = $"{hour}:{min}";
                    break;
                case "duration":
                    node.Duration = Consume(TokenType.Duration, "Expected duration").Lexeme;
                    break;
                case "location":
                    node.Location = Consume(TokenType.String, "Expected location string").Lexeme;
                    break;
                default:
                    throw new Exception($"[Line {Peek().Line}] Unknown structured event property: {property}");
            }

            Consume(TokenType.Semicolon, "Expected ';' after property assignment");
        }

        Consume(TokenType.RightBrace, "Expected '}' at the end of structured event");
        return node;
    }

    private ConditionalNode ParseConditional()
    {
        var node = new ConditionalNode();

        // Parse initial 'if' condition
        var condition = ParseCondition();
        var ifBody = ParseBlock();
        node.IfElseChain.Add((condition, ifBody));

        // Parse optional 'else if' chains
        while (Match(TokenType.Else) && Match(TokenType.If))
        {
            var elifCondition = ParseCondition();
            var elifBody = ParseBlock();
            node.IfElseChain.Add((elifCondition, elifBody));
        }

        // Parse optional 'else'
        if (Match(TokenType.Else))
        {
            var elseBody = ParseBlock();
            node.ElseBody = elseBody;
        }

        return node;
    }

    private ConditionNode ParseCondition()
    {
        Consume(TokenType.LeftParen, "Expected '(' after 'if' or 'else if'");

        string left;

        if (Match(TokenType.Count))
        {
            Consume(TokenType.LeftParen, "Expected '(' after 'count'");
            var inner = ConsumeAny(TokenType.Monday, TokenType.Tuesday, TokenType.Wednesday,
                TokenType.Thursday, TokenType.Friday, TokenType.Saturday, TokenType.Sunday).Lexeme;
            Consume(TokenType.RightParen, "Expected ')' after count parameter");

            Consume(TokenType.In, "Expected 'in' after count function");
            var scopeToken = ConsumeAny(TokenType.Identifier, TokenType.Month);
            var scope = scopeToken.Lexeme;

            left = $"count({inner}) in {scope}";
        }
        else
        {
            left = Consume(TokenType.Identifier, "Expected left-hand identifier").Lexeme;
        }

        string op;
        if (Match(TokenType.Equal)) op = "==";
        else if (Match(TokenType.NotEqual)) op = "!=";
        else throw new Exception($"[Line {Peek().Line}] Expected comparison operator");

        var right = Consume(TokenType.Number, "Expected right-hand value").Lexeme;
        Consume(TokenType.RightParen, "Expected ')' to close condition");

        return new ConditionNode
        {
            Left = left,
            Operator = op,
            Right = right
        };
    }

    private List<AstNode> ParseBlock()
    {
        Consume(TokenType.LeftBrace, "Expected '{'");
        var body = new List<AstNode>();
        while (!Check(TokenType.RightBrace) && !IsAtEnd())
        {
            var stmt = ParseStatement();
            if (stmt != null)
                body.Add(stmt);
        }

        Consume(TokenType.RightBrace, "Expected '}'");
        return body;
    }

    private Token ConsumeAny(params TokenType[] types)
    {
        foreach (var type in types)
        {
            if (Check(type))
                return Advance();
        }

        throw new Exception($"[Line {Peek().Line}] Unexpected token: {Peek().Type}");
    }


    // === Helpers ===
    private bool Match(TokenType type)
    {
        if (Check(type))
        {
            Advance();
            return true;
        }

        return false;
    }

    private bool Check(TokenType type)
    {
        if (IsAtEnd()) return false;
        return Peek().Type == type;
    }

    private Token Advance()
    {
        if (!IsAtEnd()) _current++;
        return Previous();
    }

    private Token PeekNext()
    {
        if (_current + 1 >= _tokens.Count)
            return _tokens[^1]; // Last token (EOF)
        return _tokens[_current + 1];
    }

    private Token PeekAhead(int offset)
    {
        int index = _current + offset;
        return index < _tokens.Count ? _tokens[index] : _tokens[^1];
    }

    private bool IsAtEnd() => Peek().Type == TokenType.EndOfFile;

    private Token Peek() => _tokens[_current];
    private Token Previous() => _tokens[_current - 1];

    private Token Consume(TokenType type, string errorMessage)
    {
        if (Check(type)) return Advance();
        throw new Exception($"[Line {Peek().Line}] Error: {errorMessage}");
    }

    private bool IsMonthToken(TokenType type)
    {
        return type >= TokenType.January && type <= TokenType.December;
    }
}