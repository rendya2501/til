# GraphQL サンプルプログラム

## 開発環境

- Windows11
- .NET 6
- サーバー側
  - ASP.NET Core Minimal API
- クライアント側
  - コンソールアプリ

---

## 仕様

プログラム起動直後は一覧を表示する。  
一覧では「ID,Title,Author」を表示するものとする。  

キー入力による選択肢は以下の通り。  

- 1:Get  
- 2:Insert  
- 3:Update  
- 4:Delete  
- q:exit  

getはIDを入力するとその書籍に関する詳しい結果が表示される。  
insertは「Title,Author,Publisher,publicationDate」を入力する。  
updateはidを入力し、名前だけを変更できるようにする。  
deleteはidを入力する。  
qはプログラムを終了する。  

各選択肢を実行した後、再び一覧を表示する。  
この処理はwhileループとする。  
qを押下することでbreakする。  

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
