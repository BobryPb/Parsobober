using Parsobober.Pkb.Relations.Dto;
using Parsobober.Pql.Query.Arguments;
using Parsobober.Pql.Query.Queries.Abstractions;

namespace Parsobober.Pql.Query.Organizer.Clueless;

public class Route
{
    private readonly List<IQueryDeclaration> _queries;
    private readonly IDeclaration _select;
    private readonly IPkbDto _dto;
    private IArgument Argument => IArgument.Parse(_dto);
    private readonly Dictionary<IDeclaration, HashSet<IPkbDto>> _cache;

    public Route(
        List<IQueryDeclaration> queries,
        IDeclaration select,
        IPkbDto argument,
        Dictionary<IDeclaration, HashSet<IPkbDto>> cache
    )
    {
        _queries = queries;
        _select = select;
        _dto = argument;
        _cache = cache;
    }

    public (bool result, List<IQueryDeclaration>? restQueries) Execute()
    {
        var candidateQueries = _queries
            .Where(q =>
                q is { Left: IDeclaration, Right: IDeclaration } or
                    { Left: IDeclaration, Right: IDeclaration });

        var query = candidateQueries.FirstOrDefault(q => q.Left == _select || q.Right == _select);

        if (query is null)
        {
            return (true, _queries);
        }

        _queries.Remove(query);

        var queryWithArgument = query.ReplaceArgument(_select, Argument);
        var nextSelect = query.GetAnotherSide(_select)!;

        if (_cache.TryGetValue(nextSelect, out var cached))
        {
            var cacheResult = cached.Any(c =>
            {
                var queryWithDoubleArgument = queryWithArgument.ReplaceArgument(nextSelect, IArgument.Parse(c));
                var result = queryWithDoubleArgument.Do().Any();
                return result;
            });

            return (cacheResult, _queries);
        }

        var results = queryWithArgument.Do();
        return results
            .Select(result =>
            {
                var cache = new Dictionary<IDeclaration, HashSet<IPkbDto>>(_cache) { { nextSelect, [result] } };
                var subQuery = new Route(_queries.ToList(), nextSelect, result, cache);
                return subQuery.Execute();
            })
            .FirstOrDefault();
    }
}