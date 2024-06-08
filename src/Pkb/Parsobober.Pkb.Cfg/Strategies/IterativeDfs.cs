using Parsobober.Pkb.Cfg.Abstractions;

namespace Parsobober.Pkb.Cfg.Strategies;

public class IterativeDfs : ICfgTraversalStrategy
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

            foreach (var nextNode in node.Next)
            {
                if (removeStartNodeFromResult && nextNode == startNode)
                {
                    removeStartNodeFromResult = false;
                    continue;
                }
                if (!visited.Contains(nextNode))
                {
                    stack.Push(nextNode);
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