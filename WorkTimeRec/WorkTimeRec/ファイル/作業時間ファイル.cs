using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using WorkTimeRec.データ型;
using WorkTimeRec.ユーティリティ;

namespace WorkTimeRec.ファイル
{
    internal class 作業時間ファイル
    {
        public const string 格納フォルダ = "Record";
        private readonly string _folderPath;

        public string 格納フォルダパス => _folderPath;

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

        /// <summary>
        /// アプリケーション終了時に実施
        /// </summary>
        /// <param name="times"></param>
        public void ファイル保存(IList<作業時間管理> times)
        {
            if (!Directory.Exists(_folderPath))
            {
                Directory.CreateDirectory(_folderPath);
            }

            string fname = $"{時間操作.年月日文字列(DateTime.Now, false)}.log";
            string fpath = Path.Combine(_folderPath, fname);
            bool isFirst = !File.Exists(fpath);

            using var sw = new StreamWriter(fpath, append:true, Encoding.UTF8);
            foreach (var t in times)
            {
                if (t.終了 == DateTime.MinValue)
                {
                    t.終了 = DateTime.Now;
                }

                if (isFirst)
                {
                    isFirst = false;
                    // ヘッダー書き込み
                    sw.WriteLine("開始日\t開始時刻\t終了日\t終了時刻\t作業時間\t作業内容");
                }
                sw.WriteLine($"{時間操作.年月日文字列(t.開始, true)}\t{時間操作.時刻文字列(t.開始, true, true)}\t{時間操作.年月日文字列(t.終了, true)}\t{時間操作.時刻文字列(t.終了, true, true)}\t{時間操作.時間間隔文字列(t.作業時間)}\t{t.作業内容}");
            }
        }
    }
}
