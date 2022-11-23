# Dapperメモ

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

---

## DapperAction

リポジトリパターンを実装している時に、参考動画を元にDapperContextクラスを作った。  
コネクションをオープンにして返すだけでDapperとついているが、DapperをUsingしていない。  
そして使用するときはDapperのクエリを呼ぶだけ。  
なら使用と同時にオープンして結果を返すようにしてしまったほうが良いのではないかと思った。  
[Using a Dapper Base Repository in C# to Improve Readability](https://exceptionnotfound.net/using-a-dapper-base-repository-in-c-to-improve-readability/)  
このリンクの通り、Dapperメソッドをラップして、エラー制御とコネクション制御までやってくれている。  
使う側はDapperのラップクラスのメソッドを用途に応じて使用するだけ。  
なんで、この方法を皆取らないのだろうかと不思議に思う。  

リポジトリパターンのサンプルに書くと、少々複雑というか、余計なことになりそうなので、こちらのほうに退避させる。  

``` cs : Program
builder.Service.AddSingleton<DapperContext>();
```

``` cs : DapperContext
using Microsoft.Data.SqlClient;
using System.Data;

public class DapperContext
{
    private readonly IConfiguration _configuration;
    private readonly string _connectionString;

    public DapperContext(IConfiguration configuration)
    {
        _configuration = configuration;
        _connectionString = _configuration.GetConnectionString("DefaultConnection");
    }

    public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
}
```

``` cs
public class BookRepository : IBookRepository
{
    private readonly DapperContext _context;

    public BookRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Book>> GetAllAsync()
    {
        var sql = "SELECT * FROM Book";
        using var connection = _context.CreateConnection();
        var result = (await connection.QueryAsync<Book>(sql)).ToList();
        return result;
    }

    public async Task<int> AddAsync(Book entity){}
    public async Task<int> DeleteAsync(int id){}
    public async Task<Book> GetByIdAsync(int id){}
    public async Task<int> UpdateAsync(Book entity){}
}
```

[Using Dapper with ASP.NET Core Web API](https://www.youtube.com/watch?v=C763K-VGkfc&t=147s)  
[Using a Dapper Base Repository in C# to Improve Readability](https://exceptionnotfound.net/using-a-dapper-base-repository-in-c-to-improve-readability/)  

---

## コンソールアプリでサクッとDapperを使うテンプレート

``` cs
using Dapper;
using System.Data.SqlClient;

string constr = @"Server=.\SQLEXPRESS;Database=<db_name>;Trusted_Connection=True;Trust Server Certificate=true";
using SqlConnection connection = new SqlConnection(constr);
var query = "SELECT * FROM <table>";
var result = connection.Query<dynamic>(query);
foreach (var item in result)
    Console.WriteLine(item.<field_name>);
```

---

## join dynamic

Joinした結果を受け取るPOCOがない。  
もしくは定義するのに値しない。  
サクッと受け取りたい場合どのようにすればよいのか。  

**dynamic**で受けるべし。  

それか素直にTuple作って受け取ればいい。  

``` cs
const string sql = @"
    select 
        Products.ProductId, 
        Products.ProductName, 
        Products.ProductCategory, 
        ProductPrice.Amount, 
        ProductPrice.Currency
    from Products join ProductPrice 
    on Products.ProductId = ProductPrice.ProductId";

// get dynamic
IEnumerable<dynamic> products = sqlConnection.Query(sql);

// get tuple
(int ProductId, string ProductName, byte ProductCategory, int Amount, decimal Currency) tupleProduts = sqlConnection.Query(sql);
```

>最初の例では、テーブルの各行を1つではなく、2つのオブジェクト（商品と仕入先）にマッピングするマルチマッピングを行っていますが、これは商品が返される前に参照によってリンクされます。  
動的なオブジェクトでは、このようなことはできないと思います。  
なぜなら、Dapperは、列をどのように分割すればよいのか、わからないからです。  
一般的なパラメータ`<Product, Supplier, Product>`を`<dynamic, dynamic, dynamic>`に置き換えて、テストで確認することができます。  
[Using dynamic list of objects in dapper join queries](https://stackoverflow.com/questions/35254330/using-dynamic-list-of-objects-in-dapper-join-queries)  

公式でもそうしろって言ってる。  
[github_dapper](https://github.com/DapperLib/Dapper#execute-a-query-and-map-it-to-a-list-of-dynamic-objects)  
