using Logical_Expression_Interpreter_UI.HelpingCommands;
using Logical_Expression_Interpreter_UI.Structures;
using Logical_Expression_Interpreter_UI.Structures.CustomStructures;
using Logical_Expression_Interpreter_UI.Structures.Node;

namespace Logical_Expression_Interpreter_UI.Commands
{
    public class SolveCommand
    {
        private readonly StringCommands _helper;
        private readonly FunctionTable _functionTable;

        public SolveCommand(StringCommands helper, FunctionTable functionTable)
        {
            _helper = helper;
            _functionTable = functionTable;
        }

        public int Execute(string? input)
        {
            if (!_helper.StartsWithIgnoreCase(input, "SOLVE"))
                throw new InvalidOperationException("Invalid command. Must start with 'SOLVE'.");

            var openParen = _helper.FindCharacter(input, '(');
            var closeParen = _helper.FindCharacter(input, ')');
            if (openParen == -1 || closeParen == -1 || closeParen < openParen)
                throw new InvalidOperationException("Invalid syntax. Missing parentheses.");

            var funcName = _helper.Trim(_helper.Substring(input, 6, openParen - 6));
            var args = _helper.Trim(_helper.Substring(input, openParen + 1, closeParen - openParen - 1));
            var values = _helper.SplitByComma(args);

            if (!_functionTable.Contains(funcName))
                throw new InvalidOperationException($"Function '{funcName}' is not defined.");

            var functionNode = _functionTable.Get(funcName);

            if (functionNode.Parameters == null)
                throw new InvalidOperationException($"Function '{funcName}' has no parameters.");

            if (functionNode.Parameters.Length != values.Length)
                throw new InvalidOperationException($"Function '{funcName}' requires {functionNode.Parameters.Length} arguments, but {values.Length} were provided.");

            var variableMap = BuildVariableMap(functionNode.Parameters, values);
            return Evaluate(functionNode, variableMap);
        }

        private CustomDictionary BuildVariableMap(string?[] parameters, string?[] values)
        {
            if (parameters.Length != values.Length)
                throw new InvalidOperationException("Mismatched argument count.");

            var map = new CustomDictionary(parameters.Length);
            for (var i = 0; i < parameters.Length; i++)
            {
                map.Add(parameters[i], int.Parse(values[i]));
            }
            return map;
        }

        private int Evaluate(ExpressionNode node, CustomDictionary variableMap)
        {
            if (node.IsFunctionCall)
            {
                return EvaluateNestedFunction(node, variableMap);
            }

            if (node.Left == null && node.Right == null)
            {
                if (variableMap.ContainsKey(node.Value))
                    return variableMap.Get(node.Value);

                return int.Parse(node.Value);
            }

            var leftResult = (node.Left != null) ? Evaluate(node.Left, variableMap) : 0;
            var rightResult = (node.Right != null) ? Evaluate(node.Right, variableMap) : 0;

            return node.Value switch
            {
                "&" => leftResult & rightResult,
                "|" => leftResult | rightResult,
                "!" => 1 - leftResult,
                _ => throw new InvalidOperationException($"Unknown operator: {node.Value}")
            };
        }

        private int EvaluateNestedFunction(ExpressionNode callNode, CustomDictionary variableMap)
        {
            var functionName = callNode.Value;
            var functionNode = _functionTable.Get(functionName);

            if (functionNode.Parameters == null)
                throw new InvalidOperationException($"Function '{functionName}' has no parameters.");

            if (callNode.Arguments == null)
                throw new InvalidOperationException($"Function call '{functionName}' has no arguments array.");

            if (functionNode.Parameters.Length != callNode.Arguments.Length)
                throw new InvalidOperationException(
                    $"Function '{functionName}' requires {functionNode.Parameters.Length} arguments, " +
                    $"but {callNode.Arguments.Length} were provided."
                );

            var nestedMap = new CustomDictionary(functionNode.Parameters.Length);
            for (var i = 0; i < functionNode.Parameters.Length; i++)
            {
                var paramName = functionNode.Parameters[i];
                var argValue = Evaluate(callNode.Arguments[i], variableMap);
                nestedMap.Add(paramName, argValue);
            }

            return Evaluate(functionNode, nestedMap);
        }


    }
}
