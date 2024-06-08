using Parsobober.Pql.Query.Arguments;
using Parsobober.Pql.Query.Queries.Abstractions;

namespace Parsobober.Pql.Query.Queries;

public static class QueryDeclarationExtensions
{
    public static bool IsBooleanQuery(this IQueryDeclaration query) =>
        query is { Left: not IDeclaration, Right: not IDeclaration };
}