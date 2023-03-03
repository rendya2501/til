# QueryMultiple

## QueryMultipleで受け取る

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
```

``` cs
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
