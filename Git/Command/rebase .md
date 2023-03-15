# Git Rebase

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

---

## Visual Studioで rebase する方法

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

流れがあってわかりやすい。  
[リベースによるより良いGitワークフロー](https://www.youtube.com/watch?v=f1wnYdLEpgI)  

[あなたはmerge派？rebase派？綺麗なGitログで実感したメリット](https://style.biglobe.co.jp/entry/2022/03/22/090000#develop%E3%83%96%E3%83%A9%E3%83%B3%E3%83%81%E3%82%92%E7%B6%BA%E9%BA%97%E3%81%AB%E4%BF%9D%E3%81%A4Git%E6%93%8D%E4%BD%9C%E3%83%9E%E3%83%BC%E3%82%B8%E7%B7%A8)  

---

## push後のrebaseについて

pushするまではrebase、pushしたあとはmerge  

基本的に、push後のrebaseはやめたほうがいいらしい。  

そういえば昔こんなことがあった。  

- AブランチをBブランチでリベース。  
- Bブランチにマージ。
- Aブランチで変更を加えてBブランチでリベース。
- Bブランチにマージ。

すると同じ修正が2回も入ってしまったのを確認したのでリベースは最初の一回のみに限ると思った。  
普通そんなことしないんだろうけど、やったらそうなったのでまとめ。  

[pushしてからrebaseはダメ！ - Qiita](https://qiita.com/riku929hr/items/15415d34ee5fc412c126)  
