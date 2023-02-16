# Tools

EntityFrameworkCoreを使用した開発において必要となるツールの紹介とまとめ。  

---

## パッケージ マネージャー コンソール (PMC)

>.NET プロジェクトにライブラリーやツールを追加するためのパッケージ管理ツール。  
[.NET 用パッケージマネージャー NuGet のインストールと使い方](http://yohshiy.blog.fc2.com/blog-entry-236.html)  

<!--  -->
>パッケージ マネージャー コンソール（PMC）とは、.NET Core SDKを使用して.NET Coreとその他のアプリケーション開発ツールをインストール、管理、更新するためのコンソール ベースのツールです。  
PMCは、アプリケーションの開発に必要なすべてのツールを保持し、簡単かつ一貫した方法でアップグレードすることができます。  
[openai]

EntityFrameworkにおいては、NuGetから`Microsoft.EntityFrameworkCore.Tools`をインストールすることでEFCoreに関するコマンドを実行することができる。  

VisualStudioでは下記手順を踏むことでコンソールを開くことができる。  
`ツール → NuGetパッケージマネージャー → パッケージ マネージャー コンソール`  

PMCコマンド  
[Entity Framework Core ツールのリファレンス - Visual Studio のパッケージ マネージャー コンソール](https://learn.microsoft.com/ja-jp/ef/core/cli/powershell)  

---

## dotnet-ef

Entity Framework Core 用のコマンドライン インターフェイス ツール  

パッケージマネージャーコンソールと同等の動作を他のCLIツールから実行するためのツール。  
これをインストールすることで、PMC以外のシェルからEFCoreに関する操作を行うことができる。  

文献が豊富でサポートも手厚く、環境に依存せずにEFCoreを扱えるようになるので入れるべし。  

dotnet-efコマンド  
[Entity Framework Core ツールのリファレンス - .NET Core CLI](https://learn.microsoft.com/ja-jp/ef/core/cli/dotnet)  

### dotnet-ef のインストール

■**グローバルインストール**

``` txt
dotnet tool install --global dotnet-ef
```

[Entity Framework Core ツールのリファレンス - .NET Core CLI_ツールのインストール](https://learn.microsoft.com/ja-jp/ef/core/cli/dotnet)  

■**ローカルインストール**

影響範囲をフォルダのみに限定したい場合や、環境の汚染を考慮する場合に行う。  

対象プロジェクトの階層で下記コマンドを実行  

``` txt
dotnet new tool-manifest
dotnet tool install dotnet-ef
```

バージョン指定してインストールしたい場合は下記のように記述する。  

``` txt
dotnet tool install dotnet-ef --version 6.*
```

[.NET ツールの管理方法](https://learn.microsoft.com/ja-jp/dotnet/core/tools/global-tools#install-a-local-tool)  

---

## Entity Framework Core tools

Entity Framework Coreを使用して開発するためのコマンドラインツール。  
これらのツールを使用すると、Entity Framework Coreモデルを作成したり、データベースを管理したりできる。  
NuGetから取得して使用する。  

>Entity Framework Core ツールは、設計時の開発タスクに役立ちます。  
主に移行の管理と、DbContext およびエンティティ型のスキャフォールディング (データベースのスキーマをリバース エンジニアリングする) に使用されます。  
[Entity Framework Core tools reference](https://learn.microsoft.com/ja-jp/ef/core/cli/)  

---

## dotnet tool メジャーバージョンの最新をインストールする

■メジャー バージョンが 5の最新を取得する。  
`dotnet tool install dotnet-ef --version 5.*`  

■メジャー バージョンが 5 で、マイナー バージョンが 0 で最新を取得する。  
`dotnet tool install dotnet-ef --version 5.0.*`  

■アップデートする場合  
`dotnet tool update --version 6.* dotnet-ef`  

※メジャー、マイナー、リビジョン  

[dotnet tool update](https://learn.microsoft.com/ja-jp/dotnet/core/tools/dotnet-tool-update)  
