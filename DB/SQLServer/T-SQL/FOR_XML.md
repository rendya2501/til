# FOR XML

---

## 概要

`FOR XML`は指定したフィールドの要素をXMLタグで囲う命令句。  
たいていはGROUP BY した時に文字列を連結する時に使われる。  

下記例のように`FOR XML PATH('a')`と書いた場合、`<a></a>`タグによって囲ったXMLを生成する。  

``` txt
NO | TEAM | MEMBER
---+------+---------
1  | 紅組 | 佐藤
2  | 紅組 | 鈴木
3  | 紅組 | 高橋
4  | 青組 | 田中
5  | 黄組 | 伊藤
6  | 黄組 | 渡辺
```

``` sql
WITH Temp AS (
    SELECT 1 AS NO , '紅組' AS TEAM ,'佐藤' AS MEMBER
    UNION 
    SELECT 2,'紅組','鈴木'
    UNION 
    SELECT 3,'紅組','高橋'
    UNION 
    SELECT 4,'青組','田中'
    UNION 
    SELECT 5,'黄組','伊藤'
    UNION 
    SELECT 6,'黄組','渡辺'
)
SELECT * FROM Temp FOR XML PATH('a')
```

``` xml : 結果
<a>
  <NO>1</NO>
  <TEAM>紅組</TEAM>
  <MEMBER>佐藤</MEMBER>
</a>
<a>
  <NO>2</NO>
  <TEAM>紅組</TEAM>
  <MEMBER>鈴木</MEMBER>
</a>
...
```

`SELECT * FROM Temp FOR XML PATH('')`とした場合、空白のタグで前後を囲うことになり、XMLの形式は崩壊する。  
下記結果のように、`<a>`タグが消失する。  

``` XML
<NO>1</NO>
<TEAM>紅組</TEAM>
<MEMBER>佐藤</MEMBER>
<NO>2</NO>
<TEAM>紅組</TEAM>
<MEMBER>鈴木</MEMBER>
・・・
```

さらに、列名を失くすことで、要素のタグを消失させることができる。  
列名がないので空白のタグで前後を囲うことになるからだと思われる。  

``` sql
SELECT
    NO + '',
    TEAM + '',
    MEMBER + ''
FROM
    Table1
FOR XML PATH('')

-- 1紅組佐藤2紅組鈴木3紅組高橋4青組田中5黄組伊藤6黄組渡辺
```

この時点で文字列の結合はできているが、このままでは出力形式がXML形式であるのと、XML形式故、意図しない文字列が出力される可能性があるので、TYPEディレクティブの話になっていく。  

[[SQL Server] 縦に並んだデータを横にカンマ区切りの列データで取得する方法](https://webbibouroku.com/Blog/Article/forxmlpath)  
[SQLのGroupで、文字列を集計](https://qiita.com/nuller/items/01813da7f7d60b65c220)  
[SQL Serverで取得結果行を1列に連結するSQL(FOR XML PATH)](https://fumokmm.github.io/it/sqlserver/for_xml_path)  

---

## TYPE ディレクティブ

FOR XMLで出力した結果はXML型であり、完全な文字列型ではない。  
SSMSで出力結果を確認すれば分かるが、XMLエディターで開けるようになっている。  
また、この後に紹介するが、XMLタグの解釈の違いによる結果への影響もある。  

XML型の出力結果の型を明示的に指定する命令がTYPEディレクティブとなる。  
無くても結果に影響しないことはあるが、後述の例もあるので、出来れば書いておいたほうがよい。  

TYPEディレクティブは.value()メソッドとペアで使う。  
また形式上、FOR XML部分は必ずサブクエリとしなければならない。  

`SELECT ~~ FOR XML PATH('') , TYPE`までをサブクエリとして、それに対して`().value()`メソッドの形とする。  

``` sql
SELECT
    (SELECT
         NO + '',
         TEAM + '',
         MEMBER + ''
     FROM
         Table1
     FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)')

-- (列名なし)
-- 1紅組佐藤2紅組鈴木3紅組高橋4青組田中5黄組伊藤6黄組渡辺
```

このように処理することで、XMLエディターで開かれないようになる。  
つまり、「XMLとして解釈されなくなった」と思われる。  

---

## TYPEディレクティブ有無の差が明確に出るパターン

TYPEディレクティブの有無によって最も影響が出るパターンとして空白同士の結合を行った場合があげられる。  
以下、当時まとめた内容。  

空白同士のFOR XMLで`&#x20;`こんな表示がされてしまうので調べた。  
`&#x20; sql`で検索。  
どういう理屈かまだ分からないが、例の,type.valueの構文を含めたら解決した。  
→  
ようやく意味が分かった。  
FOR XMLは結果をxmlのマークアップに使われる特殊文字として出力する模様。  
なので`<>`の文字は`&lt;&gt;`みたいに特殊文字として解釈して出力する模様。  
それを通常の文字列型として出力するためのメソッドがvalueメソッドで、通常文字への変換オプションがnvarchar指定というわけだ。  
なので、FOR XMLを使うならValueメソッドをセットで使うのが安全っぽい。  

■**,type).value('.', 'NVARCHAR(MAX)')なし**

``` sql
WITH [val] AS (
    SELECT '202202080022' AS [ID],'2022-11-05 00:00:00.000' AS [BusinessDate],'' AS [Comment]
    UNION
    SELECT '202202080022' AS [ID],'2022-11-05 00:00:00.000' AS [BusinessDate],'' AS [Comment]
)
SELECT 
    [ID],
    (
        SELECT ' ' + [Comment] 
        FROM [val] AS [T1]
        WHERE [T1].[ID] = [T2].[ID] AND [T1].[BusinessDate] = [T2].[BusinessDate]
        ORDER BY [T1].[BusinessDate]
        FOR XML PATH('')
    ) AS [Comment]
FROM [val] AS [T2]
GROUP BY [ID],[BusinessDate]

-- ID            Comment
-- 202202080022  &#x20;
```

■**,type).value('.', 'NVARCHAR(MAX)')あり**

``` sql
WITH [val] AS (
    SELECT '202202080022' AS [ID],'2022-11-05 00:00:00.000' AS [BusinessDate],'' AS [Comment]
    UNION
    SELECT '202202080022' AS [ID],'2022-11-05 00:00:00.000' AS [BusinessDate],'' AS [Comment]
)
SELECT 
    [ID],
    (
        (SELECT ' ' + [Comment] 
        FROM [val] AS [T1]
        WHERE [T1].[ID] = [T2].[ID] AND [T1].[BusinessDate] = [T2].[BusinessDate]
        ORDER BY [T1].[BusinessDate]
        FOR XML PATH(''),type).value('.', 'NVARCHAR(MAX)')
    ) AS [Comment]
FROM [val] AS [T2]
GROUP BY [ID],[BusinessDate]

-- ID            Comment
-- 202202080022  
```

[FOR XML PATH('') で結合した文字列中に改行を含む場合の処理について](https://social.technet.microsoft.com/Forums/lync/ja-JP/2995ac01-434f-498a-ba88-b75cc9e5dc02/for-xml-path?forum=sqlserverja)  
[FOR XML PATH(''): Escaping "special" characters](https://stackoverflow.com/questions/1051362/for-xml-path-escaping-special-characters)  
[Group Byでの文字列連結[SQL Server]](https://foolexp.wordpress.com/2013/03/04/p2wpsu-7j/)  

---

## TYPE .valueとは何か？

これはTYPEディレクティブの意味がさっぱり分からなかったときにまとめた内容だが、いい思い出なので備忘録として残しておく。  
以下、当時まとめた内容。  

インテリセンスが働かないが、`,TYPE).value(,)`なるオプション？が使える模様。  

型を変換するための命令っぽい。  
別にそこまで厳密に型指定しなくても動く。  
しかし、型の変換が必要な場合もあるのだろう。  

``` SQL
-- このSQLを実行しても上でやったSQLの結果と違うことはない。
-- ABC202007250541,ABC202007250541,ABC202007250541
SELECT STUFF(
    (SELECT ',' + TestID FROM TestTable FOR xml path(''),TYPE).value('.', 'NVARCHAR(MAX)'),
    1, 1, ''
)
```

>私は人々が時々`, TYPE).value('.', 'NVARCHAR(MAX)')`のテクニックを使うのを省略しているのを見ます。  
これに伴う問題は、一部の文字をXMLエンティティ参照（引用符など）でエスケープする必要があるため、その場合、結果の文字列が期待どおりにならないことです。  
[Please explain what does "for xml path(''), TYPE) .value('.', 'NVARCHAR(MAX)')" do in this code](https://dba.stackexchange.com/questions/207371/please-explain-what-does-for-xml-path-type-value-nvarcharmax)  

<!--  -->
>FOR XML句で出力した値はXML形式で出力されるため、他のフィールドとは扱いが異なりますので、value()メソッドを使い、通常のSQL型に変換します。  
サブクエリを表す()の後に「.value(‘.’, ‘VARCHAR(MAX)’)」と記述しております。  
value()メソッドの第一引数はXQuery式で、第二引数はSQL型となります。  
第一引数の「'.'」はXQuery式「self::node()」の省略形、  
第二引数では、ユーザー名を扱い、出力文字数の制限をしたくないので、VARCHAR(MAX)としており、  
出力されたXML形式のデータからvalue値を取得するため、  
属性が指定されていようが関係なく、「・アイチャン・ワークン」といったvalue値を取得し、  
第二引数で指定されたVARCHARとして、SQL型の結果を返します。  
[SQLServerで複数レコードの文字列を結合](http://icoctech.icoc.co.jp/blog/?p=998)  

[value() メソッド (xml データ型)](https://docs.microsoft.com/ja-jp/sql/t-sql/xml/value-method-xml-data-type?redirectedfrom=MSDN&view=sql-server-ver15)  

---

## for xml order by

FOR XMLで文字列を結合する時に、ORDER BYで順番を指定したい。  
ORDER BYによる結合の順番は指定可能。  

ORDER BYはFOR XMLと同じ階層で記述しなければならない。  
GROUP BYを用いる場合、ORDER BYに使うキーはGROUP BYに含まれていなければエラーとなってしまう。  
必要に応じてDISTINCTを使うべし。  

``` sql : エラーになる例
WITH TmpTable AS (
    SELECT 1 AS ID, 1 AS RowNum, 'and seventy nine' AS [Data]
    UNION
    SELECT 1, 2, 'five hundred'
    UNION
    SELECT 1, 3, 'two thousand'
)
select
    ID,
    [Data] = (
        Select
            ' ' + [Data]
        from
            TmpTable t2
        where
            t2.ID = t1.ID
        order by
            t1.RowNum for xml path('')
    )
from
    TmpTable t1
group by
    t1.ID

-- メッセージ 8120、レベル 16、状態 1、行 41
-- 列 'TmpTable.RowNum' は選択リスト内では無効です。この列は集計関数または GROUP BY 句に含まれていません。
```

``` sql : 正しい例
WITH TmpTable AS (
    SELECT 1 AS ID, 1 AS RowNum, 'and seventy nine' AS [Data]
    UNION
    SELECT 1, 2, 'five hundred'
    UNION
    SELECT 1, 3, 'two thousand'
)
SELECT
    DISTINCT
    ID,
    [Data] = (
        SELECT
            ' ' + Data
        FROM
            TmpTable t2
        WHERE
            t2.ID = t1.ID
        ORDER BY
            t2.RowNum DESC FOR XML PATH('')
    )
FROM
    TmpTable t1

-- ID | Data
------+--------------------------------------------
-- 1  | two thousand five hundred and seventy nine
```

[Unable to use order by clause with for xml path properly(Sql server)](https://stackoverflow.com/questions/4387303/unable-to-use-order-by-clause-with-for-xml-path-properlysql-server)
