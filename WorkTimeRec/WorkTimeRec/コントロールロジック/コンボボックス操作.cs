using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WorkTimeRec.コントロールロジック
{
    internal class コンボボックス操作
    {
        public static async Task コンボ読み込みAsync(
            ComboBox target,
            string filePath,
            bool selectFirstItem)
        {
            target.Items.Clear();

            if (!File.Exists(filePath))
            {
                return;
            }

            using var sr = new StreamReader(filePath, Encoding.UTF8);
            string? line;
            while ((line = await sr.ReadLineAsync()) != null)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }
                target.Items.Add(line);
            }
            if (target.Items.Count != 0 &&
                selectFirstItem)
            {
                // データありなら最初の要素を選択状態にする
                target.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// コンボボックス選択肢をドロップダウンの一番上に設定。ドロップダウンの中になければ追加。
        /// </summary>
        /// <param name="combo"></param>
        /// <param name="text">追加候補のテキスト</param>
        /// <returns>更新したらtrue</returns>
        public static bool コンボリスト更新(ComboBox combo, string text)
        {
            int i = combo.Items.IndexOf(text);

            if (i == 0)
            {
                // 既に先頭
                return false;
            }

            if (1 <= i)
            {
                // 既に存在する場合は削除
                combo.Items.RemoveAt(i);
            }

            // 先頭に挿入
            combo.Items.Insert(0, text);

            if (string.IsNullOrEmpty(combo.Text))
            {
                // 削除した場合、Textが空になるので再設定
                combo.Text = text;
            }

            return true;
        }

        public static void テキストクリア(IEnumerable<ComboBox> combos)
        {
            foreach (var cmb in combos)
            {
                // 無効状態（作業中）のものはスキップ
                if (cmb.IsEnabled != true)
                {
                    continue;
                }
                cmb.Text = "";
            }
        }

    }
}
