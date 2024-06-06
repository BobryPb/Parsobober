using Parsobober.Pkb.Relations.Abstractions.Accessors;
using Parsobober.Pkb.Relations.Dto;
using Parsobober.Pql.Query.Arguments;
using Parsobober.Pql.Query.Queries.Abstractions;
using Parsobober.Pql.Query.Queries.Core;
using Parsobober.Pql.Query.Queries.Exceptions;

namespace Parsobober.Pql.Query.Queries;

internal static class Follows
{
    public class QueryDeclaration(IArgument followed, IArgument follows, IFollowsAccessor accessor)
        : ReplaceableArgumentQueryDeclaration<QueryDeclaration>, IQueryDeclaration
    {
        public override IArgument Left { get; } = followed;
        public override IArgument Right { get; } = follows;

        public override IEnumerable<IPkbDto> Do()
        {
            var query = (Left, Right) switch
            {
                (Line left, Line right) =>
                    new BooleanFollowsQuery(accessor, left.Value, right.Value).Build(),

                _ => DoDeclaration()
            };

            return query;
        }

        public override IEnumerable<IPkbDto> Do(IDeclaration select)
        {
            // pattern matching argumentów
            var query = (Left, Right) switch
            {
                // Follows(stmt, 1)
                (IStatementDeclaration declaration, Line follows) =>
                    new GetFollowsByLineNumber(accessor, follows.Value).Build(declaration),

                //Follows(1, stmt) 
                (Line followed, IStatementDeclaration follows) =>
                    new GetFollowedByLineNumber(accessor, followed.Value).Build(follows),

                // Follows(stmt, stmt)
                (IStatementDeclaration followed, IStatementDeclaration follows) =>
                    BuildFollowsWithSelect(followed, follows),

                // Follows(1, 2) nie wspierane w tej wersji todo już wspierane
                _ => throw new QueryNotSupported(this, $"Follows({Left}, {Right}) is not supported.")
            };

            return query;

            IEnumerable<IPkbDto> BuildFollowsWithSelect(
                IStatementDeclaration followed,
                IStatementDeclaration follows
            )
            {
                if (followed == select)
                {
                    return new GetFollowedByFollowsType(accessor).Create(followed).Build(follows);
                }

                if (follows == select)
                {
                    return new GetFollowsByFollowedType(accessor).Create(follows).Build(followed);
                }

                throw new DeclarationNotFoundException(select, this);
            }
        }

        protected override QueryDeclaration CloneSelf(IArgument left, IArgument right) => new(left, right, accessor);
    }

    #region Queries

    private class GetFollowedByFollowsType(IFollowsAccessor followsAccessor)
    {
        public FollowsQuery Create(IStatementDeclaration followedStatementDeclaration) =>
            followedStatementDeclaration switch
            {
                IStatementDeclaration.Statement => new GetFollowedByFollowsType<Statement>(followsAccessor),
                IStatementDeclaration.Assign => new GetFollowedByFollowsType<Assign>(followsAccessor),
                IStatementDeclaration.While => new GetFollowedByFollowsType<While>(followsAccessor),
                IStatementDeclaration.If => new GetFollowedByFollowsType<If>(followsAccessor),
                IStatementDeclaration.Call => new GetFollowedByFollowsType<Call>(followsAccessor),
                _ => throw new ArgumentOutOfRangeException(nameof(followedStatementDeclaration))
            };
    }

    /// <summary>
    /// Gets followed of given type by follows type.
    /// </summary>
    /// <param name="followsAccessor">Follows accessor.</param>
    /// <typeparam name="TFollowed">Followed type.</typeparam>
    private class GetFollowedByFollowsType<TFollowed>(IFollowsAccessor followsAccessor) : FollowsQuery
        where TFollowed : Statement
    {
        public override IEnumerable<Statement> Build(IStatementDeclaration followsStatementDeclaration) =>
            followsStatementDeclaration switch
            {
                IStatementDeclaration.Statement => followsAccessor.GetFollowed<TFollowed>(),
                IStatementDeclaration.Assign => followsAccessor.GetFollowed<TFollowed>().OfType<Assign>(),
                IStatementDeclaration.While => followsAccessor.GetFollowed<TFollowed>().OfType<While>(),
                IStatementDeclaration.If => followsAccessor.GetFollowed<TFollowed>().OfType<If>(),
                IStatementDeclaration.Call => followsAccessor.GetFollowed<TFollowed>().OfType<Call>(),
                _ => throw new ArgumentOutOfRangeException(nameof(followsStatementDeclaration))
            };
    }

    private class GetFollowsByFollowedType(IFollowsAccessor followsAccessor)
    {
        public FollowsQuery Create(IStatementDeclaration declaration) =>
            declaration switch
            {
                IStatementDeclaration.Statement => new GetFollowedByFollowedType<Statement>(followsAccessor),
                IStatementDeclaration.Assign => new GetFollowedByFollowedType<Assign>(followsAccessor),
                IStatementDeclaration.While => new GetFollowedByFollowedType<While>(followsAccessor),
                IStatementDeclaration.If => new GetFollowedByFollowedType<If>(followsAccessor),
                IStatementDeclaration.Call => new GetFollowedByFollowedType<Call>(followsAccessor),
                _ => throw new ArgumentOutOfRangeException(nameof(declaration))
            };
    }

    /// <summary>
    /// Gets follows of given type by followed type.
    /// </summary>
    /// <param name="followsAccessor">Followed accessor.</param>
    /// <typeparam name="TFollows">Follows type.</typeparam>
    private class GetFollowedByFollowedType<TFollows>(IFollowsAccessor followsAccessor) : FollowsQuery
        where TFollows : Statement
    {
        public override IEnumerable<Statement> Build(IStatementDeclaration declaration) =>
            declaration switch
            {
                IStatementDeclaration.Statement => followsAccessor.GetFollowed<TFollows>(),
                IStatementDeclaration.Assign => followsAccessor.GetFollowed<TFollows>().OfType<Assign>(),
                IStatementDeclaration.While => followsAccessor.GetFollowed<TFollows>().OfType<While>(),
                IStatementDeclaration.If => followsAccessor.GetFollowed<TFollows>().OfType<If>(),
                IStatementDeclaration.Call => followsAccessor.GetFollowed<TFollows>().OfType<Call>(),
                _ => throw new ArgumentOutOfRangeException(nameof(declaration))
            };
    }

    /// <summary>
    /// Get Followed of given type by follows line number.
    /// </summary>
    /// <param name="followsAccessor">Followed accessor.</param>
    /// <param name="line">Line number.</param>
    private class GetFollowedByLineNumber(IFollowsAccessor followsAccessor, int line) : FollowsQuery
    {
        public override IEnumerable<Statement> Build(IStatementDeclaration follows)
        {
            var followsStatement = followsAccessor.GetFollowed(line);

            var result = follows switch
            {
                IStatementDeclaration.Statement => followsStatement,
                IStatementDeclaration.Assign => followsStatement as Assign,
                IStatementDeclaration.While => followsStatement as While,
                IStatementDeclaration.If => followsStatement as If,
                IStatementDeclaration.Call => followsStatement as Call,
                _ => throw new ArgumentOutOfRangeException(nameof(follows))
            };

            if (result is null)
            {
                return Enumerable.Empty<Statement>();
            }

            return Enumerable.Repeat(result, 1);
        }
    }

    /// <summary>
    /// Gets follows of given type by followed line number.
    /// </summary>
    /// <param name="followsAccessor">Followed accessor.</param>
    /// <param name="line">Line number.</param>
    private class GetFollowsByLineNumber(IFollowsAccessor followsAccessor, int line) : FollowsQuery
    {
        public override IEnumerable<Statement> Build(IStatementDeclaration follows)
        {
            var followsStatement = followsAccessor.GetFollower(line);

            var result = follows switch
            {
                IStatementDeclaration.Statement => followsStatement,
                IStatementDeclaration.Assign => followsStatement as Assign,
                IStatementDeclaration.While => followsStatement as While,
                IStatementDeclaration.If => followsStatement as If,
                IStatementDeclaration.Call => followsStatement as Call,
                _ => throw new ArgumentOutOfRangeException(nameof(follows))
            };

            if (result is null)
            {
                return Enumerable.Empty<Statement>();
            }

            return Enumerable.Repeat(result, 1);
        }
    }

    private class BooleanFollowsQuery(IFollowsAccessor accessor, int left, int right)
    {
        public IEnumerable<IPkbDto> Build() => IPkbDto.Boolean(accessor.IsFollowed(left, right));
    }

    /// <summary>
    /// Represents a follows query.
    /// </summary>
    private abstract class FollowsQuery
    {
        /// <summary>
        /// Builds a query.
        /// </summary>
        /// <param name="declaration"> The declaration to build the query for. </param>
        /// <returns> The query. </returns>
        public abstract IEnumerable<IPkbDto> Build(IStatementDeclaration declaration);
    }

    #endregion
}