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
        event EventHandler PersonChanged;
        void Update(List<Person> personList);
        List<Person> Read();
    }
}
