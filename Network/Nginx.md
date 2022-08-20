# Nginxまとめ

[Nginx 入門](http://www2.matsue-ct.ac.jp/home/kanayama/text/nginx/all.html)  

---

## nginxコマンド

### 起動

`sudo service nginx start`  

### 再起動

`sudo nginx -s reload`  

[[nginx]設定の反映](https://qiita.com/WisteriaWave/items/fa2e7f4442aee497fe46)  
通常、Webサーバーの再起動を行った場合、瞬断が発生するが、-sオプションをつけることでそれがなくなるらしい。  
-sオプションを指定すると、nginxのworkerプロセスが少しずつ新しいプロセスに置き換わっていくだとかなんだとか。  

### 停止

`sudo nginx -s stop`  

---

## ポートの変更

[nginx 起動ポートの変更](https://qiita.com/Watercat3/items/38b2bac5fa70b232bee3)  

ポートにおける設定ファイル  

/etc/nginx/conf.d/2061.conf : bugfix  
/etc/nginx/conf.d/2062.conf : hotfix  

---

## 開発用hotfixポートでエラーが正常に表示されない問題

[cakephp3 + CentOS8 + nginx + php-fpmでエラー表示がされないとき](https://blog.supersonico.info/archives/4277/)  

こいつが悪さしてた。  
→`fastcgi_intercept_errors on;`  

`location ~ \.php$ {}`の中に記述すると300以上のエラーがクライアントに伝わらなくなるので、正常にエラー表示ができなくなる。  
2062ファイルにはこれが設定されていたので、hotfixでは意図的に500エラーを発生させても、bugfixのようなエラー表示にならなくて困っていた。  
原因はサーバーの設定であって、Laravelの機能ではなかった。  