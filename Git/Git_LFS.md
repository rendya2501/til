# Git LFS

LFS(Large File Storage)  

---

## LFSのインストール

Windows・Linixは[公式サイト](https://git-lfs.com/)からインストーラーをダウンロードして展開する。  
インストールが完了したら次のコマンドを実行する。  

`git lfs install`  

Macは`Brew`コマンドで一発でいける模様。  

[[GitHub] Git LFSで巨大なファイルを扱う](https://blog.katsubemakito.net/git/github-gitlfs)  

---

## LFSの使用方法

事前にLFSをインストールしていること。  
既にpushしたいもの一式とディレクトリを容易していること。  

1. GitHubでリポジトリを作成  
2. 対象ディレクトリで以下のコマンドを実行していく  
   - `git init`  
   - `git lfs track '*.pdf'`  
   - `git add -A`  
   - `git commit -m "first commit"`  
   - `git branch -M main`  
   - `git remote add origin https://github.com/****`  
   - `git push -u origin main`  

先にリポジトリを作成して、クローンしてからpushしようとするとうまくいかなかった。  
git initの段階からスタートしないと、上手いことLFSを認識させることができない模様。  
クローンした後にgitの情報を削除してinitからやり直したらうまくいったので、ローカルですべて設定してからpushするのがよい。  

---

## 参考サイト

大体このサイト、リスペクト。  
他のサイトは要領を得ない解説ばかりでわかりにくかったが、このサイトだけ良くまとまっていると感じた。  
実際、このサイトで紹介されている通りにやってうまくいった。  
[[GitHub] Git LFSで巨大なファイルを扱う](https://blog.katsubemakito.net/git/github-gitlfs)  

LFSとは直接関係ないが、やろうとしていることは同じなので、参考として挙げた。  
やってることは、かなりグレーでいつ規制されてもおかしくないとのことなのでやるなら自己責任で。  
ただ、発想は面白い。  
[GitHub Hacking ~GitHubを容量無制限のクラウドストレージとして使用する試み~](https://qiita.com/taptappun/items/e499dfa937cd8f04d74d)  

LFSの概要を理解するにはよい。  
[Git LFSの使用方法 – Backlog ヘルプセンター](https://support-ja.backlog.com/hc/ja/articles/360038329474-Git-LFS%E3%81%AE%E4%BD%BF%E7%94%A8%E6%96%B9%E6%B3%95)  
