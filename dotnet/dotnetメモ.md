# dotnetメモ

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

## dotnet cli
