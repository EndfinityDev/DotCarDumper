using System;
using System.Collections.Generic;

namespace DotCarDumper.FileProcessing
{
    class CarStructureTable
    {
        #region Public Fields

        public readonly CarStructureTable Parent = null;
        public readonly Int32 Size = 0; // Expected size of the table
        public Int32 Count { get; private set; } = 0; // Actual amount of pairs in the table
        #endregion

        #region Member Fields

        readonly List<object> _keys;
        readonly List<object> _values;
        #endregion

        #region Initialization
        public CarStructureTable(Int32 size, CarStructureTable parent = null)
        {
            Size = size;
            Parent = parent;
            _keys = new List<object>(size);
            _values = new List<object>(size);
        }
        #endregion

        #region Public Interface
        public void Add(object key, object value)
        {
            _keys.Add(key);
            _values.Add(value);
            Count++;
        }

        public object[] Keys() { return _keys.ToArray(); }
        public object[] Values() { return _values.ToArray(); }

        public KeyValuePair<object, object> GetPair(Int32 id)
        {
            return new KeyValuePair<object, object>(_keys[id], _values[id]);
        }

        public void SetValue(Int32 id, object value)
        {
            _values[id] = value;
        }
        public void SetKey(Int32 id, object value)
        {
            _keys[id] = value;
        }
        #endregion
    }
}
