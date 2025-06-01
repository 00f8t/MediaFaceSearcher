using FaceAiSharp;
using Microsoft.ML.OnnxRuntime;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaFaceSearcher.Model;
using SixLabors.ImageSharp;
using Image = SixLabors.ImageSharp.Image;
using PointF = SixLabors.ImageSharp.PointF;

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

        public float[] Detect(Image<Rgb24> image, IReadOnlyList<PointF> landmarks)
        {
            _rec.AlignFaceUsingLandmarks(image, landmarks);
            var result = _rec.GenerateEmbedding(image);
            return result;
        }

        public Bitmap AlignFaceToBitmap(Image<Rgb24> image, IReadOnlyList<PointF> landmarks)
        {
            _rec.AlignFaceUsingLandmarks(image, landmarks);
            return image.ToBitmap();
        }
    }
}
