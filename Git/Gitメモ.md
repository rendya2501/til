# Gitメモ

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

実際に特定のブランチを拝見したかったら、そのリポジトリをクローンしたのち、VSCodeで開いて左下のブランチを切り替えるくらいしか方法がないと思われるが、もしかしたら本当に実現できるのかも？  
というか、リポジトリとかブランチの関係を完全に把握しているわけではないので、なんとも言えないのが正直なところ。  

pullではなくcloneなら普通にありました。  
[リモートのブランチをcloneする](https://qiita.com/shim0mura/items/85aa7fc762112189bd73)  

`git checkout -b ブランチ名 リモートリポジトリ名`  
→  
`git checkout -b development origin/development`  

2022/05/20 追記  
普通にありました。  

`git clone -b ブランチ名 https://リポジトリのアドレス`  

[リモートから特定のブランチを指定してcloneする](https://qiita.com/icoxfog417/items/5776e0f0f758f0f0e48a)  

---

## リモートブランチを削除する

[Gitのリモートブランチを削除するまとめ](https://qiita.com/yuu_ta/items/519ea47ac2c1ded032d9)  

git push --delete origin Fix/2345

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

## index.lockファイル

index.lockファイルがあるとpullやcommitができなくなる模様。  
ある日突然そうなって、エラーログを見たらこのファイルがうんたらって言ってて、思いっきりlockってあるので消したらpullできるようになりました。  
そもそもこいつ何ぞやっていう。  

>git/index.lockとは  
>同じgitのリポジトリ内で同時に複数の処理を行わないための排他制御を行うもの。  
>つまり，index.lockが存在するということは，裏で他のgit操作が行われている。  

思い当たる節はある。  
.gitフォルダを含めた状態でいっぺんにコミットしようとしたらなった記憶。  

[git/index.lockを解除し無事にadd/commitできました。](https://creepfablic.site/2019/07/07/git-index-lock/)  

---

## Special Repository (GitHub_プロフィールのREADME)

転職のため、折角アカウントを公開するのだから、最初に開いたときにわかりやすい説明を表示させたい。  
と、思ったので調べた。  

■**作り方**  

1. New Repositoryで自分のアカウント名と同じ名前でリポジトリを作る  

`Special Repository`という扱いらしく、その名の通り特別扱いらしい。  
リポジトリを作ろうとするとスペシャルな文言が表示される。  
>○○/○○ is a ✨special ✨ repository that you can use to add a README.md to your GitHub profile. Make sure it’s public and initialize it with a README to get started.  

[GitHub のプロフィールをかっこよくしたい](https://blog.kosappi.net/entry/2021/04/17/002051)  
[GitHub Readme Stats](https://github.com/anuraghazra/github-readme-stats)  

---

## Aブランチをリベースした後にBブランチにマージして、Aブランチを更にBブランチでリベースしてBブランチにマージすると同じ修正が2回も入るのを確認したのでリベースは最初の一回のみに限る

普通そんなことしないんだろうけど、やったらそうなったのでまとめ。  

---

## ローカルのGit

Gitって別にGithubじゃないといけないってわけじゃないんだよな。  

考えてみれば、ソースコードってどこかのコンピューターに保存されているだけで、そのコンピューターにそういう仕組みが備わっているだけなんだよな。  
その仕組みをローカルに再現して、向き先をそこにすれば、Githubと同じことができるわけだよな。  

[[VS Code 2022年版]GitHub いらず。ローカルだけで Git を使う](https://www.create-forever.games/vs-code-2022-githubless/)

---

[git workflow](https://twitter.com/bibryam/status/1601499207977693184/photo/1)  

[Git 基本の用語集](https://qiita.com/toshi_um/items/72c9d929a600323b2e77)  
[Git で「追跡ブランチ」って言うのやめましょう](https://qiita.com/uasi/items/69368c17c79e99aaddbf)
