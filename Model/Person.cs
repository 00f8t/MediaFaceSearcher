using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;

namespace MediaFaceSearcher.Model
{
    public class Person
    {
        public string Name { get; set; }
        public Guid Id { get; } = Guid.NewGuid();
        public List<Photo> Photos { get; set; }
        [JsonIgnore]
        public BitmapSource MainPhoto { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
