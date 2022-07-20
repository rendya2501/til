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

[](https://atmarkit.itmedia.co.jp/bbs/phpBB/viewtopic.php?topic=32176&forum=26)  
