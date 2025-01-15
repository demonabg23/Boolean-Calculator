using Logical_Expression_Interpreter_UI.Commands;
using Logical_Expression_Interpreter_UI.HelpingCommands;
using Logical_Expression_Interpreter_UI.Structures;
using Logical_Expression_Interpreter_UI.Structures.Node;

namespace Logical_Expression_Interpreter_UI;

public partial class Form1 : Form
{

    private readonly FunctionTable _functionTable;
    private ExpressionNode? _rootNode;
    private readonly StringCommands _helper;
    private readonly Parser _parser;

    public Form1()
    {
        InitializeComponent();

        _functionTable = new FunctionTable();
        _helper = new StringCommands();
        _parser = new Parser(_helper);
    }

    private void Visualize()
    {
        _rootNode = _functionTable.Get(textBoxFuncName.Text);
        panel1.Invalidate();
    }

    private void Builder_Click(object sender, EventArgs e)
    {
        var funcName = _helper.Trim(textBoxFuncName.Text);

        if (!_functionTable.Contains(funcName))
        {
            MessageBox.Show($@"Function '{funcName}' is not defined.");
            return;
        }

        Visualize();
    }

    private void DefineButton_Click(object sender, EventArgs e)
    {
        var input = _helper.Trim(textBoxDefinition.Text);

        if (input == null)
        {
            label1.Text = @"Please enter a function definition";
            return;
        }

        try
        {
            if (!_helper.StartsWithIgnoreCase(input, "DEFINE")) input = "DEFINE " + input;

            var command = new DefineCommand(_helper, _functionTable, _parser);
            command.Parse(input);

            textBoxFuncName.Text = command.FunctionName;

            label1.Text = $@"Defined function '{command.FunctionName}' with parameters: " +
                          $@"{string.Join(", ", command.Parameters)}. Expression body: {command.ExpressionBody}";

        }
        catch (Exception exception)
        {

            label1.Text = $@"Error: {exception.Message}";
        }
    }

    private void panel1_Paint(object sender, PaintEventArgs e)
    {
        if (_rootNode == null) return;

        var layoutRoot = BuildLayoutTree(_rootNode);

        var rt = new ReingoldTilford(nodeWidth: 2.0f, levelHeight: 60.0f);
        rt.Layout(layoutRoot);

        var xOffset = panel1.Width / 2f;

        DrawLayoutTree(e.Graphics, layoutRoot, xOffset, 50f);

    }

    private void DrawLayoutTree(Graphics g, LayoutNode node, float offsetX, float offsetY)
    {
        // node.X is in "tree space", so let's shift it by offsetX
        var x = offsetX + node.X * 50;  // multiply by 50 for bigger spacing
        var y = offsetY + node.Y;       // Y is already in pixels, from levelHeight

        // Draw edges first
        foreach (var child in node.Children)
        {
            var childX = offsetX + child.X * 50;
            var childY = offsetY + child.Y;

            g.DrawLine(Pens.Black, x, y, childX, childY);
            DrawLayoutTree(g, child, offsetX, offsetY);
        }

        const float radius = 20;
        var rect = new RectangleF(x - radius, y - radius, radius * 2, radius * 2);
        g.FillEllipse(Brushes.White, rect);
        g.DrawEllipse(Pens.Black, rect);

        string text;

        if (node.ExpressionNode.IsFunctionCall)
        {
            // If it's a function call node
            // show function name + (param1, param2, ...)
            text = node.ExpressionNode.Value ?? "?";
            if (node.ExpressionNode.Parameters is { Length: > 0 })
            {
                text += "(" + string.Join(", ", node.ExpressionNode.Parameters) + ")";
            }
        }
        else
        {
            // If it's an operator/variable/literal
            text = node.ExpressionNode.Value ?? "?";
        }

        var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        using var font = new Font("Arial", 9);
        g.DrawString(text, font, Brushes.Black, rect, sf);
    }

    private LayoutNode BuildLayoutTree(ExpressionNode root)
    {
        if (root == null)
            throw new ArgumentNullException(nameof(root));

        var wrapper = new LayoutNode(root);

        // If it's a function call with multiple arguments
        if (root is { IsFunctionCall: true, Arguments: not null })
        {
            foreach (var arg in root.Arguments)
            {
                wrapper.Children.Add(BuildLayoutTree(arg));
            }
        }
        else
        {
            // Otherwise, treat it as unary or binary operator or leaf
            if (root.Left != null)
            {
                wrapper.Children.Add(BuildLayoutTree(root.Left));
            }
            if (root.Right != null)
            {
                wrapper.Children.Add(BuildLayoutTree(root.Right));
            }
        }

        return wrapper;
    }

    private void textBoxDefinition_TextChanged(object sender, EventArgs e)
    {

    }
}