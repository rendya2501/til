# Gitメモ

[Git 基本の用語集](https://qiita.com/toshi_um/items/72c9d929a600323b2e77)  

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

---

## gitのローカルのブランチ名を変更したい

[gitのローカルのブランチ名を変更したい](https://qiita.com/suin/items/96c110b218d919168d64)  

>ローカルのブランチ名を変更する方法です。Pull Requestのissues番号を間違っていたときなどに活躍します。  
>
>`git branch -m <古いブランチ名> <新しいブランチ名>`  
>今開いているブランチをリネームする場合は、単純に新しいブランチ名を指定するだけです。  
>
>`git branch -m <新しいブランチ名>`  

---

## gitignoreでディレクトリを除外する

``` txt
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

assetsを除外したい場合。  

``` .gitignore
.DS_Store
*/assets
```

[[Git] .gitignoreの仕様詳解](https://qiita.com/anqooqie/items/110957797b3d5280c44f)  

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
