using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WorkTimeRec.ファイル;
using WorkTimeRec.ユーティリティ;

namespace WorkTimeRec.Views
{
    /// <summary>
    /// SearchWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class SearchWindow : Window
    {
        private readonly 作業時間ファイル _timesFile;

        public SearchWindow(作業時間ファイル timesFile)
        {
            _timesFile = timesFile;
            InitializeComponent();
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                IsEnabled = false;

                if (string.IsNullOrWhiteSpace(SearchText.Text))
                {
                    IsEnabled = true;
                    メッセージボックス.警告("検索文字列を入力してください。");
                    SearchText.Focus();
                    return;
                }

                int searchCnt = 50;
                foreach (var radio in コントロール操作.子取得<RadioButton>(SelectTopContainer))
                {
                    if (radio.IsChecked == true)
                    {
                        if (int.TryParse(radio.Tag.ToString(), out searchCnt))
                        {
                            break;
                        }
                    }
                }

                await ファイル検索Async(
                    SearchText.Text,
                    searchCnt);

                IsEnabled = true;
            }
            catch (Exception ex)
            {
                IsEnabled = true;
                メッセージボックス.エラー(ex.ToString());
            }
        }

        private async Task ファイル検索Async(string keyword, int 取得件数)
        {
            var files = 対象ファイル一覧取得(取得件数);

            var buf = new StringBuilder();
            buf.AppendLine(作業時間ファイル.ヘッダー);

            foreach (var f in files)
            {
                await 一ファイル読み込みAsync(f.FullName, Encoding.UTF8, keyword, buf);
            }
            ResultText.Text = buf.ToString();
        }

        private IOrderedEnumerable<FileInfo> 対象ファイル一覧取得(int 取得件数)
        {
            var di = new DirectoryInfo($"{_timesFile.格納フォルダパス}");
            if (!di.Exists)
            {
                return Enumerable.Empty<FileInfo>().OrderBy(x => x);
            }

            var files = di.GetFiles("????????.log", SearchOption.TopDirectoryOnly)
                .OrderByDescending(f => f.Name)!;

            return 取得件数 < 1
                ? files
                : files.Take(取得件数).OrderByDescending(f => f.Name);
        }

        private async Task 一ファイル読み込みAsync(
            string filePath, Encoding encoding, string keyword, StringBuilder buf)
        {
            using var reader = new StreamReader(filePath, encoding);
            bool firstLine = true;

            while (!reader.EndOfStream)
            {
                // 一行読み込み
                string? line = await reader.ReadLineAsync();
                if (line is null)
                {
                    continue;
                }

                if (firstLine)
                {
                    // ヘッダー行は読み飛ばす
                    firstLine = false;
                    continue;
                }

                バッファに追加(line, keyword, buf);
            }
        }

        private void バッファに追加(string line, string keyword, StringBuilder buf)
        {
            string[] columns = line.Split('\t');
            if (columns.Length < 作業時間ファイル.列数)
            {
                return;
            }

            if (columns[作業時間ファイル.作業内容インデックス].IndexOf(
                keyword, StringComparison.OrdinalIgnoreCase) != -1)
            {
                buf.AppendLine(line);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (SearchText.TextContent.IsFocused &&
                    !string.IsNullOrEmpty(SearchText.Text))
                {
                    SearchText.Clear();
                }
                else
                {
                    e.Handled = true;
                    Close();
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            bool visible = (bool)e.NewValue;
            if (visible)
            {
                Window_Shown();
            }
        }

        private void Window_Shown()
        {
            SearchText.Focus();
        }
    }
}
