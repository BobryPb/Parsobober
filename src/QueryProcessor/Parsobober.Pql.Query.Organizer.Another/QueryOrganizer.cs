using Parsobober.Pkb.Relations.Abstractions.Accessors;
using Parsobober.Pkb.Relations.Dto;
using Parsobober.Pql.Query.Abstractions;
using Parsobober.Pql.Query.Arguments;
using Parsobober.Pql.Query.Queries;
using Parsobober.Pql.Query.Queries.Abstractions;

namespace Parsobober.Pql.Query.Organizer.Another;

public class QueryOrganizer : IQueryOrganizer
{
    private readonly List<IQueryDeclaration> _queries;
    private readonly List<IAttributeQuery> _attributes;
    private readonly IDtoProgramContextAccessor _context;

    public QueryOrganizer(
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
        var input = select
            .ExtractFromContext(_context)
            .ToHashSet();

        var attribute = _attributes.FirstOrDefault(a => a.Declaration == select);
        if (attribute is not null)
        {
            input = attribute.ApplyAttribute(input).ToHashSet();
        }

        // select something else
        if (_queries.All(q => q.Left != select && q.Right != select))
        {
            return OrganizeBoolean() ? input : Enumerable.Empty<IPkbDto>();
        }

        if (!RemoveBooleanQueries())
        {
            return Enumerable.Empty<IPkbDto>();
        }

        var result = InnerOrganize(select, input);
        while (true)
        {
            var anotherOne = InnerOrganize(select, result);
            if (result.SetEquals(anotherOne))
            {
                break;
            }

            result = anotherOne;
        }

        if (_queries.Count > 0)
        {
            var allow = OrganizeBoolean();
            if (allow)
            {
                return result;
            }

            return Enumerable.Empty<IPkbDto>();
        }

        return result;
    }

    private HashSet<IPkbDto> InnerOrganize(IDeclaration previousSelect, HashSet<IPkbDto> input)
    {
        input = ApplyArgumentQueries(previousSelect, input);

        var query = _queries
            .FirstOrDefault(q =>
                q is { Left: IDeclaration left, Right: IDeclaration } && left == previousSelect ||
                q is { Left: IDeclaration, Right: IDeclaration right } && right == previousSelect);

        if (query is null)
        {
            return input;
        }

        var result = new Dictionary<IPkbDto, List<IPkbDto>>();

        foreach (var dto in input)
        {
            var queryWithArgument = query.ReplaceArgument(previousSelect, IArgument.Parse(dto));
            var queryResult = queryWithArgument.Do().ToList();
            if (queryResult.Count == 0)
            {
                continue;
            }

            result.Add(dto, queryResult);
        }

        var intersection = result.SelectMany(x => x.Value).ToHashSet();
        var select = query.GetAnotherSide(previousSelect)!;

        var attribute = _attributes.FirstOrDefault(a => a.Declaration == select);
        if (attribute is not null)
        {
            intersection = attribute.ApplyAttribute(intersection).ToHashSet();
        }

        _queries.Remove(query);
        var next = InnerOrganize(select, intersection);

        var output = result
            .Where(x => x.Value.Any(v => next.Contains(v)))
            .Select(x => x.Key)
            .ToHashSet();

        return output;
    }

    public bool OrganizeBoolean()
    {
        var query = _queries
            .FirstOrDefault(q => q is { Left: IDeclaration } or { Right: IDeclaration });

        if (query is null)
        {
            return RemoveBooleanQueries();
        }

        var select = query.Left as IDeclaration ?? (query.Right as IDeclaration)!;

        var result = Organize(select);

        return result.Any();
    }

    public IEnumerable<Dictionary<IDeclaration, IPkbDto>> OrganizerTuple(IEnumerable<IDeclaration> selects)
    {
        throw new NotImplementedException();
    }

    private bool RemoveBooleanQueries()
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

    private HashSet<IPkbDto> ApplyArgumentQueries(IDeclaration select, HashSet<IPkbDto> input)
    {
        var argumentQueries = _queries
            .Where(q =>
                q is { Left: IDeclaration left, Right: not IDeclaration } && left == select ||
                q is { Left: not IDeclaration, Right: IDeclaration right } && right == select)
            .ToList();

        if (argumentQueries.Count <= 0)
        {
            return input;
        }

        var argumentQueriesResults = argumentQueries
            .Select(argumentQuery =>
            {
                var innerResults = new HashSet<IPkbDto>();

                foreach (var dto in input)
                {
                    var queryWithArgument = argumentQuery.ReplaceArgument(select, IArgument.Parse(dto));
                    var queryResult = queryWithArgument.Do();
                    if (!queryResult.Any())
                    {
                        continue;
                    }

                    innerResults.Add(dto);
                }

                return innerResults;
            })
            .ToList();

        var results = argumentQueriesResults
            .Aggregate((current, next) =>
            {
                current.IntersectWith(next);
                return current;
            });

        _queries.RemoveAll(q => argumentQueries.Contains(q));

        input = input.Intersect(results).ToHashSet();

        return input;
    }
}