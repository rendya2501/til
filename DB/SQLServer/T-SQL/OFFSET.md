# ページング (OFFSET ~ FETCH)

「n件目からm件目を取得する」といった機能。  
インターネットの検索エンジンサイトの検索結果等でおなじみの10件ずつデータを表示する機能。  

---

## OFFSET ~ FETCH

SQLServer 2012から `OFFSET ~ FETCH` によるページング機能がサポートされるようになった。  
ROW_NUMBERやサブクエリを利用しなくても、n件目 ~ m件目のデータを取得できるようになった。  

``` sql
SELECT * FROM Table
OFFSET n ROWS
FETCH NEXT m ROWS ONLY
```

- OFFSET : スキップしたい件数を指定する。  
- FETCH NEXT : 取得したい件数を指定する。  

---

## 取得例

20までスキップして、21から30までの10個を取得する。

``` sql
WITH SeqTable AS (
    SELECT 1 AS SeqNo
    UNION ALL
    SELECT SeqNo+1 FROM SeqTable WHERE SeqNo < 50
)
SELECT SeqNo 
FROM SeqTable 
ORDER BY SeqNo
OFFSET 20 ROWS           -- 20個スキップして
FETCH NEXT 10 ROWS ONLY  -- 10個取得
/*
21
22
23
24
25
26
27
28
29
30
*/
```

---

## 仕様

OFFSET 句には0以上、FETCH 句には1以上の整数を指定する必要がある。  
それぞれ、負の値、0以下の値を指定するとエラーとなる。  

``` sql
-- OFFSET 句に負の値を指定するとエラー
WITH SeqTable AS (
    SELECT 1 AS SeqNo
    UNION ALL
    SELECT SeqNo+1 FROM SeqTable WHERE SeqNo < 50
)
SELECT SeqNo 
FROM SeqTable 
ORDER BY SeqNo
OFFSET -1 ROWS
FETCH NEXT 10 ROWS ONLY
/*
メッセージ 10742、レベル 15、状態 1、行 35
OFFSET 句のオフセットに負の値を指定することはできません。
*/

-- FETCH 句に0以下の値を指定するとエラー
WITH SeqTable AS (
    SELECT 1 AS SeqNo
    UNION ALL
    SELECT SeqNo+1 FROM SeqTable WHERE SeqNo < 50
)
SELECT SeqNo 
FROM SeqTable 
ORDER BY SeqNo
OFFSET 1 ROWS
FETCH NEXT 0 ROWS ONLY
/*
メッセージ 10744、レベル 15、状態 1、行 36
FETCH 句の行数には 0 より大きい値を指定する必要があります。
*/
```

OFFSET 句は省略不可だが FETCH 句は省略可能。  

``` sql
WITH SeqTable AS (
    SELECT 1 AS SeqNo
    UNION ALL
    SELECT SeqNo+1 FROM SeqTable WHERE SeqNo < 50
)
SELECT SeqNo 
FROM SeqTable 
ORDER BY SeqNo
OFFSET 45 ROWS
/*
46
47
48
49
50
*/
```

50までしかない状態で10個FETCHした場合、46～50までの5個が取得される。  

``` sql
WITH SeqTable AS (
    SELECT 1 AS SeqNo
    UNION ALL
    SELECT SeqNo+1 FROM SeqTable WHERE SeqNo < 50
)
SELECT SeqNo 
FROM SeqTable 
ORDER BY SeqNo
OFFSET 45 ROWS
FETCH NEXT 10 ROWS ONLY
/*
46
47
48
49
50
*/
```

---

## ROW_NUMBERによるページング

サブクエリでROW_NUMBER関数を利用して入社日の新しい順に行番号を取得し、その結果に対してBETWEEN演算子を利用することでn件目からm件目を取得する事ができる。  
下記例では`1 AND 3`で1件目から3件目を意味するので、n件目からm件目を取得した場合はそのまま`n AND m`と記述すれば良い。  

``` sql
SELECT *
FROM 
(
    SELECT
        ROW_NUMBER() OVER (ORDER BY 入社日 DESC) AS rownum,
        *
    FROM 社員
) t
WHERE rownum BETWEEN 1 AND 3
```

---

2023/08/30 Wed 追記  

上下にページングする方法はwindow関数のサンプルの方に置きました。  

``` txt
1-1
1-2
1-3 ← 前へボタンを押したら取得したい
2-1 ← 現在地
2-2 ← 次へボタンを押したら取得したい
2-3
```

---

[SQL Server - OFFSET 句と FETCH 句 - いちろぐ](https://ichiroku11.hatenablog.jp/entry/2014/03/03/003411)  
