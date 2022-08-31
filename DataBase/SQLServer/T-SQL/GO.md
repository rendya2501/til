# GOコマンド

---

## 概要

公式曰く  

- sqlcmd ユーティリティ、osql ユーティリティ、および SQL Server Management Studio のコード エディターによって認識されるコマンド。  
- C#のSqlDataAdapterを使ってクエリを発行した時は、このGOは認識されない。  
- T-SQLではない。  
- GO は、Transact-SQL ステートメントのバッチの終了を SQL Server ユーティリティに通知する。  

[SQL Server のユーティリティのステートメント - GO](https://docs.microsoft.com/ja-jp/sql/t-sql/language-elements/sql-server-utilities-statements-go?view=sql-server-ver16)  

---

## 気を付けること

実際にあったのでメモする。  

変数を使うときは注意が必要。  
変数を事前に定義していても、GOコマンドを実行すると、それ以降の行ではその変数は有効でなくなってしまう。  
GOコマンドは何かしらの区切り的な意味らしい。  

``` sql
DECLARE @test varchar(10) = 'hogehoge'

SELECT @test
GO

-- スカラー変数 "@test" を宣言してください。
SELECT @test
```

[SQL ServerのGOコマンドとは？](https://sql-oracle.com/sqlserver/?p=708)  
[GOとセミコロンってなんだろうか](https://sotoattanito.hatenablog.com/entry/2015/10/08/230340)  
