

namespace Logical_Expression_Interpreter_UI.Structures.Node;

public class LayoutNode
{
    public ExpressionNode ExpressionNode { get; set; }
    public List<LayoutNode> Children { get; set; } = [];

    public float X { get; set; }
    public float Y { get; set; }

    public LayoutNode(ExpressionNode? exprNode)
    {
        ExpressionNode = exprNode ?? throw new ArgumentNullException(nameof(exprNode));
    }

}