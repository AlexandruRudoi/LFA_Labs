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

            // Variant 3 regexes
            var regexes = new List<string>
            {
                "O(P|Q|R)+2(3|4)",
                "A*B(C|D|E)F(G|H|I){2}",
                "J+K(L|M|N)*0?(P|Q){3}"
            };

            for (int i = 0; i < regexes.Count; i++)
            {
                Console.WriteLine($"=== Regex {i + 1}: {regexes[i]} ===");
                var tree = parser.Parse(regexes[i]);
                var samples = generator.GenerateMany(tree, 10);

                foreach (var s in samples)
                    Console.WriteLine(s);

                Console.WriteLine();
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}