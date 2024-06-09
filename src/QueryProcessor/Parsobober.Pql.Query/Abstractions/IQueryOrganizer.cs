using Parsobober.Pkb.Relations.Dto;
using Parsobober.Pql.Query.Arguments;

namespace Parsobober.Pql.Query.Abstractions;

public interface IQueryOrganizer
{
    /// <summary>
    /// Organizes queries and select statement into query tree.
    /// </summary>
    /// <returns>Query tree</returns>
    IEnumerable<IPkbDto> Organize(IDeclaration select);

    bool OrganizeBoolean();
    
    IEnumerable<Dictionary<IDeclaration, IPkbDto>> OrganizerTuple(IEnumerable<IDeclaration> selects);
}