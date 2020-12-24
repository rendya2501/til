# テーブル構造変更系SQL

## テーブルのカラム名を変更するSQL

```SQL
-- MariaDB
ALTER TABLE [TMa_Product]
RENAME COLUMN [RevenuTypeCD] TO [RevenueTypeCD]

-- SQL Server
EXEC sp_rename 'スキーマ名.テーブル名.現在のカラム名', '新しいカラム名', 'COLUMN';

EXEC sp_rename 'TMa_Product.RevenuTypeCD','RevenueTypeCD','COLUMN'
```

---

## テーブルのデータ型を変更するSQL

```SQL
-- SQL Server
ALTER TABLE (操作対象テーブル) ALTER column (データ型を変更する列名) (変更するデータ型)

-- 例 : TMa_ProductテーブルのRevenueTypeCDカラムの型をintに変更するクエリ
ALTER TABLE [TMa_Product] ALTER column [RevenueTypeCD] int
```
