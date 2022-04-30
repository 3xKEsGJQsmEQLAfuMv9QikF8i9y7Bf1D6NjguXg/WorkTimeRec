using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using WorkTimeRec.データ型;
using WorkTimeRec.ユーティリティ;

namespace WorkTimeRec.ファイル
{
    public class 作業時間ファイル
    {
        public const string 格納フォルダ = "Record";
        private readonly string _folderPath;

        public string 格納フォルダパス => _folderPath;

        public static readonly string ヘッダー = "開始日    \t開始時刻\t終了日    \t終了時刻\t作業時間\t作業内容";
        public static readonly int 列数 = ヘッダー.Split('\t').Length;
        public static readonly int 作業時間インデックス = 4;
        public static readonly int 作業内容インデックス = 5;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="ベースパス"></param>
        /// <exception cref="FileNotFoundException"></exception>
        public 作業時間ファイル(string ベースパス)
        {
            if (!Directory.Exists(ベースパス))
            {
                throw new FileNotFoundException();
            }

            _folderPath = Path.Combine(ベースパス, 格納フォルダ);
        }

        public void ファイル保存(IList<作業時間管理> times)
        {
            if (!Directory.Exists(_folderPath))
            {
                Directory.CreateDirectory(_folderPath);
            }

            StreamWriter? writer = null;
            int 開始日 = 0;
            try
            {
                foreach (var t in times)
                {
                    if (t.開始.Day != 開始日)
                    {
                        ログファイルオープン(t, ref 開始日, ref writer);
                    }

                    if (t.終了 == DateTime.MinValue)
                    {
                        t.終了 = DateTime.Now;
                    }
                    writer?.WriteLine($"{時間操作.年月日文字列(t.開始, true)}\t{時間操作.時刻文字列(t.開始, true, true)}\t{時間操作.年月日文字列(t.終了, true)}\t{時間操作.時刻文字列(t.終了, true, true)}\t{時間操作.時間間隔文字列(t.作業時間, true)}\t{t.作業内容}");
                }
            }
            finally
            {
                writer?.Dispose();
            }

        }

        private void ログファイルオープン(作業時間管理 t, ref int 開始日, ref StreamWriter? writer)
        {
            開始日 = t.開始.Day;
            string fpath = Path.Combine(_folderPath, $"{時間操作.年月日文字列(t.開始, false)}.log");
            writer?.Dispose();

            bool ヘッダ出力が必要 = false;
            if (!File.Exists(fpath))
            {
                ヘッダ出力が必要 = true;
            }

            writer = new StreamWriter(fpath, append: true, Encoding.UTF8);
            if (ヘッダ出力が必要)
            {
                // 新規ファイル作成時はヘッダー書き込み
                writer.WriteLine(ヘッダー);
            }
        }

        public async IAsyncEnumerable<string[]> ファイル読み込みAsync(DateTime date)
        {
            string logFilePath = Path.Combine(格納フォルダパス, $"{時間操作.年月日文字列(date, false)}.log");
            if (!File.Exists(logFilePath))
            {
                yield return Array.Empty<string>();
                yield break;
            }

            using var reader = new StreamReader(logFilePath);
            
            while (!reader.EndOfStream)
            {
                string? s = await reader.ReadLineAsync();
                if (s is null)
                {
                    continue;
                }

                if (s.StartsWith("開始日"))
                {
                    // ヘッダーは読み飛ばす
                    continue;
                }
                yield return s.Split('\t');
            }

        }
    }
}
