namespace WorkTimeRec.ユーティリティ
{
    internal class 文字列操作
    {
        public static string 右端(string? value)
        {
            if (value is null)
            {
                return "";
            }
            if (value.Length <= 1)
            {
                return value;
            }

            // 末尾の1文字を返す
            return value[^1..];
        }
    }
}
