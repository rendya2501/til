# TRY CATCH

---

## 基本

``` sql
BEGIN TRY
    BEGIN TRANSACTION;
    
    COMMIT TRANSACTION;
END TRY

BEGIN CATCH
    ROLLBACK TRANSACTION;
END CATCH
```

[トランザクション処理をさらっとマスターしよう](https://atmarkit.itmedia.co.jp/ait/articles/0803/24/news138_2.html)  

---

## SELECT

``` sql
BEGIN TRY
    BEGIN TRANSACTION;

    -- ===========================
    -- 処理をここに記述
    -- ===========================

    COMMIT TRANSACTION;
END TRY

BEGIN CATCH
    IF @@TRANCOUNT > 0
        BEGIN 
            ROLLBACK TRANSACTION;
        END;

    SELECT
        ERROR_NUMBER() AS ErrorNumber,
        ERROR_SEVERITY() AS ErrorSeverity,
        ERROR_STATE() AS ErrorState,
        ERROR_PROCEDURE() AS ErrorProcedure,
        ERROR_LINE() AS ErrorLine,
        ERROR_MESSAGE() AS ErrorMessage;
END CATCH
```

``` txt
ERROR_NUMBER()    :: エラーの数を返します。
ERROR_SEVERITY()  :: 重大度を返します。
ERROR_STATE()     :: エラー状態番号を返します。
ERROR_PROCEDURE() :: エラーが発生したストアドプロシージャまたはトリガーの名前を返します。
ERROR_LINE()      :: エラーを発生させたルーチン内の行番号を返します。
ERROR_MESSAGE()   :: エラーメッセージの全文を返します。
```

[SQLServerのストアドプロシージャーで例外をキャッチする方法](https://it-engineer-info.com/database/excption-try-catch)  

---

## THROW

2012以降であればTHROWが使えるのでそちらが楽かと思われる。  
内容も一連のSELECTと同じ。  
SELECTと共存させた場合、THROWの表示が優先されるのでSELECTの意味はなくなる。  

``` sql
BEGIN TRY
    BEGIN TRANSACTION;

    -- ===========================
    -- 処理をここに記述
    -- ===========================

    COMMIT TRANSACTION;
END TRY

BEGIN CATCH
    IF @@TRANCOUNT > 0
        BEGIN 
            ROLLBACK TRANSACTION;
        END;

    THROW;
END CATCH
```

[SQLServerでTRY-CATCHしちゃうと呼び出し側でエラー判別しにくい](https://qiita.com/ryo_naka/items/2e79f8dcf9c24f1b7269)  

---

## RAISERROR

RAISERRORで頑張る事もできるが、それなら素直にSELECTしたほうが良い。  
でもって0除算エラーはエラー番号8134なのだが、RAISEERRORすると50000になるっぽいので、なおさら使う機会はないのかもしれない。  
一応備忘録として残す。  

``` sql
BEGIN TRY  
    SELECT 1/0;
END TRY  

BEGIN CATCH  
    DECLARE 
        @ErrorMessage   nvarchar(4000),  
        @ErrorSeverity   int,  
        @ErrorState int,  
        @ErrorLine  int,  
        @ErrorNumber   int  

    SELECT 
        @ErrorMessage = ERROR_MESSAGE(),
        @ErrorSeverity = ERROR_SEVERITY(),
        @ErrorState = ERROR_STATE(),
        @ErrorNumber = ERROR_NUMBER(),
        @ErrorLine = ERROR_LINE()

    RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState, @ErrorNumber, @ErrorLine)  
END CATCH
```

[TRY CATCH ROLLBACKパターンを含むネストされたストアドプロシージャ](https://www.web-dev-qa-db-ja.com/ja/sql-server-2005/try-catch-rollback%E3%83%91%E3%82%BF%E3%83%BC%E3%83%B3%E3%82%92%E5%90%AB%E3%82%80%E3%83%8D%E3%82%B9%E3%83%88%E3%81%95%E3%82%8C%E3%81%9F%E3%82%B9%E3%83%88%E3%82%A2%E3%83%89%E3%83%97%E3%83%AD%E3%82%B7%E3%83%BC%E3%82%B8%E3%83%A3/968296485/)  

---

## Best Practice

``` sql : OptionA
BEGIN TRY
    BEGIN TRANSACTION
    INSERT sometable(a, b) VALUES (@a, @b)
    INSERT sometable(a, b) VALUES (@b, @a)
    COMMIT TRANSACTION
END TRY
BEGIN CATCH
    IF @@trancount > 0 ROLLBACK TRANSACTION
    DECLARE @msg nvarchar(2048) = error_message()  
    RAISERROR (@msg, 16, 1)
    RETURN 55555
END CATCH
```

``` sql : OptionB
BEGIN TRY
    BEGIN TRANSACTION
    INSERT sometable(a, b) VALUES (@a, @b)
    INSERT sometable(a, b) VALUES (@b, @a)
END TRY
BEGIN CATCH
    IF @@trancount > 0 ROLLBACK TRANSACTION
    DECLARE @msg nvarchar(2048) = error_message()  
    RAISERROR (@msg, 16, 1)
    RETURN 55555
END CATCH
IF @@trancount > 0  COMMIT TRANSACTION
```

``` sql : best
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION

    /*
        Code goes here
    */

    COMMIT TRANSACTION
END TRY

BEGIN CATCH
    DECLARE @ErrorMessage NVARCHAR(4000);
    DECLARE @ErrorSeverity INT;
    DECLARE @ErrorState INT;

    SELECT 
        @ErrorMessage = ERROR_MESSAGE(),
        @ErrorSeverity = ERROR_SEVERITY(),
        @ErrorState = ERROR_STATE();

    RAISERROR (@ErrorMessage, -- Message text.
               @ErrorSeverity, -- Severity.
               @ErrorState -- State.
               );

    -- If >= SQL 2012 replace all code in catch block above with
    -- THROW;

    WHILE @@TRANCOUNT > 0
    BEGIN
        ROLLBACK TRANSACTION;
    END
END CATCH
```

[Best practices for committing a transaction in SQL Server where TRY CATCH is used](https://dba.stackexchange.com/questions/233079/best-practices-for-committing-a-transaction-in-sql-server-where-try-catch-is-use)  

---

## カーソル処理に置けるTryCatch

基本的にTRY CATCHで囲って、

[複数のカーソルを開く方法 -ヘッダーと明細で複数のカーソルが必要な場合の記述方法- | SQL Server](https://itblogdsi.blog.fc2.com/blog-entry-315.html)  
[トランザクション処理をさらっとマスターしよう](https://atmarkit.itmedia.co.jp/ait/articles/0803/24/news138_3.html)  

---

## 参考

Sqlserver TRYCATCH ROLLBACK
[エラー発生時のトランザクションのロールバック - SET XACT_ABORT ON　と TRY...CATCH](https://sql55.com/column/rollback-transaction-set-xact-abort.php)  
