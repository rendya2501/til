# UNOIN

``` SQL
SELECT 1 AS NUM
UNION
SELECT 2 AS NUM;
```

- [union]は出力テーブルの構造が同じでないといけない。  
- [union]は重複チェックする。  
- [union all]は重複チェックしない。  
  → なので、速度的にはunion all のほうが早い。  
