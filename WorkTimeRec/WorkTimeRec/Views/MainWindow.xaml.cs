using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
        private const string _workFreeText = "開始";
        private const string _workingText = "作業中・・・";
        private static readonly RoutedEventArgs _uiReactionSuppressEventArgs = new();
        private static readonly RoutedEventArgs _otherRoutedEventArgs = new();
        private static readonly int _minWorkIndex = 1;
        private static readonly int _maxWorkIndex = 5;

        private readonly Dictionary<ToggleButton, ComboBox> _buttonCombos = new();
        private readonly ObservableCollection<作業時間管理> _times = new();
        private readonly 作業時間ファイル _timesFile = new(パス操作.ベースパス取得());

        public MainWindow()
        {
            InitializeComponent();
            App.Current.TerminateProc = 終了処理;
        }

        private void タイトルのテキスト設定()
        {
            int cnt = _buttonCombos.Count(x => x.Key.IsChecked == true);
            Title = cnt == 0
                ? App.AppName :
                $"{cnt} - {App.AppName}";
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

                foreach (int i in Enumerable.Range(_minWorkIndex, _maxWorkIndex))
                {
                    if (FindName($"StartButton{i}") is ToggleButton btn)
                    {
                        if (FindName($"WorkContent{i}") is ComboBox cmb)
                        {
                            _buttonCombos.Add(btn, cmb);
                        }
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
            全ボタンOFF();
            作業時間ファイル保存();
        }

        private void 全ボタンOFF()
        {
            void ボタンOFF(ToggleButton btn)
            {
                btn.IsChecked = false;
                StartToggleButton_Click(btn, _uiReactionSuppressEventArgs);
            }

            foreach (var btn in _buttonCombos.Keys)
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

        /// <summary>
        /// 開始/作業中ボタンクリック時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartToggleButton_Click(object sender, RoutedEventArgs e)
        {
            #region ローカル関数

            int GetIndex(ToggleButton btn)
            {
                return int.Parse(文字列操作.右端(btn.Name));
            }

            string GetWork(ToggleButton btn)
            {
                return _buttonCombos[btn].Text;
            }

            #endregion

            if (sender is not ToggleButton btn)
            {
                return;
            }

            if (btn.IsChecked == true &&
                e != _otherRoutedEventArgs &&
                string.IsNullOrEmpty(_buttonCombos[btn].Text))
            {
                btn.IsChecked = false;
                メッセージボックス.警告("作業内容を入力してください。");
                return;
            }

            try
            {
                btn.Content = btn.IsChecked == true
                    ? _workingText
                    : _workFreeText;

                int idx = GetIndex(btn);
                string work = GetWork(btn);

                if (btn.IsChecked == true)
                {
                    // 開始 → 作業中

                    作業時間情報追加(idx, work);
                    最初のリスト登録時の表示設定();

                    _buttonCombos[btn].IsEnabled = false;

                    // コンボボックス選択肢の更新
                    if (コンボボックス操作.コンボリスト更新(_buttonCombos[btn], work))
                    {
                        作業内容ファイル.一覧ファイル保存(_buttonCombos[btn].Items, 作業内容ファイル.ファイルパス取得(idx));
                    }
                }
                else
                {
                    // 作業中 → 開始（作業終了）

                    作業終了時の作業時間情報更新(idx, work);

                    _buttonCombos[btn].IsEnabled = true;
                }

                if (IsParallelWork.IsChecked == true)
                {
                    // タイトル設定
                    タイトルのテキスト設定();

                    // 並行作業
                    return;
                }

                // 開始→作業中に変更 かつ 単一作業の場合
                foreach (var b in _buttonCombos.Keys)
                {
                    if (b == btn)
                    {
                        continue;
                    }
                    if (b.IsChecked == true)
                    {
                        var t = _times.FirstOrDefault(
                            x =>
                            x.Index == GetIndex(b) &&
                            x.終了 == DateTime.MinValue &&
                            x.作業内容 == GetWork(b));
                        if (t is not null)
                        {
                            t.終了 = DateTime.Now;
                        }
                        b.IsChecked = false;
                        b.Content = _workFreeText;
                        _buttonCombos[b].IsEnabled = true;
                    }
                }
                // タイトル設定
                タイトルのテキスト設定();
            }
            catch (Exception ex)
            {
                メッセージボックス.エラー(ex.ToString());
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
            var t = _times.FirstOrDefault(
                x =>
                x.Index == idx &&
                x.終了 == DateTime.MinValue &&
                x.作業内容 == work);
            if (t is not null)
            {
                t.終了 = DateTime.Now;
            }
        }

        private void 最初のリスト登録時の表示設定()
        {
            if (TimeList.Opacity != 0.9)
            {
                TimeList.Opacity = 0.9;
            }
        }

        private async Task コンボ再読み込みAsync()
        {
            foreach (int i in Enumerable.Range(_minWorkIndex, _maxWorkIndex))
            {
                if (FindName($"WorkContent{i}") is ComboBox cmb)
                {
                    await コンボボックス操作.コンボ読み込みAsync(cmb, 作業内容ファイル.ファイルパス取得(i));
                }
            }
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
            コンボボックス操作.テキストクリア(_buttonCombos.Values.ToList());
        }

        /// <summary>
        /// 作業停止ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
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

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            void ボタンクリック(ToggleButton btn)
            {
                btn.Focus();
                btn.IsChecked = !btn.IsChecked;
                StartToggleButton_Click(btn, _otherRoutedEventArgs);
            }

            if (キー操作.Ctrlキー押下())
            {
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
        }

        private void TimeList_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!(sender is ListView lv))
            {
                return;
            }

            if (!(lv.View is GridView v))
            {
                return;
            }

            const double MarginW = 18.0;
            const double MinW = 75.0;
            var w = lv.ActualWidth
                - v.Columns[0].ActualWidth
                - v.Columns[1].ActualWidth
                - v.Columns[2].ActualWidth
                - MarginW;
            if (w < MinW)
            {
                w = MinW;
            }
            v.Columns[3].Width = w;
        }

    }
}
