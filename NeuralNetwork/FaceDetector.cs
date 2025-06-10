using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FaceAiSharp;
using MediaFaceSearcher.DAL;
using MediaFaceSearcher.Model;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.ML.OnnxRuntime;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace MediaFaceSearcher.NeuralNetwork
{
    class FaceDetector
    {
        private IFaceDetectorWithLandmarks _det;
        private readonly Settings _settings;
        public FaceDetector(Settings settings)
        {
            _settings = settings;

            var options = new SessionOptions()
            {
                LogSeverityLevel = OrtLoggingLevel.ORT_LOGGING_LEVEL_VERBOSE,
                LogVerbosityLevel = 1,
                EnableMemoryPattern = true,
            };
            _det = CreateFaceDetectorWithLandmarks(options);
        }

        public IFaceDetectorWithLandmarks CreateFaceDetectorWithLandmarks(SessionOptions? sessionOptions = null)
        {
            var c = CreateMemoryCache();
            var modelPath = Path.Combine(GetExeDir(), "onnx", "scrfd_2.5g_kps.onnx");
            var opt = new ScrfdDetectorOptions() { ModelPath = modelPath,  ConfidenceThreshold = _settings.DetectionConfidence};
            return new ScrfdDetector(c, opt, sessionOptions);
        }
        private string GetExeDir() => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;

        private IMemoryCache CreateMemoryCache()
        {
            var opts = new MemoryCacheOptions();
            var iopts = Options.Create(opts);
            return new MemoryCache(iopts);
        }

        public IEnumerable<FaceDetectorResult> Detect(string imagePath)
        {
            using var image = Image.Load<Rgb24>(imagePath);
            var result = _det.DetectFaces(image).Where(
                     x => x.Box.Width > _settings.MinFaceWidth && 
                     x.Box.Height > _settings.MinFaceHeight);

            return result;
        }
    }
}
