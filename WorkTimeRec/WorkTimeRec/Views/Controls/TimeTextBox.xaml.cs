using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WorkTimeRec.キーボード;
using WorkTimeRec.ユーティリティ;

namespace WorkTimeRec.Views.Controls
{
    /// <summary>
    /// TimeTextBox.xaml の相互作用ロジック
    /// </summary>
    public partial class TimeTextBox : UserControl
    {
        private static readonly SolidColorBrush _normalBorderBrush = new(Color.FromArgb(0xFF, 0xAB, 0xAD, 0xB3));
        private static readonly SolidColorBrush _mouseOverBorderBrush = new(Color.FromArgb(0xFF, 0x7E, 0xB4, 0xEA));
        private static readonly SolidColorBrush _focusBorderBrush = new(Color.FromArgb(0xFF, 0x56, 0x9D, 0xE5));
        private static readonly Regex _numPattern = new("^[0-9]+$");

        public TimeTextBox()
        {
            InitializeComponent();
        }

        public DateTime GetTime()
        {
            return DateTime.Parse($"{HText.Text}:{MText.Text}");
        }

        public void SetTime(DateTime value)
        {
            HText.Text = value.Hour.ToString().PadLeft(2, '0');
            MText.Text = value.Minute.ToString().PadLeft(2, '0');
        }

        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is not Border b)
            {
                return;
            }
            if (b.BorderBrush == _focusBorderBrush)
            {
                return;
            }

            b.BorderBrush = _mouseOverBorderBrush;
        }

        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is not Border b)
            {
                return;
            }
            if (b.BorderBrush == _focusBorderBrush)
            {
                return;
            }

            b.BorderBrush = _normalBorderBrush;
        }

        private void Text_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is not TextBox t)
            {
                return;
            }

            if (t == HText)
            {
                MPopup.IsOpen = false;
            }
            else if (t == MText)
            {
                HPopup.IsOpen = false;
            }

            t.BorderBrush = _focusBorderBrush;
            t.SelectAll();
        }

        private void Text_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
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

        private void Text_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is not TextBox t)
            {
                return;
            }

            t.BorderBrush = _normalBorderBrush;

            if (t == HText)
            {
                if (!時間操作.時として正しい(t.Text))
                {
                    t.Text = "00";
                    return;
                }
            }
            else if (t == MText)
            {
                if (!時間操作.分として正しい(t.Text))
                {
                    t.Text = "00";
                    return;
                }
            }

            t.Text = t.Text.Trim().PadLeft(2, '0');
        }

        private void HText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape &&
                HPopup.IsOpen)
            {
                e.Handled = true;
                HPopup.IsOpen = false;
            }
        }

        private void MText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape &&
                MPopup.IsOpen)
            {
                e.Handled = true;
                MPopup.IsOpen = false;
            }
        }

        private void HPopupButton_Click(object sender, RoutedEventArgs e)
        {
            HPopup.IsOpen = true;
        }

        private void MPopupButton_Click(object sender, RoutedEventArgs e)
        {
            MPopup.IsOpen = true;
        }

        private void HPopup_Closed(object sender, System.EventArgs e)
        {
            HText.Focus();
        }

        private void MPopup_Closed(object sender, System.EventArgs e)
        {
            MText.Focus();
        }

        private void Text_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !_numPattern.IsMatch(e.Text);
        }

        private void Text_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Paste)
            {
                e.Handled = true;
            }
        }

        private void Text_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                // 選択状態からの空白入力抑止
                e.Handled = true;
            }
        }

        private void HPopup_Opened(object sender, System.EventArgs e)
        {
            H1Button.Focus();

            string h = DateTime.Now.Hour.ToString();
            foreach (var btn in GetHButtons(HButtonContainer))
            {
                if (btn.Content.ToString() == h)
                {
                    btn.Tag = "1";
                }
                else
                {
                    btn.Tag = null;
                }
            }
        }

        private IEnumerable<Button> GetHButtons(DependencyObject obj)
        {
            var children = Enumerable.Range(0, VisualTreeHelper.GetChildrenCount(obj))
                .Select(idx => VisualTreeHelper.GetChild(obj, idx))
                .Where(x => x is not null);

            foreach (var child in children)
            {
                if (child is StackPanel pnl)
                {
                    var children2 = Enumerable.Range(0, VisualTreeHelper.GetChildrenCount(pnl))
                        .Select(idx => VisualTreeHelper.GetChild(pnl, idx))
                        .Where(x => x is not null);
                    foreach (var obj2 in children2)
                    {
                        if (obj2 is Button btn)
                        {
                            yield return btn;
                        }
                    }
                }
            }
        }

        private void MPopup_Opened(object sender, System.EventArgs e)
        {
            M0Button.Focus();
        }

        private void Popup_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                HPopup.IsOpen = false;
                MPopup.IsOpen = false;
                e.Handled = true;
            }
        }

        private void HButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn)
            {
                return;
            }

            HText.Text = btn?.Content?.ToString()?.PadLeft(2, '0');
            HPopup.IsOpen = false;
        }

        private void MButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn)
            {
                return;
            }

            MText.Text = btn?.Content?.ToString()?.PadLeft(2, '0');
            MPopup.IsOpen = false;
        }

    }
}
