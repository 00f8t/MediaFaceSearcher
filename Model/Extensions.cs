using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaFaceSearcher.Model
{
    public static class Extensions
    {
        public static Bitmap ToBitmap<TPixel>(this Image<TPixel> image) where TPixel : unmanaged, IPixel<TPixel>
        {
            using var memoryStream = new MemoryStream();
            image.SaveAsBmp(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);
            return new Bitmap(memoryStream);
        }

        public static Emotion ToEmotion(this string input)
        {
            return input switch
            {
                "Happiness" => Emotion.Щастя,
                "Surprise" => Emotion.Подив,
                "Sadness" => Emotion.Сум,
                "Anger" => Emotion.Гнів,
                "Disgust" => Emotion.Огида,
                "Fear" => Emotion.Страх,
                _ => Emotion.Нейтрально
            };
        }
    }
}