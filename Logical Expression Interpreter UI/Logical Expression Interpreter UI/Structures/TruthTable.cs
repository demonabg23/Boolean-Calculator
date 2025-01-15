namespace Logical_Expression_Interpreter_UI.Structures;

public class TruthTable
{
    public int NumInputs { get; set; }

    public int[][] Rows { get; set; }

    public TruthTable(int numInputs, int[][] rows)
    {
        NumInputs = numInputs;
        Rows = rows;
    }
}