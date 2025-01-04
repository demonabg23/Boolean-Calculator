using System.Collections;

namespace Logical_Expression_Interpreter.Structures.CustomStructures
{
    public class CustomList<T> : IEnumerable<T>
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

        public void Set(int index, T value)
        {
            if (index < 0 || index >= _count)
                throw new IndexOutOfRangeException("Index out of range.");
            _items[index] = value;
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

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= _count)
                throw new IndexOutOfRangeException("Index out of range.");

            for (var i = index; i < _count - 1; i++)
            {
                _items[i] = _items[i + 1];
            }

            _count--;
        }

        public bool Contains(T item)
        {
            for (var i = 0; i < _count; i++)
            {
                if (_items[i]?.Equals(item) == true)
                    return true;
            }

            return false;
        }

        public int IndexOf(T item)
        {
            for (var i = 0; i < _count; i++)
            {
                if (_items[i]?.Equals(item) == true)
                    return i;
            }

            return -1;
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (var i = 0; i < _count; i++)
            {
                yield return _items[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
