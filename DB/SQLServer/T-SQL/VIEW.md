# View

## 概要

SELECTステートメントを1つのパッケージとして保存する機能。  
ビューはテーブルと同じように利用できるので"仮想表"とも呼ばれる。  
作成したビューはオブジェクトエクスプローラーの[ビュー]フォルダーで確認できる。  

``` sql
CREATE VIEW ビュー名
AS
SELECT ステートメント
```

---

## 作成例

``` sql
CREATE VIEW 受注商品一覧
AS
SELECT 受注.受注コード,受注日,受注明細,単価,数量,商品名,区分名
FROM 受注明細
INNER JOIN 受注 ON 受注明細.受注コード = 受注.受注コード
INNER JOIN 商品 ON 受注明細.商品コード = 商品.商品コード
INNER JOIN 商品区分 ON 商品.区分コード = 商品区分.区分コード
```

こんな感じでもビューは作成できる。

``` sql
CREATE VIEW ParentReservation_View 
AS 
SELECT 'AAA001' AS ReservationNo, 'CommentC' AS Comment
UNION
SELECT 'AAA002', 'CommentX'
```

``` sql
DROP VIEW IF EXISTS ParentReservation_View;
```

---

## ビュー経由の更新

ビューに対して、UPDATEやINSERT,DELETEなど、更新系のステートメントを実行することが可能。  
ただし、ビュー内でGROUP BYや集計関数、DISTINCT、演算結果を行っている列に対しては、更新系のステートメントを実行することはできない。  
なお、このような場合に「INSTED OF トリガー」と呼ばれるトリガーを作成すると、ビューに対して更新系のステートメントが実行されたときに、別の処理を記述できるようになる。  
具体的には、ビューのもとになっているテーブルを直接更新するようなステートメントを記述できるようになる。  

---

## ビューの変更

``ALTER VIEW ステートメント`を利用する。  

``` sql
ALTER VIEW ビュー名
AS
新しいSELECT ステートメント
```

``` sql
CREATE VIEW 受注商品一覧
AS
SELECT 
    YEAR(受注日) AS 年,
    MONTH(受注日) AS 月,
    受注明細.単価 * 数量 AS 受注金額,
    受注.受注コード,
    受注日,
    受注明細,
    単価,
    数量,
    商品名,
    区分名
FROM 受注明細
INNER JOIN 受注 ON 受注明細.受注コード = 受注.受注コード
INNER JOIN 商品 ON 受注明細.商品コード = 商品.商品コード
INNER JOIN 商品区分 ON 商品.区分コード = 商品区分.区分コード
```

---

## LAG関数

SQLServer2012からLAG関数という関数がサポートされ、前年同月を簡単に取得できるようになった。  
LAG関数は、指定した行数分前のデータを取得することができる。  

``` SQL
SELECT 年,月,SUM(受注金額),LAG(SUM(受注金額),12) OVER (ORDER BY 年,月)
FROM 受注商品一覧
GROUP BY 年,月
ORDER BY 年,月
```

---

## ウィンドウ操作で累計金額を取得

SQLServer 2012から、ウィンドウ操作という機能がSUM関数でサポートされ、累積金額を簡単に取得できるようになった。  

OVERの部分がウィンドウ操作、PARTITION BYでグループ化、ORDER BYで並び替え、ROWS UNBOUNDED PRECEDINGでグループ内の先頭行から現在処理している行までを取得することができるので、SUM関数によって累積金額を取得できる。  

ウィンドウ操作はANSI SQL99規格として制定されている。  

``` sql
SELECT 年,月,受注金額,SUM(受注金額),OVER(PARTITION BY 年 ORDER BY 年,月 ROWS UNBOUNDED PRECEDING)
FROM (
    SELECT 年,月,SUM(受注金額) AS 受注金額
    FROM 受注金額一覧
    GROUP BY 年,月
) t1
ORDER BY 年,月
```
