using System.Text.RegularExpressions;
using Parsobober.Pkb.Relations.Abstractions;
using Parsobober.Pkb.Relations.Abstractions.Accessors;
using Parsobober.Pql.Query.Abstractions;
using Parsobober.Pql.Query.Arguments;
using Parsobober.Pql.Query.Queries;
using Parsobober.Pql.Query.Queries.Abstractions;
using Parsobober.Pql.Query.Queries.With;
using Parsobober.Pql.Query.QueryResult;

namespace Parsobober.Pql.Query;

internal partial class QueryBuilder(
    IPkbAccessors accessor,
    IProgramContextAccessor programContext,
    IQueryOrganizerBuilder queryOrganizerBuilder
) : IQueryBuilder
{
    private string _select = string.Empty;

    private IDeclaration? Select => _declarations.GetValueOrDefault(_select);

    private readonly Dictionary<string, IDeclaration> _declarations = new();

    private readonly List<(string, string)> _aliases = [];

    private record AttributeDeclaration(string Attribute, string Value);

    private readonly Dictionary<string, AttributeDeclaration> _attributes = new();

    private record QueryDeclaration(string Left, string Right);

    private readonly List<QueryDeclaration> _parent = [];

    private readonly List<QueryDeclaration> _parentTransitive = [];

    private readonly List<QueryDeclaration> _follows = [];

    private readonly List<QueryDeclaration> _followsTransitive = [];

    private readonly List<QueryDeclaration> _modifies = [];

    private readonly List<QueryDeclaration> _uses = [];

    private readonly List<QueryDeclaration> _calls = [];

    private readonly List<QueryDeclaration> _callsTransitive = [];

    private readonly List<QueryDeclaration> _next = [];

    private readonly List<QueryDeclaration> _nextTransitive = [];

    private IQueryResultFactory _queryResultFactory = new QueryListResult.Factory();

    private void AddQueries<T>(List<QueryDeclaration> relations, Func<IArgument, IArgument, T> queryCreator)
        where T : IQueryDeclaration
    {
        foreach (var (l, r) in relations.Distinct())
        {
            var left = IArgument.Parse(_declarations, l);
            var right = IArgument.Parse(_declarations, r);
            queryOrganizerBuilder.AddQuery(queryCreator(left, right));
        }
    }

    public IQueryResult Build()
    {
        AddQueries(_parent, (l, r) => new Parent.QueryDeclaration(l, r, accessor.Parent));
        AddQueries(_parentTransitive, (l, r) => new ParentTransitive.QueryDeclaration(l, r, accessor.Parent));
        AddQueries(_follows, (l, r) => new Follows.QueryDeclaration(l, r, accessor.Follows));
        AddQueries(_followsTransitive,
            (l, r) => new FollowsTransitive.QueryDeclaration(l, r, accessor.Follows));
        AddQueries(_modifies, (l, r) => new Modifies.QueryDeclaration(l, r, accessor.Modifies));
        AddQueries(_uses, (l, r) => new Uses.QueryDeclaration(l, r, accessor.Uses));
        AddQueries(_calls, (l, r) => new Calls.QueryDeclaration(l, r, accessor.Calls));
        AddQueries(_callsTransitive, (l, r) => new CallsTransitive.QueryDeclaration(l, r, accessor.Calls));
        AddQueries(_next, (l, r) => new Next(l, r, accessor.Next));
        AddQueries(_nextTransitive, (l, r) => new NextTransitive(l, r, accessor.Next));

        var factory = new WithQueryFactory(programContext);
        var attributes = _attributes
            .Distinct()
            .Select(a =>
            {
                var (key, (attribute, value)) = a;
                return factory.Create(_declarations[key], attribute, value);
            });
        foreach (var attribute in attributes)
        {
            queryOrganizerBuilder.AddAttribute(attribute);
        }

        foreach (var alias in _aliases)
        {
            var (key1, key2) = alias;
            queryOrganizerBuilder.AddAlias((_declarations[key1], _declarations[key2]));
        }

        var organizer = queryOrganizerBuilder.Build();

        if (_tuples.Count > 0)
        {
            return _queryResultFactory.Create(organizer.OrganizerTuple(_tuples.Select(t => _declarations[t])));
        }

        if (Select is null)
        {
            return _queryResultFactory.Create(organizer.OrganizeBoolean());
        }

        return _queryResultFactory.Create(organizer.Organize(Select));
    }

    public IQueryBuilder AddSelect(string synonym)
    {
        _queryResultFactory = new QueryListResult.Factory();
        _select = synonym;
        return this;
    }

    public IQueryBuilder SetBoolean()
    {
        _queryResultFactory = new QueryBooleanResult.Factory();
        return this;
    }

    private readonly List<string> _tuples = [];

    public IQueryBuilder AddTuple(string tuple)
    {
        if (_queryResultFactory is not QueryTupleResult.Factory)
        {
            _queryResultFactory = new QueryTupleResult.Factory();
        }

        _tuples.Add(tuple);
        return this;
    }

    public IQueryBuilder AddDeclaration(string declaration)
    {
        var split = declaration.Split();
        var rest = string.Join("", split.Skip(1)).Split(',');

        var type = split.First();

        foreach (var key in rest.Select(s => s.Replace(";", "")))
        {
            _declarations.Add(key, IDeclaration.Parse(type, key));
        }

        return this;
    }

    public IQueryBuilder With(string attribute, string value)
    {
        var regex = AttributeRegex();
        var match = regex.Match(attribute);
        var groups = match.Groups.Values.ToList();
        var key = groups[1].Value;
        var attributeKey = groups[2].Value;

        _attributes.Add(key, new AttributeDeclaration(attributeKey, value.Replace("\"", "")));

        return this;
    }

    public IQueryBuilder WithCombined(string attribute1, string attribute2)
    {
        var key1 = AttributeRegex().Match(attribute1).Groups[1].Value;
        var key2 = AttributeRegex().Match(attribute2).Groups[1].Value;
        _aliases.Add((key1, key2));
        return this;
    }

    #region Relation methods

    public IQueryBuilder AddFollows(string reference1, string reference2)
    {
        _follows.Add(new QueryDeclaration(reference1, reference2));
        return this;
    }

    public IQueryBuilder AddFollowsTransitive(string reference1, string reference2)
    {
        _followsTransitive.Add(new QueryDeclaration(reference1, reference2));
        return this;
    }

    public IQueryBuilder AddParent(string parent, string child)
    {
        _parent.Add(new QueryDeclaration(parent, child));
        return this;
    }

    public IQueryBuilder AddParentTransitive(string parent, string child)
    {
        _parentTransitive.Add(new QueryDeclaration(parent, child));
        return this;
    }

    public IQueryBuilder AddModifies(string reference1, string reference2)
    {
        _modifies.Add(new QueryDeclaration(reference1, reference2));
        return this;
    }

    public IQueryBuilder AddUses(string reference1, string reference2)
    {
        _uses.Add(new QueryDeclaration(reference1, reference2));
        return this;
    }

    public IQueryBuilder AddCalls(string reference1, string reference2)
    {
        _calls.Add(new QueryDeclaration(reference1, reference2));
        return this;
    }

    public IQueryBuilder AddCallsTransitive(string reference1, string reference2)
    {
        _callsTransitive.Add(new QueryDeclaration(reference1, reference2));
        return this;
    }

    public IQueryBuilder AddNext(string reference1, string reference2)
    {
        _next.Add(new QueryDeclaration(reference1, reference2));
        return this;
    }

    public IQueryBuilder AddNextTransitive(string reference1, string reference2)
    {
        _nextTransitive.Add(new QueryDeclaration(reference1, reference2));
        return this;
    }

    [GeneratedRegex(@"^(.+)\.(.+)$", RegexOptions.Compiled)]
    private static partial Regex AttributeRegex();

    #endregion
}