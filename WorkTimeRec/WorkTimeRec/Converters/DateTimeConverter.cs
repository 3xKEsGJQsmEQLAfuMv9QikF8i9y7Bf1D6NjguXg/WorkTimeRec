using System;
using System.Globalization;
using System.Windows.Data;
using WorkTimeRec.ユーティリティ;

namespace WorkTimeRec.Converters
{
    [ValueConversion(typeof(DateTime), typeof(string))]
    internal class DateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null ||
                (DateTime)value == DateTime.MinValue)
            {
                return "";
            }

            return 時間操作.時刻文字列((DateTime)value, true, true);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
