# EFCore + Console

コンソールアプリからMigrationの実行、及びバンドルの作成に関してまとめ

---

## DIパターン 1

中々面倒な書き方だが、こうしないと`dotnet ef`コマンドによるマイグレーションファイルやバンドルの作成ができない。  
参考リンクの話によるとEF Core CLI は ASP.NET Core アプリケーションの Program クラスに定義されている (であろう) CreateWebHostBuilder メソッドを必要とするらしいので、ASP.Net CoreのStartupみたいな書き方でないといけないらしい。  

``` cs : Program.cs
using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services
            .AddDbContext<DatContext>(options =>
            {
                var appsettings = hostContext.Configuration.GetConnectionString("DefaultConnection");
                options.UseSqlServer(appsettings);
            });
    })
    .Build();
host.Services.GetService<DatContext>().Database.Migrate();
```

[dotnet ef migrations でエラーになった話](https://qiita.com/wukann/items/53462f4b21104ed75c31)  
[デザイン時 DbContext 作成](https://learn.microsoft.com/ja-jp/ef/core/cli/dbcontext-creation?tabs=dotnet-core-cli)  

---

## DIパターン 2

こちらのパターンの場合は`Migration`メソッドを実行することはできるが、`dotnet ef`コマンドによるマイグレーションファイルの生成などはできない。  
以下のようなエラーが発生してしまう。  

``` txt
Unable to create an object of type '○○DbContext'. For the different patterns supported at design time, see https://go.microsoft.com/fwlink/?linkid=851728
```

わざわざこの形にしてまでDIする必要もないので、これは備忘録として残しておく。  

``` cs
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using EFCoreSample.Model;

namespace EFCoreSample.Context;

public partial class DatContext : DbContext
{
    public DatContext(){}
    public DatContext(DbContextOptions<DatContext> options) : base(options) { }

    public virtual DbSet<HogeEntity> HogeEntity { get; set; }
}
```

``` cs : Program.cs
using Microsoft.EntityFrameworkCore;
using System;

var ob = new DbContextOptionsBuilder<DbContext>();
ob.UseSqlServer(@"Server=.\SQLEXPRESS;Database=<db_name>;Integrated Security=true");

using var dbContext = new DbContext(ob.Options);
dbContext.Database.Migrate();
```

jsonから取得するならこうなる。

``` cs
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var ob = new DbContextOptionsBuilder<DbContext>();
ob.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
dbContext.Database.Migrate();
```

---

## DIなし

DIしないのであれば、OnConfigureメソッドに直接記述して実行する。  
OnConfiguringから直接、接続文字列を与えないと起動できないし、bundleも生成できない。  

``` cs
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using EFCoreSample.Model;

namespace EFCoreSample.Context;

public partial class DatContext : DbContext
{
    public DatContext(){}

    // Contextクラスにおいて直接、接続情報を記述した場合
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(@"Server=.\SQLEXPRESS;Database=<db_name>;Integrated Security=True");
    }

    public virtual DbSet<HogeEntity> HogeEntity { get; set; }
}
```

``` cs : Program.cs
// Contextクラスにおいて直接、接続情報を記述した場合のMigrate実行
Console.WriteLine("開始");
using var datContext = new DatContext();
datContext.Database.Migrate();
```

---

## MigrationをTransactionで囲んだ場合RollBackされるか？

Bundleで実行した場合、途中でエラーになっても、エラー直前までのMigrationは適応されてしまう。  
ならばコンソールアプリからMigrationを実行した時にTransactionで囲んだ場合はRollbackされるのか検証してみた。  

**Rollbackされた**。  

既にMigrationされているデータベースに対しても有効であることを確認した。  

例:  
1,2,3,4という4つのMigrationfileがあるとして、3で強制的にエラーとさせる。  
既に1が適応されているデータベースに対して、TransactionありのMigrationを実行した結果、1のままであった。  
Transactionを外して実行したら、3でエラーとなって、2まで適応されることを確認した。  
なのでTransactionは有効である。  

しかし、この状態でバンドルを生成して同じように移行してみたが、その場合はRollbaskされなかった。  
割と残念。  

``` cs
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MigrationBundleConsoleAppExample.Context;

class Program
{
    private const string connectionString = @"Server=<server>;Database=<db_name>;User ID=<user_name>;Password=<passwd>";

    static void Main(string[] args)
    {
        using IHost host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddDbContext<DatContext>(options => options.UseSqlServer(connectionString));
            })
            .Build();
            
        using var dbContext = host.Services.GetService<DatContext>();
        try
        {
            dbContext.Database.BeginTransaction();
            dbContext.Database.Migrate();
            dbContext.Database.CommitTransaction();
        }
        catch (Exception e)
        {
            dbContext.Database.RollbackTransaction();
        }
        finally
        {
            dbContext.Dispose();
        }
    }
}
```

---

[EntityFrameworkCoreを.NET Core コンソールアプリでCodeFirstに使う](https://qiita.com/namoshika/items/7d1bf911bc03ed03e17d)  
