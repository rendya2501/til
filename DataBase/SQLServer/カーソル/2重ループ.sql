/* 
2重ループ最小サンプル

手順
1. 変数を宣言  
2. カーソルを宣言  
3. カーソルを開く  
4. レコードをフェッチしてループ処理  
5. カーソルを閉じて、リソースを開放する  
親カーソルの4と5の間で子カーソルの1~5を実行するイメージ  

[SQL SERVERでFOR文を2回まわす(2重ループ)](https://shishimaruby.hatenadiary.org/entry/20100531/1275314636)
*/

-- CURSOR 変数定義
DECLARE @A_ID INTEGER 
DECLARE @B_ID INTEGER

-- CURSOR_1 定義
DECLARE A_TABLE CURSOR FOR
    SELECT id
    FROM a_table 

-- CURSOR_1 オープン
OPEN A_TABLE

-- CURSOR_1 最初の1行目を取得して変数へ値をセット
FETCH NEXT FROM A_TABLE
    INTO @A_ID

-- CURSOR_1 ループ
WHILE @@FETCH_STATUS = 0
    BEGIN 
        -- CURSOR_2 定義
        DECLARE B_TABLE CURSOR FOR
            SELECT id
            FROM b_table

        -- CURSOR_2 オープン
        OPEN B_TABLE 

        -- CURSOR_2 最初の1行目を取得して変数へ値をセット
        FETCH NEXT FROM B_TABLE
            INTO @B_ID

        -- CURSOR_2 ループ
        WHILE @@FETCH_STATUS = 0
            BEGIN
                PRINT (convert(varchar,@A_ID) + convert(varchar,@B_ID))
            END

        -- CURSOR_2 カーソルを閉じて、リソースを開放する
        CLOSE B_TABLE
        DEALLOCATE B_TABLE

        -- CURSOR_1 次のレコードを取得
        FETCH NEXT FROM @A_ID
            INTO @A_ID
    END

-- CURSOR_1 カーソルを閉じて、リソースを開放する
CLOSE A_TABLE
DEALLOCATE A_TABLE

