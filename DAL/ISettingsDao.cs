using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaFaceSearcher.Model;

namespace MediaFaceSearcher.DAL
{
    public interface ISettingsDao
    {
        public Settings Read();
        public void Save(Settings settings);
    }
}
