using System;
using System.Globalization;
using System.Windows.Data;

namespace WorkTimeRec.Converters
{
    [ValueConversion(typeof(bool), typeof(bool?))]
    internal class BooleansToBooleanConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values is null ||
                values.Length < 1)
            {
                return false;
            }

            foreach (var value in values)
            {
                if (value is bool b &&
                    b == true)
                {
                    // 1つでもtrueがあればtrueを返す
                    return true;
                }
            }

            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
