using System.IO;
using System.Text;
using System.Windows.Controls;
using WorkTimeRec.ユーティリティ;

namespace WorkTimeRec.ファイル
{
    internal class 作業内容ファイル
    {
        public static void 一覧ファイル保存(ItemCollection items, string filePath)
        {
            using var sw = new StreamWriter(filePath, append:false, Encoding.UTF8);

            foreach (var item in items)
            {
                sw.WriteLine(item.ToString());
            }
        }

        public static string ファイルパス取得(int index)
        {
            return index switch
            {
                1 => パス操作.フルパス取得(App.WorkList1FileName),
                2 => パス操作.フルパス取得(App.WorkList2FileName),
                3 => パス操作.フルパス取得(App.WorkList3FileName),
                4 => パス操作.フルパス取得(App.WorkList4FileName),
                5 => パス操作.フルパス取得(App.WorkList5FileName),
                _ => "",
            };
        }

    }
}
