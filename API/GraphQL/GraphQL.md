# GraphQL

## GraphQLとは

GraphQLは、Facebookが開発し、2015年にオープンソースとして公開されたデータクエリおよび操作言語です。  
APIを介してデータを取得・操作する際に利用されます。  
RESTful APIに代わる新しいアプローチとして広く普及しています。  

GraphQLの主な特徴は以下の通りです：  

1. **データ要求の柔軟性**: REST（REpresentational State Transfer）APIとは異なり、GraphQLは、クライアントが必要とするデータ構造を指定してリクエストできるため、過剰なデータや不足しているデータを受け取ることがありません。  
   クライアントが必要なデータのみを取得できるため、通信量の削減やパフォーマンスの向上が可能です。  
2. **型システム**: GraphQLは型システムを持っており、クエリの結果が事前に分かるため、データ検証やドキュメント作成が容易になります。  
3. **単一エンドポイント**: すべてのデータ操作が単一のエンドポイントで行われるため、APIのバージョン管理やエンドポイントの管理が容易になります。  

これらの特徴により、GraphQLは現代のウェブアプリケーションやモバイルアプリケーション開発で広く使われており、多くの開発者に支持されています。  

---

## RESTとの比較

``` mermaid
flowchart BT
    subgraph "REST API"
        direction TB
        REST_Client[Client]
        REST_Client-->Users
        REST_Client-->Posts
        REST_Client-->Comments
    end
    subgraph "GraphQL"
        direction TB
        QL_Client[Client]
        QL_Client-->Query
        Query-->User
        Query-->Post
        Query-->Comment
    end
```

---

## \.NET における代表的な GraphQL ライブラリ

### 1. GraphQL\.NET

- 概要: GraphQL\.NET は、.NET 環境で利用できる GraphQL サーバーおよびクライアントの実装を提供するライブラリです。  
- [リポジトリ: https://github.com/graphql-dotnet/graphql-dotnet](https://github.com/graphql-dotnet/graphql-dotnet)  

<!--  -->
- 主な特徴  
  - **柔軟性**: 機能豊富で柔軟性があり、多くのプロジェクトで使用されています。  
  - **LINQ サポート**: 独自の LINQ 式を使って GraphQL クエリを組み立てることができます。  
  - **型安全**: 型安全なクエリの組み立てをサポートしています。  
  - **データローディング**: データのバッチ処理やキャッシングをサポートする DataLoader を提供しています。  

### 2. HotChocolate

- 概要: Hot Chocolate は、.NET Core および .NET 5/6 で利用できる GraphQL サーバー実装を提供するライブラリです。  
- [リポジトリ: https://github.com/ChilliCream/hotchocolate](https://github.com/ChilliCream/hotchocolate)  

<!--  -->
- 主な特徴  
  - **現代的な設計**: 現代的なアプリケーション開発に適した設計がされており、ASP.NET Core との統合が容易です。  
  - **高度な機能**: GraphQL クエリの最適化やサブスクリプション、リアルタイム通信などの高度な機能をサポートしています。  
  - **コードファーストおよびスキーマファースト**: コードファーストとスキーマファーストの両方のアプローチをサポートしています。  
  - **フィルタリングやソート**: データのフィルタリングやソートを簡単に行うための機能を提供しています。  

---

## 参考

[GraphQL を使うと何が嬉しいのか？いろいろ触って検証してみた](https://sitest.jp/blog/?p=11001)  
[GraphQL入門](https://zenn.dev/yoshii0110/articles/2233e32d276551)  

[GraphQLを導入する時に考えておいたほうが良いこと | メルカリエンジニアリング](https://engineering.mercari.com/blog/entry/20220303-concerns-with-using-graphql/)  

---

## C#のGraphQL Clientの参考

内部クラスを駆使するやり方もある。  
[graphql-client/examples/GraphQL.Client.Example at master · graphql-dotnet/graphql-client · GitHub](https://github.com/graphql-dotnet/graphql-client/tree/master/examples/GraphQL.Client.Example)  

レスポンスがRESTであるが、基本的な構想は自分のイメージそのもの。  
一番参考にしたかもしれない。  
[Securing a graphQL API with Azure Active Directory :: my tech ramblings — A blog for writing about my techie ramblings](https://www.mytechramblings.com/posts/securing-dotnet-graphql-api-with-aad/)  
[GitHub - karlospn/securing-graphql-api-with-aad: A practical example about how you can secure and consume a graphQL api with Azure Active Directory](https://github.com/karlospn/securing-graphql-api-with-aad)  

これも基本的な部分を構築するなら充分参考になる。  
[GraphQl API Client Integration In ASP.NET Core 3.1](https://www.c-sharpcorner.com/blogs/graphql-api-client-integration-in-asp-net-core-31)  

Blazorバージョンではあるが、参考になった。  
[GitHub - CloudBloq/GraphQLSampleAppUI: A Blazor WebAssembly app that demostrates how to consume a GraphQL API in .NET](https://github.com/CloudBloq/GraphQLSampleAppUI)  

直接的なGraphQLの参考ではないが、Repositoryのインターフェースが具象クラスに依存していいのか迷ったときにやってるところがあったので拝借した。  
Entityならばプログラム中、全てにおいて使うので依存していいらしい。  
[GraphQL_HotChoclate_EFCore/GraphQL_HotChoclate_EFCore/Services at master · JayKrishnareddy/GraphQL_HotChoclate_EFCore · GitHub](https://github.com/JayKrishnareddy/GraphQL_HotChoclate_EFCore/tree/master/GraphQL_HotChoclate_EFCore/Services)  

打って変わってこちらでは`<T>`で定義している。  
MVVMのサンプルではあるが、参考になる。  
[Foreign keys in local databases with SQLite-net and .NET MAUI - DEV Community](https://dev.to/icebeam7/foreign-keys-in-local-databases-with-sqlite-net-and-net-maui-22a1)  
[GitHub - icebeam7/DemoFK](https://github.com/icebeam7/DemoFK)  

C#のGraphQLサンプル貴重なので  
[Building GraphQL API With .Net 5 — EF Core And Hot Chocolate | by Jay Krishna Reddy | Level Up Coding](https://levelup.gitconnected.com/building-graphql-api-with-net-5-ef-core-and-hot-chocolate-ad1d2482dd69)  
