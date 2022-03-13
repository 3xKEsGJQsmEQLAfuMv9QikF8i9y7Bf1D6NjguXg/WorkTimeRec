using System;
using System.Globalization;
using System.Windows.Data;
using WorkTimeRec.ユーティリティ;

namespace WorkTimeRec.Converters
{
    [ValueConversion(typeof(TimeSpan), typeof(string))]
    internal class TimeSpanConverter : IMultiValueConverter
    {
        //public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        //{
        //    if (value is null ||
        //        (TimeSpan)value == TimeSpan.MinValue)
        //    {
        //        return "";
        //    }
            
        //    return 時間操作.時間間隔文字列((TimeSpan)value);
        //}

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values is null ||
                values.Length < 2 ||
                (DateTime)values[0] == DateTime.MinValue ||
                (TimeSpan)values[1] == TimeSpan.MinValue)
            {
                return "";
            }

            return 時間操作.時間間隔文字列((TimeSpan)values[1], zeroPadding: true);
        }

        //public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        //{
        //    throw new NotImplementedException();
        //}

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
