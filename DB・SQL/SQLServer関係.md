# SQLServer関係メモ

[SQL Serverチートシート](https://qiita.com/esflat/items/7885e53737163eb955fe)  

---

## SQL Server のデータ型

[[SQLServer] データ型一覧](http://teabreak.info/%E3%83%A1%E3%83%A2/sqlserver-%E3%82%AB%E3%83%A9%E3%83%A0%E5%9E%8B%E4%B8%80%E8%A6%A7/)  
[【SQL Server】データ型の種類](https://pg-life.net/db/data-type/)  

### 整数型

``` txt
bigint   :: -2^63 (-9,223,372,036,854,775,808) ～ 2^63-1(9,223,372,036,854,775,807)。使用する領域は8バイト(64ビット)
int      :: -2^31 (-2,147,483,648) ～ 2^31-1 (2,147,483,647)。使用する領域は4バイト(32ビット)
smallint :: -2^15 (-32,768) ～ 2^15-1 (32,767)。使用する領域は2バイト(16ビット)
tinyint  :: 0~255。使用する領域は1バイト(8ビット)
bit      :: 1 or 0
```

### 実数型

``` txt
decimal  ::
```

#### decimal

deciaml(10, 3)のように定義すると、全体の桁数が10、小数点以下の桁数が3までという意味になります。  
4桁目以降の数値は四捨五入されます。  

#### real / float

real と float は少数のデータを格納するためのデータ型です。  
decimalとは異なり、小数点以下の桁数を指定することができません。  

real は整数部と小数部の合計桁数が7桁まで。
float は15桁まで。  

### 文字列型

``` txt
char[(n)]        :: Unicodeではない、固定長、文字列型、1 ～ 8,000
varchar[(n|max)] :: Unicodeではない、可変長、文字列型、1 ～ 8,000  max：最大値 2^31-1 (4バイト)
text             :: Unicodeではない、可変長、文字列型、文字列の最大長は 2^31-1 (2,147,483,647)
```

#### 固定長文字列と可変長文字列の違い

char(10)とvarchar(10)を例にして説明します。

固定長文字列は、定義したバイト数分の領域を使用します。  
半角1文字(1バイト)で登録しようとすると、残りの9バイトが余白として10バイトのデータとして登録されます。  

可変長文字列は、登録するデータ分の領域のみ使用します。  
半角1文字（1バイト）で登録しようとすると、1バイトのデータとして登録されます。  

### Unicode文字列型

``` txt
nchar[(n)]          :: Unicode、固定長、文字列型。1～4,000文字
nvarchar[(n | max)] :: Unicode、可変長、文字列型。1～4,000文字 max：最大値。2^31-1 バイト (2 GB)
ntext               :: 2^30 – 1 (1,073,741,823) 文字以内の可変長の Unicode データを格納する
```

>NCHAR/NVARCHAR型
national character/national character varying（SQL-92での表記法）
固定長文字列（Unicode）/可変長文字列（Unicode）
NVARCHARはSQL-Server2000では4000文字まで、VARCHAR2は8000文字まで格納可です。

#### charとnchar、varcharとnvarchar の違い

nが付かない char や varchar はShift-JISです。  
つまり、半角だと1文字1バイトで、全角だと1文字2バイトになります。  
char(10)や varchar(10)は最大10バイトまでという意味になります。  

nが付く nchar や nvarchar はUnicodeです。  
つまり、半角全角にかかわらず、1文字2バイトになります。  
nchar(10)や nvarchar(10)は最大10文字までという意味になります。  

また、Unicodeであるnchar や nvarchar は外国の文字を格納することが可能です。  
Shift-JISであるchar や varchar に外国の文字を格納すると、文字化けします。  

### 非推奨のデータ型

[SQLServerで非推奨となったデータ型 ntext text image](https://johobase.com/sqlserver-deprecated-data-type/)  

・ntext  
・text  
・image  

代替案  
text → varchar(max)  
ntext → nvarchar(max)  
image → varbinary(max)  

---

## SQL Server のトランザクション命令

BEGIN TRANSACTION  
COMMIT TRANSACTION  
ROLLBACK TRANSACTION  

---

## SQL Server のロック命令

``` C#
   .AppendLine("SELECT")
   .Append(GetFields())
   .AppendLine("FROM [TRe_ReservationExclusive] WITH (TABLOCKX)");
```

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

## SQL Server のGOコマンド

[SQL ServerのGOコマンドとは？](https://sql-oracle.com/sqlserver/?p=708)  
[SQL Server のユーティリティのステートメント - GO](https://docs.microsoft.com/ja-jp/sql/t-sql/language-elements/sql-server-utilities-statements-go?view=sql-server-ver16)  

---

## null許可のフィールドを追加→変更の保存が許可されていません

[変更の保存は、エラー メッセージで許可SSMS](https://docs.microsoft.com/ja-jp/troubleshoot/sql/ssms/error-when-you-save-table)  

科目大区分を新規作成するに当たり、科目区分テーブルにnull許可の科目大区分CDフィールドを追加しようとしたら発生した。  
フィールドを追加する際に結構発生するイメージがある。  
オプション一つで解決可能であるが、備忘録として残しておく。  

1. [ツール] メニューの [オプション] をクリックします。  
2. [オプション] ウィンドウのナビゲーション ウィンドウ で 、[デザイナー] をクリックします。  
3. [テーブルの再作成を必要とする変更の保存を防止する] チェックボックスをオンまたはオフにし 、[OK] をクリックします。  

[SQL Server カラム定義を変更すると「変更の保存が許可されていません」が表示された場合の対処法](https://nasunoblog.blogspot.com/2013/10/sql-server-column-edit-error.html)  
このサイトのほうが読みやすい。  
因みにこの現象が発生する原因としてはテーブルの監視履歴が削除されてしまうからだそうで。  

---

## テーブルデザイナーにコメント列を追加する方法

あると便利といわれたのでまとめ。  
レジストリをいじらないと駄目です。  

[SSMS のテーブルデザイナの列をカスタマイズした](https://qiita.com/d01tsumath/items/906043d69f86a6a53cef)  
[SQL Server Management Studio のテーブルデザイナの列をカスタマイズする](https://blog.xin9le.net/entry/2018/06/17/165526)  

---

## SQL SERVER のバックアップの仕方

1. バックアップしたいテーブルを右クリックする。  
2. [タスク∟バックアップ] を選択する。  
3. 全般のバックアップ先に[保存先]と[保存名.bak]を設定する。  
4. オプション ∟ メディア ∟ 上書きのオプションボタンを[既存のすべてのバックアップセットを上書きする]を選択する。  
5. OKを押してバックアップを実行する。  

---

## バックアップからの復元の仕方

1. 適当なデータベース右クリックする。  
2. 新しいデータベースの作成を選択する。  
3. データベース名を設定する。  
4. 特に何も設定しないでOKボタンを押す。  
5. 作成したDB右クリック
6. タスク→復元→データベース

●全般  
・ソース欄のチェックボタンをデータソースからデバイスに変更。  
・デバイス入力欄右にあるメニューを開く。  
・追加ボタンを押す。  
・バックアップする.bakファイルを選択する。  
・OKボタンを押して復元するファイルを確定する。  
・転送先を作成したデータべースに変更する。(この変更先を間違えてはいけない)  

●ファイル  
・[すべてのファイルをフォルダーに移動する] チェック入れない  
・論理ファイル名、ファイルの種類、元のファイル名、復元先と並んでいるリスト部分において復元先に書いてあるパスの最後のほうにあるデータベース名を作成したデータべースに変更する。(ここも間違えてはいけない)  
・2つリストに登録されているが、どちらのデータベース名も変更する。  

●オプション  
・[既存のデータベースを上書きする] チェック入れる  
・[ログ末尾のバックアップ] チェック外す(チェックが入っていることがある)  

---

## Insert Go文の作り方

1. データベースを右クリック  
2. タスク→スクリプトの生成  
3. 次へボタンを押す  
4. 特定のデータベースオブジェクトの選択  
5. データベースの中から取り出したいテーブルを選択して次へ  
6. スクリプトを指定した場所に保存  
7. 詳細設定ボタン  
   ・全般の中のUSE DATABASEのスクリプトを作成をFalseに  
   ・サーバーのバーション互換のスクリプトをSQLServer2008R2に  
   ・スクリプトを作成するデータの種類をデータのみに設定  
8. クリップボードにコピーか新しいクエリウィンドウに保存  
9. 次へを押していけば完了  

---

## SQLServer 列追加 位置

[浦下.com](https://urashita.com/archives/13652)  

SQLServerManagementStudio でならデザイナで任意の場所に列を追加することが可能。  

T-SQL(Transact-SQL)でやる場合、そのようなコマンドは存在しないので以下のような手順を踏んでフィールドを追加する必要あり。  
そもそも、MySQL以外のデータベースではカラム追加時の位置指定が軒並み不可能な模様。  

1. テーブルをバックアップ  
2. テーブルを削除  
3. テーブルを再作成  
4. 再作成したテーブルにバックアップからリストア  

``` SQL
BEGIN TRAN
BEGIN TRY
    -- ①対象テーブルのデータをテンポラリテーブルにバックアップ
    SELECT * INTO #Temp FROM TBL_USER

    -- ②対象テーブルを削除
    DROP TABLE TBL_USER

    -- ③カラムを追加して対象テーブルを再作成
    CREATE TABLE TBL_USER
    (
        UserNo int NOT NULL DEFAULT (0),
        Name nvarchar(255) NOT NULL DEFAULT (),
        Age int NOT NULL DEFAULT (0),
        Addr nvarchar(255) NOT NULL DEFAULT (),
        Tel nvarchar(255) NOT NULL DEFAULT (),
        CONSTRAINT TBL_USER_PK PRIMARY KEY CLUSTERED (UserNo)
    )

    -- ④テンポラリテーブルからバックアップデータを復元
    INSERT INTO TBL_USER
    SELECT
        Tmp.UserNo,
        Tmp.Name,
        0,
        Tmp.Addr,
        Tmp.Tel
    FROM #Temp Tmp

    -- ⑤テンポラリテーブルを削除
    DROP TABLE #Temp

    COMMIT TRAN
END TRY
BEGIN CATCH
    ROLLBACK TRAN
    SELECT
        ERROR_NUMBER() AS ErrorNumber,
        ERROR_SEVERITY() AS ErrorSeverity,
        ERROR_STATE() AS ErrorState,
        ERROR_PROCEDURE() AS ErrorProcedure,
        ERROR_LINE() AS ErrorLine,
        ERROR_MESSAGE() AS ErrorMessage
END CATCH
```

一番最後に列を追加するのは当たり前なので割愛するが、コマンドはこんな感じ。

``` sql : TBK_USERに年齢カラムを追加するクエリ
ALTER TABLE TBL_USER ADD Age int DEFAULT 0
```

---

## テーブル構造を出力する

``` sql
EXEC sp_help [TRe_ReservationPlayer2];
```

[【SQL Server】SSMSを使用して、各種定義やレコードをエクスポートする](https://sqlserver.work/2020/06/28/ssms%E3%82%92%E4%BD%BF%E7%94%A8%E3%81%97%E3%81%A6%E3%80%81%E5%90%84%E7%A8%AE%E5%AE%9A%E7%BE%A9%E3%82%84%E3%83%AC%E3%82%B3%E3%83%BC%E3%83%89%E3%82%92%E3%82%A8%E3%82%AF%E3%82%B9%E3%83%9D%E3%83%BC/)  

CREATE TABLEする時のSQL文を出力するためにはスクリプトを実行する必要あり。  

1. データベースを右クリック  
2. タスク→スクリプトの生成  
3. 次へボタンを押す  
4. 特定のデータベースオブジェクトの選択  
   - 対象テーブルを選択して次へ  
5. クリップボードに保存  
6. 詳細設定→[スクリプトを作成するデータの種類]が[スキーマのみ]になっていることを確認  
7. 次へを押していけば完了  

---

## sqlserver 断片化  

[SQL Serverの断片化したインデックスを再構築する方法](https://www.fenet.jp/dotnet/column/database/sql-server/4365/)  

インデックスの断片化とは
毎日稼働しているシステムは、日々テーブルにデータがため込まれていきます。  
トランザクションテーブルのように最終行にデータが追加されるテーブルもあれば、マスタテーブルのように行の途中にデータを追加、またはインデックスのキー値を変更するテーブルも存在します。  
テーブルによっては、データの物理的な順序や論理的な順序がバラバラになることがあり、このことを「断片化」と呼びます。  

断片化の状況は、断片化率を見ることで確認可能です  

``` sql
declare @DB_ID int
  ,@OBJECT_ID int
 
set @DB_ID = DB_ID('DB_TEST')
set @OBJECT_ID = OBJECT_ID('USER')
 
select *
from sys.dm_db_index_physical_stats(@DB_ID, @Object_ID, null, null, 'DETAILED') as A
join sys.objects as B on A.object_id = B.object_id
```

---

## SQLServer 改行

[T-SQL: 文字列に改行を挿入する](https://sql55.com/query/how-to-insert-carriage-return.php)  

``` sql
SELECT 'test1' + CHAR(13) + CHAR(10) + 'test2' AS 'CR+LF';

-- 以下は改行されない
SELECT 'test1' + CHAR(9) + 'test2' AS 'TAB';
SELECT 'test1' + CHAR(10) + 'test2' AS 'LF';
SELECT 'test1' + CHAR(13) + 'test2' AS 'CR';
```
