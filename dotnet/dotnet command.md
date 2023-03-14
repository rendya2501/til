# dotnet command

---

## C#プロジェクト事始め

`dotnet new console`

---

## ignoreファイル作成

`dotnet new gitignore`

---

## ソリューション

`dotnet new sln`  
デフォルトではフォルダ名がソリューション名となる。  

例 : [\<SolutionName>]でフォルダを作りつつソリューションを作成する  
`dotnet new -o <SolutionName>`  

---

## パッケージ

`dotnet add package <PackageName>`  

バージョンを指定しない場合、常に最新をインストールする。  
バージョンを指定した場合、addコマンドであるが、指定したバージョンになってくれるので、実質的にアップデートもダウングレードも兼ねる。  
全てはバージョン指定次第。  

バージョン指定例:  
`dotnet add package Microsoft.EntityFrameworkCore -v 6.*`  

主にnugetサイトでパッケージ名を検索して使うことになるので、コマンドに迷く事はないと思われる。  
nugetサイトを見ればわかるが、提示されているコマンドは`dotnet add package`である。  

`dotnet nuget`コマンドもあるらしいが、これを解説しているサイトが全然ないので、特にこだわりがなければnugetサイトでは``をメインに紹介しているので、素直にそちらでよろしいかと思われる。  

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

### 2022/11/23 Wed 追記

`dotnet new console --help`で確認してみたら`--use-program-main`というトップレベルステートメントを無効化するオプションが追加されていたので、それを使えばよい。  

因みにこの状態から`Console.WriteLine("Hello, World!");`以外を全て削除しても動くので、そこのところもちゃんと考えられているっぽい。  

---

## dotnet watch run

`dotnet run` のリロードに相当する機能は `dotnet watch run` で行える。  

.net6からの機能。  

[[.NET 6 新機能] dotnet watch によるホットリロード](https://watermargin.net/programming/net-6-dotnet-watch-hot-reload/)  

---

## カレントディレクトリにフォルダ名とは別にプロジェクトを作成する

- -o でカレントディレクトリ(`./`)を指定  
- -n でプロジェクト名を指定  

例 : 現在の場所にtestという名前でコンソールプロジェクトを作成するコマンド  
`dotnet new console -o ./ -n test`  

---

## dotnetコマンドによるリビルド

リビルドには `--no-incremental` を指定する。  

`dotnet build --no-incremental`  

[dotnet コマンドによるビルド | Do Design Space](https://sakapon.wordpress.com/2018/08/01/dotnet-build/)  

---

>庭に小さな穴を掘るのにショベルカーを使うような感じである。  
[dotnetコマンドを使って、Visual Studioを起動せずに簡単にプログラムを作成する](https://ascii.jp/elem/000/004/082/4082000/)  
