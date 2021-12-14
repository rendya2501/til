# Gitメモ

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

[リモートのブランチをcloneする](https://qiita.com/shim0mura/items/85aa7fc762112189bd73)  

`git checkout -b ブランチ名 リモートリポジトリ名`  
→  
`git checkout -b development origin/development`  

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
