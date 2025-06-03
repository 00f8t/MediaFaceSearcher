using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using MediaFaceSearcher.Model;

namespace MediaFaceSearcher.Converters
{
    class PhotoToFaceConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string path = values[0] as string;
            RectangleF faceBox = (RectangleF)values[1];

            var original = new Bitmap(path);
            return original.Clone(faceBox, System.Drawing.Imaging.PixelFormat.Format24bppRgb).ToBItmapSource();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
