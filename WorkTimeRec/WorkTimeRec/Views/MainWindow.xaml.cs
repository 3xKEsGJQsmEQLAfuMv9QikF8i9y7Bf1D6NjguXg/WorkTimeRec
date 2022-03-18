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

        private readonly Dictionary<ToggleButton, ComboBox> _入力項目管理 = new();
        private readonly ObservableCollection<作業時間管理> _times = new();
        private readonly 作業時間ファイル _timesFile = new(パス操作.ベースパス);
        private readonly 処理制御 _終了処理;

        public MainWindow()
        {
            InitializeComponent();
            _終了処理 = new 処理制御(() =>
            {
                全ボタンOFF();
                作業時間ファイル保存();
            });
            App.Current.TerminateProc = 終了処理;
        }

        private void タイトルのテキスト設定()
        {
            int cnt = _入力項目管理.Count(x => x.Key.IsChecked == true);
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
                            _入力項目管理.Add(btn, cmb);
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

        /// <summary>
        /// 開始/作業中ボタンクリック時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartToggleButton_Click(object sender, RoutedEventArgs e)
        {
            #region ローカル関数

            int ボタン番号取得(ToggleButton btn)
            {
                return int.Parse(文字列操作.右端(btn.Name));
            }

            string 作業内容取得(ToggleButton btn)
            {
                return _入力項目管理[btn].Text;
            }

            #endregion

            if (sender is not ToggleButton btn)
            {
                return;
            }

            if (btn.IsChecked == true &&
                e != _otherRoutedEventArgs &&
                string.IsNullOrEmpty(_入力項目管理[btn].Text))
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

                int idx = ボタン番号取得(btn);
                string work = 作業内容取得(btn);

                if (btn.IsChecked == true)
                {
                    // 開始 → 作業中

                    作業時間情報追加(idx, work);
                    リストビュー操作.最初のリスト登録時の表示設定(TimeList);

                    _入力項目管理[btn].IsEnabled = false;

                    // コンボボックス選択肢の更新
                    if (コンボボックス操作.コンボリスト更新(_入力項目管理[btn], work))
                    {
                        作業内容ファイル.一覧ファイル保存(_入力項目管理[btn].Items, 作業内容ファイル.ファイルパス取得(idx));
                    }
                }
                else
                {
                    // 作業中 → 開始（作業終了）

                    作業終了時の作業時間情報更新(idx, work);

                    _入力項目管理[btn].IsEnabled = true;
                }

                if (IsParallelWork.IsChecked == true)
                {
                    // タイトル設定
                    タイトルのテキスト設定();

                    // 並行作業
                    return;
                }

                // 開始→作業中に変更 かつ 単一作業の場合
                foreach (var b in _入力項目管理.Keys)
                {
                    if (b == btn)
                    {
                        continue;
                    }
                    if (b.IsChecked == true)
                    {
                        var t = _times.FirstOrDefault(
                            x =>
                            x.Index == ボタン番号取得(b) &&
                            x.終了 == DateTime.MinValue &&
                            x.作業内容 == 作業内容取得(b));
                        if (t is not null)
                        {
                            t.終了 = DateTime.Now;
                        }
                        b.IsChecked = false;
                        b.Content = _workFreeText;
                        _入力項目管理[b].IsEnabled = true;
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
            コンボボックス操作.テキストクリア(_入力項目管理.Values.ToList());
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
            if (!(sender is ComboBox cmb))
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
