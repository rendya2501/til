# TRY CATCH

``` sql
BEGIN TRAN
BEGIN TRY
    -- 実行文をここに記述
    COMMIT TRAN
END TRY
BEGIN CATCH
    ROLLBACK TRAN
    SELECT
        ERROR_NUMBER() AS ErrorNumber,
        ERROR_SEVERITY() AS ErrorSeverity,
        ERROR_STATE() AS ErrorState,
        ERROR_PROCEDURE() AS ErrorProcedure,
        ERROR_LINE() AS ErrorLine,
        ERROR_MESSAGE() AS ErrorMessage
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
