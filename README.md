# WorkTimeRec
作業時間記録ツール

# 動作環境

- Windows
- .NET 6

# インストール

[Download .NET 6.0 (Linux, macOS, and Windows)](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)から
`.NET Desktop Runtime 6.x`の`x64`をダウンロード・インストール。

[Releases](https://github.com/3xKEsGJQsmEQLAfuMv9QikF8i9y7Bf1D6NjguXg/WorkTimeRec/releases)から`WorkTimeRec.zip`をダウンロード。

zipを右クリックし、プロパティを選択、「許可する」（ブロックの解除）のチェックを入れてOKボタンで閉じる。

![プロパティ](https://user-images.githubusercontent.com/99333667/158997528-5b5e8158-f8c1-4416-bae7-45a5e45a139c.png)

zipを展開し、管理者権限不要な任意の場所に配置する。

# バージョンアップ

[Releases](https://github.com/3xKEsGJQsmEQLAfuMv9QikF8i9y7Bf1D6NjguXg/WorkTimeRec/releases)から`WorkTimeRec.zip`をダウンロード。

zipを右クリックし、プロパティを選択、「許可する」（ブロックの解除）のチェックを入れてOKボタンで閉じる。

zipを展開し、展開ファイルを前回配置場所に上書きコピーする。

# アンインストール

展開フォルダごと削除する。

# 使い方

`WorkTimeRec.exe`を起動する。

![image1](https://user-images.githubusercontent.com/99333667/159472405-0b3da0a7-6928-4a20-abd9-34c3e5801cef.png)

左のコンボボックスに作業内容を入力し、右の「開始」ボタンを押す。
そうするとボタンが「作業中」の表示になり、下の一覧に作業開始時刻が追加される。

![image2](https://user-images.githubusercontent.com/99333667/159472504-a85929cf-5c7a-42f1-97f2-75aa104a5a9c.png)

作業が終わったら「作業中」のボタンを押すと、ボタンテキストが「開始」に戻り、下の一覧に作業終了時刻と、かかった時間が設定される。

![image3](https://user-images.githubusercontent.com/99333667/159472611-5268bdad-4788-48d1-a5d5-020ad0bb3d3d.png)

作業Aを作業中に（作業Aを終了させずに）作業Bを開始すると、作業Aの作業中が自動解除され、作業Bが作業中になる（作業Aを終えて作業Bに着手）。

複数の作業時間を同時に計測したい場合は、「並行作業をする」チェックをONにする。
そうすると複数作業中状態にできる。
タイトルバーに表示される数字は作業中の数。

![image4](https://user-images.githubusercontent.com/99333667/159472647-2c8e19d4-2ebb-4b61-b026-ee2a80d1b5fc.png)

作業コンボボックスは上から順に`Ctrl`+`1` ～ `Ctrl`+`5`のショートカットキーでフォーカス移動できる。

開始/作業中ボタンは上から順に`Ctrl`+`Shift`+`1` ～ `Ctrl`+`Shift`+`5`のショートカットキーで押すことができる。
作業コンボボックス上でEnterキーでも可。

設定ボタンは設定画面を表示する。

ログフォルダボタンは作業時間ログが出力されるフォルダをエクスプローラーで開く。

作業クリアボタンは全コンボボックスのテキスト入力部分をクリアする。

作業終了ボタンは作業中状態を一括終了させる。

## 設定画面

![image5](https://user-images.githubusercontent.com/99333667/159472680-d5907bfa-027f-40ad-ad17-3d7ab383bf75.png)

# 出力ファイル

作業内容のコンボボックスに入力したものは、「開始」ボタンを押したタイミングで`WorkTimeRec.exe`と同じ場所に`WorkList*.txt`というファイル名で出力/更新され、
次回アプリ起動時にコンボボックスの選択肢として読み込まれる。

作業時間一覧情報は、アプリ終了時に`WorkTimeRec.exe`と同じ場所に`Record`という名前でフォルダを作成し、
その中に`yyyymmdd.log`のファイル名フォーマットのログに出力される。
ファイル内容はタブ区切り形式で追記。
出力のみで、起動時にログファイルの内容が画面上に読み込まれることはない。
