# DB接続文字列

---

## appsettings.json

``` json
ConnectionStrings
DefaultConnection "Server=localhost\\SQLEXPRESS;Database=[db_name];Trusted_Connection=true;"
// or DefaultConnection "Server=(local)\\SQLEXPRESS;Database=[db_name];Trusted_Connection=true;"
```

``` cs
{
    private readonly IConfiguration _config;

    public constractor(IConfiguration config){
        _config = config;
    }

    void Test (){
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
    }
}
```

## App.config

``` xml
<connectionStrings>
  <add name="DB1" 
       providerName="System.Data.SqlClient"
       connectionString="Server=.\MSSQLSERVER2017;Database=Test;Integrated Security=SSPI;"/>
</connectionStrings>
```

``` xml
<connectionStrings>
    <add name="connectionStringName"
         connectionString="Data Source=(※データソース指定);Initial Catalog=(データベース名);User ID=(ユーザーID);Password=(パスワード)"
         providerName="System.Data.SqlClient"/>
</connectionStrings>
```

``` cs
using System.Configuration;
{
    void aa(){
        string db = ConfigurationManager.ConnectionStrings["connectionStringName"].ConnectionString;
    }
}
```

[C# - データベースの接続文字列を App.config から取得する](https://csharp.sql55.com/database/get-connection-string-from-app-config.php)  

---

## App.Configとappsettings.json

[.NET Core の設定情報の仕組みをしっかり理解したい方向け基本のキ](https://blog.ecbeing.tech/entry/2020/03/16/114500)

App.Configは.NetFramework時代の産物
.NetCoreからappsetting.jsonになったっぽいので、特に理由やこだわりがなければappsettings.jsonだけでよい

---

## 接続文字列構築パターン(愚直)

``` C# : SQL Serverへの接続文字列の構築
    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder
    {
        // 接続先の SQL Server インスタンス
        DataSource = "localhost",
        // 接続ユーザー名
        UserID = "sa",
        // 接続パスワード
        Password = "your_password",
        // 接続するデータベース
        InitialCatalog = "master",
        // 接続タイムアウトの秒数(ms) デフォルトは 15 秒
        ConnectTimeout = 60000
    };
    SqlConnection connection = new SqlConnection(builder.ConnectionString);
```

``` C# : ローカルDBへの接続文字列の構築 SqlConnectionStringBuilderパターン
    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder
    {
        DataSource = @"(LocalDB)\MSSQLLocalDB",
        AttachDBFilename = System.IO.Path.GetFullPath(@"..\..\..\SampleDatabase.mdf"),
        IntegratedSecurity = true,
    };
    SqlConnection connection = new SqlConnection(builder.ConnectionString);
```

``` C# : ローカルDBへの接続文字列の構築 直接指定パターン1
    //Data Source=(SQL Server のホスト名またはIPアドレス);Initial Catalog=(接続先データベース名);Connect Timeout=60;Persist Security Info=True;User ID=(SQL Serverに接続するユーザーID);Password=(ユーザーのパスワード)
    string constr = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\~~~~~\LocalDB\SampleDatabase.mdf;Integrated Security=True";
    SqlConnection con = new SqlConnection(constr);
```

``` C# : ローカルDBへの接続文字列の構築 直接指定パターン2
    // 相対パスで指定するのもあり
    string constr = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={System.IO.Path.GetFullPath(@"..\..\SampleDatabase.mdf")};Integrated Security=True";
    SqlConnection con = new SqlConnection(constr);
```
