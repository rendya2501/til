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

## SQL Server のデッドロック対応

[SQLServer: with(nolock)ヒントでロックを確実に回避できるという認識は間違い](https://qiita.com/maaaaaaaa/items/209a681f0a771cf80df4)  
[SQL Serverのwith(NOLOCK)の挙動について](https://tech.excite.co.jp/entry/2021/05/22/120000)  
[SQLServerのテーブルロック状態を取得するSQL](https://www.excellence-blog.com/2016/11/11/sqlserver%E3%81%AE%E3%83%86%E3%83%BC%E3%83%96%E3%83%AB%E3%83%AD%E3%83%83%E3%82%AF%E7%8A%B6%E6%85%8B%E3%82%92%E5%8F%96%E5%BE%97%E3%81%99%E3%82%8Bsql/)  
[SQL Serverのロックについて出来る限り分かりやすく解説](https://qiita.com/maaaaaaaa/items/38fd95b142b07acf7700)  
[続・SQL Serverのロックについて出来る限り分かりやすく解説](https://qiita.com/maaaaaaaa/items/28c8a1affe36a6bd811a)  

[SQL SERVERにおけるデッドロック（内部仕様）〜クラスタ化インデックスと非クラスタ化インデックス間のデッドロック〜](https://bxdxmx.hatenablog.com/entry/20090820/1250746566)  
[デッドロック（SQL Server）](https://www.dbsheetclient.jp/blog/?p=1609)  
[デッドロックのサンプルクエリ](https://blog.engineer-memo.com/2013/07/15/%E3%83%87%E3%83%83%E3%83%89%E3%83%AD%E3%83%83%E3%82%AF%E3%81%AE%E3%82%B5%E3%83%B3%E3%83%97%E3%83%AB%E3%82%AF%E3%82%A8%E3%83%AA/)  
[SQL Server 2012 のデッドロック情報の取得について](https://blog.engineer-memo.com/2012/04/19/sql-server-2012-%E3%81%AE%E3%83%87%E3%83%83%E3%83%89%E3%83%AD%E3%83%83%E3%82%AF%E6%83%85%E5%A0%B1%E3%81%AE%E5%8F%96%E5%BE%97%E3%81%AB%E3%81%A4%E3%81%84%E3%81%A6/)  
[【SQL server】デッドロックの調査方法](https://memorandom-nishi.hatenablog.jp/entry/2016/11/14/024856)  
[EXISTSとSQLの高速化について](http://kkoudev.github.io/blog/2013/09/14/sql/)  

[SQL Serverで処理時間を計測する](https://qiita.com/maitake9116/items/ed3037badc90de18b0e6)  

[SQL Server の Wait Resource から実オブジェクトを判断する](https://blog.engineer-memo.com/2019/10/06/sql-server-%E3%81%AE-wait-resource-%E3%81%8B%E3%82%89%E5%AE%9F%E3%82%AA%E3%83%96%E3%82%B8%E3%82%A7%E3%82%AF%E3%83%88%E3%82%92%E5%88%A4%E6%96%AD%E3%81%99%E3%82%8B/)  

[クラスター化インデックスと非クラスター化インデックスの概念](https://docs.microsoft.com/ja-jp/sql/relational-databases/indexes/clustered-and-nonclustered-indexes-described?view=sql-server-ver15)  
[【SQL Server】クラスター化インデックスと非クラスター化インデックス](https://memorandom-nishi.hatenablog.jp/entry/2017/02/05/232703)  

[sys.dm_exec_sql_text (Transact-SQL)](https://docs.microsoft.com/ja-jp/sql/relational-databases/system-dynamic-management-views/sys-dm-exec-sql-text-transact-sql?view=sql-server-ver15)

``` sql
-- pass sql_handle to sys.dm_exec_sql_text
SELECT * FROM sys.dm_exec_sql_text(0x01000600B74C2A1300D2582A2100000000000000000000000000000000000000000000000000000000000000) -- modify this value with your actual sql_handle
```

[エスカレーション 件数　sqlserver]  
[sqlserver デッドロック キーロック]  
[sqlserver 非クラスター化インデックス 含まれている列]  

インデックスは必須であることは分かったが、根本的な原因は分からなかった。  
色々勉強にはなったが、実力不足が否めない。  

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
