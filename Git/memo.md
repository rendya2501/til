# 雑記

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

```git
git fetch
git checkout <ブランチ名>
```

fetchはリモートリポジトリの情報をローカルに持ってくるだけで、実際にワークツリーには反映されていません。  
そこからcheckoutすることで、ローカルにブランチを生成する。  
つまり、リモートのブランチをローカルに持ってくるというわけ。  

[リモートブランチをローカルで取得する[git fetch][Sourcetree]](https://noumenon-th.net/programming/2018/12/24/git-fetch/)  
[リモートブランチを取得する方法 #git](https://www.yokoyan.net/entry/2018/06/18/183000)  

こういうのもあるみたい。  

[【Git】リモートブランチをチェックアウトしたいときは「git fetch origin <ブランチ名>」と「git checkout <ブランチ名>」を実行すれば良い](https://dev.classmethod.jp/articles/how-to-checkout-remote-branch/)  

これすごい。  

[【Git】リモートからの取得とリモートへの反映で行っていること(fetch,pull,push)](https://qiita.com/forest1/items/db5ac003d310449743ca)  

---

## コミットメッセージなしのGitコミット

```git
git commit -a --allow-empty-message -m ''
```

[参考サイト](https://okamerin.com/nc/title/509.htm)

---

## カレントブランチをpushする方法

```git
git push origin HEAD
```

[参考サイト](https://qiita.com/mabots/items/76d48aa33720287253bf)
