# ALTER TABLE

テーブルの構造を変更する命令

---

## テーブルのカラム名を変更する

```SQL : MariaDB
ALTER TABLE [TMa_Product] RENAME COLUMN [RevenuTypeCD] TO [RevenueTypeCD]
```

``` sql : SQL Server
-- https://docs.microsoft.com/ja-jp/sql/relational-databases/system-stored-procedures/sp-rename-transact-sql?view=sql-server-ver15
-- 珍しく公式サイトが参考になった。
EXEC sp_rename 'スキーマ名.テーブル名.現在のカラム名', '新しいカラム名', 'COLUMN';

-- データベースの指定はUSEするしかないみたい。
USE Round3Dat_Test;
GO
EXEC sp_rename 'dbo.TMa_Supplier.ValidFalg','ValidFlag','COLUMN';
GO
```

---

## テーブルのデータ型を変更する

```SQL : SQL Server
ALTER TABLE (操作対象テーブル) ALTER column (データ型を変更する列名) (変更するデータ型)

-- 例 : TMa_ProductテーブルのRevenueTypeCDカラムの型をintに変更するクエリ
ALTER TABLE [TMa_Product] ALTER column [RevenueTypeCD] int
```

``` sql : MariaDB
--- カラム追加
-- TmOpenPlanPGMWEBテーブルのHolidayExtraPriceOneBagフィールドの後にTaxSelectionStatusTypeを追加。型はboolで初期値は0。コメント付き。
ALTER TABLE `TmOpenPlanPGMWEB` ADD COLUMN `TaxSelectionStatusType` TINYINT(1) NOT NULL DEFAULT 0 comment '税選択状態区分 税抜(外税):0 税込(内税):1' AFTER `HolidayExtraPriceOneBag`;

-- TmOpenPlanGDOテーブルからMailPushSendFlagフィールドを削除する
ALTER TABLE TmOpenPlanGDO DROP COLUMN MailPushSendFlag;
```
