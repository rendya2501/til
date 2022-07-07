# CentOSに関する色々

## Web開発環境について

OS : CentOS 7  
※CentOSはRedHat系  

・OS確認コマンド  
`$ cat /etc/redhat-release`  

・chrome update コマンド  
`$ sudo yum install https://dl.google.com/linux/direct/google-chrome-stable_current_x86_64.rpm`  

・chrome version 確認コマンド  
`$ google-chrome --version`  

[参考サイト](http://www.ajisaba.net/develop/chrome/install_centos7.html)  

---

## VisualStudioUpdate手順  

1. .rpmを選択  
   するとコマンドページに飛ばされるので、上から順番にコマンドを実行していく。  
2. `$ sudo rpm --import [指定されたURL]`  
3. `$ sudo sh -c 'echo -e [なんかとても長い呪文]`  
4. `$ sudo yum check-update`  
   ここで結構待つ。  
5. `$ sudo yum -y install code`  

---

## 管理者権限でフォルダを消す

確認あり : `sudo rm -rf opt`  
確認なし : `sudo rm -r opt`  

rm : remove  
-r : 配下のデータも削除する  
-f : 確認なしで削除する  

背景:  
ログが増えすぎたのでフォルダ毎消そうとしたとき、そのフォルダには鍵マークが付いていた。  
最初は`rm -rf opt`コマンドを実行したけど、権限がないといわれたので、管理者権限`sudo`を付けて再実行した。  
`-f`が付いていたので警告もなしに一瞬で消えたので、戦慄したのを覚えている。  
