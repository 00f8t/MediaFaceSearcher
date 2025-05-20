using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaFaceSearcher.Model;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MediaFaceSearcher.DAL
{
    public class PersonDao : IPersonDao
    {
        private readonly string path = "person.json";
        public void Update(List<Person> personList)
        {
            var serialized = JsonConvert.SerializeObject(personList);
            File.WriteAllText(path, serialized);
        }
        public List<Person> Read()
        {
            try
            {
                if (!File.Exists(path)) return [];

                return JsonConvert.DeserializeObject<List<Person>>(File.ReadAllText(path)) ?? new List<Person>();
            }
            catch (Exception ex)
            {
                // Log the exception
                return new List<Person>();
            }
        }
    }
}
