using System.Text;
using Parsobober.Pkb.Relations.Dto;
using Parsobober.Pql.Query.Abstractions;
using Parsobober.Pql.Query.Arguments;

namespace Parsobober.Pql.Query.QueryResult;

internal class QueryTupleResult(IEnumerable<Dictionary<IDeclaration, IPkbDto>> result) : IQueryResult
{
    public class Factory : IQueryResultFactory
    {
        public IQueryResult Create(object query)
        {
            if (query is not IEnumerable<Dictionary<IDeclaration, IPkbDto>> result)
            {
                throw new IQueryResultFactory.QueryFactoryException("Query must be a boolean");
            }

            return new QueryTupleResult(result);
        }
    }

    public string Execute()
    {
        var sb = new StringBuilder();

        foreach (var r in result)
        {
            var inner = new StringBuilder();
            foreach (var (key, value) in r)
            {
                inner.Append($" {value}");
            }

            inner.Append(',');
            sb.Append(inner.ToString().Trim());
        }

        return sb.ToString();
    }
}