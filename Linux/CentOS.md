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
