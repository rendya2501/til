# SQLite

## 概要

SQLiteは、軽量で高速なオープンソースの関係データベース管理システム (RDBMS) です。  
SQLiteは、アプリケーションに組み込まれたデータベースエンジンであり、サーバーに依存しないため、独立したアプリケーションに適しています。  
主な特徴は以下の通りです。  

1. サーバーレス: SQLiteは、サーバーを必要とせず、他のデータベースシステムと違って独立して動作します。アプリケーションと同じプロセスで動作し、直接ディスクファイルへの読み書きを行います。  
2. セルフコンテインド: SQLiteは外部の依存関係がほとんどなく、アプリケーションに組み込むことが容易です。  
3. トランザクションサポート: SQLiteは、ACID (原子性、一貫性、分離性、持続性) 特性を持つトランザクションをサポートしています。これにより、データベースの整合性と信頼性が向上します。  
4. クロスプラットフォーム: SQLiteは、Windows、macOS、Linuxなど、さまざまなオペレーティングシステムで動作します。  
5. 単一ファイル: SQLiteデータベースは、単一のディスクファイルに格納されます。これにより、データベースのバックアップや移行が容易になります。  
6. SQLサポート: SQLiteは、標準的なSQL言語をサポートしており、データの操作やクエリの実行が可能です。ただし、一部の高度な機能やストアドプロシージャはサポートされていません。  
7. リソース効率: SQLiteは、低メモリ使用量と小さなディスクスペース要件で知られており、埋め込みシステムやモバイルアプリケーションに適しています。  

SQLiteは、単純なデータ管理が必要なアプリケーションや、サーバーレスアーキテクチャを採用したプロジェクトに特に適しています。  
しかし、複数のクライアントからの同時書き込みや、大規模なデータベースアプリケーションには、よりスケーラブルなデータベースシステム（MySQLやPostgreSQLなど）が適していることがあります。  

---

## Microsoft.Data.Sqlite

Microsoft.Data.Sqliteは、.NET Coreや.NET StandardアプリケーションでSQLiteデータベースを使用するためのマイクロソフトが提供するライブラリです。  
これにより、.NET開発者は、アプリケーションにSQLiteデータベースを簡単に統合し、データの読み書きや操作を行うことができます。  

Microsoft.Data.Sqliteは、以下の主な機能を提供しています。  

1. データベース接続: SQLiteデータベースに接続し、データベースファイルを作成するためのAPIを提供しています。  
2. SQLコマンドの実行: SQLクエリやコマンドをデータベースに対して実行するための簡単なインターフェースを提供します。  
3. パラメータ化クエリ: SQLインジェクション攻撃を防ぐため、パラメータ化されたSQLクエリをサポートしています。  
4. トランザクション管理: ACID特性を持つトランザクションをサポートし、データベースの整合性と信頼性を向上させます。  
5. 便利なデータ型マッピング: .NETのデータ型とSQLiteのデータ型を自動的にマッピングし、データの読み書きを容易にします。  

Microsoft.Data.Sqliteは、.NET Coreや.NET Standardアプリケーションにおいて、組み込みデータベースとしてSQLiteを使う場合に、開発者がデータベース操作を簡単かつ効率的に行えるように設計されています。  
これにより、デスクトップアプリケーション、モバイルアプリケーション、IoTデバイスなど、様々なプラットフォームでのSQLiteの使用が容易になります。  

---

## インメモリデータベース

`Data Source=:memory:`という接続文字列を使用すると、SQLiteデータベースがインメモリデータベースとして作成されます。  
インメモリデータベースは、データベースがアプリケーションのメモリ内に存在し、高速なデータ操作が可能ですが、一時的なデータ保持のみが可能であり、アプリケーションが終了するとデータベースは破棄されます。  

---

## その他のモードとオプション

1. **Temporaryファイル**: 一時的なデータベースを作成することができます。これは、ディスク上にデータベースファイルを作成するものの、アプリケーションが終了するとファイルが削除されます。接続文字列でData Source= に空文字列を指定することで作成できます。  

2. **読み取り専用モード**: データベースを読み取り専用モードで開くことができます。これにより、データベースの内容を変更できないようになります。接続文字列にRead Only=Trueを追加することで有効にできます。  

3. **URIモード**: SQLiteデータベースに対して、特定の構成オプションを使用して接続するためにURI形式の接続文字列を使用することができます。例えば、ファイルの同期モードを無効にしたい場合、接続文字列をData Source=file:myDatabase.db?mode=rwc&cache=shared&journal_mode=WAL&synchronous=OFFのように設定することができます。  

それぞれのモードやオプションは、アプリケーションの要件に応じて選択・適用してください。  
ただし、一般的なアプリケーションでは、ディスクベース（通常モード）またはインメモリモードが主に使用されます。  

---

## サンプルプログラム

### 仕様

データベースがなければ作成。
最初に一覧を表示する。

qを押すまで処理を続ける。  

- 1:insert  
- 2:update  
- 3:delete  
- q:exit  

insertはnameを入力。  
updateはidを入力。その後名前を入力する。  
deleteはidを入力。  

### 開発環境

- Windows10以降  
- .Net6  
- コンソールアプリ  
- SQLite  
- Dapper  

### パッケージのインストール

nugetからパッケージを取得  
`dotnet add package Microsoft.Data.Sqlite`  
`dotnet add package Dapper`  

### 実装

Program.csを以下のように実装する。  

``` cs
using Microsoft.Data.Sqlite;
using Dapper;


// データベースに接続
var connectionString = "Data Source=myDatabase.db";
using var connection = new SqliteConnection(connectionString);
connection.Open();

// テーブルの作成
var createTableCommand = @"CREATE TABLE IF NOT EXISTS users (id INTEGER PRIMARY KEY, name TEXT NOT NULL);";
connection.Execute(createTableCommand);

string userInput;
do
{
    DisplayUsers(connection);

    Console.WriteLine("\nChoose an option:");
    Console.WriteLine("1. Insert");
    Console.WriteLine("2. Update");
    Console.WriteLine("3. Delete");
    Console.WriteLine("q. Exit");
    Console.Write("Enter your choice: ");
    userInput = Console.ReadLine();

    switch (userInput)
    {
        case "1":
            InsertUser(connection);
            break;
        case "2":
            UpdateUser(connection);
            break;
        case "3":
            DeleteUser(connection);
            break;
    }
    Console.WriteLine("");
} while (userInput != "q");

connection.Close();


static void DisplayUsers(SqliteConnection connection)
{
    Console.WriteLine("Current users in the database:");
    var selectCommand = "SELECT id, name FROM users";
    var users = connection.Query<User>(selectCommand).ToList();

    foreach (var user in users)
    {
        Console.WriteLine($"ID: {user.Id}, Name: {user.Name}");
    }
}

static void InsertUser(SqliteConnection connection)
{
    Console.Write("Enter a name to add: ");
    var name = Console.ReadLine();

    var insertCommand = "INSERT INTO users (name) VALUES (@Name)";
    connection.Execute(insertCommand, new { Name = name });
    Console.WriteLine($"Added {name} to the database.");
}

static void UpdateUser(SqliteConnection connection)
{
    Console.Write("Enter the ID of the user to update: ");
    var idInput = Console.ReadLine();
    if (int.TryParse(idInput, out int id))
    {
        Console.Write("Enter the new name for the user: ");
        var newName = Console.ReadLine();

        var updateCommand = "UPDATE users SET name = @Name WHERE id = @Id";
        connection.Execute(updateCommand, new { Name = newName, Id = id });
        Console.WriteLine($"Updated user with ID {id} to {newName}.");
    }
    else
    {
        Console.WriteLine("Invalid ID.");
    }
}

static void DeleteUser(SqliteConnection connection)
{
    Console.Write("Enter the ID of the user to delete: ");
    var idInput = Console.ReadLine();
    if (int.TryParse(idInput, out int id))
    {
        var deleteCommand = "DELETE FROM users WHERE id = @Id";
        connection.Execute(deleteCommand, new { Id = id });
        Console.WriteLine($"Deleted user with ID {id}.");
    }
    else
    {
        Console.WriteLine("Invalid ID.");
    }
}

public record User
{
    public int Id { get; init; }
    public string Name { get; init; }
    public User(long id, string name)
    {
        Id = (int)id;
        Name = name;
    }
}
```
