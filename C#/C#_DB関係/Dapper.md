# Dapper

---

## 概要

- C#でDBアクセスするためのライブラリ  
  - IDbConnection というインターフェースを拡張するライブラリ  
- .NET 環境で使えるシンプルなORマッパー  
  - SQLを実行して取得した結果をオブジェクトに対していい感じにマッピングしてくれる。  

- ORマッパーと言えばEntity Frameworkが思いつきますが、速度面ではDapperの方が圧倒的に優れている  
- Entity Frameworkほど高性能なことはできない  

[Dapperを使ったデータマッピングのはじめ方](https://webbibouroku.com/Blog/Article/dapper)  
[【C#】Dapperの使い方](https://pg-life.net/csharp/dapper/)  
[Dapper本家](https://dapper-tutorial.net/dapper)  

---

## Entity Framework との違い

- .NET で使える ORM の一つ。  
- データベースの情報とオブジェクトを直接マッピングすることで、データベースを意識することなく開発が行えるようになる。  
- 機能的には非常に高性能だが、いろいろ面倒。  

---

## Dapper できること

- データのマッピング  
- DB操作のラッピング  

## Dapper できないこと

- クエリ(SQL)の自動生成  
- マッピングするクラスの自動生成  

Dapperはあくまでオブジェクトへのデータマッピングが主な機能なので、SQLを自動で生成することはできない。  
SQLは自分で書く必要があります。  
同様にマッピングするクラスを自動的に生成することもできません。  

---

## Dapper のインストール

Nugetで提供されているので、Dapperで検索してインストールすればよろしい。  
因みにアイコンはDという文字を起点になんか色々線が入ってるやつ。  

---

## DapperのSelectサンプル

1. Database1というローカルDBを作成する  
2. ローカルDB上でテーブル生成クエリを流しておく  
3. 適当なクラスを作って、コードを貼り付けて実行する  

``` sql
CREATE TABLE Person(
    [ID] INT,
    [Name] NVARCHAR(100)
)
INSERT INTO [Person] VALUES (1, N'山田'), (2, N'鈴木'), (3, N'佐藤')
```

``` C#
using Dapper;
using System.Data.SqlClient;

    public void Execute()
    {
        SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder
        {
            DataSource = @"(LocalDB)\MSSQLLocalDB",
            AttachDBFilename = System.IO.Path.GetFullPath(@"..\..\Database1.mdf"),
            IntegratedSecurity = true,
        };
        using (var connection = new SqlConnection(builder.ConnectionString))
        {
            connection.Open();
            var query = "SELECT * FROM [Person]";
            // SQLの発行とデータのマッピング
            // 取得データは IEnumerable<Person> 型
            var result = connection.Query<Person>(query);
            foreach (var p in result)
            {
                Console.WriteLine($"{nameof(Person.ID)}: {p.ID}  {nameof(Person.Name)}: {p.Name}");
            }
        }
    }
    
    /// <summary>
    /// マッピング用クラス
    /// </summary>
    class Person
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }
```

---

## Query

複数レコードを取得するメソッド。  
クエリの結果が何もない場合、空のIEnumerableを取得する。  

単純にSELECT文を実行したければこれを使えばよい。  

``` C#
    string sql = "SELECT * FROM [Person]";
    var person = connection.Query<Person>(sql);
```

---

## QueryFirstOrDefault

単一レコードを取得する。  
クエリの結果が何もない場合、nullを取得する。  

Query メソッドで取得した結果にFirstOrDefaultを使っても同じ結果になるが、パフォーマンスが落ちるので、単一のレコードを取得する場合はQueryFirstOrDefaultを使用すると良い。  

``` C#
    string sql = "SELECT * FROM [Person] WHERE [ID] = @ID";
    var param = new { ID = 1 };
    var person = connection.QueryFirstOrDefault<Person>(sql, param);
```

---

## ExecuteScalar

単一レコードの特定の列を取得するメソッド。  
クエリの結果が何もない場合、nullを取得する。  

レコードを取得してから任意のプロパティを取得はできるが、QueryFirstOrDefaultと同様に、パフォーマンスが高くなるため、特定の列を取得する場合はExecuteScalarを使用すると良い。  

``` C#
    string sql = "SELECT [Name] FROM [Person] WHERE [ID] = @ID";
    var param = new { ID = 1 };
    var name = connection.ExecuteScalar<string>(sql, param);
```

---

## Execute

INSERT,UPDATE,DELETE等を実行するためのメソッド。  

---

## トランザクション

Executeメソッドの第三引数にSqlTransactionインスタンスを渡せば良い。  

``` C#
{
    //トランザクション開始
    using (var tran = connection.BeginTransaction())
    {
        try
        {
            var result = connection.Execute(query, param, tran);
            tran.Commit();
        }
        catch (Exception e)
        {
            tran.Rollback();
            throw;
        }
        finally
        {
            connection.Close();
        }
    }
}
```

[C# DapperでDB接続する方法（トランザクション編）](https://learning-collection.com/c-dapper%E3%81%A7db%E6%8E%A5%E7%B6%9A%E3%81%99%E3%82%8B%E6%96%B9%E6%B3%95%EF%BC%88%E3%83%88%E3%83%A9%E3%83%B3%E3%82%B6%E3%82%AF%E3%82%B7%E3%83%A7%E3%83%B3%E7%B7%A8%EF%BC%89/)  

---

## Dapper where in

``` C#
string sql = "SELECT * FROM SomeTable WHERE id IN @ids"
var results = conn.Query(sql, new { ids = new[] { 1, 2, 3, 4, 5 }});
```

[SELECT * FROM X WHERE id IN (...) with Dapper ORM](https://stackoverflow.com/questions/8388093/select-from-x-where-id-in-with-dapper-orm)  

---

## Dapper Like

``` C#
// 案1 CONCAT関数を使用する
string sql = "SELECT * FROM SomeTable WHERE Name LIKE CONCAT('%',@name,'%')"

// 案2 +演算子を使用する
string sql = "SELECT * FROM SomeTable WHERE Name LIKE '%' + @name + '%'"

// 案3 パラメーターで頑張る
var results = conn.Query(sql, new { name = "%" + name + "%" });
```

`LIKE '%' || @name || '%'`も紹介されていたが、SQLServerでは使えなかった。  
`||`演算子による文字列連結はOracleやPostgreSQLで有効な模様。  
SQLServerはもっぱら`CONCAT`か`+`演算子になる模様。  

[Does Dapper support the like operator?](https://stackoverflow.com/questions/6030099/does-dapper-support-the-like-operator)  
[文字列を連結する](https://www.sql-reference.com/string/concatenate.html)  

---

## Dapper boolの受け取り

SELECTで `1 AS [Flag]` とか `0 AS [Flag]` と定義して、タプルとかでboolのつもりで受けようとしても取得できない。  
intとして解釈されるため、型の関係で取得できない模様。  

`CONVERT(BIT,'TRUE') AS [Flag]` のように、ちゃんとコンバートしないと取得できない模様。  

[SQLServerでboolean型（True/Falseの真偽値）を扱うbit型](https://johobase.com/sqlserver-boolean-bit/)  

---

## ToListしないで済ませる

Dapperに流すクエリの中でUNIONしてやればそのひと手間をなくせるのでは？というサンプル。  

``` C#
var code = "aaaaaa";
var stringList = _Dapper.Execute(
    // ①Dapperでデータベースからコード一覧を取得する。
    // WHERE Code = @code
);
// ②addメソッドを使いたいのでToListする
stringList.ToList();
// ③検索条件に使ったcodeをAddすることで1つのコード一覧とする。
stringList.Add(code);
```

上の方法だと、Where条件として使ってるのに、もう一度追加する必要があるのでなんか無駄に感じる。

``` C#
var code = "aaaaaa";
var stringList = _Dapper.Execute(
    // SELECT @code AS code
    // UNION
    // SELECT code FROM table
    // Where Code = @code
)
```

---

## メモ

``` txt
実行するときにコネクションを取得。
終わったら解放。
取得してやるか、Actionを渡してやるか。

一貫してトランザクションを実行したい場合、TransactionScopeのOnScopeの中でやるか。
実務ではそうしている。
だけどOnScopeの中でOnScopeというあれもある。
実務のやつが地味に優秀だったのかなぁ。
あとは、各種Selectの種類と簡単な接続ラップサンプルをやりたい。
```
