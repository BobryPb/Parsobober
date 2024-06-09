using Parsobober.Pkb.Relations.Abstractions.Accessors;
using Parsobober.Pkb.Relations.Dto;
using Parsobober.Pql.Query.Abstractions;
using Parsobober.Pql.Query.Arguments;
using Parsobober.Pql.Query.Queries;
using Parsobober.Pql.Query.Queries.Abstractions;
using Cache =
    System.Collections.Generic.Dictionary<Parsobober.Pql.Query.Arguments.IDeclaration,
        System.Collections.Generic.HashSet<Parsobober.Pkb.Relations.Dto.IPkbDto>>;

namespace Parsobober.Pql.Query.Organizer.Clueless;

public class QueryOrganizer : IQueryOrganizer
{
    private readonly List<IQueryDeclaration> _queries;
    private readonly List<IAttributeQuery> _attributes;
    private readonly IDtoProgramContextAccessor _context;

    internal QueryOrganizer(
        List<IQueryDeclaration> queries,
        List<IAttributeQuery> attributes,
        IDtoProgramContextAccessor context
    )
    {
        _context = context;
        _queries = queries;
        _attributes = attributes;
    }


    public IEnumerable<IPkbDto> Organize(IDeclaration select)
    {
        // Eat booleans, example Parent(1,2)
        if (!ResolveBooleanQueries())
        {
            return Enumerable.Empty<IPkbDto>();
        }

        // Eat single argument queries, example Parent(w, 1)
        var cache = ResolveSingleArgumentQueries();

        // Resolve select route
        var inputs = cache.TryGetValue(select, out var cached)
            ? cached
            : select.ExtractFromContext(_context).ToHashSet();

        var results = new List<IPkbDto>();
        List<IQueryDeclaration> booleanRoutes = [];

        foreach (var input in inputs)
        {
            var copyCache = new Cache(cache)
            {
                [select] = [input]
            };
            var inner = new Route(_queries.ToList(), select, input, copyCache);

            var (result, rest) = inner.Execute();

            if (!result)
            {
                continue;
            }

            booleanRoutes = rest!;
            results.Add(input);
        }

        // Resolve boolean routes
        if (!ResolveBooleanRoutes(booleanRoutes, cache))
        {
            return Enumerable.Empty<IPkbDto>();
        }

        return results;
    }

    public bool OrganizeBoolean()
    {
        // Eat booleans, example Parent(1,2)
        if (!ResolveBooleanQueries())
        {
            return false;
        }

        // Eat single argument queries, example Parent(w, 1)
        var cache = ResolveSingleArgumentQueries();

        return ResolveBooleanRoutes(_queries, cache);
    }

    public IEnumerable<Dictionary<IDeclaration, IPkbDto>> OrganizerTuple(IEnumerable<IDeclaration> selects)
    {
        throw new NotImplementedException();
    }

    private bool ResolveBooleanRoutes(List<IQueryDeclaration> rest, Cache cache)
    {
        if (rest.Count == 0)
        {
            return true;
        }

        var select = (rest.First().Left as IDeclaration)!;
        var inputs = cache.TryGetValue(select, out var cached)
            ? cached
            : select.ExtractFromContext(_context).ToHashSet();

        List<IQueryDeclaration> booleanRoutes = [];

        foreach (var input in inputs)
        {
            var copyCache = new Cache(cache)
            {
                [select] = [input]
            };
            var inner = new Route(rest.ToList(), select, input, copyCache);

            var (result, restInPiece) = inner.Execute();

            if (restInPiece is not null)
            {
                booleanRoutes = restInPiece;
            }

            if (result && restInPiece!.Count == 0)
            {
                return ResolveBooleanRoutes(booleanRoutes, cache);
            }
        }

        if (booleanRoutes.Any(q => q.Left != select && q.Right != select))
        {
            return ResolveBooleanRoutes(booleanRoutes, cache);
        }

        return false;
    }

    /// <summary>
    /// Returns cache.
    /// </summary>
    private Dictionary<IDeclaration, HashSet<IPkbDto>> ResolveSingleArgumentQueries()
    {
        var cache = new Dictionary<IDeclaration, HashSet<IPkbDto>>();
        var argumentQueries = _queries
            .Where(q =>
                q is { Left: IDeclaration, Right: not IDeclaration } or { Left: not IDeclaration, Right: IDeclaration })
            .Select(q => (((q.Left is IDeclaration ? q.Left : q.Right) as IDeclaration)!, q))
            .ToList();

        if (argumentQueries.Count == 0)
        {
            return cache;
        }

        foreach (var (declaration, query) in argumentQueries)
        {
            var result = query.Do();
            if (cache.TryGetValue(declaration, out var list))
            {
                cache[declaration] = list.Intersect(result).ToHashSet();
            }
            else
            {
                cache.Add(declaration, result.ToHashSet());
            }
        }

        _queries.RemoveAll(q => argumentQueries.Select(a => a.Item2).Contains(q));

        return cache;
    }

    private bool ResolveBooleanQueries()
    {
        var booleans = _queries
            .Where(QueryDeclarationExtensions.IsBooleanQuery)
            .ToList();

        var result = booleans.All(q => q.Do().Any());

        if (result)
        {
            _queries.RemoveAll(q => booleans.Contains(q));
        }

        return result;
    }
}