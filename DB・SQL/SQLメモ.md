# SQLメモ

---

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

## union について

``` SQL
SELECT 1 AS NUM
UNION
SELECT 2 AS NUM;
```

union系は出力テーブルの構造が同じでないといけない。  
union は重複チェックする。  
union all は重複チェックしない。  
なので、速度的にはunion all のほうが早い。  

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

## LIKE句

基本情報技術者過去問題 平成31年春期 午後問3より。  

LIKE句は、指定したパターンと文字列比較を行うための演算子で、次の特殊記号を用いて文字列のパターンを指定します。  
・`_` … 任意の1文字  
・`%` … 0文字以上の任意の文字列  

`_`が任意の1文字だとは知らなかった。  
つまり、普段よく使っている`LIKE '%○○%'`は、どこでもいいから○○があるかどうかを調べているってわけか。  

エ : `= '201%'`  
を選択したけど、ワイルドカードを使用した文字列は、LIKE句と同時に使用しなければ効果を生じません。年度が"201%"の行は存在しませんので結果は0行になります。  
との事。  

---

## SQLでTrue,Falseだけの判定を返す方法

IsExist系の検索する時は、True,Falseだけわかればいいので、それをやる方法をまとめる。  
COUNTが2以上の場合でもTrue扱いになる。
CONVERT,BITとは。
やってることは単純だが、補足が必要。
後TOP1って必要あるのか？

``` SQL
-- 1件の有無で判定は十分なのでTOP1する。
SELECT TOP 1 CONVERT(BIT,COUNT([TRe_ReservationFrame].[ReservationFrameNo]))
FROM [TRe_ReservationFrame]
WHERE [TRe_ReservationFrame].[OfficeCD] = @officeCD
```
