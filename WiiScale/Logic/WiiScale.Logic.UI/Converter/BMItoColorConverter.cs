using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace WiiScale.Logic.UI.Converter
{
    public class BMItoColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return new SolidColorBrush(Colors.Black);

            var bmi = (double) value;

            if (bmi < 25.0)
            {
                return new SolidColorBrush(Colors.Green);
            }
            else if (bmi > 25.0 && bmi < 30.0)
            {
                return new SolidColorBrush(Colors.Orange);
            }
            else
            {
                return new SolidColorBrush(Colors.Red);
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}