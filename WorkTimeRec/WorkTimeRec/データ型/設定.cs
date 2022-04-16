using System;
using System.Collections.Generic;

namespace WorkTimeRec.データ型
{
    public record 設定(
        作業中タスクバーアイコン タスクバーアイコン,
        作業中タスクバー色 タスクバー色,
        作業中コンボボックス状態 コンボボックス状態,
        bool 起動時に作業コンボボックスのテキスト設定,
        bool 並行作業保存,
        bool 並行作業,
        bool 作業終了の確認,
        通知情報[] 通知
    );

    public enum 作業中タスクバーアイコン
    {
        通常 = 0,
        進捗固定 = 1,
        アニメーション = 2,
    }

    public enum 作業中タスクバー色
    {
        通常の色 = 0,
        黄色 = 1,
        赤色 = 2,
    }

    public enum 作業中コンボボックス状態
    {
        無効 = 0,
        色付け = 1,
        アニメーション = 2,
    }

    public record 通知情報(
        bool 通知表示,
        DateTime 通知時刻,
        string メッセージ
    );

}