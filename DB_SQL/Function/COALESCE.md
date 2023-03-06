# COALESCE関数

``` sql
-- HOGE
SELECT COALESCE(NULL,NULL,'HOGE','FUGA',NULL)

-- PIYO
SELECT COALESCE('PIYO',NULL,'HOGE','FUGA',NULL)
```

[COALESCE 引数のうち最初のNULLではない値を返すSQL関数の使い方](https://segakuin.com/oracle/function/coalesce.html)  
