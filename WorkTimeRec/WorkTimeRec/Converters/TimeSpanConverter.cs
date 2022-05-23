using System;
using System.Globalization;
using System.Windows.Data;
using WorkTimeRec.ユーティリティ;

namespace WorkTimeRec.Converters
{
    [ValueConversion(typeof(TimeSpan), typeof(string))]
    internal class TimeSpanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not TimeSpan t ||
                t.TotalSeconds < 1)
            {
                return "--:--:--";
            }

            return 時間操作.時間間隔文字列(t, zeroPadding: true);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
}
