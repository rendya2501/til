# IF

## 基本

T-SQLはIFキーワードによる条件分岐を記述可能。  
IFの括弧は省略可能。  

``` sql
IF (条件式)
[BEGIN]
    条件がTrueの場合に実行したいステートメント
[END]
```

``` sql
DECLARE @x int
SET @x = DATEPART(hour,GETDATE())
IF @x < 12
    BEGIN
        PRINT 'ohayou'
    END
```

---

## BEGIN と END の省略

IFの中で実行したいステートメントが1つだけの場合は省略可能。  

実行したいステートメントが2つ以上ある場合に省略すると以下のように解釈されてしまう。  

``` sql
IF @x < 12
    PRINT 'ohayou'
    SELECT * FROM 社員

-- 上記クエリは下記クエリとして解釈される。

IF @x < 12
    BEGIN
        PRINT 'ohayou'
    END

SELECT * FROM 社員
```

---

## IF ~ ELSE

``` sql
IF 条件式
    [BEGIN]
        条件がtrueの場合に実行したいステートメント
    [END]
ELSE
    [BEGIN]
        条件がFALSEの場合に実行したいステートメント
    [END]

```

---

## IF EXISTS , IF NOT EXISTS

IF EXISTS または IF NOT EXISTS を利用すると、SELECTステートメントによる検索結果があるか、ないかで条件分岐をできるようになる。  

``` sql
IF [NOT] EXISTS (SELECT ステートメント)
    [BEGIN]
        データがある場合に実行したいステートメント
    [END]
ELSE
    [BEGIN]
        データがない場合に実行したいステートメント
    [END]
```

---

## なければ実行、あれば何もしないサンプル  

``` sql
IF NOT EXISTS (SELECT TOP 1 1 FROM TestTable WHERE [Code] = 99)
    -- 上記select文の結果がfalseならInsertが実行される。
    INSERT INTO ~~~
GO
```

---

## IF EXISTS での BEGIN END

IF NOT EXISTSでもBEGIN ENDを省略した場合、対象となるクエリは1つだけになる。  
実際にやらかしたのだが、例えばINSERT文を2つ書いた場合、適応されるのは最初のINSERT文だけで2つ目のINSERTはこの条件に関わらず絶対に実行されてしまうことに注意。  

``` sql
IF NOT EXISTS(SELECT TOP 1 1 FROM TestTable WHERE [Code] = 99)
    INSERT INTO ~~~

    -- 最初のINSERTの直下にINSERTを書いても、条件が適応されるされるのは最初のINSERT文だけなので、2つ目のINSERT文は実行されてしまうことに注意する
    INSERT INTO ~~~
GO
```

---

## DROP TABLE IF EXISTS

`DROP TABLE`に`IF EXISTS`を繋げて、テーブルが存在したらDropするように記述することができる。  
この記述方法は**SQLServer2016**から使用可能。  

``` sql
DROP TABLE IF EXISTS テーブル名
```

2016以前では`OBJECT_ID()`関数を使用して、「テーブル名」が存在するかチェックする必要があった。  

``` sql
IF OBJECT_ID (N'テーブル名', N'U') IS NOT NULL  
    --テーブルが見つかったときの処理
ELSE
    --テーブルが見つからなかったときの処理
```

[SQLServer テーブルが存在していたらdropしてcreateする方法](https://note.mokuzine.net/sqlserver-if-exist-table/)  
[【SQL Server】テーブルの存在チェックするやり方を解説します](https://www.tairax.com/entry/Microsoft-SQL-Server/Check-existence-of-table)  
[テーブルなどのデータベースオブジェクトの存在確認](https://johobase.com/exists-database-object-sqlserver/#IF_EXISTS_ELSE)  

---

## CASE

CASE式はSELECTステートメント内など、ステートメントの一部として条件分岐を行うことはできるが、IFのように単独で実行することはできない。  

直接のPRINTは不能  

``` sql
DECLARE @x int = DATEPART(hour,GETDATE())
SELECT
    CASE
        WHEN @x < 12 THEN PRINT 'おはよう'
        WHEN @x < 17 THEN PRINT 'こんにちは'
        ELSE PRINT 'こんばんは'
    END

-- メッセージ 156、レベル 15、状態 1、行 4
-- キーワード 'PRINT' 付近に不適切な構文があります。
-- メッセージ 102、レベル 15、状態 1、行 7
-- 'END' 付近に不適切な構文があります。
```

一度変数に受けて出力するか

``` sql
DECLARE @x int = DATEPART(hour,GETDATE()),@msg varchar(20);
SELECT @msg = 
    CASE
        WHEN @x < 12 THEN 'おはよう'
        WHEN @x < 17 THEN 'こんにちは'
        ELSE 'こんばんは'
    END
PRINT @msg;
```

そのままSELECTする。

``` sql
DECLARE @x int = DATEPART(hour,GETDATE())
SELECT
    CASE
        WHEN @x < 12 THEN 'おはよう'
        WHEN @x < 17 THEN 'こんにちは'
        ELSE 'こんばんは'
    END
```

---

[テーブルなどのデータベースオブジェクトの存在確認](https://johobase.com/exists-database-object-sqlserver/#IF_EXISTS_ELSE)  
[Using cursor to update if exists and insert if not](https://dba.stackexchange.com/questions/218994/using-cursor-to-update-if-exists-and-insert-if-not)  
