# SQLServer関係メモ

## SQL SERVER のトランザクション命令

BEGIN TRANSACTION  
COMMIT TRANSACTION  
ROLLBACK TRANSACTION  

---

## null許可のフィールドを追加→変更の保存が許可されていません

[変更の保存は、エラー メッセージで許可SSMS](https://docs.microsoft.com/ja-jp/troubleshoot/sql/ssms/error-when-you-save-table)  

科目大区分を新規作成するに当たり、科目区分テーブルにnull許可の科目大区分CDフィールドを追加しようとしたら発生した。  
フィールドを追加する際に結構発生するイメージがある。  
オプション一つで解決可能であるが、備忘録として残しておく。  

1.[ツール] メニューの [オプション] をクリックします。  
2.[オプション] ウィンドウのナビゲーション ウィンドウ で 、[デザイナー] をクリックします。  
3.[テーブルの再作成を必要 とする 変更の保存を防止する] チェック ボックスをオンまたはオフにし 、[OK] をクリックします。  

[SQL Server カラム定義を変更すると「変更の保存が許可されていません」が表示された場合の対処法](https://nasunoblog.blogspot.com/2013/10/sql-server-column-edit-error.html)  
このサイトのほうが読みやすい。  
因みにこの現象が発生する原因としてはテーブルの監視履歴が削除されてしまうからだそうで。  

---

## テーブルデザイナーにコメント列を追加する方法

あると便利といわれたのでまとめ。  
レジストリをいじらないと駄目です。  

[SSMS のテーブルデザイナの列をカスタマイズした](https://qiita.com/d01tsumath/items/906043d69f86a6a53cef)  
[SQL Server Management Studio のテーブルデザイナの列をカスタマイズする](https://blog.xin9le.net/entry/2018/06/17/165526)  
