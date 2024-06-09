using System.Text;
using Parsobober.Pkb.Relations.Dto;
using Parsobober.Pql.Query.Abstractions;

namespace Parsobober.Pql.Query.QueryResult;

internal class QueryListResult(IEnumerable<IPkbDto> query) : IQueryResult
{
    public class Factory : IQueryResultFactory
    {
        public IQueryResult Create(object query)
        {
            if (query is not IEnumerable<IPkbDto> queryList)
            {
                throw new IQueryResultFactory.QueryFactoryException("Query must be a list of IComparable");
            }

            return new QueryListResult(queryList);
        }
    }

    public string Execute()
    {
        if (!query.Any())
        {
            return "none";
        }

        var stringBuilder = new StringBuilder();

        stringBuilder.AppendJoin(',', query.OrderBy(s => s, new PkbDtoComparer()));

        return stringBuilder.ToString();
    }
}