﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotCarReader.FileProcessing
{
    static class CarDataStringConverter
    {
        public static void ConvertStringTables(CarStructureTable table)
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
                    table.SetValue(i, fr.MainTable);
                    continue;
                }
                if (vals[i] is CarStructureTable subTable)
                {
                    ConvertStringTables(subTable);
                }
                if (keys[i] is byte[] buffer2)
                {
                    var stream = new MemoryStream(buffer2);
                    DotCarFileReader fr = new DotCarFileReader(stream);
                    table.SetKey(i, fr.MainTable);
                    continue;
                }
                if (keys[i] is CarStructureTable subTable2)
                {
                    ConvertStringTables(subTable2);
                }
            }
        }
    }
}
