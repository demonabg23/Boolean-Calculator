using Logical_Expression_Interpreter.Structures;
using Logical_Expression_Interpreter.Structures.CustomStructures;
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

                tokenizer.GetNextToken(); // consume '|'
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

                tokenizer.GetNextToken(); // consume '&'
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
                    {
                        var node = ParseExpressionNode(tokenizer);
                        var closing = tokenizer.GetNextToken();
                        if (closing != ")")
                            _helper.ThrowError("Mismatched parentheses");
                        return node;
                    }

                default:
                    {
                        var next = tokenizer.PeekNextToken();

                        if (next == "(")
                        {
                            return ParseFunctionCall(token, tokenizer);
                        }
                        else
                        {
                            return new ExpressionNode(token);
                        }
                    }
            }
        }

        private ExpressionNode ParseFunctionCall(string funcName, Tokenizer tokenizer)
        {
            var openParen = tokenizer.GetNextToken();
            if (openParen != "(")
                _helper.ThrowError($"Expected '(' after function name '{funcName}'.");

            var args = new CustomList<ExpressionNode>();

            var next = tokenizer.PeekNextToken();
            if (next != null && next != ")")
            {
                while (true)
                {
                    var arg = ParseExpressionNode(tokenizer);
                    args.Add(arg);

                    var maybeComma = tokenizer.PeekNextToken();
                    if (maybeComma == ",")
                    {
                        tokenizer.GetNextToken(); // consume ","
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            var closeParen = tokenizer.GetNextToken();
            if (closeParen != ")")
                _helper.ThrowError($"Expected ')' after function call '{funcName}'.");

            var callNode = new ExpressionNode(funcName)
            {
                IsFunctionCall = true, 
                Arguments = args.ToArray()
            };

            return callNode;
        }
    }
}
