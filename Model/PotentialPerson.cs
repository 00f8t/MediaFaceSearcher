using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using FaceAiSharp;
using MediaFaceSearcher.NeuralNetwork;

namespace MediaFaceSearcher.Model
{
    public class PotentialPerson
    {
        public string Name { get; set; }
        public Person ClosestPerson { get; set; }
        public BitmapSource CroppedImage { get; set; }
        public float Confidence { get; set; }
        public RectangleF FaceBox { get; set; }
        public string FilePath { get; set; }
        public float[] Embedding { get; set; }
        public Emotion Emotion { get; set; }
    }
}
