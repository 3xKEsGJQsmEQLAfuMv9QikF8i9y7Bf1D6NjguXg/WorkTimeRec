using System;
using System.Windows;
using System.Windows.Input;

namespace WorkTimeRec.Views
{
    /// <summary>
    /// NotifyWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class NotifyWindow : Window
    {
        /// <summary>
        /// 作業終了ボタン選択時の処理
        /// </summary>
        public Action? StopAction { get; set; }

        public NotifyWindow()
        {
            InitializeComponent();
        }

        public string Message
        {
            get { return Msg.Text; }
            set { Msg.Text = value; } 
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            StopAction?.Invoke();
            Close();
        }
    }
}
