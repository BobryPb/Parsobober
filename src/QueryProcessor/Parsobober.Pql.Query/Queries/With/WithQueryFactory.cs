using Parsobober.Pkb.Relations.Abstractions.Accessors;
using Parsobober.Pkb.Relations.Dto;
using Parsobober.Pkb.Relations.Utilities;
using Parsobober.Pql.Query.Arguments;
using Parsobober.Pql.Query.Queries.Abstractions;
using Parsobober.Pql.Query.Queries.Exceptions;

namespace Parsobober.Pql.Query.Queries.With;

public class WithQueryFactory(IProgramContextAccessor accessor)
{
    public WithQuery Create(IDeclaration declaration, string attribute, string value)
    {
        return attribute switch
        {
            "stmt#" => new StatementLine(declaration, int.Parse(value), accessor),
            "varName" => new VariableName(declaration, value, accessor),
            "value" => new ConstantValue(declaration, int.Parse(value), accessor),
            "procName" => new ProcedureName(declaration, value, accessor),
            _ => throw new AttributeParseException(attribute)
        };
    }

    private class ConstantValue(IDeclaration declaration, int value, IProgramContextAccessor accessor)
        : WithQuery(declaration)
    {
        public override IEnumerable<IPkbDto> Do()
        {
            if (accessor.ConstantsEnumerable.Any(c => c == value))
            {
                return Enumerable.Repeat(value.ToConstant(), 1);
            }

            return Enumerable.Empty<IPkbDto>();
        }

        public override IQueryDeclaration ApplyAttribute(IQueryDeclaration queryDeclaration)
        {
            if (queryDeclaration.Left == Declaration || queryDeclaration.Right == Declaration)
            {
                return queryDeclaration.ReplaceArgument(Declaration, new Arguments.ConstantValue(value));
            }

            return queryDeclaration;
        }
    }

    public class ProcedureName(IDeclaration declaration, string name, IProgramContextAccessor accessor)
        : WithQuery(declaration)
    {
        public string Name => name;

        public override IEnumerable<IPkbDto> Do()
        {
            if (accessor.ProceduresDictionary.TryGetValue(name, out var procedure))
            {
                return Enumerable.Repeat(procedure.ToProcedure(), 1);
            }

            return Enumerable.Empty<IPkbDto>();
        }

        public override IQueryDeclaration ApplyAttribute(IQueryDeclaration queryDeclaration)
        {
            if (queryDeclaration.Left == Declaration || queryDeclaration.Right == Declaration)
            {
                return queryDeclaration.ReplaceArgument(Declaration, new Name(name));
            }

            return queryDeclaration;
        }
    }

    private class StatementLine(IDeclaration declaration, int line, IProgramContextAccessor accessor)
        : WithQuery(declaration)
    {
        public override IEnumerable<IPkbDto> Do()
        {
            if (accessor.StatementsDictionary.TryGetValue(line, out var statement))
            {
                return Enumerable.Repeat(statement.ToStatement(), 1);
            }

            return Enumerable.Empty<IPkbDto>();
        }

        public override IQueryDeclaration ApplyAttribute(IQueryDeclaration queryDeclaration)
        {
            if (queryDeclaration.Left == Declaration || queryDeclaration.Right == Declaration)
            {
                return queryDeclaration.ReplaceArgument(Declaration, new Line(line));
            }

            return queryDeclaration;
        }

#if DEBUG
        public override string ToString() => $"{Declaration.Name}.stmt# = {line}";
#endif
    }

    private class VariableName(IDeclaration declaration, string name, IProgramContextAccessor accessor)
        : WithQuery(declaration)
    {
        public override IEnumerable<IPkbDto> Do()
        {
            if (accessor.VariablesDictionary.TryGetValue(name, out var variable))
            {
                return Enumerable.Repeat(variable.ToVariable(), 1);
            }

            return Enumerable.Empty<IPkbDto>();
        }

        public override IQueryDeclaration ApplyAttribute(IQueryDeclaration queryDeclaration)
        {
            if (queryDeclaration.Left == Declaration || queryDeclaration.Right == Declaration)
            {
                return queryDeclaration.ReplaceArgument(Declaration, new Name(name));
            }

            return queryDeclaration;
        }
    }
}