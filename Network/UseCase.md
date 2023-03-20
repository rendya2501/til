# 逆引き

## IPアドレスからドメイン名を取得する

■ **linuxの場合**

- `dig`コマンド  
  `dig -x xxx:xxx:xxx:xxx`  

- `host`コマンド  
  `host xxx:xxx:xxx:xxx`  

■ **windowsの場合**

- `nslookup`コマンド  
  `nslookup xxx:xxx:xxx:xxx`  

[LinuxでIPアドレスからホスト名/ドメイン名を取得する方法–Linuxヒント](https://ciksiti.com/ja/chapters/3833-how-to-get-a-hostname-domain-name-from-an-ip-address-in-linu)  
[IPアドレスからドメイン名を割り出す「逆引き」について](https://rainbow-engine.com/domain-from-address-revdns/#:~:text=DNS%E3%82%B5%E3%83%BC%E3%83%90%E3%81%8C%E5%88%A9%E7%94%A8%E3%81%A7%E3%81%8D%E3%82%8B,%E6%96%B9%E6%B3%95%E3%81%8C%E3%81%8A%E3%82%B9%E3%82%B9%E3%83%A1%E3%81%A7%E3%81%99%E3%80%82&text=Windows%E3%81%AE%E6%A4%9C%E7%B4%A2%E7%AA%93%E3%81%8B%E3%82%89,%E3%81%A8%E3%81%97%E3%81%A6%E5%AE%9F%E8%A1%8C%E3%80%8D%E3%81%A7%E9%96%8B%E3%81%8D%E3%81%BE%E3%81%99%E3%80%82&text=%E5%AE%9F%E8%A1%8C%E7%B5%90%E6%9E%9C%E3%81%AE%E4%B8%AD%E3%81%8B%E3%82%89,%E3%81%9F%E8%A1%8C%E3%82%92%E6%8E%A2%E3%81%97%E3%81%BE%E3%81%99%E3%80%82)  

---

## ドメイン名からIPアドレスを取得する

■ **windowsの場合**

- `ping`コマンド  

---

## IPアドレスとポート番号を取得する

■ **windowsの場合**

- `nslookup`コマンド  

---

## あるポート番号が何のアプリケーションで使用されているか調べる方法

「netstat」コマンドを実行する。  
`netstat -nao`  

■ 調べたいポート番号が決まっているとき  
`netstat -nao|find "ポート番号"`コマンドで実行時にポート番号を指定して実行することができます。  
`netstat -nao|find "1433"`  

開発をしていると、たまに自分のポートが使えなくなるときがあるのでそれで調べた。  

[あるポート番号が何のアプリケーションで使用されているか調べる方法](https://www.projectgroup.info/tips/Windows/comm_0133.html)  
