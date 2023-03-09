# Dapper Where

条件指定(Where)におけるユースケースをまとめる

---

## Dapper where in

``` C#
string sql = "SELECT * FROM SomeTable WHERE id IN @ids"
var results = conn.Query(
    sql, 
    new { 
        ids = new[] { 1, 2, 3, 4, 5 }
    }
);
```

Dapperが括弧で括ってくれる模様。  

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

## 動的に条件文を組み立てる

例えば、性別が指定されなかった場合は全てのデータを取得し、指定されたときはその性別を取得するという条件を動的に組み立てる場合、SQLの組み立ては以下のように行うと思われる。  

``` cs
var sql = "select * from customers" + (gender != null ? " where gender = @Gender" : "");
```

これは下記のように書き換え可能。  

``` cs
const string sql = @"
select * 
from customers
where (gender = @Gender or @Gender is null)
";
var customers = _connection.Query(sql, new {Gender = (object) null});
```

インデックスが効くかどうかは不明な模様。  

[Dapper のクエリ](https://qiita.com/masakura/items/3409a766e46580a5ad99)  

---

## paramに渡せる型

実践して分かっているのは3つ。  

- 匿名型  
- ExpandoObject  
- ユーザー定義型  

itemsテーブルには以下のデータが入っているものとする。  

|id|name|price|category|
|---|---|---|---|
|1|りんご|190|くだもの|
|2|みかん|100|くだもの|
|3|きゅうり|80|野菜|
|4|人参|110|野菜|
|5|キャベツ|110|野菜|
|6|豚肉|300|肉|
|7|牛肉|400|肉|

### ExpandoObject

直前でobject型への変換が必要。  

``` cs
string constr = @"Server=.\SQLEXPRESS;Database=SandBox;Integrated Security=True;";
using SqlConnection connection = new SqlConnection(constr);

var query = "SELECT * FROM items WHERE price = @price AND category = @category";
dynamic expando = new ExpandoObject();
expando.price = 110;
expando.category = "野菜";

var result = connection.Query<dynamic>(query, (object)expando);
// [0] [object]:
// {{DapperRow, id = '4', name = '人参', price = '110', category = '野菜'}}
// [1] [object]:
// {{DapperRow, id = '5', name = 'キャベツ', price = '110', category = '野菜'}}
```

### ユーザー型

``` cs
string constr = @"Server=.\SQLEXPRESS;Database=SandBox;Integrated Security=True;";
using SqlConnection connection = new SqlConnection(constr);

var query = "SELECT * FROM items WHERE price = @price AND category = @category";
var condition = new Condition()
{
    price = 110,
    category = "野菜"
};

var result = connection.Query<Items>(query,condition);
// [0] [object]:
// {{DapperRow, id = '4', name = '人参', price = '110', category = '野菜'}}
// [1] [object]:
// {{DapperRow, id = '5', name = 'キャベツ', price = '110', category = '野菜'}}

class Condition
{
    public decimal price { get; set; }
    public string category { get; set; }
}
```
