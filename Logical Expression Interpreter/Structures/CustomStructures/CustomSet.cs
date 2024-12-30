namespace Logical_Expression_Interpreter.Structures.CustomStructures
{
    public class CustomSet
    {
        private const int DefaultCapacity = 16;
        private const double LoadFactorThreshold = 0.75;

        private string[] _elements;
        private bool[] _occupied;
        private int _count;

        public CustomSet(int capacity = DefaultCapacity)
        {
            _elements = new string[capacity];
            _occupied = new bool[capacity];
            _count = 0;
        }

        public int Count => _count;

        public void Add(string element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            if (_count >= _elements.Length * LoadFactorThreshold)
                Resize();

            var index = GetIndexForElement(element);
            while (_occupied[index])
            {
                if (_elements[index] == element)
                    return;
                index = (index + 1) % _elements.Length;
            }

            _elements[index] = element;
            _occupied[index] = true;
            _count++;
        }

        public bool Contains(string element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            var index = GetIndexForElement(element);
            var startIndex = index;

            while (_occupied[index])
            {
                if (_elements[index] == element)
                    return true;
                index = (index + 1) % _elements.Length;
                if (index == startIndex) break;
            }

            return false;
        }

        public string[] ToArray()
        {
            var result = new string[_count];
            var index = 0;
            for (var i = 0; i < _elements.Length; i++)
            {
                if (_occupied[i])
                {
                    result[index++] = _elements[i];
                }
            }
            return result;
        }

        private int GetIndexForElement(string element)
        {
            var hash = 0;
            foreach (var t in element)
            {
                hash = (31 * hash + t) % _elements.Length;
            }
            return hash >= 0 ? hash : hash + _elements.Length;
        }

        private void Resize()
        {
            var newCapacity = _elements.Length * 2;
            var oldElements = _elements;
            var oldOccupied = _occupied;

            _elements = new string[newCapacity];
            _occupied = new bool[newCapacity];
            _count = 0;

            for (var i = 0; i < oldElements.Length; i++)
            {
                if (oldOccupied[i])
                {
                    Add(oldElements[i]);
                }
            }
        }
    }
}
