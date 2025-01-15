namespace Logical_Expression_Interpreter_UI.Structures.Node
{
    public class ExpressionNode
    {
        public string? Value { get; set; }
        public ExpressionNode? Left { get; set; }
        public ExpressionNode? Right { get; set; }
        public bool IsFunctionCall { get; set; }
        public ExpressionNode[] Arguments { get; set; }
        public string[]? Parameters { get; set; }


        public ExpressionNode(string? value, string[]? parameters = null)
        {
            Value = value;
            Parameters = parameters;
        }
    }
}