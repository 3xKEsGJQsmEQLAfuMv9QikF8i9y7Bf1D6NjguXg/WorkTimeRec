using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WorkTimeRec.ユーティリティ;

namespace WorkTimeRec.Views
{
    /// <summary>
    /// WorkHistoryWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class WorkHistoryWindow : Window
    {
        public int SelectedNo { get; private set; } = 0;
        public string SelectedText { get; private set; } = "";

        public WorkHistoryWindow()
        {
            InitializeComponent();
        }

        public void SetHistory(ComboBox[] combos)
        {
            const int 履歴種類数 = 5;

            if (combos.Length != 履歴種類数)
            {
                throw new ArgumentException("引数不正");
            }

            for (int i = 1; i <= 履歴種類数; i++)
            {
                if (FindName($"Txt{i}") is TextBox txt)
                {
                    txt.Text = combos[i - 1].Text;
                }
                if (FindName($"List{i}") is ListBox list)
                {
                    list.ItemsSource = combos[i - 1].Items;
                    list.SelectedIndex = 0;
                    list.IsEnabled = combos[i - 1].IsEnabled;
                }
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void ListItem_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is not ListBoxItem item)
            {
                return;
            }

            SelectedText = item.Content.ToString() ?? "";
            if (string.IsNullOrEmpty(SelectedText))
            {
                return;
            }
            var obj = VisualTreeHelper.GetParent(item);
            while (obj is not null and
                not ListBox)
            {
                obj = VisualTreeHelper.GetParent(obj);
            }

            SelectedNo = int.Parse(文字列操作.右端((obj as ListBox)?.Name));
            Hide();
        }

        private void List_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
            {
                return;
            }

            if (sender is not ListBox list)
            {
                return;
            }

            if (list.SelectedIndex == -1)
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
            Hide();
        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            bool visible = (bool)e.NewValue;
            if (visible)
            {
                InitializeValues();
            }
        }

        private void InitializeValues()
        {
            SelectedNo = 0;
            SelectedText = "";
        }
    }
}
