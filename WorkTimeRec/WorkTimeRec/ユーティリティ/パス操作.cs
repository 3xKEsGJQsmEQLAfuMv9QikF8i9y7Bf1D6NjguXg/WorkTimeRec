using System;
using System.IO;

namespace WorkTimeRec.ユーティリティ
{
    internal class パス操作
    {
        public static string ベースパス =>
            System.AppContext.BaseDirectory;

        public static string ベースフルパス取得(string ファイル名) =>
            Path.Combine(ベースパス, ファイル名);

        public static bool アプリパスチェック()
        {
            if (ベースパス.StartsWith(
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), StringComparison.OrdinalIgnoreCase) ||
                ベースパス.StartsWith(
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), StringComparison.OrdinalIgnoreCase))
            {
                // NG
                return false;
            }

            return true;
        }

        public static string 推奨インストールパス取得() => Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                @"Tools\WorkTimeRec");
    }
}
