# Dapper Select

値の取得(Select)におけるユースケースをまとめる

---

## Dapper boolの受け取り

SELECTで `1 AS [Flag]` とか `0 AS [Flag]` と定義して、boolで受けようとしても取得できない。  
intとして解釈されるため、型の関係で取得できない模様。  

`CONVERT(BIT,'TRUE') AS [Flag]` のように、ちゃんとコンバートしないと取得できない模様。  

[SQLServerでboolean型（True/Falseの真偽値）を扱うbit型](https://johobase.com/sqlserver-boolean-bit/)  

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
on Products.ProductId = ProductPrice.ProductId
";

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

---

## カラムと値の出力

[Dapper ORMのQueryでカラム名を取得する方法 – EXCEEDSYSTEM](https://www.exceedsystem.net/2021/11/16/how-to-get-column-names-from-dynamic-type-result-records-using-query-method-of-dapper/)  

---

## ローカルDBでのSelectサンプル

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

``` cs
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
    // @"Data Source=.\MSSQLLocalDB;AttachDbFilename=..\..\Database1.mdf;Integrated Security=True"
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

## Selectサンプル1

こんな受け取り方もできるよっていう備忘録。  
キーバリューに対するSELECTは実質決まっており、それらを動的生成して結果をそのまま受け取る的な。  

``` cs
var con_str = @"Server=.\SQLEXPRESS;Database=SandBox;Integrated Security=True;";
using SqlConnection connection = new SqlConnection(con_str);

var query = @"
    SELECT A,B,C
    FROM (
        VALUES
            (1,1,(SELECT 'Hoge')),
            (1,2,(SELECT 'Fuga')),
            (2,1,(SELECT 'Piyo'))
    ) AS T(A,B,C)
";
var result = connection.Query<(int A, int B, string C)>(query);

var list = new List<(int A, int B)>() {
    (1,1),
    (1,2),
    (1,3),
    (2,1),
    (2,2),
};
_ = list.GroupJoin(
    result,
    l => new { l.A, l.B },
    r => new { r.A, r.B },
    (l, r) => new
    {
        l.A,
        l.B,
        C = r.FirstOrDefault().C ?? "NULLです"
    }
);

// [0]:{ A = 1, B = 1, C = "Hoge" }
// [1]:{ A = 1, B = 2, C = "Fuga" }
// [2]:{ A = 1, B = 3, C = "NULLです" }
// [3]:{ A = 2, B = 1, C = "Piyo" }
// [4]:{ A = 2, B = 2, C = "NULLです" }
```
