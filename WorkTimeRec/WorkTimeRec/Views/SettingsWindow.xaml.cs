using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WorkTimeRec.データ型;
using WorkTimeRec.ファイル;
using WorkTimeRec.ユーティリティ;

namespace WorkTimeRec.Views
{
    /// <summary>
    /// SettingsWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public 設定 設定内容 { get; private set; }
        private readonly 作業時間ファイル _timesFile;
        private const int 保持する作業時間ログファイル数 = 30;
        private const int 保持する作業項目履歴数 = 40;
        private const string ゴミ箱マーク = "🗑";
        private readonly ComboBox[] _combos;

        public SettingsWindow(設定 設定内容, 作業時間ファイル timesFile, ComboBox[] combos)
        {
            InitializeComponent();
            this.設定内容 = 設定内容;
            _timesFile = timesFile;
            _combos = combos;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            作業時間ログ削除ボタン状態更新();
            作業項目削除ボタン状態更新();
            設定内容を画面に適用();
        }

        private void 作業時間ログ削除ボタン状態更新()
        {
            const string BtnText = "古い作業時間ログ削除";

            LogDeleteButton.IsEnabled = false;
            LogDeleteButton.Content = BtnText;

            var di = new DirectoryInfo($"{_timesFile.格納フォルダパス}");
            if (!di.Exists)
            {
                return;
            }

            var files = di.GetFiles("????????.log", SearchOption.TopDirectoryOnly);
            int ファイル数 = files.Length;
            if (ファイル数 <= 保持する作業時間ログファイル数)
            {
                return;
            }

            int ゴミ箱の数 = ファイル数 switch
            {
                int x when x <= 60 => 1,
                int x when x <= 90 => 2,
                int x when x <= 120 => 3,
                int x when x <= 150 => 4,
                int x when x <= 180 => 5,
                _ => 6,
            };

            LogDeleteButton.IsEnabled = true;
            LogDeleteButton.Content = $"{string.Concat(Enumerable.Repeat(ゴミ箱マーク, ゴミ箱の数))} {BtnText}";
        }

        private void 作業項目削除ボタン状態更新()
        {
            const string BtnText = "古い作業項目削除";
            const int 履歴種類数 = 5;

            if (_combos.Length != 履歴種類数)
            {
                throw new ArgumentException("引数不正");
            }

            ItemHistoryDeleteButton.IsEnabled = false;
            ItemHistoryDeleteButton.Content = BtnText;

            int 最大履歴数 = 0;
            foreach (var combo in _combos)
            {
                if (最大履歴数 < combo.Items.Count)
                {
                    最大履歴数 = combo.Items.Count;
                }
            }

            if (最大履歴数 <= 保持する作業項目履歴数)
            {
                return;
            }

            int ゴミ箱の数 = 最大履歴数 switch
            {
                int x when x <= 100 => 1,
                int x when x <= 150 => 2,
                _ => 3,
            };

            ItemHistoryDeleteButton.IsEnabled = true;
            ItemHistoryDeleteButton.Content = $"{string.Concat(Enumerable.Repeat(ゴミ箱マーク, ゴミ箱の数))} {BtnText}";
        }

        public void 参照モードON()
        {
            MainContainer.IsEnabled = false;
            OkButton.IsEnabled = false;
        }

        private void 設定内容を画面に適用()
        {
            タスクバーアイコンの設定を画面に適用();
            コンボボックスの設定を画面に適用();
            通知設定を画面に適用();

            RestoreComboText.IsChecked = 設定内容.起動時に作業コンボボックスのテキスト設定;
            ParallelSave.IsChecked = 設定内容.並行作業保存;
            ConfirmClear.IsChecked = 設定内容.作業クリアの確認;
            ConfirmStop.IsChecked = 設定内容.作業終了の確認;
            NotifySound.IsChecked = 設定内容.通知音;
        }

        private void タスクバーアイコンの設定を画面に適用()
        {
            switch (設定内容.タスクバーアイコン)
            {
                case 作業中タスクバーアイコン.通常:
                    TbIconNormal.IsChecked = true;
                    break;
                case 作業中タスクバーアイコン.進捗固定:
                    TbIconFixedProgress.IsChecked = true;
                    break;
                case 作業中タスクバーアイコン.アニメーション:
                    TbIconAnimation.IsChecked = true;
                    break;
                default:
                    break;
            }

            switch (設定内容.タスクバー色)
            {
                case 作業中タスクバー色.通常の色:
                    TbIconFixedG.IsChecked = true;
                    break;
                case 作業中タスクバー色.黄色:
                    TbIconFixedY.IsChecked = true;
                    break;
                case 作業中タスクバー色.赤色:
                    TbIconFixedR.IsChecked = true;
                    break;
                default:
                    break;
            }
        }

        private void コンボボックスの設定を画面に適用()
        {
            switch (設定内容.コンボボックス状態)
            {
                case 作業中コンボボックス状態.無効:
                    ComboStateDisabled.IsChecked = true;
                    break;
                case 作業中コンボボックス状態.色付け:
                    ComboStateColor.IsChecked = true;
                    break;
                case 作業中コンボボックス状態.アニメーション:
                    ComboStateAnimation.IsChecked = true;
                    break;
                default:
                    break;
            }
        }

        private void 通知設定を画面に適用()
        {
            for (int i = 0; i < 設定内容.通知.Length; i++)
            {
                var chk = NotifyContainer.FindName($"NotifyEnabled{i + 1}") as CheckBox;
                if (chk is not null)
                {
                    chk.IsChecked = 設定内容.通知[i].通知表示;
                }

                var time = NotifyContainer.FindName($"NotifyTime{i + 1}") as Controls.TimeTextBox;
                if (time is not null)
                {
                    time.SetTime(設定内容.通知[i].通知時刻);
                }

                var txt = NotifyContainer.FindName($"NotifyMsg{i + 1}") as Controls.SimpleTextBox;
                if (txt is not null)
                {
                    txt.Text = 設定内容.通知[i].メッセージ;
                }
            }
        }

        /// <summary>
        /// OKボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (!入力項目確認())
            {
                return;
            }

            設定内容 = 設定内容 with
            {
                タスクバーアイコン = 作業中のタスクバーアイコン画面設定値,
                タスクバー色 = 作業中のタスクバー色画面設定値,
                コンボボックス状態 = 作業中のコンボボックス状態画面設定値,
                起動時に作業コンボボックスのテキスト設定 = RestoreComboText.IsChecked == true,
                並行作業保存 = ParallelSave.IsChecked == true,
                作業クリアの確認 = ConfirmClear.IsChecked == true,
                作業終了の確認 = ConfirmStop.IsChecked == true,
                通知 = new 通知情報[]
                {
                    new(
                        NotifyEnabled1.IsChecked == true,
                        NotifyTime1.GetTime(),
                        NotifyMsg1.Text),
                    new(
                        NotifyEnabled2.IsChecked == true,
                        NotifyTime2.GetTime(),
                        NotifyMsg2.Text),
                    new(
                        NotifyEnabled3.IsChecked == true,
                        NotifyTime3.GetTime(),
                        NotifyMsg3.Text),
                },
                通知音 = NotifySound.IsChecked == true,
            };

            DialogResult = true;
        }

        /// <summary>
        /// 入力チェック
        /// </summary>
        /// <returns></returns>
        private bool 入力項目確認()
        {
            if (!メッセージ入力確認と時刻一覧取得(out List<DateTime> times))
            {
                return false;
            }

            if (times.Count != times.GroupBy(x => x).Count())
            {
                メッセージボックス.エラー("時刻が重複しています。");
                return false;
            }

            return true;
        }

        private bool メッセージ入力確認と時刻一覧取得(out List<DateTime> times)
        {
            times = new();

            for (int i = 0; i < 設定内容.通知.Length; i++)
            {
                if (!メッセージ表示時刻収集(i + 1, times))
                {
                    continue;
                }

                if (NotifyContainer.FindName($"NotifyMsg{i + 1}") is not Controls.SimpleTextBox txt)
                {
                    continue;
                }

                if (string.IsNullOrEmpty(txt.Text))
                {
                    メッセージボックス.エラー("メッセージ内容を入力してください。");
                    return false;
                }
            }

            return true;
        }

        private bool メッセージ表示時刻収集(int no, List<DateTime> times)
        {
            if (NotifyContainer.FindName($"NotifyEnabled{no}") is not CheckBox chk ||
                chk.IsChecked != true)
            {
                // チェックOFF、対象外
                return false;
            }

            if (NotifyContainer.FindName($"NotifyTime{no}") is not Controls.TimeTextBox time)
            {
                return false;
            }

            times.Add(time.GetTime());

            return true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private 作業中タスクバーアイコン 作業中のタスクバーアイコン画面設定値
        {
            get
            {
                if (TbIconFixedProgress.IsChecked == true)
                {
                    return 設定ファイル.GetEnum<作業中タスクバーアイコン>(1);
                }
                else if (TbIconAnimation.IsChecked == true)
                {
                    return 設定ファイル.GetEnum<作業中タスクバーアイコン>(2);
                }
                else
                {
                    return 設定ファイル.GetEnum<作業中タスクバーアイコン>(0);
                }
            }
        }

        private 作業中タスクバー色 作業中のタスクバー色画面設定値
        {
            get
            {
                if (TbIconFixedY.IsChecked == true)
                {
                    return 設定ファイル.GetEnum<作業中タスクバー色>(1);
                }
                else if (TbIconFixedR.IsChecked == true)
                {
                    return 設定ファイル.GetEnum<作業中タスクバー色>(2);
                }
                else
                {
                    return 設定ファイル.GetEnum<作業中タスクバー色>(0);
                }
            }
        }

        private 作業中コンボボックス状態 作業中のコンボボックス状態画面設定値
        {
            get
            {
                if (ComboStateColor.IsChecked == true)
                {
                    return 設定ファイル.GetEnum<作業中コンボボックス状態>(1);
                }
                else if (ComboStateAnimation.IsChecked == true)
                {
                    return 設定ファイル.GetEnum<作業中コンボボックス状態>(2);
                }
                else
                {
                    return 設定ファイル.GetEnum<作業中コンボボックス状態>(0);
                }
            }
        }

        /// <summary>
        /// 「古い作業時間ログ削除」ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LogDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (メッセージボックス.確認($"作業時間ログを直近{保持する作業時間ログファイル数}日分を残して削除しますか？")
                != MessageBoxResult.Yes)
            {
                return;
            }

            var di = new DirectoryInfo($"{_timesFile.格納フォルダパス}");
            if (!di.Exists)
            {
                return;
            }

            try
            {
                // 文字数でのチェックなので他のファイルもヒットする可能性あり
                var files = di.GetFiles("????????.log", SearchOption.TopDirectoryOnly)
                    .OrderByDescending(f => f.Name)
                    .Skip(保持する作業時間ログファイル数);

                foreach (var f in files)
                {
                    f.Delete();
                }
            }
            catch (Exception ex)
            {
                メッセージボックス.エラー(ex.ToString());
            }

            作業時間ログ削除ボタン状態更新();
        }

        /// <summary>
        /// 「古い作業項目削除」ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ItemHistoryDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (メッセージボックス.確認($"作業項目の履歴を直近{保持する作業項目履歴数}件を残して削除しますか？")
                != MessageBoxResult.Yes)
            {
                return;
            }

            try
            {
                for (int i = 0; i < _combos.Length; i++)
                {
                    if (_combos[i].Items.Count <= 保持する作業項目履歴数)
                    {
                        continue;
                    }

                    int maxIdx = _combos[i].Items.Count - 1;
                    for (int j = maxIdx; 保持する作業項目履歴数 - 1 < j; j--)
                    {
                        _combos[i].Items.RemoveAt(j);
                    }

                    作業内容ファイル.一覧ファイル保存(
                        _combos[i].Items,
                        作業内容ファイル.ファイルパス取得(i + 1));
                }
            }
            catch (Exception ex)
            {
                メッセージボックス.エラー(ex.ToString());
            }

            作業項目削除ボタン状態更新();
        }
    }
}
