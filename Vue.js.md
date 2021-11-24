# Vue.jsについて

## Vue CLI

Vue CLIとは CLIはCommnad Line Interfaceの略で、コマンドラインを使ってvue.jsで開発を行うための前準備を支援してくれるツールです。  
開発はプロジェクトという単位で行います。  


## ネイティブ

アンドロイドのアプリのこと



vueへのパスはユーザーの環境変数で充分だった。
SETコマンドで適当な環境変数作っても意味がない。
ユーザーの環境変数のPathに追加する必要がある。
コマンドプロンプトは立ち上げなおさないと、反映されない。

Pathの環境変数はセミコロンで区切る。
最後に C:\Users\[ユーザー名]\AppData\Roaming\npm に追加する。

[管理者権限のないWindowsでvue開発（サンプル付き）](https://qiita.com/nobu-maple/items/9b99cfd22bd811d74765)
WindowsのPathは面倒臭い・・・。

`npm install -g @vue/cli`でグローバルインストールした場合、DotNetDev4ではC:\Users\s.ito\AppData\Roaming\npmにインストールされる。

[node_modulesの再インストール方法](https://zenn.dev/mo_ri_regen/articles/node-modules-article)  

[【Node.js】 Windows上で動作させるための設定知識](https://note.affi-sapo-sv.com/nodejs-windows-setting-knowledge.php)  
`インストーラーでNode.jsをインストールした場合、パッケージのグローバルインストール先として C:\Users\ユーザー名\AppData\Roaming\npm が使用されます。`

node_modules インストール
