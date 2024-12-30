using Logical_Expression_Interpreter.HelpingCommands;
using Logical_Expression_Interpreter.Structures;
using Logical_Expression_Interpreter.Structures.Node;

namespace Logical_Expression_Interpreter.Commands;

public class DefineCommand
{
    private readonly StringCommands _helpers;
    private readonly FunctionTable _functionTable;

    public string FunctionName { get; private set; }
    public string[] Parameters { get; private set; }
    public string ExpressionBody { get; private set; }

    public DefineCommand(StringCommands helpers, FunctionTable functionTable)
    {
        _helpers = helpers ?? throw new ArgumentNullException(nameof(helpers));
        _functionTable = functionTable ?? throw new ArgumentNullException(nameof(functionTable));
    }

    public void Parse(string input)
    {
        if (string.IsNullOrEmpty(input))
            throw new ArgumentNullException(nameof(input), "Input cannot be null or empty.");

        var defineIndex = _helpers.FindCharacter(input, ' ');
        if (defineIndex == -1)
            _helpers.ThrowError("Invalid syntax. Missing space after DEFINE.");

        var commandPart = _helpers.Trim(_helpers.Substring(input, 0, defineIndex));
        var normalizedCommand = _helpers.ToLower(commandPart);
        var expectedCommand = _helpers.ToLower("DEFINE");

        if (normalizedCommand != expectedCommand)
            _helpers.ThrowError("Input must start with DEFINE.");

        var colonIndex = _helpers.FindCharacter(input, ':');
        if (colonIndex == -1)
            _helpers.ThrowError("Missing ':' in DEFINE command.");

        var signature = _helpers.Trim(
            _helpers.Substring(input, defineIndex + 1, colonIndex - (defineIndex + 1))
        );

        ExpressionBody = _helpers.Trim(
            _helpers.Substring(input, colonIndex + 1, input.Length - (colonIndex + 1))
        );

        ParseSignature(signature);
        ValidateExpression();
        BuildAndStoreAST();
    }

    private void ParseSignature(string signature)
    {
        var openParen = _helpers.FindCharacter(signature, '(');
        var closeParen = _helpers.FindCharacter(signature, ')');
        if (openParen == -1 || closeParen == -1 || closeParen < openParen)
            _helpers.ThrowError("Invalid function signature.");

        FunctionName = _helpers.Trim(_helpers.Substring(signature, 0, openParen));
        var paramsList = _helpers.Trim(_helpers.Substring(signature, openParen + 1, closeParen - openParen - 1));

        Parameters = _helpers.SplitByComma(paramsList);

        _helpers.ValidateName(FunctionName);
        foreach (var param in Parameters)
            _helpers.ValidateName(param);
    }

    private void ValidateExpression()
    {
        var tokenizer = new Tokenizer(ExpressionBody, _helpers);

        string? token;
        while ((token = tokenizer.GetNextToken()) != null)
        {
            if (_helpers.IsLetter(token[0]) && _functionTable.Contains(token))
            {
                ValidateFunctionCall(token, tokenizer);
            }
            else if (_helpers.IsLetter(token[0]) && !_helpers.IsParameterOrFunction(token, Parameters, _functionTable))
            {
                _helpers.ThrowError($"Undefined variable or function: {token}");
            }
        }
    }

    private void ValidateFunctionCall(string functionName, Tokenizer tokenizer)
    {
        var openParen = tokenizer.GetNextToken();
        if (openParen != "(")
            _helpers.ThrowError($"Expected '(' after function name '{functionName}'.");

        var functionNode = _functionTable.Get(functionName);

        var argumentCount = 0;
        string? token;
        while ((token = tokenizer.PeekNextToken()) != null && token != ")")
        {
            token = tokenizer.GetNextToken();
            if (token == ",") continue; 

            argumentCount++;
        }

        var closeParen = tokenizer.GetNextToken();
        if (closeParen != ")")
            _helpers.ThrowError($"Expected ')' to close function call for '{functionName}'.");

        var expectedCount = functionNode.Parameters?.Length ?? 0;
        if (argumentCount != expectedCount)
        {
            _helpers.ThrowError($"Function '{functionName}' requires {expectedCount} arguments, but {argumentCount} were provided.");
        }
    }


    private void BuildAndStoreAST()
    {
        var parser = new Parser(_helpers);
        var builder = new ExpressionNodeBuilder(ExpressionBody, parser);
        var node = builder.Build();
        node.Parameters = Parameters;

        _functionTable.Add(FunctionName, node);
    }

}
