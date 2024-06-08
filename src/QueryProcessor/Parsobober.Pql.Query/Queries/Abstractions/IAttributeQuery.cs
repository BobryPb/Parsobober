using Parsobober.Pkb.Relations.Dto;
using Parsobober.Pql.Query.Arguments;

namespace Parsobober.Pql.Query.Queries.Abstractions;

public interface IAttributeQuery
{
    IDeclaration Declaration { get; }

    IEnumerable<IPkbDto> Do();

    IQueryDeclaration ApplyAttribute(IQueryDeclaration queryDeclaration);

    IEnumerable<IPkbDto> ApplyAttribute(IEnumerable<IPkbDto> input);
}