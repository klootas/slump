using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;


namespace SlumpaGrupper
{
    class TextReader
    {
        public static Person[] LoadFromFile()
        {
            string file = "Studenter.json";

            Person[] people = Array.Empty<Person>();
            if (File.Exists(file))
            {
                string json = File.ReadAllText(file);
                List<PersonData> loadedData = JsonSerializer.Deserialize<List<PersonData>>(json);
                people = loadedData.Select(p => new Person(p.Name)
                {
                    Group = p.Group
                }).ToArray();
            }


            return people;
        }

        public static void SaveToFile(List<Person> content)
        {
            List<PersonData> dataList = content.Select(p => new PersonData
            {
                Name = p.Name,
                Group = p.Group
            }).ToList();
            string json = JsonSerializer.Serialize(dataList);
            File.WriteAllText("Studenter.json", json);
        }
    }
}
