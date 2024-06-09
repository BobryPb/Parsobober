using Parsobober.Pkb.Relations.Abstractions.Accessors;
using Parsobober.Pkb.Relations.Dto;
using Parsobober.Pql.Query.Abstractions;
using Parsobober.Pql.Query.Arguments;
using Parsobober.Pql.Query.Queries;
using Parsobober.Pql.Query.Queries.Abstractions;
using Parsobober.Shared;
using QueryContext =
    System.Collections.Generic.Dictionary<
        Parsobober.Pql.Query.Arguments.IDeclaration,
        System.Collections.Generic.IEnumerable<Parsobober.Pkb.Relations.Dto.IPkbDto>
    >;

namespace Parsobober.Pql.Query.Organizer;

public class QueryOrganizer : IQueryOrganizer
{
    private readonly List<IQueryDeclaration> _queries;
    private readonly List<IAttributeQuery> _attributes;
    private readonly IComparer<IQueryDeclaration> _comparer;
    private readonly IDtoProgramContextAccessor _context;
    private readonly List<(IDeclaration, IDeclaration)> _aliases;

    public QueryOrganizer(
        List<IQueryDeclaration> queries,
        List<IAttributeQuery> attributes,
        IDtoProgramContextAccessor context,
        List<(IDeclaration, IDeclaration)> aliases,
        IComparer<IQueryDeclaration> comparer
    )
    {
        _context = context;
        _aliases = aliases;
        _queries = queries;
        _attributes = attributes;
        _comparer = comparer;

        // map all declarations used in queries
        _declarations = _queries
            .SelectMany<IQueryDeclaration, IDeclaration?>(
                q => new[] { q.Left as IDeclaration, q.Right as IDeclaration })
            .Distinct()
            .WhereNotNull()
            .ToList();

        _declarationsMap = new QueryContext();
        _declarations.ForEach(d => TryAddDeclarationToMap(d));

        foreach (var (a1, a2) in _aliases)
        {
            TryAddDeclarationToMap(a1);
            TryAddDeclarationToMap(a2);
        }

        // apply aliases
        ApplyAliases();

        // replace arguments with attributes
        _queries = _queries
            .Select(q =>
            {
                var attribute = _attributes.Where(a => a.Declaration == q.Left || a.Declaration == q.Right);
                return attribute.Aggregate(q, (current, a) => a.ApplyAttribute(current));
            })
            .ToList();
    }

    private void ApplyAliases()
    {
        foreach (var (from, to) in _aliases)
        {
            _declarationsMap[from] = _declarationsMap[from]
                .Intersect(_declarationsMap[to], new PkbDtoComparer())
                .ToList();

            _declarationsMap[to] = _declarationsMap[to]
                .Intersect(_declarationsMap[from], new PkbDtoComparer())
                .ToList();
        }
    }

    private readonly List<IDeclaration> _declarations;
    private readonly QueryContext _declarationsMap;

    private bool TryAddDeclarationToMap(IDeclaration declaration)
    {
        // if declaration is already in map => fail
        if (_declarationsMap.ContainsKey(declaration))
        {
            return false;
        }

        // add declaration to map
        _declarationsMap[declaration] = declaration.ExtractFromContext(_context);

        // Apply attribute
        var attribute = _attributes.FirstOrDefault(a => a.Declaration == declaration);
        if (attribute is not null)
        {
            _declarationsMap[declaration] =
                _declarationsMap[declaration].Intersect(attribute.Do(), new PkbDtoComparer());
        }

        return true;
    }

    public IEnumerable<IPkbDto> Organize(IDeclaration select)
    {
        var selectNothing = TryAddDeclarationToMap(select);

        // check if it is not boolean query
        var booleans = _queries.Where(QueryDeclarationExtensions.IsBooleanQuery).ToList();

        // optimize boolean queries
        if (booleans.Count > 0)
        {
            var result = _queries
                .Select(q => q.Do())
                .All(r => r.Any());

            if (!result)
            {
                return [];
            }

            if (booleans.Count == _queries.Count)
            {
                return _declarationsMap[select];
            }

            _queries.RemoveAll(b => booleans.Contains(b));
        }


        var factory = new QueryOptimizerFactory(_queries, _comparer);

        // todo everything is too bugged :D
        // Prepare(factory.Create());

        if (_queries.Count > 0)
        {
            var count = _queries
                .SelectMany(q => new[] { q.Left, q.Right })
                .GroupBy(q => q)
                .Select(g => g.Count())
                .Max();

            foreach (var optimizer in Enumerable.Range(0, count).Select(_ => factory.Create()))
            {
                // iterate multiple times because I said so
                Iterate(optimizer);
            }
        }

        return selectNothing switch
        {
            true when !_declarationsMap.Values.All(v => v.Any()) => Enumerable.Empty<IPkbDto>(),
            _ => _declarationsMap[select]
        };
    }

    private void Prepare(QueryOptimizer optimizer)
    {
        var easy = _declarations
            .Select(optimizer.GetBest)
            .WhereNotNull();

        foreach (var (select, query) in easy)
        {
            var result = query.Do(select);
            _declarationsMap[select] = result;
        }

        ApplyAliases();
    }

    private void Iterate(QueryOptimizer optimizer)
    {
        while (optimizer.Count > 0)
        {
            if (!optimizer.HasQueries)
            {
                break;
            }

            var (query, currentSelect) = optimizer.GetBest();

            ProcessQuery(currentSelect, query);

            optimizer.Consume(query);
        }
    }

    public bool OrganizeBoolean()
    {
        var select = _declarations.FirstOrDefault();

        // if there is select in any query declaration => Organize
        if (select is not null)
        {
            return Organize(select).Any();
        }

        // otherwise just do every query in place
        var result = _queries
            .Select(q => q.Do())
            .All(r => r.Any());

        return result;
    }

    public IEnumerable<Dictionary<IDeclaration, IPkbDto>> OrganizerTuple(IEnumerable<IDeclaration> selects)
    {
        throw new NotImplementedException();
    }

    private void ProcessQuery(IDeclaration currentSelect, IQueryDeclaration query)
    {
        var (left, right) = (query.Left, query.Right);

        // if both left and right are not currentSelect => skip
        if (left != currentSelect && right != currentSelect)
        {
            return;
        }

        var other = left == currentSelect ? right : left;

        // if other is not declaration, it is a constant argument
        if (other is not IDeclaration otherDeclaration)
        {
            _declarationsMap[currentSelect] =
                _declarationsMap[currentSelect].Intersect(query.Do(), new PkbDtoComparer());
            return;
        }

        var newCurrentSelect = _declarationsMap[otherDeclaration]
            .Select(value => query
                .ReplaceArgument(otherDeclaration, IArgument.Parse(value))
                .Do(currentSelect))
            .SelectMany(x => x)
            .Distinct(new PkbDtoComparer());
        _declarationsMap[currentSelect] =
            _declarationsMap[currentSelect].Intersect(newCurrentSelect, new PkbDtoComparer());

        ApplyAliases();

        var newOtherSelect = _declarationsMap[currentSelect]
            .Select(value => query.ReplaceArgument(currentSelect, IArgument.Parse(value))
                .Do(otherDeclaration))
            .SelectMany(x => x)
            .Distinct(new PkbDtoComparer());
        _declarationsMap[otherDeclaration] =
            _declarationsMap[otherDeclaration].Intersect(newOtherSelect, new PkbDtoComparer());

        ApplyAliases();
    }
}