# dotnet command

---

## C#プロジェクト事始め

`dotnet new console`

---

## ignoreファイル作成

`dotnet new gitignore`

---

## パッケージ

`dotnet add package Unity.Configuration`
`dotnet nuget --help`

nugetサイトで検索した結果を使うべし。  
`dotnet nuget`コマンドもあるが、これを解説しているサイトがない。  
nugetサイトでは`dotnet add package`をメインに紹介しているので、素直にそちらでよろしいかと思われる。  

---

## トップレベルステートメントの無効化

.NET 5 でプロジェクトを作成し、手動で .NET 6 に更新する。  
それ以外は現状、無理な模様。  

`$ dotnet new console --framework net5.0`  

作成された .csproj を開き、TargetFramework を`net5.0`から`net6.0`に変更。  
必要に応じて Implicit using directives (暗黙的な using ディレクティブ) や Nullable (Null許容) を有効にすれば完成。  

``` xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net6.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  
</Project>
```

[git_dotnet](https://github.com/dotnet/docs/blob/main/docs/core/tutorials/top-level-templates.md)  
[開発メモ その286 最上位ステートメント (top-level statements) を無効にする](https://taktak.jp/2022/07/09/4445/)  

---

>庭に小さな穴を掘るのにショベルカーを使うような感じである。  
[dotnetコマンドを使って、Visual Studioを起動せずに簡単にプログラムを作成する](https://ascii.jp/elem/000/004/082/4082000/)  
