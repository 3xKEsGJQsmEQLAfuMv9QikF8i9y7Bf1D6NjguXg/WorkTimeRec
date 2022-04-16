using System;
using System.Configuration;
using WorkTimeRec.データ型;

namespace WorkTimeRec.ファイル
{
    internal class 設定ファイル
    {
        private static readonly string[] _settingNames = 
        {
            "TaskbarIcon",
            "TaskbarColor",
            "WorkingComboState",
            "RestoreComboText",
            "ParallelSave",
            "Parallel",
            "StopConfirm",
            "Notify1",
            "NotifyTime1",
            "NotifyMsg1",
            "Notify2",
            "NotifyTime2",
            "NotifyMsg2",
            "Notify3",
            "NotifyTime3",
            "NotifyMsg3",
        };

        public static 設定 読み込み()
        {
            var cfg = ConfigurationManager.OpenExeConfiguration(
                ConfigurationUserLevel.None);

            設定ファイル項目存在チェック(cfg);

            var cfgData = new 設定(
                タスクバーアイコン: GetEnum<作業中タスクバーアイコン>(GetInt(cfg.AppSettings.Settings["TaskbarIcon"].Value, 0)),
                タスクバー色: GetEnum<作業中タスクバー色>(GetInt(cfg.AppSettings.Settings["TaskbarColor"].Value, 0)),
                コンボボックス状態: GetEnum<作業中コンボボックス状態>(GetInt(cfg.AppSettings.Settings["WorkingComboState"].Value, 0)),
                起動時に作業コンボボックスのテキスト設定: GetBool(cfg.AppSettings.Settings["RestoreComboText"].Value, true),
                並行作業保存: GetBool(cfg.AppSettings.Settings["ParallelSave"].Value, false),
                並行作業: GetBool(cfg.AppSettings.Settings["Parallel"].Value, false),
                作業終了の確認: GetBool(cfg.AppSettings.Settings["StopConfirm"].Value, false),
                通知: new 通知情報[]
                {
                    new(
                        GetBool(cfg.AppSettings.Settings["Notify1"].Value, false),
                        GetDateTime(cfg.AppSettings.Settings["NotifyTime1"].Value, DateTime.MinValue),
                        cfg.AppSettings.Settings["NotifyMsg1"].Value
                    ),
                    new(
                    GetBool(cfg.AppSettings.Settings["Notify2"].Value, false),
                    GetDateTime(cfg.AppSettings.Settings["NotifyTime2"].Value, DateTime.MinValue),
                    cfg.AppSettings.Settings["NotifyMsg2"].Value
                    ),
                    new(
                    GetBool(cfg.AppSettings.Settings["Notify3"].Value, false),
                    GetDateTime(cfg.AppSettings.Settings["NotifyTime3"].Value, DateTime.MinValue),
                    cfg.AppSettings.Settings["NotifyMsg3"].Value
                    )
                }
            );

            return cfgData;
        }

        private static void 設定ファイル項目存在チェック(Configuration? cfg)
        {
            if (cfg is null)
            {
                return;
            }

            string key = "";
            try
            {
                foreach (string k in _settingNames)
                {
                    key = k;
                    _ = cfg.AppSettings.Settings[key].Value;
                }
            }
            catch
            {
                throw new FormatException($"キー「{key}」が見つかりません。");
            }
        }

        public static void 保存(設定 cfgData)
        {
            var cfg = ConfigurationManager.OpenExeConfiguration(
                ConfigurationUserLevel.None);
            
            cfg.AppSettings.Settings["TaskbarIcon"].Value = ((int)cfgData.タスクバーアイコン).ToString();
            cfg.AppSettings.Settings["TaskbarColor"].Value = ((int)cfgData.タスクバー色).ToString();
            cfg.AppSettings.Settings["WorkingComboState"].Value = ((int)cfgData.コンボボックス状態).ToString();
            cfg.AppSettings.Settings["RestoreComboText"].Value = cfgData.起動時に作業コンボボックスのテキスト設定.ToString();
            cfg.AppSettings.Settings["ParallelSave"].Value = cfgData.並行作業保存.ToString();
            cfg.AppSettings.Settings["Parallel"].Value = cfgData.並行作業.ToString();
            cfg.AppSettings.Settings["StopConfirm"].Value = cfgData.作業終了の確認.ToString();

            for (int i = 0; i < cfgData.通知.Length; i++)
            {
                cfg.AppSettings.Settings[$"Notify{i+1}"].Value = cfgData.通知[i].通知表示.ToString();
                cfg.AppSettings.Settings[$"NotifyTime{i+1}"].Value = cfgData.通知[i].通知時刻.ToString("HH:mm");
                cfg.AppSettings.Settings[$"NotifyMsg{i+1}"].Value = cfgData.通知[i].メッセージ.ToString();
            }

            cfg.Save();
        }

        public static int GetInt(string value, int defaultValue)
        {
            return int.TryParse(value, out int v)
                ? v
                : defaultValue;
        }

        public static bool GetBool(string value, bool defaultValue)
        {
            return bool.TryParse(value, out bool b)
                ? b
                : defaultValue;
        }

        public static T GetEnum<T>(int value)
        {
            return (T)Enum.ToObject(typeof(T), value);
        }

        public static DateTime GetDateTime(string value, DateTime defaultValue)
        {
            return DateTime.TryParse(value, out DateTime d)
                ? d
                : defaultValue;

        }

    }
}
