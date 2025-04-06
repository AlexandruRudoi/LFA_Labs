using Lab_4.Domain;
using Lab_4.Services;

namespace Lab_4.Application
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var parser = new RegexParser();
            var generator = new RegexGenerator();

            // Variant 27 % 4 = 3 regexes
            var regexes = new List<string>
            {
                "O(P|Q|R)+2(3|4)",
                "A*B(C|D|E)F(G|H|I){2}",
                "J+K(L|M|N)*0?(P|Q){3}"
            };

            var tracedSamples = new List<(string Text, List<string> Trace)>();

            for (int i = 0; i < regexes.Count; i++)
            {
                Console.WriteLine($"=== Regex {i + 1}: {regexes[i]} ===");
                var tree = parser.Parse(regexes[i]);

                for (int j = 0; j < 10; j++)
                {
                    var trace = new List<string>();
                    var word = generator.GenerateWithTrace(tree, trace);
                    tracedSamples.Add((word, trace));
                    Console.WriteLine($"{i + 1}.{j + 1}: {word}");
                }

                Console.WriteLine();
            }

            while (true)
            {
                Console.WriteLine(
                    "Enter string index like '2.5' (regex 2, string 5) to see how it was generated, or 0 to exit:");
                var input = Console.ReadLine();

                if (input == "0" || input?.ToLower() == "exit")
                    break;

                var match = System.Text.RegularExpressions.Regex.Match(input ?? "", @"^(\d)\.(\d)$");
                if (match.Success)
                {
                    int regexIndex = int.Parse(match.Groups[1].Value) - 1;
                    int stringIndex = int.Parse(match.Groups[2].Value) - 1;
                    int flatIndex = regexIndex * 10 + stringIndex;

                    if (flatIndex >= 0 && flatIndex < tracedSamples.Count)
                    {
                        var (text, trace) = tracedSamples[flatIndex];
                        Console.WriteLine($"\n--- Generation trace for: {text} ---");
                        foreach (var step in trace)
                            Console.WriteLine(step);
                        Console.WriteLine();
                    }
                    else
                    {
                        Console.WriteLine("Out of range.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid format. Use format like '2.5' (regex 2, sample 5).");
                }
            }
        }
    }
}