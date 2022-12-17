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

[ASP.NET Core 7 MVC Full Crud Operation Using ADO.NET](https://www.youtube.com/watch?v=YNcF53YvAwE)  

---

## 躓いたところ

- ローカルDBへのアクセス  
- 接続文字列のReplase  
- ignoreのignore  

ローカルDBへのアクセスは右クリックして接続情報を取得すべし。  
接続文字列は決まりきっているので、メモしておくのがよい。  

ローカルDBは`AttachDBFilename`の指定が必要。  
でもって、`AttachDBFilename=|DataDirectory|\\Database1.mdf`の`|DataDirectory|`の意味がわからなかった。  
これはこの部分をReplaceするための代替文字列みたいなモノらしい。  

``` json
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDBFilename=|DataDirectory|\\Database1.mdf;Integrated Security=True;",
  },
```

``` cs
// string path = Directory.GetCurrentDirectory();
// path = path.Replace(@"\TekitouCRUD\Server", @"\TekitouCRUD\Shared\Database");
var path = string.Concat(new DirectoryInfo(Directory.GetCurrentDirectory()!).Parent.FullName, @"\Shared\Database");
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection").Replace("|DataDirectory|", path)));
```

[How to use |DataDirectory| substitution string in appsettings.json with asp.net core?](https://stackoverflow.com/questions/55955282/how-to-use-datadirectory-substitution-string-in-appsettings-json-with-asp-net)  

`Directory.GetCurrentDirectory()`から1つ前の親を取得したい場合に調べたリンク  
[親ディレクトリのパスを取得する (C#プログラミング)](https://www.ipentec.com/document/csharp-get-parent-directory-path)  

---

## CORS

[この動画](https://www.youtube.com/watch?v=TCLLVz8Wk3A)ではCORSの問題が発生して、[このサンプル](https://www.c-sharpcorner.com/article/crud-operations-using-blazor-net-6-0-entity-framework-core/)ではCORSの問題が発生しない。  
違いは何か？

動画の54分ごろ。  
動画の方では全てURLで指定している。  

記事のサンプルの方では`GetFromJsonAsync`メソッドでapi以下のURLを指定している。  
自分自身のAPIへのアクセスは内部のロジックから行うべきということか。  

CORSが発生する。

``` cs
protected async Task GetUser(){
    string apiUrl = "https://localhost:7172/api/User";
    var response = await Http.GetStringAysnc(apiUrl);
}
```

CORSが発生しない。  

``` cs
protected async Task GetUser()
{
    var response = await Http.GetFromJsonAsync<List<User>>("api/User");
}
```

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
