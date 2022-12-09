# Blazorメモ

[BlazorSample](https://github.com/rendya2501/BlazorSample)  

---

## ファンクションのキーバインド

ファンクションのキーバインド設定可能。  
ダイアログ上でF5を掌握できることを確認した。  

``` razor
@if (ShowConfirmation)
{
    <div class="modal fade show" @onkeydown="KeyHandler" @onkeydown:preventDefault style="display:block" tabindex="-1">
    </div>
}

@code {
    private void KeyHandler(KeyboardEventArgs e)
    {
        if (e.Key == "F5")
        {
            _ = 1;
        }
    }
}
```

[ASP.NET Core Blazor のイベント処理](https://learn.microsoft.com/ja-jp/aspnet/core/blazor/components/event-handling?view=aspnetcore-6.0)  

---

## ダイアログ

[Blazor で確認ダイアログを削除する](https://www.youtube.com/watch?v=Caw5hmq4dEY)  
[Blazor Tutorial C# - Part 5 - Blazor Component Reference](https://www.youtube.com/watch?v=3Gr83lIaENg)  

[bootstrap](https://getbootstrap.jp/docs/5.0/components/modal/)  

---

## Styleタグ中のcharset

`@charset "utf-8";`はrazorファイルのStyleタグの中に記述できない。  
でも、全体のプロジェクトではutf8が適応されているっぽいので、別に書かなくても問題ない模様。  

---

## BrazorのCSSの分離

トップレベルにCSSを配置できることは分かったが、CSSってどうやって適応されていくのか？  
トップレベルに大本となるCSSを配置して、微妙に色などを変えたい場合はそれぞれのrazorファイルにCSSを書くのがお作法なのだろうか。  
bootstrapを使いつつ、共通的に使いたいcssコンポーネントを作った時、それはどこに配置すべきなのだろうか。  
そもそもCSSって後書きが強いのか？  
CSSの勉強しないとわからんな。  

[【Blazor Server】【ASP.NET Core】CSS isolation と MapControllers](https://mslgt.hatenablog.com/entry/2020/12/16/203458)  

---

## 静的ファイルを wwwroot 以外のフォルダに配置する

ASP.Netではwwwrootなるフォルダに静的コンテンツを配置する決まりになっている模様。  
各プロジェクトに静的コンテンツを配置したくばスタートアッププログラムで追加の設定が必要。  
その手順は全て参考サイトに乗っているので、そちらを参照されたし。  

[静的ファイルを wwwroot 以外のフォルダに配置する](https://sorceryforce.net/ja/tips/asp-net-core-content-static-file-another-folder)  

---

## ComboBox

InputSelect  
dropdown selection  
dropdownlist  

とりあえず、純正のコンポーネントで作ってみたが、Blazorは良質なUIフレームワークがたくさんあるので、特にこだわりがなければそちらを使ったほうがいいかもね。  

``` html
@page "/combo_box"

<h3>ComboBox</h3>

<EditForm Model="@this.dummyModel">

    <InputSelect @bind-Value="_selectId" @oninput="OnItemInput">
        @foreach (var item in this.Values)
        {
            <option value="@item.id">@item.name</option>
        }
    </InputSelect>

    <h2>選択中ID :  @SelectedItem.id</h2>
    <h2>選択中Name :  @SelectedItem.name</h2>
</EditForm>
```

``` cs
@code {
    private DummyModel dummyModel = new();
    public class DummyModel { }

    protected List<(int id, string name)> Values { get; set; }
    protected (int id, string name) SelectedItem { get; set; }
    protected int _selectId;

    protected override async Task OnInitializedAsync()
    {
        this.Values = new List<(int id, string name)>();
        for (var i = 0; i < 10; i++)
        {
            this.Values.Add((id: i, name: $"test{i}"));
        }
        SelectedItem = this.Values.First();
        _selectId = SelectedItem.id;
    }

    private void OnItemInput(ChangeEventArgs e)
    {
        if (e.Value == null) return;

        var selected_id = Int32.Parse(e.Value.ToString());
        var person = Values.FirstOrDefault(x => x.id == selected_id);
        SelectedItem = (person.id, person.name);
    }
}
```

[radzen](https://blazor.radzen.com/)  
[Blazor の InputSelect コンポーネントについて学ぶ](https://www.gunshi.info/entry/2021/11/19/020708)  

---

## Blazor_CRUD

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

---

## blazor multi select bind

[.NET6 Blazor selectタグ multiple @bind](https://sumomo.ohwaki.jp/wordpress/?p=406)  

---

RazorでのDapperサンプル（dynamicで受け取る場合）
dapper-smple1.cshtml

``` cs
@using Dapper;
@{
    dynamic users;
    using( var cn = new System.Data.SqlServerCe.SqlCeConnection("接続文字列"))
    {
       cn.open();
       users　= cn.Query("SELECT * FROM users");
    }
}
<!DOCTYPE html>
<html lang="en">
    <head >
        <meta charset="utf-8" />
        <title></ title>
    </head>
    <body>
        <ul>
        @foreach( var d in users){
            <li> @d.Name : @ d.Email : @d.Age</li>
        }
        </ul>
    </body>
</html>
```

[kiyokura/dapper-smple1.cshtml](https://gist.github.com/kiyokura/7185300)  

---

## 参考サイト

<https://qiita.com/tags/blazor>
[.NET 6 と Entity Framework Core InMemory を使用した Blazor Server CRUD](https://www.youtube.com/watch?v=ii6QzWudZ6E)  
