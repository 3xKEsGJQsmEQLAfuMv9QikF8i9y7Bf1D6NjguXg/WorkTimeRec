using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using WorkTimeRec.キーボード;
using WorkTimeRec.データ型;
using WorkTimeRec.ファイル;
using WorkTimeRec.ユーティリティ;

namespace WorkTimeRec.Views
{
    /// <summary>
    /// WorkHistoryWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class WorkHistoryWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private static readonly SolidColorBrush _normalListForecolor = new(Colors.White);
        private static readonly SolidColorBrush _disabledlListForecolor = new(Colors.DarkGray);

        private double _progressWidth = 200;
        public double ProgressWidth
        {
            get => _progressWidth;
            set
            {
                if (value != _progressWidth)
                {
                    _progressWidth = value;
                    RaisePropertyChanged();
                }
            }
        }

        public int SelectedNo { get; private set; } = 0;
        public string SelectedText { get; private set; } = "";

        private readonly ObservableCollection<作業内容と時間>[] _graphDatas = { new(), new(), new(), new(), new() };
        private ReadOnlyObservableCollection<作業時間管理>? _times;
        private static readonly ReadOnlyObservableCollection<作業時間管理> 空リスト = new(new());
        private readonly 作業時間ファイル _timesFile;
        public SearchWindow? SearchWindow { get; private set; }
        private bool _graphLoaded = false;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="timesFile"></param>
        /// <param name="searchWindow"></param>
        public WorkHistoryWindow(作業時間ファイル timesFile, ref SearchWindow? searchWindow)
        {
            _timesFile = timesFile;
            SearchWindow = searchWindow;
            InitializeComponent();
            DataContext = this;
        }

        public void 作業項目設定(ComboBox[] combos)
        {
            const int 履歴種類数 = 5;

            if (combos.Length != 履歴種類数)
            {
                throw new ArgumentException("引数不正");
            }

            for (int i = 1; i <= 履歴種類数; i++)
            {
                作業項目一列分設定(i, combos[i - 1]);
            }
        }

        private void 作業項目一列分設定(int no, ComboBox combo)
        {
            if (FindName($"Txt{no}") is TextBox txt)
            {
                txt.Text = combo.Text;
            }

            if (FindName($"List{no}") is ListBox list)
            {
                list.ItemsSource = combo.Items;
                list.SelectedIndex = 0;

                list.Foreground = combo.IsEnabled
                    ? _normalListForecolor
                    : _disabledlListForecolor;
                list.IsHitTestVisible = combo.IsEnabled;
            }
        }

        public void 作業時間設定(ReadOnlyObservableCollection<作業時間管理> list)
        {
            _times = list;
        }

        private void ListItem_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is not ListBoxItem item)
            {
                return;
            }
            if (GraphButton.IsChecked == true)
            {
                return;
            }

            SelectedText = item.Content.ToString() ?? "";
            if (string.IsNullOrEmpty(SelectedText))
            {
                return;
            }

            var list = コントロール操作.親取得<ListBox>(item);

            SelectedNo = int.TryParse(文字列操作.右端(list?.Name), out int no) ? no : 0;

            Hide();
        }

        private void List_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
            {
                return;
            }
            if (GraphButton.IsChecked == true)
            {
                return;
            }

            if (sender is not ListBox list ||
                list.SelectedIndex == -1 ||
                !list.IsHitTestVisible)
            {
                return;
            }

            SelectedText = list.SelectedItem?.ToString() ?? "";
            if (string.IsNullOrEmpty(SelectedText))
            {
                return;
            }

            SelectedNo = int.Parse(文字列操作.右端(list.Name));
            Hide();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            _graphLoaded = false;
            Hide();
        }

        private async void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            bool visible = (bool)e.NewValue;
            if (visible)
            {
                await Window_ShownAsync();
            }
        }

        /// <summary>
        /// ウィンドウ表示時
        /// </summary>
        private async Task Window_ShownAsync()
        {
            InitializeItemIndex();
            if (GraphButton.IsChecked == true)
            {
                await グラフリストボックス初期化Async();
                _graphLoaded = true;
            }
        }

        private void InitializeItemIndex()
        {
            SelectedNo = 0;
            SelectedText = "";
        }

        private async Task グラフリストボックス初期化Async()
        {
            if (_timesFile is null)
            {
                throw new Exception("初期化ミス");
            }

            TxtG5.Text = 時間操作.月日と曜日文字列(DateTime.Now);
            for (int i = 0; i < _graphDatas.Length; i++)
            {
                if (FindName($"ListG{i + 1}") is not ListBox list)
                {
                    continue;
                }

                _graphDatas[i].Clear();

                list.ItemsSource = _graphDatas[i];
            }

            await 今日の作業時間設定Async(_graphDatas[4]);
            await 以前の作業時間設定Async(_graphDatas);

            作業内容と時間.時間割合計算(_graphDatas);

            for (int i = 0; i < _graphDatas.Length; i++)
            {
                // ソート設定
                var cvs = CollectionViewSource.GetDefaultView(_graphDatas[i]);
                cvs.SortDescriptions.Add(new SortDescription("Value", ListSortDirection.Descending));
            }
        }

        private async Task 今日の作業時間設定Async(ObservableCollection<作業内容と時間> graphList)
        {
            var 今日 = DateTime.Now;
            try
            {
                await ログファイル読み込みAsync(graphList, 今日);
            }
            catch (Exception ex)
            {
                メッセージボックス.エラー(ex.ToString());
                return;
            }

            作業時間一覧の内容を反映(graphList, 今日);
        }

        private async Task ログファイル読み込みAsync(ObservableCollection<作業内容と時間> list, DateTime date)
        {
            await foreach (string[]? columns in _timesFile.ファイル読み込みAsync(date))
            {
                if (columns is null ||
                    columns.Length == 0)
                {
                    continue;
                }

                作業内容ごとに作業時間集計(
                    columns[作業時間ファイル.作業時間インデックス],
                    columns[作業時間ファイル.作業内容インデックス],
                    list);
            }

        }

        private void 作業時間一覧の内容を反映(ObservableCollection<作業内容と時間> graphList, DateTime date)
        {
            foreach (var item in _times ?? 空リスト)
            {
                if (item.開始.Date != date.Date)
                {
                    continue;
                }

                var 一致項目 = graphList.FirstOrDefault(x =>
                    x.作業内容 == item.作業内容);

                if (一致項目 is null)
                {
                    graphList.Add(new(item.作業内容, item.作業時間));
                    continue;
                }

                if (0 < item.作業時間.TotalSeconds)
                {
                    一致項目.作業時間 += item.作業時間;
                }
            }
        }

        private async Task 以前の作業時間設定Async(ObservableCollection<作業内容と時間>[] graphList)
        {
            var files = 対象ファイル一覧取得();
            DateTime 今日 = DateTime.Now;

            var 対象日候補 = new SortedSet<DateTime>();
            
            画面の一覧から対象日候補収集(今日, 対象日候補);
            ログファイルから対象日候補収集(今日, files, 対象日候補);

            int columnIndex = 3;
            foreach (var 対象日 in 対象日候補.OrderByDescending(x => x.Date).Take(4))
            {
                if (FindName($"TxtG{columnIndex + 1}") is not TextBox txtBox)
                {
                    break;
                }
                txtBox.Text = 時間操作.月日と曜日文字列(対象日);

                作業時間一覧の内容を反映(graphList[columnIndex], 対象日);

                var f = files.FirstOrDefault(x => x.Name.StartsWith(時間操作.年月日文字列(対象日, false)));
                if (f is not null)
                {
                    await 指定日の作業時間設定Async(graphList[columnIndex], columnIndex + 1, f.Name);
                }

                columnIndex--;
                if (columnIndex < 0)
                {
                    break;
                }
            }
        }

        private void 画面の一覧から対象日候補収集(DateTime 今日, SortedSet<DateTime> 対象日候補)
        {
            var 画面の作業時間一覧 = _times
                ?.Where(x => x.開始.Date < 今日.Date)
                .GroupBy(x => x.開始.Date)
                .OrderByDescending(x => x.Key.Date);

            if (画面の作業時間一覧 is not null)
            {
                foreach (var item in 画面の作業時間一覧)
                {
                    対象日候補.Add(item.Key.Date);
                    if (4 <= 対象日候補.Count)
                    {
                        return;
                    }
                }
            }
        }

        private void ログファイルから対象日候補収集(
            DateTime 今日,
            IOrderedEnumerable<FileInfo> files,
            SortedSet<DateTime> 対象日候補)
        {
            int cnt = 0;
            string 今日の年月日文字列 = 時間操作.年月日文字列(今日, false);

            foreach (var f in files)
            {
                if (f.Name.Substring(0, 8).CompareTo(今日の年月日文字列) != -1)
                {
                    continue;
                }

                if (!時間操作.年月日に変換(Path.GetFileNameWithoutExtension(f.Name), out DateTime d))
                {
                    continue;
                }

                対象日候補.Add(d.Date);
                cnt++;

                if (4 <= cnt)
                {
                    return;
                }
            }
        }

        private IOrderedEnumerable<FileInfo> 対象ファイル一覧取得()
        {
            var di = new DirectoryInfo($"{_timesFile.格納フォルダパス}");
            if (!di.Exists)
            {
                return Enumerable.Empty<FileInfo>().OrderBy(x => x);
            }

            return di.GetFiles("????????.log", SearchOption.TopDirectoryOnly)
                .OrderByDescending(f => f.Name);
        }

        private async Task<bool> 指定日の作業時間設定Async(
            ObservableCollection<作業内容と時間> list, int columnIndex, string fname)
        {
            if (!時間操作.年月日に変換(Path.GetFileNameWithoutExtension(fname), out DateTime d))
            {
                return false;
            }

            await ログファイル読み込みAsync(list, d);

            作業時間一覧の内容を反映(list, d);

            return true;
        }

        private void 作業内容ごとに作業時間集計(
            string time, string work, ObservableCollection<作業内容と時間> list)
        {
            if (TimeSpan.TryParse(time, out TimeSpan t))
            {
                var 一致項目 = list.FirstOrDefault(x => x.作業内容 == work);

                if (一致項目 is null)
                {
                    list.Add(new(work, t));
                }
                else
                {
                    一致項目.作業時間 += t;
                }
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (キー操作.Ctrlキー押下)
            {
                // Ctrl

                Ctrlショートカットキー処理(sender, e);
            }
        }

        private void Ctrlショートカットキー処理(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.D1:
                    リストにフォーカス設定(1);
                    break;
                case Key.D2:
                    リストにフォーカス設定(2);
                    break;
                case Key.D3:
                    リストにフォーカス設定(3);
                    break;
                case Key.D4:
                    リストにフォーカス設定(4);
                    break;
                case Key.D5:
                    リストにフォーカス設定(5);
                    break;
                case Key.F:
                    検索画面表示();
                    break;
                case Key.G:
                    GraphButton.Focus();
                    GraphButton.IsChecked = true;
                    break;
                case Key.I:
                    ItemButton.Focus();
                    ItemButton.IsChecked = true;
                    break;
                default:
                    break;
            }
        }

        private void 検索画面表示()
        {
            Opacity = 0.5;
            try
            {
                if (SearchWindow is null)
                {
                    SearchWindow = new SearchWindow(_timesFile);
                }
                SearchWindow.Owner = this;

                SearchWindow.ShowDialog();
            }
            finally
            {
                Opacity = 1.0;
            }
        }

        private void リストにフォーカス設定(int no)
        {
            if (ItemButton.IsChecked == true)
            {
                (FindName($"List{no}") as ListBox)?.Focus();
            }
            else
            {
                (FindName($"ListG{no}") as ListBox)?.Focus();
            }

        }

        /// <summary>
        /// 「作業履歴検索」ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            検索画面表示();
        }

        /// <summary>
        /// 「作業時間グラフ」ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void GraphButton_Checked(object sender, RoutedEventArgs e)
        {
            if (!_graphLoaded)
            {
                ItemButton.IsEnabled = false;
                GraphButton.IsEnabled = false;

                await グラフリストボックス初期化Async();
                _graphLoaded = true;

                ItemButton.IsEnabled = true;
                GraphButton.IsEnabled = true;
            }

        }

        /// <summary>
        /// 「閉じる」ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            _graphLoaded = false;
            Hide();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            プログレスバー幅設定();
        }

        private void プログレスバー幅設定()
        {
            if (RenderSize.Width < 1150)
            {
                ProgressWidth = 100;
            }
            else if (RenderSize.Width < 1800)
            {
                ProgressWidth = 200;
            }
            else
            {
                ProgressWidth = 300;
            }
        }

    }
}
