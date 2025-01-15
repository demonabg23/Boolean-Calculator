using Logical_Expression_Interpreter_UI.HelpingCommands;
using Logical_Expression_Interpreter_UI.Structures;
using Logical_Expression_Interpreter_UI.Structures.CustomStructures;
using Logical_Expression_Interpreter_UI.Structures.Node;

namespace Logical_Expression_Interpreter_UI.Commands
{
    public class FindCommandGA
    {
        private const int PopulationSize = 50;
        private const int MaxGenerations = 1000;
        private const double CrossoverRate = 0.8;
        private const double MutationRate = 0.1;
        private const int MaxDepth = 3;

        private static readonly string[] VarNames = GenerateVariableNames();

        private readonly StringCommands _helper;
        private readonly FunctionTable _functionTable;

        public FindCommandGA(StringCommands helper, FunctionTable functionTable)
        {
            _helper = helper ?? throw new ArgumentNullException(nameof(helper));
            _functionTable = functionTable ?? throw new ArgumentNullException(nameof(functionTable));
        }

        public string Execute(string input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input), "FIND command cannot be null.");

            if (!_helper.StartsWithIgnoreCase(input, "FIND"))
                throw new InvalidOperationException("Invalid command. Must start with 'FIND'.");

            // Remove "FIND " from the front and trim
            if (input.Length <= 5)
                throw new InvalidOperationException("No data provided after 'FIND'.");

            var content = _helper.Substring(input, 5, input.Length - 5);
            content = _helper.Trim(content);

            // Parse the truth table either from a file or inline
            var truthTable = ParseTruthTable(content);
            if (truthTable.NumInputs <= 0)
            {
                return "No expression possible: Truth table has 0 inputs.";
            }

            var expression = GeneticSearch(truthTable, truthTable.NumInputs);

            return expression != null
                ? SerializeExpression(expression)
                : "No expression found within generation limit.";
        }

        /// <summary>
        /// Depending on whether the content is in quotes, we either read from file or parse inline.
        /// </summary>
        private TruthTable ParseTruthTable(string data)
        {
            if (data == null)
            {
                return new TruthTable(0, new int[0][]);
            }

            var rowList = new CustomList<int[]>();

            if (_helper.IsQuoted(data))
            {
                var path = _helper.TrimQuotes(data);
                var fileLines = _helper.ReadFileLines(path);
                foreach (var t in fileLines)
                {
                    ParseSingleLine(t, rowList);
                }
            }
            else
            {
                // Otherwise assume inline semicolon-delimited data
                var lines = _helper.SplitBySemicolon(data);
                for (var i = 0; i < lines.Count; i++)
                {
                    var line = lines.Get(i);
                    ParseSingleLine(line, rowList);
                }
            }

            if (rowList.Count == 0)
            {
                // no valid data => 0 inputs
                return new TruthTable(0, new int[0][]);
            }

            var numInputs = rowList.Get(0).Length - 1;

            // convert rowList => int[][]
            var finalRows = new int[rowList.Count][];
            for (int i = 0; i < rowList.Count; i++)
            {
                finalRows[i] = rowList.Get(i);
            }

            return new TruthTable(numInputs, finalRows);
        }

        /// <summary>
        /// Helper that parses a single line of the form "0,0,1:1" and appends to rowList.
        /// </summary>
        private void ParseSingleLine(string line, CustomList<int[]> rowList)
        {
            if (line == null) return;

            var colonPos = _helper.FindCharacter(line, ':');
            if (colonPos < 0) return; // or throw an exception

            var leftPart = _helper.Substring(line, 0, colonPos);
            var rightPart = _helper.Substring(line, colonPos + 1, line.Length - (colonPos + 1));

            var leftTokens = _helper.SplitByComma(leftPart);
            var inputBits = new int[leftTokens.Length];
            for (int j = 0; j < leftTokens.Length; j++)
            {
                inputBits[j] = _helper.ParseInt(leftTokens[j]);
            }

            var outputStr = _helper.Trim(rightPart);
            var outputBit = _helper.ParseInt(outputStr);

            var row = new int[inputBits.Length + 1];
            for (int j = 0; j < inputBits.Length; j++)
            {
                row[j] = inputBits[j];
            }
            row[^1] = outputBit;

            rowList.Add(row);
        }

        /// <summary>
        /// GA main loop: 
        ///  1) initial population
        ///  2) evolve up to max gens
        ///  3) return best match
        /// </summary>
        private ExpressionNode GeneticSearch(TruthTable table, int numInputs)
        {
            var population = GenerateInitialPopulation(numInputs, PopulationSize);

            var maxScore = table.Rows.Length;

            for (var gen = 1; gen <= MaxGenerations; gen++)
            {
                // Evaluate (expr, score)
                var scoredPop = new CustomList<(ExpressionNode, int)>(population.Count);
                for (var i = 0; i < population.Count; i++)
                {
                    var expr = population.Get(i);
                    var score = Fitness(expr, table);
                    scoredPop.Add((expr, score));
                }

                // find best
                var bestIndex = 0;
                var bestScore = -1;
                for (var i = 0; i < scoredPop.Count; i++)
                {
                    var (candidate, candidateScore) = scoredPop.Get(i);
                    if (candidateScore > bestScore)
                    {
                        bestIndex = i;
                        bestScore = candidateScore;
                    }
                }

                // perfect match?
                if (bestScore == maxScore)
                {
                    return scoredPop.Get(bestIndex).Item1;
                }

                var newPopulation = new CustomList<ExpressionNode>(PopulationSize) {
                    // keep the best (elitism)
                    CloneExpression(scoredPop.Get(bestIndex).Item1)
                };

                // fill rest
                while (newPopulation.Count < PopulationSize)
                {
                    var parentA = SelectParent(scoredPop);
                    var parentB = SelectParent(scoredPop);

                    var childA = CloneExpression(parentA);
                    var childB = CloneExpression(parentB);

                    if (RandomDouble() < CrossoverRate)
                    {
                        Crossover(childA, childB);
                    }

                    if (RandomDouble() < MutationRate)
                    {
                        Mutate(childA, numInputs);
                    }
                    if (RandomDouble() < MutationRate)
                    {
                        Mutate(childB, numInputs);
                    }

                    newPopulation.Add(childA);
                    if (newPopulation.Count < PopulationSize)
                    {
                        newPopulation.Add(childB);
                    }
                }

                population = newPopulation;
            }

            // no solution in time
            return null;
        }

        private CustomList<ExpressionNode> GenerateInitialPopulation(int numInputs, int populationSize)
        {
            if (numInputs < 1)
                throw new InvalidOperationException("Cannot generate expressions with zero inputs.");

            var pop = new CustomList<ExpressionNode>(populationSize);
            for (var i = 0; i < populationSize; i++)
            {
                pop.Add(GenerateRandomExpression(MaxDepth, numInputs));
            }
            return pop;
        }

        private int Fitness(ExpressionNode expr, TruthTable table)
        {
            if (expr == null) return 0;

            var score = 0;
            foreach (var row in table.Rows)
            {
                var expected = row[^1];
                var varMap = new CustomDictionary(table.NumInputs);
                for (int j = 0; j < table.NumInputs; j++)
                {
                    varMap.Add(VarNames[j], row[j]);
                }

                var actual = EvaluateExpression(expr, varMap);
                if (actual == expected)
                {
                    score++;
                }
            }
            return score;
        }

        private ExpressionNode SelectParent(CustomList<(ExpressionNode, int)> scoredPop)
        {
            const int tournamentSize = 3;
            ExpressionNode bestExpr = null;
            var bestScore = -1;

            for (int i = 0; i < tournamentSize; i++)
            {
                var index = RandomInt(0, scoredPop.Count);
                var (expr, score) = scoredPop.Get(index);
                if (score > bestScore)
                {
                    bestExpr = expr;
                    bestScore = score;
                }
            }
            return bestExpr;
        }

        private void Crossover(ExpressionNode a, ExpressionNode b)
        {
            if (a == null || b == null) return;

            var nodeA = GetRandomNode(a);
            var nodeB = GetRandomNode(b);
            if (nodeA == null || nodeB == null) return;

            // swap
            var tempVal = nodeA.Value;
            var tempLeft = nodeA.Left;
            var tempRight = nodeA.Right;
            var tempIsFunc = nodeA.IsFunctionCall;
            var tempArgs = nodeA.Arguments;

            nodeA.Value = nodeB.Value;
            nodeA.Left = nodeB.Left;
            nodeA.Right = nodeB.Right;
            nodeA.IsFunctionCall = nodeB.IsFunctionCall;
            nodeA.Arguments = nodeB.Arguments;

            nodeB.Value = tempVal;
            nodeB.Left = tempLeft;
            nodeB.Right = tempRight;
            nodeB.IsFunctionCall = tempIsFunc;
            nodeB.Arguments = tempArgs;
        }

        private void Mutate(ExpressionNode node, int numInputs)
        {
            if (node == null) return;

            var target = GetRandomNode(node);
            if (target == null) return;

            if (RandomDouble() < 0.5)
            {
                // replace subtree
                var newSubtree = GenerateRandomExpression(2, numInputs);
                if (newSubtree == null) return;

                target.Value = newSubtree.Value;
                target.IsFunctionCall = newSubtree.IsFunctionCall;
                target.Left = newSubtree.Left;
                target.Right = newSubtree.Right;
                target.Arguments = newSubtree.Arguments;
            }
            else
            {
                // flip operator or pick new function
                if (IsOperator(target.Value))
                {
                    target.Value = GetRandomOperator();
                }
                else if (target.IsFunctionCall)
                {
                    var allFns = _functionTable.GetAllFunctionNames();
                    if (allFns.Count > 0)
                    {
                        var rndIdx = RandomInt(0, allFns.Count);
                        target.Value = allFns.Get(rndIdx);
                    }
                }
            }
        }

        private ExpressionNode GenerateRandomExpression(int maxDepth, int numInputs)
        {
            if (maxDepth <= 0)
            {
                var idx = RandomInt(0, numInputs);
                return new ExpressionNode(VarNames[idx]);
            }

            var pick = RandomDouble();
            if (pick < 0.3)
            {
                // variable
                var idx = RandomInt(0, numInputs);
                return new ExpressionNode(VarNames[idx]);
            }
            else if (pick < 0.6)
            {
                // operator
                var op = GetRandomOperator();
                if (op == "!")
                {
                    return new ExpressionNode("!")
                    {
                        Left = GenerateRandomExpression(maxDepth - 1, numInputs)
                    };
                }
                else
                {
                    return new ExpressionNode(op)
                    {
                        Left = GenerateRandomExpression(maxDepth - 1, numInputs),
                        Right = GenerateRandomExpression(maxDepth - 1, numInputs)
                    };
                }
            }
            else
            {
                var allFns = _functionTable.GetAllFunctionNames();
                if (allFns.Count == 0)
                {
                    // fallback => operator
                    return GenerateRandomExpression(maxDepth - 1, numInputs);
                }

                var randomFnIndex = RandomInt(0, allFns.Count);
                var fnName = allFns.Get(randomFnIndex);

                var fnNode = _functionTable.Get(fnName);
                var paramCount = fnNode.Parameters?.Length ?? 0;

                var args = new ExpressionNode[paramCount];
                for (var i = 0; i < paramCount; i++)
                {
                    args[i] = GenerateRandomExpression(maxDepth - 1, numInputs);
                }

                return new ExpressionNode(fnName)
                {
                    IsFunctionCall = true,
                    Arguments = args
                };
            }
        }

        private int EvaluateExpression(ExpressionNode node, CustomDictionary varMap)
        {
            if (node is { IsFunctionCall: true, Arguments: not null })
            {
                return EvaluateFunctionCall(node, varMap);
            }

            if (node.Left == null && node.Right == null)
            {
                return varMap.ContainsKey(node.Value) ? varMap.Get(node.Value) : _helper.ParseInt(node.Value);
            }

            var leftVal = node.Left != null ? EvaluateExpression(node.Left, varMap) : 0;
            var rightVal = node.Right != null ? EvaluateExpression(node.Right, varMap) : 0;

            return node.Value switch
            {
                "&" => leftVal & rightVal,
                "|" => leftVal | rightVal,
                "!" => 1 - leftVal,
                _ => 0
            };
        }

        private int EvaluateFunctionCall(ExpressionNode callNode, CustomDictionary varMap)
        {
            var functionNode = _functionTable.Get(callNode.Value);
            if (functionNode.Parameters == null || callNode.Arguments == null)
                return 0;

            var nestedMap = new CustomDictionary(functionNode.Parameters.Length);
            for (var i = 0; i < functionNode.Parameters.Length; i++)
            {
                var param = functionNode.Parameters[i];
                var argVal = EvaluateExpression(callNode.Arguments[i], varMap);
                nestedMap.Add(param, argVal);
            }

            return EvaluateExpression(functionNode, nestedMap);
        }

        private ExpressionNode GetRandomNode(ExpressionNode root)
        {
            var allNodes = new CustomList<ExpressionNode>();
            CollectNodes(root, allNodes);

            if (allNodes.Count == 0) return root;

            var idx = RandomInt(0, allNodes.Count);
            return allNodes.Get(idx);
        }

        private void CollectNodes(ExpressionNode node, CustomList<ExpressionNode> list)
        {
            if (node == null) return;

            list.Add(node);

            if (node is { IsFunctionCall: true, Arguments: not null })
            {
                for (int i = 0; i < node.Arguments.Length; i++)
                {
                    CollectNodes(node.Arguments[i], list);
                }
            }
            if (node.Left != null)
            {
                CollectNodes(node.Left, list);
            }
            if (node.Right != null)
            {
                CollectNodes(node.Right, list);
            }
        }

        private ExpressionNode CloneExpression(ExpressionNode node)
        {
            if (node == null) return null;

            var clone = new ExpressionNode(node.Value)
            {
                IsFunctionCall = node.IsFunctionCall
            };
            if (node.Left != null) clone.Left = CloneExpression(node.Left);
            if (node.Right != null) clone.Right = CloneExpression(node.Right);

            if (node.Arguments != null)
            {
                clone.Arguments = new ExpressionNode[node.Arguments.Length];
                for (int i = 0; i < node.Arguments.Length; i++)
                {
                    clone.Arguments[i] = CloneExpression(node.Arguments[i]);
                }
            }
            return clone;
        }

        private string SerializeExpression(ExpressionNode expr)
        {
            if (expr == null) return string.Empty;

            if (expr.IsFunctionCall && expr.Arguments != null)
            {
                var customList = new CustomList<string>(expr.Arguments.Length);
                foreach (var t in expr.Arguments)
                {
                    customList.Add(SerializeExpression(t));
                }
                var argsStr = _helper.Join(", ", customList);
                return $"{expr.Value}({argsStr})";
            }

            if (expr.Left == null && expr.Right == null)
            {
                return expr.Value;
            }

            return expr.Value == "!"
                ? $"!({SerializeExpression(expr.Left)})"
                : $"({SerializeExpression(expr.Left)} {expr.Value} {SerializeExpression(expr.Right)})";
        }

        private bool IsOperator(string val) => val is "&" or "|" or "!";

        private string GetRandomOperator()
        {
            var ops = new[] { "&", "|", "!" };
            var idx = RandomInt(0, ops.Length);
            return ops[idx];
        }

        private double RandomDouble()
        {
            return 0.01 * (RandomInt(0, 100));
        }

        private int RandomInt(int min, int maxExclusive)
        {
            if (maxExclusive <= min)
                return min;

            var seed = (int)System.DateTime.Now.Ticks & 0xFFFF;
            seed = (214013 * seed + 2531011) & 0x7fffffff;
            var rand = (seed >> 16) & 0x7FFF;
            var range = maxExclusive - min;
            var modded = rand % range;
            return min + modded;
        }

        private static string[] GenerateVariableNames()
        {
            var list = new CustomList<string>(26);
            for (char c = 'a'; c <= 'z'; c++)
            {
                list.Add(c.ToString());
            }
            return list.ToArray();
        }
    }
}
