using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MediaFaceSearcher.Model
{
    public static class Settings
    {
        public static int MinFaceWidth { get; set; } = 30; // 10% of the image size
        public static int MinFaceHeight { get; set; } = 30; // 10% of the image size
        public static bool SavePhotoCopy { get; set; } = true;
        public static string SavePhotoCopyPath { get; set; } = AppDomain.CurrentDomain.BaseDirectory + "/SavedPhotos";
        public static float DetectionConfidence { get; set; } = 0.5f; // Confidence threshold for face detection
        public static float RecognitionConfidence { get; set; } = 0.5f; // Confidence threshold for face recognition

    }
}
