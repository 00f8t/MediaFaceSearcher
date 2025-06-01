using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FaceONNX;
using UMapx.Core;

namespace MediaFaceSearcher.NeuralNetwork
{
    class EmotionDetector
    {
        private readonly FaceEmotionClassifier _emotionClassifier = new();


        public string Detect(Bitmap aligned)
        {
            var emotion = _emotionClassifier.Forward(aligned);
            var max = Matrice.Max(emotion, out int argmax);
            var emotionLabel = FaceEmotionClassifier.Labels[argmax];
            aligned.Dispose();

            return emotionLabel;
        }
    }
}
