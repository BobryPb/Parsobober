using Parsobober.Pkb.Relations.Abstractions.Accessors;
using Parsobober.Pkb.Relations.Dto;
using Parsobober.Pql.Query.Arguments;
using Parsobober.Pql.Query.Queries.Abstractions;
using Parsobober.Pql.Query.Queries.Core;
using Parsobober.Pql.Query.Queries.Exceptions;

namespace Parsobober.Pql.Query.Queries;

internal static class Uses
{
    public class QueryDeclaration(IArgument left, IArgument right, IUsesAccessor accessor)
        : ReplaceableArgumentQueryDeclaration<QueryDeclaration>, IQueryDeclaration
    {
        public override IArgument Left { get; } = left;
        public override IArgument Right { get; } = right;

        public override IEnumerable<IComparable> Do()
        {
            // pattern matching argumentów
            var query = (Left, Right) switch
            {
                (IArgument.Line line, IArgument.VarName name) =>
                    new BooleanUsesQuery(accessor, line.Value, name.Value).Build(),
                _ => DoDeclaration()
            };

            return query;
        }

        public override IEnumerable<IComparable> Do(IDeclaration select)
        {
            // pattern matching argumentów
            var query = (Left, Right) switch
            {
                // Uses(stmt, 'v')
                (IStatementDeclaration declaration, IArgument.VarName right) =>
                    new GetStatementsByVariable(accessor, right.Value).Build(declaration),

                // Uses(1, variable)
                (IArgument.Line left, IVariableDeclaration) =>
                    new GetVariablesByLineNumber(accessor, left.Value).Build(),

                // Uses(stmt, variable)
                (IStatementDeclaration left, IVariableDeclaration right) => BuildUsesWithSelect(left, right),

                // Uses(1, 'v') nie wspierane w tej wersji todo już wspierane
                _ => throw new QueryNotSupported(this, $"Uses({Left}, {Right}) is not supported.")
            };

            return query;

            IEnumerable<IComparable> BuildUsesWithSelect(IStatementDeclaration left, IVariableDeclaration right)
            {
                // tu nastąpi samowywrotka przy zapytaniach, w których nie ma wartości z selecta
                // przykład: Select x such that uses(s, v)

                if (left == select)
                {
                    return new GetStatements(accessor).Build(left);
                }

                if (right == select)
                {
                    return new GetVariablesByStatementType(accessor).Build(left);
                }

                throw new DeclarationNotFoundException(select, this);
            }
        }

        protected override QueryDeclaration CloneSelf(IArgument left, IArgument right) => new(left, right, accessor);
    }

    #region Queries

    private class GetStatements(IUsesAccessor usesAccessor) : UsesQueryStatement
    {
        public override IEnumerable<Statement> Build(IStatementDeclaration statementDeclaration) =>
            statementDeclaration switch
            {
                IStatementDeclaration.Statement => usesAccessor.GetStatements(),
                IStatementDeclaration.Assign => usesAccessor.GetStatements().OfType<Assign>(),
                IStatementDeclaration.While => usesAccessor.GetStatements().OfType<While>(),
                IStatementDeclaration.If => usesAccessor.GetStatements().OfType<If>(),
                IStatementDeclaration.Call => usesAccessor.GetStatements().OfType<Call>(),
                _ => throw new ArgumentOutOfRangeException(nameof(statementDeclaration))
            };
    }

    private class GetVariablesByStatementType(IUsesAccessor usesAccessor) : UsesQueryVariable
    {
        public override IEnumerable<Variable> Build(IStatementDeclaration statementDeclaration) =>
            statementDeclaration switch
            {
                IStatementDeclaration.Statement => usesAccessor.GetVariables<Statement>(),
                IStatementDeclaration.Assign => usesAccessor.GetVariables<Assign>(),
                IStatementDeclaration.While => usesAccessor.GetVariables<While>(),
                IStatementDeclaration.If => usesAccessor.GetVariables<If>(),
                IStatementDeclaration.Call => usesAccessor.GetVariables<Call>(),
                _ => throw new ArgumentOutOfRangeException(nameof(statementDeclaration))
            };
    }


    /// <summary>
    /// Get statements of given type by variable name.
    /// </summary>
    /// <param name="usesAccessor">Uses accessor.</param>
    /// <param name="variableName">Variable name.</param>
    private class GetStatementsByVariable(IUsesAccessor usesAccessor, string variableName)
        : UsesQueryStatement
    {
        public override IEnumerable<Statement> Build(IStatementDeclaration left)
        {
            return left switch
            {
                IStatementDeclaration.Statement => usesAccessor.GetStatements(variableName),
                IStatementDeclaration.Assign => usesAccessor.GetStatements(variableName).OfType<Assign>(),
                IStatementDeclaration.While => usesAccessor.GetStatements(variableName).OfType<While>(),
                IStatementDeclaration.If => usesAccessor.GetStatements(variableName).OfType<If>(),
                IStatementDeclaration.Call => usesAccessor.GetStatements(variableName).OfType<Call>(),
                _ => throw new ArgumentOutOfRangeException(nameof(left))
            };
        }
    }

    /// <summary>
    /// Gets variables used by statement with given line number.
    /// </summary>
    /// <param name="usesAccessor">Uses accessor.</param>
    /// <param name="line">Line number.</param>
    private class GetVariablesByLineNumber(IUsesAccessor usesAccessor, int line)
    {
        public IEnumerable<Variable> Build() => usesAccessor.GetVariables(line);
    }

    /// <summary>
    /// Represents a uses query that returns statements.
    /// </summary>
    private abstract class UsesQueryStatement
    {
        /// <summary>
        /// Builds a query.
        /// </summary>
        /// <param name="declaration"> The declaration to build the query for. </param>
        /// <returns> The query. </returns>
        public abstract IEnumerable<Statement> Build(IStatementDeclaration declaration);
    }

    /// <summary>
    /// Represents a uses query that returns variables.
    /// </summary>
    private abstract class UsesQueryVariable
    {
        /// <summary>
        /// Builds a query.
        /// </summary>
        /// <param name="declaration"> The declaration to build the query for. </param>
        /// <returns> The query. </returns>
        public abstract IEnumerable<Variable> Build(IStatementDeclaration declaration);
    }

    private class BooleanUsesQuery(IUsesAccessor accessor, int line, string variableName)
    {
        public IEnumerable<IComparable> Build()
        {
            if (accessor.IsUsed(line, variableName))
            {
                return Enumerable.Repeat<IComparable>(true, 1);
            }

            return Enumerable.Empty<Statement>();
        }
    }

    #endregion
}