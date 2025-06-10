using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaFaceSearcher.Model;
using MediaFaceSearcher.Model.Events;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NumSharp;

namespace MediaFaceSearcher.DAL
{
    public class PersonDao : IPersonDao
    {
        private readonly string _path = "DataBase/SentiFace.db";
        private readonly IEventAggregator _eventAggregator;

        public PersonDao(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }
        public void Update(List<Person> personList, bool triggerEvent)
        {
            var serialized = JsonConvert.SerializeObject(personList);
            if(!Path.Exists(_path))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_path));
            }

            File.WriteAllText(_path, serialized);

            if(triggerEvent)
                _eventAggregator.GetEvent<PersonListChangedEvent>().Publish();
        }
        public List<Person> Read()
        {
            try
            {
                if (!File.Exists(_path)) return [];
                return JsonConvert.DeserializeObject<List<Person>>(File.ReadAllText(_path)) ?? new List<Person>();
            }
            catch (Exception ex)
            {
                // Log the exception
                return new List<Person>();
            }
        }
    }
}
