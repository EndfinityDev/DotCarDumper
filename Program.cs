using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DotCarDumper.FileProcessing;

namespace DotCarDumper
{
    static class Program
    {
        [STAThreadAttribute]
        static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US"); // Forces floating point numbers to use periods

            string filePath = string.Empty;
            string fileName = string.Empty;
            if (args.Length > 0)
                filePath = args[0];
            else
            {
                OpenFileDialog dialog = new OpenFileDialog
                {
                    Filter = "Car export files (*.car)|*.car|All files (*.*)|*.*",
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) +
                    "\\AutomationGame\\Saved\\UserData\\CarSaveExport"
                };

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    filePath = dialog.FileName;
                    if (!File.Exists(filePath))
                    {
                        MessageBox.Show("Failed to find the file", "File not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Environment.Exit(0);
                    }
                }
            }

        #if !DEBUG
            try
        #endif
            {
                DotCarFileReader fr = new DotCarFileReader(filePath);

                fileName = filePath.Split('/', '\\').Last();
                fileName = fileName.Replace(".car", "");
                fileName += ".txt";
                //File.WriteAllText("Output.txt", fr.GetTableString());
                File.WriteAllText(fileName, fr.BuildString());
            }
        #if !DEBUG
            catch (Exception x)
            {
                MessageBox.Show(x.Message, "Could not deserialize file", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        #endif
        }
    }
}
