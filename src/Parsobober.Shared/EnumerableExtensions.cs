namespace Parsobober.Shared;

public static class EnumerableExtensions
{
    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> enumerable)
        where T : class =>
        enumerable
            .Where(v => v is not null)
            .Select(v => v!);

    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> enumerable)
        where T : struct =>
        enumerable
            .Where(v => v.HasValue)
            .Select(v => v!.Value);

    public static List<List<T>> CartesianProduct<T>(this List<IEnumerable<T>> lists)
    {
        var result = new List<List<T>>();
        if (lists.Count == 0)
        {
            result.Add([]);
            return result;
        }

        var firstList = lists[0];
        var remainingLists = CartesianProduct(lists.GetRange(1, lists.Count - 1));

        foreach (var item in firstList)
        {
            foreach (var remainingList in remainingLists)
            {
                var combination = new List<T> { item };
                combination.AddRange(remainingList);
                result.Add(combination);
            }
        }

        return result;
    }

    public static IEnumerable<T> IntersectMany<T>(List<IEnumerable<T>> lists)
    {
        if (lists.Count == 0)
            return Enumerable.Empty<T>();

        var intersection = lists.First();

        foreach (var list in lists.Skip(1))
        {
            intersection = intersection.Intersect(list);
        }

        return intersection;
    }
}