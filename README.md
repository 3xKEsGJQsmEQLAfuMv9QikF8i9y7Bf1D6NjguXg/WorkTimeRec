- [WorkTimeRec](https://github.com/3xKEsGJQsmEQLAfuMv9QikF8i9y7Bf1D6NjguXg/WorkTimeRec#worktimerec)
- [動作環境](https://github.com/3xKEsGJQsmEQLAfuMv9QikF8i9y7Bf1D6NjguXg/WorkTimeRec#%E5%8B%95%E4%BD%9C%E7%92%B0%E5%A2%83)
- [インストール](https://github.com/3xKEsGJQsmEQLAfuMv9QikF8i9y7Bf1D6NjguXg/WorkTimeRec#%E3%82%A4%E3%83%B3%E3%82%B9%E3%83%88%E3%83%BC%E3%83%AB)
- [バージョンアップ](https://github.com/3xKEsGJQsmEQLAfuMv9QikF8i9y7Bf1D6NjguXg/WorkTimeRec#%E3%83%90%E3%83%BC%E3%82%B8%E3%83%A7%E3%83%B3%E3%82%A2%E3%83%83%E3%83%97)
- [アンインストール](https://github.com/3xKEsGJQsmEQLAfuMv9QikF8i9y7Bf1D6NjguXg/WorkTimeRec#%E3%82%A2%E3%83%B3%E3%82%A4%E3%83%B3%E3%82%B9%E3%83%88%E3%83%BC%E3%83%AB)
- [使い方](https://github.com/3xKEsGJQsmEQLAfuMv9QikF8i9y7Bf1D6NjguXg/WorkTimeRec#%E4%BD%BF%E3%81%84%E6%96%B9)
  - [基本的な使い方](https://github.com/3xKEsGJQsmEQLAfuMv9QikF8i9y7Bf1D6NjguXg/WorkTimeRec#%E5%9F%BA%E6%9C%AC%E7%9A%84%E3%81%AA%E4%BD%BF%E3%81%84%E6%96%B9)
  - [ボタン説明](https://github.com/3xKEsGJQsmEQLAfuMv9QikF8i9y7Bf1D6NjguXg/WorkTimeRec#%E3%83%9C%E3%82%BF%E3%83%B3%E8%AA%AC%E6%98%8E)
  - [作業時間一覧の編集](https://github.com/3xKEsGJQsmEQLAfuMv9QikF8i9y7Bf1D6NjguXg/WorkTimeRec#%E4%BD%9C%E6%A5%AD%E6%99%82%E9%96%93%E4%B8%80%E8%A6%A7%E3%81%AE%E7%B7%A8%E9%9B%86)
  - [設定画面](https://github.com/3xKEsGJQsmEQLAfuMv9QikF8i9y7Bf1D6NjguXg/WorkTimeRec#%E8%A8%AD%E5%AE%9A%E7%94%BB%E9%9D%A2)
  - [作業履歴画面](https://github.com/3xKEsGJQsmEQLAfuMv9QikF8i9y7Bf1D6NjguXg/WorkTimeRec#%E4%BD%9C%E6%A5%AD%E5%B1%A5%E6%AD%B4%E7%94%BB%E9%9D%A2)
- [出力ファイル](https://github.com/3xKEsGJQsmEQLAfuMv9QikF8i9y7Bf1D6NjguXg/WorkTimeRec#%E5%87%BA%E5%8A%9B%E3%83%95%E3%82%A1%E3%82%A4%E3%83%AB)

# 🟦WorkTimeRec

作業時間記録ツール

# 🟦動作環境

- Windows
- .NET 6

# 🟦インストール

[Download .NET 6.0 (Linux, macOS, and Windows)](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)から
`.NET Desktop Runtime 6.x`の`x64`をダウンロード・インストール。

[Releases](https://github.com/3xKEsGJQsmEQLAfuMv9QikF8i9y7Bf1D6NjguXg/WorkTimeRec/releases)から`WorkTimeRec.zip`をダウンロード。

zipを右クリックし、プロパティを選択、「許可する」（ブロックの解除）のチェックを入れてOKボタンで閉じる。

![プロパティ](https://user-images.githubusercontent.com/99333667/158997528-5b5e8158-f8c1-4416-bae7-45a5e45a139c.png)

zipを展開し、管理者権限不要な任意の場所に配置する。

# 🟦バージョンアップ

[Releases](https://github.com/3xKEsGJQsmEQLAfuMv9QikF8i9y7Bf1D6NjguXg/WorkTimeRec/releases)から`WorkTimeRec.zip`をダウンロード。

zipを右クリックし、プロパティを選択、「許可する」（ブロックの解除）のチェックを入れてOKボタンで閉じる。

zipを展開し、展開ファイルを前回配置場所に上書きコピーする。

# 🟦アンインストール

展開フォルダごと削除する。

# 🟦使い方

## 💠基本的な使い方

`WorkTimeRec.exe`を起動する。

![image1](https://user-images.githubusercontent.com/99333667/160762415-74275104-6d74-4551-ad1c-27a0e404c880.png)

左のコンボボックスに作業内容を入力し、右の「開始」ボタンを押す。
そうするとボタンが「作業中」の表示になり、下の一覧に作業開始時刻が追加される。

![image2](https://user-images.githubusercontent.com/99333667/160762472-55a79426-d4a9-4b0f-bb01-d7b3ae816231.png)

作業が終わったら「作業中」のボタンを押すと、ボタンテキストが「開始」に戻り、下の一覧に作業終了時刻と、かかった時間が設定される。

![image3](https://user-images.githubusercontent.com/99333667/160762523-c54fc5aa-e9d7-46be-999a-5957b1d91f16.png)

作業Aを作業中に（作業Aを終了させずに）作業Bを開始すると、作業Aの作業中が自動解除され、作業Bが作業中になる（作業Aを終えて作業Bに着手したとみなされる）。

並行して複数の作業をする場合は、「並行作業をする」チェックをONにする。
そうすると複数作業中状態にできる（チェックOFFの場合は1度に1つしか作業中にできない）。
タイトルバーに表示される数字は作業中の数。

![image4](https://user-images.githubusercontent.com/99333667/160762572-5845a674-d26a-4073-8084-8cf6250ba4da.png)

作業コンボボックスは上から順に`Ctrl`+`1` ～ `Ctrl`+`5`のショートカットキーでフォーカス移動できる。

開始/作業中ボタンは上から順に`Ctrl`+`Shift`+`1` ～ `Ctrl`+`Shift`+`5`のショートカットキーで押すことができる。
作業コンボボックス上でEnterキーでも可。

## 💠ボタン説明

設定ボタンは設定画面を表示する。ショートカットキーは`Ctrl`+`,`。

ログフォルダボタンは作業時間ログが出力されるフォルダをエクスプローラーで開く。ショートカットキーは`Ctrl`+`L`。

作業履歴ボタンは作業コンボボックスの履歴を一覧表示する画面を表示する。ショートカットキーは`Ctrl`+`H`。

作業クリアボタンはすべての作業コンボボックスのテキスト入力部分をクリアする。

作業終了ボタンは作業中状態を一括終了させる。

## 💠作業時間一覧の編集

作業中の項目がないときに作業時間一覧で行を選択し、右クリックでマージ、削除、ログファイルに出力して一覧クリアができる。

**マージ**は2つ以上選択状態にして実行する。同じ作業内容 かつ 終了時間から次の開始時間までの間が2分以内のときに1つにまとめることができる。
複数選択は1つ目の行をクリックし、2つ目以降は`Ctrl`を押しながらクリックする。連続した範囲を一括で選択する場合は、範囲の開始行をクリック後、終了行を`Shift`を押しながらクリックする。

![image7](https://user-images.githubusercontent.com/99333667/160762841-c7a4d551-7230-4fdd-8936-7beebb61896e.png)

![image8](https://user-images.githubusercontent.com/99333667/160762878-59222902-1997-4019-9a8f-9798049e7fcd.png)

**削除**は1つ以上選択状態にして実行する。選択した項目が一覧から削除される。ログに残したくない項目を削除するときに使用する。

**ログに保存して一覧クリア**は、一覧の内容をすべてログに出力し、一覧の項目をすべて削除する。通常、ログへの出力はアプリケーション終了時に行われるが、このコマンドで任意のタイミングでログ出力できる。

## 💠設定画面

![image5](https://user-images.githubusercontent.com/99333667/163673299-ee59f9aa-da52-45b1-bab1-9daeb0dbda8d.png)

### 作業中のタスクバーアイコン

作業中状態のときのタスクバーアイコンの表示の仕方。「進捗固定」はプログレスバーMAX値固定表示。「アニメーション」はプログレスバーアニメーション。

「進捗固定」選択時はタスクバーの進捗色を設定可能。

#### Windows 10

![win10taskbar](https://user-images.githubusercontent.com/99333667/160762997-dfd98fc0-019c-4580-8f9f-66873862968c.png)

#### Windows 11

![win11taskbar](https://user-images.githubusercontent.com/99333667/160763043-0d44110d-82fb-4cde-a2a2-855e8738b723.png)

### 作業中のコンボボックス

作業中状態のときの作業コンボボックスの表示の仕方。「色付け」はコンボボックスの色を変える。「アニメーション」はコンボボックスにプログレスバー表示。

### 指定時刻にメッセージ表示

指定時刻にメッセージを表示する。チェックボックスをONにすると有効化。
⏱ボタンは入力補助。

#### 「時」入力補助
押したボタンの数値がテキストボックスに入力される。
現在時のボタンは背景色が他と変わる。

![image5_2](https://user-images.githubusercontent.com/99333667/163673457-9fe62727-ed6f-4b6a-b102-ec6f4e8516a4.png)

#### 「分」入力補助
押したボタンの数値がテキストボックスに入力される。

![image5_3](https://user-images.githubusercontent.com/99333667/163673391-fb1fe654-53d1-475f-96b9-42db85ddd1d1.png)

#### 表示メッセージ例
![image9](https://user-images.githubusercontent.com/99333667/163673416-ebcf08a4-ff5e-4b1d-8e5e-1560e3ca85e7.png)

### 起動時、作業コンボボックスに直近の作業内容を設定

チェックONにすると起動時にコンボボックス履歴の先頭項目をコンボボックステキストに設定。チェックOFFだと起動時はコンボボックステキストが空。

### 「並行作業をする」のチェック状態を次回起動時も維持

チェックONにすると次回起動時に「並行作業をする」のチェック状態が復元される。チェックOFFにすると、起動時の「並行作業をする」のチェック初期値はOFF。

### 「作業終了」ボタンの実行前に確認

チェックONにすると「作業終了」ボタンを押したときに実行確認をするようになる。チェックOFFは「作業終了」ボタンを押したら即座に終了処理を実行する。

## 💠作業履歴画面

全ての作業コンボボックスの履歴を一覧表示する。

また、一覧の項目をダブルクリックするかEnterキーで、その項目をコンボボックスのテキストエリアに入力する。

![image6](https://user-images.githubusercontent.com/99333667/160763114-b6fecd41-4677-4d09-913c-e28edf6000d7.png)

# 🟦出力ファイル

作業内容のコンボボックスに入力したものは、「開始」ボタンを押したタイミングで`WorkTimeRec.exe`と同じ場所に`WorkList1.txt`～`WorkList5.txt`というファイル名で出力/更新され、
次回アプリケーション起動時にコンボボックスの選択肢として読み込まれる。

作業時間一覧情報は、アプリ終了時に`WorkTimeRec.exe`と同じ場所に`Record`という名前でフォルダを作成し、
その中に`yyyymmdd.log`のファイル名フォーマットのログに追記される。
ファイル内容はタブ区切り形式。
出力のみで、起動時にログファイルの内容が画面上に読み込まれることはない。
