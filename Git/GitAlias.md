# Git Ailias

Gitのコマンドの「別名」を付ける機能のこと。  
タイピング量を減らしたり、覚えやすいコマンド名に置き換えるために利用される。  
毎回コメント書いて、コマンドを打って上げるのが面倒くさかったので調べてみたら、  
その通りのことがあったのでメモとして残すことにした。  

git隠しフォルダのconfigファイルに`[alias]`の項目を追加する。  

```Git
[alias]
    hoge = !echo hogehoge
    <!-- コメントが必要だけど、自動でプッシュまでする -->
    acp = "!f() { git add -A && git commit -a -m \"$@\" && git push; }; f"
    <!-- コメント無しでプッシュまでする -->
    anccp = "!f() { git add -A && git commit --allow-empty-message -m '' && git push; }; f"
```

なんか実験ぽい奴

```Git
    echo = "!f() { echo $1 %ad; }; f"
    logp = log --pretty=format:'%h %ad | %s%d [%an]' --graph --date=short
    datetag2 = echo $(date +%Y%m%d)
```

[参考サイト1](https://www.it-swarm-ja.tech/ja/git/git-add%E3%80%81commit%E3%80%81push%E3%82%B3%E3%83%9E%E3%83%B3%E3%83%89%E3%82%921%E3%81%BE%E3%81%A8%E3%82%81%E3%81%A6%E3%81%BE%E3%81%97%E3%81%9F%E3%81%8B%EF%BC%9F/1043252019/)
[参考サイト2](https://qiita.com/YamEiR/items/d98ba009d2925e7eb305)

## エイリアス追加コマンド

```Git
git config --global alias.nccommit 'commit -a --allow-empty-message -m ""'
```

## エイリアスの設定

`hoge = !echo hogehoge`
これはエイリアステスト用のエイリアス。  
`git hoge` と入力すると `hogehoge` と表示される。
