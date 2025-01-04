namespace Logical_Expression_Interpreter.Structures.CustomStructures
{
    public class CustomDictionary
    {
        private const int DefaultCapacity = 16;
        private const double LoadFactorThreshold = 0.75;

        private string?[] _keys;
        private int[] _values;
        private bool[] _occupied;
        private int _count;

        public CustomDictionary(int capacity = DefaultCapacity)
        {
            _keys = new string[capacity];
            _values = new int[capacity];
            _occupied = new bool[capacity];
            _count = 0;
        }

        public int Count => _count;

        public void Add(string? key, int value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (_count >= _keys.Length * LoadFactorThreshold)
                Resize();

            var index = GetIndexForKey(key);
            while (_occupied[index])
            {
                if (_keys[index] == key)
                    throw new InvalidOperationException($"Key '{key}' already exists.");
                index = (index + 1) % _keys.Length;
            }

            _keys[index] = key;
            _values[index] = value;
            _occupied[index] = true;
            _count++;
        }

        public bool ContainsKey(string? key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            var index = GetIndexForKey(key);
            var startIndex = index;

            while (_occupied[index])
            {
                if (_keys[index] == key)
                    return true;
                index = (index + 1) % _keys.Length;
                if (index == startIndex) break;
            }

            return false;
        }

        public int Get(string? key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            var index = GetIndexForKey(key);
            var startIndex = index;

            while (_occupied[index])
            {
                if (_keys[index] == key)
                    return _values[index];
                index = (index + 1) % _keys.Length;
                if (index == startIndex) break;
            }

            throw new InvalidOperationException($"Key '{key}' not found.");
        }

        public void Remove(string? key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            var index = GetIndexForKey(key);
            var startIndex = index;

            while (_occupied[index])
            {
                if (_keys[index] == key)
                {
                    _occupied[index] = false;
                    _count--;
                    return;
                }
                index = (index + 1) % _keys.Length;
                if (index == startIndex) break;
            }

            throw new InvalidOperationException($"Key '{key}' not found.");
        }

        private int GetIndexForKey(string? key)
        {
            var hash = 0;
            foreach (var t in key)
            {
                hash = (31 * hash + t) % _keys.Length;
            }
            return hash >= 0 ? hash : hash + _keys.Length;
        }

        private void Resize()
        {
            var newCapacity = _keys.Length * 2;
            var oldKeys = _keys;
            var oldValues = _values;
            var oldOccupied = _occupied;

            _keys = new string[newCapacity];
            _values = new int[newCapacity];
            _occupied = new bool[newCapacity];
            _count = 0;

            for (var i = 0; i < oldKeys.Length; i++)
            {
                if (oldOccupied[i])
                {
                    Add(oldKeys[i], oldValues[i]);
                }
            }
        }
    }
}
