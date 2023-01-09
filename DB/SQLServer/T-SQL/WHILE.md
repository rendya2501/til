# WHILE

## 基本

条件式の括弧は省略可能。  
実行したいステートメントが1つだけの場合、BEGIN,ENDを省略可能。  

ループ内でBREAKEが実行されるとループを抜ける。  
ループ内でCONTINUEが実行されるとループの先頭に戻る。  

``` sql
WHILE (条件式)
[BEGIN]
    条件を満たしている間、実行したいステートメント
[END]
```

---

## WHILE 10回ループ  

``` SQL
--変数宣言&初期化
DECLARE @i int = 0

WHILE @i < 10
BEGIN
    SET @i = @i + 1
    PRINT @i
END
```

`SET @i = @i + 1`は`SET @i += 1`のようにインクリメント演算子を利用することができる。  
インクリメント演算子はSQLServer2008以降からサポート。  

## 10回ループしてBREAKでループを抜ける  

``` SQL
--変数宣言&初期化
DECLARE @i int = 0

WHILE 1=1
BEGIN
    SET @i = @i + 1
    IF @i > 10
        BREAK

    PRINT @i
END
```

---

[SQL Server - WHILEによるループ(T-SQL)](https://www.curict.com/item/bb/bb80194.html)  
