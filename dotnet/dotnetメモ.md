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
