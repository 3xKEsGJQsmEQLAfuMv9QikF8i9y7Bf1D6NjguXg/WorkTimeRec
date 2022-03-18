using System.Windows;

namespace WorkTimeRec.ユーティリティ
{
    internal class メッセージボックス
    {
        public static MessageBoxResult エラー(string メッセージ) =>
            MessageBox.Show(メッセージ, "エラー", MessageBoxButton.OK, MessageBoxImage.Error);

        public static MessageBoxResult 確認(string メッセージ) =>
            MessageBox.Show(メッセージ, "確認", MessageBoxButton.YesNo, MessageBoxImage.Question);

        public static MessageBoxResult 警告(string メッセージ) =>
            MessageBox.Show(メッセージ, "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
    }
}
