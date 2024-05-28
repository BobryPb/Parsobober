using Parsobober.Pkb.Relations.Abstractions.Accessors;
using Parsobober.Pkb.Relations.Dto;
using Parsobober.Pql.Query.Arguments;
using Parsobober.Pql.Query.Queries.Abstractions;
using Parsobober.Pql.Query.Queries.Core;
using Parsobober.Pql.Query.Queries.Exceptions;

namespace Parsobober.Pql.Query.Queries;

internal static class Modifies
{
    public class QueryDeclaration(IArgument left, IArgument right, IModifiesAccessor accessor)
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
                    new BooleanModifiesQuery(accessor, line.Value, name.Value).Build(),
                _ => DoDeclaration()
            };

            return query;
        }

        public override IEnumerable<IComparable> Do(IDeclaration select)
        {
            // pattern matching argumentów
            var query = (Left, Right) switch
            {
                // Modifies(stmt, 'v')
                (IStatementDeclaration declaration, IArgument.VarName right) =>
                    new GetStatementsByVariable(accessor, right.Value).Build(declaration),

                // Modifies(1, variable)
                (IArgument.Line left, IVariableDeclaration) =>
                    new GetVariablesByLineNumber(accessor, left.Value).Build(),

                // Modifies(stmt, variable)
                (IStatementDeclaration left, IVariableDeclaration right) => BuildModifiesWithSelect(left, right),

                _ => throw new QueryNotSupported(this, $"Modifies({Left}, {Right}) is not supported.")
            };

            return query;

            IEnumerable<IComparable> BuildModifiesWithSelect(
                IStatementDeclaration left,
                IVariableDeclaration right
            )
            {
                // tu nastąpi samowywrotka przy zapytaniach, w których nie ma wartości z selecta
                // przykład: Select x such that modifes(s, v)

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

    private class GetStatements(IModifiesAccessor modifiesAccessor) : ModifiesQueryStatement
    {
        public override IEnumerable<Statement> Build(IStatementDeclaration statementDeclaration) =>
            statementDeclaration switch
            {
                IStatementDeclaration.Statement => modifiesAccessor.GetStatements(),
                IStatementDeclaration.Assign => modifiesAccessor.GetStatements().OfType<Assign>(),
                IStatementDeclaration.While => modifiesAccessor.GetStatements().OfType<While>(),
                IStatementDeclaration.If => modifiesAccessor.GetStatements().OfType<If>(),
                IStatementDeclaration.Call => modifiesAccessor.GetStatements().OfType<Call>(),
                _ => throw new ArgumentOutOfRangeException(nameof(statementDeclaration))
            };
    }

    private class GetVariablesByStatementType(IModifiesAccessor modifiesAccessor) : ModifiesQueryVariable
    {
        public override IEnumerable<Variable> Build(IStatementDeclaration statementDeclaration) =>
            statementDeclaration switch
            {
                IStatementDeclaration.Statement => modifiesAccessor.GetVariables<Statement>(),
                IStatementDeclaration.Assign => modifiesAccessor.GetVariables<Assign>(),
                IStatementDeclaration.While => modifiesAccessor.GetVariables<While>(),
                IStatementDeclaration.If => modifiesAccessor.GetVariables<If>(),
                IStatementDeclaration.Call => modifiesAccessor.GetVariables<Call>(),
                _ => throw new ArgumentOutOfRangeException(nameof(statementDeclaration))
            };
    }


    /// <summary>
    /// Get statements of given type by variable name.
    /// </summary>
    /// <param name="modifiesAccessor">Modifies accessor.</param>
    /// <param name="variableName">Variable name.</param>
    private class GetStatementsByVariable(IModifiesAccessor modifiesAccessor, string variableName)
        : ModifiesQueryStatement
    {
        public override IEnumerable<Statement> Build(IStatementDeclaration left)
        {
            return left switch
            {
                IStatementDeclaration.Statement => modifiesAccessor.GetStatements(variableName),
                IStatementDeclaration.Assign => modifiesAccessor.GetStatements(variableName).OfType<Assign>(),
                IStatementDeclaration.While => modifiesAccessor.GetStatements(variableName).OfType<While>(),
                IStatementDeclaration.If => modifiesAccessor.GetStatements(variableName).OfType<If>(),
                IStatementDeclaration.Call => modifiesAccessor.GetStatements(variableName).OfType<Call>(),
                _ => throw new ArgumentOutOfRangeException(nameof(left))
            };
        }
    }

    /// <summary>
    /// Gets variables modified by statement with given line number.
    /// </summary>
    /// <param name="modifiesAccessor">Modifies accessor.</param>
    /// <param name="line">Line number.</param>
    private class GetVariablesByLineNumber(IModifiesAccessor modifiesAccessor, int line)
    {
        public IEnumerable<Variable> Build() => modifiesAccessor.GetVariables(line);
    }

    /// <summary>
    /// Represents a modifies query that returns statements.
    /// </summary>
    private abstract class ModifiesQueryStatement
    {
        /// <summary>
        /// Builds a query.
        /// </summary>
        /// <param name="declaration"> The declaration to build the query for. </param>
        /// <returns> The query. </returns>
        public abstract IEnumerable<Statement> Build(IStatementDeclaration declaration);
    }

    private class BooleanModifiesQuery(IModifiesAccessor accessor, int line, string variableName)
    {
        public IEnumerable<IComparable> Build()
        {
            if (accessor.IsModified(line, variableName))
            {
                return Enumerable.Repeat<IComparable>(true, 1);
            }

            return Enumerable.Empty<Statement>();
        }
    }

    /// <summary>
    /// Represents a modifies query that returns variables.
    /// </summary>
    private abstract class ModifiesQueryVariable
    {
        /// <summary>
        /// Builds a query.
        /// </summary>
        /// <param name="declaration"> The declaration to build the query for. </param>
        /// <returns> The query. </returns>
        public abstract IEnumerable<Variable> Build(IStatementDeclaration declaration);
    }

    #endregion
}