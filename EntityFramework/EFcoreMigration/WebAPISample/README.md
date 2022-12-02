# WebAPISample

ASP.Net Core Web API のSwaggerを使ったマイグレーションを実行する管理画面を作成する。

---

## 開発環境

- .Net6  
- ASP.NET Web API  

---

## プロジェクト作成

``` .NET CLI
dotnet new webapi -n "WebAPISample"
```

---

## EFcoreツール(dotnet-ef)をインストールする  

環境を考慮し、ローカルインストールを採用する。  

``` .NET CLI
dotnet new tool-manifest
dotnet tool install dotnet-ef
```

---

## ライブラリのインストール

以下のライブラリをNuGetからインストールする。  

- `Microsoft.EntityFrameworkCore`  
- `Microsoft.EntityFrameworkCore.Design`  
- `Microsoft.EntityFrameworkCore.Tools`  
- `Microsoft.EntityFrameworkCore.SqlServer`  
- `Swashbuckle.AspNetCore.Swagger`  

PowerShellやCommandPromptからCLIで実行する場合は下記コマンドを実行する。  
NuGet公式サイトから検索した場合のコマンド。  

``` txt
dotnet add package Microsoft.EntityFrameworkCore --version 6.*
dotnet add package Microsoft.EntityFrameworkCore.Design --version 6.*
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 6.*
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 6.*
dotnet add package Swashbuckle.AspNetCore.Swagger --version 6.*
```

---

## コーディング

以下のプロジェクト構成でコーディングする。  

``` txt
project
├─Context
｜ └─AppDbContext.cs
├─Entities
｜ ├─Category.cs
｜ └─Product.cs
├─appsettings.json
└─Program.cs
```

``` cs : Entity
public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<Product> Products { get; set; }
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int CategoryId { get; set; }
}
```

``` cs : context
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Category> Category { get; set; }
    public DbSet<Product> Product { get; set; }
}
```

``` json : appsettings.json
{
    ~~,
    "ConnectionStrings": {
        "DefaultConnection": "Server=<server>;Database=<db_name>;User ID=<user_id>;Password=<passwd>"
    }
}
```

``` cs : Program.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services
            .AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(hostContext.Configuration.GetConnectionString("DefaultConnection"));
            });
    })
    .Build();
// await host.RunAsync();
```

### 移行を追加する1

コマンドを実行してmigrationファイルを生成する。  

``` txt : PMCコマンド  
Add-Migration First
```

``` txt : dotnet-ef
dotnet ef migrations add First
```

Migrationsフォルダが自動生成される。  

Migrationsフォルダにファイルが生成される。  
`yyyymmddhhmmss_First.cs`  
`yyyymmddhhmmss_First.Designer.cs`  

### 移行を追加する2

Descriptionフィールド追加  

``` cs : entity
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int CategoryId { get; set; }
}
```

コマンドを実行してmigrationファイルを生成する。  

``` txt : PMCコマンド  
Add-Migration Second
```

``` txt : dotnet-ef
dotnet ef migrations add Second
```

Migrationフォルダに2つ目のファイルが生成される。  
`yyyymmddhhmmss_Second.cs`  
`yyyymmddhhmmss_Second.Designer.cs`  

### バンドルを生成する  

PowerShell or CommandPromptでコマンド実行  

`dotnet ef migrations bundle --configuration Bundle`  

※PMCコマンドではエラーとなりバンドルを作成できない。  
プロジェクトがdllを参照しているからだと思われる。  

- 参考  
  - バンドル作成から実行までの流れを確認するのに適した動画  
    [Introduction to Migration Bundles - What can they do the migration scripts don't?](https://www.youtube.com/watch?v=mBxSONeKbPk)  
  - プロジェクトの構成はこのサイトのzipを参考にした  
    [EF Core 6  - Apresentando Migration Bundles](https://macoratti.net/21/09/efc6_migbndl1.htm)  

Swaggerにドキュコメを反映させる方法  
[Enhance Swagger Documentation with Annotations in ASP.NET core](https://medium.com/@niteshsinghal85/enhance-swagger-documentation-with-annotations-in-asp-net-core-d2981803e299)  

プログラムでマイグレーションを実行するための命令  
[EF Core – Apply migrations programmatically](https://makolyte.com/ef-core-apply-migrations-programmatically/)  