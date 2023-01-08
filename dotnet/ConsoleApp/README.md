# ConsoleApp

コンソールアプリについてまとめるところ

---

## 埋め込みリソース

exeにファイルを埋め込むこと。  

>Visual Studioで開発をおこなっていると、画像ファイルやテキストファイルなどを、プログラムと一緒に配布する必要が出てきたりします。  
>多くの場合、付属するファイルをある特定のパスに格納することで対応したりします。  
>ただし、画像ファイルをユーザが編集できないようにプログラムを配布する場合もあります。  
>こういった場合、画像ファイルを実行プログラムに埋め込むことで、安易に編集できないように配布することができます。  
>これにより実行ファイルのフォルダに画像ファイルを配置せずとも、ビルドファイル内に埋め込まれたリソースファイルを利用することで画像の表示が可能になります。  
>[Visual Studioの埋め込みリソースについて](https://freestyle.nvo.jp/archives/59)  

---

## C#のコンソールアプリのコマンドライン引数

文字列のまとまりはダブルコーテーション。  
スプリットはスペース。  

``` C#
Console.WriteLine("Hello, World!");
foreach (var arg in args)
    Console.WriteLine(arg);
```

``` txt
TestConsole "tetst  aaa" "aaa wwww"

Hello, World!
tetst  aaa
aaa wwww
```

``` txt
TestConsole 'tetst  aaa' "aaa wwww"

Hello, World!
'tetst
aaa'
aaa wwww
```

---

## コンソールアプリから別のexeを引数ありで実行する

``` cs
using System;
using System.Diagnostics;

// Processクラスのオブジェクトを作成
Process process = new Process();
// コマンドプロンプト
process.StartInfo.FileName = "cmd.exe";
// コマンドプロンプトに渡す引数
process.StartInfo.Arguments = "/c dir";
// ウィンドウを表示しない
process.StartInfo.CreateNoWindow = true;
process.StartInfo.UseShellExecute = false;
// 標準出力および標準エラー出力を取得可能にする
process.StartInfo.RedirectStandardOutput = true;
process.StartInfo.RedirectStandardError = true;

// プロセス起動
process.Start();
// 標準出力を取得
string standardOutput = process.StandardOutput.ReadToEnd();
// 標準出力を表示
Console.WriteLine(standardOutput);
```

[C#でのexeファイルの扱い方とは？exeの起動・exeの実行結果を取り込む・exeの終了を待ち合わせる・exeのパスを取得する方法](https://www.fenet.jp/dotnet/column/language/9700/)

---

## コンソールアプリでappsettings.jsonを使う

単純そうに見えて、意外とやることが多い。  

NuGetからライブラリをインストールする。  

`Microsoft.Extensions.Configuration`  
`Microsoft.Extensions.Configuration.Json`  

`dotnet add package Microsoft.Extensions.Configuration --version 6.*`
`dotnet add package Microsoft.Extensions.Configuration.Json --version 6.*`

``` json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=<sv_name>;Database=<db_name>;User ID=<user_id>;Password=<passwd>"
  }
}
```

### appsettings.jsonをビルド結果に含める場合

プロジェクトファイルにセクション追加

``` xml : csproj
  <ItemGroup>
    <None Update="appsettings.json">
      <CopyToOutputDirectory>Always</CopyToOutputDirectory>
    </None>
  </ItemGroup>
```

デフォルトの場合、`bin/debug`階層のappsettings.jsonファイルを参照しに行く模様。  

``` cs
using Microsoft.Extensions.Configuration;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

Console.WriteLine(configuration.GetConnectionString("DefaultConnection"));
```

### プロジェクトファイルと同じ階層のappsettings.jsonを使用する場合

`Directory.GetCurrentDirectory()`はPCが処理を行っている場所を指し示すので、プロジェクトファイル階層で`dotnet run`を行う限りはプロジェクトファイル階層のappsettings.jsonを参照できる。  

``` cs
using System.IO;
using Microsoft.Extensions.Configuration;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

Console.WriteLine(configuration.GetConnectionString("DefaultConnection"));
```

[.NETのコンソールアプリでappsettings.jsonを使う (.NET6)](https://zenn.dev/higmasu/articles/b3dab3c7bea6db)
[How to Get Connection String from Appsettings.json? .net Core Console Application](https://yarkul.com/how-to-get-connection-string-from-appsettings-json-net-core-console-application/)  

---

## exeに全て内包させる

①csprojに`PublishSingleFile`タグを追加

``` xml
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net7.0</TargetFramework>
    <RootNamespace>Console_Self</RootNamespace>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <PublishSingleFile>true</PublishSingleFile> ←追加
  </PropertyGroup>
```

コマンド  

``` txt
dotnet publish -o Output -c Release --self-contained=true -r win-x64 -p:PublishSingleFile=true -p:PublishTrimmed=true
```

[単体で使えるexeファイルの生成](https://teratail.com/questions/217007)  
[単一ファイルの配置と実行可能ファイル](https://learn.microsoft.com/ja-jp/dotnet/core/deploying/single-file/overview?tabs=cli)  

---

## フォルダ全体を埋め込みリソースにする方法

特定のフォルダのファイルを全て埋め込みリソースとしたい場合の設定。  
csprojファイルに以下のタグを設定する。  

``` xml
  <ItemGroup>
    <EmbeddedResource Include="対象フォルダ名\*" />
  </ItemGroup>
```

ワイルドカードにより、拡張子を指定したり、除外したりすることができるので、必要十分な設定が可能。  

実際はこのような形で埋め込むことになる。  

``` xml
<Project Sdk="Microsoft.NET.Sdk">
  <hoge>
    ~~
  </hoge>

  <ItemGroup>
    <EmbeddedResource Include="FugaFolder\*.bmp" />
  </ItemGroup>
</Project>
```

- 参考  
  - [How can I have an entire folder be an embedded resource in a Visual Studio project?](https://stackoverflow.com/questions/8994258/how-can-i-have-an-entire-folder-be-an-embedded-resource-in-a-visual-studio-proje)  

■**例**

EFCoreのマイグレーションでコンソールアプリで遭遇した事例をそのまま使用する。  
以下のようなフォルダ構成の時にどのように表示されるか調査した。  

フォルダ構成

``` txt
Project
├─Migration
｜  ├─ConsoleAppSample.Migrations.20221129070349_First.cs
｜  ├─ConsoleAppSample.Migrations.20221129070349_First.Designer.cs
｜  ├─ConsoleAppSample.Migrations.20221129070420_Second.cs
｜  ├─ConsoleAppSample.Migrations.20221129070420_Second.Designer.cs
｜  └─ConsoleAppSample.Migrations.AppDbContextModelSnapshot.cs
└─Program.cs
```

出力コード

``` cs
    Assembly assembly = Assembly.GetExecutingAssembly();
    var stream = assembly.GetManifestResourceNames();
    foreach (var item in stream)
        Console.WriteLine(item);
```

■**```*```の場合の出力**

全て出力される。  

``` xml
  <ItemGroup>
    <EmbeddedResource Include="Migrations\*" />
  </ItemGroup>
```

``` txt
ConsoleAppSample.Migrations.20221129070349_First.cs
ConsoleAppSample.Migrations.20221129070349_First.Designer.cs
ConsoleAppSample.Migrations.20221129070420_Second.cs
ConsoleAppSample.Migrations.20221129070420_Second.Designer.cs
ConsoleAppSample.Migrations.AppDbContextModelSnapshot.cs
```

■**ワイルドカードを使用**

条件に該当するファイルが出力される。  

``` xml
  <ItemGroup>
    <EmbeddedResource Include="Migrations\*.Designer.cs" />
  </ItemGroup>
```

``` txt
ConsoleAppSample.Migrations.20221129070349_First.Designer.cs
ConsoleAppSample.Migrations.20221129070420_Second.Designer.cs
```

■**除外条件の設定**

`Remove`プロパティで設定可能な模様。  

``` xml
  <ItemGroup>
    <EmbeddedResource Include="Migrations\*.cs" />
    <EmbeddedResource Remove="Migrations\*.Designer.cs" />
    <EmbeddedResource Remove="Migrations\AppDbContextModelSnapshot.cs" />
  </ItemGroup>
```

``` txt
ConsoleAppSample.Migrations.20221129070349_First.cs
ConsoleAppSample.Migrations.20221129070420_Second.cs
```

- 参考  
  - [How do I exclude files/folders from a .NET Core/Standard project?](https://stackoverflow.com/questions/43173811/how-do-i-exclude-files-folders-from-a-net-core-standard-project)  

---

## 自己完結型で発行しつつ、zipで圧縮するバッチ

``` batch
@echo off
rem 自己完結型でwin-x86とlinux-x64向けに発行し、GitHubにアップロードするためにzip圧縮するバッチファイル

dotnet publish （１） -o （２） -c Release --self-contained=true -r linux-x64 -p:PublishSingleFile=true -p:PublishTrimmed=true
powershell Compress-Archive -Path （２） -DestinationPath （２）.zip -Force

dotnet publish （１） -o （３） -c Release --self-contained=true -r win-x86 -p:PublishSingleFile=true -p:PublishTrimmed=true
powershell Compress-Archive -Path （３） -DestinationPath （３）.zip -Force

dotnet publish （１） -o （３） -c Release --self-contained=true -r win-x64 -p:PublishSingleFile=true -p:PublishTrimmed=true
powershell Compress-Archive -Path （３） -DestinationPath （３）.zip -Force

@echo --- Finished ---
pause > nul
```

[.Net Coreのコントロールアプリを自己完結型で発行する際に参考にした情報](https://qiita.com/yusuke-sasaki/items/80bb84c4b3534d1481fc)  

---

## 単一exeとして発行するためのオプション設定

``` bat
dotnet publish -o Output-win -c Release --self-contained true -r win-x64 -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true

dotnet publish -o Output-linux -c Release --self-contained true -r linux-x64 -p:PublishSingleFile=true
```

windowsでsqlserverへの接続処理があると、`PublishSingleFile`をつけていても、○○.dllが絶対に生成されてしまう。  
それすらも内包させるオプションが`IncludeNativeLibrariesForSelfExtract`。  

---

## git.bashでexeを実行する方法

`./ <exe_name>`  

例:カレントディレクトリのefbundle.exeを実行する  
`./ efbundle`  

- 気を付けること  
  - exe名の直接指定では実行できない  
  - `.\`ではなく`./`  

[Windows git-bash.exeでバッチファイルを実行](https://teratail.com/questions/100039)  