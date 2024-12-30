using Logical_Expression_Interpreter.HelpingCommands;

namespace Logical_Expression_Interpreter.Structures;

public class Tokenizer
{
    private readonly string _expression;
    private int _position;
    private readonly StringCommands _helpers;

    public Tokenizer(string expression, StringCommands helpers)
    {
        if (expression == null || expression.Length == 0)
            _helpers?.ThrowError("Expression cannot be null or empty.");

        _expression = expression;
        _position = 0;
        _helpers = helpers ?? throw new ArgumentNullException(nameof(helpers));
    }

    public string? GetNextToken()
    {
        _helpers.SkipWhitespace(_expression, ref _position);

        if (_position >= _expression.Length)
            return null;

        var currentChar = _expression[_position];

        if (currentChar is '&' or '|' or '!' or '(' or ')' or ',')
        {
            _position++;
            return _helpers.CharToString(currentChar);
        }

        if (!_helpers.IsLetter(currentChar))
            throw new InvalidOperationException(_helpers.GetErrorMessage(_expression, _position, "Invalid character"));
        var start = _position;
        while (_position < _expression.Length &&
               (_helpers.IsLetter(_expression[_position]) || _helpers.IsDigit(_expression[_position])))
        {
            _position++;
        }

        return _helpers.Substring(_expression, start, _position - start);

    }

    public string? PeekNextToken()
    {
        var currentPosition = _position;
        var nextToken = GetNextToken();
        _position = currentPosition;
        return nextToken;
    }
}