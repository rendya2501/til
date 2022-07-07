# Dapper

Dapper は、.NET 環境で使えるシンプルなオブジェクトマッパーです。  
SQLを実行して取得した結果をオブジェクトに対していい感じにマッピングしてくれます。  

Dapper は、ORM(Entity Framework)ほど高性能なことはできません。
Dapper は、IDbConnection というインターフェースを拡張するライブラリです。  
データのマッピングがいい感じでできるようになります。  

[Dapperを使ったデータマッピングのはじめ方](https://webbibouroku.com/Blog/Article/dapper)  

---

## Entity Framework との違い

.NET で使える ORM の一つに Entity Framework があります。  
これはデータベースの情報とオブジェクトを直接マッピングすることで、データベースを意識することなく開発が行えるようになります。
機能的には非常に高性能なのですが、いろいろな面倒事もあったりするので、それが嫌な人は簡単に使える Dapper を検討するとよいでしょう。  

---

## Dapper できること

- データのマッピング  
- DB操作のラッピング  

## Dapper できないこと

- クエリ(SQL)の自動生成  
- マッピングするクラスの自動生成  

Dapperはあくまでオブジェクトへのデータマッピングが主な機能なので、SQLを自動で生成することはできません。  
SQLは自分で書く必要があります。  
同様にマッピングするクラスを自動的に生成することもできません。  

---

## Dapper where in

[SELECT * FROM X WHERE id IN (...) with Dapper ORM](https://stackoverflow.com/questions/8388093/select-from-x-where-id-in-with-dapper-orm)  

``` C#
string sql = "SELECT * FROM SomeTable WHERE id IN @ids"
var results = conn.Query(sql, new { ids = new[] { 1, 2, 3, 4, 5 }});
```

---

## Dapper boolの受け取り

SELECTで `1 AS [Flag]` とか `0 AS [Flag]` と定義して、タプルとかでboolのつもりで受けようとしても取得できない。  
0件になってしまう。  
intと解釈される模様。  
`CONVERT(BIT,'TRUE') AS [Flag]` みたいにしてあげないと取得できないことを知った。  

[SQLServerでboolean型（True/Falseの真偽値）を扱うbit型](https://johobase.com/sqlserver-boolean-bit/)  
