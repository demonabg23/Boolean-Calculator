namespace Logical_Expression_Interpreter.Structures.Node
{
    public class ExpressionNode
    {
        public string Value { get; set; }
        public ExpressionNode Left { get; set; }
        public ExpressionNode Right { get; set; }



        public ExpressionNode(string value)
        {
            Value = value;
        }
    }
}