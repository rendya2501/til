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

`dotnet new gitignore`  

PowerShell または DOSプロンプトでコマンドを実行するだけでgitignoreが生成される。  
そのディレクトリでやったほうがいいので、右クリックしてプロンプトを立ち上げてから実行するのが良い。  

直接置いてあるっぽいので、コピるのもありかも。  
[github_VisualStudio.gitignore](https://github.com/github/gitignore/blob/main/VisualStudio.gitignore)  
[ipentec](https://www.ipentec.com/document/visual-studio-create-gitignore-file)  

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

メニュー → 表示 → ターミナル → 歯車マーク → 追加 → 下記の設定を行う → 適応  

``` txt
名前         :: git-bash
シェルの場所 :: C:\Program Files\Git\bin\sh.exe
引数         :: -l -i

※要: シェルの場所はボタンでダイアログを開いて選択すべし
```

[git-bashをVisual Studio 2019 のTerminalに追加する手順](https://qiita.com/murasuke/items/45bce6bf40f0d701d595)  
