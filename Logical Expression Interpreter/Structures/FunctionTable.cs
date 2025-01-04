using Logical_Expression_Interpreter.Structures.CustomStructures;
using Logical_Expression_Interpreter.Structures.Node;

namespace Logical_Expression_Interpreter.Structures;

public class FunctionTable
{
    private string?[] _keys;
    private ExpressionNode[] _values;
    private int _count;

    public FunctionTable(int initialSize = 10)
    {
        _keys = new string[initialSize];
        _values = new ExpressionNode[initialSize];
        _count = 0;
    }

    public void Add(string? key, ExpressionNode value)
    {
        for (var i = 0; i < _count; i++)
        {
            if (_keys[i] != key) continue;
            _values[i] = value;
            return;
        }

        if (_count >= _keys.Length)
        {
            ResizeArrays();
        }

        _keys[_count] = key;
        _values[_count] = value;
        _count++;
    }

    private void ResizeArrays()
    {
        var newSize = _keys.Length * 2;
        string?[] newKeys = new string[newSize];
        var newValues = new ExpressionNode[newSize];
        for (var i = 0; i < _keys.Length; i++)
        {
            newKeys[i] = _keys[i];
            newValues[i] = _values[i];
        }
        _keys = newKeys;
        _values = newValues;
    }

    public ExpressionNode Get(string? key)
    {
        for (var i = 0; i < _count; i++)
        {
            if (_keys[i] == key)
            {
                return _values[i];
            }
        }
        throw new Exception($"Function {key} not found.");
    }
    public CustomList<string?> GetAllFunctionNames()
    {
        var names = new CustomList<string?>();
        for (var i = 0; i < _count; i++)
        {
            names.Add(_keys[i]);
        }
        return names;
    }

    public bool Contains(string? key)
    {
        for (var i = 0; i < _count; i++)
        {
            if (_keys[i] == key)
            {
                return true;
            }
        }
        return false;
    }
}