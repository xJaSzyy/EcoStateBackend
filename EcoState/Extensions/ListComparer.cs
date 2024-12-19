namespace EcoState.Extensions;

public class ListComparer : IEqualityComparer<List<double>>
{
    public bool Equals(List<double> x, List<double> y)
    {
        if (x == null && y == null)
            return true;
        if (x == null || y == null)
            return false;
        if (x.Count != y.Count)
            return false;
        for (int i = 0; i < x.Count; i++)
        {
            if (x[i] != y[i])
                return false;
        }
        return true;
    }

    public int GetHashCode(List<double> obj)
    {
        int hash = 17;
        foreach (double d in obj)
        {
            hash = hash * 31 + d.GetHashCode();
        }
        return hash;
    }
}