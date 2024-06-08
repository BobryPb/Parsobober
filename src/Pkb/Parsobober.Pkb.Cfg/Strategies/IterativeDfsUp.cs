using Parsobober.Pkb.Cfg.Abstractions;

namespace Parsobober.Pkb.Cfg.Strategies;

public class IterativeDfsUp : ICfgTraversalStrategy
{
    public IEnumerable<ICfgNode> Traverse(ICfgNode startNode)
    {
        var visited = new HashSet<ICfgNode>();
        var stack = new Stack<ICfgNode>();
        var result = new List<ICfgNode>();
        var removeStartNodeFromResult = true;

        stack.Push(startNode);

        while (stack.Count > 0)
        {
            var node = stack.Pop();

            if (!visited.Add(node))
            {
                continue;
            }

            result.Add(node);

            foreach (var previousNode in node.Previous)
            {
                if (removeStartNodeFromResult && previousNode == startNode)
                {
                    removeStartNodeFromResult = false;
                    continue;
                }
                if (!visited.Contains(previousNode))
                {
                    stack.Push(previousNode);
                }
            }
        }

        if (removeStartNodeFromResult)
        {
            result.Remove(startNode);
        }

        return result;
    }
}