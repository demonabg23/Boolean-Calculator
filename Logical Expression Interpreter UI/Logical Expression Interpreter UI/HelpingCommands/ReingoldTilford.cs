using Logical_Expression_Interpreter_UI.Structures.Node;

namespace Logical_Expression_Interpreter_UI.HelpingCommands;

public class ReingoldTilford
{
    private readonly float _nodeWidth;   
    private readonly float _levelHeight; 

    public ReingoldTilford(float nodeWidth = 1.0f, float levelHeight = 1.0f)
    {
        _nodeWidth = nodeWidth;
        _levelHeight = levelHeight;
    }

    public void Layout(LayoutNode root)
    {
        FirstWalk(root, 0);

        SecondWalk(root, 0, 0);
    }

    private void FirstWalk(LayoutNode node, int depth)
    {
        if (node.Children.Count == 0)
        {
            node.X = 0;
            return;
        }

        // Recurse
        foreach (var t in node.Children)
        {
            FirstWalk(t, depth + 1);
        }

        if (node.Children.Count == 1)
        {
            node.X = node.Children[0].X;
        }
        else
        {
            // More than one child => position X halfway between first and last child
            var leftX = node.Children[0].X;
            var rightX = node.Children[^1].X;
            node.X = (leftX + rightX) / 2f;
        }

        // Adjust children spacing if siblings overlap
        ResolveSiblingConflicts(node);
    }

    private void ResolveSiblingConflicts(LayoutNode node)
    {
        // For each child i from the second to the last check left sibling overlap
        for (var i = 1; i < node.Children.Count; i++)
        {
            var leftChild = node.Children[i - 1];
            var rightChild = node.Children[i];

            var spacing = _nodeWidth; // minimal gap you want between sibling subtrees
            var diff = (leftChild.X + spacing) - rightChild.X;

            if (diff >= 0)
            {
                // Shift this child's subtree to the right
                ShiftSubtree(rightChild, diff);
            }
        }
    }

    private void ShiftSubtree(LayoutNode node, float shift)
    {
        node.X += shift;
        // Recursively shift node's children
        foreach (var child in node.Children)
        {
            ShiftSubtree(child, shift);
        }
    }

    private void SecondWalk(LayoutNode node, int depth, float modSum)
    {
        // Apply the "mod" offset plus the parent's modSum
        var finalX = node.X + modSum;
        node.X = finalX;
        node.Y = depth * _levelHeight; // place each level some distance apart

        // Recurse
        foreach (var child in node.Children)
        {
            SecondWalk(child, depth + 1, modSum);
        }
    }
}