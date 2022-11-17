# DB接続文字列

---

## ConnectionString

①サーバを「DataSource」指定する場合  

【SQLSERVER認証】  
`Data Source=.\\SQLEXPRESS;Initial Catalog=[db_name];User ID=[userName];Password=[passwd]`  

【Windows認証】  
`Data Source=.\\SQLEXPRESS;Initial Catalog=[db_name];Integrated Security=True`  

②サーバを「Server」指定する場合  

【SQLSERVER認証】  
`Server=.\SQLEXPRESS;Database=[db_name];User ID=[userName];Password=[passwd]`

【Windows認証】  
`Server=.\SQLEXPRESS;Database=[db_name];Trusted_Connection=True`  

ローカルの指定は`.`以外にもあるがここでは`.`を使う。  

[【Sql Server2016】接続文字列が２種類ある件について](https://www.topse.work/entry/2019/05/29/120000)  

---

## コマンドプロンプトでの接続

コマンドプロンプトで接続情報を記述する場合、`\`は１つだけで良い。  
2つ書くとエラーになる。  
割と嵌った。  

○  
`.\efbundle.exe --connection "Data Source=.\SQLEXPRESS;Initial Catalog=BundleDB2;Integrated Security=True"`

×
`.\efbundle.exe --connection "Data Source=.\\SQLEXPRESS;Initial Catalog=BundleDB2;Integrated Security=True"`

○  
`.\efbundle.exe --connection "Server=.\SQLEXPRESS;Database=BundleDB2;Integrated Security=True"`

×  
`.\efbundle.exe --connection "Server=.;Database=BundleDB2;Integrated Security=True"`

---

## コンソールアプリからappsettings.jsonを使用する

`dotnet add package Microsoft.Extensions.Configuration --version 6.*`  

``` json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=.;Initial Catalog=[db_name];Integrated Security=True"
  }
}
```

``` cs
using Microsoft.Extensions.Configuration;

class Program
{
    static void Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
           .AddJsonFile("appsettings.json")
           .Build();
        string connectionString = configuration.GetConnectionString("DefaultConnection");
    }
}
```

[.NETのコンソールアプリでappsettings.jsonを使う (.NET6)](https://zenn.dev/higmasu/articles/b3dab3c7bea6db)  

---

## DIでappsettings.jsonを使用する1

``` cs
public class HogeClass
{
    private readonly IConfiguration _config;

    public HogeClass(IConfiguration config){
        _config = config;
    }

    void Use (){
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
    }
}
```

---

## DIでappsettings.jsonを使用する2

EntityFrameworkでの使い方に近い。  

``` cs
using Microsoft.Extensions.Options;

public class HogeClass
{
    private readonly ConnectionSetting _connection;

    public HogeClass(IOptions<ConnectionSetting> connection)
    {
        _connection = connection.Value;
    }
}
```

``` cs
builder.Services.Configure<ConnectionSetting>(builder.Configuration.GetSection("DefaultConnection"));
```

``` cs
public class ConnectionSetting
{
    public string SQLString { get; set; }
}
```

---

## App.config

``` xml
<connectionStrings>
  <add name="connectionStringName" 
       providerName="System.Data.SqlClient"
       connectionString="Server=.\MSSQLSERVER2017;Database=Test;Integrated Security=SSPI;"/>
</connectionStrings>
```

``` xml
<connectionStrings>
    <add name="connectionStringName"
         providerName="System.Data.SqlClient"
         connectionString="Data Source=(※データソース指定);Initial Catalog=(データベース名);User ID=(ユーザーID);Password=(パスワード)" />
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

App.Configは.NetFramework時代の産物。  
.NetCoreからappsetting.jsonになったっぽいので、特に理由やこだわりがなければappsettings.jsonだけでよい。  

>コンソールアプリでも App.congig に代わる新しい定義ファイルの形式である appsettings.json を使用する場合の設定と実装方法の紹介です。  
>ASP.NET Core および ASP.NET 5～6 であれば、IServiceCollection.Configure にセクション名を渡せば勝手に内容をオブジェクトにマッピングしてくれる機能が存在しますが、コンソールアプリで始めてしまうとそこらへんが存在しないので自分で読み書きする形になります。  
>[【C#】appsetting.jsonをコンソールで扱う](https://takap-tech.com/entry/2021/10/09/182217)  

[.NET Core の設定情報の仕組みをしっかり理解したい方向け基本のキ](https://blog.ecbeing.tech/entry/2020/03/16/114500)

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

---

【Persist Security Info】
規定はFalse
true または yes に設定すると、ユーザー ID やパスワードなどのセキュリティ関連情報を、接続を開いた後にその接続から取得できる

【Integrated Security または Trusted_Connection】
Windows 認証 (統合セキュリティ) を使用する

【Server または Data Source】
接続先DB名

【Database または Initial Catalog】
規定のDB

【User ID または UID】
アカウント名

【Password または PWD】
アカウントのパスワード

[SQLServer接続文字列](https://blog.goo.ne.jp/turukit/e/697074faf2ada034c446994e70f49637)  

---

## Integrated Security・Trusted_Connection

Windows認証モードで接続する。  

>Windows 認証は、接続文字列に `Integrated Security` キーワードまたは `Trusted_Connection` キーワードを使用することによって指定できます。こうすることで、ユーザー ID とパスワードを使う必要がなくなります。  
>[接続情報の保護](https://learn.microsoft.com/ja-jp/dotnet/framework/data/adonet/protecting-connection-information)  

---

## Persist Security Info

>`Persist Security Info` の既定値は false です。  
>すべての接続文字列には、この既定値を使用することをお勧めします。  
>`Persist Security Info` を true または yes に設定すると、ユーザー ID やパスワードなどのセキュリティ関連情報を、接続を開いた後にその接続から取得できます。
>`Persist Security Info` を false または no に設定した場合、その情報を使って接続を開いた後で、セキュリティ情報が破棄されるため、信頼できないソースによってセキュリティ関連情報がアクセスされることを確実に防ぐことができます。  
>[接続情報の保護](https://learn.microsoft.com/ja-jp/dotnet/framework/data/adonet/protecting-connection-information)  

---

## Windows認証のすゝめ

>ユーザー情報の漏洩を防ぐためにも、できるだけ Windows 認証 ("統合セキュリティ" とも呼ばれます) を使用することをお勧めします。  
>[接続情報の保護](https://learn.microsoft.com/ja-jp/dotnet/framework/data/adonet/protecting-connection-information)  

---

## ローカルの指定の仕方

sqlserver connection string dot  

>.と(local)とYourMachineNameはすべて同等であり、自分のマシンを参照しています。  
>ローカル マシンにすべてのデフォルト オプションでインストールされた「通常の」SQL Server の場合は、  
>`.    or   (local)     or          YourMachineName`  
>すべてのデフォルト設定でインストールされたSQL Server Expressの場合は、次を使用します。  
>`.\SQLEXPRESS    or   (local)\SQLEXPRESS     or          YourMachineName\SQLEXPRESS`  
>[SQL Server Connection Strings - dot(".") or "(local)" or "(localdb)"](https://stackoverflow.com/questions/20217518/sql-server-connection-strings-dot-or-local-or-localdb)  

---

## 「信頼されていない機関によって証明書チェーンが発行されました」の解決法

appsettings.jsonの接続文字列に追加  
`Trust Server Certificate=true`  

[SNAC アプリケーションのアップグレード後に"信頼されていない機関によって証明書チェーンが発行されました" というエラー](https://learn.microsoft.com/ja-jp/troubleshoot/sql/connect/certificate-chain-not-trusted?tabs=ole-db-driver-19)  
