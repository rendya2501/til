# INとEXISTSの動作の違い

IN演算子の後にサブクエリがある場合、SQLはサブクエリから実行される。  
EXISTS演算子の後にサブクエリがある場合でも、メインクエリから実行される。  

---

## IN

``` sql
select * 
from products 
where category in (
    select category 
    from sale
)
```

IN演算子では、IN以降の副問い合わせは入力として処理されるため以下のコードと同様になる模様。  

IN演算子を用いた場合、サブクエリの結果はメモリに展開され、一時テーブルとして記録される。  
そのため、サブクエリのサイズが大きい場合、パフォーマンスの低下を引き起こす可能性がある。  

``` cs
var saleResult = execsql("select category from sale");
for (var i = 0 ; i < saleResult.Count() ; i++) {
    var match = execsql($"select * from products where category = { saleResult[i] }");
    if (match == true) {
        // 結果セットに挿入
    }
}
```

---

## EXISTS

``` sql
select * 
from products 
where exists(
    select * 
    from sale 
    where sale.category = products.category
)
```

EXISTSは相関副問い合わせ(相関クエリ/相関サブクエリ)となり、メインのクエリの結果に対してサブクエリでの評価をするため、コード的には以下のようになる。

``` cs
foreach (var record in execsql("select * from products")) {
    var match = execsql($"select id from sale where sale.category = {record.category}");
    if (match == true) {
        // 結果セットに挿入
    }
}
```

---

[IN 演算子の高速化 | iPentec](https://www.ipentec.com/document/sql-tuning-in-operator)  
