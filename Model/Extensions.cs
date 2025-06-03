using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows;
using FaceAiSharp;
using RectangleF = System.Drawing.RectangleF;
using Size = System.Drawing.Size;

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

        public static  BitmapSource ToBItmapSource(this Bitmap bitmap)
        {
            //bitmap.Save(@"C:\Users\rosse\Downloads\foo.jpg");
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                bitmap.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
        }

        public static RectangleF GetClampedFaceRectangle(this FaceDetectorResult face, Size originalSize)
        {
            var faceX = face.Box.X;
            var faceY = face.Box.Y;
            var faceW = face.Box.Width;
            var faceH = face.Box.Height;

            // Clamp X and adjust width
            if (faceX < 0)
            {
                faceW += faceX; // reduce width by the amount x is negative
                faceX = 0;
            }

            if (faceX + faceW > originalSize.Width)
            {
                faceW = originalSize.Width - faceX;
            }

            // Clamp Y and adjust height
            if (faceY < 0)
            {
                faceH += faceY; // reduce height by the amount y is negative
                faceY = 0;
            }

            if (faceY + faceH > originalSize.Height)
            {
                faceH = originalSize.Height - faceY;
            }

            // Ensure width and height are not negative
            faceW = Math.Max(0, faceW);
            faceH = Math.Max(0, faceH);

            return new RectangleF(faceX, faceY, faceW, faceH);
        }

        public static System.Drawing.RectangleF ToRectangleF(this SixLabors.ImageSharp.RectangleF rect)
        {
            return new System.Drawing.RectangleF(rect.X, rect.Y, rect.Width, rect.Height);
        }
    }
}