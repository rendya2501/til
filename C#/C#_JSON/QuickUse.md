# JsonQuickUse

サクッとカレントディレクトリのappsetting.jsonを読み取るサンプル

---

## プロジェクトの作成

ターミナルから`dotnet new`コマンドを実行する。  
`dotnet new console -o JsonQuickUse -f net6.0`  

VSCodeを起動する。  
`code JsonQuickUse`  

`appsettings.json`を作成する。  

``` json
{
  "MySetting": "Hello,World!"
}
```

---

## Microsoft.Extensions.Configuration での実装

ライブラリをnugetからインストールする。  
`dotnet add package Microsoft.Extensions.Configuration --version 7.0.0`  
`dotnet add package Microsoft.Extensions.Configuration.Json --version 7.0.0`

以下の実装を行う。  

``` cs
using Microsoft.Extensions.Configuration;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Build();

var mySetting = configuration["MySetting"];
Console.WriteLine($"MySetting value is {mySetting}");
// MySetting value is Hello,World!
```

---

## Newtonsoft での実装

ライブラリをnugetからインストールする。  
`dotnet add package Newtonsoft.Json --version 13.0.3`  

以下の実装を行う。  

```cs
using Newtonsoft.Json.Linq;

// 現在のディレクトリからappsettings.jsonファイルのパスを取得する
var appSettingsPath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");

// appsettings.jsonファイルを読み込む
var appSettingsJson = File.ReadAllText(appSettingsPath);

// appsettings.jsonファイルの内容をJObjectに変換する
var appSettings = JObject.Parse(appSettingsJson);

// キーを指定して値を取得する
var someSetting = appSettings["MySetting"].Value<string>();
Console.WriteLine($"MySetting value is {someSetting}");
// MySetting value is Hello,World!
```

---

## System.Text.Json での実装

ライブラリ自体はデフォルトで入っているので、nuget等からインストールする必要はない。  

以下の実装を行う。  

``` cs
using System.Text.Json.Nodes;

// 現在のディレクトリからappsettings.jsonファイルのパスを取得する
var appSettingsPath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");

// appsettings.jsonファイルを読み込む
var appSettingsJson = File.ReadAllText(appSettingsPath);

// 不定形の取得にはSystem.Text.Json.Nodes.JsonNodeを使う
var appSettings = JsonNode.Parse(appSettingsJson);

// キーを指定して値を取得する
var mySetting = appSettings?["MySetting"];
Console.WriteLine($"MySetting value is {mySetting}");
// MySetting value is Hello,World!
```

[System.Text.Json で型が不定のJSONを扱う(.NET) | Qreat](https://qreat.tech/3292/)  