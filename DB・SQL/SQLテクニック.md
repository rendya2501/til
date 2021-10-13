# SQLテクニック

## 横縦変換 : CROSS APPLY

[複数列のデータを縦に並べる方法【SQLServer】](https://qiita.com/sugarboooy/items/0750d0ccb83a2af4dc0e)
`たて→よこ`ではない。`よこ→たて`である。  
似たような物にUNPIVOTがあるが、あちらは複数行に対応できないので、こちらを推奨された。  
たぶんSQL Server独自の関数だと思われるが、とても便利な物があるものだ。  
使い方はサンプルを見れば大体わかる。  

``` SQL : 実際に業務で使用したSQL
SELECT
    CONCAT('ALP', CONVERT(nvarchar,[TU_売掛残高].[営業日], 112), [v].[SlipNumber]) AS [SettlementID],
    [v].[Seq],
    (SELECT TOP 1 OfficeCD FROM [Round3Dat].[dbo].[TMa_CompanyBasicInfo]) AS [OfficeCD],
    [v].[PaymentDay],
    [v].[DepositAmount],
    [v].[AdjustmentAmount],
    [v].[PaymentCD],
    [v].[AccountCD]
FROM
    [RoundDatMigrationSource].[dbo].[TU_売掛残高]
CROSS APPLY(
    VALUES
        (伝票番号, 1, 入金日1, 入金額1, 調整額1, 入金CD1, 口座CD1),
        (伝票番号, 2, 入金日2, 入金額2, 調整額2, 入金CD2, 口座CD2),
        (伝票番号, 3, 入金日3, 入金額3, 調整額3, 入金CD3, 口座CD3),
        (伝票番号, 4, 入金日4, 入金額4, 調整額4, 入金CD4, 口座CD4),
        (伝票番号, 5, 入金日5, 入金額5, 調整額5, 入金CD5, 口座CD5)
) AS [v] ([SlipNumber], [Seq], [PaymentDay], [DepositAmount], [AdjustmentAmount], [PaymentCD], [AccountCD])
WHERE
    [v].[DepositAmount] <> 0
```

---

## ROW_NUMBER()

[SQL Fiddle](http://sqlfiddle.com/#!18/7374f/71)  
[よく使われる順位付け関数 1 - ROW_NUMBER](https://sql55.com/t-sql/sql-server-built-in-ranking-function-1.php)  
<https://style.potepan.com/articles/23566.html>  
連番を割り振る関数。連番の順番と区分ごとに連番を生成できるらしい。  

``` SQL
ROW_NUMBER ( ) OVER ( [ PARTITION BY [パティションカラム 1 ], [パティションカラム 2], ... ] ORDER BY [ソートカラム 1], [ソートカラム 2], ...  )
```

``` SQL
-- こんな感じで使うらしいよ。
-- これなら実務でも使うからイメージしやすいでしょう。
select ROW_NUMBER() OVER (PARTITION BY DepartMentCD ORDER BY UpdateDateTime DESC), * from TMa_Product

-- 関数 'ROW_NUMBER' には ORDER BY 句を伴う OVER 句が必要です。
-- こうやって怒られるので、最低限、ORDER BY は必要見たい。
select ROW_NUMBER() OVER (), * from TMa_Product
```

---

## SQL Server ループ処理

[SQL Server - WHILEによるループ(T-SQL)](https://www.curict.com/item/bb/bb80194.html)  

WHILEの書き方  

``` SQL
WHILE *ループ継続条件*
BEGIN
    *繰り返し実行したいコード*
END
-- ループ内でBREAKEが実行されるとループを抜けます。
-- ループ内でCONTINUEが実行されるとループの先頭に戻ります。
```

WHILE 10回ループ

``` SQL
--変数宣言
DECLARE @index INTEGER
--ループ用変数を初期化
SET @index = 0

WHILE @index < 10
BEGIN
    --ループ用変数をインクリメント
    SET @index = @index + 1
    PRINT @index
END
```

10回ループ(BREAKでループを抜ける)(まぁつかわんやろ)  

``` SQL
--変数宣言
DECLARE @index INTEGER
--ループ用変数を初期化
SET @index = 0

WHILE 1=1
BEGIN
    --ループ用変数をインクリメント
    SET @index = @index + 1
    IF @index > 10
    BEGIN
        --ループ終了
        BREAK
    END
    PRINT @index
END
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

## GroupByで文字列を集計

[SQLのGroupで、文字列を集計](https://qiita.com/nuller/items/01813da7f7d60b65c220)  

``` SQL:sql serverの場合
SELECT
    SlipID,
    (
        SELECT PlayerNo + ','
        FROM TFr_Slip AS t1
        WHERE t1.SlipID = t2.SlipID 
        FOR xml path('')
    )
FROM TFr_Slip t2
GROUP BY SlipID

-- SlipID                      PlayerNo
-- ALP20200725001000009001434  ALP2020072500655,ALP2020072500655,ALP2020072500655,ALP2020072500655,
-- ALP20200725001000011000942  ALP202007250384,ALP202007250384,ALP202007250384,ALP202007250384,
-- ALP20200725001000012000176  ALP2020072500069,ALP2020072500069,ALP2020072500069,ALP2020072500069,ALP2020072500069,
-- ALP20200725001000018001431  ALP2020072500654,ALP2020072500654,ALP2020072500654,ALP2020072500654,ALP2020072500654,
```

FOR XML PATHは指定したフィールドの要素をXMLタグで囲う動作をするらしい。  
なので仮に`SELECT TOP 3 PlayerNo + ',' FROM TFr_Slip FOR xml path('a')`と書いた場合、  
`<a>ALP202007250541,</a><a>ALP202007250541,</a><a>ALP202007250541,</a>`となる。  
PATH('')とするのは、空白のタグで前後を囲う動作というわけだ。  

[【MySQL】GROUP_CONCAT()を使ってみる](https://www.softel.co.jp/blogs/tech/archives/3154)  
GROUP_CONCAT関数 : group byしたときに任意の列の値を連結させる関数。  
mysql系独自の関数見たいなので、他では使えない。  

``` SQL:mariaDBの場合
SELECT
    SlipID,
    GROUP_CONCAT(PlayerNo,',') as PlayerNo
FROM TFr_Slip
GROUP BY SlipID
```

---

## FOR XML PATH で文字列を結合した後に前後のつなぎ文字を削除していい感じにするサンプル

2021/10/12 Tue  
居林さんと編み出したサンプル。  
`SELECT TOP 3 PlayerNo + ',' FROM TFr_Slip FOR xml path('')`だと`ALP202007250541,ALP202007250541,ALP202007250541,`となり、  
最後にコロンがついてしまうので、消したいなってことでやった。  

STUFF関数なるものを使うことで解決することができた。  
`STUFF(置換対象文字列、開始位置、終了位置、置換文字)`  
XML PATHのコロンの位置を先頭に持ってきて、先頭のコロンを消すことで実現した訳である。  

``` SQL
-- 置換対象,開始位置、終了位置、置換文字
SELECT STUFF( (SELECT TOP 3 ',' + PlayerNo FROM TFr_Slip FOR xml path('')) , 1 , 1 , '')
-- ALP202007250541,ALP202007250541,ALP202007250541
```

---

## FOR XML PATH の TYPE .valueとは何か？

for xml pathは、なんかインテリセンスが働かないけど、`,TYPE).value(,)`なるオプション？が使える模様。  
結論からいうと、型を変換するための命令っぽい。  
今回のサンプルに関しては別にそこまで厳密に型指定しなくても動く。  
しかし、型の変換が必要な場合もあるのだろう。  

``` SQL
-- このSQLを実行しても上でやったSQLの結果と違うことはない。
-- ALP202007250541,ALP202007250541,ALP202007250541
SELECT STUFF(
    (SELECT TOP 3 ',' + PlayerNo FROM TFr_Slip FOR xml path(''),TYPE).value('.', 'NVARCHAR(MAX)'),
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

## 本日の日付

``` SQL
-- ●SQL SERVER
SELECT GETDATE()
-- 他
-- SYSDATETIME(),
-- CURRENT_TIMESTAMP

-- ●MARIA DB
SELECT CURDATE();
SELECT DATE(NOW());
```

---

## SELECT GROUPで1件目を取得

<https://oshiete.goo.ne.jp/qa/3819843.html>  
「group by 先頭1件」で検索。  
Rank() Over構文を使うらしい。  

``` SQL
INSERT INTO
    TMa_SubjectSummary
SELECT
    (SELECT TOP 1 OfficeCD FROM TMa_CompanyBasicInfo) AS OfficeCD,
    x.集計CD AS SubjectSummaryCD,
    '' AS SubjectSummaryName,
    '' AS SubjectSummaryShortName,
    - 16777216 AS Color,
    - 1 AS BackgroundColor,
    x.集計CD AS Sort,
    1 AS ValidFlag,
    '' AS SearchKey,
    x.作成日 AS InsertDateTime,
    '' AS InsertStaffCD,
    '' AS InsertStaffName,
    x.更新Prg AS InsertProgram,
    x.ComputerName AS InsertTerminal,
    x.更新日 AS UpdateDateTime,
    '' AS UpdateStaffCD,
    '' AS UpdateStaffName,
    x.更新Prg AS UpdateProgram,
    x.ComputerName AS UpdateTerminal
FROM
(
    SELECT
        Rank() over(partition by CONVERT(int, 集計CD) order by 科目分類CD) as rk,
        CONVERT(int, 集計CD) AS 集計CD,
        作成日,
        更新日,
        更新Prg,
        ComputerName
    FROM 
        [RoundDatMigrationSource].[dbo].[TM_科目分類]
) AS x
WHERE 
    rk=1
```

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

商品マスタ   購入リスト     商品マスタ
CODE NAME   CODE NAME     CODE NAME
0001 りんご  0001 りんご   0001 りんご
0003 みかん  0002 ぶどう → 0002 ぶどう
0004 ばなな  0003 みかん   0003 みかん
            0004 ばなな   0004 ばなな

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

## 対象カラムが存在するかどうかをチェックする

``` SQL
    ,CASE
        WHEN EXISTS(
            SELECT *
            FROM   [Eco21_Otaru].sys.columns
            WHERE  Name = N'訂正区分'
            AND    Object_ID = OBJECT_ID(N'[Eco21_Otaru].[dbo].[TS_請求]')
        ) THEN ISNULL([A].[訂正区分],0)
        ELSE 0 --0:請求 小樽以外は0:請求しかありえない。 1:訂正
    END AS [CorrectClassification]
```

---

## ある列の値が最大もしくは最小の値のレコードを取得する

働かない頭で「sql minれこーど」で調べたらやりたいことが出てきてくれた。  

[SQLで同一グループの中で最大/最小のレコードを取得する](https://qiita.com/inouet/items/4f1d7f299725597d8407)  
[SQL ある列の値が最大もしくは最小の値のレコードを取得する](https://zukucode.com/2017/08/sql-row_number-technique.html)  
[特定のカラムのグループごとの最大値が格納されている行](https://dev.mysql.com/doc/refman/5.6/ja/example-maximum-column-group-row.html)  

SQLServerなら`ROW_NUMBER関数`を使っていい感じに実現できた。  
愚直にやるならMAXかMINで抽出して、そのサブクエリの中でさらに条件を絞って、最終定期に出力するみたいな感じになるみたい。  
またはEXITSのような相関副問い合わせやJEFT JOIN を組み合わせるやり方など、やり方はたくさんあるみたい。  

``` SQL
    SELECT
        [A].[SlipID],
        [A].[AccountNo],
        [A].[Name]
    FROM(
        SELECT
            -- SlipIDでまとめて、AccountNoを昇順で並べる。
            -- Whereで1番目を取れば、同一グループの中の最小値レコードを取得できる。
            ROW_NUMBER() OVER(PARTITION BY [TFr_Slip].[SlipID] ORDER BY [TRe_ReservationPlayer].[AccountNo]) [MinAccountNo],
            [TFr_Slip].[SlipID],
            [TRe_ReservationPlayer].[AccountNo],
            [TRe_ReservationPlayer].[Name]
        FROM
            [TFr_Slip]
            LEFT OUTER JOIN [TRe_ReservationPlayer]
            ON [TFr_Slip].[OfficeCD] = [TRe_ReservationPlayer].[OfficeCD]
            AND [TFr_Slip].[BusinessDate] = [TRe_ReservationPlayer].[BusinessDate]
            AND [TFr_Slip].[PlayerNo] = [TRe_ReservationPlayer].[PlayerNo]
        WHERE
            [TFr_Slip].[OfficeCD] = 'ALP'
            AND [TFr_Slip].[BusinessDate] = '2020/07/25'
            AND [TFr_Slip].[SlipType] = '150'
            AND [TFr_Slip].[DetailType] <> 99
        ) AS [A]
    WHERE
        [A].[MinAccountNo] = 1
```

---

## SQLでTrue,Falseだけの判定を返す方法

IsExist系の検索する時は、True,Falseだけわかればいいので、それをやる方法をまとめる。  
COUNTが2以上の場合でもTrue扱いになる。
CONVERT,BITとは。
やってることは単純だが、補足が必要。
後TOP1って必要あるのか？

``` SQL
-- 1件の有無で判定は十分なのでTOP1する。
SELECT TOP 1
CONVERT(BIT,COUNT([TRe_ReservationFrame].[ReservationFrameNo])),
CAST (COUNT([TRe_ReservationFrame].[ReservationFrameNo])) AS BIT)
FROM [TRe_ReservationFrame]
WHERE [TRe_ReservationFrame].[OfficeCD] = @officeCD
```

---

## Substring

最後の文字を削除する時に`substring([ReservationNo], 1, len(ReservationNo)-1)`ってやったらエラーになったけど何かわかる？って萬君に質問されたのでまとめ。  
答えられなかったのだが、Len(空白) = 0になって 0-1 = -1 で、マイナスを第3引数に指定するとエラーになるってのが原因だった。  
気になって調べたらいろいろわかったので沼にはまってしまった。  

SUBSTRING ( expression ,start , length )  
SUBSTRING (抜き出す文字列, 開始地点, 切り取り文字数)  

対象の文字列  
切り取る対象の文字列を指定します。  

開始位置  
切り取りの開始位置を指定します。  
勘違いしやすいですが、「1」スタートなので、「0」を指定すると何も取得できません。なお、マイナスにしても何も取得できません。  

取得する文字数  
正の整数または bigint 式を指定する。  
負の場合はエラーが生成され、ステートメントは終了する。  
start と length の合計が 対象の文字列 の文字数を上回る場合は、start の先頭から値式全体が返されます。  

``` SQL
select
-- 右1文字を削除する方法
substring([ReservationNo], 1, len(ReservationNo)-1),
-- 右1文字だけを抜き出す方法
substring([ReservationNo], len(ReservationNo), len(ReservationNo)),
-- 先頭1文字だけを抜き出す方法
substring([ReservationNo], 1, 1),
-- これでも先頭を取れる
substring([ReservationNo], 0, 1),
-- 全部空白になる
substring([ReservationNo], 0, 0),
-- 基本的に、lengthに0を指定すると空白になる。
substring([ReservationNo], 2, 0),
-- 超過しても全文表示されるだけ
substring([ReservationNo], 1, 1000),
-- 何も表示されない。
substring([ReservationNo], -100, 100),
-- 全部NULLになる
-- len(null) = null . null - 1 = null なのでnull
substring([ReservationNo], len(ReservationNo), NULL),
-- エラーになる
-- len(空白)の結果は0になる。この状態で-1するとエラーになる。
substring([ReservationNo], 1, -1),
from TRe_Reservation
```

---

## mariaDBで0埋めして文字列結合するサンプル

意外と調べるのに苦労したのでまとめる。  
GORAPriceに20とか30レコードもあるようなプランがあるか調べたかったので、  
IDを作る要領で3つのキーを0埋めして文字列結合して、GroupByしてCOUNT取ってHAVINGで20以上のIDを表示ってやつ。  

左0埋め→LPAD  
文字列結合→CONCAT  
数値→文字列変換→CAST(数値 AS CHAR)  
数値文字列変換に関してはLPADしてCONCATしたらそのまま行けたので別に必要ないのかも。  

``` SQL
SELECT
    CONCAT(LPAD(GolfCode, 4, '0'),LPAD(PlanCode, 6, '0'),LPAD(OpenPlanCode, 6, '0')),
    COUNT(CONCAT(LPAD(GolfCode, 4, '0'),LPAD(PlanCode, 6, '0'),LPAD(OpenPlanCode, 6, '0')))
FROM
    TmOpenPlanGORAPrice
GROUP BY
    CONCAT(LPAD(GolfCode, 4, '0'),LPAD(PlanCode , 6, '0'),LPAD(OpenPlanCode, 6, '0'))
HAVING
    COUNT(CONCAT(LPAD(GolfCode, 4, '0'),LPAD(PlanCode, 6, '0'),LPAD(OpenPlanCode, 6, '0'))) > 30
```
