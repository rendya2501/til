# Gitメモ

[Git 基本の用語集](https://qiita.com/toshi_um/items/72c9d929a600323b2e77)  
[Git で「追跡ブランチ」って言うのやめましょう](https://qiita.com/uasi/items/69368c17c79e99aaddbf)

---

## pushまでの一連のコマンド

``` bash
git add -A
git commit -a -m "コメント"
git push
```

…or create a new repository on the command line

``` bash
echo "# SignalRSample" >> README.md
git init
git add README.md
git commit -m "first commit"
git branch -M main
git remote add origin https://github.com/rendya2501/SignalRSample.git
git push -u origin main
```

…or push an existing repository from the command line

``` bash
git remote add origin https://github.com/rendya2501/SignalRSample.git
git branch -M main
git push -u origin main
```

---

## git pull

git push pgmweb fix-366
git pull pgmweb fix-366

git pull [リポジトリ] [ブランチ]
         [リモート]   [ローカル]

425をやっているうちにbug-fix-develop-alpに適応された変更を425にマージしておきたい。
そのためのコマンド
git pull origin bug-fix-develop-alp

---

## merge

gitのマージはマージしたいブランチに行ってから、マージ元を指定するしかないみたい。  
てっきり、マージ元とマージ先を指定できるのかと思ってた。  
TeelaだとPushしてそのままだし、ブランチのマージはGitLab上でやるし、その後、Pullして勝手に修正が反映されるだけだから触れる機会が全くなかった。  

``` git
git checkout マージ先ブランチ名
git merge マージ元ブランチ名
```

---

## Notepad++からGitBashを起動する方法

1.gitを開いたときデフォルトの作業ディレクトリを開くようにする  
<https://www.granfairs.com/blog/staff/gitbash-setting-shortcut>  
git-bash.exeの本体からショートカットを作成する。  
そのショートカットの作業フォルダーの項目に目的の作業ディレクトリのパスを設定する。  

2.そのショートカットをNotepad++に登録する  
メニューバーから「実行」を選択する。  
「ファイル名を指定して実行」を選択して、①の作業で作成したショートカットを指定して「実行」して問題がなかったら「登録」する。  
任意のショートカットキーを指定する。  

---

## リモートブランチを取得する

参考にできるサイトは数多くあるけど、とりあえずここだけでいいかな。  
[【Git】リモートからの取得とリモートへの反映で行っていること(fetch,pull,push)](https://qiita.com/forest1/items/db5ac003d310449743ca)  
自分がよく遭遇するパターンとしては、リモートにあってローカルにないブランチの取得なのでそのパターンをメモする。  

手順  
1.`git fetch`→リモートの情報を取得する。  
2.`git checkout <ブランチ名>`→ローカルブランチにマージする。  

**fetchコマンドでリモートの情報を取得する**  
fetchコマンドは、前回取得して以降に更新された、全ブランチの全コミットデータを取ってきます。  
「リモート追跡ブランチ」のみに反映するため、ローカルブランチは変更しない。  

**checkoutコマンドでローカルブランチを作成する**  
ブランチ名から origin/ を外した、checkoutコマンドを実行する事でローカルブランチが作成される。  
この時、対称のローカルブランチとリモート追跡ブランチが紐づけられる。  

こういうやり方もあるみたい。  
[【Git】リモートブランチをチェックアウトしたいときは「git fetch origin <ブランチ名>」と「git checkout <ブランチ名>」を実行すれば良い](https://dev.classmethod.jp/articles/how-to-checkout-remote-branch/)  

以下は別としてまとめる必要がある。  

checkoutはリモート追跡ブランチからローカルブランチを作成するコマンド。  
またはローカルブランチの作業領域を切り替えるコマンド。  

リモート追跡ブランチ  
あくまでも「リモートを"追跡する"ブランチ」  

pullコマンド  
fetch + mergeの動作を行います。  
fetchで全ブランチの全コミットデータを取得してから、checkout中のカレントブランチに、対応するリモート追跡ブランチをマージします。  
これはまた別にまとめる必要があるかも。  

---

## コミットメッセージなしでコミットする方法

``` git
git commit -a --allow-empty-message -m ''
```

[参考サイト](https://okamerin.com/nc/title/509.htm)

---

## カレントブランチをpushする方法

``` git
git push origin HEAD
```

[参考サイト](https://qiita.com/mabots/items/76d48aa33720287253bf)

---

## 特定のブランチをpushする方法

[gitで特定のブランチだけpushするには](https://kekaku.addisteria.com/wp/20180426204009)  

コミットまで済ませたが未プッシュ。  
別の修正のためブランチを切替。  
プッシュが必要になったがブランチを切り替えるのが面倒くさい。  
特定のブランチだけpushできないだろうか、ということで調べた。  

``` git : 基本形
git push レポジトリ名 ローカルブランチ名:リモートブランチ名 
```

### リモートブランチ名を省略したらローカルブランチと同じ名前のブランチを指定したことになる

``` git
git push レポジトリ名 ローカルブランチ名 
```

「:リモートブランチ名」を省略すると、ローカルブランチ名と同じブランチが指定されます。  

`git push origin develop`  

とすると、  

`git push origin develop:develop`  

と同じ意味になります。  

``` git : 実際に入力してうまくいったコマンド
git push origin Fix/2239
```

### ローカルブランチ名の省略は、リモートブランチの削除の意味になるので注意

逆にローカルブランチの方を省略すると、リモートブランチを削除してしまうので注意して下さい。

`git push レポジトリ名 :リモートブランチ名`

とすると、リモートブランチを削除します。

これは、

`git push レポジトリ名 {空}:リモートブランチ名`

のように空っぽをリモートブランチに反映するという意味かもしれませんね。

---

## リモートの特定ブランチをプルする方法

そういう質問をされたのでまとめる。  
リポジトリの中にブランチがあるので、ブランチをプルという表現自体がおかしい。  

実際に特定のブランチを拝見したかったら、そのリポジトリをクローンしたのち、VSCodeで開いて左下のブランチを切り替えるくらいしか方法がないと思われるが、  
もしかしたら本当に実現できるのかも？  
というか、リポジトリとかブランチの関係を完全に把握しているわけではないので、なんとも言えないのが正直なところ。  
はぁ。勉強したいなぁ。  

pullではなくcloneなら普通にありました。  
[リモートのブランチをcloneする](https://qiita.com/shim0mura/items/85aa7fc762112189bd73)  

`git checkout -b ブランチ名 リモートリポジトリ名`  
→  
`git checkout -b development origin/development`  

2022/05/20 追記  

普通にありました。  

[リモートから特定のブランチを指定してcloneする](https://qiita.com/icoxfog417/items/5776e0f0f758f0f0e48a)  

リモートから特定のブランチを指定してcloneする方法

`git clone -b ブランチ名 https://リポジトリのアドレス`

---

## ローカルのブランチ名を変更する方法

[gitのローカルのブランチ名を変更したい](https://qiita.com/suin/items/96c110b218d919168d64)  

>ローカルのブランチ名を変更する方法です。Pull Requestのissues番号を間違っていたときなどに活躍します。  
>
>`git branch -m <古いブランチ名> <新しいブランチ名>`  
>今開いているブランチをリネームする場合は、単純に新しいブランチ名を指定するだけです。  
>
>`git branch -m <新しいブランチ名>`  

---

## gitignoreでディレクトリを除外する方法

[[Git] .gitignoreの仕様詳解](https://qiita.com/anqooqie/items/110957797b3d5280c44f)  

`*/ディレクトリ名`  

例  
以下のようなフォルダ構成があった時にassetsディレクトリ以下を除外したい場合。  

``` txt : フォルダ構成
src
├─assets
├─components
├─constants
├─router
├─scripts
│  ├─api
│  ├─services
│  └─storage
├─types
│  └─api
└─views
```

``` txt : gitignore
.DS_Store
*/assets
```

---

## なぜGitのブランチを分けるのか

master,relase,developそれぞれの役割とは？  
なんとなくやってただけなのでまとめる。  

とりあえず、git-flowと呼ばれるルールに従った場合のブランチ名らしい。  
master,develop,feature,release,hotfixが主たるブランチ。  
今の開発環境と随分違うところを見れば、基本から逸脱してるのがわかるね。  

[【Git】git-flowを知ろう！　利用時のルールについて](https://cloudsmith.co.jp/blog/efficient/2020/08/1534208.html)  
[git-flowで用いるブランチまとめ](https://qiita.com/hatt0519/items/23ef0866f4abacce7296)  
更に知りたかったら「git flow」で調べればごまんと出てくる。  

### git-flow

Gitにおけるリポジトリの分岐モデル。ルールの事を指す。  
それぞれのブランチを明確に定義し、複数人での開発時にそれぞれが好き勝手にブランチを作成し混乱することを防ぐ。  

- メインブランチ  
  開発のコアとなるブランチ。  
  master,develop  
- サポートブランチ  
  機能の追跡、製品リリースの準備、製品に起きた問題をすばやく修正すること、などを容易にするためのブランチ。  
  feature,hotfix,release  

### master

プロダクトとしてリリースする用のブランチ。  
リリースしたらタグ付けする。  
製品として出荷可能な状態であり、アプリケーションが安定して動く状態にする必要がある。  
※このブランチ上での作業は行わない  

### develop

開発用ブランチ。  
コードが安定し、リリース準備ができたらreleaseへマージする。  
次のリリースのための最新の開発作業の変更が反映されている状態。このブランチが常に最新。  
※このブランチ上での作業は行わない

### feature

分岐元：develop  
マージ先：develop  

developからブランチを切り、新機能の開発を行うのに用いる。  
ブランチ名はfeature/news_feedなど、実装する機能をfeature/の後ろに書くなどすればいい。  
最終的にはdevelopにmergeする。この時、--no-ffオプションを付けてmergeすると、その機能実装に使ったコミットがまとまり、差分の管理が楽になる。  

### hotfix

分岐元： master  
マージ先： develop と master  

リリース後の緊急対応（クリティカルなバグフィックスなど）用。  
masterから分岐し、masterにマージすると共にdevelopにマージする。  

### release

分岐元： develop  
マージ先： develop と master  

プロダクトリリースの準備用。品質保証(QA)用ブランチ。  
release/1.0といった感じで切ればいい。  
リリース予定の機能やバグフィックスが反映された状態のdevelopから分岐する。  
この間に新機能に不具合が見つかった場合は、work/fix_bugなど修正用のブランチをここから切り、修正が終わったらこのブランチにmergeする。  
デバッグが終わるまでdevelopにmergeすることは許されない。  
リリース準備が整ったら、masterにマージすると共にdevelopにマージする。  

### git-flowのメリット

- 本番リリースのリポジトリと開発中、修正中のリポジトリを明確に区別することで本番に紛れ込むことを防ぐ  
- 開発、修正毎にリポジトリを作成でき、リリースタイミングに関わらず並行して作業が可能  
- リリース後の緊急対応が開発、修正と独立して対応可能（昔、たまに緊急対応リリース時に開発中の別機能が紛れ込む怪現象が・・・）  
- 履歴によりリリース内容を後から追跡可能  

---

## error: failed to push some refs to "URL"のエラー対処法

[【git】error: failed to push some refs to "URL"のエラー対処法](https://qiita.com/chiaki-kjwr/items/118a5b3237c78d720582)

>いくつか対処法があるみたいです。
>
>①git pull origin develop
>git pull origin developでリモートの環境をローカルファイルにpullした後、
>再度pushを行う。
>
>②git fetchした後、git mergeする
>①とやっていることはほとんど変わらず、pull＝fetch + mergeという意味合いなのかと思います。
>
>③git push ––forceで矯正的にpushする
>こちらは、個人開発なら自分一人しかリポジトリを操作しないので大きな影響はなさそうですが、
>チーム開発の場合だと自分以外にcommitやpushする人がいる無闇に使用すべきでは無い、という記事をいくつか確認しました。
>
>①か②で対処するのが無難かもしれません・・・（私は①で対処しました）

2022/03/17 Thuに発生。  
push出来なくなってしまったので、エラー文で検索したら色々ヒットした。  

git fetch,git mergeを実行したらいけたので備忘録として残しておく。  

---

## VisualStudioでの対処色々

後でちゃんとまとめる。  

右クリック、リセット、変更を削除(--hard)に該当するコマンドは`git reset --hard`の模様。  

git reset --hard HEAD  
編集・ステージングいずれの変更内容を取り消し、最後にコミットした状態に戻す  

--hardはヘッドの位置をずらすという認識っぽい。

大き目の別のブランチをマージした場合、直近のコミットを取り消すのではなく、その根本から取り消す必要がある模様。  
これが分からなくて、明らかに整合性が取れていない状態でしか戻せないのに納得が行かなくて午前をつぶしてしまった。

基本的に、ある特定のコミットだけを取り消すことはできないらしい。
と、思ったが調べればごまんとでてくるぞ。


revert は指定したコミットを打ち消す。

reset はHEADを移動させる。


resetはノードのnextを書き換える感じかな。
revertは履歴は残るので、指定したコミットはそのままでそれを打ち消すコミットを作るだけ。

VSから右クリックの時とgit bashを使った時で動作が違う？
なんか明らかに履歴が一致しなかった。
というか、VSからgit bashを起動させることはできないのだろうか。


revertは個別に巻き戻し可能。
履歴に残る。
ただし、場合によっては巻き戻しの時に競合が発生する可能性あり。
小規模な対応にはいいかも。
別のブランチをごっそり差し戻す場合は、量が多すぎて巻き戻すときの競合リスクや大量の履歴が発生してしまうのでお勧めしない。

resetは個別の巻き戻しは不可能。
ヘッドを移動させる処理に近いので、上から何番目までヘッドを戻すという形になるので、個別は無理。
履歴に残らない。
小規模でも大規模でも行ける。
ただ、ヘッドを戻すので、他人がそのブランチを開いた時に未プッシュがある状態となって、何も知らずにプッシュされると整合性が取れなくなる必要があるので、コミュニケーションが必要。

revertにコミットIDを指定した場合、そのコミットに対する巻き戻しだけを行う。
resetにコミットIDを指定した場合、それより前のコミット全てなかったことにする。


git 特定のコミット 取り消し  
<https://rurukblog.com/post/git-merge-delete/>  
git 別のブランチ reset
https://nanayaku.com/git-delete-reset/
git revert

https://qiita.com/rch1223/items/9377446c3d010d91399b
https://qiita.com/aki4000/items/bec93ba631a83b687fb4
https://naomi-homma.hatenablog.com/entry/2020/08/11/170039
https://qiita.com/shuntaro_tamura/items/06281261d893acf049ed
https://blog.shibayu36.org/entry/20100517/1274099459
https://qiita.com/MitsukiYamauchi/items/8229cd55d4cf58b0db89
https://qiita.com/S42100254h/items/db435c98c2fc9d4a68c2

---

## VisualStudioのターミナルにGit Bashを追加する方法

[git-bashをVisual Studio 2019 のTerminalに追加する手順](https://qiita.com/murasuke/items/45bce6bf40f0d701d595)  

メニュー「表示」→ターミナル→歯車マーク→追加→下記の設定を行う→適応

``` txt
名前         :: git-bash
シェルの場所 :: C:\Program Files\Git\bin\sh.exe
引数         :: -l -i

※要: シェルの場所はボタンでダイアログを開いて選択すべし
```

---

## 現在のブランチ以外から新しいブランチを作成する方法

[developブランチ以外のブランチから切ってpushするまで](https://zenn.dev/suzuki_yu/articles/a1b8a13bea39c2)  

大き目の修正をやっているときに、ブランチを切り替えずに新しいブランチを作成する方法。  
バックアップブランチを作成しないといけない時とか  
