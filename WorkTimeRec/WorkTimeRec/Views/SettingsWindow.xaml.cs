using System;
using System.Collections.Generic;
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

        public SettingsWindow(設定 設定内容)
        {
            InitializeComponent();
            this.設定内容 = 設定内容;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            設定内容を画面に適用();
        }

        public void 参照モードON()
        {
            MainContainer.IsEnabled = false;
            OkButton.IsEnabled = false;
        }

        private void 設定内容を画面に適用()
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

            switch (設定内容.コンボボックス状態)
            {
                case 作業中コンボボックス状態.無効:
                    ComboStateDisabled.IsChecked = true;
                    break;
                case 作業中コンボボックス状態.色付け:
                    ComboStateColor.IsChecked = true;
                    break;
                case 作業中コンボボックス状態.アニメーション:
                    ComboStateAnimation.IsChecked= true;
                    break;
                default:
                    break;
            }

            for (int i = 0; i < 設定内容.通知.Length; i++)
            {
                var chk = NotifyContainer.FindName($"NotifyEnabled{i+1}") as CheckBox;
                if (chk is not null)
                {
                    chk.IsChecked = 設定内容.通知[i].通知表示;
                }

                var time = NotifyContainer.FindName($"NotifyTime{i+1}") as Controls.TimeTextBox;
                if (time is not null)
                {
                    time.SetTime(設定内容.通知[i].通知時刻);
                }

                var txt = NotifyContainer.FindName($"NotifyMsg{i+1}") as Controls.SimpleTextBox;
                if (txt is not null)
                {
                    txt.Text = 設定内容.通知[i].メッセージ;
                }
            }

            RestoreComboText.IsChecked = 設定内容.起動時に作業コンボボックスのテキスト設定;
            ParallelSave.IsChecked = 設定内容.並行作業保存;
            ConfirmStop.IsChecked = 設定内容.作業終了の確認;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (!入力内容OK())
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
                }
            };

            DialogResult = true;
        }

        private bool 入力内容OK()
        {
            bool result = true;
            List<DateTime> times = new();

            for (int i = 0; i < 設定内容.通知.Length; i++)
            {
                if (NotifyContainer.FindName($"NotifyEnabled{i + 1}") is not CheckBox chk ||
                    chk.IsChecked != true)
                {
                    continue;
                }

                if (NotifyContainer.FindName($"NotifyTime{i + 1}") is not Controls.TimeTextBox time)
                {
                    continue;
                }
                times.Add(time.GetTime());

                if (NotifyContainer.FindName($"NotifyMsg{i + 1}") is not Controls.SimpleTextBox txt)
                {
                    continue;
                }

                if (string.IsNullOrEmpty(txt.Text))
                {
                    メッセージボックス.エラー("メッセージ内容を入力してください。");
                    result = false;
                    break;
                }
            }

            if (times.Count != times.GroupBy(x => x).Count())
            {
                メッセージボックス.エラー("時刻が重複しています。");
                result = false;
            }

            return result;
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

    }
}
