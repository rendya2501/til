# Git Ailias

Gitのコマンドを省略して書けたり、オリジナルのコマンドを登録したりするための機能。
職場で書いた日記を毎回コメント書いて、コマンドを打って上げるのが面倒くさかったので調べてみたら、
その通りのことがあったのでメモとして残すことにした。
現状の設定は以下の通りである。

```Git
[alias]
  hoge = !echo hogehoge
  acp = "!f() { git add -A && git commit -a -m \"$@\" && git push; }; f"
  anccp = "!f() { git add -A && git commit --allow-empty-message -m '' && git push; }; f"
```

職場で書いてたなんか実験ぽい奴

```Git
  echo = "!f() { echo $1 %ad; }; f"
  logp = log --pretty=format:'%h %ad | %s%d [%an]' --graph --date=short
  datetag2 = echo $(date +%Y%m%d)
```

[参考サイト](https://www.it-swarm-ja.tech/ja/git/git-add%E3%80%81commit%E3%80%81push%E3%82%B3%E3%83%9E%E3%83%B3%E3%83%89%E3%82%921%E3%81%BE%E3%81%A8%E3%82%81%E3%81%A6%E3%81%BE%E3%81%97%E3%81%9F%E3%81%8B%EF%BC%9F/1043252019/)

## エイリアス追加コマンド

```Git
git config --global alias.nccommit 'commit -a --allow-empty-message -m ""'
```

## コミットメッセージなしのGitコミット

```Git
git commit -a --allow-empty-message -m ''
```

[参考サイト](https://okamerin.com/nc/title/509.htm)

## エイリアスの設定

`hoge = !echo hogehoge`
これはエイリアステスト用のエイリアス。  
`git hoge` と入力すると `hogehoge` と表示される。
