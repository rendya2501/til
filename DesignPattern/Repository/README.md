# Repository Pattern

---

## 概要

ビジネスロジックとデータベース処理を分離するパターン。  
データベースへのアクセスを担当する中間層を設けるパターン。  

データベースへのアクセス処理等をまとめる。  
ロジック層はデータベースへのアクセスを気にする必要がなくなる。  
煩雑なDBアクセスのロジックを書く必要がなくなる。  
本来の業務ロジックに集中する事ができる。  

リポジトリパターンはドメイン駆動開発(DDD)やUnitOfWork,CQRSなど複数の概念と密接に関係しているパターン。  
このパターン単体ではなく、他のパターンや概念を理解した上で使っていくのがベスト。  

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
[Repositoryパターン × ノ × チョウサ](https://www.kinakomotitti.net/entry/2018/08/22/223309)  

---

## RepositoryはDB通信・API通信担当

>RepositoryではServiceから指示をうけて、データベースとのやり取りや外部APIとのやり取りを担当します。  
>逆にデータベースとのやり取り・API通信以外のことは書かないでください。  
>たまにDB通信しやすいようにデータを加工する部分をRepositoryに書いてあることがありますが（昔の自分）、それはServiceの担当なので、仕事奪わないであげてください。  
>[Repositoryパターンにおける、MVC + Service + Repositoryの役割をもう一回整理してみる](https://zenn.dev/naoki_oshiumi/articles/0467a0ecf4d56a)  

MVCにおける「Model」「View」「Controller」「Service」「Repository」をわかりやすく解説している。  
詳しくはそちらを参照されたし。  
たいていのアーキテクチャーは、階層の分離と階層の橋渡しで成り立っているので、それさえわかっていれば腑に落ちるだろう。  

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

## Entity Framework を使うならRepository PatternやUnitOfWork Patternは必要ない?

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

## 実装1

EntityFrameworkなど、CRUDを共通化できるならこちらの方式を採用しても問題ない。  
Dapper等は実装2のほうが楽かと思われる。  

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

- クラス図参考  
  - [Repository Pattern Implementation](https://www.dotnettricks.com/learn/mvc/implementing-repository-and-unit-of-work-patterns-with-mvc)  

``` cs
public interface IRepository<TEntity> where TEntity : class{}

public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : class{}

public interface IServiceRepository : IRepository<TEntity>{}

public class ServiceRepository : Repository<TEntity>, IServiceRepository{}
```

- 実装参考
  - [Repository and Unit of work pattern in ASP.net core](https://pradeepl.com/blog/repository-and-unit-of-work-pattern-asp-net-core-3-1/)  
  - [How to use Dapper with ASP.NET Core and Repository Pattern](https://blog.christian-schou.dk/how-to-use-dapper-with-asp-net-core/)  
  - [Dapper in ASP.NET Core with Repository Pattern – Detailed](https://codewithmukesh.com/blog/dapper-in-aspnet-core/)  
  - [Repository Pattern in ASP.NET Core – Ultimate Guide](https://codewithmukesh.com/blog/repository-pattern-in-aspnet-core/)  
  - [Generic repository pattern using Dapper](https://tacta.io/en/news/generic-repository-pattern-using-dapper/20)  
  - [Using Dapper with ASP.NET Core Web API](https://www.youtube.com/watch?v=C763K-VGkfc&t=147s)  

- フォルダ構成  
  - [ASP.NET MVC Architecture: Repository Pattern](https://code.sweetmustard.be/dotnet/asp-mvc-repository-pattern/)  
  - [ASP.NET Core 6 Three Tier Architecture](https://www.youtube.com/watch?v=j2AYkOSzTUw)  
  - [自分が現状気に入っているアプリケーションのパッケージ構成をさらす](https://qiita.com/os1ma/items/286eeec028e30e27587d)  

---

## 実装2

EntityFrameworkなど、CRUDを共通化できるならabstractのRepositoryClassとして定義できるかもしれないが、Dapper等、生でクエリを書く場合、CRUDを共通化するのは難易度が高くなる。  
調べた感じ、SELECTのテーブル名の指定をどうするか？とか、全てのテーブルに対して1つINSERT文で事足りるのか？とか色々ありそうではあった。  
そういう時は、共通処理をInterfaceで定義して、各RepositoryInterfaceで共通Interfaceを実装する形もありなのかもしれないと思った。  

``` mermaid
classDiagram
direction BT

class IRepository {
    <<interface>>
}
class IServiceRepository{
    <<interface>>
}
class ServiceRepository{
    
}

IServiceRepository --> IRepository : implemente
ServiceRepository ..|> IServiceRepository : implemente
```

``` cs
public interface IRepository<TEntity> where TEntity : class{}

public interface IServiceRepository : IRepository<TEntity>{}

public class ServiceRepository : IServiceRepository{}
```

[Generic repository pattern using Dapper](https://tacta.io/en/news/generic-repository-pattern-using-dapper/20)  

---

## 汎用インターフェース

>汎用 Repoitory インタフェースを定義したい気持ちは理解できますが、管理人的にはイマイチ納得できません。  
>納得できない理由として Delete や Update はトランザクション系のデータには不要な場合も多々ありますし、キーを指定してデータを取得する GetById メソッドは文字列のキーにしか対応できない、複合キーにも対応できない等…インタフェースとしてして定義するには不完全だと思うからです。  
>
>但し、汎用 Repoitory インタフェースを全否定している訳ではなくマスタ系データのように汎用 Repoitory インタフェースを定義した方が良い場合もあるので、その辺りは臨機応変にすべきだと思っています。  
>[ようこそ Dapper 至上主義の DataAccess へ【#5 WPF MVVM L@bo】](https://elf-mission.net/programming/wpf/mvvm-labo/phase05/#DataAccess_Repository)  

ログテーブルの操作とか、INSERTしかないパターンもあるので、汎用的なインターフェースを実装しないパターンもあり得るのかもしれない。  
その場合は、汎用インターフェースを実装せず、個別でインターフェースを定義するのもありだろうか。  

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

---

## アンチパターン

- 機能やロールでRepositoryを分ける  
- 子テーブルに対して沢山Repositoryを作る  
- 複雑なクエリをRepositoryで頑張って発行する  

[Repositoryパターンのアンチパターン](https://qiita.com/mikesorae/items/ff8192fb9cf106262dbf)  

---

## AIに実装させたやつ

EntityはBookとCatalogueとして、リポジトリパターンのサンプルを記述してください。
C#10,Dapperを使用してください。
BookとCatalogueにはIRepositoryを実装したIBookRepositoryとICatalogueRepositoryを実装してください。
IRepositoryの実装もしてください。
WebAPIとして実装してください。

``` cs
// IRepository インターフェイス
public interface IRepository<T>
{
    IEnumerable<T> GetAll();
    T GetById(int id);
    void Insert(T entity);
    void Update(T entity);
    void Delete(T entity);
}

// IBookRepository
public interface IBookRepository : IRepository<Book>
{
    // ここにBook固有のメソッドを記述
}

// ICatalogueRepository
public interface ICatalogueRepository : IRepository<Catalogue>
{
    // ここにCatalogue固有のメソッドを記述
}
```

``` cs
// IBookRepository の実装
public class BookRepository : IBookRepository
{
    private readonly IDbConnection _connection;

    public BookRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public IEnumerable<Book> GetAll()
    {
        return _connection.Query<Book>("SELECT * FROM Books");
    }

    public Book GetById(int id)
    {
        return _connection.QueryFirstOrDefault<Book>("SELECT * FROM Books WHERE Id = @id", new {id});
    }

    public void Insert(Book entity)
    {
        _connection.Execute("INSERT INTO Books (Name, Author) VALUES(@Name, @Author)", new { entity.Name, entity.Author });
    }

    public void Update(Book entity)
    {
        _connection.Execute(
          "UPDATE Books SET Name = @Name, Author = @Author WHERE Id = @Id",
          new { entity.Name, entity.Author, entity.Id }
        );
    }

    public void Delete(Book entity)
    {
        _connection.Execute("DELETE FROM Books WHERE Id = @Id", new { entity.Id });
    }
}

// ICatalogueRepository の実装
public class CatalogueRepository : ICatalogueRepository
{
    private readonly IDbConnection _connection;

    public CatalogueRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public IEnumerable<Catalogue> GetAll()
    {
        return _connection.Query<Catalogue>("SELECT * FROM Catalogues");
    }

    public Catalogue GetById(int id)
    {
        return _connection.QueryFirstOrDefault<Catalogue>("SELECT * FROM Catalogues WHERE Id = @id", new {id});
    }

    public void Insert(Catalogue entity)
    {
        _connection.Execute(
          "INSERT INTO Catalogues (Name, Description) VALUES(@Name, @Description)", 
          new { entity.Name, entity.Description }
        );
    }

    public void Update(Catalogue entity)
    {
        _connection.Execute(
          "UPDATE Catalogues SET Name = @Name, Description = @Description WHERE Id = @Id", 
          new { entity.Name, entity.Description, entity.Id }
        );
    }

    public void Delete(Catalogue entity)
    {
        _connection.Execute("DELETE FROM Catalogues WHERE Id = @Id", new { entity.Id });
    }
}
```

``` cs : WebAPI
public class BooksController : ApiController
{
    private readonly IBookRepository _bookRepository;

    public BooksController(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    // GET api/books
    public IEnumerable<Book> Get()
    {
        return _bookRepository.GetAll();
    }

    // GET api/books/1
    public Book Get(int id)
    {
        return _bookRepository.GetById(id);
    }

    // POST api/books
    public void Post([FromBody]Book book)
    {
        _bookRepository.Insert(book);
    }

    // PUT api/books/1
    public void Put(int id, [FromBody]Book book)
    {
        book.Id = id;
        _bookRepository.Update(book);
    }

    // DELETE api/books/1
    public void Delete(int id)
    {
        _bookRepository.Delete(_bookRepository.GetById(id));
    }
}

public class CataloguesController : ApiController
{
    private readonly ICatalogueRepository _catalogueRepository;

    public CataloguesController(ICatalogueRepository catalogueRepository)
    {
        _catalogueRepository = catalogueRepository;
    }

    // GET api/catalogues
    public IEnumerable<Catalogue> Get()
    {
        return _catalogueRepository.GetAll();
    }

    // GET api/catalogues/1
    public Catalogue Get(int id)
    {
        return _catalogueRepository.GetById(id);
    }

    // POST api/catalogues
    public void Post([FromBody]Catalogue catalogue)
    {
        _catalogueRepository.Insert(catalogue);
    }

    // PUT api/catalogues/1
    public void Put(int id, [FromBody]Catalogue catalogue)
    {
        catalogue.Id = id;
        _catalogueRepository.Update(catalogue);
    }

    // DELETE api/catalogues/1
    public void Delete(int id)
    {
        _catalogueRepository.Delete(_catalogueRepository.GetById(id));
    }
}
```

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

時間があれば公式のドキュメントを読み漁りたい。
[microsoft公式_インフラストラクチャの永続レイヤーの設計](https://learn.microsoft.com/ja-jp/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/infrastructure-persistence-layer-design?view=aspnetcore-2.1#the-repository-pattern)  

実装動画
[Step by Step - Repository Pattern and Unit of Work with Asp.Net Core 5](https://www.youtube.com/watch?v=-jcf1Qq8A-4)  

[Dapper And Repository Pattern In Web API](https://www.c-sharpcorner.com/article/dapper-and-repository-pattern-in-web-api/)  

transaction scopeを使った例が乗っている  
[Generic repository pattern using Dapper](https://tacta.io/en/news/generic-repository-pattern-using-dapper/20)  

Dapper以前のADOでやっている動画だが、割と参考になる。  
何よりスキャフォールドで一瞬でrazorのページを作っている。  
[ASP.NET Core 6 with ADO.Net + Repository Pattern](https://www.youtube.com/watch?v=N22gKbrLgK0&t=441s)  
