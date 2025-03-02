namespace Lab_2.Domain;

public class HashSetComparer : IEqualityComparer<HashSet<string>>
{
    public bool Equals(HashSet<string>? x, HashSet<string>? y)
    {
        if (x == null || y == null) return false;
        return x.SetEquals(y); // Checks if both sets contain the same elements
    }

    public int GetHashCode(HashSet<string> obj)
    {
        int hash = 0;
        foreach (string s in obj.OrderBy(e => e)) // Order elements to ensure consistent hash
        {
            hash ^= s.GetHashCode(); // XOR hash codes for all elements
        }

        return hash;
    }
}