using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotCarReader.FileProcessing
{
    class DotCarFileReader
    {
        CarStructureTable _table;
        readonly BinaryReader _binaryReader;
        CarStructureTable _currentTable = null;

        CarStructureTable _nextTable = null;
        
        //FileStream _fileStream;
        //UIntPtr _cursor;

        public DotCarFileReader(string filePath)
        {
            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            _binaryReader = new BinaryReader(fileStream, Encoding.UTF8);

            ReadFile();

            ConvertStringTables(_table);

        }
        public DotCarFileReader(MemoryStream stream)
        {
            _binaryReader = new BinaryReader(stream);

            //File.WriteAllBytes("bytes", stream.GetBuffer());

            ReadFile();

            //ConvertStringTables(_table);
        }

        public CarStructureTable GetTable() { return _table; }

        void ConvertStringTables(CarStructureTable table)
        {
            object[] vals = table.Values();
            object[] keys = table.Keys();
            for (UInt32 i = 0; i < table.Count; i++)
            {
                if (vals[i] is byte[] buffer)
                {
                    //File.WriteAllBytes("bytes", buffer);
                    var stream = new MemoryStream(buffer);
                    DotCarFileReader fr = new DotCarFileReader(stream);
                    table.SetValue(i, fr.GetTable());
                    continue;
                }
                if(vals[i] is CarStructureTable subTable)
                {
                    ConvertStringTables(subTable);
                }
                if (keys[i] is byte[] buffer2)
                {
                    var stream = new MemoryStream(buffer2);
                    DotCarFileReader fr = new DotCarFileReader(stream);
                    table.SetKey(i, fr.GetTable());
                    continue;
                }
                if (keys[i] is CarStructureTable subTable2)
                {
                    ConvertStringTables(subTable2);
                }
            }
        }
        public string GetTableString()
        {
            return GetTableString(_table);
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

                if (kv.Key is CarStructureTable keyTable2)
                {
                    sb.AppendLine();
                    byte ni = (byte)(indentationLevel + 0b1);
                    sb.Append(GetTableString(keyTable2, ni));
                }
                else
                    sb.Append(kv.Key.ToString());

                sb.Append(": ");
                if (kv.Value is CarStructureTable keyTable)
                {
                    sb.AppendLine();
                    byte ni = (byte)(indentationLevel + 0b1);
                    sb.Append(GetTableString(keyTable, ni));
                }
                else
                    sb.Append(kv.Value.ToString());

                sb.AppendLine();
            }
                return sb.ToString();
        }

        void ReadFile()
        {
            if (_binaryReader.ReadByte() != 0x01)
            {
                throw new Exception("No .car entry point was found");
            }
            if(_binaryReader.ReadByte() != 0x54)
            {
                throw new Exception("No toplevel table definition was found");
            }

            _table = ReadTable();
            _currentTable = _table;

            object key, value;
            while (true)
            {
                try
                {
                    key = ProcessNextValue();
                    value = ProcessNextValue();
                    _currentTable.Add(key, value);

                    if(_nextTable != null)
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

            if (b == 0x53)
                return ReadString();
            if (b == 0x54)
            {
                _nextTable = ReadTable();
                return _nextTable;
            }
            if (b == 0x4E)
                return ReadDouble();
            else
                //throw new Exception("Unknown data type: " + b);
                return ProcessNextValue(); // Annoyingly, some strings contain more characters than they state
        }

        object ReadString()
        {
            UInt32 stringSize = _binaryReader.ReadUInt32();
            byte[] byteString = _binaryReader.ReadBytes((Int32)stringSize);
            if (byteString.Length == 0)
                return string.Empty;
            if (byteString[0] == 0x01)
                return byteString;
            while (true)
            {
                byte b = _binaryReader.ReadByte(); // Another workaround
                if (!(b == 0x4E || b == 0x54 || b == 0x53))
                {
                    byteString.Append(b);
                }
                else
                {
                    _binaryReader.BaseStream.Position--;
                    break;
                }
            }
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
                _ = _binaryReader.ReadUInt32();

            CarStructureTable table = new CarStructureTable(tableSize, _currentTable);

            return table;
        }
    }
}
