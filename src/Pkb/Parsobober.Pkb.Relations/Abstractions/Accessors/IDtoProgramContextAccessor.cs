using Parsobober.Pkb.Relations.Dto;

namespace Parsobober.Pkb.Relations.Abstractions.Accessors;

public interface IDtoProgramContextAccessor
{
    IEnumerable<Statement> Statements { get; }
    IEnumerable<Assign> Assigns { get; }
    IEnumerable<While> Whiles { get; }
    IEnumerable<Variable> Variables { get; }
}