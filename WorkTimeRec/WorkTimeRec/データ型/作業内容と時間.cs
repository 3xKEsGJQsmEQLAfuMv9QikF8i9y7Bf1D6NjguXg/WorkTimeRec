using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace WorkTimeRec.データ型
{
    internal class 作業内容と時間 : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private string _作業内容 = "";
        public string 作業内容
        {
            get => _作業内容;
            set
            {
                if (value != _作業内容)
                {
                    _作業内容 = value;
                    RaisePropertyChanged();
                }
            }
        }

        private TimeSpan _作業時間;
        public TimeSpan 作業時間
        {
            get => _作業時間;
            set
            {
                if (value != _作業時間)
                {
                    _作業時間 = value;
                    RaisePropertyChanged();
                }
            }
        }

        private int _value;
        public int Value
        { 
            get => _value;
            set
            {
                if (value != _value)
                {
                    _value = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="作業内容"></param>
        /// <param name="作業時間"></param>
        public 作業内容と時間(string 作業内容, TimeSpan 作業時間)
        {
            this.作業内容 = 作業内容;
            this.作業時間 = 作業時間;
        }

        public static void 時間割合計算(ObservableCollection<作業内容と時間>[] listArray)
        {
            double max = 0;

            foreach (var item in listArray.SelectMany(
                list => list.Where(item => max < item.作業時間.TotalSeconds)))
            {
                max = item.作業時間.TotalSeconds;
            }

            foreach (var item in listArray.SelectMany(list => list))
            {
                item.Value = (int)Math.Round(
                    (item.作業時間.TotalSeconds * 100 / max),
                    MidpointRounding.AwayFromZero);
            }
        }

    }
}
