using System;
using System.Globalization;

namespace WorkTimeRec.ユーティリティ
{
    internal class 時間操作
    {
        public static string 現在日付取得()
        {
            return 年月日文字列(DateTime.Now, false);
        }

        public static string 年月日文字列(DateTime dt, bool 区切り文字あり)
        {
            if (dt == DateTime.MinValue)
            {
                return "";
            }

            return 区切り文字あり
                ? dt.ToString(@"yyyy\/MM\/dd", CultureInfo.InvariantCulture)
                : dt.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
        }

        public static string 時間間隔文字列(TimeSpan ts, bool zeroPadding = false)
        {
            if (ts == TimeSpan.MinValue ||
                ts.TotalSeconds < 1)
            {
                return zeroPadding ? "00:00:00" : "";
            }

            return ts.ToString(@"hh\:mm\:ss");
        }

        public static string 時刻文字列(DateTime dt, bool 区切り文字あり, bool 秒あり)
        {
            if (dt == DateTime.MinValue)
            {
                return "";
            }

            if (区切り文字あり)
            {
                return 秒あり
                    ? dt.ToString(@"HH\:mm\:ss", CultureInfo.InvariantCulture)
                    : dt.ToString(@"HH\:mm", CultureInfo.InvariantCulture);
            }
            else
            {
                return 秒あり
                    ? dt.ToString("HHmmss", CultureInfo.InvariantCulture)
                    : dt.ToString("HHmm", CultureInfo.InvariantCulture);
            }
        }
    }
}
