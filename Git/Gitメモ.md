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

VisualStudioでの作業なら右下のブランチ一覧からブランチを右クリックで、名前を変更する項目があるので、それだけでいける。  

---

## gitignoreでディレクトリを除外する方法

[[Git] .gitignoreの仕様詳解](https://qiita.com/anqooqie/items/110957797b3d5280c44f)  

`*/ディレクトリ名`  

例  
以下のようなフォルダ構成があった時にassetsディレクトリ以下を除外したい場合。  

``` txt : フォルダ構成
src
├─assets ← このディレクトリの中身を対象とさせない
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

### 特定のディレクトリを無視する場合は先頭と末尾に/を付ける

``` txt : gitignore
/assets/
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

## Please make sure you have the correct access rights and the repository exists

[【解決方法(画像付き)】急に。git pushしたら「Please make sure you have the correct access rights and the repository exists.」](https://kenjimorita.jp/please-make-sure-you-have-the-correct-access-rights-and-the-repository-exists/)  

[【Git】ローカルからリモートリポジトリに push できない問題を解決するまでの記録！](https://oki2a24.com/2016/01/28/resolve-error-to-push-from-local-to-remote-in-git/)  
いつぞやこれが発生してこのページ見なかったっけか。  

---

## VisualStudioでの対処色々

[元に戻す]は指定したコミットを打ち消すコミットを作る処理。  
[リセット]は指定したコミットより上をなかったことにする処理。  

[リセット_変更を保持(--mixed)]はそこまでに変更したコードを保持するので、reset後、変更履歴が表示される。  
[リセット_変更を削除(--hard)]はそこまでに変更したコードも削除するので、reset後、変更履歴に何も表示されない。  

[変更を削除(--mixed)]に該当するコマンドは`git reset --mixed`  
[変更を削除(--hard)]に該当するコマンドは`git reset --hard`  

間違えたところを右クリックして[リセット]は意味ない。罠。  
[リセット]は右クリックしたところまで戻す処理なので、間違えたところではなく、間違えたところより下のコミットを右クリックして[リセット]すべし。  

gitは別のブランチをマージした場合、そのブランチでやったコミットが全て履歴に表示される。  
大き目の修正を行った別のブランチをマージした場合、直近のコミットを取り消すのではなく、その根本から取り消したほうがよい。  
元に戻す場合、大量のコミットに対して1つ1つしてしないといけないし、打消しコミットの競合の可能性も出てしまう。  
日付とコメントを見て、どこがマージ前だったのかを確認して、そこまでresetすべし。  

VSから右クリックの時とgit bashを使った時で動作が違う？  
なんか明らかに履歴が一致しなかった。  
そうしないと明らかに整合性が取れていない状態でしか戻せない状態になってしまう。  

git 特定のコミット 取り消し  
<https://rurukblog.com/post/git-merge-delete/>  
git 別のブランチ reset  
<https://nanayaku.com/git-delete-reset/>  

<https://qiita.com/rch1223/items/9377446c3d010d91399b>  
<https://qiita.com/aki4000/items/bec93ba631a83b687fb4>  
<https://naomi-homma.hatenablog.com/entry/2020/08/11/170039>  
<https://qiita.com/shuntaro_tamura/items/06281261d893acf049ed>  
<https://blog.shibayu36.org/entry/20100517/1274099459>  
<https://qiita.com/MitsukiYamauchi/items/8229cd55d4cf58b0db89>  
<https://qiita.com/S42100254h/items/db435c98c2fc9d4a68c2>  

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

## 特定のブランチから新しいブランチを作成する方法

大き目の修正をやっているときに、Releaseのバックアップブランチを作ってプッシュだけしたい、でもブランチは変えたくない。  
そういう時の何か、いい方法がないか探したが、VisualStudioでの作業なら[ブランチの管理]から、分岐させたいコミットを右クリックして[新しいブランチ]を選べば実現できた。  
この方法ならブランチが勝手に切り替わることもなかった。  

コマンドでやる場合、checkoutコマンドでしか無理な模様。  
それによってブランチが自動的に切り替わるのかはやっていないので不明。  

[Git で特定のコミットからブランチを切りたい（作成したい）](https://qiita.com/wnoguchi/items/dabe6e05388faf75f00c)  
[【Git】git checkout -b [ローカルブランチ名] origin/[リモートブランチ名]：リモートブランチをローカルに持ってきてチェックアウト](https://qiita.com/megu_ma/items/26799c89f593a2414333)  
[developブランチ以外のブランチから切ってpushするまで](https://zenn.dev/suzuki_yu/articles/a1b8a13bea39c2)  
git 特定のブランチからブランチ  

---

## 集中管理と分散管理の違い

### 分散管理 (Git)

各利用者はリモートリポジトリから手元のコンピュータに「ローカルリポジトリ」（local repository）と呼ばれる複製（クローン）を作成し、これに対して変更を加える。  
変更されたファイルは再びリモート側へ送信され、大本のファイルに変更箇所が統合（マージ）され、他の利用者のローカルリポジトリへも反映される。  

分散の由来はローカルリポジトリという形でリポジトリを複数持つことが出来るため。  

### 集中管理 (SVN,SourceSafe)

Subversion（SVN）のようなシステムではリポジトリはリモートの一か所のみが設置され、各利用者はファイル単位でこれを取り出したり（チェックアウト）、変更点を反映（コミット）させたりする。  
リポジトリ自体がローカルとリモートに分かれているわけではないため、サーバ上のリポジトリを「リモートリポジトリ」とは呼ばないのが一般的である。  

[Git初心者が最初から学ぶGitの入門（リモートリポジトリ）](https://ottan.xyz/posts/2019/04/git-basis-beginner-remote-repository/)  
1つのリポジトリに対して複数人が作業を実施します。  
ユーザーは、同一のファイルに対してあらかじめ編集の競合が起きないように、「チェックアウト」（ロック）を行います。  
また、マスターとなるリポジトリが決まっているため、そのリポジトリにアクセスできないユーザーは作業を実施できません。  
競合しないことが前提となっているためシンプルである分、複数人による同時作業には向きません。  

---

## リポジトリ

[ローカルリポジトリとリモートリポジトリについて〜SourceTreeでGitを使う](https://itstudio.co/2016/07/22/6014/)  

ファイルやディレクトリの履歴を管理する場所のこと。  
※リポジトリ : 貯蔵庫  

### リモートリポジトリ

専用のサーバに配置して複数人で共有するためのリポジトリです。

### ローカルリポジトリ

ユーザーがローカルマシン上で作業するために利用するリポジトリです。  
基本的にそのユーザーはこの領域にて変更履歴を管理しつつ、作業を行います。  

### originとは？

[Gitでよく使う「origin」って何？わかりやすく説明します](https://reasonable-code.com/git-origin/)  

リモートリポジトリのアクセス先に対してGitがデフォルトでつける名前です。  
リモートリポジトリのアクセス先（例. <https://github.com/donchan922/rails-board.git>）がoriginという名前で設定されていることがわかります。  

→  
つまり、長ったらしいURLに対するデフォルトのエイリアスがoriginという名のリモート名ということ。  

[master以外のブランチにgit pullしたい](https://teratail.com/questions/325508)  
>originっていうのはローカルで任意に名前をつけたリモートリポジトリのURLです。  

ただのエイリアスなので、URLで直接指定してプッシュすることも可能  
`git push https://github.com/donchan922/rails-board.git master`

### リモートリポジトリってたいていoriginだけど、origin以外にもリポジトリって作れるの？  

ローカルでGitプロジェクトを作成するときに、git initしますよね。  
そのあとに以下のようなコマンドでリモートリポジトリの設定を行っているはずです。  

`$ git remote add origin https://github.com/donchan922/rails-board.git`  

これに対してさらにこのようなコマンドを実行した場合  

`$ git remote add hogehoge https://github.com/donchan922/rails-board.git`  

git push hogehoge HEAD とやるだけで同じリモート先のサーバーに対してプッシュされるというわけ。  

teelaはhotfixとdevelopってどうやっていたか。  
同じリポジトリの中でブランチを分けていたか、そもそもリポジトリが別だったか。  
次回出社時に確認してみる。  

``` txt
origin  http://192.168.150.42/osp-dev/hotfix-alp.git (fetch)
origin  http://192.168.150.42/osp-dev/hotfix-alp.git (push)
pgm     http://192.168.150.21/pgm-osp/pgmweb.git (fetch)
pgm     http://192.168.150.21/pgm-osp/pgmweb.git (push)
pgmweb  http://192.168.150.21/PGM-OSP/pgmweb.git (fetch)
pgmweb  http://192.168.150.21/PGM-OSP/pgmweb.git (push)
```

ALP側開発用リポジトリとTeela側リポジトリで分かれていた。  
ローカルリポジトリでoriginが使われている場合、新規でリポジトリを追加する場合はorigin以外を指定しないといけない。  
1つのローカル環境で複数のリポジトリを管理しないといけない場合、必然的にorigin以外の名前をつけることになるだろう。  
