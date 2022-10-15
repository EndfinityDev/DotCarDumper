using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotCarDumper.FileProcessing
{
    class DotCarFileReader
    {
        public CarStructureTable MainTable { get; private set; } // Toplevel table
        readonly BinaryReader _binaryReader;

        CarStructureTable _currentTable = null;
        CarStructureTable _nextTable = null; // Used for subtables
        

        public DotCarFileReader(string filePath)
        {
            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            _binaryReader = new BinaryReader(fileStream, Encoding.UTF8);

            ReadStream();

            CarDataStringConverter.ConvertStringTables(MainTable);

            _binaryReader.Close();
        }
        public DotCarFileReader(MemoryStream stream)
        {
            _binaryReader = new BinaryReader(stream);

            ReadStream();

            _binaryReader.Close();
        }


        
        public string BuildString()
        {
            return GetTableString(MainTable);
        }

        string GetTableString(CarStructureTable table, byte indentationLevel = 0)
        {
            StringBuilder sb = new StringBuilder();
            KeyValuePair<object, object> kv;

            for(UInt32 i = 0; i < table.Count; i++)
            {
                for(byte o = 0; o < indentationLevel; o++)
                {
                    sb.Append("\t");
                }
                kv = table.GetPair(i);

                if (kv.Key is CarStructureTable keyTable)
                {
                    sb.AppendLine();
                    byte newIndentationLevel = (byte)(indentationLevel + 0b1);
                    sb.Append(GetTableString(keyTable, newIndentationLevel));
                }
                else
                    sb.Append(kv.Key.ToString());

                sb.Append(": ");
                if (kv.Value is CarStructureTable valueTable)
                {
                    sb.AppendLine();
                    byte newIndentationLevel = (byte)(indentationLevel + 0b1);
                    sb.Append(GetTableString(valueTable, newIndentationLevel));
                }
                else
                    sb.Append(kv.Value.ToString());

                sb.AppendLine();
            }
                return sb.ToString();
        }

        void ReadStream()
        {
            if (_binaryReader.ReadByte() != 0x01)
            {
                throw new Exception("No entry point was found");
            }
            if(_binaryReader.ReadByte() != 0x54) // T
            {
                throw new Exception("No toplevel table definition was found");
            }

            MainTable = ReadTable();
            _currentTable = MainTable;

            object key, value;
            while (true)
            {
                try
                {
                    key = ProcessNextValue();
                    value = ProcessNextValue();
                    _currentTable.Add(key, value);

                    if(_nextTable != null) // Check if there is a subtable to process
                    {
                        _currentTable = _nextTable;
                        _nextTable = null;
                    }

                    if (_currentTable.Count >= _currentTable.Size)
                        if (_currentTable.Parent != null)
                            _currentTable = _currentTable.Parent;   // exit the current table to the one above
                        else break; // exit the loop if all tables have been processed
                }
                catch (EndOfStreamException)
                {
                    break;
                }
            }
        }

        object ProcessNextValue()
        {
            byte b = _binaryReader.ReadByte();
            if (b == 0x30) // 0
                return false;
            if (b == 0x31) // 1
                return true;
            if (b == 0x53) // S
                return ReadString();
            if (b == 0x54) // T
            {
                _nextTable = ReadTable(); // Mark subtable for processing
                return _nextTable;
            }
            if (b == 0x4E) // N
                return ReadDouble();
            
            throw new Exception("Unknown data type: " + b);
                //return ProcessNextValue();
        }

        object ReadString()
        {
            UInt32 stringSize = _binaryReader.ReadUInt32();
            byte[] byteString = _binaryReader.ReadBytes((Int32)stringSize);
            if (byteString.Length == 0)
                return string.Empty;
            if (byteString[0] == 0x01) // Marks a subtable
                return byteString; // Return as a byte array for later processing
            return Encoding.UTF8.GetString(byteString);
        }

        Double ReadDouble()
        {
            return _binaryReader.ReadDouble();
        }

        CarStructureTable ReadTable()
        {
            UInt32 tableSize;

            tableSize = _binaryReader.ReadUInt32();

            if (tableSize == 0)
                tableSize = _binaryReader.ReadUInt32();
            else
                _ = _binaryReader.ReadUInt32(); // Jump over the second integer if the value was found in the first one
                                                // as only one of the two contains a value
            CarStructureTable table = new CarStructureTable(tableSize, _currentTable);

            return table;
        }
    }
}
