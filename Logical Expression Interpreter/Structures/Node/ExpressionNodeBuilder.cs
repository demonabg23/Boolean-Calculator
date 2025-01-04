using Logical_Expression_Interpreter.HelpingCommands;

namespace Logical_Expression_Interpreter.Structures.Node
{
    public class ExpressionNodeBuilder
    {
        private readonly Tokenizer _tokenizer;
        private readonly StringCommands _helpers = new();
        private readonly Parser _parser;

        public ExpressionNodeBuilder(string? expression, Parser parser)
        {
            _tokenizer = new Tokenizer(expression, _helpers);
            _parser = parser;
        }

        public ExpressionNode Build()
        {
            return _parser.ParseExpressionNode(_tokenizer);
        }
    }
}