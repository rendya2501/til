# T-SQLまとめ

---

## SQL Server のトランザクション命令

BEGIN TRANSACTION  
COMMIT TRANSACTION  
ROLLBACK TRANSACTION  

---

## SQL Server のロック命令

[ＳＱＬサーバー　ロック](https://development.station-t.com/SqlServer_Lock.htm)  

``` sql : テーブルロック
SET LOCK_TIMEOUT 1000/*ミリ秒単位でロック待ち時間を指定できる*/
SELECT * from TABLE1 WITH(TABLOCKX)
```

### TABLOCK

テーブルにロックを使用し、ステートメント終了まで保持することを指定します。  
データの読み取り中は、共有ロックが使用されます。  
データの変更中は、排他ロックが使用されます。  
HOLDLOCK も指定してある場合は、共有テーブル ロックがトランザクション終了まで保持されます。  

インデックスのないテーブルにデータをインポートするため、OPENROWSET 一括行セット プロバイダで TABLOCK を使用すると、対象テーブルへのデータ読み込みを、ログ記録とロックとを最適化して、複数のクライアントで同時に行うことができます。  

### TABLOCKX

トランザクションが完了するまでテーブルに排他ロックを使用することを指定します。  
(TABLOCKX)は(TABLOCK, XLOCK)でも同じようだが、後者を使用するとデッドロックになるらしい  

### UPDLOCK

更新ロックを使用することと、これをトランザクション終了まで保持することを指定します。  

### XLOCK

排他ロックを使用することと、これをトランザクション終了まで保持することを指定します。  
ROWLOCK、PAGLOCK、または TABLOCK と組み合わせて指定すると、排他ロックは適切な粒度レベルに適用されます。  

---
