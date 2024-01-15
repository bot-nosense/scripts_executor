using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ExcuteScripts.Config
{
    public static class ConstantsReader
    {
        public static Dictionary<string, string> ReadConstantsFromFile(string filePath)
        {
            Dictionary<string, string> constants = new Dictionary<string, string>();

            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);

                foreach (string line in lines)
                {
                    string[] parts = line.Split('=');
                    if (parts.Length == 2)
                    {
                        string key = parts[0];
                        string value = parts[1];
                        constants[key] = value;
                    }
                }
            }
            else
            {
                Console.WriteLine("File not found.");
            }

            return constants;
        }
    }
}
