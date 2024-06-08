using Parsobober.Pql.Query.Abstractions;

namespace Parsobober.Pql.Query.QueryResult;

internal class QueryBooleanResult(bool result) : IQueryResult
{
    public class Factory : IQueryResultFactory
    {
        public IQueryResult Create(object query)
        {
            if (query is not bool result)
            {
                throw new IQueryResultFactory.QueryFactoryException("Query must be a boolean");
            }

            return new QueryBooleanResult(result);
        }
    }

    public string Execute() => result ? "true" : "false";
}