# DapperQuiqStart

---

## 実行環境

- Windows10以上  
- VSCode  
- .Net6  
- SQLServer  
- Dapper v2.0.123  
- System.Data.SqlClient 4.8.5

SQLServerインストール済みであること。  
ローカルの環境に適当なデータベースとデータがあること。  

---

## プロジェクトの用意

- プロジェクト作成  
  - `dotnet new console -o DapperQuiqStart -f net6.0`  

- VSCodeを起動  
  - `Code DapperQuiqStart`  

- `Ctrl + J` でターミナルを起動  

- NuGetパッケージのインストール  
  - `dotnet add package Dapper --version 2.0.123`  
  - `dotnet add package System.Data.SqlClient --version 4.8.5`  

---

## コーディング

program.csに以下のコードをコピペ。  

`Database=<db_name>`や`FROM <table>`はあらかじめ作成していたものを指定する。  
`item.<field_name>`は表示させたフィールド名を指定すればよろしい。  
resultの時点でデバッグで止めていれば、値が格納されていることがわかるはず。  

``` cs : program.cs
using Dapper;
using System.Data.SqlClient;

string constr = @"Server=.\SQLEXPRESS;Database=<db_name>;Integrated Security=True;";
using SqlConnection connection = new SqlConnection(constr);
var query = "SELECT * FROM <table>";
var result = connection.Query<dynamic>(query);

foreach (var item in result)
    Console.WriteLine(item.<field_name>);
```
