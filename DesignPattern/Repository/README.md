# Repository Pattern

---

## 概要

ビジネスロジックとデータベース処理を分離するパターン。  
データベースへのアクセスを担当する中間層を設けるパターン。  

>リポジトリパターンとはビジネスロジックとデータ操作のロジックを分離し、データ操作を抽象化したレイヤに任せるデザインパターンのことです。  
>リポジトリパターンでは、DBの操作や外部APIによるデータ取得等のデータソースへのアクセス部分は Repository インターフェースから完全に隠蔽されます。  
>[リポジトリパターンと Laravel アプリケーションでのディレクトリ構造](https://qiita.com/karayok/items/d7740ab2bd0adbab2e06#:~:text=%E3%83%AA%E3%83%9D%E3%82%B8%E3%83%88%E3%83%AA%E3%83%91%E3%82%BF%E3%83%BC%E3%83%B3%E3%81%A8%E3%81%AF%E3%83%93%E3%82%B8%E3%83%8D%E3%82%B9,%E5%AE%8C%E5%85%A8%E3%81%AB%E9%9A%A0%E8%94%BD%E3%81%95%E3%82%8C%E3%81%BE%E3%81%99%E3%80%82)  

<!--  -->
>Repositoryパターンは、アプリケーションのデータアクセス層とビジネスロジック層の間に抽象化レイヤーを作成するために使用されます。  
>リポジトリはデータアクセス層（DAL）と直接通信し、データを取得し、ビジネスロジック層（BAL）に提供します。  
>[Dapper And Repository Pattern In Web API](https://www.c-sharpcorner.com/article/dapper-and-repository-pattern-in-web-api/)  

<!--  -->
>リポジトリ パターンは、ドメインおよびデータ アクセス層 (Entity Framework Core / Dapper など) との間でデータを仲介する設計パターンです。  
>リポジトリは、データの保存または取得に必要なロジックを隠すクラスです。  
>したがって、ORM に関連するものはすべてリポジトリ レイヤー内で処理されるため、アプリケーションは使用している ORM の種類を気にしません。  
>これにより、懸念事項をより明確に分離できます。  
>リポジトリ パターンは、よりクリーンなソリューションを構築するために頻繁に使用されるデザイン パターンの 1 つです。  
>[Repository Pattern in ASP.NET Core – Ultimate Guide](https://codewithmukesh.com/blog/repository-pattern-in-aspnet-core/)  

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

## Entity Framework を使うならRepository PatternやUnitOfWork Patternは必要ない

>DbContextクラスはUnitOfWorkとRepositoryを混在させて提供しています。  
>DbContext の `DbSet<Something>` プロパティに対するコンテナに過ぎないリポジトリクラスは必要ないのです。  
>また、SaveChanges() の呼び出しをラップするための新しいクラスも必要ない。  
>
>[No need for repositories and unit of work with Entity Framework Core](https://gunnarpeipman.com/ef-core-repository-unit-of-work/)  

世界一の企業が実装しているのだから、それくらいのアーキテクチャーは既に採用済み。  
なので、EFCoreを使う場合、改めてRepository PatternやUnit Of Work Patternを実装する必要はない。  

逆にRepository Patternを再実装してUPDATEやDELETEメソッドで`_dataContext.SaveChanges()`を実行した場合、それは他の全ての変更にも影響してしまうのでやめたほうがいいとのこと。  

やるならDbContextで色々やるべし。  

>さらに良いことに、カスタムデータベースコンテキストは私たちが書いたクラスです。  
>EF CoreのDbContextを継承しているので、カスタムコンテキストを構築する上でかなり自由度が高いです。  
>これはコンテキストの単位として働くこともできます。また、必要であれば、ワンショットレポジトリとしても機能します。  
>
>DbContextの機能をラップするだけで、新しい価値を追加しないのであれば、独自のユニットオブワークやリポジトリを実装する必要はありません。  
>すでにあるものを使うというシンプルな考えです。  

<!--  -->
>Microsoft は、Repository パターンと Unit of Work パターンを使用して Entity Framework Core を構築しました。  
>では、データ アクセスのさらに別の抽象化である Entity Framework Core に別の抽象化レイヤーを追加する必要があるのはなぜですか。  
>Microsoft 自身は、複雑なシナリオでリポジトリ パターンを使用して結合を減らし、ソリューションのテスト容易性を向上させることを推奨しています。  
>可能な限り単純なコードが必要な場合は、リポジトリ パターンを避けた方がよいでしょう。  
>[Repository Pattern in ASP.NET Core – Ultimate Guide](https://codewithmukesh.com/blog/repository-pattern-in-aspnet-core/)  

---

## クラス図

``` mermaid
classDiagram
direction BT

class IRepository {
    <<interface>>
}
class Repository{
    <<abstract>>
}
class IServiceRepository{
    <<interface>>
}
class ServiceRepository{
    
}

Repository ..|> IRepository : implemente
ServiceRepository ..|> IServiceRepository : implemente
ServiceRepository --> Repository : inheritance
IServiceRepository --> IRepository : inheritance
```

[Repository Pattern Implementation](https://www.dotnettricks.com/learn/mvc/implementing-repository-and-unit-of-work-patterns-with-mvc)  

---

## 実装

``` cs
public interface IRepository<TEntity> where TEntity : class{}

public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : class{}

public interface IServiceRepository : IRepository<TEntity>{}

public class ServiceRepository : Repository<TEntity>, IServiceRepository{}
```

- 実装参考
  - [Repository and Unit of work pattern in ASP.net core](https://pradeepl.com/blog/repository-and-unit-of-work-pattern-asp-net-core-3-1/)  
  - [Implementing Unit Of Work Pattern](https://social.msdn.microsoft.com/Forums/en-US/b2c68f7e-3cbd-435a-a7bc-a71227f2d47e/implementing-unit-of-work-pattern?forum=csharpgeneral)  
  - [How to use Dapper with ASP.NET Core and Repository Pattern](https://blog.christian-schou.dk/how-to-use-dapper-with-asp-net-core/)  
  - [Dapper in ASP.NET Core with Repository Pattern – Detailed](https://codewithmukesh.com/blog/dapper-in-aspnet-core/)  
  - [Repository Pattern in ASP.NET Core – Ultimate Guide](https://codewithmukesh.com/blog/repository-pattern-in-aspnet-core/)  
  - [Generic repository pattern using Dapper](https://tacta.io/en/news/generic-repository-pattern-using-dapper/20)  
  - [Using Dapper with ASP.NET Core Web API](https://www.youtube.com/watch?v=C763K-VGkfc&t=147s)  

---

## Unit Of Work パターンのすゝめ

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

[Dapper とリポジトリ パターンを使用した ASP.Net Core Web Api CRUD](https://www.youtube.com/watch?v=3moKgzS7AWo)  

transaction scopeを使った例が乗っている  
[Generic repository pattern using Dapper](https://tacta.io/en/news/generic-repository-pattern-using-dapper/20)  
