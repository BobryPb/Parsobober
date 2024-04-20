﻿using Parsobober.Pkb.Ast;
using Parsobober.Simple.Parser.Abstractions;

namespace Parsobober.Simple.Parser.Extractor;

internal abstract class SimpleExtractor : ISimpleExtractor
{
    public virtual void Assign(TreeNode node)
    {
        return;
    }

    public virtual void Expr(TreeNode node)
    {
        return;
    }

    public virtual void Procedure(TreeNode node)
    {
        return;
    }

    public virtual void Stmt(TreeNode node)
    {
        return;
    }

    public virtual void Variable(TreeNode node)
    {
        return;
    }

    public virtual void While(TreeNode node)
    {
        return;
    }

    public virtual void Factor(TreeNode node)
    {
        return;
    }

    public virtual void StmtLst()
    {
        return;
    }
}