using System;

namespace WorkTimeRec.ユーティリティ
{
    internal class 処理制御
    {
        private int _実行回数 = 0;
        private readonly Action _処理;

        public 処理制御(Action 処理)
        {
            _処理 = 処理;
        }

        public void 一回実行()
        {
            if (0 < _実行回数)
            {
                return;
            }

            _処理();
            _実行回数++;
        }

    }
}
