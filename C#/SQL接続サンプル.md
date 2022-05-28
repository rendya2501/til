# VisualStudioで簡易データベース構築

---

## LocalDB

Visual Studioと一緒にインストールされる必要最低限の機能を備えたSQL Server。

---

## LocalDB 作成から接続までの最小サンプル

[MicroSoft公式](https://docs.microsoft.com/ja-jp/visualstudio/data-tools/create-a-sql-database-by-using-a-designer?view=vs-2022)  
[C#からDB接続でSQLServerに接続してSELECT文を実行する方法](https://rainbow-engine.com/csharp-dbconnection-sqlserver/)
[SQL Server LocalDB へ接続してSQLを実行する](https://www.ipentec.com/document/csharp-sql-server-localdb-connect-exec-sql)  
[SQL Serverに接続してSQLを実行する (C#プログラミング)](https://www.ipentec.com/document/csharp-sql-server-connect-exec-sql)  

1. コンソールアプリプロジェクトを作成する  
2. プロジェクト右クリック→新しい項目の追加→サービスベースのデータベース  
3. 以下のプログラムを貼り付け  
4. 作成したLocalDBを右クリックしてプロパティから接続文字列を取得して接続文字列部分に貼り付け  

※System.Data.SqlClientはnugetから取得しておくこと  

``` C#
using System.Data.SqlClient;

    try
    {
        // 接続文字列の構築
        string constr = @"LocalDBを右クリックしてプロパティから接続文字列を取得して貼り付け";
        // 接続オブジェクト生成
        using SqlConnection connection = new SqlConnection(constr);
        // サーバー接続
        connection.Open();
        // SQLコマンド生成
        StringBuilder query = new StringBuilder()
            .AppendLine("DROP TABLE IF EXISTS [Products];")
            .AppendLine("CREATE TABLE [Products] ( ")
            .AppendLine("    [Id]    INT        NOT NULL,")
            .AppendLine("    [name]  NCHAR (32) NULL, ")
            .AppendLine("    [price] INT        NULL ")
            .AppendLine("    PRIMARY KEY CLUSTERED ([Id] ASC)")
            .AppendLine("); ")
            .AppendLine("INSERT INTO [Products]")
            .AppendLine("VALUES ")
            .AppendLine("    (1,N'ペンギンクッキー',150), ")
            .AppendLine("    (2,N'シロクマアイス',200), ")
            .AppendLine("    (3,N'らくだケーキ',320), ")
            .AppendLine("    (4,N'くじらキャンディー',60), ")
            .AppendLine("    (5,N'ふくろうサブレ',120); ")
            .AppendLine("SELECT * FROM [Products];");
        using SqlCommand command = new SqlCommand(query.ToString(), connection);
        // SQL実行
        using SqlDataReader reader = command.ExecuteReader();
        // 結果表示
        while (reader.Read())
            Console.WriteLine($"{reader.GetString(1)},{reader.GetInt32(2)}");
            // Console.WriteLine($"{(string)sdr["name"]:s}:{(int)sdr["price"]:d}");
    }
    catch (SqlException Ex)
    {
        Console.WriteLine(Ex.ToString());
    }
```

メモ

``` sql
CREATE TABLE [dbo].[Products] (
   [Id]    INT        NOT NULL,
   [name]  NCHAR (32) NULL,
   [price] INT        NULL,
   PRIMARY KEY CLUSTERED ([Id] ASC)
);

INSERT INTO [Products] VALUES(1,N'ペンギンクッキー',150)
INSERT INTO [Products] VALUES(2,N'シロクマアイス',200)
INSERT INTO [Products] VALUES(3,N'らくだケーキ',320)
INSERT INTO [Products] VALUES(4,N'くじらキャンディー',60)
INSERT INTO [Products] VALUES(5,N'ふくろうサブレ',120)

select * from Products
```

---

## 微妙に嵌った事

System.Data.SqlClient は単純にusingしただけでは使えない。  
nugetからパッケージをインストールする必要があった。  

[SqlConnectionを.NET Core アプリケーションで利用する](https://www.ipentec.com/document/csharp-using-sqlconnection-in-dot-net-core-application)  
>対処法1: NuGetを利用して、System.Data.SqlClient パッケージを参照する  

---

## 一連のCRUDデモ

[Windows上でSQL Serverを使用してC#アプリを作成する](https://qiita.com/ymasaoka/items/944e8a5f1987cc9e0d37#c-%E3%82%A2%E3%83%97%E3%83%AA%E3%82%92-100-%E5%80%8D%E9%80%9F%E3%81%AB%E3%81%99%E3%82%8B)  

``` C#
private void Execute()
{
    try
    {
        Console.WriteLine("LocalDB に接続し、Create、Read、Update、Delete 操作のデモを行います。");
        Console.WriteLine("---");

        // 接続文字列の構築
        string constr = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\rendy\Desktop\CSharpSample1\CSharpSample1\LocalDB\SampleDatabase.mdf;Integrated Security=True";
        // 接続オブジェクト生成
        using SqlConnection connection = new SqlConnection(constr);

        // サーバー接続
        Console.WriteLine("SQL Server に接続しています... ");
        connection.Open();
        Console.WriteLine("接続成功");
        Console.WriteLine("---");

        // CreateDataBase();
        CreateTable();
        CreateDefaultData();
        Insert();
        Update();
        Delete();
        //DropTbale();

        Task.WaitAll(
            Task.Run(Read),
            Task.Run(() =>
            {
                string sqlstr = "select * from products";
                using SqlCommand com = new SqlCommand(sqlstr, connection);
                // SQL実行
                using SqlDataReader reader = com.ExecuteReader();
                // 結果表示
                while (reader.Read())
                {
                    Console.WriteLine($"{reader.GetString(1)},{reader.GetInt32(2)}");
                }
            }
            )
        );
    

        // データベースの作成
        void CreateDataBase()
        {
            Console.WriteLine("既に作成されている SampleDB データベースを削除し、再作成します... ");
            string sql = "DROP DATABASE IF EXISTS [SampleDB]; CREATE DATABASE [SampleDB]";
            // コマンドがタイムアウトする場合は秒数を変更(ms) デフォルトは 30秒
            using SqlCommand command = new SqlCommand(sql, connection) { CommandTimeout = 60000 };
            command.ExecuteNonQuery();
            Console.WriteLine("SampleDB データベースを作成しました。");
            Console.WriteLine("---");
        }

        // テーブルの作成
        void CreateTable()
        {
            Console.WriteLine("テーブルを作成");
            StringBuilder query = new StringBuilder()
                    //.AppendLine("USE SampleDB; ")
                .AppendLine("DROP TABLE IF EXISTS [Employees];")
                .AppendLine("CREATE TABLE [Employees] ( ")
                .AppendLine("    Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY, ")
                .AppendLine("    Name NVARCHAR(50), ")
                .AppendLine("    Location NVARCHAR(50) ")
                .AppendLine("); ");
            using (SqlCommand command = new SqlCommand(query.ToString(), connection) { CommandTimeout = 60000 })
            {
                command.ExecuteNonQuery();
                Console.WriteLine("テーブル作成完了");
                Console.WriteLine();
            }
            Read();
        }

        // サンプルデータの登録
        void CreateDefaultData()
        {
            Console.WriteLine("デフォルトデータを作成します。");
            StringBuilder query = new StringBuilder()
                .AppendLine("INSERT INTO Employees (Name, Location)")
                .AppendLine("VALUES ")
                .AppendLine("    (N'Jared', N'Australia'), ")
                .AppendLine("    (N'Nikita', N'India'), ")
                .AppendLine("    (N'Tom', N'Germany'); ");
            using (SqlCommand command = new SqlCommand(query.ToString(), connection) { CommandTimeout = 60000 })
            {
                command.ExecuteNonQuery();
                Console.WriteLine("作成完了");
                Console.WriteLine();
            }
            Read();
        }

        // INSERT デモ
        void Insert()
        {
            Console.WriteLine("テーブルに新しい行を挿入するには、任意のキーを押して続行します...");
            StringBuilder query = new StringBuilder()
                .AppendLine("INSERT Employees (Name, Location) ")
                .AppendLine("VALUES (@name, @location);");
            using (SqlCommand command = new SqlCommand(query.ToString(), connection) { CommandTimeout = 60000 })
            {
                command.Parameters.AddWithValue("@name", "Jake");
                command.Parameters.AddWithValue("@location", "United States");
                int rowsAffected = command.ExecuteNonQuery();
                Console.WriteLine(rowsAffected + " 行 挿入されました。");
                Console.WriteLine();
            }
            Read();
        }

        // UPDATE デモ
        void Update()
        {
            string userToUpdate = "Nikita";
            Console.WriteLine("ユーザー名 '" + userToUpdate + "' の 'Location' を更新中です。任意のキーを押して処理を続行します...");
            StringBuilder query = new StringBuilder()
                .AppendLine("UPDATE Employees")
                .AppendLine("SET Location = N'United States'")
                .AppendLine("WHERE Name = @name");
            using (SqlCommand command = new SqlCommand(query.ToString(), connection) { CommandTimeout = 60000 })
            {
                command.Parameters.AddWithValue("@name", userToUpdate);
                int rowsAffected = command.ExecuteNonQuery();
                Console.WriteLine(rowsAffected + " 行 更新されました。");
                Console.WriteLine();
            }
            Read();
        }

        // DELETE デモ
        void Delete()
        {
            string userToDelete = "Jared";
            Console.WriteLine("ユーザー名 '" + userToDelete + "' を削除中です。任意のキーを押して処理を続行します...");
            StringBuilder query = new StringBuilder()
                .AppendLine("DELETE FROM Employees WHERE Name = @name;");
            using (SqlCommand command = new SqlCommand(query.ToString(), connection) { CommandTimeout = 60000 })
            {
                command.Parameters.AddWithValue("@name", userToDelete);
                int rowsAffected = command.ExecuteNonQuery();
                Console.WriteLine(rowsAffected + " 行 削除されました。");
                Console.WriteLine();
            }
            Read();
        }

        // Select
        void Read()
        {
            StringBuilder query = new StringBuilder()
                .AppendLine("SELECT Id, Name, Location FROM Employees;");
            using SqlCommand command = new SqlCommand(query.ToString(), connection);
            using SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine($"{reader.GetInt32(0)} {reader.GetString(1)} {reader.GetString(2)}");
                //_TextBox1.Text += $"{reader.GetInt32(0)} {reader.GetString(1)} {reader.GetString(2)}";
            }
            Console.WriteLine("---");
        }

        // テーブル削除
        void DropTbale()
        {
            Console.WriteLine("SampleDB データベースを削除します。");
            string sql = "DROP TABLE [Employees];";
            //string sql = "DROP DATABASE [SampleDB];";
            using SqlCommand command = new SqlCommand(sql, connection) { CommandTimeout = 60000 };
            command.ExecuteNonQuery();
            Console.WriteLine("削除完了");
            Console.WriteLine("---");
        }
    }
    catch (SqlException e)
    {
        Console.WriteLine(e.ToString());
    }

    Console.WriteLine("全て完了しました");
}
```

---

## 接続文字列構築パターン

``` C# : SQL Serverへの接続文字列の構築
    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder
    {
        // 接続先の SQL Server インスタンス
        DataSource = "localhost",
        // 接続ユーザー名
        UserID = "sa",
        // 接続パスワード
        Password = "your_password",
        // 接続するデータベース
        InitialCatalog = "master",
        // 接続タイムアウトの秒数(ms) デフォルトは 15 秒
        ConnectTimeout = 60000
    };
    SqlConnection connection = new SqlConnection(builder.ConnectionString);
```

``` C# : ローカルDBへの接続文字列の構築 SqlConnectionStringBuilderパターン
    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder
    {
        DataSource = @"(LocalDB)\MSSQLLocalDB",
        AttachDBFilename = @"C:\Users\~~~~~\LocalDB\SampleDatabase.mdf",
        IntegratedSecurity = true,
    };
    SqlConnection connection = new SqlConnection(builder.ConnectionString);
```

``` C# : ローカルDBへの接続文字列の構築 直接指定パターン
    //Data Source=(SQL Server のホスト名またはIPアドレス);Initial Catalog=(接続先データベース名);Connect Timeout=60;Persist Security Info=True;User ID=(SQL Serverに接続するユーザーID);Password=(ユーザーのパスワード)
    string constr = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\~~~~~\LocalDB\SampleDatabase.mdf;Integrated Security=True";
    SqlConnection con = new SqlConnection(constr);
```
