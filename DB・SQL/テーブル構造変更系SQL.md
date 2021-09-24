# テーブル構造変更系SQL

## CASCADE

削除するテーブルに依存しているオブジェクト（ビューなど）を自動的に削除します。  

DROP TABLEは、削除対象のテーブル内に存在するインデックス、ルール、トリガ、制約を全て削除します。  
しかし、ビューや他のテーブルから外部キー制約によって参照されているテーブルを削除するにはCASCADEを指定する必要があります  
（CASCADEを指定すると、テーブルに依存するビューは完全に削除されます。  
しかし、外部キー制約によって参照されている場合は、外部キー制約のみが削除され、その外部キーを持つテーブルそのものは削除されません）。  

---

## テーブルのカラム名を変更するSQL

```SQL
-- MariaDB
ALTER TABLE [TMa_Product] RENAME COLUMN [RevenuTypeCD] TO [RevenueTypeCD]

-- SQL Server
-- https://docs.microsoft.com/ja-jp/sql/relational-databases/system-stored-procedures/sp-rename-transact-sql?view=sql-server-ver15
-- 珍しく公式サイトが参考になった。
EXEC sp_rename 'スキーマ名.テーブル名.現在のカラム名', '新しいカラム名', 'COLUMN';

-- テーブルを指定したい場合はUSEでテーブルを切り替えて実行するしかないみたい。
USE Round3Dat_Test;
GO
EXEC sp_rename 'TMa_Supplier.ValidFalg','ValidFlag','COLUMN';
GO
```

---

## テーブルのデータ型を変更するSQL

```SQL
-- SQL Server
ALTER TABLE (操作対象テーブル) ALTER column (データ型を変更する列名) (変更するデータ型)

-- 例 : TMa_ProductテーブルのRevenueTypeCDカラムの型をintに変更するクエリ
ALTER TABLE [TMa_Product] ALTER column [RevenueTypeCD] int

--- MariaDB,カラム追加
-- TmOpenPlanPGMWEBテーブルのHolidayExtraPriceOneBagフィールドの後にTaxSelectionStatusTypeを追加。型はboolで初期値は0。コメント付き。
ALTER TABLE `TmOpenPlanPGMWEB` ADD COLUMN `TaxSelectionStatusType` TINYINT(1) NOT NULL DEFAULT 0 comment '税選択状態区分 税抜(外税):0 税込(内税):1' AFTER `HolidayExtraPriceOneBag`;

-- TmOpenPlanGDOテーブルからMailPushSendFlagフィールドを削除する
ALTER TABLE TmOpenPlanGDO DROP COLUMN MailPushSendFlag;
```

## 後でまとめるけど、きっかけは応用の問題から

<https://qiita.com/ryota_i/items/d17c7630bacb36d26864>

INSERT INTO テーブル VALUES()
INSERT INTO テーブル SELECT 項目名 FROM 社員

他のテーブルの内容をselectして追加することも可能なのね。
普通に考えたらできるだろうけど、割と新鮮である。

```sql
-- テーブルの値を別テーブルの値でUPDATEする(其の壱)
UPDATE
    テーブル名1
    INNER JOIN テーブル名2 ON テーブル名1.列名X = テーブル名2.列名X
SET
    テーブル名1.列名1 = テーブル名2.列名2;

UPDATE
    テーブル名1,
    テーブル名2
SET
    テーブル名1.列名1 = テーブル名2.列名2
WHERE
    テーブル名1.列名X = テーブル名2.列名X;
```

```sql
-- テーブルの値を別テーブルの値でUPDATEする(其の弐)
UPDATE
    [Round3Dat_Test].[dbo].[TMa_ProductCls]
SET
    [Round3Dat_Test].[dbo].[TMa_ProductCls].[DepartmentCD] = (
        SELECT
            [DepartmentCD]
        FROM
            [Round3Dat_20210205].[dbo].[TMa_ProductCls]
        WHERE
            [Round3Dat_Test].[dbo].[TMa_ProductCls].[ProductClsCD] = [Round3Dat_20210205].[dbo].[TMa_ProductCls].[ProductClsCD]
    )
```

```sql
-- テーブルの更新条件を副問い合わせで取ってくる
UPDATE
    name A
SET
    A.attr_name = 'ほのお'
WHERE
    A.id IN (
        SELECT
            B.id
        FROM
            attribute2 B
        WHERE
            B.attr_name = 'ほのお'
    );
```

早速例のアップデート文の復習ができた。
UPDATE文ってUPDATEしようとしているテーブルの情報をサブクエリに使うこともできたのね。
というか、UPDATE文なんてそうそうやるものじゃないから、全然しらなかったはー。

```sql
select SUM(RoundCount)  
from TRe_ReservationFrame  
where BusinessDate = '20210215'  
```

GROUP BY しなくても合計も止まりました。
集計関数ってのは別にGROUP BYしなくても、これくらい単純なSQLなら自動的に算出してくれるみたいですね。

---
