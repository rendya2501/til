# FOR XML

---

## FOR XML概要

`FOR XML`は指定したフィールドの要素をXMLタグで囲う命令句。  

``` SQL
SELECT
    TestID,
    (
        SELECT PlayerNo + ','
        FROM TestTable1 AS t1
        WHERE t1.TestID = t2.TestID 
        FOR xml path('')
    )
FROM TestTable2 t2
GROUP BY TestID

-- TestID                      PlayerNo
-- ABC20200725001000009001434  ABC2020072500655,ABC2020072500655,ABC2020072500655,ABC2020072500655,
-- ABC20200725001000011000942  ABC202007250384,ABC202007250384,ABC202007250384,ABC202007250384,
-- ABC20200725001000012000176  ABC2020072500069,ABC2020072500069,ABC2020072500069,ABC2020072500069,ABC2020072500069,
-- ABC20200725001000018001431  ABC2020072500654,ABC2020072500654,ABC2020072500654,ABC2020072500654,ABC2020072500654,
```

仮に`SELECT PlayerNo + ',' FROM SlipTable FOR xml path('a')`と書いた場合、  
`<a>ABC202007250541,</a><a>ABC202007250541,</a><a>ABC202007250541,</a>`となる。  

PATH('')とするのは、空白のタグで前後を囲う動作というわけだ。  

[[SQL Server] 縦に並んだデータを横にカンマ区切りの列データで取得する方法](https://webbibouroku.com/Blog/Article/forxmlpath)  
[SQLのGroupで、文字列を集計](https://qiita.com/nuller/items/01813da7f7d60b65c220)  

---

## TYPE .valueとは何か？

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
→  
まぁ、やっておいて損はないということだな。  
省略せず書くべし!!  
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
