using Dapper;
using System.Data.SqlClient;

// 接続文字列
string constr_source = @"Data Source=.\SQLEXPRESS;Initial Catalog=BundleDB;Integrated Security=True";
string constr_server = @"Server=.\SQLEXPRESS;Database=BundleDB;Trusted_Connection=True";
string constr_hyblid = @"Server=.\SQLEXPRESS;Initial Catalog=BundleDB;Integrated Security=True";
// データベース一覧を取得するだけならデータベースの指定はいらない模様  
string constr_no = @"Server=.\SQLEXPRESS;Integrated Security=True";

using SqlConnection connection = new SqlConnection(constr_no);
var query = "SELECT * FROM sys.databases";
var result = connection.Query<dynamic>(query);
foreach (var item in result)
    Console.WriteLine(item.name);
