# IF

---

## IF NOT EXIST

なければ実行、あれば何もしないサンプル  

``` sql
IF NOT EXISTS
(
    SELECT TOP 1 1 FROM TestTable WHERE [Code] = 99
)
    -- 上記select文の結果がfalseならInsertが実行される。
    INSERT INTO
    ~~~
GO
```

※IF NOT EXISTSで対象となるクエリは1つだけ。  
なので、例えばINSERT文を2つ書いた場合、適応されるのは最初のINSERT文だけで2つ目のINSERTはこの条件に関わらず絶対に実行されてしまうことに注意。  

``` sql
IF NOT EXISTS
(
    SELECT TOP 1 1 FROM TestTable WHERE [Code] = 99
)
    INSERT INTO
    ~~~
    -- 最初のINSERTの直下にINSERTを書いても、条件が適応されるされるのは最初のINSERT文だけなので、2つ目のINSERT文は実行されてしまうことに注意する
    INSERT INTO
    ~~~
GO
```

``` sql
IF NOT EXISTS
(
    SELECT TOP 1 1 FROM TestTable WHERE [Code] = 99
)
    INSERT INTO
    ~~~
GO
IF NOT EXISTS
(
    SELECT TOP 1 1 FROM TestTable WHERE [Code] = 99
)
    INSERT INTO
    ~~~
GO
```
