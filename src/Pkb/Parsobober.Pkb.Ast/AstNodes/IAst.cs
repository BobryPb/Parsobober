namespace Parsobober.Pkb.Ast.AstNodes;

public interface IAst
{
    TreeNode CreateTNode(int lineNumber, EntityType type);

    void SetRoot(TreeNode node);
    void SetAttr(TreeNode node, String attr);
    void SetSibling(TreeNode left, TreeNode right);
    int SetParenthood(TreeNode parent, TreeNode child);

    TreeNode? GetRoot();
    TreeNode GetChildN(TreeNode node, int n);
}