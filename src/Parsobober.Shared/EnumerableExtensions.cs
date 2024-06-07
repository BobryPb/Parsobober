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
}