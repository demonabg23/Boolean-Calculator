using Logical_Expression_Interpreter.HelpingCommands;

namespace Logical_Expression_Interpreter.Structures
{
    public enum CommandList
    {
        Define,
        Solve,
        All,
        Find,
        Exit,
        Quit,
        Unknown
    }

    public static class CommandListHelper
    {
        public static CommandList DetectCommandType(string? input, StringCommands helper)
        {
            if (helper.StartsWithIgnoreCase(input, "define")) return CommandList.Define;
            if (helper.StartsWithIgnoreCase(input, "solve")) return CommandList.Solve;
            if (helper.StartsWithIgnoreCase(input, "all")) return CommandList.All;
            if (helper.StartsWithIgnoreCase(input, "find")) return CommandList.Find;
            if (helper.StartsWithIgnoreCase(input, "exit")) return CommandList.Exit;
            return helper.StartsWithIgnoreCase(input, "quit") ? CommandList.Quit : CommandList.Unknown;
        }
    }
}