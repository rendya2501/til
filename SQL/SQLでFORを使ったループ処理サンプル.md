# SQLでFORを使ったループ処理サンプル

[SQL SERVERでFOR文を2回まわす(2重ループ)](<https://shishimaruby.hatenadiary.org/entry/20100531/1275314636>)
[SELECT した結果をカーソルを使用してループ処理をする方法](https://www.projectgroup.info/tips/SQLServer/SQL/SQL000028.html)
[直前のINSERTで自動採番したIDENTITY列の値を取得する](https://lightgauge.net/database/sqlserver/get-identity-before/)

実務でうまく作れたのでまとめ。  
SELECTした結果をループしながら計算して別テーブルにINSERTという芸当をするには、  
1クエリでやるにはやばすぎたので、SQLで変数とForやwhileなどのループを使って実現した。  
複数のテーブルにINSERTできる`INSERT ALL`なるものも存在する見たいだが、ループ中の計算を再現しながらそれは困難だった。  

以下使った技術
・selectした結果を変数に代入  
・中身がなくなるまでWHILEでカーソル移動  
・2重ループ  
・直前で発番されたINDENTITY値を取得

``` SQL
-- 売掛入金と売掛入金精算IDリスト
DECLARE @SettlementID NVARCHAR(18)
DECLARE @AccountsReceivable Decimal(18,0)
DECLARE @BEFORE_ID BIGINT
DECLARE @OfficeCD AS NVARCHAR(3)
DECLARE @PaymentDay AS Date
DECLARE @DepositAmount AS Decimal(18,2)
DECLARE @AdjustmentAmount AS Decimal(18,2)
DECLARE @PaymentCD AS INT
DECLARE @AccountCD AS INT
DECLARE @InsertDateTime AS Date
DECLARE @InsertStaffCD AS NVARCHAR(16)
DECLARE @InsertStaffName AS NVARCHAR(20)
DECLARE @InsertProgram AS NVARCHAR(20)
DECLARE @InsertTerminal AS NVARCHAR(20)
DECLARE @UpdateDateTime AS Date
DECLARE @UpdateStaffCD AS NVARCHAR(16)
DECLARE @UpdateStaffName AS NVARCHAR(20)
DECLARE @UpdateProgram AS NVARCHAR(20)
DECLARE @UpdateTerminal AS NVARCHAR(20)

-- 売掛明細でループ
DECLARE CUR_TCs_AccountsReceivableDetail CURSOR FOR
    SELECT [SettlementID],[AccountsReceivable]
    FROM [TCs_AccountsReceivableDetail]

    -- 売掛明細カーソルオープン
    OPEN CUR_TCs_AccountsReceivableDetail;

    -- 売掛明細,最初の1行目を取得して変数へ値をセット
    FETCH NEXT FROM CUR_TCs_AccountsReceivableDetail
    INTO @SettlementID,@AccountsReceivable;

    -- 売掛明細,データの行数分ループ処理を実行する
    WHILE @@FETCH_STATUS = 0
    BEGIN
        -- ========= 売掛明細ループ内の実際の処理 ここから===

        -- サブクエリ(売掛入金)でループ
        DECLARE CUR_Payment_ID CURSOR FOR
            SELECT
                CONCAT((select top 1 OfficeCD from TMa_CompanyBasicInfo), CONVERT(nvarchar,[TU_売掛残高].[営業日], 112), [v].[SlipNumber]) AS [SettlementID],
                (SELECT TOP 1 OfficeCD FROM [Round3Dat].[dbo].[TMa_CompanyBasicInfo]) AS [OfficeCD],
                [v].[PaymentDay],
                [v].[DepositAmount],
                ISNULL([v].[AdjustmentAmount],0) AS [AdjustmentAmount],
                -- nvarchar→intの変換において空文字が含まれてエラーになったので全て殺した
                CASE WHEN TRY_CAST([v].[PaymentCD] AS INT) IS NOT NULL THEN CONVERT(int,[v].[PaymentCD]) ELSE Null END AS [PaymentCD],
                CASE WHEN TRY_CAST([v].[AccountCD] AS INT) IS NOT NULL THEN CONVERT(int,[v].[AccountCD]) ELSE Null END AS [AccountCD],
                [v].[InsertDateTime],
                '' AS [InsertStaffCD],
                '' AS [InsertStaffName],
                [v].[InsertProgram],
                [v].[InsertTerminal],
                [v].[UpdateDateTime],
                '' AS [UpdateStaffCD],
                '' AS [UpdateStaffName],
                [v].[UpdateProgram],
                [v].[UpdateTerminal]
            FROM
                [RoundDatMigrationSource].[dbo].[TU_売掛残高]
                CROSS APPLY(
                    VALUES
                        (伝票番号,1,入金日1,入金額1,調整額1,入金CD1,口座CD1,作成日,更新Prg,ComputerName,更新日,更新Prg,ComputerName),
                        (伝票番号,2,入金日2,入金額2,調整額2,入金CD2,口座CD2,作成日,更新Prg,ComputerName,更新日,更新Prg,ComputerName),
                        (伝票番号,3,入金日3,入金額3,調整額3,入金CD3,口座CD3,作成日,更新Prg,ComputerName,更新日,更新Prg,ComputerName),
                        (伝票番号,4,入金日4,入金額4,調整額4,入金CD4,口座CD4,作成日,更新Prg,ComputerName,更新日,更新Prg,ComputerName),
                        (伝票番号,5,入金日5,入金額5,調整額5,入金CD5,口座CD5,作成日,更新Prg,ComputerName,更新日,更新Prg,ComputerName)
                ) AS [v] (
                    [SlipNumber],
                    [SeqNo],
                    [PaymentDay],
                    [DepositAmount],
                    [AdjustmentAmount],
                    [PaymentCD],
                    [AccountCD],
                    [InsertDateTime],
                    [InsertProgram],
                    [InsertTerminal],
                    [UpdateDateTime],
                    [UpdateProgram],
                    [UpdateTerminal]
                )
            WHERE
                CONCAT((select top 1 OfficeCD from TMa_CompanyBasicInfo), CONVERT(nvarchar,[TU_売掛残高].[営業日], 112), [v].[SlipNumber]) = @SettlementID
                AND [v].[DepositAmount] <> 0

            -- サブクエリ(売掛入金)カーソルオープン
            OPEN CUR_Payment_ID;

            -- サブクエリ(売掛入金)最初の1行目を取得して変数へ値をセット
            FETCH NEXT FROM CUR_Payment_ID
            INTO @SettlementID,
                @OfficeCD,
                @PaymentDay,
                @DepositAmount,
                @AdjustmentAmount,
                @PaymentCD,
                @AccountCD,
                @InsertDateTime,
                @InsertStaffCD,
                @InsertStaffName,
                @InsertProgram,
                @InsertTerminal,
                @UpdateDateTime,
                @UpdateStaffCD,
                @UpdateStaffName,
                @UpdateProgram,
                @UpdateTerminal;

            -- サブクエリ(売掛入金)データの行数分ループ処理を実行する
            WHILE @@FETCH_STATUS = 0
            BEGIN

                -- 売掛入金テーブルへのInsert
                INSERT INTO [TCs_AccountsReceivablePayment] (
                    OfficeCD,
                    PaymentDay,
                    DepositAmount,
                    AdjustmentAmount,
                    PaymentCD,
                    AccountCD,
                    InsertDateTime,
                    InsertStaffCD,
                    InsertStaffName,
                    InsertProgram,
                    InsertTerminal,
                    UpdateDateTime,
                    UpdateStaffCD,
                    UpdateStaffName,
                    UpdateProgram,
                    UpdateTerminal)
                VALUES (
                    @OfficeCD,
                    @PaymentDay,
                    @DepositAmount,
                    @AdjustmentAmount,
                    @PaymentCD,
                    @AccountCD,
                    @InsertDateTime,
                    @InsertStaffCD,
                    @InsertStaffName,
                    @InsertProgram,
                    @InsertTerminal,
                    @UpdateDateTime,
                    @UpdateStaffCD,
                    @UpdateStaffName,
                    @UpdateProgram,
                    @UpdateTerminal)

                -- 直前に採番された IDENTITY 列の値を取得する
                SET @BEFORE_ID = @@IDENTITY

                -- 中間テーブルへのInsert
                INSERT INTO [TCs_AccountsReceivablePaymentSettleIDList]
                VALUES (
                    @BEFORE_ID,
                    @SettlementID,
                    @AccountsReceivable,
                    @DepositAmount,
                    @AdjustmentAmount,
                    @AccountsReceivable - @DepositAmount - @AdjustmentAmount,
                    @InsertDateTime,
                    @InsertStaffCD,
                    @InsertStaffName,
                    @InsertProgram,
                    @InsertTerminal,
                    @UpdateDateTime,
                    @UpdateStaffCD,
                    @UpdateStaffName,
                    @UpdateProgram,
                    @UpdateTerminal)

                -- 次のループの残額を計算
                SET @AccountsReceivable = @AccountsReceivable - @DepositAmount - @AdjustmentAmount;

                -- 次の行のデータを取得して変数へ値をセット
                FETCH NEXT FROM CUR_Payment_ID
                INTO @SettlementID,
                    @OfficeCD,
                    @PaymentDay,
                    @DepositAmount,
                    @AdjustmentAmount,
                    @PaymentCD,
                    @AccountCD,
                    @InsertDateTime,
                    @InsertStaffCD,
                    @InsertStaffName,
                    @InsertProgram,
                    @InsertTerminal,
                    @UpdateDateTime,
                    @UpdateStaffCD,
                    @UpdateStaffName,
                    @UpdateProgram,
                    @UpdateTerminal;
            END

        -- サブクエリ(売掛入金)カーソルを閉じる
        CLOSE CUR_Payment_ID;
        DEALLOCATE CUR_Payment_ID;

        -- ========= 売掛明細ループ内の実際の処理 ここまで===

        -- 売掛明細,次の行のデータを取得して変数へ値をセット
        FETCH NEXT FROM CUR_TCs_AccountsReceivableDetail
        INTO @SettlementID,@AccountsReceivable;
    END

-- 売掛明細カーソルを閉じる
CLOSE CUR_TCs_AccountsReceivableDetail;
DEALLOCATE CUR_TCs_AccountsReceivableDetail;

```
