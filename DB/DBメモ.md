# データベース

---

## レプリケーション

レプリケーション : 複製

[わかりそうレプリケーション (replication)](https://wa3.i-3-i.info/word12869.html)  

平成30年春午後のネットワークでレプリケーションサーバーってのが出てきて、単語だけ調べるつもりだったけど、DBにまで範囲が及んだのでまとめることにした。  
レプリケーションは複製という意味になる。  
オリジナルDBに書き込みつつ、同じ内容をレプリカDBにも書き込む事をレプリケーションというらしい。  
つまり、リアルタイムに複製を生成する技術ということになるか。  

話だけ聞くとバックアップと同じように感じるが、バックアップはある時点のデータをメディアに記録して別の場所に保管することを指すので、全然違う。  
後、障害が発生した場合、環境から用意しないといけないが、レプリケーションは同じ環境で動作しているものを切り替えるだけで済むので対応力も違う。  
一応その記事も置いておきますね。  
[レプリケーションとは？バックアップと何が違う？](https://bcblog.sios.jp/what-is-replication/)  

---

## CASCADE

削除するテーブルに依存しているオブジェクト（ビューなど）を自動的に削除します。  

DROP TABLEは、削除対象のテーブル内に存在するインデックス、ルール、トリガ、制約を全て削除します。  
しかし、ビューや他のテーブルから外部キー制約によって参照されているテーブルを削除するにはCASCADEを指定する必要があります  
（CASCADEを指定すると、テーブルに依存するビューは完全に削除されます。  
しかし、外部キー制約によって参照されている場合は、外部キー制約のみが削除され、その外部キーを持つテーブルそのものは削除されません）。  

---

## リレーション

[わかりそう](https://wa3.i-3-i.info/word11596.html)  
>リレーション（英：relation）とは「リレーションシップ」のこと。  
用語の中身としてはアレとコレの「関係性」のこと。  
E-R図で出てくる線のこと、とも言えます。
データベースの話で出てきたら「テーブル」のこと。  

<https://twitter.com/n_1215/status/1554268701951934465>  
>RDBではテーブルこそがリレーションだと知らずテーブル同士の関連をリレーションと呼んでしまうITエンジニアの割合 - Wikipedia  

[wiki]  
>関係（かんけい、リレーション、英: relation）とは関係モデル（リレーショナルモデル）において、一つの見出しと0以上の同じ型の組 (タプル、行) の順序づけられていない集合からなるデータ構造のことである。  
>関係データベースのデータベース言語であるSQL では、関係変数とほぼ同じ意味で表 (テーブル) という用語が使われている。  

リレーションはテーブル同士の関連だと思ってたところ、ツイートを見たので調べてみたら、どうやら本当にテーブルの事らしい。  

---

## Redis

>Redisは、ネットワーク接続された永続化可能なインメモリデータベース。  
>連想配列（キー・バリュー）、リスト、セットなどのデータ構造を扱える。  
>いわゆるNoSQLデータベースの一つ。  
>オープンソースソフトウェアプロジェクトであり、Redis Labs（英語版）がスポンサーとなって開発されている。  
>[wiki]  

## Vertica

カラム指向データベース  

>Vertica Systems (ヴァーティカシステムズ)、はアメリカ合衆国マサチューセッツ州に本拠を置く、分析データプラットフォームを提供するソフトウェア会社である。  
グリッドベースかつ列指向のVertica Analytic Databaseは大規模かつ急増するデータを管理し、データウェアハウス等の多クエリアプリケーションに使用するため、高速のクエリ性能を提供するために設計されている。  

[wiki](https://ja.wikipedia.org/wiki/Vertica)  

---

## オンプレミス

>オンプレミスとは、システムを運用する上で必要なソフトウェア・ハードウェアを自社で保有・管理する運用形態です。  
自社でハードウェアなどを保有しない運用形態であるクラウドとは、対になるあり方として、よく比較されます。  

- メリット  
  - セキュリティ管理を自社で行える  
  - カスタマイズや他システムとの連携がしやすい  

- デメリット  
  - 運用開始までにコストや時間がかかる  
  - 保守やトラブル対応など運用の負担が大きい  

[クラソル](https://business.ntt-east.co.jp/content/cloudsolution/column-251.html#:~:text=%E3%82%AA%E3%83%B3%E3%83%97%E3%83%AC%E3%83%9F%E3%82%B9%E3%81%A8%E3%81%AF%E3%80%81%E3%82%B7%E3%82%B9%E3%83%86%E3%83%A0%E3%82%92,%E3%81%A8%E3%81%97%E3%81%A6%E3%80%81%E3%82%88%E3%81%8F%E6%AF%94%E8%BC%83%E3%81%95%E3%82%8C%E3%81%BE%E3%81%99%E3%80%82)  

---

## カーディナリティ

>SQL (Structured Query Language) では、カーディナリティという用語は、データベーステーブルの特定の列 (属性) に含まれるデータ値の一意性を指します。  
カーディナリティが低いほど、列内の要素の重複が多くなります。  
したがって、カーディナリティが最も低い列は、すべての行で同じ値になります。SQL データベースはカーディナリティを使用して、特定のクエリに最適なクエリ プランを決定します。  
[wiki]

男女は2なのでカーディナリーは低い。  
1年は365日なのでカーディナリーは365となり、2より高いと言える。  
インデックスはカーディナリーが高いものに張るべし。  

※cardinality : 数学的な用法で 濃度  

[カーディナリティについてまとめてみた](https://qiita.com/soyanchu/items/034be19a2e3cb87b2efb)  
[カーディナリティが高い低いとは](https://dolphinpg.net/program/sql/cardinality/)  

---

## フルテキストインデックス

>インデックス（索引）は、データベースの性能を向上させる方法の一つです。  
しかし、通常のIndex では text ベースのカラム(CHAR型、VARCHAR型、TEXT型) から特定の文字列を検索する全文検索には向いていません。  
それは、通常のIndex はカラムの値の一部ではなく、値全体に対する検索に最適化されているからです。  
そのため、全文検索 (カラムの値の一部が一致している結果を取得) するには、別のインデックス FULLTEXT INDEX が必要です。  
[世界一わかりやすい FULLTEXT INDEX の説明と気を付けるべきポイント](https://zenn.dev/hiroakey/articles/9f68ad249af20c)  

---

## プリペアードステートメント

SQL文で値がいつでも変更できるように、変更する箇所だけ変数のようにした命令文を作る仕組みのこと。  
パラメータクエリともいうらしい。  
Dapperにおいて`@変数`として動的にSQL文を組み立てるあの部分がそういうことになる。  

[プリペアドステートメントを利用してデータを取得する方法について解説！](https://qiita.com/wakahara3/items/d7a3674eecd3b021a21e)  
[C# .NET パラメータクエリでSQLインクジェクション対策](https://greentown.tokyo/dotnet-sqlinjection/)  

---

## トランザクションの肥大化

[トランザクションログ肥大化の対処方法 (log_reuse_wait_desc : LOG_BACKUP) [SQL Server]](https://www.nobtak.com/entry/tlogs2)  
[【INDEX】SQL Server トランザクションログ肥大化 (原因／対処方法)](https://www.nobtak.com/tlogidx#2-%E3%83%88%E3%83%A9%E3%83%B3%E3%82%B6%E3%82%AF%E3%82%B7%E3%83%A7%E3%83%B3%E3%83%AD%E3%82%B0%E3%82%92%E8%82%A5%E5%A4%A7%E5%8C%96%E3%81%95%E3%81%9B%E3%81%A6%E3%81%84%E3%82%8B%E3%82%AF%E3%82%A8%E3%83%AA%E3%81%AE%E7%89%B9%E5%AE%9A%E6%96%B9%E6%B3%95)  
[トランザクションログを肥大化させているクエリの特定方法 [SQL Server]](https://www.nobtak.com/entry/tlogs0)  

---

## DROP TABLEやTRUNCATE TABLEはロールバック可能か？

SQLServerではロールバック可能であることを確認した。  
他はDBによる模様。  
ロールバック不可能なDBに関してはdrop tableやtruncate等のDDL(テーブル構造)命令は、RollBackが利かないらしい。  

``` sql
drop table if exists employees;
create table employees(dept_id int,name varchar(32));
insert into employees(dept_id,name) values(1,'田中');
insert into employees(dept_id,name) values(2,'玉木');
insert into employees(dept_id,name) values(3,'鈴木');
GO

BEGIN TRY
    BEGIN TRANSACTION

    DROP TABLE employees;
    -- TRUNCATE TABLE employees;
    SELECT 1/0

    COMMIT TRANSACTION
END TRY

BEGIN CATCH
    THROW
    ROLLBACK TRANSACTION
END CATCH
GO

select * from employees
```

---

## DDLのトランザクション

■**PostgreSQL**  

CREATE TABLEやALTER TABLEなどのDDL命令もCOMMIT、ROLLBACKの対象になる。  

PostgreSQLでは、CREATE TABLE や DROP TABLE などの DDL もトランザクションの一部となるため、トランザクションの途中でDROP TABLE を実行した場合でも、最後に ROLLBACK すれば、DROP したテーブルが元に戻ります。  

■**Oracle**

DDLはトランザクション対象にはならない。暗黙コミットされる。

■**MySQL**

DDLはトランザクション対象にはならない。暗黙コミットされる。

■**SqlServer**

DDL(create文等)はロールバックすることが出来る。  
SQL Server ではTransaction 管理下ではRollback が可能なようです。

[DDLのトランザクション(PostgresSQL,Oracle,MySQL)](https://tamata78.hatenablog.com/entry/2017/02/20/112026)  
[SqlServerではDDL(create文等)をロールバックすることが出来る](https://culage.hatenablog.com/entry/20110129/p6)  
[SQL Server でDDLがRollbackできる？](https://www.ilovex.co.jp/Division/ITD/archives/2005/05/sql_server_ddlr.html)  

### DELETE

表内のデータを(全)削除する。  

`DELETE FROM (表名);`  

- 語訳は「削除」  
- DML(データ操作言語)  
- COMMITしていなければロールバック可能です。  

### TRUNCATE

表内のデータを全削除する。

`TRUNCATE TABLE (表名);`  

- 語訳は「切り取る」  
- DDL(データ定義言語)  
- TRUNCATE文はWHERE句で指定できませんのでテーブルのデータを全て削除する。  
- テーブルごと削除してから再作成するのでDELETE文よりも高速。  
- トランザクションが効かない。  
- ロールバックができない。  

### DROP

表内のオブジェクトを完全に削除する。  

`DROP TABLE (表名);`  

- 語訳は「捨てる」  
- DDL(データ定義言語)  
- 完全に削除するのでロールバックができません。表構造も残りません。  
- DROP文はオブジェクトに対するSQL文なのでTABLEを変えてあげれば索引なども削除できる。  

[DELETE文 TRUNCATE文 DROP文の違い(SQL構文)](https://www.earthlink.co.jp/engineerblog/technology-engineerblog/2680/)  

---

## 暗黙的なコミット

MySQLでの話にはなるが、概要として理解するには十分なのでそのまま引用する。  

MySQLの暗黙的なコミットは、特定のクエリを実行した際に現在のセッションで実行されているトランザクションを全てコミットしてから実行されるクエリで、クエリ自身の実行後もコミットされます。  

DROP TABLEを行ったクエリの順番で考えていきます。

``` sql
mysql> START TRANSACTION;
mysql> DROP TABLE zipcode;
mysql> ROLLBACK;
```

上記のクエリに、先ほど説明した内容の暗黙的なコミットを明示的に入れ込んでみると、以下のようになります。

``` sql
mysql> START TRANSACTION;
mysql> COMMIT; -- 処理の前に自動的にコミットされる
mysql> DROP TABLE zipcode;
mysql> COMMIT; -- 処理の後に自動的にコミットされる
mysql> ROLLBACK;
```

ということで、ROLLBACKを打ったとしても結果が全てコミットされてしまっているため、元に戻せないことがわかります。

よくある悲劇的な話としては、テーブル内のデータの削除の高速化のためにDELETE文で削除していたものを、TRUNCATE TABLEに変更した時などに起こります。  
トランザクション処理の途中で単純に置き換えをしてしまった場合に、暗黙のコミットが挟まってしまって予期せぬ挙動になってしまうことがあります。  

[DDLと暗黙的なコミットについて](https://gihyo.jp/dev/serial/01/mysql-road-construction-news/0134)  

---

## CONSTRAINT 句

`CONSTRAINT 制約の名前 制約`  

`CONSTRAINT [○○_PKC] PRIMARY KEY ([フィールド1],[フィールド2],・・・)`  

テーブルのインデックスフォルダの中を見ると`Customers_PKC(クラスター化)`という名称でインデックスが生成される。  
CONSTRAINT でインデックス名の指定をしない場合`PK_Customer_*****(クラスター化)`という名称でインデックスが生成される。  
`*****`の部分は16桁のランダムな16進数となる模様。  

制約を生成するのでFOREIGN KEY等も作成可能な模様。  

CONSTRAINT句で制約名を設定しなくても複合主キーは設定できるが、ランダムチックな制約名になってしまう。  
SQLServer2016の教科書でもできるなら任意の名前をつけたほうがよいとのこと。  

``` sql
CREATE TABLE Customers(
    CustomerID nvarchar(20), 
    CustomerName nvarchar(20), 
    CustomerAdd nvarchar(50) NULL,
    PRIMARY KEY(CustomerID,CustomerName)
);
```

``` sql
CREATE TABLE Customers(
    CustomerID nvarchar(20), 
    CustomerName nvarchar(20), 
    CustomerAdd nvarchar(50) NULL,
    CONSTRAINT [Customers_PKC] PRIMARY KEY(CustomerID,CustomerName)
);
```

外部キー制約も作れる。

``` sql
CONSTRAINT `制約の名前`
    FOREIGN KEY (`このテーブルの列名を外部キーに設定`)
    REFERENCES `データベース名`.`テーブル名` (`カラム名`)
    ON DELETE NO ACTION ←親テーブルの削除時何もしない
    ON UPDATE NO ACTION ←親テーブルの更新時何もしない
```

CONSTRAINT : 制限、強制  

[SQL文でのINDEX句、CONSTRAINT句について](https://toru-takagi.dev/article/3)  

---

## PRIMARY KEY指定

主キーが1つだけの場合は`フィールド名 型 PRIMARY KEY`でもよいし、一番最後に`PRIMARY KEY (フィールド名)`のどちらでもよい。  
主キーが複数の場合は、CREATE TABLEの一番最後に`PRIMARY KEY (フィールド1,フィールド2,・・・)`の形でなければならない。  

``` sql
CREATE TABLE Customers(
    CustomerID nvarchar(20), 
    CustomerName nvarchar(20), 
    CustomerAdd nvarchar(50) NULL,
    PRIMARY KEY(CustomerID)
);
```

``` sql
CREATE TABLE Customers(
    CustomerID nvarchar(20) PRIMARY KEY,
    CustomerName nvarchar(20),
    CustomerAdd nvarchar(50) NULL
);
```

``` sql
CREATE TABLE Customers(
    CustomerID nvarchar(20), 
    CustomerName nvarchar(20), 
    CustomerAdd nvarchar(50) NULL,
    CONSTRAINT [Customers_PKC] PRIMARY KEY(CustomerID)
);
```

---

## sql where 仕組み

SELECT文の内部的な処理の基本は、テーブルの各行に対する繰り返し処理  
プログラミングでいうところのfor文。  

SELECT文の処理は2つに分けられる。  

- テーブルを1行ずつ処理する。  
- テーブル内の複数の行を集約する。  

2つのうち、どちらの処理が行われるかは以下で判断できる。  

- SELECT句で集約関数を使わない場合、テーブルは1行ずつ処理される。  
- SELECT句で集約関数を1つでも使った場合、テーブル内の複数の行で集約が行われる。  

[SELECT文の処理の仕組みを説明してみた。](https://nattou-curry-2.hatenadiary.org/entry/20090315/1237089749)

---

## UNION VS UNION ALL

UNIONくっそ重いのでUNION ALL使おうねって話。  
重複削除したかったらUNION ALLした後にDISTICTでもいいよね。  

てかUNIONはJoinの度に重複削除するわけだから効率が悪い。  
全部UNIONした後に重複削除で十分だ。  

``` sql
-- バッチ相対 0%
SELECT 'aaa' 
UNION ALL
SELECT 'ccc' 
UNION ALL
SELECT 'ddd'
UNION ALL
SELECT 'ddd'
-- (列名なし)
-- aaa
-- ccc
-- ddd
-- ddd

-- バッチ相対 100%
SELECT 'aaa' 
UNION 
SELECT 'ccc' 
UNION 
SELECT 'ddd'
UNION 
SELECT 'ddd'
-- aa
-- aaa
-- ccc
-- ddd

SELECT DISTINCT
    *
FROM (
    SELECT 'aaa'  AS aa
    UNION ALL
    SELECT 'ccc' 
    UNION ALL
    SELECT 'ddd'
    UNION ALL
    SELECT 'ddd'
) AS a
-- aa
-- aaa
-- ccc
-- ddd
```

``` sql
SELECT 'aaa' as A1,'bbb' AS A2,'ccc' AS A3
GO
-- A1    A2    A3
-- aaa    bbb    ccc


SELECT
    [CD]
FROM
    (SELECT 'aaa' as A1,'bbb' AS A2,'ccc' AS A3) As a
CROSS APPLY(
    values
    (A1),(A2),(A3)
) AS [v] ([CD])
GO
-- CD
-- aaa
-- bbb
-- ccc


WITH TEMP
AS (
    SELECT 'aaa' as A1,'bbb' AS A2,'ccc' AS A3
)
SELECT
    [CD]
FROM TEMP
CROSS APPLY(
    values
    (A1),(A2),(A3)
) AS [v] ([CD])
-- CD
-- aaa
-- bbb
-- ccc
```

---
