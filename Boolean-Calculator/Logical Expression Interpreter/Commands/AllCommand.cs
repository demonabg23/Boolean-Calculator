using Logical_Expression_Interpreter.HelpingCommands;
using Logical_Expression_Interpreter.Structures;
using Logical_Expression_Interpreter.Structures.CustomStructures;
using Logical_Expression_Interpreter.Structures.Node;

namespace Logical_Expression_Interpreter.Commands
{
    public class AllCommand
    {
        private readonly StringCommands _helper;
        private readonly FunctionTable _functionTable;

        public AllCommand(StringCommands helper, FunctionTable functionTable)
        {
            _helper = helper ?? throw new ArgumentNullException(nameof(helper));
            _functionTable = functionTable ?? throw new ArgumentNullException(nameof(functionTable));
        }

        public void Execute(string input)
        {
            if (!_helper.StartsWithIgnoreCase(input, "ALL"))
                throw new InvalidOperationException("Invalid command. Must start with 'ALL'.");

            var funcName = _helper.Trim(_helper.Substring(input, 4, input.Length - 4));
            if (!_functionTable.Contains(funcName))
                throw new InvalidOperationException($"Function '{funcName}' is not defined.");

            var rootNode = _functionTable.Get(funcName);
            var parameters = CollectVariables(rootNode);

            var combinations = GenerateTruthTable(parameters.Length);

            Console.WriteLine($"{string.Join(", ", parameters)} : {funcName}");

            foreach (var combination in combinations)
            {
                var variableMap = BuildVariableMap(parameters, combination);
                var result = Evaluate(rootNode, variableMap);
                Console.WriteLine($"{string.Join(", ", combination)} : {result}");
            }
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

        private int[][] GenerateTruthTable(int variableCount)
        {
            var rows = 1 << variableCount; // 2^variableCount
            var table = new int[rows][];
            for (var i = 0; i < rows; i++)
            {
                table[i] = new int[variableCount];
                for (var j = 0; j < variableCount; j++)
                {
                    table[i][j] = (i >> (variableCount - j - 1)) & 1;
                }
            }
            return table;
        }

        private CustomDictionary BuildVariableMap(string[] variables, int[] values)
        {
            if (variables.Length != values.Length)
                throw new InvalidOperationException("Mismatched argument count.");

            var map = new CustomDictionary(variables.Length);
            for (var i = 0; i < variables.Length; i++)
            {
                map.Add(variables[i], values[i]);
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
