using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SixLabors.ImageSharp;

namespace MediaFaceSearcher.Model
{
    public class Photo
    {
        public float[] Embedding { get; set; }
        public RectangleF FaceBox { get; set; }
        public List<PointF> Keypoints { get; set; }
        public string FilePath { get; set; }
    }
}
