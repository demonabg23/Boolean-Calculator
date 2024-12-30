using Logical_Expression_Interpreter.Structures;
using Logical_Expression_Interpreter.Structures.Node;

namespace Logical_Expression_Interpreter.HelpingCommands
{
    public class Parser
    {
        private readonly StringCommands _helper;

        public Parser(StringCommands helper)
        {
            _helper = helper ?? throw new ArgumentNullException(nameof(helper));
        }

        public ExpressionNode ParseExpressionNode(Tokenizer tokenizer)
        {
            if (tokenizer == null)
                throw new ArgumentNullException(nameof(tokenizer), "Tokenizer cannot be null.");

            var left = ParseTermNode(tokenizer);

            while (true)
            {
                var token = tokenizer.PeekNextToken();

                if (token == null || token != "|") break;

                tokenizer.GetNextToken(); // consume "|"
                var node = new ExpressionNode("|")
                {
                    Left = left,
                    Right = ParseTermNode(tokenizer)
                };
                left = node;
            }

            return left;
        }

        public ExpressionNode ParseTermNode(Tokenizer tokenizer)
        {
            if (tokenizer == null)
                throw new ArgumentNullException(nameof(tokenizer), "Tokenizer cannot be null.");

            var left = ParseFactorNode(tokenizer);

            while (true)
            {
                var token = tokenizer.PeekNextToken();

                if (token == null || token != "&") break;

                tokenizer.GetNextToken(); // consume "&"
                var node = new ExpressionNode("&")
                {
                    Left = left,
                    Right = ParseFactorNode(tokenizer)
                };
                left = node;
            }

            return left;
        }

        public ExpressionNode ParseFactorNode(Tokenizer tokenizer)
        {
            if (tokenizer == null)
                throw new ArgumentNullException(nameof(tokenizer), "Tokenizer cannot be null.");

            var token = tokenizer.GetNextToken();
            if (string.IsNullOrEmpty(token))
                throw new InvalidOperationException("Unexpected end of expression.");

            switch (token)
            {
                case "!":
                    return new ExpressionNode("!")
                    {
                        Left = ParseFactorNode(tokenizer)
                    };
                case "(":
                    var node = ParseExpressionNode(tokenizer);
                    token = tokenizer.GetNextToken();
                    if (token != ")")
                        _helper.ThrowError("Mismatched parentheses");
                    return node;
                default:
                    // It's either an identifier or something else
                    return new ExpressionNode(token);
            }
        }
    }
}
