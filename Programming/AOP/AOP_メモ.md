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

---

## C#のアスペクト指向プログラミングが無意味な理由

依存性の注入が行えない。  
2021年の時点でも依存性の注入を実行してアトリビュートを単体テストを実行するためのエレガントな方法がない。  
完全にアトリビュートだけで完結させようとした場合は有料のライブラリを使うか、ある程度妥協する必要がある。  
デコレーションパターンなるモノを使うことで依存性の注入を行うことができる。  
やるにしても結構ガリガリ書く必要がある。  

[Why aspect-oriented programming in C# is pointless](https://www.youtube.com/watch?v=dLPKwEeqwgU)
