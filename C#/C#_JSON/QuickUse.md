# JsonQuickUse

サクッとカレントディレクトリのappsetting.jsonを読み取る

---

## プロジェクトの作成

ターミナルから`dotnet new`コマンドを実行する。  
`dotnet new console -o JsonQuickUse -f net6.0`  

VSCodeを起動する。  
`code JsonQuickUse`  

VSCodeのターミナルを起動。  
`ctrl + j`  

ライブラリをnugetからインストール。  
`dotnet add package Microsoft.Extensions.Configuration --version 7.0.0`  
`dotnet add package Microsoft.Extensions.Configuration.Json --version 7.0.0`

---

## 実装

`appsettings.json`を作成する  

``` json
{
  "MySetting": "Hello,World!"
}
```

コードをコピペする。  

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
