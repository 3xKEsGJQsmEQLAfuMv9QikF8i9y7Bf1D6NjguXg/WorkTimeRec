using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WorkTimeRec.Views.Controls
{
    /// <summary>
    /// SimpleTextBox.xaml の相互作用ロジック
    /// </summary>
    public partial class SimpleTextBox : UserControl
    {
        public SimpleTextBox()
        {
            InitializeComponent();
        }

        public string Text
        {
            get { return TextContent.Text; }
            set { TextContent.Text = value; }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is not TextBox t)
            {
                return;
            }

            t.SelectAll();
        }

        private void TextBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not TextBox t)
            {
                return;
            }

            if (t.IsFocused)
            {
                return;
            }
            e.Handled = true;
            t.Focus();
        }
    }
}
