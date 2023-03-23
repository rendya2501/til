# GraphQL サンプルプログラム

## 開発環境

- Windows11
- .NET 6
- サーバー側
  - ASP.NET Core Minimal API
- クライアント側
  - コンソールアプリ

---

## サーバー側（ASP.NET Core Minimal API）

依存関係の追加

HotChocolate.AspNetCore を使用するため、NuGet からパッケージをインストールします。  

- `dotnet add package HotChocolate.AspNetCore`  

---

## クライアント側（ コンソールアプリケーション）

依存関係の追加  

GraphQL.Client.Http と GraphQL.Client.Serializer.SystemTextJson を使用するため、NuGet からパッケージをインストールします。  

- `dotnet add package GraphQL.Client.Http`  
- `dotnet add package GraphQL.Client.Serializer.SystemTextJson`  
