
using System.Drawing;

namespace MediaFaceSearcher.Model
{
    public class Photo
    {
        public float[] Embedding { get; set; }
        public RectangleF FaceBox { get; set; }
        public string FilePath { get; set; }
        public Emotion Emotion { get; set; }
        public DateTime DateAdded { get; set; }
    }
}
