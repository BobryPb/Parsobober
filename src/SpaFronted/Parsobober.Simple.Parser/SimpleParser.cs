using Microsoft.Extensions.Logging;
using Parsobober.Pkb.Ast;
using Parsobober.Pkb.Ast.Abstractions;
using Parsobober.Simple.Lexer;
using Parsobober.Simple.Parser.Abstractions;

namespace Parsobober.Simple.Parser;

internal class SimpleParser(
    IEnumerator<LexicalToken> tokens,
    IAst ast,
    ILogger<SimpleParser> logger
) : ISimpleParser, ISimpleExtractor
{
    private LexicalToken _currentToken = tokens.Current;

    public IAst Parse()
    {
        logger.LogInformation("Starting parsing");
        GetToken();
        TreeNode procedureNode = Procedure();
        ast.SetParenthood(ast.Root, procedureNode);
        logger.LogInformation("Parsing completed successfully");
        return ast;
    }

    private void GetToken()
    {
        if (!tokens.MoveNext())
        {
            _currentToken = new LexicalToken("EOF", SimpleToken.WhiteSpace, 0);
            return;
        }

        _currentToken = tokens.Current;
    }

    private void Match(string value, SimpleToken type)
    {
        if (_currentToken.Type != type || _currentToken.Value != value)
        {
            throw new ParseException(_currentToken, type);
        }
        GetToken();
    }

    private void Match(SimpleToken type)
    {
        if (_currentToken.Type != type)
        {
            throw new ParseException(_currentToken, type);
        }
        GetToken();
    }

    private TreeNode CreateTreeNode(EntityType type, int lineNumber, string? attr = null)
    {
        TreeNode node = ast.CreateTreeNode(lineNumber, type);
        if (attr is not null)
        {
            ast.SetAttribute(node, attr);
        }

        return node;
    }

    private void AddNthChild(TreeNode parent, TreeNode child, int n)
    {
        int number = ast.SetParenthood(parent, child);
        if (number != n)
        {
            throw new Exception("Parser internal error - bad child position");
        }
    }

    public TreeNode Procedure()
    {
        Match("procedure", SimpleToken.Keyword);
        var procedureName = _currentToken.Value;
        var procedureLine = _currentToken.LineNumber;
        Match(SimpleToken.Name);
        var procedureNode = CreateTreeNode(EntityType.Procedure, procedureLine, procedureName);

        Match("{", SimpleToken.Separator);
        TreeNode stmtNode = StmtLst();
        Match("}", SimpleToken.Separator);
        AddNthChild(procedureNode, stmtNode, 1);
        return procedureNode;
    }

    public TreeNode StmtLst()
    {
        TreeNode node = Stmt();
        if (_currentToken.Value == "}")
        {
            return node;
        }

        TreeNode nextNode = StmtLst();
        ast.SetSiblings(node, nextNode);
        return node;
    }

    public TreeNode Stmt()
    {
        if (_currentToken.Value == "while")
        {
            return While();
        }
        return Assign();
    }

    public TreeNode While()
    {
        var whileLine = _currentToken.LineNumber;

        Match("while", SimpleToken.Keyword);
        var variableNode = Variable();
        Match("{", SimpleToken.Separator);
        TreeNode stmtNode = StmtLst();
        Match("}", SimpleToken.Separator);

        var whileNode = CreateTreeNode(EntityType.While, whileLine);
        AddNthChild(whileNode, variableNode, 1);
        AddNthChild(whileNode, stmtNode, 2);
        return whileNode;
    }

    public TreeNode Assign()
    {
        var line = _currentToken.LineNumber;

        var varNode = Variable();
        Match("=", SimpleToken.Operator);
        TreeNode exprNode = Expr();
        Match(";", SimpleToken.Separator);

        var assignNode = CreateTreeNode(EntityType.Assign, line);
        AddNthChild(assignNode, varNode, 1);
        AddNthChild(assignNode, exprNode, 2);
        return assignNode;
    }

    public TreeNode Expr()
    {
        TreeNode factorNode = Factor();
        if (_currentToken.Value == ";")
        {
            return factorNode;
        }
        var exprLine = _currentToken.LineNumber;
        Match("+", SimpleToken.Operator);
        TreeNode exprNode = Expr();

        var mainExprNode = CreateTreeNode(EntityType.Plus, exprLine, "+");
        AddNthChild(mainExprNode, factorNode, 1);
        AddNthChild(mainExprNode, exprNode, 2);
        return mainExprNode;
    }

    public TreeNode Factor()
    {
        if (_currentToken.Type == SimpleToken.Integer)
        {
            var factorLine = _currentToken.LineNumber;
            var factorValue = _currentToken.Value;

            Match(SimpleToken.Integer);
            return CreateTreeNode(EntityType.Constant, factorLine, factorValue);
        }
        else
        {
            return Variable();
        }
    }

    public TreeNode Variable()
    {
        var name = _currentToken.Value;
        var line = _currentToken.LineNumber;
        Match(SimpleToken.Name);

        return CreateTreeNode(EntityType.Variable, line, name);
    }
}