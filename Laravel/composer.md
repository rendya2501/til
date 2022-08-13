# composer

---

## composerとは？

PHPのパッケージ依存管理ツール  

[(PHP) composerとは【使い方・概念など】](https://hara-chan.com/it/programming/php-composer/)  

---

## composerインストールで躓いたこと

ここと全く同じ現象だった。  

>Xdebugの設定をxdebug.start_with_request = yesにしてあるのが原因。
必要がないときもXdebugが呼び出されてしまう。  
[ComposerのWindowsインストーラー実行時に発生したエラーへの対処](https://qiita.com/ienoue/items/d8f7c40fcd3528f38717)  

php.iniに以下の行を追加することで解決した。  
`xdebug.log_level=0`  

インストール出来たらシェルでコマンドを実行。  
`composer -v`  

応答が帰ってきたらインストールは成功  

---

## composerのアップデート

`composert self-update`  

[composer のバージョンをアップデートする](https://qiita.com/onkbear/items/f98d274d38eacfe7a209)  
