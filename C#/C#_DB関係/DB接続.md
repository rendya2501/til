# VisualStudioで簡易データベース構築

---

## LocalDB 作成から接続までの最小サンプル

1. コンソールアプリプロジェクトを作成する  
2. プロジェクト右クリック→新しい項目の追加→サービスベースのデータベース  
3. [System.Data.SqlClient]はnugetから取得  
4. 以下のプログラムを貼り付け  
5. 作成したLocalDBを右クリックしてプロパティから接続文字列を取得して接続文字列部分に貼り付け  

``` C#
using System.Data.SqlClient;

    try
    {
        // 接続文字列
        string constr = @"LocalDBを右クリックしてプロパティから接続文字列を取得して貼り付け";
        // 接続オブジェクト生成
        using SqlConnection connection = new SqlConnection(constr);
        // データベース接続
        connection.Open();
        // クエリ生成
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
        // SQLコマンド生成
        using SqlCommand command = new SqlCommand(query.ToString(), connection);
        // SQL実行
        using SqlDataReader reader = command.ExecuteReader();
        // 結果表示
        while (reader.Read()){
            Console.WriteLine($"{reader["name"]}:{reader["price"]}");
            // 律儀にやるならこう
            // Console.WriteLine($"{(string)reader["name"]:s}:{(int)reader["price"]:d}");
            // DataReaderには型で取得するメソッドがあるのでそれでも良い
            // Console.WriteLine($"{reader.GetString(1)},{reader.GetInt32(2)}");
        }
    }
    catch (SqlException Ex)
    {
        Console.WriteLine(Ex.ToString());
    }
```

[MicroSoft公式](https://docs.microsoft.com/ja-jp/visualstudio/data-tools/create-a-sql-database-by-using-a-designer?view=vs-2022)  
[C#からDB接続でSQLServerに接続してSELECT文を実行する方法](https://rainbow-engine.com/csharp-dbconnection-sqlserver/)
[SQL Server LocalDB へ接続してSQLを実行する](https://www.ipentec.com/document/csharp-sql-server-localdb-connect-exec-sql)  
[SQL Serverに接続してSQLを実行する (C#プログラミング)](https://www.ipentec.com/document/csharp-sql-server-connect-exec-sql)  

### 微妙に嵌った事

[System.Data.SqlClient] は単純にusingしただけでは使えない。  
nugetからパッケージをインストールする必要があった。  

[SqlConnectionを.NET Core アプリケーションで利用する](https://www.ipentec.com/document/csharp-using-sqlconnection-in-dot-net-core-application)  
>対処法1: NuGetを利用して、System.Data.SqlClient パッケージを参照する  

---

## LocalDB

Visual Studioと一緒にインストールされる必要最低限の機能を備えたSQL Server。

---

## 1つのコネクションの中で並列して処理を実行することはできるか？

A.できない。  

デッドロックではなさそうだけど、明らかに処理が止まり、しばらくしたらタイムアウトエラーになる。  
それぞれコネクションをopenして並列して実行する分には問題ないことは確認した。  

``` txt : エラー内容
内部例外 1:
Win32Exception: 待ち操作がタイムアウトになりました。
```

``` C# : NG①
    using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
    {
        // データベース接続
        connection.Open();
        // テーブルの生成と初期データを準備
        CreateTable(connection);
        Insert(connection);
        // 1つのコネクションで並列実行→エラー
        Task.WaitAll(
           Task.Run(() => Read1(connection)),
           Task.Run(() => Read2(connection))
        );
    }
```

``` C# : NG②
    using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
    {
        // データベース接続
        connection.Open();
        // テーブルの生成と初期データを準備
        CreateTable(connection);
        Insert(connection);
    }
    // ②コネクションを張りなおすが、相変わらず1つのコネクションを使って並列実行 → エラー
    using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
    {
       connection.Open();
       Task.WaitAll(
           Task.Run(() => Read1(connection)),
           Task.Run(() => Read2(connection))
       );
    }
```

``` C# : OK
    // テーブルの生成と初期データを準備済みの体

    // それぞれでコネクションを張って並列実行 → OK
    Task.WaitAll(
        Task.Run(() =>
        {
            using (SqlConnection connection = new SqlConnection(builder.ConnectionString)){
                connection.Open();
                Read1(connection);
            }
        }),
        Task.Run(() =>
        {
            using (SqlConnection connection = new SqlConnection(builder.ConnectionString)){
                connection.Open();
                Read2(connection);
            }
        })
    );
```
