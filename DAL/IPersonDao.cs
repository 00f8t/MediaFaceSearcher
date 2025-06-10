using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaFaceSearcher.Model;

namespace MediaFaceSearcher.DAL
{
    public interface IPersonDao
    {
        void Update(List<Person> personList, bool triggerEvent = true);
        List<Person> Read();
    }
}
