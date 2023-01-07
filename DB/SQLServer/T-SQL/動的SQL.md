# 動的SQL (EXEC, sp_executesql)

## 概要

動的SQLは動的に(ローカル変数の値に応じて)SQLを組み立てて実行する機能。  
`EXECUTE`ステートメントまたは`sp_executesql`システムストアドプロシージャを利用して実行することができる。  

これにより、SQLステートメントの一部をパラメーター化して実行できるようになる。  

---

## テーブル名や列名の変数化(パラメーター化)

FROMの後に記述できるローカル変数はテーブル変数のみで、通常のローカル変数を指定する事ができない。  
テーブル名や列名を変数化したい場合には、そのままでは利用できないため、動的SQLを利用しなければならない。  

---

## EXECUTEステートメントによる動的SQL

``` sql
EXECUTE ( {'文字列' | ローカル変数})
```

EXECUTEの後に、カッコを記述して、実行したいSQLステートメントの文字列またはローカル変数を指定する。  
「EXECUTE」は「`EXEC`」と省略することも可能。  

``` sql
EXEC ('SELECT * 社員')
```

テーブル名を変数「@tabName」へ格納してSELECTステートメントを実行する。  
テーブル名を変数化(パラメーター化)したい場合には、文字列として組み立てて、EXECUTEステートメントで動的SQLとして実行するようにする。
列名に関しても同様。  

``` sql
DECLARE @tabName varchar(20)
SELECT @tabName = '社員'
EXEC ('SELECT * FROM ' + @tabName)
```

---

## sp_executesqlによる動的SQL

動的SQLは「`sp_executesql`」システム ストアドプロシージャを利用しても実行する事ができる。  

EXECUTEステートメントとの違いは、完成形の(文字列連結が完了した)SQLステートメントをローカル変数として与えている点。  
EXECUTEステートメントでは、引数の中で文字列連結を行う事ができたが、sp_executesqlで同じような記述をした場合、エラーとなる。  

sp_excecutesqlでは、文字列連結が完了したSQLステートメントを引数へ与える必要があることに注意する。  

``` sql
[EXECUTE] sp_executesql N'文字列' | ローカル変数
```

sp_executesqlでは、Nプレフィックスをつけた文字列またはローカル変数として、実行したいSQLステートメントを指定する。  
(NプレフィックスはUnicodeデータであることを明示するためのもの)。  
先頭のEXECUTE(EXECでも可)は、sp_executesqlをバッチの先頭で実行する場合には省略する事ができる。  

``` sql
DECLARE @sql nvarchar(100), @tabName varchar(20)
SELECT @tabName = '社員'
SELECT @sql = N'SELECT * FROM ' + tabName
EXEC sp_executesql @sql
```

``` sql
sp_executesql N'SELECT * FROM 社員'
```

## sq_executesqlでのパラメーター化

sp_executesqlとEXECUTEステートメントとの一番の違いは、sp_executesqlではSQLのパラメーター化が行える点にある。  
次のようにWHERE句の条件式で値を指定する部分で利用することができる。  

``` sql
sp_executesql N'SELECT … FROM …
    WHERE 列1 = @パラメーター1 … 列2 = @パラメーター2 …'
    ,N'@パラメーター1 データ型, @パラメーター2 データ型, …'
    ,@パラメーター1 = 代入したい値, @パラメーター2 = 代入したい値, …
```

ローカル変数と同じように@を付けてパラメーターを記述し、第2引数へパラメーターの定義(データ型の指定)、第３引数以降でパラメータへ代入したい値を指定する。  

次のクエリは「`氏名 LIKE '%田%' AND 給与 > 400000`」と解釈されて実行される。  
パラメータ2つ(@p1と@p2)のデータ型を第2引数で定義し、(varchar(50とint型)、代入する値を第3引数以降で指定している。  

``` sql
sq_executesql N'SELECT * FROM 社員 WHERE 氏名 LIKE @1 AND 給与 > @2'
    ,N'@p1 varchar(50), @p2 int'
    ,@p1 = '%田%', @p2 = 400000
```

## sq_executesqlではテーブル名や列名のパラメーター化はできない

sp_executesqlでは、テーブル名や列名のパラメーター化を行うことはできないため、次のような実装方法はエラーになる。  

``` sql
sp_executesql SELECT * FROM @tabName WHERE 氏名 LIKE @1 AND 給与 > @2
    ,N'@tabName varchar(20), @p1 varchar(50), @p2 int'
    ,@tabName = '社員', @p1 = '%田%', @p2 = 400000

-- メッセージ 1087、レベル 15、状態 2、行 23
-- テーブル変数 "@tabName" を宣言してください。
```

パラメーター化はWHERE句の条件式で値を指定する部分のみで利用する事ができる。  
従って、テーブル名や列名をパラメーター化したい場合には、次のように文字列として組み立てなければならない。  

``` sql
DECLARE @sql nvarchar(100), @tabName varchar(20) = '社員'
SELECT @sql = N'SELECT * FROM ' + @tabName + ' WHERE 氏名 LIKE @1 AND 給与 > @2'
EXEC sp_executesql @sql
    ,N'@p1 varchar(50), @p2 int'
    ,@p1 = '%田%', @p2 = 400000
```
