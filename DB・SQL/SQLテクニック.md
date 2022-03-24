# SQLテクニック

## 横縦変換 : CROSS APPLY

[複数列のデータを縦に並べる方法【SQLServer】](https://qiita.com/sugarboooy/items/0750d0ccb83a2af4dc0e)
`たて→よこ`ではない。`よこ→たて`である。  
似たような物にUNPIVOTがあるが、あちらは複数行に対応できないので、こちらを推奨された。  
たぶんSQL Server独自の関数だと思われるが、とても便利な物があるものだ。  
使い方はサンプルを見れば大体わかる。  

2022/03/10 追記  
横持ちから縦持ちへの変換は、union all を使うことでも実現できるらしい。  

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

## 縦横変換

PIVOT関数ってSQL SERVERだけなのね。  
メジャーなのはMAX CASE WHEN使うパターンみたい。  

GROUP BYとMAXをつけないで実行すると斜めにずらーっと並ぶ形になる。  
それをGROUP BYで絞って、MAXで1つだけ取ることで縦横変換を実現するというロジック。  

``` sql : 基本的な構文 <https://crmprogrammer38.hatenablog.com/entry/2017/08/01/154831>
Select
   グループ
  ,max( case when 連番=1 then 担当 end )
  ,max( case when 連番=2 then 担当 end )
  ,max( case when 連番=3 then 担当 end )
  ,max( case when 連番=4 then 担当 end )
  ,max( case when 連番=5 then 担当 end )
From 1のデータ
Group By グループ
```

``` sql : 縦横変換しつつそれをUPDATEするサンプル
UPDATE [TB_会員]
SET
    [日付0] = [SQ].[日付0],
    [日付1] = [SQ].[日付1],
    [日付2] = [SQ].[日付2],
    [日付3] = [SQ].[日付3],
    [日付4] = [SQ].[日付4],
    [日付5] = [SQ].[日付5],
    [日付6] = [SQ].[日付6],
    [日付7] = [SQ].[日付7],
    [日付8] = [SQ].[日付8],
    [日付9] = [SQ].[日付9],
    [数値0] = [SQ].[数値0],
    [数値1] = [SQ].[数値1],
    [数値2] = [SQ].[数値2],
    [数値3] = [SQ].[数値3],
    [数値4] = [SQ].[数値4],
    [数値5] = [SQ].[数値5],
    [数値6] = [SQ].[数値6],
    [数値7] = [SQ].[数値7],
    [数値8] = [SQ].[数値8],
    [数値9] = [SQ].[数値9],
    [名称0] = [SQ].[名称0],
    [名称1] = [SQ].[名称1],
    [名称2] = [SQ].[名称2],
    [名称3] = [SQ].[名称3],
    [名称4] = [SQ].[名称4],
    [名称5] = [SQ].[名称5],
    [名称6] = [SQ].[名称6],
    [名称7] = [SQ].[名称7],
    [名称8] = [SQ].[名称8],
    [名称9] = [SQ].[名称9]
FROM(
    -- ここが縦横変換部分
    SELECT
        [TB_会員].[顧客CD],
        MAX(CASE WHEN [AA].[ItemCD] = '101' THEN [AA].[Date] ELSE NULL END) AS [日付0],
        MAX(CASE WHEN [AA].[ItemCD] = '102' THEN [AA].[Date] ELSE NULL END) AS [日付1],
        MAX(CASE WHEN [AA].[ItemCD] = '103' THEN [AA].[Date] ELSE NULL END) AS [日付2],
        MAX(CASE WHEN [AA].[ItemCD] = '104' THEN [AA].[Date] ELSE NULL END) AS [日付3],
        MAX(CASE WHEN [AA].[ItemCD] = '105' THEN [AA].[Date] ELSE NULL END) AS [日付4],
        MAX(CASE WHEN [AA].[ItemCD] = '106' THEN [AA].[Date] ELSE NULL END) AS [日付5],
        MAX(CASE WHEN [AA].[ItemCD] = '107' THEN [AA].[Date] ELSE NULL END) AS [日付6],
        MAX(CASE WHEN [AA].[ItemCD] = '108' THEN [AA].[Date] ELSE NULL END) AS [日付7],
        MAX(CASE WHEN [AA].[ItemCD] = '109' THEN [AA].[Date] ELSE NULL END) AS [日付8],
        MAX(CASE WHEN [AA].[ItemCD] = '110' THEN [AA].[Date] ELSE NULL END) AS [日付9],
        MAX(CASE WHEN [AA].[ItemCD] = '111' THEN [AA].[Number] ELSE NULL END) AS [数値0],
        MAX(CASE WHEN [AA].[ItemCD] = '112' THEN [AA].[Number] ELSE NULL END) AS [数値1],
        MAX(CASE WHEN [AA].[ItemCD] = '113' THEN [AA].[Number] ELSE NULL END) AS [数値2],
        MAX(CASE WHEN [AA].[ItemCD] = '114' THEN [AA].[Number] ELSE NULL END) AS [数値3],
        MAX(CASE WHEN [AA].[ItemCD] = '115' THEN [AA].[Number] ELSE NULL END) AS [数値4],
        MAX(CASE WHEN [AA].[ItemCD] = '116' THEN [AA].[Number] ELSE NULL END) AS [数値5],
        MAX(CASE WHEN [AA].[ItemCD] = '117' THEN [AA].[Number] ELSE NULL END) AS [数値6],
        MAX(CASE WHEN [AA].[ItemCD] = '118' THEN [AA].[Number] ELSE NULL END) AS [数値7],
        MAX(CASE WHEN [AA].[ItemCD] = '119' THEN [AA].[Number] ELSE NULL END) AS [数値8],
        MAX(CASE WHEN [AA].[ItemCD] = '120' THEN [AA].[Number] ELSE NULL END) AS [数値9],
        MAX(CASE WHEN [AA].[ItemCD] = '121' THEN [AA].[Text] ELSE '' END) AS [名称0],
        MAX(CASE WHEN [AA].[ItemCD] = '122' THEN [AA].[Text] ELSE '' END) AS [名称1],
        MAX(CASE WHEN [AA].[ItemCD] = '123' THEN [AA].[Text] ELSE '' END) AS [名称2],
        MAX(CASE WHEN [AA].[ItemCD] = '124' THEN [AA].[Text] ELSE '' END) AS [名称3],
        MAX(CASE WHEN [AA].[ItemCD] = '125' THEN [AA].[Text] ELSE '' END) AS [名称4],
        MAX(CASE WHEN [AA].[ItemCD] = '126' THEN [AA].[Text] ELSE '' END) AS [名称5],
        MAX(CASE WHEN [AA].[ItemCD] = '127' THEN [AA].[Text] ELSE '' END) AS [名称6],
        MAX(CASE WHEN [AA].[ItemCD] = '128' THEN [AA].[Text] ELSE '' END) AS [名称7],
        MAX(CASE WHEN [AA].[ItemCD] = '129' THEN [AA].[Text] ELSE '' END) AS [名称8],
        MAX(CASE WHEN [AA].[ItemCD] = '130' THEN [AA].[Text] ELSE '' END) AS [名称9]
    FROM [TB_会員]
    INNER JOIN [Round3DatBRK_20220308].[dbo].[TMc_CustomerGenericInfoContent] AS [AA]
    ON [TB_会員].[顧客CD] = REPLACE([AA].[CustomerCD],[AA].[OfficeCD],'')
    AND [AA].[UpdateProgram] LIKE 'RN3.WPF%'
    GROUP BY [TB_会員].[顧客CD]
) AS [SQ]
WHERE
    [TB_会員].顧客CD = [SQ].[顧客CD]
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

``` SQL : WHILEの書き方
WHILE *ループ継続条件*
BEGIN
    *繰り返し実行したいコード*
END
-- ループ内でBREAKEが実行されるとループを抜けます。
-- ループ内でCONTINUEが実行されるとループの先頭に戻ります。
```

``` SQL : WHILE 10回ループ
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

``` SQL : 10回ループ(BREAKでループを抜ける)(まぁつかわんやろ)
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

## FOR XML PATHで文字をつなげつつ、重複は削除する例

[SQL Serverで取得結果行を1列に連結するSQL(FOR XML PATH)](https://fumokmm.github.io/it/sqlserver/for_xml_path)  

このサイトに使い方全部乗ってる。  

``` sql
SELECT
    a.TEAM AS TEAM,
    STUFF(
        (SELECT ',' + MEMBER FROM Table1 b WHERE b.TEAM = a.TEAM ORDER BY b.NO FOR XML PATH(''))
        , 1, 1, ''
    ) AS MEMBER
FROM
    (SELECT DISTINCT TEAM FROM Table1) a
ORDER BY
    a.TEAM
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

## グループ毎に1行だけ取り出したけど、それぞれ何行がひとまとまりになったのか表示する

[ndid単位で抜き出したけど、それぞれ何行がひとまとまりになったのだろうか？](https://ameblo.jp/lovetanpopo/entry-10280370777.html)  
`COUNT(フィールド名) OVER ( PARTITION BY フィールド名 )`  

RANK,ROW_NUMBER,COUNTにOVERを使えるあたり、OVERはOVERでそういう構文になっているのだろうか。  

組数を表示するときに使った。  
予約枠でサマった時にGROUPBYするほどではないけど、2つあったら「+」を表示したい。  
その時に活躍した。  

``` sql
select *
from
(
 select
  id,name,ndid,stid,
  row_number()
    over( partition by ndid order by stid ) row_num,
  count(ndid) over ( partition by ndid ) cn
 from test
)
where row_num = 1
```

``` sql : 成果物その1
SELECT
    [SQ].[ReservationFrameNo],
    [SQ].[Numerator] + '/' + [SQ].[Denominator] + CASE
        WHEN [SQ].[Count] > 1 THEN '+'
        ELSE ''
    END AS [FrameCount]
FROM
    (
        SELECT
            [SQ_Numerator].[ReservationFrameNo],
            [SQ_Numerator].[ReservationNo],
            [SQ_SeatNo].[SeatNo],
            COUNT(*) OVER(PARTITION BY [SQ_Numerator].[ReservationFrameNo]) AS [Count],
            CONVERT(nvarchar, [SQ_Numerator].[Numerator]) AS [Numerator],
            CONVERT(nvarchar, [SQ_Denominator].[Denominator]) AS [Denominator]
        FROM
            (
                SELECT
                    [ReservationFrameNo],
                    [ReservationNo],
                    ROW_NUMBER() OVER(PARTITION BY [ReservationNo] ORDER BY [ReservationFrameNo]) AS [Numerator]
                FROM
                    [TRe_ReservationPlayer]
                WHERE
                    ISNULL([ReservationCancelFlag], 0) = 0
                GROUP BY
                    [ReservationFrameNo],
                    [ReservationNo]
            ) AS [SQ_Numerator]
            INNER JOIN
                (
                    SELECT
                        [ReservationNo],
                        COUNT(DISTINCT [ReservationFrameNo]) AS [Denominator]
                    FROM
                        [TRe_ReservationPlayer]
                    WHERE
                        ISNULL([ReservationCancelFlag], 0) = 0
                    GROUP BY
                        [ReservationNo]
                ) AS [SQ_Denominator]
            ON  [SQ_Numerator].[ReservationNo] = [SQ_Denominator].[ReservationNo]
            INNER JOIN
                (
                    SELECT
                        [ReservationFrameNo],
                        [ReservationNo],
                        MIN([SeatNo]) AS [SeatNo]
                    FROM
                        [TRe_ReservationPlayer]
                    WHERE
                        ISNULL([ReservationCancelFlag], 0) = 0
                    GROUP BY
                        [ReservationFrameNo],
                        [ReservationNo]
                ) AS [SQ_SeatNo]
            ON  [SQ_Numerator].[ReservationNo] = [SQ_SeatNo].[ReservationNo]
            AND [SQ_Numerator].[ReservationFrameNo] = [SQ_SeatNo].[ReservationFrameNo]
    ) AS [SQ]
WHERE
    [SQ].[SeatNo] = 1
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

---

## 空白を除去する→複数のREPLACEを実行する

[REPLACE関数・CASE式 複数の文字列を置換する方法](https://buralog.jp/sql-using-replace-function-update-column/)  
REPLACEの中にREPLACEを記述する。  
それで全角の空白と半角の空白を空文字にすることができる。  
単純だけど、すぐに思いつかなかったので、まとめる。  

``` SQL
CASE WHEN REPLACE(REPLACE([A].[銀行CD], ' ',''), '　','') = '' 
    THEN NULL 
    ELSE [A].[銀行CD] 
END AS [BankCD],
```

---

## FLOOR関数とCEILING関数

萬君から、「これ何したいかわかります？」って質問されたが、そもそも関数が何やるかわからなかったのでまとめる。  

``` SQL
    CEILING(FLOOR(金額 * 10) / 10)
```

FLOOR : 床→切り下げ  
CEILING : 天井→切り上げ  
なるほど。言葉の意味そのままなのね。  
因みにROUNDは四捨五入ね。

例えば金額が「112.23」と来た場合  
112.23  デフォ  
1122.3  *10  
1122    FLOOR  
112.2   /10  
113     CEILING  
普通に112.23の地点でCEILING使えばよくね？ってなるよね。  
修正の履歴を見る限り、過去の修正を生かしつつ、辻褄を合わせるためにこうなったのだろうという結論で終結した。  

---

## 値が入っているかNullか、をBoolに変換する

[What is the best way to convert an int or null to boolean value in an SQL query?](https://stackoverflow.com/questions/177956/what-is-the-best-way-to-convert-an-int-or-null-to-boolean-value-in-an-sql-query)  

``` SQL
SELECT 
    CASE WHEN ValueColumn IS NULL THEN 'FALSE' ELSE 'TRUE' END BooleanOutput 
FROM 
    table 
```

---

## LIKE結合とEXCEPT

全データがあるテーブルから存在しないレコードだけを抜き出したいという案件を受けた。  
それだけなら差集合でいいけど、微妙に文字列が追加されていて、単純な結合ができない。  
SQL SERVERで正規表現が使えればよかったが、SQL SERVERでは使えない模様。  
LIKE句を使った結合はできたが、その先が分からなかった。  
そんな時にドンピシャな記事があったのでまとめる。  

[テーブル結合　LIKE演算子](https://teratail.com/questions/65949)
[SQL EXCEPTのサンプル(差分を抽出する)](https://itsakura.com/sql-except)  

>下記の状況で最適なSQLを作成したいのです。  
>
>【情報】  
>テーブル1 カラム名：項目X  
>テーブル2 カラム名：項目Y(※レコード内容は項目Xに文字列を付与したものが入っています)  
>
>手順1.ある条件を元にして、テーブル1の項目XのSELECTします。(結果は複数行返ってきます)  
>手順2.手順1のSELECT結果をWHERE条件にLIKE演算子の前方一致でテーブル2の項目YをSELECTします。  
>
>最終的には、手順2で前方一致でぶつからなかった項目Xの値を知りたいと思っています。  
>これをSQLにて効率的に実行するには、どのようなやり方が望ましいのでしょうか。  
>テーブル1とテーブル2を結合する際の条件にLIKE演算子を用いるとかですかね、、、？  

``` sql
-- 全データ
SELECT `項目X` FROM `テーブル1`
EXCEPT
-- 抽出したデータ
SELECT
    [Table2].`項目X`
FROM
    `テーブル2` AS [Table1]
INNER JOIN
    (SELECT `項目X` FROM `テーブル1` WHERE "ある条件") AS [Table2]
ON
    [Table1].`項目Y` LIKE [Table2].`項目X` + '%'
```

``` sql : こっちでもいけた
SELECT
    テーブル1.X 
FROM
    テーブル1
    LEFT JOIN テーブル2 
    ON テーブル2.Y 
    LIKE テーブル1.X + '%' （AND / OR テーブル2でさらに絞りたい条件式）
WHERE
    テーブル2.Y IS NULL AND
```

---

## ストアドのIF EXISTS

[テーブルなどのデータベースオブジェクトの存在確認](https://johobase.com/exists-database-object-sqlserver/#IF_EXISTS_ELSE)  
[Using cursor to update if exists and insert if not](https://dba.stackexchange.com/questions/218994/using-cursor-to-update-if-exists-and-insert-if-not)  

ストアドの `IF EXISTS ELSE` には閉じる構文がない。  
ELSE以降のNEXT FETCHがELSEでしか実行されないことを心配したが大丈夫であることを確認できた。  
とりあえずBEGIN ENDで処理を囲めばよいらしい。  

``` sql : 実際にうまくいったストアド
    -- カーソル定義
    DECLARE myCursor CURSOR FOR 
    SELECT ~~ FROM ~~ WHERE ~~
    -- カーソルオープン
    OPEN myCursor
    -- カーソルから変数に値をいれて、次のカーソルを参照
    FETCH NEXT FROM myCursor INTO @~~

    BEGIN
        -- 存在したらUPDATE
        IF EXISTS(SELECT ~~ FROM ~~ WHERE ~~)
            -- IFの処理
            BEGIN
                UPDATE ~~
                SET ~~
            END
        -- 存在しなければINSERT
        ELSE
            -- ELSEの処理
            BEGIN
                INSERT INTO ~~
            END

        -- if文外で実行したい場合はここにBEGIN ENDで囲って書くと実行される。
        BEGIN
            INSERT INTO ~~
        END
    END
    -- カーソルを次に進める。
    FETCH NEXT FROM myCursor4 INTO @~~
```

---

## WITH句

[SQL WITH句で同じSQLを１つのSQLに共通化する](https://zukucode.com/2017/09/sql-with.html)  

ビューみたいな定義ができる構文。  
JOIN句の中では使えないことが判明した。  
なので、単品で使う分でしか無理っぽい。  

``` sql : 使い方例
WITH employee_with AS (
  SELECT *
  FROM
    employee T1
  WHERE
    T1.last_name = '山田'
)
SELECT
  T1.id,
  T1.first_name,
  T1.last_name,
  T1.department_id,
  (
    SELECT
      AVG(SUB1.height)
    FROM
      -- WITH句で指定したテーブルを参照
      employee_with SUB1
    WHERE
      T1.department_id = SUB1.department_id
  ) AS avg_height,
  (
    SELECT
      MAX(SUB1.height)
    FROM
      -- WITH句で指定したテーブルを参照
      employee_with SUB1
    WHERE
      T1.department_id = SUB1.department_id
  ) AS max_height
FROM
  -- WITH句で指定したテーブルを参照
  employee_with T1
```

``` sql : 複数のテーブルを指定する
WITH sample_with AS (
  SELECT *
  FROM sample
  WHERE COL1 = 'sample'
),
sample2_with AS (
  SELECT *
  FROM sample2
    -- WITH句で定義したテーブルも参照可能
    JOIN sample_with
    ON sample2.COL1 = sample_with.COL1
  WHERE sample2.COL1 = 'sample'
)
SELECT
```

``` sql
-- 後でどこかにまとめるけど、いい感じのWITH句と相関副問い合わせのサンプルが出来たのでまとめておく。
WITH employee_with AS (
  SELECT 1 As id ,"りんご" as name,"フルーツ" as category ,10 as kosuu
  UNION
  SELECT 2,"みかん","フルーツ",20
  UNION
  SELECT 3,"にんじん","野菜",30
  UNION
  SELECT 4,"大根","野菜",40
)
select id,name 
from employee_with a
where kosuu =
 (select 
  max(kosuu)
  from employee_with b 
  where a.category = b.category);

```

---

## 分子分母を出力するサンプル

``` sql
SELECT
    ReservationFrameNo,
    [SQ].[Numerator] + '/' + [SQ].[Denominator] + CASE WHEN COUNT(*) > 1 THEN '+' ELSE '' END  AS [FrameCount]
FROM (
    SELECT
        [A].ReservationFrameNo,
        [A].ReservationNo,
        CONVERT(nvarchar,[SQ_Numerator].[Numerator]) AS [Numerator],
        CONVERT(nvarchar,[SQ_Denominator].[Denominator]) AS [Denominator]
    FROM (
   -- 予約枠を上から見ていった時のシーケンス(分子:Numerator)
        SELECT
            [ReservationFrameNo],
            [ReservationNo],
            ROW_NUMBER() OVER(PARTITION BY [ReservationNo] ORDER BY ReservationFrameNo) AS Numerator
        FROM 
            [TRe_ReservationPlayer]
        WHERE
            ISNULL(ReservationCancelFlag,0) = 0
        GROUP BY
            ReservationFrameNo,ReservationNo
        ) AS [A]
    JOIN (
   -- 予約が持っている予約枠数(分母:Denominator)
        SELECT
            [ReservationNo],
            COUNT(DISTINCT [ReservationFrameNo]) AS Denominator
        FROM 
            [TRe_ReservationPlayer]
        WHERE 
            ISNULL(ReservationCancelFlag,0) = 0
        GROUP BY 
            [ReservationNo]
    ) AS [B]
    ON [A].ReservationNo = [B].ReservationNo
) AS [SQ]
GROUP BY
    [ReservationFrameNo]
```

``` sql : もっと簡単にできた
SELECT
    [ReservationFrameNo],
    [ReservationNo],
    ROW_NUMBER() OVER(PARTITION BY [ReservationNo] ORDER BY [ReservationFrameNo]) AS [Numerator],
    COUNT(1) OVER(PARTITION BY [ReservationNo]) AS [MinAccountNo]
FROM 
    [TRe_ReservationPlayer]
WHERE
    ISNULL([ReservationCancelFlag],0) = 0
GROUP BY
    [ReservationFrameNo],[ReservationNo]
```

[COUNT OVER(PARTITION BY)](https://zukucode.com/2017/08/sql-over-partition-by.html)  

``` sql
SELECT
  last_name,
  --全体の総件数
  COUNT(1) OVER() total_count,
  --部門ごとの件数
  COUNT(1) OVER(PARTITION BY department_id) section_count,
  --部門ごとの最大身長
  MAX(height) OVER(PARTITION BY department_id) section_max_height,
  --部門ごとの身長順（身長順に並び替えたときの行番号）
  ROW_NUMBER() OVER(PARTITION BY department_id ORDER BY height DESC) section_height_order,
  --全体の身長順（身長順に並び替えたときの行番号）
  ROW_NUMBER() OVER(ORDER BY height DESC) height_order
FROM
  employee
ORDER BY
  id
```

---

## Identityの発番テーブルで保存時に発番した値を取得する

`OUTPUT句`なるものがあるらしい。  
部長が知ってた。  

``` sql
INSERT INTO Tfr_Reception
(OfficeCD)
OUTPUT
inserted.SeqNo
values('aaa')
```

福田さんはこんなのも提示した。  
`var id = rnWebConnection.QueryFirstOrDefault<long>("SELECT LAST_INSERT_ID()");`  

---

## Time型をString型に変換する

[SQL Serverで日付型を文字列に変換する](https://it-engineer-info.com/database/2630/)

Time(7)型の`07:23:00.0000000`って表示を`07:23`にしたい。  

``` sql
    -- 108 → HH:MM:SS → 07:23:00
    CONVERT(NVARCHAR,[Frame].[StartTime],108)

    LEFT(CONVERT(NVARCHAR,[Frame].[StartTime],108), 5)
```
