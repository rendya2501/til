# QueryMultiple

## 基本

複数のSELECTステートメントの結果を受け取ることができる。  

``` cs
public static void Execute()
{
    var con_str = @"Server=.\SQLEXPRESS;Database=SandBox;Integrated Security=True;";
    using SqlConnection connection = new SqlConnection(con_str);

    var query = @"
        SELECT 1;
        SELECT 2;
        SELECT 'Hoge';
    ";

    var reader = connection.QueryMultiple(query);
    var one = reader.Read<int>();     // 1
    var two = reader.Read<int>();     // 2
    var hoge = reader.Read<string>(); // hoge
}
```

順番は守らないとエラーになる。  

``` cs
public static void Execute()
{
    var con_str = @"Server=.\SQLEXPRESS;Database=SandBox;Integrated Security=True;";
    using SqlConnection connection = new SqlConnection(con_str);

    var query = @"
        SELECT 1;
        SELECT 'Hoge';
        SELECT 2;
    ";

    var reader = connection.QueryMultiple(query);
    var one = reader.Read<int>();  // 1
    // 例外が発生しました: CLR/System.FormatException
    // 型 'System.FormatException' のハンドルされていない例外が System.Private.CoreLib.dll で発生しました: 
    // 'The input string 'Hoge' was not in a correct format.'
    var two = reader.Read<int>();
    var hoge = reader.Read<string>();
}
```

POCOを定義しておけば、その通りに受け取れる。  

``` cs
string constr = @"Server=<server_name>;Database=<db_name>;User ID=<id>;Password=<passwd>;Trust Server Certificate=true";
using SqlConnection connection = new SqlConnection(constr);

var query = @"
    SELECT * FROM Subject;
    SELECT * FROM SubjectType;
";

var reader = connection.QueryMultiple(query);
var subject = reader.Read<Subject>();
var subjectType = reader.Read<SubjectType>();

public class TSubject
{
    public int SubjectCD { get; set; }
    public string? SubjectName { get; set; }
    public int SubjectTypeCD {get;set;}
}
public class SubjectType
{
    public int SubjectTypeCD { get; set; }
    public string? SubjectTypeName { get; set; }
}
```

[“querymultiple dapper c#” Code Answer](https://www.codegrepper.com/code-examples/csharp/querymultiple+dapper+c%23)  

---

## Join結果をQueryMultipleで受け取る

参考リンクの方法ではReadメソッドの第2引数にフィールドの指定はなくとも取得できるとあったが、それでは駄目だった。  
デフォルトでは"id"でsplitされるようだが、エラーとなるので、適当なフィールドを入れたら全て取得出来た。  

``` cs
using Dapper;
using Microsoft.Data.SqlClient;

string constr = @"Server=<server_name>;Database=<db_name>;User ID=<id>;Password=<passwd>;Trust Server Certificate=true";
using SqlConnection connection = new SqlConnection(constr);
var query = @"
    SELECT 
        Subject.SubjectCD,
        Subject.SubjectName,
        Subject.SubjectTypeCD,
        SubjectType.SubjectTypeName
    FROM Subject 
    JOIN SubjectType 
    ON Subject.SubjectTypeCD = SubjectType.SubjectTypeCD
";
SqlMapper.GridReader multiQueryResult  = connection.QueryMultiple(query);
var result = multiQueryResult
    .Read((dynamic p, dynamic c) => new
    {
        SubjectCD = Convert.ToInt32(p.SubjectCD), 
        SubjectName = p.SubjectName,
        SubjectTypeCD = p.SubjectTypeCD,
        SubjectTypeName = c.SubjectTypeName
    },"SubjectTypeName");

foreach (var elem in result)
{
    Console.WriteLine($@"
        SubjectCD: {elem.SubjectCD} 
        SubjectName: {elem.SubjectName} 
        SubjectTypeCD: {elem.SubjectTypeCD} 
        SubjectTypeName: {elem.SubjectTypeName} 
    ");
}

public class Subject
{
    public int SubjectCD { get; set; }
    public string? SubjectName { get; set; }
    public int SubjectTypeCD {get;set;}
}

public class SubjectType
{
    public int SubjectTypeCD { get; set; }
    public string? SubjectTypeName { get; set; }
}
```

[DapperのQueryMutilpeを使って結合テーブルの分割マッピングをしてみる](https://qiita.com/Tokeiya/items/7e0e25757080e0259416)  
