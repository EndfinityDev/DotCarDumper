using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotCarReader.FileProcessing
{
    class CarStructureTable
    {
        public readonly CarStructureTable Parent = null;

        readonly List<object> _keys;
        readonly List<object> _values;

        public readonly UInt32 Size = 0; // Expected size of the table
        public UInt32 Count { get; private set; } = 0; // Actual amount of pairs in the table

        public CarStructureTable(UInt32 size, CarStructureTable parent = null)
        {
            Size = size;
            Parent = parent;
            _keys = new List<object>((int)size);
            _values = new List<object>((int)size);
        }

        public void Add(object key, object value)
        {
            _keys.Add(key);
            _values.Add(value);
            Count++;
        }

        public object[] Keys() { return _keys.ToArray(); }
        public object[] Values() { return _values.ToArray(); }

        public KeyValuePair<object, object> GetPair(UInt32 id)
        {
            return new KeyValuePair<object, object>(_keys[(Int32)id], _values[(Int32)id]);
        }

        public void SetValue(UInt32 id, object value)
        {
            _values[(Int32)id] = value;
        }
        public void SetKey(UInt32 id, object value)
        {
            _keys[(Int32)id] = value;
        }
    }
}
