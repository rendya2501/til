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
`INSERT INTO TMa_Route2 (OfficeCD,RouteCD) VALUES ('ALP' AS [AAA],100 AS [BBB])`  
→  
`メッセージ 156、レベル 15、状態 1、行 5 キーワード 'AS' 付近に不適切な構文があります。`  

---

## 列名指定 + SELECTタイプ で列名に対する別名をあべこべに定義した場合

・構文エラーになることはないが、別名は関係なく、左から順番通りに入る。  
・定義していない列にはデフォルト値が入る  

`INSERT INTO TMa_Route2 (OfficeCD,RouteCD) SELECT 'ALP' AS [AAA],100 AS [BBB]`  
→  

``` txt
OfficeCD    RouteCD    RouteName    RouteShortName    ClsCD    Color    BackgroundColor    Sort    ValidFlag    SearchKey    InsertDateTime    InsertStaffCD    InsertStaffName    InsertProgram    InsertTerminal    UpdateDateTime    UpdateStaffCD    UpdateStaffName    UpdateProgram    UpdateTerminal
ALP    100    NULL    NULL    NULL    NULL    NULL    NULL    NULL    NULL    NULL    NULL    NULL    NULL    NULL    NULL    NULL    NULL    NULL    NULL
```

---

## 列名指定なし + VALUESタイプ では列の数が一致しないとエラーになる

`INSERT INTO TMa_Route2 VALUES ('ALP',102)`  
→  
`メッセージ 213、レベル 16、状態 1、行 5 列名または指定された値の数がテーブルの定義と一致しません。`  

列名を指定しない場合は、列数と同じデータを用意しないとエラーになる。  

---

## 列名指定 + VALUESタイプ は列数とVALUESの列数が一致している必要がある

`INSERT INTO TMa_Route2 (OfficeCD,RouteCD) VALUES ('ALP',100,22)`  
→  
`メッセージ 110、レベル 15、状態 1、行 5`  
`VALUES 句で指定された値よりも INSERT ステートメントの列数が少なすぎます。VALUES 句の値の数は、INSERT ステートメントで指定される列数と一致させてください。`  

---

## フィールドのデフォルト値

デフォルト値が入るようになっているテーブルへのINSERTでそのフィールドはずしたらどうなる？  
デフォルト値が入るでしょう。  
→事実入りました。  

FrontCancelFlagがデフォルト値 0 に設定している。
FrontCancelFlagをフィールドから外したINSERT VALUES で見事に初期値が入っている。  
ちなみにnull許可の場合の初期値はnull。当たり前か。  

``` sql
INSERT INTO TRe_ReservationPlayer2(PlayerNo,BusinessDate,SeatNo) VALUES('KHG202299999999','2022-05-20',1)

-- PlayerNo         BusinessDate  SeatNo  FrontCancelFlag  CooperationClsCD
-- KHG202299999999  2022-05-20    1       0                NULL
```
