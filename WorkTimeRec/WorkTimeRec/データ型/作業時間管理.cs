using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using WorkTimeRec.ユーティリティ;

namespace WorkTimeRec.データ型
{
    public class 作業時間管理 : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        /// <summary>
        /// 1～5
        /// </summary>
        public int No { get; set; }

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

        private DateTime _開始;
        public DateTime 開始
        {
            get => _開始;
            set
            {
                if (value != _開始)
                {
                    _開始 = value;
                    RaisePropertyChanged();
                }
            }
        }

        private DateTime _終了;
        public DateTime 終了
        {
            get => _終了;
            set
            {
                if (value != _終了)
                {
                    _終了 = value;
                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(作業時間));
                }
            }
        }

        public TimeSpan 作業時間 => 
            時間操作.秒まで(終了) - 時間操作.秒まで(開始);
    }
}
