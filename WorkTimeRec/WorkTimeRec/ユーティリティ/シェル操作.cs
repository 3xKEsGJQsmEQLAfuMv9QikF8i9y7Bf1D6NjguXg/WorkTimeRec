using System.Diagnostics;

namespace WorkTimeRec.ユーティリティ
{
    internal class シェル操作
    {
        public static bool 実行(string コマンド)
        {
            var p = new Process();
            p.StartInfo.FileName = コマンド;
            p.StartInfo.UseShellExecute = true;
            return p.Start();
        }
    }
}
