using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotCarReader.FileProcessing;

namespace DotCarReader
{
    class Program
    {
        static void Main(string[] args)
        {
            string filePath;
            if (args.Length > 0)
                filePath = args[0];
            else filePath = "Model_22_-_Trim_22.car";

            DotCarFileReader fr = new DotCarFileReader(filePath);

            File.WriteAllText("Output.txt", fr.GetTableString());
            
        }
    }
}
