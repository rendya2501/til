# WSL2+UbuntuでRemoteDesktop接続できる環境を構築してみた

## 経緯

[マーケットプレイスにつながらない問題](../VSCode/マーケットプレイスにつながらない問題/マーケットプレイスに繋がらない問題.md)  
仮想環境でVSCode入れて拡張を確認できないかやったときに意外と奥が深かったのでこれはこれでまとめたいと思った。  

## 環境構築までの一連の作業

[全部ここ参考](https://www.briteccomputers.co.uk/posts/ubuntu-on-windows-10-with-wsl2-and-gui-setup-via-remote-desktop-2/)  
[ここ同じ事してる](https://qiita.com/atomyah/items/887a5185ec9a8206c7c4)  
[ここもだー](https://ntaka1970.hatenablog.com/entry/2020/08/08/142810#xfce4-goodies-%E3%82%92%E3%82%A4%E3%83%B3%E3%82%B9%E3%83%88%E3%83%BC%E3%83%AB)  

①Ubuntuの環境を最新にする。  
`$ sudo apt update && sudo apt -y upgrade`  

②xrdpパッケージの削除(設定ファイルも)  
`$ sudo apt-get purge xrdp`  

③xrdp,xfce4,xfce4-goodiesパッケージのインストール  
`$ sudo apt install -y xrdp`  
`$ sudo apt install -y xfce4`  
※途中でGDM3とLightDMの選択がありますが、動画に沿って「GDM3」を選択  
`$ sudo apt install -y xfce4-goodies`  

④コピーコマンドでバックアップを作成  
`$ sudo cp /etc/xrdp/xrdp.ini /etc/xrdp/xrdp.ini.bak`  

⑤xrdp.iniを編集  
ポートを3389から3390に変更  
`$ sudo sed -i ‘s/3389/3390/g’ /etc/xrdp/xrdp.ini`  
色を32bitから128bitに変更  
`$ sudo sed -i ‘s/max_bpp=32/#max_bpp=32\nmax_bpp=128/g’ /etc/xrdp/xrdp.ini`  
バックエンド X サーバーの色深度を24bitから128bitに変更  
`$ sudo sed -i ‘s/xserverbpp=24/#xserverbpp=24\nxserverbpp=128/g’ /etc/xrdp/xrdp.ini`  

⑥.xsessionを作成  
`$ echo xfce4-session > ~/.xsession`

⑦nanoエディターでstartwm.shファイルを編集する。  
`$ sudo nano /etc/xrdp/startwm.sh`  
※Editorはお好みで。nanoの部分をviとかに変更したらいける。

以下の最終2行をコメントアウト  

```sh
# test -x /etc/X11/Xsession && exec /etc/X11/Xsession
# exec /bin/sh /etc/X11/Xsession
```

以下2行を最終行として追加  

```sh
# xfce
startxfce4
```

nanoエディターを`Ctrl + x`コマンドで終了。  
編集が行われ、保存するかどうか聴いてくるので`y`を押す。  
また何か言われるけど、保存して問題ないので`Enter`を押す。  

⑧xrdpサービス起動  
`$ sudo /etc/init.d/xrdp start`  

⑨リモートデスクトップ接続  
コンピューターの接続先を`localhost:3390`に変更して接続。  

⑩ログイン  
ログインウィンドウにユーザ名とパスワードを入力して「OK」  

---

ちなみにthe ubuntu の代表たるデスクトップはgnomeらしい。
今回はとにかくできればよかったので、参考サイトにあったらxfce4を使ったがこちらでも試してみたい。
現在の標準デスクトップ環境はGNOMEである。
他のデスクトップ環境を採用した派生ディストリビューションとして、Kubuntu、Xubuntu、Lubuntuなどがある（Unity時代はUbuntu GNOMEも存在した）。

①
「update」は、現在インストールされているそれぞれのパッケージの最新バージョンの情報を取得し、ローカルパッケージインデックスに記録します。つまり、「update」だけでは情報を収集しただけであり、Ubuntuのアップデートは実行されません。
apt upgrade
既存のパッケージを更新。
アップデートに伴い必要な新しいパッケージのインストールも行う。
既存のパッケージは削除しない。

②<https://qiita.com/white_aspara25/items/723ae4ebf0bfefe2115c>

③Xfce
X Window System上で動作するデスクトップ環境の一つ。  
豪華な見た目と簡単な使用感を保ちながら、軽量・高速なデスクトップ環境を目指している。  
<https://ja.wikipedia.org/wiki/Xfce>

xfce4-goodies
たぶんプラグイン的なやつ？入れとけば幸せになれるって書いてあった。  

⑤sedコマンド  
-i : 上書き  
`sed -i '置換条件' ファイルPATH`  
<https://orebibou.com/ja/home/201507/20150731_001/>  

指定の文字列を置換する【「s」】  
「s」が置換のコマンドで、「スラッシュ」3つで挟んだ、前に処理対象、後に置換後の文字列を入れます。  
　指定行内の最初の文字列だけ置換したい場合には、最後の「g」はつけません。  

```bash
  #行で最初にマッチングした文字列だけ
  sed -e s/ポテト/potato/ 対象ファイル
  #行内でマッチングした全て
  sed -e s/ポテト/potato/g 対象ファイル
```

<https://www.yutry.net/pc/program/sed-shellscript#str-replace-sed>  

⑦
[nano参考サイト](http://www.obenri.com/_nano/close_nano.html)  

---

## その他色々

UbuntuはDebian系  
[wiki](https://ja.wikipedia.org/wiki/Ubuntu)  

xrdpとは、LinuxにRDP（Microsoft Remote Desktop Protocol）を提供するサーバー・プログラムである。
<https://qiita.com/yamada-hakase/items/a8efe626f598c5eb6f8c#:~:text=xrdp%20%E3%81%A8%E3%81%AF%E3%80%81Linux%E3%81%ABRDP%EF%BC%88Microsoft,Remote%20Desktop%20Protocol%EF%BC%89%E3%82%92%E6%8F%90%E4%BE%9B%E3%81%99%E3%82%8B%E3%82%B5%E3%83%BC%E3%83%90%E3%83%BC%E3%83%BB%E3%83%97%E3%83%AD%E3%82%B0%E3%83%A9%E3%83%A0%E3%81%A7%E3%81%82%E3%82%8B%E3%80%82x>

[UbuntuでIPアドレスを確認する方法](https://news.mynavi.jp/article/20190825-883094/)  
