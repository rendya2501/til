# 雑記

## XPSファイル

<http://office-qa.com/Word/wd183.htm>  
<https://e-words.jp/w/XPS.html>  

XML Paper Specification  
WordやExcelといったファイルをXPSファイルに変換すると、環境に依存せず様々なコンピュータでファイルを表示できます。  
また編集不可能な為、改ざんされたくない文書を配布する場合にも便利です。XPSファイル はXPSビューワー （無料）で表示可能です。  

福田さんに変換の仕方を聞かれて初めて存在を確認した形式ファイル。  
なじみがなかったけど、そんなのもあるのね。  

---

## HTML5の廃止

<https://future-architect.github.io/articles/20210621a/>  
<https://www.tohoho-web.com/html/memo/htmlls.htm>  

全然知らなかった。
HTML5は2021年1月28日に廃止された模様。  
次期HTMLのバージョンはHTML Living Standardと呼ぶらしい。  
ヴァージョンに関して解説してあるページがあるのでそこを参照されたし。  
ざっくりいうと、開発元(W3C)とWeb系(Apple,Mozilla)の強い会社でバチバチにやりあって、Webの会社が勝利したって結末。  
色々あって面白い。  

---

## ローカルログイン

酒井君のPCセットアップの時に岡部部長が発した単語。  
普通のログインって何に当たるのか。ログインにはどのような種類があるのか気になったのでまとめた。  

<https://milestone-of-se.nesuke.com/sv-advanced/activedirectory/domain-and-local-users/>  
どうも、ActiveDirectory前提の話みたいだ。  
うちの会社見たく、ALPというドメインに参加している場合のログインと、そういうのに属していない個別のユーザーでのログインに大別されるらしい。  
大抵は個別ユーザーでのログイン(WORKGROUP環境(デフォルト状態))になるっぽい。  
普通のご家庭なら断然これだろう。  
うちはActiveDirectoryを導入していているので、ドメインユーザーとしてログインする必要があるのだろう。  
セットアップ中、ファイルサーバーに入れなかったりしたので、  
ドメインではなくローカルでログインしているのでは？という意味での発言だったのだろう。  

因みに、気になって自分のドメインを確認してみたが、どこを探してもALPという文言はなく、MicroSoftアカウントでログインしてるっぽいです。  
いいのかね。  
