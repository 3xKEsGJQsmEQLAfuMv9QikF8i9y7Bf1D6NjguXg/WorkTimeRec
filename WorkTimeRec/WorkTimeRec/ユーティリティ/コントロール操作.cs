using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace WorkTimeRec.ユーティリティ
{
    internal class コントロール操作
    {
        public static T? 親取得<T>(DependencyObject obj) where T : DependencyObject
        {
            var parent1 = VisualTreeHelper.GetParent(obj);
            if (parent1 is null)
            {
                return null;
            }
            var parent2 = parent1 as T;

            return parent2 ?? 親取得<T>(parent1);
        }

        public static IEnumerable<T> 子取得<T>(DependencyObject obj) where T : DependencyObject
        {
            int cnt = VisualTreeHelper.GetChildrenCount(obj);
            if (cnt == 0)
            {
                yield break;
            }

            for (int i = 0; i < cnt; i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);
                if (child is T t)
                {
                    yield return t;
                }
            }
        }

    }
}
