using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Collections;

namespace SlumpaGrupper
{
    class TextReader
    {
        static string[] lines;

        public static string[] LoadFromFile()
        {
            string file = "Studenter.txt";

            if (File.Exists(file))
            {
                lines = File.ReadAllLines(file, Encoding.Default);
            }
            else
            {
                File.Create("Studenter.txt");
                lines = new string[1];
                lines[0] = "";
            }

            return lines;
        }

        public static void SaveToFile(string content)
        {
            File.WriteAllText("Studenter.txt", content, Encoding.Default);
        }

        public static void SaveGroup(IEnumerable groupToSave)
        {
            string json = JsonConvert.SerializeObject(groupToSave);
            File.WriteAllText("lastGroup.txt", json, Encoding.Default);
        }

        public static IEnumerable LoadLastGroup()
        {
            string file = "lastGroup.txt";
            IEnumerable group = null;

            if (File.Exists(file))
            {
                group = JsonConvert.DeserializeObject<IEnumerable>(File.ReadAllText(file));
            }
            else
            {
                group = null;
            }
            return group;
        }
    }
}
