/*
基本カーソル処理

1. 変数を宣言  
2. カーソルを宣言  
3. カーソルを開く  
4. レコードをフェッチしてループ処理  
5. カーソルを閉じて、リソースを開放する  

[SELECT した結果をカーソルを使用してループ処理をする方法](https://www.projectgroup.info/tips/SQLServer/SQL/SQL000028.html)  
*/

-- CURSOR 変数定義
DECLARE @W_COL1 varchar(50)
DECLARE @W_COL2 decimal(18,0)

-- CURSOR 定義
DECLARE CUR_AAA CURSOR FOR
    SELECT COL1,COL2
    FROM   TAB_A
    WHERE  TAB_A.COL1 = ＜条件値＞

-- CURSOR オープン
OPEN CUR_AAA;

-- CURSOR 最初の1行目を取得して変数へ値をセット
FETCH NEXT FROM CUR_AAA
    INTO @W_COL1,@W_COL2;

-- CURSOR ループ
WHILE @@FETCH_STATUS = 0
    BEGIN
        -- ========= ループ内の実際の処理 ここから===
        INSERT INTO TAB_B
        VALUES (@W_COL1,@W_COL2)
        -- ========= ループ内の実際の処理 ここまで===

        -- CURSOR 次の行のデータを取得して変数へ値をセット
        FETCH NEXT FROM CUR_AAA
            INTO @W_COL1,@W_COL2;
    END

-- CURSOR カーソルを閉じて、リソースを開放する
CLOSE CUR_AAA;
DEALLOCATE CUR_AAA;
