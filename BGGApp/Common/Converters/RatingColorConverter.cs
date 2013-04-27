using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace BGGApp.Common.Converters
{
    public class RatingColorConverter : IValueConverter
    {
        private SolidColorBrush DefaultBrush = new SolidColorBrush(Colors.Black);

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return DefaultBrush;

            decimal val;
            if (!decimal.TryParse(value.ToString(), out val))
                return DefaultBrush;

            if (val < 0 || val > 10)
                return DefaultBrush;

            Color gradeColor = GetGradeColor(val);
            return new SolidColorBrush(gradeColor);
        }

        private Color GetGradeColor(decimal val)
        {
            //http://stackoverflow.com/questions/2011832/generate-color-gradient-in-c-sharp

            int rMax, rMin, gMax, gMin, bMax, bMin;
            if (val < 5) // From Red to Orange
            {
                rMax = Colors.Orange.R;
                gMax = Colors.Orange.G;
                bMax = Colors.Orange.B;

                rMin = Colors.Red.R;
                gMin = Colors.Red.G;
                bMin = Colors.Red.B;
            }
            else // From Orange to Green
            {
                rMax = Colors.Green.R;
                gMax = Colors.Green.G;
                bMax = Colors.Green.B;

                rMin = Colors.Orange.R;
                gMin = Colors.Orange.G;
                bMin = Colors.Orange.B;
            }


            var rAverage = rMin + (int)((rMax - rMin) *  decimal.Divide((val * 10),50));
            var gAverage = gMin + (int)((gMax - gMin) * decimal.Divide((val * 10), 50));
            var bAverage = bMin + (int)((bMax - bMin) * decimal.Divide((val * 10), 50));


            return Color.FromArgb(255, (byte)rAverage, (byte)gAverage, (byte)bAverage);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException("RatingColorConverter cannot convert back");
        }
    }
}
