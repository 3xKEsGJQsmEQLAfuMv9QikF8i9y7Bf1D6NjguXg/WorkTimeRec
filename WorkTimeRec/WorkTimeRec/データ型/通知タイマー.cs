using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace WorkTimeRec.データ型
{
    internal class 通知タイマー : IDisposable
    {
        private readonly DispatcherTimer _timer;
        private bool _disposed = false;
        private DateTime _通知した時刻 = DateTime.MinValue;
        private 通知情報[]? _通知情報一覧;
        private readonly Action<string> _tickAction;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="tickAction"></param>
        public 通知タイマー(Action<string> tickAction)
        {
            _tickAction = tickAction;
            _timer = new();
            _timer.Interval = TimeSpan.FromMilliseconds(500);
            _timer.Tick += Timer_Tick; 
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (_通知情報一覧 is null ||
                _通知情報一覧.Any(x => x.通知表示) == false)
            {
                // メッセージ表示すべてOFF
                _timer.Stop();
                return;
            }

            var now = DateTime.Now;
            if (_通知した時刻.Hour == now.Hour &&
                _通知した時刻.Minute == now.Minute)
            {
                // 通知済み
                return;
            }

            foreach (var item in _通知情報一覧.Where(x => x.通知表示))
            {
                if (item.通知時刻.Hour == now.Hour &&
                    item.通知時刻.Minute == now.Minute)
                {
                    _tickAction(item.メッセージ);
                    _通知した時刻 = now;
                    break;
                }
            }
        }

        public void 通知情報設定(通知情報[] value)
        {
            _通知情報一覧 = value;

            if (_通知情報一覧.Any(x => x.通知表示))
            {
                _timer.Start();
            }
        }

        public void 停止()
        {
            _timer.Stop();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                return;
            }

            if (disposing)
            {
                _timer.Stop();
                _timer.Tick -= Timer_Tick;
            }

            _disposed = true;
        }
    }
}
