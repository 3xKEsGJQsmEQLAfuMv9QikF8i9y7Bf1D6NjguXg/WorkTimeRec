using System.Windows.Input;

namespace WorkTimeRec.キーボード
{
    internal class キー操作
    {
        public static bool Ctrlキー押下 =>
            Keyboard.Modifiers == ModifierKeys.Control;

        public static bool Shiftキー押下 =>
            Keyboard.Modifiers == ModifierKeys.Shift;

        public static bool CtrlとShiftキー押下 =>
            Keyboard.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift);
    }
}
