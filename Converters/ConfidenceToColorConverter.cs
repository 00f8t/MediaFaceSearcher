using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace MediaFaceSearcher.Converters
{
    class ConfidenceToColorConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // Expect value to be double between 0.5 and 1.0
            if (value is double confidence)
            {
                // Clamp confidence between 0.5 and 1.0
                confidence = Math.Max(0.5, Math.Min(1.0, confidence));

                // Interpolate between yellow (1,1,0) and green (0,1,0)
                // t = (confidence - 0.5) / 0.5, so t=0 is yellow, t=1 is green
                double t = (confidence - 0.5) / 0.5;
                byte r = (byte)(255 * (1 - t));
                byte g = 255;
                byte b = 0;

                return new SolidColorBrush(Color.FromRgb(r, g, b));
            }
            return System.Windows.DependencyProperty.UnsetValue;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
