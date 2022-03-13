using System.Windows.Input;

namespace WorkTimeRec.キーボード
{
    internal class キー操作
    {
        public static bool Ctrlキー押下()
        {
            return Keyboard.Modifiers == ModifierKeys.Control;
        }
    }
}
