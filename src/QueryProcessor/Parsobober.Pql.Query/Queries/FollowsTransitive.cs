﻿using Parsobober.Pkb.Relations.Abstractions.Accessors;
using Parsobober.Pkb.Relations.Dto;
using Parsobober.Pql.Query.Arguments;
using Parsobober.Pql.Query.Queries.Abstractions;

namespace Parsobober.Pql.Query.Queries;

internal static class FollowsTransitive
{
    public class QueryDeclaration(IArgument followed, IArgument follows, IFollowsAccessor accessor) : IQueryDeclaration
    {
        public IArgument Left { get; } = followed;
        public IArgument Right { get; } = follows;

        public IEnumerable<IComparable> Do(IDeclaration select)
        {
            // pattern matching argumentów
            var query = (Left, Right) switch
            {
                // followed*(stmt, 1)
                (IStatementDeclaration declaration, IArgument.Line follows) =>
                    new GetTransitiveFollowedByLineNumber(accessor, follows.Value).Build(declaration),

                // followed*(1, stmt)
                (IArgument.Line followed, IStatementDeclaration follows) =>
                    new GetTransitiveFollowsByLineNumber(accessor, followed.Value).Build(follows),

                // followed*(stmt, stmt)
                (IStatementDeclaration followed, IStatementDeclaration follows) =>
                    BuildfollowedWithSelect(followed, follows),

                // followed*(1, 2) nie wspierane w tej wersji
                _ => throw new InvalidOperationException("Invalid query")
            };

            return query;

            IEnumerable<Statement> BuildfollowedWithSelect(
                IStatementDeclaration followed,
                IStatementDeclaration follows
            )
            {
                // tu nastąpi samowywrotka przy zapytaniach, w których nie ma wartości z selecta
                // przykład: Select x such that followed(a, b)

                if (followed == select)
                {
                    return new GetTransitiveFollowedByFollowsType(accessor).Create(follows).Build(followed);
                }

                if (follows == select)
                {
                    return new GetTransitiveFollowsByFollowedType(accessor).Create(followed).Build(follows);
                }

                throw new InvalidOperationException("Invalid query");
            }
        }
    }

    #region Queries

    private class GetTransitiveFollowedByFollowsType(IFollowsAccessor followedAccessor)
    {
        public followedQuery Create(IStatementDeclaration followsStatementDeclaration) =>
            followsStatementDeclaration switch
            {
                IStatementDeclaration.Statement => new GetTransitiveFollowedByFollowsType<Statement>(followedAccessor),
                IStatementDeclaration.Assign => new GetTransitiveFollowedByFollowsType<Assign>(followedAccessor),
                IStatementDeclaration.While => new GetTransitiveFollowedByFollowsType<While>(followedAccessor),
                _ => throw new ArgumentOutOfRangeException(nameof(followsStatementDeclaration))
            };
    }

    /// <summary>
    /// Gets transitive followeds of given type by follows type.
    /// </summary>
    /// <param name="followedAccessor">followed accessor.</param>
    /// <typeparam name="Tfollows">follows type.</typeparam>
    private class GetTransitiveFollowedByFollowsType<Tfollows>(IFollowsAccessor followedAccessor) : followedQuery
        where Tfollows : Statement
    {
        public override IEnumerable<Statement> Build(IStatementDeclaration followsStatementDeclaration) =>
            followsStatementDeclaration switch
            {
                IStatementDeclaration.Statement => followedAccessor.GetFollowedTransitive<Tfollows>(),
                IStatementDeclaration.Assign => followedAccessor.GetFollowedTransitive<Tfollows>().OfType<Assign>(),
                IStatementDeclaration.While => followedAccessor.GetFollowedTransitive<Tfollows>().OfType<While>(),
                _ => throw new ArgumentOutOfRangeException(nameof(followsStatementDeclaration))
            };
    }

    private class GetTransitiveFollowsByFollowedType(IFollowsAccessor followedAccessor)
    {
        public followedQuery Create(IStatementDeclaration followedStatementDeclaration) =>
            followedStatementDeclaration switch
            {
                IStatementDeclaration.Statement => new GetTransitiveFollowsByFollowedType<Statement>(
                    followedAccessor),
                IStatementDeclaration.Assign => new GetTransitiveFollowsByFollowedType<Assign>(followedAccessor),
                IStatementDeclaration.While => new GetTransitiveFollowsByFollowedType<While>(followedAccessor),
                _ => throw new ArgumentOutOfRangeException(nameof(followedStatementDeclaration))
            };
    }

    /// <summary>
    /// Gets transitive followsren of given type by followed type.
    /// </summary>
    /// <param name="followedAccessor">followed accessor.</param>
    /// <typeparam name="Tfollowed">followed type.</typeparam>
    private class GetTransitiveFollowsByFollowedType<Tfollowed>(IFollowsAccessor followedAccessor) : followedQuery
        where Tfollowed : Statement
    {
        public override IEnumerable<Statement> Build(IStatementDeclaration followsStatementDeclaration) =>
            followsStatementDeclaration switch
            {
                IStatementDeclaration.Statement => followedAccessor.GetFollowersTransitive<Tfollowed>(),
                IStatementDeclaration.Assign => followedAccessor.GetFollowersTransitive<Tfollowed>().OfType<Assign>(),
                IStatementDeclaration.While => followedAccessor.GetFollowersTransitive<Tfollowed>().OfType<While>(),
                _ => throw new ArgumentOutOfRangeException(nameof(followsStatementDeclaration))
            };
    }

    /// <summary>
    /// Get transitive followed of given type by follows line number.
    /// </summary>
    /// <param name="followedAccessor">followed accessor.</param>
    /// <param name="line">Line number.</param>
    private class GetTransitiveFollowedByLineNumber(IFollowsAccessor followedAccessor, int line) : followedQuery
    {
        public override IEnumerable<Statement> Build(IStatementDeclaration followsStatementDeclaration) =>
            followsStatementDeclaration switch
            {
                IStatementDeclaration.Statement => followedAccessor.GetFollowedTransitive(line),
                IStatementDeclaration.Assign => followedAccessor.GetFollowedTransitive(line).OfType<Assign>(),
                IStatementDeclaration.While => followedAccessor.GetFollowedTransitive(line).OfType<While>(),
                _ => throw new ArgumentOutOfRangeException(nameof(followsStatementDeclaration))
            };
    }

    /// <summary>
    /// Gets transitive followsren of given type by followed line number.
    /// </summary>
    /// <param name="followedAccessor">followed accessor.</param>
    /// <param name="line">Line number.</param>
    private class GetTransitiveFollowsByLineNumber(IFollowsAccessor followedAccessor, int line) : followedQuery
    {
        public override IEnumerable<Statement> Build(IStatementDeclaration followsStatementDeclaration) =>
            followsStatementDeclaration switch
            {
                IStatementDeclaration.Statement => followedAccessor.GetFollowersTransitive(line),
                IStatementDeclaration.Assign => followedAccessor.GetFollowersTransitive(line).OfType<Assign>(),
                IStatementDeclaration.While => followedAccessor.GetFollowersTransitive(line).OfType<While>(),
                _ => throw new ArgumentOutOfRangeException(nameof(followsStatementDeclaration))
            };
    }

    /// <summary>
    /// Represents a followed query.
    /// </summary>
    private abstract class followedQuery
    {
        /// <summary>
        /// Builds a query.
        /// </summary>
        /// <param name="followsStatementDeclaration"> The declaration to build the query for. </param>
        /// <returns> The query. </returns>
        public abstract IEnumerable<Statement> Build(IStatementDeclaration followsStatementDeclaration);
    }

    #endregion
}