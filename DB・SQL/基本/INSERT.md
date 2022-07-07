# INSERT

<https://itsakura.com/sql-insert>  

大別して2パターンの組み合わせがわかっていればよい。  

- 列名指定あり・なし  
- VALUESかSELECTか  

``` sql : 基本
-- ●列名指定なし + VALUES
INSERT INTO テーブル名 VALUES ( '値1' [ , '値2' ]・・・);

-- ●列名指定あり + VALUES
INSERT INTO テーブル名 ( テーブルの列名1 [ , テーブルの列名2 ]・・・) VALUES ( '値1' [ , '値2' ]・・・);

-- ●列名指定なし + SELECT (selectの結果をinsertする)
INSERT INTO テーブル名 SELECT 項目名 FROM テーブル名

-- ●列名指定あり + SELECT (selectの指定した結果だけをinsertする)
INSERT INTO テーブル名 ( [テーブルの列名1], [テーブルの列名2]... ) SELECT [項目名1],[項目名2]... FROM 別テーブル名
```

---

## INSERT VALUESの場合の省略構文

``` sql
INSERT INTO [テーブル名]
VALUES 
    ( '値1' [ , '値2' ]・・・), 
    ( '値1' [ , '値2' ]・・・), 
    ( '値1' [ , '値2' ]・・・), 
    ...;
```

---

## 列名指定 + VALUESタイプ で列名をあべこべに設定したらどうなるか

`INSERT INTO テーブル名 ([列1],[列2]) VALUES ('値1' AS [列2], '値2' AS [列1])`  

A. 構文エラーになる。  
VALUES句の中でAS句は使えない模様。  

実行結果  
`INSERT INTO TestTable (TestCode,SubCode) VALUES ('ABC' AS [AAA],100 AS [BBB])`  
→  
`メッセージ 156、レベル 15、状態 1、行 5 キーワード 'AS' 付近に不適切な構文があります。`  

---

## 列名指定 + SELECTタイプ で列名に対する別名をあべこべに定義した場合

・構文エラーになることはないが、別名は関係なく、左から順番通りに入る。  
・定義していない列にはデフォルト値が入る  

`INSERT INTO TestTable (TestCode,SubCode) SELECT 'ABC' AS [AAA],100 AS [BBB]`  
→  

``` txt
TestCode SubCode  Name  Flag  InsertDateTime  UpdateDateTime 
ABC      100      NULL  NULL  NULL            NULL           
```

---

## 列名指定なし + VALUESタイプ では列の数が一致しないとエラーになる

`INSERT INTO TestTable VALUES ('ABC',102)`  
→  
`メッセージ 213、レベル 16、状態 1、行 5 列名または指定された値の数がテーブルの定義と一致しません。`  

列名を指定しない場合は、列数と同じデータを用意しないとエラーになる。  

---

## 列名指定 + VALUESタイプ は列数とVALUESの列数が一致している必要がある

`INSERT INTO TestTable (TestCode,SubCode) VALUES ('ABC',100,22)`  
→  
`メッセージ 110、レベル 15、状態 1、行 5`  
`VALUES 句で指定された値よりも INSERT ステートメントの列数が少なすぎます。VALUES 句の値の数は、INSERT ステートメントで指定される列数と一致させてください。`  

---

## フィールドのデフォルト値

デフォルト値が入るようになっているテーブルへのINSERTでそのフィールドはずしたらどうなる？  
デフォルト値が入るでしょう。  
→事実入りました。  

- TestNo,BusinessDate 必須  
- TestFlag  デフォルト値 0  
- TestClsCD NULL許可  

TestFlagをフィールドから外したINSERT VALUES で見事に初期値が入っている。  
ちなみにnull許可の場合の初期値はnull。当たり前か。  

``` sql
INSERT INTO TestTable(TestNo,BusinessDate) VALUES('ABC202299999999','2022-05-20',1)

-- TestNo           BusinessDate  TestFlag  TestClsCD
-- ABC202299999999  2022-05-20    0         NULL
```

---

## insert intoのinto句のありなしの違い

[insert intoのinto句のありなしって違いは何ですか？](https://detail.chiebukuro.yahoo.co.jp/qa/question_detail/q1049319100)  
>MySQL、SQL Server、ACCESS では、 into は省略可能です。  
Oracle では、省略不可です。  

---

## 自動発番を主キーにした場合のINSERT

IDENTITYフィールドを除けば、どのパターンでもINSERT可能な模様。  

``` sql
-- 列名指定あり → いける
INSERT INTO [IDENTITY_INSERT_TEST_TBL] (Code,[Name]) 
VALUES(1,'A'),(2,'B'),(3,'C');

-- 列名指定なし → いける
INSERT INTO [IDENTITY_INSERT_TEST_TBL] 
VALUES(1,'A'),(2,'B'),(3,'C');

-- 列名指定なし + SELECT → いける
INSERT INTO [IDENTITY_INSERT_TEST_TBL] (Code,[Name]) 
SELECT 1,'A'
INSERT INTO [IDENTITY_INSERT_TEST_TBL] (Code,[Name]) 
SELECT 2,'B'

-- 列名指定なし + SELECT → いける
INSERT INTO [IDENTITY_INSERT_TEST_TBL] 
SELECT 1,'A'
INSERT INTO [IDENTITY_INSERT_TEST_TBL] 
SELECT 2,'B'
```

もちろん、IDENTITYフィールドを含めるとエラーになる。  

``` sql
INSERT INTO [IDENTITY_INSERT_TEST_TBL]
SELECT 1, 2,'B'

-- An explicit value for the identity column in table 'IDENTITY_INSERT_TEST_TBL' can only be specified when a column list is used and IDENTITY_INSERT is ON.
```
