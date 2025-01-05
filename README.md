# Boolean Calculator

A Boolean logic interpreter written in C# that can define, solve, and find logical expressions, subject to certain constraints. Specifically, it does not use built-in string operations (like string.Split), no LINQ, and uses fully manual data structures (CustomList, CustomDictionary) instead of standard .NET collections.

## Features

### DEFINE

Allows you to create new boolean functions with a name, parameters, and a body (logical expression).

Example: `DEFINE andFunc(a,b): a & b`

Defines a function named `andFunc` with parameters `a` and `b`, performing `a & b`.

### SOLVE

Evaluates a previously defined function with specific argument values.

Example: `SOLVE andFunc(1, 0)`

Outputs `0` because `1 & 0 = 0`.

### FIND

Discovers a boolean expression that matches a (partial) truth table.

Two approaches:

- Brute force (enumeration) approach (if enabled/committed).
- Genetic Algorithm approach for larger tables.

Example: `FIND 0,0:0; 0,1:1; 1,0:1; 1,1:0`

Tries to find an expression that yields `0` for `(0,0)`, `1` for `(0,1)`, and so on—like XOR.

### ALL (Optional)

Some versions of the project include an ALL command listing all defined functions.

## Requirements

- .NET 6 (or .NET 7) SDK
- A console environment (Windows, Mac, Linux all supported if .NET is installed).

## Project Structure

```bash
Boolean-Calculator/
│
├── Commands/
│   ├── DefineCommand.cs
│   ├── SolveCommand.cs
│   ├── FindCommand.cs
│   └── AllCommand.cs  (optional)
│
├── HelpingCommands/
│   ├── StringCommands.cs
│   ├── Parser.cs
│   ├── Tokenizer.cs
│   └── Other utilities...
│
├── Structures/
│   ├── CustomStructures/
│   │   ├── CustomList.cs
│   │   ├── CustomDictionary.cs
│   │   └── ...
│   ├── Node/
│   │   └── ExpressionNode.cs
│   ├── FunctionTable.cs
│   └── ...
│
├── Interpreter/
│   └── Interpreter.cs       // The main run loop, reading commands from console
│
└── Program.cs               // Or a main entry point if desired
```

- **Commands**: Each class handles a specific console command (DEFINE, SOLVE, FIND, etc.).
- **HelpingCommands**: Custom string operations, tokenizing, parsing logic (no built-in .Split, .IndexOf, etc.).
- **Structures**:
  - CustomList, CustomDictionary: Manually implemented data structures.
  - ExpressionNode: AST representation of logical expressions.
  - FunctionTable: Stores user-defined functions.
- **Interpreter**: Contains the main loop that reads user input and dispatches commands.

## Usage

### 1. Build & Run

#### Option A: Visual Studio

- Open `Boolean-Calculator.sln`
- Press `F5` to run or select “Start Debugging.”

#### Option B: .NET CLI

```bash
cd Boolean-Calculator
dotnet build
dotnet run
```

### 2. Example Commands

```bash
> DEFINE andFunc(a,b): a & b
Defined function 'andFunc' with parameters [a, b].

> SOLVE andFunc(1, 1)
Result: 1

> SOLVE andFunc(1, 0)
Result: 0

> DEFINE orFunc(a,b): a | b
Defined function 'orFunc' with parameters [a, b].

> SOLVE orFunc(0, 1)
Result: 1

> FIND 0,0:0; 0,1:1; 1,0:1; 1,1:0
xorSolutionHere(a,b)
```

(Your actual output for FIND may differ, e.g., `((a & !b) | (!a & b))`, or something the genetic algorithm discovered.)

### 3. Key Constraints

- No built-in string splitting, LINQ, or advanced .NET collections.
- All splits, indexes, and data structures are custom-coded (StringCommands, CustomList, CustomDictionary).
- No partial usage of standard libraries for logic (like &, |, ! are done via integer ops, or custom checks).

## Testing

### Unit Tests

Not included; you can add them to verify DefineCommand and SolveCommand logic.

### Manual

- Try small examples like `andFunc`, `orFunc`, partial truth tables for FIND.
- Ensure error handling works if you type a malformed command.

## Known Issues & Future Improvements

- Genetic Algorithm for FIND might not always converge for large or contradictory tables—tweak PopulationSize, MaxGenerations, or add parallel evaluation.
- Expressions can appear “redundant” (`(a & a)` instead of just `a`) because we do not do simplification.
- No license declared. This is primarily an academic assignment.

## Contributing

1. Fork this repo.
2. Create a feature branch (`git checkout -b feature/myImprovement`).
3. Commit changes (`git commit -am 'Add cool new feature'`).
4. Push to the branch (`git push origin feature/myImprovement`).
5. Open a Pull Request.
