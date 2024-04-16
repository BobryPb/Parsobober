using Microsoft.Extensions.Logging;
using Parsobober.Pkb.Ast;
using Parsobober.Pkb.Relations.Abstractions.Accessors;
using Parsobober.Pkb.Relations.Abstractions.Creators;

namespace Parsobober.Pkb.Relations.Implementations;

public class UsesRelation(
    ILogger<ParentRelation> logger,
    IProgramContextAccessor programContext
) : IUsesCreator
{
    /// <summary>
    /// Stores uses relation between statement and variables it uses.
    /// </summary>
    /// <remarks>[statement line number,  list of variable names].</remarks>
    private readonly Dictionary<int, List<string>> _usesDictionary = new();

    public void SetUses(TreeNode user, TreeNode variable)
    {
        // Check types
        if (!user.Type.IsStatement())
        {
            logger.LogError(
                "Statement uses relation can only be established between statement and variable node. ({user} must be statement)",
                user);

            throw new ArgumentException(
                $"User node type {user.Type} is different than any of required: {EntityType.Statement} types.");
        }

        if (!variable.Type.IsVariable())
        {
            logger.LogError(
                "Statement uses relation can only be established between statement and variable node. ({variable} must be variable)",
                variable);

            throw new ArgumentException(
                $"Variable node type {variable.Type} is different than required: {EntityType.Variable}.");
        }

        // Check required attribute
        if (variable.Attribute is null)
        {
            logger.LogError("Variable must have an attribute. ({node})", variable);

            throw new ArgumentNullException(variable.Attribute);
        }

        // Add to dictionary
        if (_usesDictionary.TryGetValue(user.LineNumber, out var variableList))
        {
            if (variableList.Contains(variable.Attribute))
            {
                return;
            }

            variableList.Add(variable.Attribute);

            return;
        }

        _usesDictionary.Add(user.LineNumber, [variable.Attribute]);
    }
}