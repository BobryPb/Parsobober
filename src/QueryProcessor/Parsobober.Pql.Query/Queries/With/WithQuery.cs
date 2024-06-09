using Parsobober.Pkb.Relations.Dto;
using Parsobober.Pql.Query.Arguments;
using Parsobober.Pql.Query.Queries.Abstractions;

namespace Parsobober.Pql.Query.Queries.With;

public abstract class WithQuery(IDeclaration declaration) : IAttributeQuery
{
    public IDeclaration Declaration => declaration;
    public abstract IEnumerable<IPkbDto> Do();
    public abstract IQueryDeclaration ApplyAttribute(IQueryDeclaration queryDeclaration);

    public IEnumerable<IPkbDto> ApplyAttribute(IEnumerable<IPkbDto> input)
    {
        var result = Do();
        return result.Intersect(input);
    }

#if DEBUG
    private IEnumerable<IPkbDto> Result => Do();
#endif
}