using Logical_Expression_Interpreter.Commands;
using Logical_Expression_Interpreter.HelpingCommands;
using Logical_Expression_Interpreter.Structures;

namespace Logical_Expression_Interpreter
{
    public class Interpreter
    {
        private readonly StringCommands _helper;
        private readonly FunctionTable _functionTable;

        public Interpreter(StringCommands helper, FunctionTable functionTable)
        {
            _helper = helper ?? throw new ArgumentNullException(nameof(helper));
            _functionTable = functionTable ?? throw new ArgumentNullException(nameof(functionTable));
        }

        public void Run()
        {
            Console.WriteLine("Welcome to the Logical Expression Interpreter!\n");
            Console.WriteLine("You can type commands like:");
            Console.WriteLine("  DEFINE func1(a, b): a & b");
            Console.WriteLine("  SOLVE func1(1, 0)");
            Console.WriteLine("  ALL");
            Console.WriteLine("  FIND func1");
            Console.WriteLine("\nType 'exit' or 'quit' to stop.\n");

            while (true)
            {
                Console.Write("> ");
                var input = Console.ReadLine();

                if (_helper.IsNullOrWhiteSpace(input))
                {
                    continue;
                }

                try
                {
                    var commandType = CommandListHelper.DetectCommandType(input!, _helper);

                    switch (commandType)
                    {
                        case CommandList.Define:
                            {
                                var defineCommand = new DefineCommand(_helper, _functionTable);
                                defineCommand.Parse(input!);

                                Console.WriteLine(
                                    $"Defined function '{defineCommand.FunctionName}' " +
                                    $"with parameters [{string.Join(", ", defineCommand.Parameters)}]."
                                );
                                break;
                            }
                        case CommandList.Solve:
                        {
                            var command = new SolveCommand(_helper, _functionTable);
                            var result = command.Execute(input!);
                            Console.WriteLine($"Result: {result}");
                            break;
                        }
                        case CommandList.All:
                            {
                               var command = new AllCommand(_helper, _functionTable);
                               command.Execute(input!);
                                break;
                            }
                        case CommandList.Find:
                            {
                                var command = new FindCommand(_helper, _functionTable);
                                command.Execute(input!);
                            }
                        case CommandList.Exit:
                        case CommandList.Quit:
                            {
                                Console.WriteLine("Exiting interpreter...");
                                return; 
                            }
                        case CommandList.Unknown:
                        default:
                            {
                                Console.WriteLine("Unrecognized command. Try DEFINE, SOLVE, ALL, FIND, or exit.");
                                break;
                            }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }
    }
}
