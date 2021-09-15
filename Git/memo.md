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
