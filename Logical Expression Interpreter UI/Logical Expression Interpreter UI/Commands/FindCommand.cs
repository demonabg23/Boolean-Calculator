using Logical_Expression_Interpreter_UI.HelpingCommands;
using Logical_Expression_Interpreter_UI.Structures;
using Logical_Expression_Interpreter_UI.Structures.CustomStructures;
using Logical_Expression_Interpreter_UI.Structures.Node;

namespace Logical_Expression_Interpreter_UI.Commands
{
    public class FindCommand
    {
        private readonly StringCommands _helper;
        private readonly FunctionTable _functionTable;
        private static readonly string?[] VarNames = GenerateVariableNames();

        private const int MaxDepth = 3;

        public FindCommand(StringCommands helper, FunctionTable functionTable)
        {
            _helper = helper ?? throw new ArgumentNullException(nameof(helper));
            _functionTable = functionTable ?? throw new ArgumentNullException(nameof(functionTable));
        }

        public string Execute(string? input)
        {
            if (!_helper.StartsWithIgnoreCase(input, "FIND"))
                throw new InvalidOperationException("Invalid command. Must start with 'FIND'.");

            var content = _helper.Trim(_helper.Substring(input, 5, input!.Length - 5));
            var truthTable = ParseTruthTable(content);

            var variables = ExtractVariables(truthTable);
            var result = GenerateAndTestFunctionParallel(variables, truthTable);

            return result ?? "No matching function found";
        }

        private CustomList<(int[], int)> ParseTruthTable(string? input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input), "Input cannot be null.");

            var rows = new CustomList<(int[], int)>();

            if (_helper.IsQuoted(input))
            {
                var path = _helper.TrimQuotes(input);
                var fileContents = _helper.ReadFileLines(path);

                foreach (var line in fileContents)
                {
                    rows.Add(ParseLine(line));
                }
            }
            else
            {
                var lines = _helper.SplitBySemicolon(input);
                for (var i = 0; i < lines.Count; i++)
                {
                    rows.Add(ParseLine(lines.Get(i)));
                }
            }

            return rows;
        }

        private (int[], int) ParseLine(string? line)
        {
            var parts = _helper.SplitByDelimiter(line, ':');

            if (parts.Count != 2)
                throw new InvalidOperationException("Invalid line format. Expected operands and result separated by ':'.");

            var operands = ParseOperands(parts.Get(0));
            var result = ParseResult(parts.Get(1));

            return (operands, result);
        }

        private int[] ParseOperands(string? input)
        {
            var parts = _helper.SplitByComma(input);
            var operands = new int[parts.Length];

            for (var i = 0; i < parts.Length; i++)
            {
                operands[i] = _helper.ParseInt(parts[i]);
            }

            return operands;
        }

        private int ParseResult(string? input)
        {
            return _helper.ParseInt(input);
        }

        private CustomList<string?> ExtractVariables(CustomList<(int[], int)> truthTable)
        {
            var numInputs = truthTable.Get(0).Item1.Length;
            var variables = new CustomList<string?>();

            for (var i = 0; i < numInputs; i++)
            {
                variables.Add(VarNames[i]);
            }

            return variables;
        }

        private string? GenerateAndTestFunctionParallel(CustomList<string?> variables, CustomList<(int[], int)> truthTable)
        {
            for (var depth = 0; depth <= MaxDepth; depth++)
            {
                var candidates = GenerateExpressions(depth, variables.Count);

                foreach (var expr in candidates)
                {
                    if (MatchesAllRows(expr, truthTable))
                    {
                        return Serialize(expr);
                    }
                }
            }

            return null;
        }

        private CustomList<ExpressionNode> GenerateExpressions(int depth, int numInputs)
        {
            var results = new CustomList<ExpressionNode>();

            if (depth == 0)
            {
                for (var i = 0; i < numInputs; i++)
                {
                    results.Add(new ExpressionNode(VarNames[i]));
                }
                return results;
            }

            var smaller = GenerateExpressions(depth - 1, numInputs);

            foreach (var child in smaller)
            {
                var node = new ExpressionNode("!")
                {
                    Left = Clone(child)
                };
                results.Add(node);
            }

            foreach (var leftChild in smaller)
            {
                foreach (var rightChild in smaller)
                {
                    results.Add(new ExpressionNode("&")
                    {
                        Left = Clone(leftChild),
                        Right = Clone(rightChild)
                    });
                    results.Add(new ExpressionNode("|")
                    {
                        Left = Clone(leftChild),
                        Right = Clone(rightChild)
                    });
                }
            }

            var funcNames = _functionTable.GetAllFunctionNames();
            foreach (var fn in funcNames)
            {
                var defNode = _functionTable.Get(fn);
                var paramCount = defNode.Parameters?.Length ?? 0;
                var combos = GetArgumentCombinations(smaller, paramCount);

                for (var i = 0; i < combos.Count; i++)
                {
                    var callNode = new ExpressionNode(fn)
                    {
                        IsFunctionCall = true,
                        Arguments = combos.Get(i)
                    };
                    results.Add(callNode);
                }
            }

            return results;
        }

        private CustomList<ExpressionNode[]> GetArgumentCombinations(CustomList<ExpressionNode> items, int paramCount)
        {
            var results = new CustomList<ExpressionNode[]>();
            if (paramCount == 0)
            {
                results.Add([]);
                return results;
            }

            var current = new ExpressionNode[paramCount];
            GetArgCombosRecursive(items, paramCount, 0, current, results);
            return results;
        }

        private void GetArgCombosRecursive(CustomList<ExpressionNode> items, int paramCount, int index, ExpressionNode[] current, CustomList<ExpressionNode[]> results)
        {
            if (index == paramCount)
            {
                var copy = new ExpressionNode[paramCount];
                for (var i = 0; i < paramCount; i++)
                {
                    copy[i] = current[i];
                }
                results.Add(copy);
                return;
            }

            for (var i = 0; i < items.Count; i++)
            {
                current[index] = items.Get(i);
                GetArgCombosRecursive(items, paramCount, index + 1, current, results);
            }
        }

        private bool MatchesAllRows(ExpressionNode expr, CustomList<(int[], int)> truthTable)
        {
            for (var i = 0; i < truthTable.Count; i++)
            {
                var row = truthTable.Get(i);
                var variableMap = new CustomDictionary(row.Item1.Length);

                for (var j = 0; j < row.Item1.Length; j++)
                {
                    variableMap.Add(VarNames[j], row.Item1[j]);
                }

                var actual = Evaluate(expr, variableMap);
                if (actual != row.Item2)
                    return false;
            }

            return true;
        }

        private int Evaluate(ExpressionNode node, CustomDictionary variableMap)
        {
            if (node is { IsFunctionCall: true, Arguments: not null })
            {
                return EvaluateFunctionCall(node, variableMap);
            }

            if (node.Left == null && node.Right == null)
            {
                return variableMap.ContainsKey(node.Value) ? variableMap.Get(node.Value) : _helper.ParseInt(node.Value);
            }

            var leftVal = node.Left != null ? Evaluate(node.Left, variableMap) : 0;
            var rightVal = node.Right != null ? Evaluate(node.Right, variableMap) : 0;

            return node.Value switch
            {
                "&" => leftVal & rightVal,
                "|" => leftVal | rightVal,
                "!" => 1 - leftVal,
                _ => throw new InvalidOperationException($"Unknown operator: {node.Value}")
            };
        }

        private int EvaluateFunctionCall(ExpressionNode callNode, CustomDictionary variableMap)
        {
            var functionNode = _functionTable.Get(callNode.Value);

            if (functionNode.Parameters == null || callNode.Arguments == null)
                throw new InvalidOperationException($"Invalid function call for '{callNode.Value}'.");

            if (functionNode.Parameters.Length != callNode.Arguments.Length)
                throw new InvalidOperationException($"Parameter count mismatch for function '{callNode.Value}'.");

            var nestedMap = new CustomDictionary(functionNode.Parameters.Length);
            for (var i = 0; i < functionNode.Parameters.Length; i++)
            {
                nestedMap.Add(functionNode.Parameters[i], Evaluate(callNode.Arguments[i], variableMap));
            }

            return Evaluate(functionNode, nestedMap);
        }

        private ExpressionNode Clone(ExpressionNode node)
        {
            var clone = new ExpressionNode(node.Value)
            {
                IsFunctionCall = node.IsFunctionCall
            };

            if (node.Left != null)
                clone.Left = Clone(node.Left);

            if (node.Right != null)
                clone.Right = Clone(node.Right);

            if (node.Arguments == null!) return clone;
            clone.Arguments = new ExpressionNode[node.Arguments.Length];
            for (var i = 0; i < node.Arguments.Length; i++)
            {
                clone.Arguments[i] = Clone(node.Arguments[i]);
            }

            return clone;
        }

        private string? Serialize(ExpressionNode? expr)
        {
            if (expr is { IsFunctionCall: true } && expr.Arguments != null!)
            {
                var argStrings = new CustomList<string>(expr.Arguments.Length);
                foreach (var t in expr.Arguments)
                {
                    var argRepr = Serialize(t);
                    if (argRepr != null) argStrings.Add(argRepr);
                }

                var argsStr = _helper.Join(", ", argStrings);

                return $"{expr.Value}({argsStr})";
            }

            if (expr?.Left == null && expr?.Right == null)
            {
                return expr?.Value;
            }

            return expr.Value == "!" ? $"!({Serialize(expr.Left)})" : $"({Serialize(expr.Left)} {expr.Value} {Serialize(expr.Right)})";
        }

        public static string[] GenerateVariableNames()
        {
            var variables = new CustomList<string>();
            for (var i = 'a'; i <= 'z'; i++)
            {
                variables.Add(i.ToString());
            }
            return variables.ToArray();
        }
    }
}
