# TRY CATCH

---

``` sql
BEGIN
    BEGIN TRY
        BEGIN TRANSACTION
        -- 実行文をここに記述
        COMMIT TRANSACTION
    END TRY

    BEGIN CATCH
        ROLLBACK TRANSACTION
        SELECT
            ERROR_NUMBER() AS ErrorNumber,
            ERROR_SEVERITY() AS ErrorSeverity,
            ERROR_STATE() AS ErrorState,
            ERROR_PROCEDURE() AS ErrorProcedure,
            ERROR_LINE() AS ErrorLine,
            ERROR_MESSAGE() AS ErrorMessage
        THROW
    END CATCH
END
```

``` txt
ERROR_NUMBER()    :: エラーの数を返します。
ERROR_SEVERITY()  :: 重大度を返します。
ERROR_STATE()     :: エラー状態番号を返します。
ERROR_PROCEDURE() :: エラーが発生したストアドプロシージャまたはトリガーの名前を返します。
ERROR_LINE()      :: エラーを発生させたルーチン内の行番号を返します。
ERROR_MESSAGE()   :: エラーメッセージの全文を返します。
```

---

## THROW

[SQLServerでTRY-CATCHしちゃうと呼び出し側でエラー判別しにくい](https://qiita.com/ryo_naka/items/2e79f8dcf9c24f1b7269)  
