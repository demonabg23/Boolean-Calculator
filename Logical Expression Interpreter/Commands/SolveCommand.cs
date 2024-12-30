using Logical_Expression_Interpreter.HelpingCommands;
using Logical_Expression_Interpreter.Structures;
using Logical_Expression_Interpreter.Structures.CustomStructures;
using Logical_Expression_Interpreter.Structures.Node;

namespace Logical_Expression_Interpreter.Commands
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

        public int Execute(string input)
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

            var rootNode = _functionTable.Get(funcName);
            var variables = CollectVariables(rootNode);
            var variableMap = BuildVariableMap(variables, values);

            return Evaluate(rootNode, variableMap);
        }

        private string[] CollectVariables(ExpressionNode node)
        {
            var variables = new CustomSet();
            CollectVariablesRecursive(node, variables);
            return variables.ToArray();
        }

        private void CollectVariablesRecursive(ExpressionNode node, CustomSet variables)
        {
            if (node == null) return;

            if (node.Left == null && node.Right == null)
            {
                if (_helper.IsLetter(node.Value[0]))
                    variables.Add(node.Value);
            }
            else
            {
                CollectVariablesRecursive(node.Left, variables);
                CollectVariablesRecursive(node.Right, variables);
            }
        }

        private CustomDictionary BuildVariableMap(string[] variables, string[] values)
        {
            if (variables.Length != values.Length)
                throw new InvalidOperationException("Mismatched argument count.");

            var map = new CustomDictionary(variables.Length);
            for (var i = 0; i < variables.Length; i++)
            {
                map.Add(variables[i], int.Parse(values[i]));
            }
            return map;
        }

        private int Evaluate(ExpressionNode node, CustomDictionary variableMap)
        {
            if (node.Left == null && node.Right == null)
            {
                return variableMap.ContainsKey(node.Value) ? variableMap.Get(node.Value) : int.Parse(node.Value);
            }

            var leftResult = Evaluate(node.Left, variableMap);
            var rightResult = node.Right != null ? Evaluate(node.Right, variableMap) : 0;

            return node.Value switch
            {
                "&" => leftResult & rightResult,
                "|" => leftResult | rightResult,
                "!" => 1 - leftResult,
                _ => throw new InvalidOperationException($"Unknown operator: {node.Value}")
            };
        }
    }
}
