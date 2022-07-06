# SQLテクニック

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

## 連番生成

[SQL で動的に連番テーブルを生成する](https://sql55.com/query/generate-sequence-number.php)  

`sys.all_objects` はシステムオブジェクトやユーザーが定義したオブジェクト等を保持している、オブジェクトカタログビューで、SQL Server のコアなビューなので、バージョンがあがっても簡単になくなったりする心配はないと思います。  
ですが、`sys.all_objects` のデフォルトのレコード数は、SQL Server のバージョンによっても違いますが、だいたい 2000 前後なので、あまり大きいを生成するのには使えませんので気をつけてください。  

### 範囲で連番生成

``` sql
--  6  12
-- 12  18
-- 18  24
-- 24  30
SELECT * FROM (
    SELECT TOP (1000)
        ROW_NUMBER() OVER (ORDER BY object_id) AS SeqNoFrom,
        ROW_NUMBER() OVER (ORDER BY object_id)+6 AS SeqNoTo
    FROM sys.all_objects
    ORDER BY SeqNoFrom
) AS rn_q
WHERE rn_q.SeqNoFrom%6 = 0
```

``` sql
--  0     4
--  5     9
-- 10    14
-- 15    19
-- ~~~~~~~~
SELECT * FROM (
    SELECT TOP (1000)
        ROW_NUMBER() OVER (ORDER BY object_id)-1 AS SeqNoFrom,
        ROW_NUMBER() OVER (ORDER BY object_id)+3 AS SeqNoTo
    FROM sys.all_objects
    ORDER BY SeqNoFrom
) AS rn_q
WHERE rn_q.SeqNoFrom%5 = 0
```

---

## 文字列分割

[SQLServerで指定した文字で文字列を分割する](https://it-engineer-info.com/database/divide-string)  

### SUBSTRING方式

``` sql
DECLARE    @separator   VARCHAR(MAX) = ','
DECLARE    @target_str  VARCHAR(MAX)='123,45678'
 
--文字列を「-」で前後に分割する
SELECT
     SUBSTRING( @target_str, 1, CHARINDEX( @separator, @target_str ) - 1 ) AS before
    ,SUBSTRING( @target_str, CHARINDEX( @separator, @target_str ) + 1, LEN( @target_str ) - CHARINDEX( @separator, @target_str )) AS after
```

### STRING_SPLIT

SQL Server 2016以降では「STRING_SPLIT」という関数が実装されています。  

[【T-SQL】文字列を分割して〇番目の値を取得したい場合 (string_split)](http://ubisnews.blogspot.com/2018/07/t-sql-stringsplit.html)  
>T-SQL(SQLSERVER)で文字列を分割したいと思ったところ、調べるとstring_split関数というものが利用できるとわかりました。  
>ただ分割した文字列は表になり分割後の文字をサクッと取り出す方法が探しても見つからなかったのでメモしておこうと思います。  
>例えば、abc,def,ghiという文字列をカンマ(,)で分割して2番目の値(ここではdef)を取り出したい場合  

``` sql
DECLARE @Val AS VARCHAR(10) --取得する値を格納する変数

SELECT @Val = (
    SELECT A.value FROM
    (
        SELECT
            ROW_NUMBER() OVER (ORDER BY (SELECT 1)) AS "Id",
            value
        FROM
            STRING_SPLIT(N'abc,def,ghi', N',')
    ) A
    WHERE A.Id = 1
)

SELECT @Val
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
ibさんと編み出したサンプル。  
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

最後の文字を削除する時に`substring([ReservationNo], 1, len(ReservationNo)-1)`ってやったらエラーになったけど何かわかる？ってYr君に質問されたのでまとめ。  
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

20桁のIDを作る要領で3つのキーを0埋めして文字列結合して、GroupByしてCOUNT取ってHAVINGで20以上のIDを表示ってやつ。  

左0埋め→LPAD  
文字列結合→CONCAT  
数値→文字列変換→CAST(数値 AS CHAR)  
数値文字列変換に関してはLPADしてCONCATしたらそのまま行けたので別に必要ないのかも。  

``` SQL
SELECT
    CONCAT(LPAD(Code1, 4, '0'),LPAD(Code2, 6, '0'),LPAD(Code3, 6, '0')),
    COUNT(CONCAT(LPAD(Code1, 4, '0'),LPAD(Code2, 6, '0'),LPAD(Code3, 6, '0')))
FROM
    [Table]
GROUP BY
    CONCAT(LPAD(Code1, 4, '0'),LPAD(Code2 , 6, '0'),LPAD(Code3, 6, '0'))
HAVING
    COUNT(CONCAT(LPAD(Code1, 4, '0'),LPAD(Code2, 6, '0'),LPAD(Code3, 6, '0'))) > 30
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

Y君から、「これ何したいかわかります？」って質問されたが、そもそも関数が何やるかわからなかったのでまとめる。  

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

fkdさんはこんなのも提示した。  
`var id = rnWebConnection.QueryFirstOrDefault<long>("SELECT LAST_INSERT_ID()");`  

``` sql
INSERT INTO Tfr_Reception
(OfficeCD)
OUTPUT
inserted.SeqNo
values('aaa')
```

---

## Time型をString型に変換する

Time(7)型の`07:23:00.0000000`を`07:23`にしたかった。  

``` sql
    -- 108 → HH:MM:SS → 07:23:00
    CONVERT(NVARCHAR,[Frame].[StartTime],108)
    -- 07:23:00 → 07:23
    LEFT(CONVERT(NVARCHAR,[Frame].[StartTime],108), 5)
```

[SQL Serverで日付型を文字列に変換する](https://it-engineer-info.com/database/2630/)  
108[HH:mm:ss]と112[YYYYMMDD]を多用すると思われる。  

``` txt
Style  日付

100    01 2 2019 3:04AM
101    01/02/2019
102    2019.01.02
103    02/01/2019
104    02.01.2019
105    02-01-2019
106    02 01 2019
107    01 02, 2019
108    03:04:05
109    01 2 2019 3:04:05:677AM
110    01-02-2019
111    2019/01/02
112    20190102
113    02 01 2019 03:04:05:677
114    03:04:05:677
120    2019-01-02 03:04:05
121    2019-01-02 03:04:05.677
126    2019-01-02T03:04:05.677
127    2019-01-02T03:04:05.677
```

---

## FORMAT関数による時間の変換の代替案

SQLServer2008R2ではFORMAT関数が使えない模様。  
変わりにCONVERT関数で代用。  

※FORMAT関数は SQL Server 2017 以降の模様。  
[SQL Serverで日付型を文字列に変換する](https://it-engineer-info.com/database/2630/)  

``` sql
--FORMAT([スタート時間], 'HH:mm:ss') as StartTime,
--→
CONVERT(VARCHAR, [スタート時間], 108),
```

---

## テーブル結合にLIKE演算子を使えるか

普通に行ける。  
[テーブル結合 LIKE演算子](https://teratail.com/questions/65949)  

``` sql
SELECT
    テーブル1.X 
FROM
    テーブル1
    LEFT JOIN テーブル2 
    ON テーブル2.Y LIKE テーブル1.X + '%' 
    (AND / OR テーブル2でさらに絞りたい条件式)
WHERE
    テーブル2.Y IS NULL 
    AND ~~
```

---

## 検索条件にマッチするデータが存在しない場合にデフォルト値を返す

レコードが存在したらそのままINSERTして、レコードがなければデフォルト値をINSERTしたい。  

言葉にすると単純なのだが、30分くらい調べたのでまとめる。  
調べても全然スマートなのが出てこなかった。  

結果としてCOALESCEで実現できた。  
その前段としてCASE WHEN使って実現しようとしたが、なんと駄目だったのでそれもまとめる。  

てか、COALESCEってNULLの場合の置き換えだと思っていたが、レコードがなくても動いてくれるのか。  
2,3調べてみたけど、どこにも0件の場合についての記事が見つからない。  
確実に言えるのはNULLを指定した値に置き換えるというものだけだ。  

sql 条件によってselectを変える
SQL 0件の場合
COALESCE　レコードなし
[検索条件にマッチするデータが存在しない場合にデフォルト値を返す](https://sebenkyo.com/2020/04/14/post-774/)
[NULLと戯れる: 集約関数とNULL](https://qiita.com/SVC34/items/dc1bc52c2d7b44a65459)
[レコードがない場合のselect結果](https://teratail.com/questions/57768)  

``` sql : CASE WHEN案  
-- 一見行けそうに見えるが、GROUP BYしないと駄目だった。

SELECT CASE
    -- レコードがあれば料金CDをそのままいれる。なければデフォルト値として9999をいれる。
    WHEN COUNT(*) != 0
    THEN 料金CD
    ELSE 9999 
END AS 料金CD
FROM RoundDat_OCC.dbo.TM_料金 
WHERE 資格区分CD = 9

-- エラー
-- 列 'RoundDat_OCC.dbo.TM_料金.料金CD' は選択リスト内では無効です。この列は集計関数または GROUP BY 句に含まれていません。
```

``` sql : COALESCE案
-- レコードがなくても動いてくれたのは以外だった。
-- 何はともあれ理想の動作を実現できた。

SELECT COALESCE(
    (SELECT FeeCD FROM TMa_Fee WHERE PrivilegeTypeCD = 9),
    9999
)
```

``` sql : COALESCE案2
-- こっちのほうがいいのかも。
-- MAXは0件ならNULLを返す。COALESCEは結果がNULLなら指定した値に置き換えるので本来の動作に準拠したものになる。

SELECT COALESCE(
    (SELECT MAX(FeeCD) FROM TMa_Fee WHERE PrivilegeTypeCD = 9),
    9999
)
```

``` sql
SELECT 
    CASE WHEN EXISTS(SELECT 料金CD FROM RoundDat_OCC.dbo.TM_料金 WHERE 資格区分CD = 0)
        THEN [A].料金CD
        ELSE 9999
    END


    SELECT IIF(COUNT(*) != 0, 料金CD,9999)
    FROM RoundDat_OCC.dbo.TM_料金 
    WHERE 資格区分CD = 0



    SELECT CASE 
        WHEN (SELECT COUNT(*) FROM RoundDat_OCC.dbo.TM_料金) != 0 
        THEN 料金CD
        ELSE 9999 
    END AS 料金CD
    FROM RoundDat_OCC.dbo.TM_料金 
    WHERE 資格区分CD = 0

    
    SELECT CASE 
        WHEN COUNT(*) = 0
        THEN 料金CD
        ELSE 9999 
    END AS 料金CD
    FROM RoundDat_OCC.dbo.TM_料金 
    WHERE 資格区分CD = 9


SELECT
    CASE WHEN COUNT(CASE WHEN 資格区分CD = 9 THEN 1 ELSE 0 END) = 0
        THEN 料金CD
        ELSE 9999
    END    
FROM RoundDat_OCC.dbo.TM_料金


select COALESCE(
    (SELECT 料金CD FROM RoundDat_OCC.dbo.TM_料金 WHERE  資格区分CD = 9),
    9999
)
```

---

## WHEREの実行タイミングで結果が変わる

WHEREをどのタイミングで実行するかによって結果が変わってくるパターンに遭遇したのでメモっておく。  
端的にいえば、最終結果に対してWHEREするか、結果を構築する最中にWHEREするかの違いだ。  

1. 全部JoinしてからWHERE→ダメ  
2. Joinする最中にWHERE→一番OK  
3. WHEREで絞った結果をサブテーブルで受け取ってJoin→OK  

``` txt
出力されてほしい結果
1 カード 166666.00
2 QR 0.00

間違った結果
1 カード 166666.00
```

QRレコードが表示されてほしいのだが、全部JOINしてからWHEREするパターンでは間違った結果になってしまう。  
愚直にWHEREした結果をサブテーブルで受け取ってからJOINすることで解決したが、それ以外にもJOIN中に条件を指定することでも解決できた。  

``` sql : 全部JoinしてからWHERE → ダメ
-- ダメなパターン

-- LEFT JOINした時点では、存在しないレコードもNULL行として表示されるが、そこからWHEREでレコードの判定を行う。
-- 結果、NULLのBusinessDateは対象外となるので、NULLを0と表示したいレコードは消えてしまう。

-- 結果
-- 1 カード 166666.00

SELECT
    [TMa_PaymentCls].[PaymentClsCD],
    [TMa_PaymentCls].[PaymentClsName],
    SUM(ISNULL([TCs_CardBalance].[TodayAccountsReceivable],0)) AS [Price]
FROM
    [TMa_PaymentCls]
    LEFT OUTER JOIN [TMa_PaymentType] 
    ON [TMa_PaymentCls].[OfficeCD] = [TMa_PaymentType].[OfficeCD]
    AND [TMa_PaymentCls].[PaymentClsCD] = [TMa_PaymentType].[PaymentClsCD]
    LEFT OUTER JOIN [TCs_CardBalance]
    ON [TMa_PaymentType].[OfficeCD] = [TCs_CardBalance].[OfficeCD]
    AND [TMa_PaymentType].[PaymentTypeCD] = [TCs_CardBalance].[PaymentTypeCD]
WHERE 
    [TCs_CardBalance].[OfficeCD] = 'ESV'
    AND [TCs_CardBalance].[BusinessDate] = '2022/04/02'
GROUP BY
    [TMa_PaymentCls].[PaymentClsCD],
    [TMa_PaymentCls].[PaymentClsName]
ORDER BY
    [TMa_PaymentCls].[PaymentClsCD]
```

``` sql : Joinする最中にWHERE → 一番OK
-- 一番スマートな解決法だと思われる

-- JOINする最中に指定日付のモノだけを抽出し、そのままLEFT JOINするので、存在しないレコードも最終結果に残る。
-- 結果、ISNULLでNULLのレコードを0として出力することが可能。

-- 結果
-- 1 カード 166666.00
-- 2 QR 0.00

SELECT
    [TMa_PaymentCls].[PaymentClsCD],
    [TMa_PaymentCls].[PaymentClsName],
    SUM(ISNULL([TCs_CardBalance].[TodayAccountsReceivable],0)) AS [Price]
FROM
    [TMa_PaymentCls]
    LEFT OUTER JOIN [TMa_PaymentType] 
    ON [TMa_PaymentCls].[OfficeCD] = [TMa_PaymentType].[OfficeCD]
    AND [TMa_PaymentCls].[PaymentClsCD] = [TMa_PaymentType].[PaymentClsCD]
    LEFT OUTER JOIN [TCs_CardBalance]
    ON [TMa_PaymentType].[OfficeCD] = [TCs_CardBalance].[OfficeCD]
    AND [TMa_PaymentType].[PaymentTypeCD] = [TCs_CardBalance].[PaymentTypeCD]
    AND [TCs_CardBalance].[OfficeCD] = 'ESV'
    AND [TCs_CardBalance].[BusinessDate] = '2022/04/02'
GROUP BY
    [TMa_PaymentCls].[PaymentClsCD],
    [TMa_PaymentCls].[PaymentClsName]
ORDER BY
    [TMa_PaymentCls].[PaymentClsCD]
```

``` sql : WHEREで絞った結果をサブテーブルで受け取ってJoin → OK
-- 一番愚直ではあるが、一番長く野暮ったい

-- サブクエリの中でNULLのレコードも出力しておき、そのまま結果に依存しない列でJOINすることで存在しないレコードを最終結果に残す。

-- 結果
-- 1 カード 166666.00
-- 2 QR 0.00

SELECT
    [TMa_PaymentCls].[PaymentClsCD],
    [TMa_PaymentCls].[PaymentClsName],
    SUM(ISNULL([TodayAccountsReceivable],0)) AS [Price]
FROM
    [TMa_PaymentCls]
    LEFT OUTER JOIN
        (
            SELECT
                [TCs_CardBalance].[OfficeCD],
                [TCs_CardBalance].[TodayAccountsReceivable],
                [TMa_PaymentType].[PaymentClsCD]
            FROM
                [TCs_CardBalance]
                LEFT JOIN [TMa_PaymentType]
                ON  [TCs_CardBalance].[OfficeCD] = [TMa_PaymentType].[OfficeCD]
                AND [TCs_CardBalance].[PaymentTypeCD] = [TMa_PaymentType].[PaymentTypeCD]
            WHERE
                [TCs_CardBalance].[OfficeCD] = 'ESV'
                AND [TCs_CardBalance].[BusinessDate] = '2022/04/02'
        ) AS [CardBalance]
    ON  [TMa_PaymentCls].[OfficeCD] = [CardBalance].[OfficeCD]
    AND [TMa_PaymentCls].[PaymentClsCD] = [CardBalance].[PaymentClsCD]
GROUP BY
    [TMa_PaymentCls].[PaymentClsCD],
    [TMa_PaymentCls].[PaymentClsName]
ORDER BY
    [TMa_PaymentCls].[PaymentClsCD]
```

---

## 1件でもあれば1、なければ0を返すクエリ

初見でびっくりしたのでまとめる。  
`TOP 1 1`ってなんやねんと思ったけど、そういうことか。  

1件でもあればよく、処理速度を挙げるためにTOP1。  
で、1を取得するって意味で`TOP 1 1`。  

``` SQL
SELECT ISNULL((SELECT TOP 1 1 FROM [Table] WHERE [Table].[Field] = 'hoge' ) , 0 )
```

---

## IF NOT EXIST

なければ実行、あれば何もしないサンプル

``` sql
IF NOT EXISTS
(
    SELECT TOP 1 1 FROM Round3DatOCC.dbo.TMa_TimeFrameType WHERE [TimeFrameTypeCD] = 99
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

## SQL てく

2022年5月21日  
名前の苗字を北海道の市区町村に置き換えたい。  
カタカナも置き換えたい。  
他のフィールドにある名前も置き換えたい。  
そんな要求を実現したのでまとめ。  

奇跡的に姓名がスペースで別れていたので、スペースでsplit。  
苗字の1文字目のUNICODEを取得して3桁を見る。  
その3桁が別テーブルに定義したFromToのどの範囲に当たるか検索。
対象FromToの市区町村名を取得。  
その市区町村名を苗字に割り当てる。  
苗字の置き換えはREPLACE関数で行った。  

``` SQL
-- 改良後
UPDATE [Base]
SET
    [MemberName] = REPLACE([Base].[MemberName],[Base].[Myouji],[CT].[Name]),
    [MemberKana] = REPLACE([Base].[MemberKana],[Base].[MyoujiKana],[CT].[Kana]),
    [SearchKeyName] = REPLACE([Base].[SearchKeyName],[Base].[Myouji],[CT].[Name]),
    [SearchKeyKana] = REPLACE([Base].[SearchKeyKana],[Base].[MyoujiKana],[CT].[Kana]),
    [SearchKeyNameIdentification] = REPLACE([Base].[SearchKeyNameIdentification],[Base].[myouji],[CT].[Name])
FROM
    (
        SELECT 
            CASE 
                WHEN REPLACE(SUBSTRING([MemberName], 1, CHARINDEX(' ', [MemberName] )),' ','') <> '' 
                THEN REPLACE(SUBSTRING([MemberName], 1, CHARINDEX(' ', [MemberName] )),' ','')
                ELSE [MemberName]
            END AS [Myouji],
            CASE 
                WHEN REPLACE(SUBSTRING([MemberKana], 1, CHARINDEX(' ', [MemberKana] )),' ','') <> '' 
                THEN REPLACE(SUBSTRING([MemberKana], 1, CHARINDEX(' ', [MemberKana] )),' ','')
                ELSE [MemberKana]
            END AS [MyoujiKana],
            *
        FROM TMc_Member
    ) AS [Base]
    JOIN [ConversionTable] AS [CT]
    ON RIGHT(UNICODE(LEFT([Base].[myouji],1)),3) BETWEEN [CT].[From] AND [CT].[To]


-- 改良前
UPDATE [Base]
SET 
    -- わざわざ左から苗字の文字数分を指定してるけど、そんなことする必要なかった。
    -- REPLACE関数は苗字と同じ部分だけを置き換えてくれるから。
    [MemberName] = REPLACE([Base].[MemberName],LEFT([Base].[MemberName],LEN([SQ].[Myouji])),[CT].[Name]),
    [MemberKana] = REPLACE([Base].[MemberKana],LEFT([Base].[MemberKana],LEN([SQ].[MyoujiKana])),[CT].[Kana]),
    SearchKeyName = REPLACE([Base].[SearchKeyName],LEFT([Base].SearchKeyName,LEN([SQ].[Myouji])),[CT].[Name]),
    SearchKeyKana = REPLACE([Base].[SearchKeyKana],LEFT([Base].[SearchKeyKana],LEN([SQ].[MyoujiKana])),[CT].[Kana]),
    SearchKeyNameIdentification = REPLACE([Base].SearchKeyNameIdentification,LEFT([Base].SearchKeyNameIdentification,LEN(myouji)),[CT].[Name])
FROM
    -- わざわざ自分自身テーブルと改めてJOINする必要はなかった。
    TMc_Member AS [Base]
    JOIN 
    (
        SELECT 
            CASE 
                WHEN REPLACE(SUBSTRING([MemberName], 1, CHARINDEX(' ', [MemberName] )),' ','') <> '' 
                THEN REPLACE(SUBSTRING([MemberName], 1, CHARINDEX(' ', [MemberName] )),' ','')
                ELSE [MemberName]
            END AS [Myouji],
            CASE 
                WHEN REPLACE(SUBSTRING([MemberKana], 1, CHARINDEX(' ', [MemberKana] )),' ','') <> '' 
                THEN REPLACE(SUBSTRING([MemberKana], 1, CHARINDEX(' ', [MemberKana] )),' ','')
                ELSE [MemberKana]
            END AS [MyoujiKana],
            *
        FROM TMc_Member
    ) AS [SQ]
    ON [Base].MemberCD = [SQ].MemberCD
    AND [Base].[HistoryNo] = [SQ].[HistoryNo]
    JOIN [ConversionTable] AS [CT]
    ON RIGHT(UNICODE(LEFT([SQ].[myouji],1)),3) BETWEEN [CT].[From] AND [CT].[To]
```

``` sql
select * from TMc_Customer
select * from TMc_CustomerAddress
select * from TMc_Member
select * from TRe_Reservation
select * from TRe_ReservationPlayer


--文字列を「-」で前後に分割する
SELECT
     CASE WHEN LEN(ISNULL(CustomerName,0)) > 1 THEN SUBSTRING(CustomerName, 1, CHARINDEX(' ', CustomerName )  ) ELSE CustomerName END AS before
    -- ,SUBSTRING( CustomerName, CHARINDEX( @separator, CustomerName ) + 1, LEN( @target_str ) - CHARINDEX( @separator, @target_str )) AS after
FROM
    TMc_Customer

    --文字列を「-」で前後に分割する
SELECT
    CHARINDEX(' ', CustomerName ),
     SUBSTRING(CustomerName, 1, CASE WHEN CHARINDEX(' ', CustomerName ) > 0 THEN CHARINDEX(' ', CustomerName ) -1 ELSE 0 END) AS before
    -- ,SUBSTRING( CustomerName, CHARINDEX( @separator, CustomerName ) + 1, LEN( @target_str ) - CHARINDEX( @separator, @target_str )) AS after
FROM
    TMc_Customer



select * from ConversionTable
where [Name] = '鶴居'


select (UNICODE('井')),(UNICODE('五'))
select RIGHT(UNICODE('井'),3),RIGHT(UNICODE('五'),3)





DECLARE @Count INT = 100;
SELECT   TOP (@Count)
         ROW_NUMBER() OVER (ORDER BY object_id)-1 AS SeqNo
FROM     sys.all_objects
ORDER BY SeqNo;



SELECT * FROM (
    SELECT   TOP (1000)
             ROW_NUMBER() OVER (ORDER BY object_id)-1 AS SeqNo,
             ROW_NUMBER() OVER (ORDER BY object_id)+3 AS SeqNoTo
    FROM     sys.all_objects
    ORDER BY SeqNo
) AS rn_q
WHERE rn_q.SeqNo%5 = 0

```

---

## 相関副問合せ(相関サブクエリ)

<https://atmarkit.itmedia.co.jp/ait/articles/1703/01/news187.html>  
副問合せの文中で、副問合せの外側の属性（検索結果）を利用して検索している問合せを、相関副問合せと呼びます。  
一般的には、パフォーマンスは悪くなる見たい。  

``` SQL
SELECT 社員番号,社員名 FROM 社員 AS S1
WHERE 生年月日 > (SELECT MIN(生年月日) FROM 社員 AS S2 WHERE S1.性別 = S2.性別)
-- サブクエリ中でメインクエリのS1テーブルを参照している。

-- EXISTSを使えば自動的に相関副問い合わせになる。
SELECT * FROM USER_MASTER A
WHERE EXISTS (SELECT * FROM AUTHORIZATION B WHERE A.USER_ID = B.USER_ID )
```

処理の流れ  
（1）外側のSELECT文を1行分だけ実行  
（2）取り出した表を副問合せに代入して実行  
（3）外側のSELECT文における、1行目のWHERE句の判定を行う  

---

## SQLの結合条件のON句の順番について

<https://detail.chiebukuro.yahoo.co.jp/qa/question_detail/q11166323581>  

SQLのLEFT JOIN とかのONの順番は関係無いらしい。  
地味に知らなかったので、メモ。  

---

## CROSS JOIN+WHERE と INNER JOIN

<https://qiita.com/zaburo/items/548b3c40fee68cd1e3b7>  
<https://stackoverflow.com/questions/17759687/cross-join-vs-inner-join-in-sql>  

CROSS JOIN して WHERE で絞る方法(等価結合)とINNER JOINの結果は同じらしい。(厳密には内部処理的には違うらしいが)  
まぁ、CROSS JOINしてWHEREで絞るくらいなら素直にINNER JOIN使えって話。  
RN2.23では結構そういうことしてて、どういう挙動をするのかわからなかったのでやってみた。  

``` SQL
-- どちらも結果は同じになる

-- CROSS JOIN + WHERE
SELECT depts.dept_name,employees.name
FROM depts,employees
WHERE depts.dept_id = employees.dept_id;

-- INNER JOIN
SELECT depts.dept_name,employees.name
FROM depts INNER JOIN employees
WHERE depts.dept_id = employees.dept_id;
```

---

## N'文字列'の意味

1. Unicodeを使うという宣言・マーキング  
2. 日本語を使う場合に必須  

[SQLのNプレフィックスっていったい何？](http://once-and-only.com/programing/sql/sql%E3%81%AEn%E3%83%97%E3%83%AC%E3%83%95%E3%82%A3%E3%83%83%E3%82%AF%E3%82%B9%E3%81%A3%E3%81%A6%E3%81%84%E3%81%A3%E3%81%9F%E3%81%84%E4%BD%95%EF%BC%9F/)

>SQL Server で Unicode 文字列定数を扱う場合には、Unicode 文字列の前に大文字の N が必ず必要です。  
>これは、SQL Server Books Online の「Unicode データの使用」で説明されています。  
>”N” プレフィックスは、SQL-92 標準の National Language を意味し、必ず大文字にする必要があります。  
>Unicode 文字列定数の前に N を付加しない場合、その文字列は、SQL Server によって、使用される前に現在のデータベースの Unicode 以外のコードページに変換されます。  

※Unicodeは何故UじゃなくてNなのか？  
→  
National Languageという意味でUnicodeが採用されているため。  
National Languageは国語という意味ではなく「様々な国の文字」というニュアンス。  

[MS SQLServer のSQLで文字列の前にN:](https://oshiete.goo.ne.jp/qa/280266.html)

>N'***' とT-SQL内で書くと、''内の文字をUnicodeで表現されたものとして処理する、という意味になります。  
>Nは、nationalの略です。  
>ですから、日本語を使おうとするとNは必須になる、という事ですね。  
>こんな感じでつかいます。＃N'Unicode 文字列'  

---

## DISTINCTとワイルドカードの併用

DISTINCTとワイルドカード `*` を併用したら.NETFrameworkでは実行速度が遅くなるらしい  

---

## TOP 句 PERCENT

[SqlServerの SELECT TOP 100 PERCENT で作成したビューの並び順は『保証されない』](https://culage.hatenablog.com/entry/20170824/p1)  
[TOP 句 PERCENT](https://haradago.hatenadiary.org/entry/20180214/p1)  

TOPにPERCENT句を付けると上位N%のデータを取得することができる  

``` sql
select TOP 50 PERCENT * FROM TPa_Reservation

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
