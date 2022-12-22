# COUNT

[【SQL】COUNTの使い方（レコード数取得）](https://medium-company.com/sql-count/#:~:text=COUNT%E9%96%A2%E6%95%B0%E3%81%AE%E5%BC%95%E6%95%B0%E3%81%AB%E5%88%97%E5%90%8D%E3%82%92%E6%8C%87%E5%AE%9A%E3%81%99%E3%82%8B,%E3%81%99%E3%82%8B%E3%81%93%E3%81%A8%E3%81%8C%E3%81%A7%E3%81%8D%E3%81%BE%E3%81%99%E3%80%82&text=%E3%80%8CID%3D%221005%22%E3%80%8D,%E7%B5%90%E6%9E%9C%E3%81%8C%E5%8F%96%E5%BE%97%E3%81%A7%E3%81%8D%E3%81%BE%E3%81%99%E3%80%829)  

---

## COUNT(*) : 件数を取得

COUNT関数の引数に*(アスタリスク)を指定することで、レコード数を取得することができます。

---

## COUNT(列名）: NULLを除いた件数を取得

COUNT関数の引数に列名を指定することで、指定した列がNULL以外のレコード数を取得することができます。

---

## COUNT(DISTINCT 列名）: 重複を除いた件数を取得

COUNT関数の引数にDISTINCT 列名を指定することで、重複を除いたレコード数を取得することができます。

---

## COUNT(*)の意味とNULLのCOUNT

[COUNT(*)　が何を意味しているのかわからない](https://ja.stackoverflow.com/questions/42915/count-%E3%81%8C%E4%BD%95%E3%82%92%E6%84%8F%E5%91%B3%E3%81%97%E3%81%A6%E3%81%84%E3%82%8B%E3%81%AE%E3%81%8B%E3%82%8F%E3%81%8B%E3%82%89%E3%81%AA%E3%81%84)  
→そもそも構文エラーになる。  

**COUNT(*)は行数を数えてくれる**  
COUNT()は、行数を数えて出力する集計関数です。→平成27年秋期 午後問3の解説より  

``` txt
OracleではCOUNT(*)とCOUNT(age)の結果は異なります。
ageにnullが入っているとCOUNT(age)では件数にカウントされません。
グループ化していても同様で、ageがnullのグループのみ0件となります。
COUNT(*)ではageにnullが入っていてもレコードの件数をカウントします。

COUNT(*)ではレコードの内容を取得するため、COUNT('X')やSUM(1)を使った方が高速化できると教わったことがあります。(10年ほど前に聞いたノウハウなので現在も適用されるのかは不明ですが…)
```

なるほど。COUNTはNULLはカウントしないのね。  
動作的にCOUNT(name)見たいにフィールド名を指定したほうが高速化できるっぽいけど、単純にレコード数を取得したいならCOUNT(*)でいいのか。  

---

## COUNT(*)とCOUNT(カラム名)の違い

基本情報27年春の問題にて遭遇。  
なんだかんだわかってなかったのでまとめ。  

[【SQL】COUNT(*)とCOUNT(カラム名)の違い](https://qiita.com/TomoProg/items/5ba5779b3015ac02f577)  

・COUNT(*)はNULL値かどうかに関係なく、取得された行の数を返す  
・COUNT(カラム名)はNULL値でない値の行数を返す  

``` txt
+----+--------+-------+
| id | name   | price |
+----+--------+-------+
|  1 | apple  |   100 |
|  2 | banana |   120 |
|  3 | grape  |   140 |
|  4 | melon  |  NULL |
|  5 | kiwi   |   120 |
+----+--------+-------+
```

select count(*) from shohin; →そもそも構文エラーになる。 5  
select count(price) from shohin; →そもそも構文エラーになる。 4  

これも基本情報27年春の問題にて遭遇。  

[SQL | COUNT(DISTINCT column_name) は「同じ値の種類数」をカウントする](https://qiita.com/YumaInaura/items/1a1123ed4f33d30d9548)  
初歩だって。トホホ・・・。  
[COUNT句内でDISTINCTを使う／重複を排除したカウント](https://nyoe3.hatenadiary.org/entry/20100313/1268468670)  

つまり、重複行を除いたカウントをしたい場合に有効な構文というわけだ。  
そうなると次はDISTINCTとはどこまで含めることができるのか気になってきたぞ。  

``` txt
+-------+--------+-------+
| name  | sex    | score |
+-------+--------+-------+
| Alice | female |    60 |
| Bob   | male   |    70 |
| Carol | female |    70 |
| David | male   |    80 |
| Eric  | male   |    80 |
+-------+--------+-------+
```

sex には male / famale の二種類がある。  
SELECT COUNT(DISTINCT(sex)) AS sex_kind FROM scores; →そもそも構文エラーになる。 2  

score には 60点 / 70点 / 80点の三種類がある。  
る。 3  

こんな感じでも書ける  
なる。 5,2  
