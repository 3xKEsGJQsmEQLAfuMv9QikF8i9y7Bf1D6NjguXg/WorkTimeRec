using System;
using System.Threading;
using System.Windows;

namespace WorkTimeRec
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        private const string _appId = "**WorkTimeRec**A9EA8A7E-AD9E-4D25-B873-98FE1EFF8D3F";
        private static readonly Mutex _mutex = new(false, _appId);
        private static bool _isMutexOwner = false;
        public const string AppName = "作業時間記録";
        public const string WorkList1FileName = "WorkList1.txt";
        public const string WorkList2FileName = "WorkList2.txt";
        public const string WorkList3FileName = "WorkList3.txt";
        public const string WorkList4FileName = "WorkList4.txt";
        public const string WorkList5FileName = "WorkList5.txt";
        public Action? TerminateProc;
        public static new App Current => (App)Application.Current;

        protected override void OnStartup(StartupEventArgs e)
        {
            if (!(_isMutexOwner = _mutex.WaitOne(0, false)))
            {
                // すでに起動中なので終了
                Shutdown();
                return;
            }

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            if (_isMutexOwner)
            {
                _mutex.ReleaseMutex();
            }
            _mutex.Close();
        }

        private void Application_SessionEnding(object sender, SessionEndingCancelEventArgs e)
        {
            TerminateProc?.Invoke();
        }

    }
}
