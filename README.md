# WorkTimeRec
作業時間記録ツール

# 動作環境

- Windows
- .NET 6

# インストール

`.NET Desktop Runtime 6.x`の`x64`をインストール。
[Download .NET 6.0 (Linux, macOS, and Windows)](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)

[Releases](https://github.com/3xKEsGJQsmEQLAfuMv9QikF8i9y7Bf1D6NjguXg/WorkTimeRec/releases)から`WorkTimeRec.zip`をダウンロード。

zipを右クリックし、「許可する」のチェックを入れてOKボタンで閉じる。

![img01](https://user-images.githubusercontent.com/99333667/158048370-86ace359-76e7-4ad8-b551-ba03e9e58b92.png)

zipを展開し、管理者権限不要な任意の場所に配置する。

# アンインストール

展開フォルダごと削除する。

# 使い方

`WorkTimeRec.exe`を起動する。

![img02](https://user-images.githubusercontent.com/99333667/158048524-f702c2b5-c865-45ed-b3c6-a2a651628c37.png)

左のコンボボックスに作業内容を入力し、右の「開始」ボタンを押す。
そうするとボタンが「作業中」の表示になり、下の一覧に作業開始時刻が追加される。

![img03](https://user-images.githubusercontent.com/99333667/158048537-da9f6af3-b54d-43d0-a621-48a96c05689a.png)

作業が終わったら「作業中」のボタンを押すと、ボタンテキストが「開始」に戻り、下の一覧に作業終了時刻と、かかった時間が設定される。

作業Aを作業中に作業Bを開始すると、作業Aの作業中が解除され、作業Bが作業中になる。

複数の作業時間を同時に計測したい場合は、「並行作業をする」チェックをONにする。
そうすると他の作業を作業中にしても自動で作業中解除はされなくなる。
タイトルバーに表示される数字は作業中の数。

開始/作業中ボタンは上から順に`Ctrl` + `1`～`Ctrl` + `5`のショートカットキーで押すことができる。

# 出力ファイル

作業内容のコンボボックスに入力したものは、「開始」ボタンを押したタイミングで`WorkTimeRec.exe`と同じ場所に`WorkList*.txt`というファイル名で出力/更新され、
次回アプリ起動時にコンボボックスの選択肢として読み込まれる。

作業時間一覧情報は、アプリ終了時に`WorkTimeRec.exe`と同じ場所に`Record`という名前でフォルダを作成し、
その中に`yyyymmdd.log`のファイル名フォーマットのログファイルにタブ区切り形式で追記される。
出力のみで、ログファイルの内容が画面上に読み込まれることはない。
