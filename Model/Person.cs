using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaFaceSearcher.Model
{
    public class Person
    {
        public string Name { get; set; }
        public Guid Id { get; } = Guid.NewGuid();
        public List<Photo> Photos { get; set; }
    }
}
