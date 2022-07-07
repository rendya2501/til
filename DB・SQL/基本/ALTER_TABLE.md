# ALTER TABLE

テーブルの構造を変更する命令

---

## カラム名を変更する

```SQL : MariaDB
ALTER TABLE [TableName] RENAME COLUMN [Falg] TO [Flag]
```

<https://docs.microsoft.com/ja-jp/sql/relational-databases/system-stored-procedures/sp-rename-transact-sql?view=sql-server-ver15>  

``` sql : SQLServer
EXEC sp_rename 'スキーマ名.テーブル名.現在のカラム名', '新しいカラム名', 'COLUMN';

-- データベースの指定はUSEするしかないみたい。
USE TableName;
GO
EXEC sp_rename 'dbo.TestTable.Falg','Flag','COLUMN';
GO
```

---

## テーブルの任意の位置にカラムを追加する

``` sql : mariaDB
--- カラム追加
-- TestテーブルのTestFlagフィールドの後にTestTypeを追加。型はboolで初期値は0。コメント付き。
ALTER TABLE `Test` ADD COLUMN `TestType` TINYINT(1) NOT NULL DEFAULT 0 comment 'コメントです' AFTER `TestFlag`;
```

SQLServerではそのような命令はないので、Tempテーブルにデータを対比してテーブルを作り直して、元に戻すという操作をする必要がある。  
そのクエリはSQLServer側でまとめているのでそちらを参照されたし。  

---

## カラムを削除する

``` sql : mariaDB
-- TestテーブルからFlagフィールドを削除する
ALTER TABLE Test DROP COLUMN Flag;
```

---

## カラムのデータ型を変更する

```SQL : SQLServer
ALTER TABLE (操作対象テーブル) ALTER column (データ型を変更する列名) (変更するデータ型)

-- 例 : ProductテーブルのTestCodeカラムの型をintに変更するクエリ
ALTER TABLE [Product] ALTER column [TestCode] int
```
