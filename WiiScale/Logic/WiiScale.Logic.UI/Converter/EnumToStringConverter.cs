using System;
using System.Windows.Data;

namespace WiiScale.Logic.UI.Converter
{
    public class EnumToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string EnumString;

            try
            {
                EnumString = Enum.GetName((value.GetType()), value);
                return EnumString;
            }
            catch
            {
                return "can not convert enum !";
            }
        }

        // No need to implement converting back on a one-way binding 
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}