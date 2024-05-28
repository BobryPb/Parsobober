using Parsobober.Pql.Query.Arguments;
using Parsobober.Pql.Query.Queries.Abstractions;
using Parsobober.Pql.Query.Tree.Abstraction;

namespace Parsobober.Pql.Query.Tree.Node;

/// <summary>
/// Replaces given <see cref="IDeclaration"/> in <see cref="IQueryDeclaration"/> with results from <see cref="IQueryNode"/>.
/// Useful in query that depends on another query.
/// </summary>
internal class ReplacerQueryNode : IQueryNode
{
    private readonly IQueryDeclaration _queryToReplace;
    private readonly IDeclaration _argumentToReplace;
    private readonly IQueryNode _replacerQueryNode;

    /// <summary>
    /// Creates new instance of <see cref="ReplacerQueryNode"/>.
    /// Replaces <paramref name="argumentToReplace"/> argument with results from <paramref name="replacerQueryNode"/>.
    /// </summary>
    /// <param name="queryToReplace">
    /// Query that will be executed for each result of <paramref name="replacerQueryNode"/>.
    /// </param>
    /// <param name="argumentToReplace">
    /// Select statement that will be replaced with results from <paramref name="replacerQueryNode"/>.
    /// </param>
    /// <param name="replacerQueryNode">
    /// Query that will be executed to get results for <paramref name="argumentToReplace"/> replacement.
    /// </param>
    public ReplacerQueryNode(
        IQueryDeclaration queryToReplace,
        IDeclaration argumentToReplace,
        IQueryNode replacerQueryNode
    )
    {
        _queryToReplace = queryToReplace;
        _argumentToReplace = argumentToReplace;
        _replacerQueryNode = replacerQueryNode;
    }

    public IEnumerable<IComparable> Do()
    {
        // god forgive me
        // I am doing this only to pass my studies

        var arguments = _replacerQueryNode
            .Do()
            .Select(IArgument.Parse);

        var result = arguments
            .SelectMany(arg =>
                _queryToReplace
                    .ReplaceArgument(_argumentToReplace, arg)
                    .Do(_argumentToReplace)
            )
            .Distinct();

        return result;
    }

#if DEBUG
    private List<IComparable> Result => Do().ToList();
#endif
}