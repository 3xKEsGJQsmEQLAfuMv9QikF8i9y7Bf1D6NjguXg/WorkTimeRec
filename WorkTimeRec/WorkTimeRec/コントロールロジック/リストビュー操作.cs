using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace WorkTimeRec.コントロールロジック
{
    internal class リストビュー操作
    {
        public static void 最初のリスト登録時の表示設定(ListView list)
        {
            if (list.Opacity != 0.9)
            {
                list.Opacity = 0.9;
            }
        }

        public static void サイズ変更(object sender, SizeChangedEventArgs e)
        {
            if (sender is not ListView lv)
            {
                return;
            }

            if (lv.View is not GridView v)
            {
                return;
            }

            const double MarginW = 18.0;
            const double MinW = 75.0;

            var w = lv.ActualWidth
                - v.Columns[0].ActualWidth
                - v.Columns[1].ActualWidth
                - v.Columns[2].ActualWidth
                - MarginW;
            if (w < MinW)
            {
                w = MinW;
            }
            v.Columns[3].Width = w;
        }

        public static void 最下行にスクロール(ListView target)
        {
            target.ScrollIntoView(target.Items.GetItemAt(target.Items.Count - 1));
        }

        public static void リスト選択項目削除<T>(
            System.Collections.IList selectedList,
            ObservableCollection<T> editDataList)
        {
            if (selectedList.Count < 1)
            {
                return;
            }

            var removeList = new T[selectedList.Count];
            selectedList.CopyTo(removeList, 0);

            foreach (var item in removeList)
            {
                editDataList.Remove(item);
            }
        }

    }

}
