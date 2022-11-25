# Tools

## パッケージ マネージャー コンソール

PowerShell コマンドを使用して NuGet パッケージを操作するためのコンソール。  

VisualStudioでは下記手順を踏むことでコンソールを開くことができる。  
`ツール → NuGetパッケージマネージャー → パッケージ マネージャー コンソール`  

NuGetから`Microsoft.EntityFrameworkCore.Tools`をインストールすることでEFCoreに関するコマンドを実行することができる。  

[Entity Framework Core ツールのリファレンス - Visual Studio のパッケージ マネージャー コンソール](https://learn.microsoft.com/ja-jp/ef/core/cli/powershell)  

■インストール手順  

`Install-Package Microsoft.EntityFrameworkCore.Tools`  

Version指定する場合  
`Install-Package Microsoft.EntityFrameworkCore.Tools -Version 5`  

インストールの確認  
`Get-Help about_EntityFrameworkCore`  

---

## dotnet-ef

Entity Framework Core 用のコマンドライン インターフェイス ツール  

パッケージマネージャーコンソールと同等の動作を他のCLIツールから実行するためのツール。  
これをインストールすることで、PowerShell以外のCLIからEFCoreに関する操作を行うことができる。  

文献が豊富でサポートも手厚く、環境に依存せずにEFCoreを扱えるようになるので入れるべし。  

[Entity Framework Core ツールのリファレンス - .NET Core CLI](https://learn.microsoft.com/ja-jp/ef/core/cli/dotnet)  

---

## Entity Framework Core tools

EFCoreのパッケージ マネージャー コンソール コマンドを実行できるようにするためのツール。  

NuGetからインストールして使用する。  

>Entity Framework Core ツールは、設計時の開発タスクに役立ちます。  
主に移行の管理と、DbContext およびエンティティ型のスキャフォールディング (データベースのスキーマをリバース エンジニアリングする) に使用されます。  
[Entity Framework Core tools reference](https://learn.microsoft.com/ja-jp/ef/core/cli/)  

---

## dotnet ef のローカルインストール

対象プロジェクトの階層で下記コマンドを実行  

`dotnet new tool-manifest`  
`dotnet tool install dotnet-ef --version 6.*`  

アップデートする場合  
`dotnet tool update --version 6.* dotnet-ef`  

[.NET ツールの管理方法](https://learn.microsoft.com/ja-jp/dotnet/core/tools/global-tools#install-a-local-tool)  

---

## メジャーバージョンの最新をインストールする

`dotnet tool install dotnet-ef --version 5.*`  

メジャー バージョンが 5 で、マイナー バージョンが 0 で最新を取得する  
`dotnet tool install dotnet-ef --version 5.0.*`  

メジャー、マイナー、リビジョン  

[dotnet tool update](https://learn.microsoft.com/ja-jp/dotnet/core/tools/dotnet-tool-update)  
