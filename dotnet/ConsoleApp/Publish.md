# 発行関連

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

## Microsoft.Data.SqlClient.SNI.dllを消すためのオプション設定

コンソールアプリをexe発行するとき、windowsでsqlserverへの接続処理があると、`PublishSingleFile=true` をつけていても、`Microsoft.Data.SqlClient.SNI.dll`が生成されてしまう。  
それすらも内包させるオプションが`IncludeNativeLibrariesForSelfExtract`となる模様。  

linuxでは問題ない。  
windows向けに発行した場合に発生する。  

■ **Windows向け オプションなし**

``` bat
dotnet publish -o Output-win -c Release --self-contained true -r win-x64 -p:PublishSingleFile=true
```

- 生成されるファイル  
  - ConsoleAppSample.exe  
  - ConsoleAppSample.pdb  
  - Microsoft.Data.SqlClient.SNI.dll  

■ **Windows向け オプションあり**

``` bat
dotnet publish -o Output-win -c Release --self-contained true -r win-x64 -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true
```

- 生成されるファイル  
  - ConsoleAppSample.exe  
  - ConsoleAppSample.pdb  

■ **Linux向け**

``` bat
dotnet publish -o Output-linux -c Release --self-contained true -r linux-x64 -p:PublishSingleFile=true
```

- 生成されるファイル  
  - ConsoleAppSample  
  - ConsoleAppSample.pdb  

[.net - How do I get rid of SNI.dll when publishing as a "single file" in Visual Studio 2019? - Stack Overflow](https://stackoverflow.com/questions/65045224/how-do-i-get-rid-of-sni-dll-when-publishing-as-a-single-file-in-visual-studio)  

---

## 発行時にexe名を指定する

`dotnet msbuild -r -p:Configuration=Release;AssemblyName=foo`  

`dotnet publish AssemblyName=appname`  

[c# - How do I change the dll name when I dotnet publish? - Stack Overflow](How do I change the dll name when I dotnet publish?)  
