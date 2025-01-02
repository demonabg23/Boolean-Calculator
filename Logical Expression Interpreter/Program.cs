using Logical_Expression_Interpreter.HelpingCommands;

namespace Logical_Expression_Interpreter
{
    internal class Program
    {
        public static void Main()
        {
            var helper = new StringCommands();
            var functionTable = new Structures.FunctionTable();
            var parser = new Parser(helper);

            var interpreter = new Interpreter(helper, functionTable, parser);
            interpreter.Run();
        }
    }
}