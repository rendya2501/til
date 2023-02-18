# dotnetメモ

---

## dotnet cli

>.NET Core コマンドライン インターフェイス（CLI）は、.NET Core アプリケーションの開発、デプロイ、および管理を行うためのツールチェーンです。  
コンソールで操作するためのコマンドラインツールおよびスクリプトを提供します。  
CLIを使用すると、.NET Core アプリケーションをプロジェクト作成、パッケージ化、およびデプロイすることができます。  
さらに、.NET Core アプリケーションを実行し、デバッグすることもできます。  
[openai]

---

## VSCode開発における他プロジェクトの参照

.csprojに以下のように直接記述するかコマンドを叩く。  
コマンド叩いた結果が以下の結果であるので、相対パスでプロジェクトを追加するほうが楽かもしれない。  

``` xml
  <ItemGroup>
    <ProjectReference Include="..\OPMLCoredotNET\src\OPMLCore.NET\OPMLCore.NET.csproj" />
  </ItemGroup>
```

コマンドの場合はこのようにするらしい。  

``` txt
dotnet add reference ../OPMLCoredotNET/src/OPMLCore.NET/OPMLCore.NET.csproj
dotnet restore
```

[.NET Core, Visual Studio Codeでプロジェクト参照を追加する](https://www.aruse.net/entry/2018/09/09/203402)  

---

## .NetFrameworkから.NetCoreへの移行

.NetCoreのプロジェクトは基本的にプロジェクトファイルだけがあればよい。  
後はプロジェクトファイルの内容が.NetCoreの書式になっていればよいので、.NetFrameworkから移行する場合は、Framework依存のフォルダ等を全て削除して、プロジェクトファイルの内容を.NetCoreの書式に書き換えるだけで移行できる。  

これを知らないとプロジェクトから作り直して、フォルダを移動してという面倒な作業が必要になる。(1敗)  

---

## Windowsデスクトップ向け業務アプリ開発には何を採用すべきか？

- UWPで要件を満たせるのであれば[UWP]  
- Webに慣れた開発者が多ければ[Electron]か[React Native]  
- iOS/Android向けアプリも一緒に開発するなら[Xamarin]か[React Native]  
- そうでなければ[WPF]一択  

WPFを選択した場合、.NET Framework 4.7/4.8にするか、.NET Core 3にするのかも選ばなければいけません。  
将来的なサポートやテストしやすさを考えれば、.NET Core 3から.NET 5へと続く道へ。  
リリースが間近か、現在ある資産をどうしても手放せないのであれば、.NET Framework 4.7/4.8でしょうか。  

[Windowsデスクトップ向け業務アプリ開発には何を採用すべきか？](https://qiita.com/sengoku/items/fb4948e0d2746e3cc26f)  
[WPF, Modern App (Metro App), UWP が低迷した理由 - iPentecのUIフレームワーク採用状況](https://www.ipentec.com/document/windows-development-new-ui-platforms-have-slumped)  

---

## WPFとWEBAPIを同じソリューションに含めて実行する  

ソリューションの`スタートアッププロジェクトの設定`により実現できる。  

- ソリューションを作成する。  
- ソリューションのプロジェクトとしてWPFプロジェクトとWebAPIプロジェクトを作成する。  
- ソリューションを右クリック → `スタートアッププロジェクトの設定(A)...`  
- 共通プロパティ→スタートアッププロジェクト→マルチスタートアッププロジェクトを選択する。  
- WPFプロジェクトとWebAPIプロジェクトのアクションを`開始`にする。  

[Asp.Net Core Web API Client/Server Application | Visual Studio 2019](https://www.youtube.com/watch?v=Bz5S86jmXQQ)  
[Consume ASP.Net Core Web API Using HttpClient in WPF](https://www.youtube.com/watch?v=qb3o_PIwVUk)  

---

## マニフェストファイル

アプリケーション マニフェスト [application manifest]  
アプリケーションおよびそのすべての構成ファイルを記述するファイル。ClickOnce アプリケーションによって使用されます。  

俺が調べたい内容はこんな事だっただろうか。  
発行の時に追加されるあれについてだった気がする。  
それ以外の意味では、見た目に関する事だとか、管理者権限を付与するものだとか、他のアセンブリとの接続情報を保持するものだとか、  
そういったメタデータの集まり的なファイルの模様。  
まぁ、でも意味合い的にはそれだけで十分な気がするけどね。  

[C#メモ Manifestファイルを追加してフォームの表示がぼやっとしているのをはっきりさせてみる](https://www.tetsuyanbo.net/tetsuyanblog/45990)  

どういう仕組みなのかは知らないが、マニフェストファイルを追加するだけで文字のぼやけが解消するらしい。  
ここら辺は必要になったらまた調べることに鳴るだろう。  
