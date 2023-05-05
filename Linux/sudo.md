# sudo

## sudoでcommand not found

npmをインストールしたい。  
nodeコマンドで`n lts`を実行する。  
権限エラーとなった。  
`sudo n lts`を実行するが、コマンドがないと言われた。  
sudoだと通常のユーザー環境とは異なる環境変数が適用される模様。  

- `n lts` × → 権限でだめ  
- `sudo n lts` × → コマンドが見つからない。
- `which n` コマンドの場所を調べる  
- `sudo /usr/local/bin/n lts` ○ →コマンドの場所を直接指定して行けた  

今回はパスが短かったのでフルパスで記述したが、他にもいろいろ解決方法はある模様。  

[sudo nodeでcommand not foundが表示される件](https://penpen-dev.com/blog/sudo-node-commandnotfound/)
