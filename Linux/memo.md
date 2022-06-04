# Linux系メモ

## .sh拡張子

シェルスクリプト。  
Windowsにおけるバッチファイル。  
Linux操作コマンドを記述したファイル。  

余談:  
UNIX系OSでは拡張子は意味を持たない。  
UNIXはファイル名に[.]を使ってもOK。  
[Test.sh]で[Test.sh]というファイル名とみなされる。  

---

## cron

UNIX系のOS（MacとかLinuxとか）に入っているプログラムのひとつ  
事前に「いついつになったら、このプログラムを動かしてね」と指示を出しておくと、  
その時間になったときに指定しておいたプログラムを動かしてくれるやつ  
こいつがスケジュールに従ってプログラムを定期実行するサービス(デーモン)ってわけか。  

---

## Linuxディストリビューション

・Red Hat系
 ・Fedora
 ・Red Hat Enterprise Linux
 ・CentOS

・Debian系
 ・Ubuntu
 ・Linux Mint
 ・Raspbian

・Slackware系
 ・Slackware
 ・openSUSE

・独立系
 ・Arck Linux

---

## 時々コマンドに付いてる--(Double Dash)って何なの？

[時々コマンドに付いてる--(Double Dash)って何なの？](https://zenn.dev/dowanna6/articles/245df006deee0c)  

結論  
「--」は「これ以降の入力はオプションではありません」と指定する記号です。  

---

## Linuxは小文字、大文字を判別する
