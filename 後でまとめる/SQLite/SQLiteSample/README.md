# サンプルプログラム

## 仕様

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

---

## 開発環境

- Windows10以降  
- .Net6  
- コンソールアプリ  
- SQLite  
- Dapper  

---

## パッケージのインストール

nugetからパッケージを取得  
`dotnet add package Microsoft.Data.Sqlite`  
`dotnet add package Dapper`  

---

## 実装

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
