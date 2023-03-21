# IServiceCollectionのDIメソッド

## IServiceCollection

`IServiceCollection`は、ASP.NET Coreの依存関係注入 (DI) の中核となるサービスコレクションを提供する。  
依存関係注入は、アプリケーションで使用するサービスやオブジェクトのライフタイムを管理し、コンポーネント間の連携を容易にする。  

主に3つのDIメソッドが存在する。  

- AddSingleton  
- AddScoped  
- AddTransient  

これら3つのDIメソッドは、ASP.NET Coreアプリケーションの構築時にサービスを適切に登録し、管理するために使用される。  
それぞれ異なるライフタイムを持つため、サービスの用途に応じて適切なDIメソッドを選択する。  

---

## AddSingleton・AddScoped・AddTransient

### 1. AddSingleton

シングルトンとしてサービスを登録する。  
アプリケーション全体で1つのインスタンスが作成され、それがアプリケーションのライフタイム全体で使用される。  
シングルトンは、アプリケーション全体で共有されるデータやリソースにアクセスするサービスに適している。  
ただし、スレッドセーフに注意して実装する必要がある。  

### 2. AddScoped

スコープされたサービスを登録する。  
一般的に、1つのHTTPリクエストに対して1つのインスタンスが作成され、そのリクエスト内で共有される。  
リクエストが完了するとインスタンスは破棄される。  
このライフタイムは、リクエストごとに状態を持つサービスに適している。  
例えば、データベースコンテキストやリクエストスコープのキャッシュに使用される。  

### 3. AddTransient

トランジェントサービスを登録する。  
このタイプのサービスは、依存関係が解決されるたびに新しいインスタンスが作成される。  
トランジェントサービスは、短期間の使用や状態を持たないサービスに適している。  
例えば、バリデーションサービスや一時的なデータ処理タスクに使用される。  

---

## 予想

AddSingleton を使用した場合、何度アクセスしても同じインスタンスのアドレスが表示されるはず。  
AddScoped を使用した場合、同一HTTPリクエスト内では同じインスタンスのアドレスが表示されるが、異なるHTTPリクエストでは異なるインスタンスのアドレスが表示されるはず。  
AddTransient を使用した場合、アクセスするたびに異なるインスタンスのアドレスが表示されるはず。  

---

## 検証結果

プロジェクトを実行し、アドレスの数値を確認してみた。  

シングルトンは何度実行しても値が同じとなった。  
スコープはリロードする度に異なる値となったが、2つの値は同じとなった。  
トランジエントは常に異なる値をとることを確認できた。  

---

## 検証プログラムの実装

- Windows11  
- .Net 6  
- ASP.Net Core webapi  

### Service

#### ScopedService

``` cs
public interface IMyScopedService
{
    string GetInstanceAddress();
}

public class MyScopedService : IMyScopedService
{
    public string GetInstanceAddress()
    {
        return this.GetHashCode().ToString();
    }
}
```

#### SingletonService

``` cs
public interface IMySingletonService
{
    string GetInstanceAddress();
}

public class MySingletonService : IMySingletonService
{
    public string GetInstanceAddress()
    {
        return this.GetHashCode().ToString();
    }
}
```

#### TransientService

``` cs
public interface IMyTransientService
{
    string GetInstanceAddress();
}

public class MyTransientService : IMyTransientService
{
    public string GetInstanceAddress()
    {
        return this.GetHashCode().ToString();
    }
}
```

### Controller

``` cs
[ApiController]
[Route("[controller]")]
public class MyController : ControllerBase
{
    private readonly IMySingletonService _mySingletonService;
    private readonly IMyScopedService _myScopedService1;
    private readonly IMyScopedService _myScopedService2;
    private readonly IMyTransientService _myTransientService1;
    private readonly IMyTransientService _myTransientService2;

    public MyController(
        IMySingletonService mySingletonService,
        IMyScopedService myScopedService1,
        IMyScopedService myScopedService2,
        IMyTransientService myTransientService1,
        IMyTransientService myTransientService2)
    {
        _mySingletonService = mySingletonService;
        _myScopedService1 = myScopedService1;
        _myScopedService2 = myScopedService2;
        _myTransientService1 = myTransientService1;
        _myTransientService2 = myTransientService2;
    }

    [HttpGet]
    public IActionResult GetInstanceAddresses()
    {
        return Ok(new
        {
            // 何回リロードしても同じ値となる。  
            SingletonAddress = _mySingletonService.GetInstanceAddress(),
            // リロードする度に変化するが、どちらも常に同じになる。  
            ScopedAddress1 = _myScopedService1.GetInstanceAddress(),
            ScopedAddress2 = _myScopedService2.GetInstanceAddress(),
            // 常にどちらも違う。  
            TransientAddress1 = _myTransientService1.GetInstanceAddress(),
            TransientAddress2 = _myTransientService2.GetInstanceAddress()
        });
    }
}
```

### Program.cs

``` cs
using TimeToLive.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
ConfigureServices(builder.Services);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

// 検証用のサービスコンテナの登録
static void ConfigureServices(IServiceCollection services)
{
    services.AddSingleton<IMySingletonService, MySingletonService>();
    services.AddScoped<IMyScopedService, MyScopedService>();
    services.AddTransient<IMyTransientService, MyTransientService>();
}
```
