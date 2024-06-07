using Parsobober.Pkb.Relations.Abstractions.Accessors;
using Parsobober.Pql.Query.Abstractions;
using Parsobober.Pql.Query.Arguments;
using Parsobober.Pql.Query.Queries.Abstractions;

namespace Parsobober.Pql.Query.Organizer;

public class QueryOrganizerBuilder(IDtoProgramContextAccessor context, IComparer<IQueryDeclaration> comparer)
    : IQueryOrganizerBuilder
{
    private readonly List<IQueryDeclaration> _queries = [];
    public void AddQuery(IQueryDeclaration query) => _queries.Add(query);

    private readonly List<IAttributeQuery> _attributes = [];
    public void AddAttribute(IAttributeQuery attributeQuery) => _attributes.Add(attributeQuery);

    private readonly List<(IDeclaration, IDeclaration)> _aliases = [];

    public void AddAlias((IDeclaration, IDeclaration) alias)
    {
        _aliases.Add(alias);
    }

    public IQueryOrganizer Build() => new QueryOrganizer(_queries, _attributes, context, _aliases, comparer);
}