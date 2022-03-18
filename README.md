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

展開ファイルを上書きする。

# アンインストール

展開フォルダごと削除する。

# 使い方

`WorkTimeRec.exe`を起動する。

![image1](https://user-images.githubusercontent.com/99333667/158997632-4083170c-63fd-4761-92b0-53e554ee79be.png)

左のコンボボックスに作業内容を入力し、右の「開始」ボタンを押す。
そうするとボタンが「作業中」の表示になり、下の一覧に作業開始時刻が追加される。

![image2](https://user-images.githubusercontent.com/99333667/158997665-982bd343-ba98-4c2a-977f-ba7d833970f0.png)

作業が終わったら「作業中」のボタンを押すと、ボタンテキストが「開始」に戻り、下の一覧に作業終了時刻と、かかった時間が設定される。

![image3](https://user-images.githubusercontent.com/99333667/158997706-49b74269-2496-4985-8f1b-2e34295bbc70.png)

作業Aを作業中に（作業Aを終了させずに）作業Bを開始すると、作業Aの作業中が自動解除され、作業Bが作業中になる（作業Aを終えて作業Bに着手）。

複数の作業時間を同時に計測したい場合は、「並行作業をする」チェックをONにする。
そうすると複数作業中状態にできる。
タイトルバーに表示される数字は作業中の数。

![image4](https://user-images.githubusercontent.com/99333667/158997883-71e3ad0a-5f80-49ba-ab29-5def3b93e781.png)

作業コンボボックスは上から順に`Ctrl`+`1` ～ `Ctrl`+`5`のショートカットキーでフォーカス移動できる。

開始/作業中ボタンは上から順に`Ctrl`+`Shift`+`1` ～ `Ctrl`+`Shift`+`5`のショートカットキーで押すことができる。
作業コンボボックス上からEnterキーでも可。

ログフォルダボタンは作業時間ログが出力されるフォルダをエクスプローラーで開く。

作業クリアボタンは全コンボボックスのテキスト入力部分をクリアする。

作業終了ボタンは作業中状態を一括終了させる。

# 出力ファイル

作業内容のコンボボックスに入力したものは、「開始」ボタンを押したタイミングで`WorkTimeRec.exe`と同じ場所に`WorkList*.txt`というファイル名で出力/更新され、
次回アプリ起動時にコンボボックスの選択肢として読み込まれる。

作業時間一覧情報は、アプリ終了時に`WorkTimeRec.exe`と同じ場所に`Record`という名前でフォルダを作成し、
その中に`yyyymmdd.log`のファイル名フォーマットのログに出力される。
ファイル内容はタブ区切り形式で追記。
出力のみで、起動時にログファイルの内容が画面上に読み込まれることはない。
