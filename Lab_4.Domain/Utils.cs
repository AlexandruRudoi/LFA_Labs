namespace Lab_4.Domain;

public static class Utils
{
    private static readonly Random Random = new();

    public static T PickRandom<T>(this IList<T> list)
    {
        return list[Random.Next(list.Count)];
    }

    public static int RandomInt(int min, int max)
    {
        return Random.Next(min, max + 1);
    }

    public static string Repeat(string s, int count)
    {
        return string.Concat(Enumerable.Repeat(s, count));
    }
}