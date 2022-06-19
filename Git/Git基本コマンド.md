# Git 基本コマンドまとめ

[基本的なGitコマンドまとめ](https://qiita.com/2m1tsu3/items/6d49374230afab251337)  

``` txt : 早見表
clone    :: リポジトリのクローンを作成する
init     :: リポジトリを新規作成する、または既存のリポジトリを初期化する
remote   :: リモートリポジトリを関連付けする
fetch    :: リモートリポジトリの内容を取得する
pull     :: リモートリポジトリの内容を取得し、現在のブランチに取り込む(「fetch」と「merge」を行う)
push     :: ローカルリポジトリの変更内容をリモートリポジトリに送信する
add      :: ファイルをインデックスに追加する(コミットの対象にする)
rm       :: ファイルをインデックスから削除する
mv       :: ファイルやディレクトリの名前を変更する
reset    :: ファイルをインデックスから削除し、特定のコミットの状態まで戻す
revert   :: 取り消したいコミットを打ち消すようなコミットを新しく作成する
status   :: ワークツリーにあるファイルの状態を表示する
show     :: ファイルの内容やコミットの差分などを表示する
diff     :: コミット同士やコミットと作業ツリーの内容を比較する
commit   :: インデックスに追加した変更をリポジトリに記録する
tag      :: コミットにタグを付ける、削除する、一覧表示する
log      :: コミット時のログを表示する
grep     :: リポジトリで管理されているファイルをパターン検索する
branch   :: ブランチを作成、削除、一覧表示する
checkout :: 作業ツリーを異なるブランチに切り替える
merge    :: 他のブランチやコミットの内容を現在のブランチに取り込む
rebase   :: コミットを再適用する(ブランチの分岐点を変更したり、コミットの順番を入れ替えたりできる)
config   :: 現在の設定を取得、変更する
cherry-pick :: 他ブランチの特定のコミットを現在のブランチに取り込む
```

---

## git add

変更内容をインデックスに追加してコミット対象にする。  

### git add コマンド実行例

全ての変更内容をインデックスに追加し、git commit の対象にする  
`git add -A`  

指定したファイルの変更内容をインデックスに追加し、「git commit」の対象にする  
`git add ファイル名`

「*.html」と「images/*.png」をインデックスに追加する  
`git add *.html images/*.png`  

カレントディレクトリの全てのファイルをインデックスに追加する  
`git add .`  

「git add .」でどのファイルが対象となるかを表示する  
`git add -n .`  

### git add の主なオプション

``` txt
短いオプション    長いオプション    意味

-A    --all、--no-ignore-removal    Gitで管理していないファイルも含めてインデックスに追加する(ワークツリーから削除したファイルは、削除したことを登録する)
-u    --update    Gitで管理している全てのファイルの変更内容をインデックスに追加する(本文を参照)
-p    --patch    ファイル単位での追加はなく、個々の変更内容を確認しながらインデックスに追加する
-e    --edit    変更内容を編集する
-f    --force    「.gitignore」で対象外としているファイルもインデックスに追加する
-i    --interactive    インタラクティブモードで処理を選択する(［q］キーまたは［CTRL］＋［C］キーで中断)
-N    --intent-to-add    後で追加する予定であることを記録する(正式に追加するまで「git commit」コマンドの対象にはならないが、変更箇所を確認する「git diff」コマンドの対象になる)
-n    --dry-run    実行内容を表示するだけで実際には実行しない(本文を参照)
-v    --verbose    実行中の内容を表示する
--ignore-errors    途中の処理に失敗しても操作を中断しない
--renormalize    正規化をやり直す(cleanプロセスを適用してインデックスに追加し直す。改行コードの自動変換設定を変更した際などに使用する)
--    この指定以降は全て引数(ファイル名やディレクトリ名)として扱う
```

---

## git commit

インデックスの内容をローカルリポジトリに記録／保管する

### git commit コマンドの書式

`git commit [オプション] [ファイル名……]`  

### git commit コマンド実行例

既存ファイル含めて全てコミットする  
`git commit -a`  

メッセージを指定してコミットする  
`git commit -m メッセージ`  

メッセージなしでコミットを実行する  
`git commit -m --allow-empty-message`  

### git commitの主なオプション

``` txt
短いオプション    長いオプション    意味

-a    --all    ワークツリーで変更済みのファイルをコミットする(「git add」コマンドによる操作を省略する、※2)
--interactive    ファイル単位で選択しながらコミットする
--patch    個々の変更内容を確認しながらコミットする
--amend    直前のコミットをやり直す
--allow-empty    空のコミットを許可する(※3)
-m メッセージ    --message=メッセージ    コミットメッセージを指定する
--allow-empty-message    メッセージを付けないコミットを許可する
-C コミット    --reuse-message=コミット    コミットメッセージを指定したコミットから読み出す(※4)
-t ファイル    --template=ファイル    コミットメッセージ用のテンプレートを指定したファイルから読み出す(※4)
--author=作者    作者(Author)を指定する(デフォルトは「git commit」コマンドを実行したユーザー名)
--reset-author    作者(Author)を置き換える(「-C」や「--amend」と併用する)
-s    --signoff    「Signed-off-by」行をコミットメッセージに追加する(※5)
--date    コミットした日時(デフォルトは「git commit」コマンドを実行した日時)
-n    --dry-run    実行内容を表示するだけで実際には実行しない
-v    --verbose    実行中の内容を表示する
```

---

## git push

ローカルリポジトリの内容をリモートリポジトリに送信する  

### git push コマンド書式

`git push [オプション] リモート名`  
`git push add [オプション] リモート名 ブランチ名`  
`git push [オプション] リモート名 ブランチ:リモートのブランチ`  

### git push コマンド実行例

上流ブランチとして設定されている場所に送信する  
`git push`  

実行内容を確認するだけで実行はしない  
`git push -n`  

カレントブランチをpushする  
`git push origin HEAD`  

リモートリポジトリ origin の mainブランチにプッシュする。その際、mainブランチを上流ブランチとして設定する。  
`git push -u origin main`  

### git pushの主なオプション

``` txt
short  long           mean

       --all          全てのブランチを対象とする
-u     --set-upstream 今回対象とするブランチを上流ブランチとして設定する(一度指定すると次回以降も対象となる)
-d     --delete       指定したブランチをリモートから削除する(「:」のみを指定すると全て削除する)
-f     --force        リモートブランチがローカルブランチの派生元ではない場合も、ローカルブランチの内容で強制的に上書きする
-v     --verbose      実行時のメッセージを増やす
-q     --quite        実行時のメッセージを減らす
-n     --dry-run      実行せずに実行する内容だけを表示する
       --prune        ローカルで削除したブランチをリモートからも削除する
       --atomic       実行途中でブランチの変更に失敗したら、実行前の状態に戻す(サーバが対応している場合のみ)
       --tags         全てのタグを対象とする
       --mirror       ローカルの内容をリモートにそのまま複製する
       --signed=設定  GPG(GNU Privacy Guard)署名付きで実行するかどうかを指定する
                      [true] :常に署名付き
                      [false] : 常に署名なし
                      [if-asked] :サーバ側が署名付きに対応していた場合だけ署名付き
       --signed       GPG署名を付けて実行する(「--signed=true」相当)
       --no-signed    GPG署名を付けないで実行する(「--signed=false」相当)
```

### カレントブランチをpushする書式のHEADとは何か

`git push origin HEAD`  

[まだ git push origin するときに current branch 名を入力して消耗しているの?](https://qiita.com/mabots/items/76d48aa33720287253bf)  

「HEAD」とは、ブランチの先頭を表す名前で、デフォルトではローカルリポジトリのmasterの先頭を表しています。  
「origin/HEAD」はリモートリポジトリのHEADの位置を表します。  

---

## git pull

ただgitpullするとそのブランチに置ける最新を取得してくれる模様。  
割と始めて知った。  

リモートリポジトリの変更内容を取り込むには、作業ディレクトリ（.gitの親ディレクトリ）で、「git pull」を実行します（画面1）。

　「git pull」では、全てのブランチの変更内容を取得し、現在作業対象としているブランチ（通常はデフォルトブランチである「master」）の変更内容を作業ディレクトリに反映します。

　特定のブランチの変更内容だけを取り込みたい場合は「git pull リモートの名前 ブランチの名前」のように指定します。

　「git pull」コマンドがリポジトリの変更内容を取り込む際、内部では2つの過程を処理します。最初の過程は「git fetch」で、リポジトリの内容がどのように変更されているのかという情報を取得します（リモートリポジトリ→ローカルリポジトリ）。

　次の過程は「git merge」で、「git fetch」で取得した情報を、現在のブランチに取り込みます（ローカルリポジトリ→作業ツリーに反映）。「fetch」や「merge」については今後の連載で取り上げる予定です。

### git pull コマンド書式

`git pull [リポジトリ] [ブランチ]`
`git pull [リモート] [ローカル]`

### git pull コマンド実行例

425をやっているうちにbug-fix-develop-alpに適応された変更を425にマージしておきたい。  
`git pull origin bug-fix-develop-alp`  

bug-fix-develop-alpを最新にするための取得先とコマンド  
`git pull pgmweb develop`  

---

## git merge

gitのマージはマージしたいブランチに行ってから、マージ元を指定するしかないみたい。  
てっきり、マージ元とマージ先を指定できるのかと思ってた。  
TeelaだとPushしてそのままだし、ブランチのマージはGitLab上でやるし、その後、Pullして勝手に修正が反映されるだけだから触れる機会が全くなかった。  

``` git
git checkout マージ先ブランチ名
git merge マージ元ブランチ名
```

---

## git remote

リモートリポジトリを追加、削除する

### git remote コマンド実行例

リモートリポジトリの名前を表示する  
`git remote`  

リモートリポジトリの名前と場所《URL》)を表示する  
`git remote -v`  

指定したリモートリポジトリの詳しい情報を表示する  
`git remote show 名前`

---

## git revert

[【gitコマンド】いまさらのrevert](https://qiita.com/chihiro/items/2fa827d0eac98109e7ee)  

取り消したいコミットを打ち消すようなコミットを新しく作成するコマンド  

指定したコミットを打ち消すコミットを作る。  
個別に巻き戻し可能。  
履歴に残る。
場合によっては巻き戻しの時に競合が発生する可能性あり。  
小規模な対応にはいいかも。  
大規模な差し戻し(別のブランチとのマージをなかったことにする場合など)は、量が多すぎて巻き戻すときの競合リスクや大量の履歴が発生してしまうのでお勧めしない。  

revertにコミットIDを指定した場合、そのコミットに対する巻き戻しだけを行う。

---

## git reset

[[git reset (--hard/--soft)]ワーキングツリー、インデックス、HEADを使いこなす方法](https://qiita.com/shuntaro_tamura/items/db1aef9cf9d78db50ffe)  

reset はHEADを移動させる。  

個別の巻き戻しは不可能。  
ヘッドを移動させる処理なので、上から何番目までヘッドを戻すという指定しかできない。  
なので、このコミットだけなかったことにしたいってのは無理。  
履歴に残らない。  
小規模でも大規模でも行ける。  
ただ、ヘッドを戻すので、他人がそのブランチを開いた時に未プッシュがある状態となって、何も知らずにプッシュされると整合性が取れなくなる必要があるので、注意が必要。  
連絡するとかコミュニケーションは必須。  

resetにコミットIDを指定した場合、それより前のコミット全てなかったことにする。

基本的に、ある特定のコミットだけを取り消すことはできないらしい。
と、思ったが調べればごまんとでてくるぞ。

git reset --hard HEAD  
編集・ステージングいずれの変更内容を取り消し、最後にコミットした状態に戻す  

---

## git rebase

RebaseはBranchの根元を変える処理。  
Rebaseは、結果として逆Mergeと同じ状態となります。  

`git rebase <base>`  
`git rebase <base> <checkout>`  

次の履歴が存在し、現在のブランチが「topic」であると想定します。  

``` txt
      A---B---C topic
    /
D---E---F---G master
```

この時点で、次のコマンドのいずれかの結果となります。

``` bash
git rebase master
git rebase master topic
```

このようになります。

``` txt
                A'--B'--C' topic
              /
D---E---F---G master
```

注記：後者の形式はgit checkout topic の省略形であり、その後にgit rebase masterが続きます。  
リベースが終了すると、topicはチェックアウトされたブランチのままになります。  

[tracpath](https://tracpath.com/docs/git-rebase/)  

### Visual Studioで rebase する方法

Visual Studioで rebase するときはmergeと同じやり方でやる。  

- Release  
- Fix/1234  

`Release`の変更を`Fix/1234`に取り込み、`Fix/1234`を最新の状態にしたい時にRebaseを使う場合を想定する。  

1. `Fix/1234`をチェックアウトする。  
2. `Release`を右クリックし、「`Fix/1234`を`Release`にリベースにする」を選択する。  
   ※ 英語版の場合、表記は「Rebase 'Fix/1234' onto 'Release'」  

マージの場合の表記は「`Release`を`Fix/1234`にマージする」となっている。  
Fix/1234にReleaseを取り込むので(取り込んで最新化する)、言葉通りに感じる。  
しかし、リベースの場合は表現が逆。  
この表現だとReleaseブランチをリベースするように見えるが、これであっているっぽい。
(Fix/1234ブランチをReleaseに持ってきて、Releaseを書き換えてしまうイメージ)  
リベースをよく知らない時に、この表現にだまされて、Releaseブランチをチェックアウトして、Fix/1234ブランチを右クリックしてリベースしてしまった。  

`git checkout Fix/1234`  
`git rebase Release`  
or  
`git rebase Release Fix/1234`  

``` txt : 元
      A---B---C Fix/1234
    /
D---E---F---G Release
```

``` txt : リベースした場合
               A'--B'--C' Fix/1234
              /
D---E---F---G Release
```

``` txt : マージした場合
      A---B---C---H  Fix/1234
     /          /
D---E---F------G  Release

※Hはマージコミット
```

誰にも影響を与えない「まだpushしていない、ローカルの開発内容」だったら、rebaseを使用しても大丈夫です。  

[classmethod_GitのRebaseによるBranchの運用](https://dev.classmethod.jp/articles/development-flow-with-branch-and-rebase-by-git/)  
→  
Rebaseでの運用  
逆Mergeの運用はどんなバージョン管理システムを使った場合でも適用できます。  
が、Gitにおいては逆Mergeの代わりにRebaseを使うと、よりクールに運用できるのでお勧めです。  
Rebaseは、結果として逆Mergeと同じ状態となります。異なるのは変更の適用方法です。  

[リベースによるより良いGitワークフロー](https://www.youtube.com/watch?v=f1wnYdLEpgI)  
→  
流れがあってわかりやすい。  

### fast-forwardマージ

[fast-forwardマージから理解するgit rebase](https://qiita.com/vsanna/items/451b42f886c599a16a55)  

ブランチXと、そこから切ったブランチYがあるとする。  
YがXでの変更をすべて含むときに行われるマージをfast-forward（早送り）マージという。  
つまり、分岐後に、元ブランチXにおいて変更がないときに行われるマージのこと  

ReleaseからFixを切り、Fixをリベースして追加の修正を行い、そのままReleaseにマージしてもマージコミットが発生しないのはこの仕組みがあるからっぽい。  
全てをマージしたときはがっつりマージコミットと分岐の履歴が残るけど、リベースの場合は残らない。  
ここから先は実環境で確かめてみるしかないかも。  

``` txt : 元の状態
      A---B---C Fix/1234
     /
D---E---F---G Release
```

``` txt : リベース
               A'--B'--C' Fix/1234
              /
D---E---F---G Release
```

``` txt: リベースしてからD',E'をコミット
               A'--B'--C'--D'--E' Fix/1234
              /
D---E---F---G Release
```

``` txt : Fix/1234をReleaseにマージした場合のコミット状況
D---E---F---G---A'--B'--C'--D'--E' Fix/1234,Release
```

あくまでReleaseFixまでの話で、Releaseにマージする時はプルリクするからマージコミットが出来上がるはず。
まぁ、それほど影響はないのかな？

---

## git cherry-pick

他ブランチの特定のコミットを現在のブランチに取り込むコマンド  
`git cherry-pick [コミットID]`  

[【Git】cherry-pickで解決](https://qiita.com/okmtz/items/62aa5a25f75b1754a861)  

### git cherry-pick コマンド実行例

ブランチBのAコミット、BコミットをブランチAに取り込む。  

`git checkout ブランチA`  
`git cherry-pick [Aのコミットid] [Bのコミットid]`  

※事前にコミットIDは調べておく必要がある

``` txt
D---E---F------------G ブランチA
   │  _ _ _ _ _ _ _/
   │ /   /
   └A---B---C ブランチB
```
