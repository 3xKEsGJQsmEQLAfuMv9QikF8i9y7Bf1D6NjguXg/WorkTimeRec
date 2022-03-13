using System;
using System.IO;

namespace WorkTimeRec.ユーティリティ
{
    internal class パス操作
    {
        public static string ベースパス取得()
        {
            return System.AppContext.BaseDirectory;
        }

        public static string フルパス取得(string ファイル名)
        {
            return Path.Combine(ベースパス取得(), ファイル名);
        }

        public static bool アプリパスチェック()
        {
            string basePath = ベースパス取得();

            if (basePath.StartsWith(
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), StringComparison.OrdinalIgnoreCase) ||
                basePath.StartsWith(
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), StringComparison.OrdinalIgnoreCase))
            {
                // NG
                return false;
            }

            return true;
        }

        public static string 推奨インストールパス取得()
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                @"Tools\WorkTimeRec");
        }
    }
}
