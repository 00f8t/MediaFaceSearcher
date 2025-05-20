using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FaceAiSharp;
using Microsoft.ML.OnnxRuntime;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace MediaFaceSearcher.NeuralNetwork
{
    class FaceDetector
    {
        private IFaceDetectorWithLandmarks _det;
        public FaceDetector()
        {
            var options = new SessionOptions()
            {
                LogSeverityLevel = OrtLoggingLevel.ORT_LOGGING_LEVEL_VERBOSE,
                LogVerbosityLevel = 1,
                EnableMemoryPattern = true
            };
            _det = FaceAiSharpBundleFactory.CreateFaceDetectorWithLandmarks(options);
        }

        public IReadOnlyCollection<FaceDetectorResult> Detect(string imagePath)
        {
            using var image = Image.Load<Rgb24>(imagePath);
            var result = _det.DetectFaces(image);
            return result;
        }
    }
}
