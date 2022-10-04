# SQLServer関係メモ

[SQL Serverチートシート](https://qiita.com/esflat/items/7885e53737163eb955fe)  

---

## null許可のフィールドを追加→変更の保存が許可されていません

フィールドを追加する際にたまに発生するので備忘録として残しておく。  
発生原因としてはテーブルの監視履歴が削除されてしまうからだそうです。  

1. [ツール] メニューの [オプション] をクリックします。  
2. [オプション] ウィンドウのナビゲーション ウィンドウ で 、[デザイナー] をクリックします。  
3. [テーブルの再作成を必要とする変更の保存を防止する] チェックボックスをオンまたはオフにし 、[OK] をクリックします。  

[SQL Server カラム定義を変更すると「変更の保存が許可されていません」が表示された場合の対処法](https://nasunoblog.blogspot.com/2013/10/sql-server-column-edit-error.html)  

[変更の保存は、エラー メッセージで許可SSMS](https://docs.microsoft.com/ja-jp/troubleshoot/sql/ssms/error-when-you-save-table)  

---

## テーブルデザイナーにコメント列を追加する方法

あると便利といわれたのでまとめ。  
レジストリをいじらないと駄目です。  

[SSMS のテーブルデザイナの列をカスタマイズした](https://qiita.com/d01tsumath/items/906043d69f86a6a53cef)  
[SQL Server Management Studio のテーブルデザイナの列をカスタマイズする](https://blog.xin9le.net/entry/2018/06/17/165526)  

---

## SQL Server のRand関数を用いたテストデータの作成

[SQLで乱数を使ってテストデータを作る](https://nonbiri-dotnet.blogspot.com/2017/04/sql.html)

``` SQL
-- とりあえず5回ループして乱数を生成するサンプル
DECLARE @counter smallint
SET @counter = 0

WHILE @counter < 5
BEGIN
    SELECT rand() * 100
    SET @counter = @counter + 1
END
```

---

## FLOOR関数とCEILING関数

Y君から、「これ何したいかわかります？」って質問されたが、そもそも関数が何やるかわからなかったのでまとめる。  

``` SQL
    CEILING(FLOOR(金額 * 10) / 10)
```

- FLOOR : 床→切り下げ  
- CEILING : 天井→切り上げ  

なるほど。言葉の意味そのままなのね。  
因みにROUNDは四捨五入ね。

``` txt : 金額が「112.23」と来た場合
デフォ  →  112.23
*10     →  1122.3
FLOOR   →  1122  
/10     →  112.2 
CEILING →  113   
```

普通に112.23の地点でCEILING使えばよくね？ってなるよね。  
修正の履歴を見る限り、過去の修正を生かしつつ、辻褄を合わせるためにこうなったのだろうという結論で終結した。  

---

## Identityの発番テーブルで保存時に発番した値を取得する

`OUTPUT句`なるものがあるらしい。  
部長が知ってた。  

fkdさんはこんなのも提示した。  
`var id = rnWebConnection.QueryFirstOrDefault<long>("SELECT LAST_INSERT_ID()");`  

``` sql
INSERT INTO IDENTITY_TABLE
(OfficeCD)
OUTPUT
inserted.SeqNo
values('aaa')
```

---

## DISTINCTとワイルドカードの併用

DISTINCTとワイルドカード `*` を併用したら.NETFrameworkでは実行速度が遅くなるらしい  

---

## TOP 句 PERCENT

[SqlServerの SELECT TOP 100 PERCENT で作成したビューの並び順は『保証されない』](https://culage.hatenablog.com/entry/20170824/p1)  
[TOP 句 PERCENT](https://haradago.hatenadiary.org/entry/20180214/p1)  

TOPにPERCENT句を付けると上位N%のデータを取得することができる  

``` sql
select TOP 50 PERCENT * FROM TastTeble

--  100 : 90121件
--   50 : 45061件
--    1 :   902件
-- -100 : '-' 付近に不適切な構文があります。
-- 5000 : パーセント値は 0 から 100 までの値を指定してください。
```

### TOP 100 PERCENTをやる意味とは？

ビューを呼び出したときに最初からソートされているとありがたい。  
ビューを取得してから改めてOrderByする手間は省きたい。  
そういう訳で、それを実現するためのテクニックとして、この方法が用いられてきたみたいだが、Microsoft的には、この動作は意図した動作ではなかったので非推奨になったらしい。  
というわけで、今はほとんど意味がないんだとか。  

実務で使っていたのは、全てのデータを取得するメソッドで並び替えまで済ませ、そのあとで必要なフィールドだけを抽出する動作を実現したかったからだと思われる。  
完全なビューとして運用するわけではないので、この方法が有効だったのかもしれない。  
疑似的なビューとしてこの方法を用いた可能性がある。  

[Road to DBD](https://plaza.rakuten.co.jp/jamshid/diary/200805080000/)  
>Viewやテーブル関数にTOP 100 PERCENTを指定すると、ORDER BYが切れるようになるというのは、SQL Server 2000時代でもある意味Tipsとして通用していたと思うが、SQL Server 2005ではこれが使えなくなったのだった。  
>「TOP 100 PERCENTであれば、全件を返すわけだから、オプティマイザは並べ替えを行わない」という変更が行われた。  

[ビューのソートについて](https://oshiete.goo.ne.jp/qa/4420769.html)  
>ビューを単純に照会したときにORDER句を切らなくても希望する並び順でSELECTできれば、運用上は便利なことが多いです。  
>そのため、SQL Serverでは昔からTipsでTOP 100 PERCENTで並び替える方法が認識されていました。  
>（Tipsとある通り、普通はViewの外でOrder切ります）  
>ただし、SQL Server 2005になって、TOP 100 PERCENTは「並び替えの必要なし」とオプティマイザが判断するように仕様が変更されてしまいました。  
>そのため、SQL Server 2005ではTOP 100 PERCENT句をViewに切っても並び替えは起こりません。  
>それだけなら分かりやすいのですが、やはりこの技を使っていた人が多かったためでしょうか。  
>SP2の後の累積パッチ「SP2の累積プログラムその２」でこれを修正するモジュールが提供されています。  
>SP2には入っていないし、わざわざ当てる人も少ないと思うので、次のSP3が出たとして、それ当てた時からTOP 100 PERCENTが効くようになるでしょう。  
>ちなみにSQL Server 2008でも初期バージョンではTOP 100 PERCENTが効かず、累積パッチが提供されています。  
>ということで、そのようなビューの使い方は正しいアプローチではありませんが、SQL Serverの裏ワザの一つですと認識ください。  

---

## 移行におけるあれこれ

Aデータベースの動作が不安定なのでBデータベースを新しく立てて、そちらにデータを移動させることになった。  
デタッチしてmdfファイルを直接移動させて、アタッチする方式を取ろうとしたが、エラーが発生してうまく行かなかった。  
エラーの内容は以下の通り。  

``` txt
TITLEMicrosoft SQL Server Management Studio
------------------------------

データベース 'sample_database' の復元に失敗しました。 (Microsoft.SqlServer.Management.RelationalEngineTasks)

------------------------------
ADDITIONAL INFORMATION:

Transact-SQL ステートメントまたはバッチの実行中に例外が発生しました。 (Microsoft.SqlServer.SmoExtended)

------------------------------

データベース 'sample_database' は、オブジェクト 'sample_tabel' の一部または全体でデータ圧縮または vardecimal ストレージ形式が有効になっているため、このエディションの SQL Server では開けません。データ圧縮および vardecimal ストレージ形式がサポートされているのは、SQL Server Enterprise Edition だけです。
データベース 'sample_database' は、このエディションの SQL Server では起動できません。このデータベースには、パーティション関数 'sample_func' が含まれています。パーティション分割は SQL Server Enterprise Edition でしかサポートされません。
SQL Server の現在のエディションではデータベース機能の一部が使用できないため、データベース 'sample_database' を開くことができません。 (Microsoft SQL Server、エラー: 909)

ヘルプを表示するには https://docs.microsoft.com/sql/relational-databases/errors-events/mssqlserver-909-database-engine-error をクリック
```

[sqlserver error 909]  
このエラー自体は上位エディション(Enterprise Edition)から下位エディション()に移行した時に発生する現象な模様。  
しかし、`SELECT SERVERPROPERTY('Edition');`このクエリで確認してみると、AもBもStandard Editionであった。  

色々調べた結果、テーブルに圧縮の設定がされている場合に発生してしまう模様。  
調査クエリで確認したり、圧縮の設定を解除するクエリを流したり、直接テーブルの設定を変えたらうまく行った。  

DB単位で Enterprise Edition のどの機能を使用しているかを確認するクエリ

``` sql
select * from sys.dm_db_persisted_sku_features
```

データ圧縮されているオブジェクトのリストを表示するクエリ

``` sql
SELECT
    SCHEMA_NAME(sys.objects.schema_id) AS SchemaName
    ,OBJECT_NAME(sys.objects.object_id) AS ObjectName
    -- VarDecimalStorage 形式の圧縮があるかどうかを確認する
    ,OBJECTPROPERTY ( OBJECT_ID ( OBJECT_NAME(sys.objects.object_id) ), 'TableHasVarDecimalStorageFormat' )
    ,[rows]
    ,[data_compression_desc]
FROM sys.partitions
INNER JOIN sys.objects ON sys.partitions.object_id = sys.objects.object_id
WHERE data_compression > 0 AND SCHEMA_NAME(sys.objects.schema_id) != 'SYS'
ORDER BY SchemaName, ObjectName
```

データ圧縮オプションを「なし」にしてインデックスを再構築し、圧縮を無効にするクエリ。  

``` sql
ALTER INDEX ALL ON sample_table REBUILD WITH ( DATA_COMPRESSION = None );
```

上記クエリは1行ずつしか実行できないのでカーソル処理としたもの。  

``` sql
--カーソルの値を取得する変数宣言
DECLARE @ObjectName varchar(255)

--カーソル定義
DECLARE CUR_AAA CURSOR FOR
    SELECT OBJECT_NAME(sys.objects.object_id) AS ObjectName
    FROM sys.partitions
    INNER JOIN sys.objects ON sys.partitions.object_id = sys.objects.object_id
    WHERE data_compression > 0 AND SCHEMA_NAME(sys.objects.schema_id) != 'SYS'
    ORDER BY ObjectName

--カーソルオープン
OPEN CUR_AAA;

--最初の1行目を取得して変数へ値をセット
FETCH NEXT FROM CUR_AAA
INTO @ObjectName;

--データの行数分ループ処理を実行する
WHILE @@FETCH_STATUS = 0
BEGIN

    -- ========= ループ内の実際の処理 ここから===
    EXEC('ALTER INDEX ALL ON ' + @ObjectName + ' REBUILD WITH ( DATA_COMPRESSION = None );')
    -- ========= ループ内の実際の処理 ここまで===

    --次の行のデータを取得して変数へ値をセット
    FETCH NEXT FROM CUR_AAA
    INTO @ObjectName;
END

--カーソルを閉じる
CLOSE CUR_AAA;
DEALLOCATE CUR_AAA;
```

なんかのついでに拾ったクエリ。  
一応参考程度に残しておく。  

``` sql
SELECT s.[name]+'.'+o.[name] AS [object], i.[type_desc] COLLATE database_default+ISNULL(' '+i.[name], '') AS index_name,
       (CASE WHEN COUNT(DISTINCT p.partition_number)>1 THEN 'Is partitioned' ELSE '' END) AS [partitioned?],
       ISNULL(MIN(NULLIF(p.data_compression_desc, 'NONE'))+' compression', '') AS [compressed?],
       (CASE WHEN ISNULL(OBJECTPROPERTY(p.[object_id], 'TableHasVarDecimalStorageFormat'), 0)=0 THEN '' ELSE 'vardecimal' END) AS [vardecimal?]
FROM sys.partitions AS p
INNER JOIN sys.indexes AS i ON p.[object_id]=i.[object_id] AND p.index_id=i.index_id
INNER JOIN sys.objects AS o ON i.[object_id]=o.[object_id]
INNER JOIN sys.schemas AS s ON o.[schema_id]=s.[schema_id]
GROUP BY p.[object_id], s.[name], o.[name], i.index_id, i.[type_desc], i.[name]
ORDER BY s.[name], o.[name], i.index_id
```

[Microsoft SQL Server: エラー: 909 データベースを SQL Server 2008 の SQL Server Enterprise Edition から任意の下位エディションに移行中に発生する](http://www.thesqlpost.com/2012/06/microsoft-sql-server-error-909-while.html)  
[SQL Server 2008 R2 で新しい (ContosoRetailDW) を作成するときのエラー 909。エラー 909 を受け取り、](https://social.msdn.microsoft.com/Forums/en-US/0057feb1-2034-448c-a9ea-c66484b048ba/error-909-when-creating-new-contosoretaildw-on-sql-server-2008-r2-receiving-error-909?forum=sqlgetstarted)  
[Enterprise Edition の DB バックアップを Standard Edition にリストア](https://blog.engineer-memo.com/2014/12/24/enterprise-edition-%E3%81%AE-db-%E3%83%90%E3%83%83%E3%82%AF%E3%82%A2%E3%83%83%E3%83%97%E3%82%92-standard-edition-%E3%81%AB%E3%83%AA%E3%82%B9%E3%83%88%E3%82%A2/)  

---

## FORMAT関数

FORMAT関数が使えるのは2012から。  
2008R2では分析ツール用のクエリでは使えるが、組み込み関数としては使えない。  

[SQLServerのFORMAT関数にハマったのでメモ (2008 R2では使えない)](https://devlights.hatenablog.com/entry/2015/03/09/143355)  

---

## テーブルを複製する方法

``` sql
SELECT * INTO コピー先テーブル名 FROM コピー元テーブル名
```

`コピー元テーブル名`は名前の指定だけあればよい。  
あらかじめテーブルを作成する必要は無し。  
実行するとテーブルもSQLServerが自動でテーブルも作成してくれます。  
※注：ただし、テーブルは規定の領域（なにも設定してなければPrimary）に作られるので、ファイルグループ管理は要注意。  

[[SQLServer]テーブルをまるっとコピーする方法](https://ameblo.jp/nature3298type-s/entry-10313449987.html)
