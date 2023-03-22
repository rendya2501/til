# アスペクト指向プログラミング(Aspect Oriented Programming)

## 概要

オブジェクト指向の苦手な部分を補強する概念  

複数の懸念に関する物事をモジュール化できるようにすること。  
ロギングや例外処理等はアプリケーションの特定のレイヤーに特化したものではない。  

- トランザクション制御  
- ロギング処理  
- 認証処理  
- 例外処理  

---

## Transaction

事の発端はこれ。  
RepositoryPatterを実装している時に、複数のリポジトリを跨いだトランザクションの制御はどうやればいいのか？という疑問に対して調査している最中、Javaの`@Transaction`アトリビュートなるものに行きついた。  

このアトリビュートをつけると、そのメソッドの開始と同時にトランザクションを開始して、終了と同時にコミットを行ってくれるという優れもの。  
そのメソッドがトランザクションを実行することも明示的に説明できるという一石二鳥的な側面もある。  

C#でもできなくはないが、メソッド単位は無理な模様。  
このように、アトリビュート（C#ではアノテーション)によって共通的な処理を行う方式はAOP(アスペクト指向プログラミング)と呼ばれるらしい。  
流行っていないのでお察しだが、トランザクションに関しては便利だなと思った。  

[C# AOP Transaction Aspect](https://stackoverflow.com/questions/71703376/c-sharp-aop-transaction-aspect)  
[Transactional annotation attribute in .NET Core](https://stackoverflow.com/questions/57441301/transactional-annotation-attribute-in-net-core)  

---

## はやらない

概念自体は便利そうだが、流行らないという事はそれなりに理由がある。  

[AOPが不要だと考える理由](https://qiita.com/yamaokunousausa/items/0e99aef0a0b3adb32d08)  
[.NET で独自の AOP を導入したら不便になってしまった話](https://qiita.com/KoKeCross/items/4b16ff8cdad65e0fea23)  

1. 言語機能とフレームワーク  
C#はオブジェクト思考プログラミング言語であり、.NET Frameworkや.NET CoreといったフレームワークがOOPパラダイムに基づいて設計されている。  
そのため、多くの開発者はOOPを主要なプログラミングスタイルとして採用し、AOPはあまり注目されていないことがある。  

2. AOPへの知名度  
AOPは、一部の開発者には人気があるものの、一般的にはまだ充分に広まっていない概念となる。  
そのため、C#開発者の多くがAOPについて充分に理解していないか、または必要性を感じていないことがある。  

3. C#におけるAOPのサポート  
C#と.NET環境には、AOPを実現するためのいくつかのライブラリが存在するが、言語自体に組み込まれたAOP機能は提供されていない。  
そのため、AOPを使用したい開発者は、追加のライブラリを探し、学習し、導入する手間が必要となる。  

4. AOPの複雑さ  
AOPは分離されたコードの横断的な関心事を効果的に管理できる一方、コードの可読性やデバッグの難しさを増すことがある。  
コードがどのように実行されるかを追跡するのが難しくなり、開発者がAOPの利点を享受できるかどうかは、プロジェクトやチームの状況によって異なる。  

---

## C#のアスペクト指向プログラミングが無意味な理由

依存性の注入が行えない。  
2021年の時点でも依存性の注入を実行してアトリビュートを単体テストを実行するためのエレガントな方法がない。  
完全にアトリビュートだけで完結させようとした場合は有料のライブラリを使うか、ある程度妥協する必要がある。  
デコレーションパターンなるモノを使うことで依存性の注入を行うことができる。  
やるにしても結構ガリガリ書く必要がある。  

[Why aspect-oriented programming in C# is pointless](https://www.youtube.com/watch?v=dLPKwEeqwgU)

---

## C# Transaction Attribute

ASP.Net Core ではHTTP通信のミドルウェアでトランザクション制御ができる模様。  
なので、この手法であればコントローラーの中でならTransaction Attributeをつける事ができる。  
が、ビジネス層でやるわけではないので、融通は効かないだろう。  

そもそも独自のAOPを実装する場合はAutofacとかCastle等のパッケージをインストールして開発していくことになる模様。  
その場合、スタートアップにおいて色々やる必要があって、そこまでしてようやく使える用になるので、正直面倒くさい。  

やりたいのはメソッドに`[Transaction]`ってつけたいだけなんだ。  

まぁ、C#で実装されていないなら素直にTransactionScopeを使うべし。  
実装できるかも？という可能性は感じたが、何が何でも実装してやっていきたいというほどではないし、苦労に見合わない。  

ミドルウェアでトランザクション処理を定義する方法を紹介しているところ。  
ざっくり見ると可能性を感じるが、どう頑張ってもControllerの中でしか生きられない。  
[Transaction middleware in ASP.NET Core](https://dev.to/moesmp/transaction-middleware-in-aspnet-core-2608)  

C#でもJavaのようなアトリビュートを！って考える人はやっぱりいるらしく、一番可能性のある記事だったが、やっぱりController限定である。  
下の方にAutofacを使った場合の処理も紹介されているが何故かスルーされている。  
[Transactional annotation attribute in .NET Core](https://stackoverflow.com/questions/57441301/transactional-annotation-attribute-in-net-core)  

少し古いけどRealProxyを使ったTransaction Annotationの実装が紹介されている。  
これを参考に色々やったらできるかもしれないね。  
[透過プロキシでアスペクト指向プログラミング (2)](https://sakapon.wordpress.com/tag/%E9%80%8F%E9%81%8E%E3%83%97%E3%83%AD%E3%82%AD%E3%82%B7/)  

Castle Coreを利用した基本的な実装方法が紹介されている。  
AOPしたいなら基本的なところから入って参考にする分にはいいかもしれない。  
[XamarinでもAOPしたい！　希望編](https://www.nuits.jp/entry/xamarin-fody-01)  

.NetFrameworkでは「RealProxy」.NetCoreでは「DispatchProxy」でAOPを実装するらしい。  
なるほど。  
アスペクト指向的な処理はProxyパターンとしてデザインパターンがあるのか。  
で、RealProxyはProxyパターンをさらに拡張したモノだとか。  
[RealProxyクラスによるアスペクト指向プログラミングに入門してみた。](RealProxyクラスによるアスペクト指向プログラミングに入門してみた。)  

---

## C# AOPライブラリ

代表的なものとして`PostSharp`と`Fody`なるものがあるらしい。  
PostSharpは有料のライブラリだが、小規模なプログラム(1000行以下とか)なら無料で使える。  
ただ、無料で使おうとした場合でも、メールアドレスの登録などは必要そうだったので断念した。  

Fodyは完全無料。  
手っ取り早くAOPしてみたいならこっちでいいんじゃないかな。  

[C#のAOPライブラリ（PostSharp）](https://kouki-hoshi.hatenablog.com/entry/2017/06/21/023159)  
[C#のAOPライブラリ（Fody）](https://kouki-hoshi.hatenablog.com/entry/2017/06/25/011038)  
