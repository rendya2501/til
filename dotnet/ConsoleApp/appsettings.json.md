# コンソールアプリでappsettings.jsonを使う

単純そうに見えて、意外とやることが多い。  

---

NuGetからライブラリをインストールする。  

- `Microsoft.Extensions.Configuration`  
- `Microsoft.Extensions.Configuration.Json`  

インストールコマンド  

`dotnet add package Microsoft.Extensions.Configuration --version 6.*`
`dotnet add package Microsoft.Extensions.Configuration.Json --version 6.*`

jsonデータは以下の通りとする。  

``` json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=<sv_name>;Database=<db_name>;User ID=<user_id>;Password=<passwd>"
  }
}
```

---

## パターン1

``` cs
using Microsoft.Extensions.Configuration;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile(path:"appsettings.json",optional:true,reloadOnChange:true)
    .Build();
```

``` xml : csproj
<ItemGroup>
  <Content Include="appsettings.json">
    <CopyToOutputDirectory>Always</CopyToOutputDirectory>
  </Content>
</ItemGroup>
```

この文法の場合は`appsettings.json`は`Copy always`だけで済む。  
jsonファイルのプロパティで出力先ディレクトリにコピーを「**常にコピーする**」に変更する。  

[Quick Way to Get Connection String from AppSettings.json Console Application .net Core 3.1 and Later](https://yarkul.com/how-to-get-connection-string-from-appsettings-json-net-core-console-application/)  

---

## パターン2

``` cs
using Microsoft.Extensions.Configuration;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

Console.WriteLine(configuration.GetConnectionString("DefaultConnection"));
```

``` xml : csproj
<ItemGroup>
  <None Update="appsettings.json">
    <CopyToOutputDirectory>Always</CopyToOutputDirectory>
  </None>
</ItemGroup>
```

この文法の場合は`.SetBasePath(Directory.GetCurrentDirectory())`は必要ない模様。  
`appsettings.json`は`Copy always`だけで済む。  

[.NETのコンソールアプリでappsettings.jsonを使う (.NET6)](https://zenn.dev/higmasu/articles/b3dab3c7bea6db)  

---

## appsettings.jsonをビルド結果に含める場合

プロジェクトファイルに以下のセクションを追加。  
もしくは jsonファイルのプロパティでビルドアクションを「**コンテンツ**」に変更し、出力先ディレクトリにコピーを「**常にコピーする**」に変更する。  
このオペレーションを実行することで、csprojに下記xmlが追記される。  

``` xml : csproj
<ItemGroup>
  <Content Include="appsettings.json">
    <CopyToOutputDirectory>Always</CopyToOutputDirectory>
  </Content>
</ItemGroup>
```

以下のコードでjsonデータを取得可能。  

``` cs
using Microsoft.Extensions.Configuration;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

Console.WriteLine(configuration.GetConnectionString("DefaultConnection"));
```

デフォルトの場合、`bin/debug`階層のappsettings.jsonファイルを参照しに行く模様。  

[.NETのコンソールアプリでappsettings.jsonを使う (.NET6)](https://zenn.dev/higmasu/articles/b3dab3c7bea6db)

---

## プロジェクトファイルと同じ階層のappsettings.jsonを使用する場合

`Directory.GetCurrentDirectory()`はPCが処理を行っている場所を指し示すので、プロジェクトファイル階層で`dotnet run`を行う限りはプロジェクトファイル階層の`appsettings.json`を参照できる。  

``` cs
using System.IO;
using Microsoft.Extensions.Configuration;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

Console.WriteLine(configuration.GetConnectionString("DefaultConnection"));
```

---

[Quick Way to Get Connection String from AppSettings.json Console Application .net Core 3.1 and Later](https://yarkul.com/how-to-get-connection-string-from-appsettings-json-net-core-console-application/)  
[.NETのコンソールアプリでappsettings.jsonを使う (.NET6)](https://zenn.dev/higmasu/articles/b3dab3c7bea6db)  
