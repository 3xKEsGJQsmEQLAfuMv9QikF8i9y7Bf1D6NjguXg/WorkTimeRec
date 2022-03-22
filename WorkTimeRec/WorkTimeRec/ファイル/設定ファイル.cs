using System;
using System.Configuration;
using WorkTimeRec.データ型;

namespace WorkTimeRec.ファイル
{
    internal class 設定ファイル
    {
        public static 設定 読み込み()
        {
            var cfg = ConfigurationManager.OpenExeConfiguration(
                ConfigurationUserLevel.None);

            var cfgData = new 設定(
                タスクバーアイコン: GetEnum<作業中タスクバーアイコン>(GetInt(cfg.AppSettings.Settings["TaskbarIcon"].Value, 0)),
                コンボボックス状態: GetEnum<作業中コンボボックス状態>(GetInt(cfg.AppSettings.Settings["WorkingComboState"].Value, 0)),
                起動時に作業コンボボックスのテキスト設定: GetBool(cfg.AppSettings.Settings["RestoreComboText"].Value, true),
                並行作業保存: GetBool(cfg.AppSettings.Settings["ParallelSave"].Value, false),
                並行作業: GetBool(cfg.AppSettings.Settings["Parallel"].Value, false),
                作業終了の確認: GetBool(cfg.AppSettings.Settings["StopConfirm"].Value, false)
                );

            return cfgData;
        }

        public static void 保存(設定 cfgData)
        {
            var cfg = ConfigurationManager.OpenExeConfiguration(
                ConfigurationUserLevel.None);
            
            cfg.AppSettings.Settings["TaskbarIcon"].Value = ((int)cfgData.タスクバーアイコン).ToString();
            cfg.AppSettings.Settings["WorkingComboState"].Value = ((int)cfgData.コンボボックス状態).ToString();
            cfg.AppSettings.Settings["RestoreComboText"].Value = cfgData.起動時に作業コンボボックスのテキスト設定.ToString();
            cfg.AppSettings.Settings["ParallelSave"].Value = cfgData.並行作業保存.ToString();
            cfg.AppSettings.Settings["Parallel"].Value = cfgData.並行作業.ToString();
            cfg.AppSettings.Settings["StopConfirm"].Value = cfgData.作業終了の確認.ToString();

            cfg.Save();
        }

        public static int GetInt(string value, int defalutValue)
        {
            return int.TryParse(value, out int v)
                ? v
                : defalutValue;
        }

        public static bool GetBool(string value, bool defalutValue)
        {
            return bool.TryParse(value, out bool b)
                ? b
                : defalutValue;
        }

        public static T GetEnum<T>(int value)
        {
            return (T)Enum.ToObject(typeof(T), value);
        }

    }
}
