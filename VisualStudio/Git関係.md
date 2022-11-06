# Git関係

---

## Revertのやり方

Revertしたいコミット履歴を右クリックして「元に戻す(R)」を選択。  
Revertコミットが自動的に生成されるのでプッシュすればRevert完了。  

---

## コミットを取り消す方法

[コミットを取り消す - リセットの利用](https://www.ipentec.com/document/visual-studio-2019-git-reset-commit)  

---

## Gitの比較のエディタがVSCodeになった

メニューバーのGit→設定→Gitグローバル設定、ツール、差分ツールをVisualStudioにする。

---

## VisualStudio開発におけるgitignore

[ipentec](https://www.ipentec.com/document/visual-studio-create-gitignore-file)  

`dotnet new gitignore`  

PowerShell または DOSプロンプトでコマンドを実行するだけでgitignoreが生成される。  
そのディレクトリでやったほうがいいので、右クリックしてプロンプトを立ち上げてから実行するのが良い。  

直接置いてあるっぽいので、コピるのもありかも。  
[github_VisualStudio.gitignore](https://github.com/github/gitignore/blob/main/VisualStudio.gitignore)  
