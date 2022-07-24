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
