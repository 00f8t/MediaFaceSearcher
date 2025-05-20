using FaceAiSharp;
using Microsoft.ML.OnnxRuntime;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SixLabors.ImageSharp;

namespace MediaFaceSearcher.NeuralNetwork
{
    public class FaceRecognizer
    {
        private IFaceEmbeddingsGenerator _rec;
        public FaceRecognizer()
        {
            var options = new SessionOptions()
            {
                LogSeverityLevel = OrtLoggingLevel.ORT_LOGGING_LEVEL_VERBOSE,
                LogVerbosityLevel = 1,
                EnableMemoryPattern = true
            };
            _rec = FaceAiSharpBundleFactory.CreateFaceEmbeddingsGenerator(options);
        }

        public float[] Detect(string imagePath, IReadOnlyList<PointF> landmarks)
        {
            using var image = Image.Load<Rgb24>(imagePath);
            _rec.AlignFaceUsingLandmarks(image, landmarks);
            var result = _rec.GenerateEmbedding(image);
            return result;
        }
    }
}
