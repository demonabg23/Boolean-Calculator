namespace Logical_Expression_Interpreter.Structures.CustomStructures;

public class CustomList<T>
{
    private T[] _items;
    private int _count;
    private int _capacity;

    public CustomList(int capacity = 4)
    {
        _items = new T[capacity];
        _capacity = capacity;
        _count = 0;
    }

    public void Add(T item)
    {
        if (_count >= _capacity)
          Grow();
        _items[_count++] = item;
    }
    private void Grow()
    {
        _capacity *= 2;
        var temp = new T[_capacity];
        for (var i = 0; i < _count; i++)
            temp[i] = _items[i];
        _items = temp;
    }

    public T Get(int index)
    {
        if (index < 0 || index >= _count)
            throw new IndexOutOfRangeException("Index out of range.");
        return _items[index];
    }

    public int Count => _count;

    public void Clear()
    {
        _count = 0;
    }

    public T[] ToArray()
    {
        var temp = new T[_count];
        for (var i = 0; i < _count; i++)
            temp[i] = _items[i];
        return temp;
    }
}