# WHERE

SQLserverとの差別化とか色々必要だけど、とりあえずWHEREに関することをまとめていく。

---

## SQLserverのWHEREにおける否定

[【SQL server】WHERE句で利用できる演算子](https://ameblo.jp/beginner-shelly/entry-10826554593.html)  

BETWEEN句でNOTが使えるのは知っていたが、では普通の場合に使うことができるのか知らなかったのでまとめた。  
結果的に使えた。  
色々興味深い事実も分かった。  

Q. 否定演算子は`<>`以外に使えるのか？  
→  
A. 使えた。`NOT` も `!=` も使えた  

``` sql
-- BETWEENのNOTはよく使うので覚えた
select * from [Table]
where BusinessDate NOT BETWEEN '2022-05-24' AND '2022-05-25'

-- BETWEEN使わない時のNOTの書き方は括弧で括って先頭にNOTをつける
-- もちろん括弧がないと死ぬ
select * from [Table]
where NOT (BusinessDate >= '2022-05-24' AND BusinessDate <= '2022-05-25')

-- NOTは単体で使うこともできる。
select * from [Table]
where NOT (BusinessDate = '2022-05-25')

-- NOTの否定はフィールド単体の場合は括弧ありでもなしでも大丈夫な模様
select * from [Table]
where NOT BusinessDate = '2022-05-25'

-- プログラミングにおいてメジャーな否定演算子である「!」も2016では使えた
-- どのバージョンから使えるようになったものなのかは不明だが非推奨とは言っていたので素直に「<>」を使う
select * from [Table]
where BusinessDate != '2022-05-25'
```

---

## 「条件を絞ってからJoin」 するのと 「Joinしてから条件を絞る」 のはどちらが良いのか？

[条件で絞ったテーブル同士でJOINのレスポンスについて](https://atmarkit.itmedia.co.jp/bbs/phpBB/viewtopic.php?topic=32176&forum=26)  
[SQL joinの際に条件を先に絞るか後に絞るかでスピードの違いはあるのでしょうか？](https://teratail.com/questions/250008)  

[インデックスと実行計画を理解する@SQLServer](https://qiita.com/okuzou1/items/f710bcde64beb22cd50b)  
[実行計画の表示方法](https://use-the-index-luke.com/ja/sql/explain-plan/sql-server/getting-an-execution-plan)  

```sql : 条件を絞ってからJoin
select * from
(select * from Table_A where Name = '田中') A
left outer join Table_B B
on A.ID = B.ID
```

``` sql : Joinしてから条件を絞る
select * from
Table_A A
left outer join Table B
on A.ID = B.ID
where A.Name = '田中'
```

特にこれといった回答はなかった。  
これに関してはケースバイケースっぽい。  
後はオプティマイザーがどのように作用するのかといった感じか。  
まずはSQLServerの実行評価の見方を勉強してからになるかなー。  

[1]  
オプティマイザが十分賢ければ同等のプランとなるはずですから、両者の違いはほとんどないはずです。  
ただし2のクエリのほうが複雑ですから、(わずかでしょうが)コンパイルに時間がかかるでしょうし、間違ったプランを吐く可能性もないとはいえません。  
ですので、クエリは可能な限りシンプルにしたほうがいいでしょう。  

[2]  
URLに書いてあるように適切なインデックスが設定してあって、実行計画が合理的か確認します。  
現在は一つのテーブルだけで何億レコードってテーブルは珍しくないので、パフォーマンスの調査もできるだけ負荷が少ない方法も習得しておかなければなりません。  
調査するテーブルが田中さんが非常に少ない場合は ① が、TABLE_A と TABLE_B の JOIN 結果が少なければ ② の方が処理時間が短いのでは？って想像できます。  

---

## WHERE + CASE

- 検索項目に対する値を切り替えるパターン  
- 検索項目自体を切り替えるパターン  

``` sql
SELECT * 
FROM dbo.社員マスタ
WHERE 
    1=1
    AND 社員番号 = CASE WHEN @社員番号 IS NULL THEN 社員番号 ELSE @社員番号 END
    AND 社員名 = CASE WHEN @社員名 IS NULL THEN 社員名 ELSE @社員名 END
    AND 拠点 = CASE WHEN @拠点 IS NULL THEN 拠点 ELSE @拠点 END;
```

歩合給（comm）がNULLであれば給料（sal）、NULLでなければ給料と歩合給の合計値が、5000以上のレコードを問い合わせるSQL文の例。  

``` sql
SELECT *
FROM emp
WHERE
    CASE
        WHEN comm IS NULL THEN sal
        WHEN comm IS NOT NULL THEN sal + comm
    END >= 5000
```

[CASE式をWHERE句で使う（SQL Server）](https://www.dbsheetclient.jp/blog/?p=2211)  
[SQL CASE式](https://segakuin.com/oracle/sql/case.html)  
