# DB接続文字列

## ConnectionString

### サーバを「DataSource」指定する場合  

【SQLSERVER認証】  
`Data Source=.\\SQLEXPRESS;Initial Catalog=[db_name];User ID=[userName];Password=[passwd];Trust Server Certificate=true`  

【Windows認証】  
`Data Source=.\\SQLEXPRESS;Initial Catalog=[db_name];Trust Server Certificate=true;Integrated Security=True`  
or  
`Data Source=.\\SQLEXPRESS;Initial Catalog=[db_name];Trust Server Certificate=true;Trusted_Connection=True`

### サーバを「Server」指定する場合  

【SQLSERVER認証】  
`Server=.\SQLEXPRESS;Database=[db_name];User ID=[userName];Password=[passwd];Trust Server Certificate=true`

【Windows認証】  
`Server=.\SQLEXPRESS;Database=[db_name];Trust Server Certificate=true;Integrated Security=True`  
or  
`Server=.\SQLEXPRESS;Database=[db_name];Trust Server Certificate=true;Trusted_Connection=True`

ローカルの指定は`.`以外にもあるがここでは`.`を使う。  

[【Sql Server2016】接続文字列が２種類ある件について](https://www.topse.work/entry/2019/05/29/120000)  

---

## Linuxでの接続

Linux上のSQLServerにアクセスする場合は以下のようにアクセスすればよい。  

`Server=localhost;Database=[myDatabase];User id=[myUserName];Password=[myPassword];Trust Server Certificate=true`  

以下、Linux上で動作しているSQLServerにアクセスしようとして色々あったのでまとめる。  
結果的にLinuxなのにWindows認証しようとしてエラーとなっていた。  

■**connection string is not valid sql server**  

こんな感じのWindow認証でログインしようとして発生した。  
`"Server=.\\SQLEXPRESS; Database=<db_name>; Integrated Security=True"`  

SQLServerでWindows認証が使えるはずないので当然と言えば当然。  
Windows認証 = WindowではローカルのSQLServerにアクセス → Linux上の(ローカルの)SQLServerにアクセス  
という理論でやったが、Localにアクセスなら素直に`localhost` or `127.0.0.1`だった。  

>Linuxは名前付きパイプの概念が違う。  
>また、統合セキュリティは、Active Directory / NTML Authorizationに関連しているため、Linuxでは動作しません（少なくとも未設定では、Kerberos認証はサポートされています）。  
>そのため、通常の標準的なユーザー名認証の接続文字列を使用します。  
>`Server=localhost;Database=myDatabase;User id=myUserName;Password=myPassword;`  
[Invalid SQL Server Connection String on Linux Entity Framework Core](https://stackoverflow.com/questions/55454878/invalid-sql-server-connection-string-on-linux-entity-framework-core)  

■**System.Data.SqlClient.SqlException: 'System.Net.Security.Native assembly:\<unknown assembly> type:\<unknown type> member:(null)**

`System.Net.Security`とあるので、セキュリティに関することなのは分かった。  
この時まだWindows認証で頑張っていたので、`Integrated Security`がセキュリティに関するオプションであることは明白だったので、消したらエラーも消えた。  
リンク先の人も消したら出てこなくなったと言っている。  

[System.Data.SqlClient.SqlException: 'System.Net.Security.Native assembly:\<unknown assembly> type:\<unknown type> member:(null)](https://learn.microsoft.com/en-us/answers/questions/979587/systemdatasqlclientsqlexception-39systemnetsecurit.html)  

---

## ローカルDBへの接続文字列

`@"Data Source=.\MSSQLLocalDB;AttachDbFilename=..\..\Database1.mdf;Integrated Security=True"`

``` cs
SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder
{
    DataSource = @".\MSSQLLocalDB",
    AttachDBFilename = System.IO.Path.GetFullPath(@"..\..\Database1.mdf"),
    IntegratedSecurity = true,
};
_ = builder.ConnectionString;
```

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

## Integrated Security vs Trusted_Connection

Trusted_ConnectionとIntegrated SecurityはどちらもWindows認証を使用してSQL Serverに接続することを指定します。  
Trusted_Connection=TrueとIntegrated Security=Trueは同じ意味となります。  
Integrated Security=SSPIとすることもあります。  
これも同じくWindows認証を使用することを意味します。  

つまり、次の3つの接続文字列は同じ接続方法を表しています。  

1. `Server=localhost;Database=your_database_name;Trusted_Connection=True;`
2. `Server=localhost;Database=your_database_name;Integrated Security=True;`
3. `Server=localhost;Database=your_database_name;Integrated Security=SSPI;`

>Windows 認証は、接続文字列に `Integrated Security` キーワードまたは `Trusted_Connection` キーワードを使用することによって指定できます。  
>こうすることで、ユーザー ID とパスワードを使う必要がなくなります。  
>[接続情報の保護 - ADO.NET | Microsoft Learn](https://learn.microsoft.com/ja-jp/dotnet/framework/data/adonet/protecting-connection-information)  

---

## Trusted_Connection

>Trusted_Connection=Trueは、Windows 認証を指定します。  
>つまり、Windows 資格情報を使用して SQL Server に接続します。  
>ライブ サーバーでは、SQL Server のユーザー名とパスワードを持つSQL Server 認証を使用します。  
>[Database-First approach in Entity Framework Core](https://www.yogihosting.com/database-first-approach-entity-framework-core/)  

---

## Persist Security Info

>`Persist Security Info` の既定値は false です。  
>すべての接続文字列には、この既定値を使用することをお勧めします。  
>`Persist Security Info` を true または yes に設定すると、ユーザー ID やパスワードなどのセキュリティ関連情報を、接続を開いた後にその接続から取得できます。
>`Persist Security Info` を false または no に設定した場合、その情報を使って接続を開いた後で、セキュリティ情報が破棄されるため、信頼できないソースによってセキュリティ関連情報がアクセスされることを確実に防ぐことができます。  
>[接続情報の保護](https://learn.microsoft.com/ja-jp/dotnet/framework/data/adonet/protecting-connection-information)  

---

## connection timeout

タイムアウト関連のエラーが発生した場合、考慮すべし。  

実例では、DbUpを使った移行作業において発生し、タイムアウト値を指定したら解決した。  

デフォルトは15秒な模様。  
[参考:SQL Serverに.NETで接続しようとすると1.2秒でタイムアウトする場合がある - give IT a try](https://blog.jnito.com/entry/20120219/1329633868)  

0を指定すると無限に待機するので絶対に指定しないこと。  

記述例  
`Data Source=(local);Initial Catalog=AdventureWorks;Integrated Security=SSPI;Connection Timeout=30`  

>サーバへの接続を待機する時間を指定します。指定した待機時間を過ぎると、接続を切断し、エラーを返します。  
>待機時間の単位は秒です。  
>0を指定した場合、待機時間に無制限が指定されます。接続が永続的に待機することになるので、0は指定しないようにしてください。  
>[付録B 接続文字列に指定可能なキーワード](https://software.fujitsu.com/jp/manual/manualfiles/m130025/j2ul1759/02z200/j1759-b-00-00.html)  

- 参考  
  - [SQL Serverに.NETで接続しようとすると1.2秒でタイムアウトする場合がある - give IT a try](https://blog.jnito.com/entry/20120219/1329633868)  
  - [SqlConnection.ConnectionTimeout プロパティ (System.Data.SqlClient) | Microsoft Learn](https://learn.microsoft.com/ja-jp/dotnet/api/system.data.sqlclient.sqlconnection.connectiontimeout?view=dotnet-plat-ext-7.0)  
  - [付録B 接続文字列に指定可能なキーワード](https://software.fujitsu.com/jp/manual/manualfiles/m130025/j2ul1759/02z200/j1759-b-00-00.html)

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
