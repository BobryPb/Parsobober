using Parsobober.Pkb.Relations.Abstractions.Accessors;
using Parsobober.Pql.Query.Abstractions;
using Parsobober.Pql.Query.Arguments;
using Parsobober.Pql.Query.Queries.Abstractions;

namespace Parsobober.Pql.Query.Organizer.Clueless;

public class QueryOrganizerBuilder(IDtoProgramContextAccessor context, IComparer<IQueryDeclaration> comparer)
    : IQueryOrganizerBuilder
{
    private readonly List<IQueryDeclaration> _queries = [];
    public void AddQuery(IQueryDeclaration query) => _queries.Add(query);

    private readonly List<IAttributeQuery> _attributes = [];
    public void AddAttribute(IAttributeQuery attributeQuery) => _attributes.Add(attributeQuery);

    public void AddAlias((IDeclaration, IDeclaration) alias)
    {
    }

    public IQueryOrganizer Build()
    {
#if DEBUG
        return new QueryOrganizer(_queries.ToList(), _attributes.ToList(), context);
#else
        return new QueryOrganizer(_queries, _attributes, context);
#endif
    }
}