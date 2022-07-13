# SQLServer関係メモ

[SQL Serverチートシート](https://qiita.com/esflat/items/7885e53737163eb955fe)  

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

## CREATE TABLEのスクリプト出力

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

## 対象カラムが存在するかどうかをチェックするクエリ

``` SQL
    SELECT *
    FROM   [Database].sys.columns
    WHERE  Name = N'FieldName'
    AND    Object_ID = OBJECT_ID(N'[Database].[dbo].[Table]')
```

---

## sqlserver 断片化  

[SQL Serverの断片化したインデックスを再構築する方法](https://www.fenet.jp/dotnet/column/database/sql-server/4365/)  

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

## FOR XML PATH の TYPE .valueとは何か？

for xml pathは、なんかインテリセンスが働かないけど、`,TYPE).value(,)`なるオプション？が使える模様。  
結論からいうと、型を変換するための命令っぽい。  
別にそこまで厳密に型指定しなくても動く。  
しかし、型の変換が必要な場合もあるのだろう。  

``` SQL
-- このSQLを実行しても上でやったSQLの結果と違うことはない。
-- ABC202007250541,ABC202007250541,ABC202007250541
SELECT STUFF(
    (SELECT TOP 3 ',' + TestID FROM TestTable FOR xml path(''),TYPE).value('.', 'NVARCHAR(MAX)'),
    1,
    1,
    ''
)
```

[Please explain what does "for xml path(''), TYPE) .value('.', 'NVARCHAR(MAX)')" do in this code](https://dba.stackexchange.com/questions/207371/please-explain-what-does-for-xml-path-type-value-nvarcharmax)  

私は人々が時々`, TYPE).value('.', 'NVARCHAR(MAX)')`のテクニックを使うのを省略しているのを見ます。  
これに伴う問題は、一部の文字をXMLエンティティ参照（引用符など）でエスケープする必要があるため、その場合、結果の文字列が期待どおりにならないことです。  
→  
まぁ、やっておいて損はないということだな。  
省略せず書くべし!!  

[SQLServerで複数レコードの文字列を結合](http://icoctech.icoc.co.jp/blog/?p=998)  
FOR XML句で出力した値はXML形式で出力されるため、他のフィールドとは扱いが異なりますので、value()メソッドを使い、通常のSQL型に変換します。  
サブクエリを表す()の後に「.value(‘.’, ‘VARCHAR(MAX)’)」と記述しております。  
value()メソッドの第一引数はXQuery式で、第二引数はSQL型となります。  
第一引数の「'.'」はXQuery式「self::node()」の省略形、  
第二引数では、ユーザー名を扱い、出力文字数の制限をしたくないので、VARCHAR(MAX)としており、  
出力されたXML形式のデータからvalue値を取得するため、  
属性が指定されていようが関係なく、「・アイチャン・ワークン」といったvalue値を取得し、  
第二引数で指定されたVARCHARとして、SQL型の結果を返します。  

[value() メソッド (xml データ型)](https://docs.microsoft.com/ja-jp/sql/t-sql/xml/value-method-xml-data-type?redirectedfrom=MSDN&view=sql-server-ver15)  
意外と奥が深かった。  

---

## 2つの表を比較して存在しない行をINSERTする

<https://www.projectgroup.info/tips/SQLServer/SQL/SQL000001.html>

完全にExistsの使い片のサンプルではある。  
存在するカラムを指定したいなら大抵INやJOINを使うので、EXISTSを使う機会はあまりない。  
唯一、IDを比較して存在しないIDのみINSERTしたい場合等は役に立つ。  
LEFT JOIN してIDがNULLってやり方でもいいかもしれないが、まぁまぁ。  
というわけで、いい機会なのでまとめることにする。  

【サンプル】  
「CODE」項目で比較し、「商品マスタ」に存在しないデータを「購入リスト」からINSERTします。  

``` txt
商品マスタ    |  購入リスト     | 商品マスタ

CODE  NAME    |  CODE  NAME     | CODE  NAME
0001  りんご  |  0001  りんご   | 0001  りんご
0003  みかん  |  0002  ぶどう   | 0002  ぶどう
0004  ばなな  |  0003  みかん   | 0003  みかん
              |  0004  ばなな   | 0004  ばなな
```

EXISTSが返すのはboolなのでEXISTS内におけるSELECTは何でもいい。  
EXISTSにおいて重要なのはWHEREでテーブルAとBを結びつけるフィールドを指定すること。  

EXISTSステートメントは本来「trueかfalse（存在するかしないか）」を返すためのものなので、  
上記のように「codeが合致するかどうか」という条件を付け加えなければ出力結果を絞ることはできない。  

<https://itsakura.com/sql-exists>  
外側のフィールドと連携させた場合、外側のSQL(主問い合わせ）を実行し、そこで取得した行でexists句内(副問い合わせ)のSQLを実行するらしい。  
そのコードが「あるかないか」の判定になる。値同士の比較を行わない。  

``` SQL
insert into 商品マスタ
select * 
from   購入リスト TAB_B
where not exists(
    select 'X' 
    from   商品マスタ TAB_A
    where  TAB_A.CODE = TAB_B.CODE
)
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

## IF NOT EXIST

なければ実行、あれば何もしないサンプル

``` sql
IF NOT EXISTS
(
    SELECT TOP 1 1 FROM TestTable WHERE [Code] = 99
)
    -- 上記select文の結果がfalseならInsertが実行される。
    INSERT INTO
    ~~~
GO
```

---

## テーブルを複製する方法

[[SQLServer]テーブルをまるっとコピーする方法](https://ameblo.jp/nature3298type-s/entry-10313449987.html)

``` sql
SELECT * INTO <コピー先テーブル名> FROM <コピー元テーブル名>
```

`<コピー元テーブル名>`は名前の指定だけあればよい。  
あらかじめテーブルを作成する必要は無し。  
実行するとテーブルもSQLServerが自動でテーブルも作成してくれます。  
※注：ただし、テーブルは規定の領域（なにも設定してなければPrimary）に作られるので、ファイルグループ管理は要注意。  

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
