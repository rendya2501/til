# UPDATE

>特定テーブルにおける、条件に当てはまるレコードの特定のカラムの値を任意の値に書き換える。  
>[SQLのUPDATE文の書き方あれこれ。 - Qiita](https://qiita.com/ryota_i/items/d17c7630bacb36d26864)  

``` sql
UPDATE テーブル名
SET 列名1 = 値1 [,列名2 = 値2]・・・
WHERE (条件);
```

---

## UPDATE文のset句で副問合せを使用する

``` sql
UPDATE syain
SET name = (
    select name
    from test
    where id = 2)
WHERE id = 2;
```

---

## UPDATE文のwhere句で副問合せを使用する(where in)

テーブルの更新条件を副問い合わせで取ってくる。  

``` sql
UPDATE syain
SET name = 'テスト'
WHERE id in (
    select id from test
    where name = 'TEST'
);
```

[SQL Serverで、SELECT結果でUPDATEする方法 - 蒼の王座・裏口](https://sqlazure.jp/r/sql-server/403/)

---

## UPDATEでCASE式

``` sql
UPDATE
    syain
SET
    name = case id
        when 1 then '鈴木一郎'
        when 2 then '田中二郎'
        when 3 then '佐藤三郎'
    end,
    romaji = case
        when id > 1 then 'tokumei'
        ELSE 'i.suzuki'
    end
WHERE
    id in(1, 2, 3)
```

---

## 別のデータベースの値と副問い合わせを行いUPDATEする

``` sql
UPDATE
    [DataBase1].[dbo].[Product]
SET
    [DataBase1].[dbo].[Product].[TestID] = (
        SELECT
            [TestID]
        FROM
            [DataBase2].[dbo].[Product]
        WHERE
            [DataBase1].[dbo].[Product].[TestClsID] = [DataBase2].[dbo].[Product].[TestClsID]
    )
```

---

## UPDATE JOIN

**SQLServerでJoinしつつUpdateしたい場合、パターン1のように書く必要がある**。  
SELECT FROM JOIN WHERE の基本の流れと同じ。  

肝はUPDATE文に指定するテーブル名は別名でなければいけないということ。  
→  
別に別名でなくてもいけことが分かった。  

``` sql : パターン1
UPDATE
    [main_table]
SET
    [main_table].col1 = [other_table].col1,
    [main_table].col2 = [other_table].col2
FROM
    [main_table]
    JOIN [other_table]
    ON [main_table].id = [other_table].id
```

### UPDATE JOIN の他のパターン

SQLServerではパターン2,パターン3の形式は使えない。  

``` sql : パターン2
UPDATE
    テーブル名1 
    INNER JOIN テーブル名2 
    ON テーブル名1.列名X = テーブル名2.列名X
SET
    テーブル名1.列名1 = テーブル名2.列名2;
```

``` sql : パターン3
-- パターン2と同じ意味
UPDATE
    テーブル名1,テーブル名2
SET
    テーブル名1.列名1 = テーブル名2.列名2
WHERE
    テーブル名1.列名X = テーブル名2.列名X;
```

[SQLServer にて他のテーブルのSELECT結果を利用したUPDATE](https://pg.4696.info/db/mssql/sqlserver-sql.html)  
[UPDATE と JOIN を使ってデータを更新する](https://sql55.com/t-sql/t-sql-update-1.php)  
[SQL Serverで、SELECT結果でUPDATEする方法](https://sqlazure.jp/r/sql-server/403/)  

---

## UPDATEでWITH句を使う

OrderByして最初のレコードだけを更新対象としたいときに、どうしても一回 SELECT FROM 別名 WHERE Seq = 1 で取らないと行けなかった。  
ならサブクエリの部分をWITHに書けば、わざわざ一段階SELECTを挟まないで直接UPDATEまでできるのではないかと考えた。  

結果的に出来たが、移行クエリでやることなので、パット見わかりにくいかなと思ったのと、SELECTで一回取得しても大した量ではなかったのでやめておいた。  
しかし、出来た事に変わりはないので備忘録として残す。  

Withのほうがインデントが浅くなる。  
それだけかもしれない。  
普通に書いたほうが意味も通じる。

``` sql
WITH SQ AS
(
    SELECT
        ROW_NUMBER() OVER(PARTITION BY [Parent].[TestID] ORDER BY [Child].[TestNo]) AS Seq,
        [Child].*
    FROM
        [ParentTable] AS [Parent]
        JOIN [ChildTable] AS [Child]
        ON [Parent].[TestID] = [Child].[TestID]
        AND ISNULL([Parent].[TestFlag],1) = 0
)
UPDATE SQ
SET    TestFlag =1
WHERE  SQ.Seq = 1;
```

普通に書いてもそれなりに分かる。

``` sql
UPDATE [SQ]
SET [SQ].[TestFlag] = 1
FROM
    (
        SELECT
            ROW_NUMBER() OVER(PARTITION BY [Parent].[TestID] ORDER BY [Child].[TestNo]) AS Seq,
            [Child].*
        FROM
            [ParentTable] AS [Parent]
            JOIN [ChildTable] AS [Child]
            ON [Parent].[TestID] = [Child].[TestID]
            AND ISNULL([Parent].[TestFlag],1) = 0
    ) AS [SQ]
WHERE
    [SQ].[Seq] = 1
```

ROW_NUMBER join

そもそも、JOINの条件にROW_NUMBERを使えないかも調べた。  
そしたらWITHで保持する方法を発見したんだ。  
ROW_NUMBERはSELECTでしか使えない見たい。  
その制限がある限り、絶対にワンクッションSELECTしないといけない。  

``` sql
-- 結局、こうやるしかないみたい。
WITH added_row_number AS (
  SELECT
    *,
    ROW_NUMBER() OVER(PARTITION BY year ORDER BY result DESC) AS row_number
  FROM exam_results
)
SELECT *
FROM added_row_number
WHERE row_number = 1;
```

[SQL ROW_NUMBER with INNER JOIN](https://stackoverflow.com/questions/9752361/sql-row-number-with-inner-join)  
[How to Select the First Row in Each GROUP BY Group](https://learnsql.com/cookbook/how-to-select-the-first-row-in-each-group-by-group/)  
[SQLでwith句とupdateを使う方法 サブクエリを共通テーブル式で置き換えるサンプルコード](https://style.potepan.com/articles/30390.html)  
