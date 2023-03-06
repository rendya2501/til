# IDENTITY

---

## Identityの発番テーブルで保存時に発番した値を取得する

`OUTPUT句`なるものがあるらしい。  

T-SQLに分類されるっぽいのでSQLServer独自の構文。  

INSERT、UPDATE、DELETE、MERGE ステートメントで影響を受けたレコードを取得することができる。  
それぞれで書き方が微妙に違う。  
実務では自動発番されたIDENTITYの値を取得するために使用していた。  

INSERTの場合はINTO句とVALUE句の間に `OUTPUT inserted.フィールド,inserted.フィールド,...`を記述することで指定した列の挿入した値を取得できる模様。  
SQLで使う場合はテーブル変数を用意してそれにINTOすることで複数の結果を取得可能。  

``` sql
-- テーブル変数を定義
DECLARE @StudentInserted TABLE (
   StudentID INT,
   FirstName VARCHAR(50),
   LastName VARCHAR(50)
);

-- INSERTとOUTPUTの実行
INSERT INTO Student
   (FirstName, LastName, Gender)
OUTPUT 
   inserted.StudentID, 
   inserted.FirstName, 
   inserted.LastName
INTO @StudentInserted (
   StudentID,
   FirstName,
   LastName
)
VALUES 
   ('Rin', 'Yokota', 'M'),
   ('Hina', 'Yokota', 'F');

-- 結果を表示
SELECT *
FROM @StudentInserted;
```

Dapperで使う場合はINTO句は必要なく、戻り値として受け取ることができる模様。  

``` C#
    // [SeqNo]という[Identity]フィールドがある[Table]
    var query = new StringBuilder()
        .AppendLine("INSERT INTO [Table] (")
        .AppendLine("   [Id],")
        .AppendLine("   [Name]")
        .AppendLine(")")
        .AppendLine("OUTPUT inserted.[SeqNo]")
        .AppendLine("VALUES (")
        .AppendLine("@Id,")
        .AppendLine("@Name")
        .AppendLine(")");
    // クエリ実行
    var identity = _DapperAction.GetFirstDataByQuery<long>(
        ConnectionTypes.SystemWithOutTransactionScope, 
        query.ToString(), 
        data
    );
```

OUTPUT句で出力する以外ではこういうものもあるらしい。  
`var id = rnWebConnection.QueryFirstOrDefault<long>("SELECT LAST_INSERT_ID()");`  

もしかしたら結果が1つだけなら変数受けすることもできるかもしれないと思ったが駄目だった。  

``` sql
DECLARE @Test INT
set @Test = (INSERT INTO [Table] (TestID,TestName) OUTPUT inserted.TestID VALUES (1,'Test'));
SELECT @Test;
```

[SQL Server - OUTPUT 句の使い方](https://sql55.com/query/sql-server-output-clause.php)  
[c# — Dapperで挿入を実行し、挿入されたIDを返すにはどうすればよいですか？](https://www.web-dev-qa-db-ja.com/ja/c%23/dapper%E3%81%A7%E6%8C%BF%E5%85%A5%E3%82%92%E5%AE%9F%E8%A1%8C%E3%81%97%E3%80%81%E6%8C%BF%E5%85%A5%E3%81%95%E3%82%8C%E3%81%9Fid%E3%82%92%E8%BF%94%E3%81%99%E3%81%AB%E3%81%AF%E3%81%A9%E3%81%86%E3%81%99%E3%82%8C%E3%81%B0%E3%82%88%E3%81%84%E3%81%A7%E3%81%99%E3%81%8B%EF%BC%9F/941401073/)  
