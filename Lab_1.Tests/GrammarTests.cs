using Lab_1.Domain;

namespace Lab_1.Tests;

[TestFixture]
public class GrammarTests
{
    private Grammar _grammar;
    private FiniteAutomaton _finiteAutomaton;

    [SetUp]
    public void Setup()
    {
        var rules = new Dictionary<string, List<string>>
        {
            { "S", new List<string> { "aA", "bB" } },
            { "A", new List<string> { "bS", "cA", "aB" } },
            { "B", new List<string> { "aB", "b" } }
        };

        _grammar = new Grammar(
            new HashSet<string> { "S", "A", "B" },
            new HashSet<string> { "a", "b", "c" },
            rules,
            "S"
        );

        _finiteAutomaton = _grammar.ToFiniteAutomaton();
    }

    [Test]
    public void GenerateString_ShouldReturnValidString()
    {
        var generatedString = _grammar.GenerateStrings(1)[0];

        Console.WriteLine($"Generated String: {generatedString}");

        Assert.IsNotNull(generatedString, "Generated string should not be null");
        Assert.IsTrue(generatedString.Length > 0, "Generated string should not be empty");

        foreach (char c in generatedString)
        {
            Assert.IsTrue(_grammar.VT.Contains(c.ToString()), $"Character {c} should be in terminal set");
        }
    }


    [Test]
    public void GenerateStrings_ShouldReturnCorrectNumber()
    {
        var generatedStrings = _grammar.GenerateStrings(5);
        Assert.That(generatedStrings.Count, Is.EqualTo(5), "Should generate exactly 5 valid strings");
    }

    [Test]
    public void ToFiniteAutomaton_ShouldConvertCorrectly()
    {
        Assert.IsNotNull(_finiteAutomaton, "FiniteAutomaton should not be null");
        Assert.That(_finiteAutomaton.States.Count, Is.EqualTo(_grammar.VN.Count),
            "FA states count should match grammar non-terminals");
    }

    [Test]
    public void StringBelongsToLanguage_ShouldReturnTrueForValidStrings()
    {
        var validWords = _grammar.GenerateStrings(5);
        foreach (var word in validWords)
        {
            Assert.IsTrue(_finiteAutomaton.StringBelongsToLanguage(word),
                $"'{word}' should be accepted by the finite automaton.");
        }
    }

    [Test]
    public void StringBelongsToLanguage_ShouldReturnFalseForInvalidStrings()
    {
        var invalidWords = new List<string> { "xyz", "aaa", "cbb", "dd", "ee" };
        foreach (var word in invalidWords)
        {
            Assert.IsFalse(_finiteAutomaton.StringBelongsToLanguage(word),
                $"'{word}' should NOT be accepted by the finite automaton.");
        }
    }
}