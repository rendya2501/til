# Go言語 メモ

## Error loading workspace: You are outside of a module and outside of $GOPATH/src

Go + VSCodeの組み合わせで発生。  
自分の場合は以下の手順で解決した。  

1. ctrl + shift + P
2. preference
3. `go.useLanguageServer`を`false`にする。

[【学習記録】Golangの入門](https://zenn.dev/gamari/scraps/7e72f66b332686)  
