# メモ

## 横縦変換

[複数列のデータを縦に並べる方法【SQLServer】](https://qiita.com/sugarboooy/items/0750d0ccb83a2af4dc0e)

``` SQL
SELECT 
    CONCAT('ALP', CONVERT(nvarchar,[TU_売掛残高].[営業日], 112), [v].[SlipNumber]) AS [SettlementID],
    [v].[Seq],
    (SELECT TOP 1 OfficeCD FROM [Round3Dat].[dbo].[TMa_CompanyBasicInfo]) AS [OfficeCD],
    [v].[PaymentDay],
    [v].[DepositAmount],
    [v].[AdjustmentAmount],
    [v].[PaymentCD],
    [v].[AccountCD]
FROM 
    [RoundDatMigrationSource].[dbo].[TU_売掛残高]
CROSS APPLY(
    VALUES
        (伝票番号, 1, 入金日1, 入金額1, 調整額1, 入金CD1, 口座CD1),
        (伝票番号, 2, 入金日2, 入金額2, 調整額2, 入金CD2, 口座CD2),
        (伝票番号, 3, 入金日3, 入金額3, 調整額3, 入金CD3, 口座CD3),
        (伝票番号, 4, 入金日4, 入金額4, 調整額4, 入金CD4, 口座CD4),
        (伝票番号, 5, 入金日5, 入金額5, 調整額5, 入金CD5, 口座CD5)
) AS [v] ([SlipNumber], [Seq], [PaymentDay], [DepositAmount], [AdjustmentAmount], [PaymentCD], [AccountCD])
WHERE 
    [v].[DepositAmount] <> 0
```
