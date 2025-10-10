using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Collections;
using System.Diagnostics.Contracts;


namespace SlumpaGrupper
{
    class TextReader
    {


        public static Person[] LoadFromFile()
        {
            string file = "Studenter.json";
            List<Person> tmp = new List<Person>();

            if (File.Exists(file))
            {
                string json = File.ReadAllText(file);
                 tmp = JsonConvert.DeserializeObject<List<Person>>(json);
            }


            return tmp.ToArray();
        }

        public static void SaveToFile(List<Person> content)
        {
            
            string json = JsonConvert.SerializeObject(content);
            File.WriteAllText("Studenter.json", json);
            //UI_Controller.SaveFile();
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
