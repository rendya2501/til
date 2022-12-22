# jsonメモ

---

## appsettings.json 有無 確認

appsettings.json existance check  

[How to check if Configuration Section exists in .NET Core?](https://stackoverflow.com/questions/44641488/how-to-check-if-configuration-section-exists-in-net-core)  

---

## appsettings.jsonの指定の仕方

``` cs
using Microsoft.EntityFrameworkCore;

// 1.
builder.Services.AddDbContext<DatContext>(options =>options.UseSqlServer("name=ConnectionStrings:DefaultConnection"));

// 2.
builder.Services.AddDbContext<DbContext>(options => options.UseSqlServer(builder.Configuration["DefaultSettings"] ?? ""));

// 3.
builder.Services.AddDbContext<DbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultSettings") ?? ""));
```

コンソールアプリの場合

``` cs
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
```

[How to use appsettings.json in Asp.net core 6 Program.cs file](https://stackoverflow.com/questions/69390676/how-to-use-appsettings-json-in-asp-net-core-6-program-cs-file)  
