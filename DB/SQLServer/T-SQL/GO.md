# GOコマンド

---

SQLServerに対してまとめて処理させたいステートメントの"塊"を**バッチ**と呼ぶ。  
クエリエディターで選択した範囲がバッチとなる。  
選択していない場合はクエリエディター内へ記述した全てのステートメントが1つのバッチとして扱われる。  
そのバッチを区切るためのコマンドが**goコマンド**。(goコマンドがバッチ終了の合図)  

従って、次のようにDECLAREによる変数宣言の後にgoを記述した場合、「変数宣言がされていない」という主旨のエラーが発生する。  

``` sql
DECLARE @x int;
go

SELECT @x = 77;
SELECT @x;
```

---

## 概要

GOコマンドはSQLServerにおける処理の区切り的な意味らしい。  
T-SQLではない模様。  

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

``` sql
DECLARE @test varchar(10) = 'hogehoge'

SELECT @test
GO

-- スカラー変数 "@test" を宣言してください。
SELECT @test
```

後、GOの最後にコロンをつけてはいけない。  
場合によってエラーになる。  

これはOK

```sql
DECLARE @test varchar(10) = 'hogehoge';
SELECT @test;
GO
```

これはダメ。  

```sql
DECLARE @test varchar(10) = 'hogehoge';
SELECT @test;
GO;
-- メッセージ 102、レベル 15、状態 1、行 7
-- 'GO' 付近に不適切な構文があります。
```

[SQL ServerのGOコマンドとは？](https://sql-oracle.com/sqlserver/?p=708)  
[GOとセミコロンってなんだろうか](https://sotoattanito.hatenablog.com/entry/2015/10/08/230340)  
