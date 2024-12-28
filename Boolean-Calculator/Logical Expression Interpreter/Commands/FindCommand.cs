using Logical_Expression_Interpreter.HelpingCommands;
using Logical_Expression_Interpreter.Structures;

namespace Logical_Expression_Interpreter.Commands;

public class FindCommand
{
    private readonly StringCommands _helper;
    private readonly FunctionTable _functionTable;

    public FindCommand(StringCommands helper, FunctionTable functionTable)
    {
        _helper = helper ?? throw new ArgumentNullException(nameof(helper));
        _functionTable = functionTable ?? throw new ArgumentNullException(nameof(functionTable));
    }

    public void Execute(string input)
    {
        if (!_helper.StartsWithIgnoreCase(input, "FIND"))
            throw new InvalidOperationException("Invalid command. Must start with 'FIND'.");

        string[] truthTable;
        if() 
        {
            
        }
    }
}