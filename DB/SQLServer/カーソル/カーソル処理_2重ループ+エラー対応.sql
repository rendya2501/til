/* 
エラー対応版 2重カーソルサンプル

手順
1. 変数を宣言  
2. カーソルを宣言  
3. カーソルを開く  
4. レコードをフェッチしてループ処理  
5. カーソルを閉じて、リソースを開放する  
親カーソルの4と5の間で子カーソルの1~5を実行するイメージ  

[SQL SERVERでFOR文を2回まわす(2重ループ)](https://shishimaruby.hatenadiary.org/entry/20100531/1275314636)
[Try/Catch in Cursor It Breaks After Failures - SQL Server Forums](https://www.sqlteam.com/forums/topic.asp?TOPIC_ID=130914)  
[複数のカーソルを開く方法 -ヘッダーと明細で複数のカーソルが必要な場合の記述方法- | SQL Server](https://itblogdsi.blog.fc2.com/blog-entry-315.html)
*/

-- CURSOR 変数定義
DECLARE @A_ID INTEGER;
DECLARE @B_ID INTEGER;

-- CURSOR_1 定義
DECLARE A_TABLE CURSOR LOCAL FOR
    SELECT id
    FROM a_table;

-- エラー処理開始
BEGIN TRY
    -- トランザクション開始
    BEGIN TRANSACTION;
        
    -- CURSOR_1 オープン
    OPEN A_TABLE;

    -- CURSOR_1 最初の1行目を取得して変数へ値をセット
    FETCH NEXT FROM A_TABLE
        INTO @A_ID;

    -- CURSOR_1 ループ
    WHILE @@FETCH_STATUS = 0
    BEGIN 
        -- CURSOR_2 定義
        DECLARE B_TABLE CURSOR LOCAL FOR
            SELECT id
            FROM b_table;

        -- CURSOR_2 オープン
        OPEN B_TABLE;

        -- CURSOR_2 最初の1行目を取得して変数へ値をセット
        FETCH NEXT FROM B_TABLE
            INTO @B_ID;

        -- CURSOR_2 ループ
        WHILE @@FETCH_STATUS = 0
        BEGIN
            -- ========= ループ内の実際の処理 ここから===
            PRINT (convert(varchar,@A_ID) + convert(varchar,@B_ID));
            -- ========= ループ内の実際の処理 ここまで===
            FETCH NEXT FROM B_TABLE
                INTO @B_ID;
        END;

        -- CURSOR_2 カーソルを閉じて、リソースを開放する
        CLOSE B_TABLE;
        DEALLOCATE B_TABLE;

        -- CURSOR_1 次のレコードを取得
        FETCH NEXT FROM @A_TABLE
            INTO @A_ID;
    END;

    -- CURSOR_1 カーソルを閉じて、リソースを開放する
    CLOSE A_TABLE;
    DEALLOCATE A_TABLE;

    -- コミット
    COMMIT TRANSACTION;
END TRY

-- エラーをキャッチした場合
BEGIN CATCH
    -- トランザクションをロールバック
    IF @@TRANCOUNT > 0
        BEGIN
            ROLLBACK TRANSACTION;
        END;

    --カーソルを閉じてリソースを開放する
    IF CURSOR_STATUS('local', 'A_TABLE') > 0
        BEGIN
            CLOSE A_TABLE ;
            DEALLOCATE A_TABLE;
        END;
    IF CURSOR_STATUS('local', 'B_TABLE') > 0
        BEGIN
            CLOSE B_TABLE ;
            DEALLOCATE B_TABLE;
        END;

    -- エラー内容を表示
    SELECT
        ERROR_NUMBER() AS ErrorNumber,
        ERROR_SEVERITY() AS ErrorSeverity,
        ERROR_STATE() AS ErrorState,
        ERROR_PROCEDURE() AS ErrorProcedure,
        ERROR_LINE() AS ErrorLine,
        ERROR_MESSAGE() AS ErrorMessage;
    -- 2012以降であればSELECTはTHROWだけでよい。
END CATCH;
