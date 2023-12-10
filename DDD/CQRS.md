# CQRS

CQRS（Command Query Responsibility Segregation、コマンド・クエリ責任分離）は、ソフトウェアアーキテクチャのパターンの一つです。  
このパターンは、アプリケーションの読み取り操作（クエリ）と書き込み操作（コマンド）を明確に分離することを目的としています。  

## CQRSの基本概念

コマンド: システムの状態を変更する操作。例えば、データベースへのデータの追加、更新、削除などが含まれます。  
クエリ: システムからデータを読み取る操作。これはシステムの状態を変更しません。  

## CQRSのメリット

明瞭な責任分担: コマンドとクエリのロジックが分離されているため、システムの構造が明確になり、理解しやすくなります。  
スケーラビリティの向上: 読み取りと書き込みの処理が独立しているため、それぞれに最適なスケーリング戦略を採用できます。  
セキュリティの向上: 読み取り専用の操作と書き込み専用の操作を分けることで、セキュリティポリシーをより細かく制御できます。  

## CQRSの実装

CQRSは、ドメイン駆動設計（DDD）やイベントソーシングと組み合わせて使用されることが多いです。  
イベントソーシングでは、すべての変更をイベントとして記録し、それらのイベントを再生することでシステムの状態を再構築します。  CQRSとイベントソーシングを組み合わせることで、複雑なドメインのロジックをより効果的に管理できるようになります。  

## 使用上の考慮事項

CQRSは多くの場合に有用ですが、すべてのシステムに適しているわけではありません。  
このパターンを採用する際には、追加の複雑さ、データの整合性の維持、パフォーマンスへの影響などを考慮する必要があります。  
シンプルなアプリケーションでは、CQRSの採用が逆にオーバーエンジニアリングになる可能性もあります。  

## どういう時に必要なのか？

複数テーブルを跨いだ情報を取得する場合、リポジトリパターンでは、それぞれのテーブルをGetListして、プログラム上で結合してアレコレすると思われるのだが、それでは無駄が多かったり様々なデメリットが発生してしまうので、読み取りはクエリ、更新系はコマンドとして分離して、複雑な検索処理は専用のクエリとして定義しましょう、責務を分けましょう、という時に使用すると見た。  

参照用モデルと更新用モデルを完全に分ける必要はない。  
部分的な導入も可能。  
「必要なところだけ参照に特化したモデルを導入する」といった使い方が適切と思われる。  

[CQRS実践入門 [ドメイン駆動設計]](https://little-hands.hatenablog.com/entry/2019/12/02/cqrs)  

---

## CQRSのイメージ

``` mermaid
---
title: CQRS
---
flowchart LR
    API -- "HTTP\n(GET/POST/PUT/DELETE)" --> Controller

    Controller -- Query --> A["Query\nHandler"]
    Controller -- Command --> B["Command\nHandler"]

    A["Query\nHandler"] --> Datastroe
    B["Command\nHandler"] --> Datastroe
```

[Implementing CQRS in ASP.NET Core Web API with MediatR - YouTube](https://www.youtube.com/watch?v=bgoGtkmUAQg)  

``` mermaid
---
title: CQRS + Mediator
---
flowchart LR
    API -- "HTTP\n(GET/POST/PUT/DELETE)" --> Controller

    Controller -- Query --> Mediator
    Controller -- Command --> Mediator

    Mediator --> A["Query\nHandler"]
    Mediator --> B["Command\nHandler"]

    A["Query\nHandler"] --> Datastroe
    B["Command\nHandler"] --> Datastroe
```

[ASP.NET Core 7.0 CQRS And MediatR Pattern Implementation, Why We use, Advantages. - YouTube](https://www.youtube.com/watch?v=33QL5eJI9AU)  

---

■ **修正前：標準的なリポジトリパターン**  

- この段階では、InMemoryBookRepository クラスがCRUD操作（作成、読み取り、更新、削除）の全てを担っています。  
- リポジトリはデータソースとして機能し、データへのすべてのアクセスをカプセル化します。  

``` cs
// リポジトリ
public class InMemoryBookRepository {
    private static readonly Dictionary<Guid,Book> Books = new();

    public List<Book> Get() {
        return Books.Values.ToList();
    }

    public Book? GetById(Guid id) {
        return Books.TryGetValue(id, out var book) ? book : default;
    }

    public void Add(Book book) {
        Books.Add(book.Id, book);
    }

    public void Update(Book book) {
        if (!Books.TryGetValue(book.Id, out var existingBook)) {
            return;
        }
        existingBook.Title = book.Title;
        existingBook.Author = book.Author;
        existingBook.NumberOfPages = book.NumberOfPages;
    }

    public void Delete(Guid id) {
        Books.Remove(id);
    }
}

// コントローラー
public static class Books {
    public static void AddBooksEndPoints(this IEndpointRouterBuilder app) {
        app.MapGet("books",(InMemoryBookRepository repository) => {
            return Results.Ok(repository.Get());
        });

        app.MapGet("books/{id}",(Guid id,ImMemoryBookRepository repository) => {
            return Results.Ok(repository.GetById(id));
        });

        app.MapPost("books",(Book book, ImMemoryBookRepository repository) => {
            repository.Add(book);
            return Results.Ok();
        });

        app.MapPut("books",(Book book, ImMemoryBookRepository repository) => {
            repository.Update(book);
            return Results.NoContent();
        });

        app.MapDelete("books/{id}",(Guid id, ImMemoryBookRepository repository) => {
            repository.Delete(id);
            return Results.NoContent();
        });
    }
}
```

■ **修正1：リポジトリの分割**  

- この修正では、リポジトリが読み取り用（BooksReadRepository）と書き込み用（BooksWriteRepository）に分けられています。  
- この変更は、CQRSの初期段階であるコマンドとクエリの責任の分離への第一歩ですが、まだ完全なCQRSパターンではありません。  

``` cs
// データベースのつもり
public static class InMemotyBookRepository {
    public static readonly Dictionary<Guid, Book> Books = new();
}

// 読み取り用リポジトリ
public class BooksReadRepository {
    public List<Book> Get() {
        return InMemotyBookRepository.Books.Values.ToList();
    }

    public Book? GetById(Guid id) {
        return InMemotyBookRepository.Books.TryGetValue(id, out var book) ? book : default;
    }
}

// 書き込み用リポジトリ
public class BooksWriteRepository {
    public void Add(Book book) {
        InMemotyBookRepository.Books.Add(book.Id, book);
    }
    
    public void Update(Book book) {
        if (!InMemotyBookRepository.Books.TryGetValue(book.Id, out var existingBook)) {
            return;
        }
        existingBook.Title = book.Title;
        existingBook.Author = book.Author;
        existingBook.NumberOfPages = book.NumberOfPages;
    }

    public void Delete(Guid id) {
        InMemotyBookRepository.Books.Remove(id);
    }
}

// コントローラー
public static class Books {
    public static void AddBooksEndPoints(this IEndpointRouterBuilder app) {
        app.MapGet("books",(BooksReadRepository repository) => {
            return Results.Ok(repository.Get());
        });

        app.MapGet("books/{id}",(Guid id,BooksReadRepository repository) => {
            return Results.Ok(repository.GetById(id));
        });

        app.MapPost("books",(Book book, BooksWriteRepository repository) => {
            repository.Add(book);
            return Results.Ok();
        });

        app.MapPut("books",(Book book, BooksWriteRepository repository) => {
            repository.Update(book);
            return Results.NoContent();
        });

        app.MapDelete("books/{id}",(Guid id, BooksWriteRepository repository) => {
            repository.Delete(id);
            return Results.NoContent();
        });
    }
}
```

■ **修正2：CQRSの適用**  

- この段階では、リポジトリパターンが完全に排除され、代わりにコマンドとクエリが導入されています。  
- コマンド（AddBookCommand, UpdateBookCommand, DeleteBookCommand）は書き込み操作を担い、クエリ（GetBooksQuery, GetBookByIdQuery）は読み取り操作を担います。  
- これはCQRSの典型的な実装で、コマンドは状態の変更を、クエリはデータの読み取りを担当します。  

``` cs
// データベースのつもり
public static class InMemotyBookRepository {
    public static readonly Dictionary<Guid, Book> Books = new();
}

// 一覧取得クエリ
public class GetBooksQuery {
    public List<Book> Handle() {
        return InMemotyBookRepository.Books.Values.ToList();
    }
}

// 単体取得クエリ
public class GetBookByIdQuery {
    public Book? Handle(Guid id) {
        return InMemotyBookRepository.Books.TryGetValue(id, out var book) ? book : default;
    }
}

// 追加コマンド
public class AddBookCommand() {
    public void Handle(Book book) {
        InMemotyBookRepository.Books.Add(book.Id, book);
    }
}

// 更新コマンド
public class UpdateBookCommand() {
    public void Handle(Book book) {
        if (!InMemotyBookRepository.Books.TryGetValue(book.Id, out var existingBook)) {
            return;
        }
        existingBook.Title = book.Title;
        existingBook.Author = book.Author;
        existingBook.NumberOfPages = book.NumberOfPages;
    }
}

// 削除コマンド
public class DeleteBookCommand {
    public void Handle(Guid id) {
        InMemotyBookRepository.Books.Remove(id);
    }
}

// コントローラー
public static class Books {
    public static void AddBooksEndPoints(this IEndpointRouterBuilder app) {
        app.MapGet("books", () => {
            return Results.Ok(new GetBooksQuery().Handle());
        });

        app.MapGet("books/{id}",(Guid id) => {
            return Results.Ok(new GetBookByIdQuery().Handle(id));
        });

        app.MapPost("books",(Book book) => {
            new AddBookCommand().Handle(book);
            return Results.Ok();
        });

        app.MapPut("books",(Book book) => {
            new UpdateBookCommand().Handle(book);
            return Results.NoContent();
        });

        app.MapDelete("books/{id}",(Guid id) => {
            new DeleteBookCommand().Handle(id);
            return Results.NoContent();
        });
    }
}
```

[Discovering The Truth About CQRS - No MediatR Required - YouTube](https://www.youtube.com/watch?v=F3xNCfP3Xew)  
