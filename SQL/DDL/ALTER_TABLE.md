# ALTER TABLE

---

## 概要

テーブルの構造を変更する命令

[テーブル定義を変更する（ALTER TABLE）](https://www.projectgroup.info/tips/SQLServer/SQL/SQL000005.html)  

---

## カラム名を変更する

```SQL : MariaDB
ALTER TABLE [TableName] RENAME COLUMN [Falg] TO [Flag]
```

``` sql : SQLServer
EXEC sp_rename 'スキーマ名.テーブル名.現在のカラム名', '新しいカラム名', 'COLUMN';

-- データベースの指定はUSEするしかないみたい。
USE TableName;
GO
EXEC sp_rename 'dbo.TestTable.Falg','Flag','COLUMN';
GO
```

[sp_rename](https://docs.microsoft.com/ja-jp/sql/relational-databases/system-stored-procedures/sp-rename-transact-sql?view=sql-server-ver15)  

---

## テーブルの名前を変更する

``` sql : SQLServer
EXEC sp_rename '現在のテーブル名','変更するテーブル名','OBJECT'
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

## テーブルに列を追加する

``` sql
ALTER TABLE テーブル名 ADD
     列名1  VARCHAR (10) DEFAULT '' NOT NULL
    ,列名2  VARCHAR (10) DEFAULT '' NOT NULL
```

---

## カラムを削除する

``` sql : mariaDB,SQLServer
ALTER TABLE テーブル名 DROP COLUMN 列名1,列名2;

-- 例 : TestテーブルからFlagフィールドを削除する
ALTER TABLE Test DROP COLUMN Flag;
```

---

## カラムのデータ型を変更する(列定義を変更する)

```SQL : SQLServer
ALTER TABLE (操作対象テーブル) ALTER COLUMN (データ型を変更する列名) (変更するデータ型)

-- 例 : ProductテーブルのTestCodeカラムの型をintに変更するクエリ
ALTER TABLE [Product] ALTER COLUMN [TestCode] int
```
