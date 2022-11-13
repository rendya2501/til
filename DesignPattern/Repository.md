# Repositor

---

## 概要

ビジネスロジックとデータベース処理を分離するパターン。  
データベースへのアクセスを担当する中間層を設けるパターン。  

>リポジトリパターンとはビジネスロジックとデータ操作のロジックを分離し、データ操作を抽象化したレイヤに任せるデザインパターンのことです。  
>リポジトリパターンでは、DBの操作や外部APIによるデータ取得等のデータソースへのアクセス部分は Repository インターフェースから完全に隠蔽されます。  
>[リポジトリパターンと Laravel アプリケーションでのディレクトリ構造](https://qiita.com/karayok/items/d7740ab2bd0adbab2e06#:~:text=%E3%83%AA%E3%83%9D%E3%82%B8%E3%83%88%E3%83%AA%E3%83%91%E3%82%BF%E3%83%BC%E3%83%B3%E3%81%A8%E3%81%AF%E3%83%93%E3%82%B8%E3%83%8D%E3%82%B9,%E5%AE%8C%E5%85%A8%E3%81%AB%E9%9A%A0%E8%94%BD%E3%81%95%E3%82%8C%E3%81%BE%E3%81%99%E3%80%82)  

<!--  -->
>リポジトリパターンとは？
>
>Repositoryパターンは、アプリケーションのデータアクセス層とビジネスロジック層の間に抽象化レイヤーを作成するために使用されます。  
>リポジトリはデータアクセス層（DAL）と直接通信し、データを取得し、ビジネスロジック層（BAL）に提供します。  
>[Dapper And Repository Pattern In Web API](https://www.c-sharpcorner.com/article/dapper-and-repository-pattern-in-web-api/)  

---

## Repositoryパターンの目的

>リポジトリパターンを使用する主な利点は、データアクセスロジックとビジネスロジックを分離し、このロジックのいずれかに変更を加えても、他のロジックに直接影響を与えないようにすることです。  
[Dapper And Repository Pattern In Web API](https://www.c-sharpcorner.com/article/dapper-and-repository-pattern-in-web-api/)  

<!--  -->
>Repositoryパターンの目的は、「データアクセス処理」と「ビジネスロジック」を分離することです。  
Mirosoft Docsでは、以下のように記載されています。  
[Repositoryパターン × ノ × チョウサ](https://www.kinakomotitti.net/entry/2018/08/22/223309)  

---

## RepositoryはDB通信・API通信担当

>RepositoryではServiceから指示をうけて、データベースとのやり取りや外部APIとのやり取りを担当します。  
>逆にデータベースとのやり取り・API通信以外のことは書かないでください。  
>たまにDB通信しやすいようにデータを加工する部分をRepositoryに書いてあることがありますが（昔の自分）、それはServiceの担当なので、仕事奪わないであげてください。  
>[Repositoryパターンにおける、MVC + Service + Repositoryの役割をもう一回整理してみる](https://zenn.dev/naoki_oshiumi/articles/0467a0ecf4d56a)  

MVCにおける「Model」「View」「Controller」「Service」「Repository」をわかりやすく解説している。  
詳しくはそちらを参照されたし。  
たいていのアーキテクチャーは、階層の分離と階層の橋渡しで成り立っているので、それさえわかっていれば「スッ」と腑に落ちるだろう。  

---

## DAL(DataAcceccLayer)との違い

>リポジトリ パターンと、従来のデータ アクセス クラス (DAL クラス) パターンの違い  
データ アクセス オブジェクトは、データ アクセスおよび永続化操作を記憶域に対して直接実行します。  
リポジトリでは、メモリ内の作業単位オブジェクトの操作対象データにマークを付けますが (例: DbContext クラスを使用する場合の EF)、更新はデータベースに対してすぐには実行されません。  

`c# dapper data access layer example`  
このキーワードで調べた結果、大体Repositoryパターンのサンプルしか出てこないので、そういう事なのだろう。  

[Data Access Layer Techniques](https://mwellner.de/en/2020/12/29/data-access-layer-techniques/)  
[DAL with dapper and C#](https://stackoverflow.com/questions/31246977/dal-with-dapper-and-c-sharp)  

---

マスター系等、1画面1テーブルに対応したプログラムならRepositoryパターンだけで事足りるかもしれないが、現実はそんな簡単に行かない。  
実務に即して、予約を取得する処理があった場合、トランザクション+複数テーブルへの処理が絶対に必要となる。  
Repositoryパターンだけではそれに対応できない。  
そこでUnit Of Work Patternとの組み合わせになってくる模様。  

Unit Of Workパターンは業務のトランザクションを作業の単位として保持するためのデザインパターンとなる。  
なので、Repositoryを複数まとめて1つの処理としたのがUnit(単位) Of Work(作業)ということになるだろうか。  
因みにこれら全てを実装しているのがEntityFrameworkになるので、Dapperで難しく考えてオレオレフレームワークにするくらいなら素直にEntityFramework使ったほうがいいということになる。  

CRUDプログラムに置けるデータベースへのアクセス及び、トランザクション処理を実現するためのベストプラクティスやアーキテクチャーはないのか探した結果、Repository PatterとUnit Of Work Patterに行きついた。  
さらにCQS/CQRSパターンなど、クエリのまとまりをどうするかといったパターンにまで発展してきた。  

奥が深い。  

[How can I implement a transaction for my repositories with Entity Framework?](https://stackoverflow.com/questions/39906474/how-can-i-implement-a-transaction-for-my-repositories-with-entity-framework)  
[Repositoryパターンのアンチパターン](https://qiita.com/mikesorae/items/ff8192fb9cf106262dbf)  

---

## 参考

一番わかりやすい動画1  
[Repository Pattern](https://www.youtube.com/watch?v=x6C20zhZHw8)  
一番わかりやすい動画2
[Repository Pattern with C# and Entity Framework, Done Right | Mosh](https://www.youtube.com/watch?v=rtXpYpZdOzM)  

WindowFormと絡めた解説動画  
[C# Tutorial - Repository Pattern C# Dependency Injection with Autofac | FoxLearn](https://www.youtube.com/watch?v=XJysyv20pzw)

概念はこの2サイトで十分わかる  
[Repositoryパターンにおける、MVC + Service + Repositoryの役割をもう一回整理してみる](https://zenn.dev/naoki_oshiumi/articles/0467a0ecf4d56a)  
[Repositoryパターン × ノ × チョウサ](https://www.kinakomotitti.net/entry/2018/08/22/223309)  

[microsoft公式_インフラストラクチャの永続レイヤーの設計](https://learn.microsoft.com/ja-jp/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/infrastructure-persistence-layer-design?view=aspnetcore-2.1#the-repository-pattern)  

[Step by Step - Repository Pattern and Unit of Work with Asp.Net Core 5](https://www.youtube.com/watch?v=-jcf1Qq8A-4)  
[Dapper And Repository Pattern In Web API](https://www.c-sharpcorner.com/article/dapper-and-repository-pattern-in-web-api/)  

やたらとパートを区切って解説している動画。  
手っ取り早く実装だけ確認したいならgithubを見ることを進める。  
てか、後で消すかも。  
[Dapper Unit Of Work Pattern With .Net Core Part-1](https://www.youtube.com/watch?v=4nQ00g4QEIw&list=PLHL2ZnB2RiY677SwX4FFGNhRk-M-1SRxd)  
[graciaheys/DapperUnitOfWorkk](https://github.com/graciaheys/DapperUnitOfWorkk)  
