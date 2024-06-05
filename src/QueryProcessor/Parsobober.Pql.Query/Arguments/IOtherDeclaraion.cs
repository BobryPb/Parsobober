namespace Parsobober.Pql.Query.Arguments;

public interface IOtherDeclaration : IDeclaration
{
    new static IDeclaration? Parse(string type, string name)
    {
        return type switch
        {
            "stmtLst" => new StatementList(name),
            "constant" => new Constant(name),
            "prog_line" => new ProgramLine(name),
            _ => null
        };
    }

    public readonly record struct StatementList(string Name) : IOtherDeclaration;
    
    public readonly record struct Constant(string Name) : IOtherDeclaration;
    
    public readonly record struct ProgramLine(string Name) : IOtherDeclaration;
}