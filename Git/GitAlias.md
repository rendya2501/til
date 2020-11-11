# Git Ailias

Gitのコマンドを省略して書けたり、オリジナルのコマンドを登録したりするための機能。
職場で書いた日記を毎回コメント書いて、コマンドを打って上げるのが面倒くさかったので調べてみたら、
その通りのことがあったのでメモとして残すことにした。
現状の設定は以下の通りである。

``` Git
[alias]
  hoge = !echo hogehoge
  anccp = git add -A && git commit -a --allow-empty-message -m '' && git push
```

## エイリアス追加コマンド

``` Git
git config --global alias.nccommit 'commit -a --allow-empty-message -m ""'
```

## コミットメッセージなしのGitコミット

```Git
git commit -a --allow-empty-message -m ''
```

## エイリアスの設定

`hoge = !echo hogehoge`
これはエイリアステスト用のエイリアス。
`git hoge` と入力すると `hogehoge` と表示される。
