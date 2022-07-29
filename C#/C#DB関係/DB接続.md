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

### 微妙に嵌った事

[System.Data.SqlClient] は単純にusingしただけでは使えない。  
nugetからパッケージをインストールする必要があった。  

[SqlConnectionを.NET Core アプリケーションで利用する](https://www.ipentec.com/document/csharp-using-sqlconnection-in-dot-net-core-application)  
>対処法1: NuGetを利用して、System.Data.SqlClient パッケージを参照する  

---

## SqlConnectionをUsingした場合、Disposeと同時にCloseされるのでFinallyで明示的にCloseする必要はない

SqlConnection using close  

[Microsoft公式_SqlConnection.Close メソッド](https://docs.microsoft.com/ja-jp/dotnet/api/system.data.sqlclient.sqlconnection.close?redirectedfrom=MSDN&view=netframework-4.7.2#System_Data_SqlClient_SqlConnection_Close)  

>次の例では、作成、 SqlConnection開き、そのプロパティの一部を表示します。  
>**接続はブロックの最後で自動的に using 閉じられます**。  

``` C#
private static void OpenSqlConnection(string connectionString)
{
    using (SqlConnection connection = new SqlConnection(connectionString))
    {
        connection.Open();
        Console.WriteLine("ServerVersion: {0}", connection.ServerVersion);
        Console.WriteLine("State: {0}", connection.State);
    }
}
```

[SqlConnectionとSqlDataReaderをusingで囲った場合Closeは必要？](https://social.msdn.microsoft.com/Forums/ja-JP/c2a0c8b2-7743-4cfa-869c-f26293b0250f/sqlconnection12392sqldatareader12434using123912225812387123832258021512close?forum=csharpgeneralja)  

>SqlConnection.Close()にはClose と Dispose は、機能的に同じです。  
>「DbDataReader.Dispose()にこのメソッドは Close を呼び出します。」と書かれています。  

[using文で初期化したDbConnection、Closeを書くべき？書かなくていい？](https://qiita.com/momotaro98/items/c4fe0fff0c173e879f2d)  
>using文は、try {} finally {XXX.Dispose();}をわざわざ書かなくてすむようにする糖衣構文ということです。  
→へぇ～そうだったんだ。  
>functionally equivalentとあるように、 DbConnectionクラスにとっては、機能的にはCloseもDisposeもどちらも変わらない ということです。  
>なので、using文ではDisposeメソッドを必ず呼ぶので、結果、Closeメソッドは基本的に不要ということです。  
>しかし、1つ異なる点があります。  
>Closeメソッドは再度そのインスタンスを再Openできるのに、対し、Disposeメソッドは一度実行されたら、そのインスタンスにはアクセスできない。  

### ではなぜCloseを書くのか

[Should I call Close() or Dispose() for stream objects?](https://stackoverflow.com/questions/7524903/should-i-call-close-or-dispose-for-stream-objects/7525134#7525134)  
>It doesn't affect the behaviour of the code, but it does aid readability.  
>つまり、 可読性のため。(既存のコードに影響を与えないし) とのこと。  
>複数のスコープがあるとどこでインスタンスがCloseされているのかがわかりにくくなるので、Closeを書くとよいということです。  

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
        AttachDBFilename = System.IO.Path.GetFullPath(@"..\..\..\SampleDatabase.mdf"),
        IntegratedSecurity = true,
    };
    SqlConnection connection = new SqlConnection(builder.ConnectionString);
```

``` C# : ローカルDBへの接続文字列の構築 直接指定パターン1
    //Data Source=(SQL Server のホスト名またはIPアドレス);Initial Catalog=(接続先データベース名);Connect Timeout=60;Persist Security Info=True;User ID=(SQL Serverに接続するユーザーID);Password=(ユーザーのパスワード)
    string constr = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\~~~~~\LocalDB\SampleDatabase.mdf;Integrated Security=True";
    SqlConnection con = new SqlConnection(constr);
```

``` C# : ローカルDBへの接続文字列の構築 直接指定パターン2
    // 相対パスで指定するのもあり
    string constr = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={System.IO.Path.GetFullPath(@"..\..\SampleDatabase.mdf")};Integrated Security=True";
    SqlConnection con = new SqlConnection(constr);
```

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
