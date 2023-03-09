# Dapper Method

## Query

複数レコードを取得する。  
クエリの結果が何もない場合、空のIEnumerableを取得する。  

``` cs
string sql = "SELECT * FROM [Person]";
var person = connection.Query<Person>(sql);
```

---

## QueryFirstOrDefault

単一レコードを取得する。  
クエリの結果が何もない場合、nullを取得する。  

Query メソッドで取得した結果にFirstOrDefaultを使っても同じ結果になるが、パフォーマンスが落ちるので、単一のレコードを取得する場合はQueryFirstOrDefaultを使用すると良い。  

``` cs
string sql = "SELECT * FROM [Person] WHERE [ID] = @ID";
var param = new { ID = 1 };
var person = connection.QueryFirstOrDefault<Person>(sql, param);
```

---

## ExecuteScalar

単一レコードの特定の列を取得するメソッド。  
クエリの結果が何もない場合、nullを取得する。  

レコードを取得してから任意のプロパティを取得はできるが、QueryFirstOrDefaultと同様に、パフォーマンスが高くなるため、特定の列を取得する場合はExecuteScalarを使用すると良い。  

``` cs
string sql = "SELECT [Name] FROM [Person] WHERE [ID] = @ID";
var param = new { ID = 1 };
var name = connection.ExecuteScalar<string>(sql, param);
```

---

## Execute

INSERT,UPDATE,DELETE等を実行するためのメソッド。  
