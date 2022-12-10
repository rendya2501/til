# Blazor_CRUD

---

プロジェクト作成  

Blazor WebAssembly App  
ASP.NET Core Hostedにチェックを入れる  

Blazor.Client : クライアント側のコードと、ブラウザに表示されるページが含まれる。  
Blazor.Server : データベース接続、オペレーション、ウェブAPIなどのサーバーサイドのコードが含まれる。  
Blazor.Shared : クライアントとサーバーの両方からアクセス可能な共有コードが含まれる。  

[CRUD Operations Using Blazor, .Net 6.0, Entity Framework Core](https://www.c-sharpcorner.com/article/crud-operations-using-blazor-net-6-0-entity-framework-core/)  
[Blazor .NET 7 CRUD using ADO.NET & Stored Procedures | Blazor CRUD Operations | C# Blazor CRUD](https://www.youtube.com/watch?v=TCLLVz8Wk3A)  

---

view ↔ viewmodel ↔ model ↔ unit of work ↔ repository ↔ db

``` cs : service
public class HogehogeDataService
    {
        // DbContext being injected by DI
        HogehogeDbContext _Context { get; }

        public HogehogeDataService(HogehogeDbContext context) =>
            _Context = context;

        /// <summary>
        /// Gets the entire user list.
        /// </summary>
        /// <returns></returns>
        public Task<List<User>> GetUsersAsync() =>
            _Context.Users
                .OrderBy(x => x.Id)
                .ToListAsync();
    }
```

``` cs
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<HogehogeDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("HogehogeConnection"),
                    providerOptions => providerOptions.CommandTimeout(120)));
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddScoped<HogehogeDataService>();
        }

        // ～略～
    }
```

``` cs : razor
@code {
    List<User> users;

    protected override async Task OnInitializedAsync()
    {
        users = await HogehogeData.GetUsersAsync();
    }
}
```

[[ASP.NET Core] Blazor Server 入門 (EF Core + SQL Server 編)](https://mseeeen.msen.jp/asp-dotnet-core-blazor-ef-core-sqlserver/)  

[How to Build a Blazor CRUD Application with Dapper](https://www.syncfusion.com/blogs/post/build-blazor-crud-application-with-dapper.aspx)  
