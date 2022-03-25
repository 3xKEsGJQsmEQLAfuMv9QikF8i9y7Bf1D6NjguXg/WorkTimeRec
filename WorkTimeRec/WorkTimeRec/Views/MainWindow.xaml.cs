using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Shell;
using WorkTimeRec.キーボード;
using WorkTimeRec.コントロールロジック;
using WorkTimeRec.データ型;
using WorkTimeRec.ファイル;
using WorkTimeRec.ユーティリティ;

namespace WorkTimeRec.Views
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string WorkFreeBtnText = "開始";
        private const string WorkingBtnText = "作業中・・・";
        private static readonly int MinWorkIndex = 1;
        private static readonly int MaxWorkIndex = 5;

        private static readonly RoutedEventArgs _uiReactionSuppressEventArgs = new();
        private static readonly RoutedEventArgs _otherRoutedEventArgs = new();

        private readonly Dictionary<ToggleButton, ComboBox> _入力項目管理 = new();
        private readonly ObservableCollection<作業時間管理> _times = new();
        private readonly 作業時間ファイル _timesFile = new(パス操作.ベースパス);
        private readonly 処理制御 _終了処理;
        private 設定? _設定;

        public MainWindow()
        {
            InitializeComponent();

            _終了処理 = new 処理制御(() =>
            {
                全ボタンOFF();
                作業時間ファイル保存();
                if (_設定 is not null)
                {
                    _設定 = _設定 with { 並行作業 = IsParallelWork.IsChecked == true };
                    try
                    {
                        設定ファイル.保存(_設定);
                    }
                    catch (Exception ex)
                    {
                        メッセージボックス.エラー($"設定ファイルの保存に失敗しました。\r\n{ex}");
                    }
                }
            });
            App.Current.TerminateProc = 終了処理;
        }

        private void タイトルのテキスト設定()
        {
            int cnt = _入力項目管理.Count(x => x.Key.IsChecked == true);
            Title = cnt == 0
                ? App.AppName
                : $"{cnt} - {App.AppName}";
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            try
            {
                _設定 = 設定ファイル.読み込み();
                設定を画面に反映(_設定);
            }
            catch (Exception ex)
            {
                メッセージボックス.エラー($"設定ファイルの読み込みに失敗しました。\r\n{ex}");
                App.Current.Shutdown();
            }
        }

        private void 設定を画面に反映(設定 設定内容)
        {
            if (設定内容.並行作業保存)
            {
                IsParallelWork.IsChecked = 設定内容.並行作業;
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!インストール先チェック())
                {
                    App.Current.Shutdown();
                    return;
                }

                await コンボ再読み込みAsync();

                foreach (int i in Enumerable.Range(MinWorkIndex, MaxWorkIndex))
                {
                    if (FindName($"StartButton{i}") is ToggleButton btn &&
                        FindName($"WorkContent{i}") is ComboBox cmb)
                    {
                        _入力項目管理.Add(btn, cmb);
                    }
                }

                TimeList.DataContext = _times;
            }
            catch (Exception ex)
            {
                メッセージボックス.エラー(ex.ToString());
            }
        }

        private bool インストール先チェック()
        {
            if (パス操作.アプリパスチェック())
            {
                return true;
            }

            メッセージボックス.警告("管理者権限が必要な場所へはインストールしないでください。");

            if (メッセージボックス.確認("お勧めのインストール先フォルダーを作成しますか？") == MessageBoxResult.Yes)
            {
                string path = パス操作.推奨インストールパス取得();
                Directory.CreateDirectory(path);
                シェル操作.実行(path);
            }

            return false;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                終了処理();
            }
            catch (Exception ex)
            {
                メッセージボックス.エラー(ex.ToString());
            }
        }

        public void 終了処理()
        {
            _終了処理.一回実行();
        }

        private void 全ボタンOFF()
        {
            void ボタンOFF(ToggleButton btn)
            {
                btn.IsChecked = false;
                StartToggleButton_Click(btn, _uiReactionSuppressEventArgs);
            }

            foreach (var btn in _入力項目管理.Keys)
            {
                if (btn.IsChecked == true)
                {
                    ボタンOFF(btn);
                }
            }
        }

        private void 作業時間ファイル保存()
        {
            if (!_times.Any())
            {
                return;
            }

            _timesFile.ファイル保存(_times);
        }

        private int ボタン番号取得(ToggleButton btn)
        {
            return int.Parse(文字列操作.右端(btn.Name));
        }

        private string 作業内容取得(ToggleButton btn)
        {
            return _入力項目管理[btn].Text;
        }

        /// <summary>
        /// 開始/作業中ボタンクリック時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not ToggleButton btn)
            {
                return;
            }

            if (btn.IsChecked == true &&
                e != _uiReactionSuppressEventArgs &&
                string.IsNullOrEmpty(_入力項目管理[btn].Text))
            {
                btn.IsChecked = false;
                メッセージボックス.警告("作業内容を入力してください。");
                return;
            }

            try
            {
                btn.Content = btn.IsChecked == true
                    ? WorkingBtnText
                    : WorkFreeBtnText;

                int idx = ボタン番号取得(btn);
                string work = 作業内容取得(btn);

                if (btn.IsChecked == true)
                {
                    // 開始 → 作業中

                    作業開始(btn, idx, work);
                }
                else
                {
                    // 作業中 → 開始（作業終了）

                    作業終了(btn, idx, work);
                }

                if (IsParallelWork.IsChecked == true)
                {
                    タイトルのテキスト設定();

                    // 並行作業
                    return;
                }

                // 開始→作業中に変更 かつ 単一作業の場合

                他を作業終了(btn);
                タイトルのテキスト設定();
            }
            catch (Exception ex)
            {
                メッセージボックス.エラー(ex.ToString());
            }

        }

        private void 作業開始(ToggleButton btn, int idx, string work)
        {
            IsParallelWork.IsEnabled = false;
            SettingsButton.IsEnabled = false;

            タスクバー状態設定(処理開始: true);
            プログレスバー状態設定(idx, 処理開始: true);
            作業時間情報追加(idx, work);
            リストビュー操作.最初のリスト登録時の表示設定(TimeList);

            _入力項目管理[btn].IsEnabled = false;

            // コンボボックス選択肢の更新
            if (コンボボックス操作.コンボリスト更新(_入力項目管理[btn], work))
            {
                作業内容ファイル.一覧ファイル保存(_入力項目管理[btn].Items, 作業内容ファイル.ファイルパス取得(idx));
            }
        }

        private bool 作業中状態()
        {
            return _入力項目管理.Any(x => x.Key.IsChecked == true);
        }

        private void 作業終了(ToggleButton btn, int idx, string work)
        {
            if (!作業中状態())
            {
                タスクバー状態設定(処理開始: false);
                IsParallelWork.IsEnabled = true;
                SettingsButton.IsEnabled = true;
            }

            プログレスバー状態設定(idx, 処理開始: false);
            作業終了時の作業時間情報更新(idx, work);

            _入力項目管理[btn].IsEnabled = true;
        }

        private void タスクバー状態設定(bool 処理開始)
        {
            if (!処理開始)
            {
                TaskbarInfo.ProgressState = TaskbarItemProgressState.None;
                return;
            }

            if (_設定?.タスクバーアイコン == 作業中タスクバーアイコン.進捗固定)
            {
                TaskbarInfo.ProgressState = TaskbarItemProgressState.Normal;
            }
            else if (_設定?.タスクバーアイコン == 作業中タスクバーアイコン.アニメーション)
            {
                TaskbarInfo.ProgressState = TaskbarItemProgressState.Indeterminate;
            }
        }

        private void プログレスバー状態設定(int idx, bool 処理開始)
        {
            var pbar = FindName($"WorkProgress{idx}") as ProgressBar;
            if (pbar is null)
            {
                return;
            }

            if (!処理開始)
            {
                pbar.Visibility = Visibility.Collapsed;
                return;
            }

            if (_設定?.コンボボックス状態 == 作業中コンボボックス状態.色付け)
            {
                pbar.IsIndeterminate = false;
                pbar.Visibility = Visibility.Visible;
            }
            else if (_設定?.コンボボックス状態 == 作業中コンボボックス状態.アニメーション)
            {
                pbar.IsIndeterminate = true;
                pbar.Visibility = Visibility.Visible;
            }
            else
            {
                pbar.Visibility = Visibility.Collapsed;
            }
        }

        private void 他を作業終了(ToggleButton clickedButton)
        {
            foreach (var b in _入力項目管理.Keys)
            {
                // 押されたボタン、作業中以外のボタンは処理対象外
                if (b == clickedButton ||
                    b.IsChecked != true)
                {
                    continue;
                }

                var t = _times.FirstOrDefault(x =>
                    x.Index == ボタン番号取得(b) &&
                    x.終了 == DateTime.MinValue &&
                    x.作業内容 == 作業内容取得(b));
                if (t is not null)
                {
                    t.終了 = DateTime.Now;
                }
                b.IsChecked = false;
                b.Content = WorkFreeBtnText;
                _入力項目管理[b].IsEnabled = true;
                プログレスバー状態設定(ボタン番号取得(b), 処理開始: false);
            }

        }

        private void 作業時間情報追加(int idx, string work)
        {
            _times.Add(new 作業時間管理
            {
                Index = idx,
                作業内容 = work,
                開始 = DateTime.Now,
            });
        }

        private void 作業終了時の作業時間情報更新(int idx, string work)
        {
            // 作業終了の情報更新
            var t = _times.FirstOrDefault(x =>
                x.Index == idx &&
                x.終了 == DateTime.MinValue &&
                x.作業内容 == work);
            if (t is not null)
            {
                t.終了 = DateTime.Now;
            }
        }

        private async Task コンボ再読み込みAsync()
        {
            bool selectFirstItem = _設定?.起動時に作業コンボボックスのテキスト設定 == true;

            foreach (int i in Enumerable.Range(MinWorkIndex, MaxWorkIndex))
            {
                if (FindName($"WorkContent{i}") is ComboBox cmb)
                {
                    await コンボボックス操作.コンボ読み込みAsync(
                        cmb,
                        作業内容ファイル.ファイルパス取得(i),
                        selectFirstItem);
                }
            }
        }

        /// <summary>
        /// 設定ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new SettingsWindow(_設定!)
            {
                Owner = this
            };

            Opacity = 0.5;
            if (window.ShowDialog() == true)
            {
                _設定 = window.設定内容;
            }
            Opacity = 1.0;
        }

        /// <summary>
        /// ログフォルダボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LogButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!Directory.Exists(_timesFile.格納フォルダパス))
                {
                    メッセージボックス.警告("まだログフォルダは存在しません。");
                    return;
                }
                シェル操作.実行(_timesFile.格納フォルダパス);
            }
            catch (Exception ex)
            {
                メッセージボックス.エラー(ex.ToString());
            }
        }

        /// <summary>
        /// 作業クリアボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            コンボボックス操作.テキストクリア(_入力項目管理.Values.ToList());
        }

        /// <summary>
        /// 作業停止ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            if (_設定?.作業終了の確認 == true)
            {
                if (メッセージボックス.確認("すべての作業を終了しますか？") != MessageBoxResult.Yes)
                {
                    return;
                }
            }

            全ボタンOFF();
        }

        /// <summary>
        /// 閉じるボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ボタンクリック(ToggleButton btn)
        {
            btn.Focus();
            btn.IsChecked = !btn.IsChecked;
            StartToggleButton_Click(btn, _otherRoutedEventArgs);
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (キー操作.CtrlとShiftキー押下)
            {
                // Ctrl + Shift
                switch (e.Key)
                {
                    case System.Windows.Input.Key.D1:
                        ボタンクリック(StartButton1);
                        break;
                    case System.Windows.Input.Key.D2:
                        ボタンクリック(StartButton2);
                        break;
                    case System.Windows.Input.Key.D3:
                        ボタンクリック(StartButton3);
                        break;
                    case System.Windows.Input.Key.D4:
                        ボタンクリック(StartButton4);
                        break;
                    case System.Windows.Input.Key.D5:
                        ボタンクリック(StartButton5);
                        break;
                    default:
                        break;
                }
            }
            else if (キー操作.Ctrlキー押下)
            {
                // Ctrl
                switch (e.Key)
                {
                    case System.Windows.Input.Key.D1:
                        WorkContent1.Focus();
                        break;
                    case System.Windows.Input.Key.D2:
                        WorkContent2.Focus();
                        break;
                    case System.Windows.Input.Key.D3:
                        WorkContent3.Focus();
                        break;
                    case System.Windows.Input.Key.D4:
                        WorkContent4.Focus();
                        break;
                    case System.Windows.Input.Key.D5:
                        WorkContent5.Focus();
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// リストビュー サイズ変更時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimeList_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            リストビュー操作.サイズ変更(sender, e);
        }

        /// <summary>
        /// 作業コンボボックス キー押下時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WorkContent_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (sender is not ComboBox cmb)
            {
                return;
            }

            if (e.Key != System.Windows.Input.Key.Enter)
            {
                return;
            }

            ボタンクリック(_入力項目管理.First(x => x.Value == cmb).Key);
        }

    }
}
