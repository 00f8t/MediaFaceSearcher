using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;

namespace MediaFaceSearcher.Model
{
    public class Person : BindableBase
    {
        public string Name { get; set; }
        public Guid Id { get; } = Guid.NewGuid();
        public List<Photo> Photos { get; set; } = new();
        public MainPhoto MainPhoto { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    public class MainPhoto (string path, RectangleF rectangle)
    {
        public string Path { get; set; } = path;
        public RectangleF Rectangle { get; set; } = rectangle;
    }
}
