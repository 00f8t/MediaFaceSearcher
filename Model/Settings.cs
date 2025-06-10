using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MediaFaceSearcher.Model
{
    public class Settings
    {
        public int MinFaceWidth { get; set; } = 30; // 10% of the image size // +
        public int MinFaceHeight { get; set; } = 30; // 10% of the image size // +
        public bool SavePhotoCopy { get; set; } = true;
        public string SavePhotoCopyPath { get; set; } = AppDomain.CurrentDomain.BaseDirectory + "SavedPhotos";
        public float DetectionConfidence { get; set; } = 0.5f; // Confidence threshold for face detection // +
        public float RecognitionConfidence { get; set; } = 0.5f; // Confidence threshold for face recognition // +

    }
}
