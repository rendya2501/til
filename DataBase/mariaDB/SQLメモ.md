# mariaDB,SQLメモ

---

## 本日の日付

``` sql : MARIA DB
SELECT CURDATE();
SELECT DATE(NOW());
```

---

## mariaDBで0埋めして文字列結合するサンプル

20桁のIDを作る要領で3つのキーを0埋めして文字列結合して、GroupByしてCOUNT取ってHAVINGで20以上のIDを表示ってやつ。  

左0埋め→LPAD  
文字列結合→CONCAT  
数値→文字列変換→CAST(数値 AS CHAR)  
数値文字列変換に関してはLPADしてCONCATしたらそのまま行けたので別に必要ないのかも。  

``` SQL
SELECT
    CONCAT(LPAD(Code1, 4, '0'),LPAD(Code2, 6, '0'),LPAD(Code3, 6, '0')),
    COUNT(CONCAT(LPAD(Code1, 4, '0'),LPAD(Code2, 6, '0'),LPAD(Code3, 6, '0')))
FROM
    [Table]
GROUP BY
    CONCAT(LPAD(Code1, 4, '0'),LPAD(Code2 , 6, '0'),LPAD(Code3, 6, '0'))
HAVING
    COUNT(CONCAT(LPAD(Code1, 4, '0'),LPAD(Code2, 6, '0'),LPAD(Code3, 6, '0'))) > 30
```
