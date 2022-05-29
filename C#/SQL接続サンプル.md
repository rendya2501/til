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

SqlConnectionをUsingした場合、Disposeと同時にCloseされるのでFinallyで明示的にCloseする必要はない。  
[SqlConnection.Close メソッド](https://docs.microsoft.com/ja-jp/dotnet/api/system.data.sqlclient.sqlconnection.close?redirectedfrom=MSDN&view=netframework-4.7.2#System_Data_SqlClient_SqlConnection_Close)  

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

---

## 実務

``` C#
        /// <summary>
        /// 枠No更新
        /// </summary>
        private void UpdateFrameNo(string frameNo1,
            string frameNo2,
            DateTime businessDate1,
            DateTime businessDate2,
            string staffCD,
            string staffName,
            string program,
            string terminal)
        {
            DateTime now = DateTime.Now;
            StringBuilder query = new StringBuilder()
                .AppendLine("UPDATE [TRe_ReservationPlayer]")
                .AppendLine("SET")
                .AppendLine("[ReservationFrameNo] = CASE [ReservationFrameNo] WHEN @frameNo1 THEN @frameNo2")
                .AppendLine("                                                 WHEN @frameNo2 THEN @frameNo1")
                .AppendLine("                                                 END,")
                .AppendLine("[BusinessDate] = CASE [ReservationFrameNo] WHEN @frameNo1 THEN @businessDate2")
                .AppendLine("                                           WHEN @frameNo2 THEN @businessDate1")
                .AppendLine("                                           END,")
                .AppendLine("[UpdateDateTime] = @now,")
                .AppendLine("[UpdateStaffCD] = @staffCD,")
                .AppendLine("[UpdateStaffName] = @staffName,")
                .AppendLine("[UpdateProgram] = @program,")
                .AppendLine("[UpdateTerminal] = @terminal")
                .AppendLine("WHERE [ReservationFrameNo] IN (@frameNo1, @frameNo2)");
            _DapperAction.ExecuteQuery(
                ConnectionTypes.Data,
                query.ToString(),
                new
                {
                    frameNo1,
                    frameNo2,
                    businessDate1,
                    businessDate2,
                    now,
                    staffCD,
                    staffName,
                    program,
                    terminal
                }
            );
        }
```

``` C#
using Dapper;
using RN3.Web.Common.Data.Enum;
using System.Collections.Generic;
using System.Linq;

namespace RN3.Web.Common.Connection
{
    /// <summary>
    /// Dapperの機能を纏めたラッパークラスです
    /// </summary>
    public class DapperAction
    {
        private ConnectionManager _ConnectionManager;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="connectionManager"></param>
        public DapperAction(ConnectionManager connectionManager)
        {
            _ConnectionManager = connectionManager;
        }

        /// <summary>
        /// クエリからデータを一覧で取得します
        /// </summary>
        /// <typeparam name="T">戻り値の型</typeparam>
        /// <param name="connectionType">コネクション区分（接続先）</param>
        /// <param name="query">クエリ</param>
        /// <param name="param">パラメータ</param>
        /// <returns>取得結果のデータ一覧</returns>
        public IEnumerable<T> GetDataListByQuery<T>(ConnectionTypes connectionType, string query, object param = null)
        {
            var connection = _ConnectionManager.Connect(connectionType);
            if (connection == null)
            {
                // コネクションが確立できない場合はnullを返す
                return null;
            }
            if (param != null)
            {
                return connection.Query<T>(query, param).ToList();
            }
            else
            {
                //TODO: BIツールの分析データ取得でタイムアウトとなるため、一時的に300秒に設定
                return connection.Query<T>(query, commandTimeout: 300).ToList();
            }
        }

        /// <summary>
        /// クエリからデータを1件取得します
        /// </summary>
        /// <typeparam name="T">戻り値の型</typeparam>
        /// <param name="connectionType">コネクション区分（接続先）</param>
        /// <param name="query">クエリ</param>
        /// <param name="param">パラメータ</param>
        /// <returns>取得結果のデータ1件</returns>
        /// <remarks>
        /// ※取得結果の先頭を返すので「Top 1」をクエリに含めるようにして下さい。
        /// </remarks>
        public T GetFirstDataByQuery<T>(ConnectionTypes connectionType, string query, object param = null)
        {
            var connection = _ConnectionManager.Connect(connectionType);
            if (connection == null)
            {
                // コネクションが確立できない場合は規定値を返す
                return default;
            }
            if (param != null)
            {
                return connection.QueryFirstOrDefault<T>(query, param);
            }
            else
            {
                return connection.QueryFirstOrDefault<T>(query);
            }
        }

        /// <summary>
        /// クエリを実行します
        /// </summary>
        /// <param name="connectionType">コネクション区分（接続先）</param>
        /// <param name="query">クエリ</param>
        /// <param name="param">パラメータ</param>
        public void ExecuteQuery(ConnectionTypes connectionType, string query, object param = null)
        {
            var connection = _ConnectionManager.Connect(connectionType);
            if (connection == null)
            {
                // コネクションが確立できない場合は処理を中断する
                return;
            }
            if (param != null)
            {
                connection.Execute(query, param);
            }
            else
            {
                connection.Execute(query);
            }
        }
    }
}
```

``` C#
using RN3.Web.Common.Data.Enum;
using RN3.Web.Common.Store.Config;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace RN3.Web.Common.Connection
{
    /// <summary>
    /// SqlConnectionとDapperActionを内包したクラスです
    /// </summary>
    /// <remarks>
    /// インスタンスをNewするとコネクションを接続し、DisposeでコネクションのDisposeを呼び出します
    /// DapperActionは自身のコネクションに対してDapperActionの機能を呼び出す機能を実装します
    /// </remarks>
    public class ConnectionManager : IDisposable
    {
        /// <summary>
        /// コネクション接続イベント
        /// </summary>
        public event EventHandler Connecting;

        /// <summary>
        /// 共有コネクションディクショナリ
        /// </summary>
        /// <remarks>
        /// 接続先ごとに保持しているため同じ接続先でコネクションを分けることはできません。
        /// </remarks>
        private IDictionary<ConnectionTypes, SqlConnection> _SharedConnectionDictionary;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ConnectionManager()
        {
            // トランザクションスコープ外の共有コネクションを接続します。
            _SharedConnectionDictionary = new Dictionary<ConnectionTypes, SqlConnection>();

            var dataConnection = new SqlConnection(Configurations.ConnectionContainer.DataConnection.ConnectionString);
            dataConnection.Open();
            _SharedConnectionDictionary.Add(
                ConnectionTypes.DataWithOutTransactionScope,
                dataConnection);

            var systemConnection = new SqlConnection(Configurations.ConnectionContainer.SystemConnection.ConnectionString);
            systemConnection.Open();
            _SharedConnectionDictionary.Add(
                ConnectionTypes.SystemWithOutTransactionScope,
                systemConnection);
        }

        /// <summary>
        /// リソースを解放します
        /// </summary>
        /// <remarks>
        /// コネクションのリソースを解放するために実装
        /// </remarks>
        public void Dispose()
        {
            foreach (var pair in _SharedConnectionDictionary)
            {
                pair.Value.Dispose();
            }
            _SharedConnectionDictionary = null;
        }

        /// <summary>
        /// 指定した接続先に対するコネクションがないのなら接続を確立し
        /// コネクションインスタンスをディクショナリに保持します
        /// </summary>
        /// <param name="connectionType">コネクション区分（接続先）</param>
        /// <returns>指定した接続先のコネクション</returns>
        public SqlConnection Connect(ConnectionTypes connectionType)
        {
            if (_SharedConnectionDictionary == null)
            {
                _SharedConnectionDictionary = new Dictionary<ConnectionTypes, SqlConnection>();
            }
            var connnectionContainer = _SharedConnectionDictionary.FirstOrDefault(f => f.Key == connectionType);
            if (!connnectionContainer.Equals(default(KeyValuePair<ConnectionTypes, SqlConnection>)))
            {
                // すでに接続が確立しているのなら処理をしない
                return connnectionContainer.Value;
            }

            SqlConnection connection = null;
            // コネクション区分から接続先DBへのコネクションを作成
            switch (connectionType)
            {
                case ConnectionTypes.Data:
                    connection = new SqlConnection(Configurations.ConnectionContainer.DataConnection.ConnectionString);
                    break;
                case ConnectionTypes.System:
                    connection = new SqlConnection(Configurations.ConnectionContainer.SystemConnection.ConnectionString);
                    break;
                default:
                    break;
            }
            if (connection != null)
            {
                // コネクション接続
                RaiseConnecting();
                connection.Open();
                _SharedConnectionDictionary.Add(connectionType, connection);
                return connection;
            }
            return null;
        }

        /// <summary>
        /// コネクション接続イベントを発火します
        /// </summary>
        private void RaiseConnecting()
        {
            Connecting?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// 保持しているコネクションを破棄します
        /// </summary>
        public void DisConnect()
        {
            if (_SharedConnectionDictionary == null)
            {
                return;
            }
            foreach (var connnectionContainer in _SharedConnectionDictionary)
            {
                if (connnectionContainer.Key == ConnectionTypes.Data
                    || connnectionContainer.Key == ConnectionTypes.System)
                {
                    connnectionContainer.Value.Dispose();
                }
            }
            if (_SharedConnectionDictionary.ContainsKey(ConnectionTypes.Data))
            {
                _SharedConnectionDictionary.Remove(ConnectionTypes.Data);
            }
            if (_SharedConnectionDictionary.ContainsKey(ConnectionTypes.System))
            {
                _SharedConnectionDictionary.Remove(ConnectionTypes.System);
            }
        }
    }
}
```

``` C#
            // 予約枠とプレーヤーの更新
            _TransactionScopeManager.OnScope(() =>
            {
                foreach (var frame in targetFrameList)
                {
                    // 枠の確定フラグを上げる
                    frame.ConfirmFlag = true;
                    _TRe_ReservationFrameModel.Save(frame, staffCD, staffName, program, terminal);

                    // プレーヤーを自動キャンセルする
                    foreach (var item in playerList.Where(w => w.ReservationFrameNo == frame.ReservationFrameNo).ToList())
                    {
                        item.ReservationCancelFlag = true;
                        item.ReservationCancelDate = DateTime.Now;
                        _TRe_ReservationPlayerModel.Save(item, staffCD, staffName, program, terminal);
                    }
                }
            });
```

``` C#
using System;
using System.Transactions;

namespace RN3.Web.Common.Connection
{
    /// <summary>
    /// TransactionScopeを管理するクラスです
    /// </summary>
    public class TransactionScopeManager
    {
        private ConnectionManager _ConnectionManager;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="connectionManager"></param>
        public TransactionScopeManager(ConnectionManager connectionManager)
        {
            _ConnectionManager = connectionManager;
        }

        /// <summary>
        /// TransactionScope範囲にてActionを実行します
        /// Actionを実行後にはConnectionManagerをDisposeし
        /// TransactionをCompleteします
        /// </summary>
        /// <param name="action"></param>
        public void OnScope(Action action)
        {
            _ConnectionManager.DisConnect();
            TransactionScope scope1 = null;
            TransactionScope scope2 = null;
            var count = 1;

            try
            {
                _ConnectionManager.Connecting += handler;
                action();
                if (scope2 != null)
                {
                    scope2.Complete();
                }
                if (scope1 != null)
                {
                    scope1.Complete();
                }
            }
            finally
            {
                _ConnectionManager.Connecting -= handler;
                if (scope2 != null)
                {
                    scope2.Dispose();
                }
                if (scope1 != null)
                {
                    scope1.Dispose();
                }
            }

            void handler(object sender, EventArgs e)
            {
                if (count == 1)
                {
                    scope1 = new TransactionScope(
                        TransactionScopeOption.RequiresNew,
                        new TransactionOptions()
                        {
                            IsolationLevel = IsolationLevel.ReadCommitted,
                            Timeout = TimeSpan.FromMinutes(10)
                        });
                }
                else if (count == 2)
                {
                    scope2 = new TransactionScope(
                        TransactionScopeOption.RequiresNew,
                        new TransactionOptions()
                        {
                            IsolationLevel = IsolationLevel.ReadCommitted,
                            Timeout = TimeSpan.FromMinutes(10)
                        });
                }
                count++;
            };
        }
    }
}
```

``` C#
namespace RN3.Web.Common.Data.Setting
{
    /// <summary>
    /// DB接続情報
    /// </summary>
    public class ConnectionInfo
    {
        /// <summary>
        /// 接続先サーバ名
        /// </summary>
        public string ServerName { get; set; }
        /// <summary>
        /// DBインスタンス名
        /// </summary>
        public string DataBaseName { get; set; }
        /// <summary>
        /// ログインユーザID
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// ログインパスワード
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 接続情報文字列
        /// </summary>
        public string ConnectionString
        {
            //TODO: BIツールの分析データ取得でタイムアウトとなるため、一時的に無限に設定
            get
            {
                return @"Data Source=" + this.ServerName
                    + ";Initial Catalog=" + this.DataBaseName
                    + ";User Id=" + this.UserId
                    + ";Password=" + this.Password + ";Connection Timeout=0;";
            }
        }
    }
}
```

``` C#
namespace RN3.Web.Common.Data.Setting
{
    /// <summary>
    /// DB接続情報コンテナ
    /// </summary>
    public class ConnectionInfoContainer
    {
        /// <summary>
        /// システムDB接続情報
        /// </summary>
        public ConnectionInfo SystemConnection { get; set; }
        /// <summary>
        /// データDB接続情報
        /// </summary>
        public ConnectionInfo DataConnection { get; set; }
    }
}
```

``` C#
using RN3.Web.Common.Data.Setting;

namespace RN3.Web.Common.Store.Config
{
    /// <summary>
    /// アプリケーション設定
    /// </summary>
    public static class Configurations
    {
        /// <summary>
        /// DB接続情報コンテナ
        /// </summary>
        public static ConnectionInfoContainer ConnectionContainer { get; set; }
        /// <summary>
        /// 認証設定
        /// </summary>
        public static AuthSetting AuthSetting { get; set; }
        /// <summary>
        /// 印刷設定
        /// </summary>
        public static PrintSetting PrintSetting { get; set; }
        /// <summary>
        /// ログ保存設定
        /// </summary>
        public static Logging Logging { get; set; }
    }
}
```

``` C# : Startup.cs
        /// <summary>
        /// サービスコンテナにサービスを登録します。
        /// </summary>
        /// <param name="services">サービスコンテナ</param>
        /// <remarks>
        /// アプリケーションのサービスを構成する Configure メソッドの前にホストによって呼び出されます。
        /// </remarks>
        public void ConfigureServices(IServiceCollection services)
        {
            // ControllerアセンブリをMVCサービスに登録
            this.SetApplicationPart(ref services);
            // DIコンテナにサービスを登録
            this.RegistService(ref services);

            etc...
        }

        /// <summary>
        /// サービスコンテナにDIするサービスを登録
        /// </summary>
        /// <param name="services">サービスコンテナ</param>
        private void RegistService(ref IServiceCollection services)
        {
            // DB接続の管理などのサービスは接続ごとに保持するAddScopedを使う
            // モデルは要求ごとに作成されるAddTransientを使います
            services
                .AddScoped<ConnectionManager>()
                .AddScoped<DapperAction>()
                .AddScoped<TransactionScopeManager>()
                .AddScoped(etc...);
        }
```

``` VB
    ''' <summary>
    ''' TddPaymentStatementHistoryに対して保存を実行する処理
    ''' </summary>
    ''' <param name="SlipNuber"></param>
    ''' <param name="EffectiveDate"></param>
    ''' <param name="BillPrice"></param>
    Public Sub SavePaymentStatementHistory(ByVal SlipNuber As String,
                                           ByVal EffectiveDate As DateTime?,
                                           ByVal BillPrice As String)
        'データ生成
        Dim obj As New PaymentStatementHistoryClass
        With obj
            .SlipNumber = SlipNuber
            .EffectiveDate = CDate(EffectiveDate)
            .BillPrice = BillPrice
        End With

        'SqlConnectionをUsingした場合、Disposeと同時にCloseされるのでFinallyで明示的にCloseする必要はない。
        'https://docs.microsoft.com/ja-jp/dotnet/api/system.data.sqlclient.sqlconnection.close?redirectedfrom=MSDN&view=netframework-4.7.2#System_Data_SqlClient_SqlConnection_Close
        Using con As New SqlConnection(New DataAccess(DataAccess.AppConectMode.DATA).CreConectStr)
            con.Open()

            'トランザクション
            Using tr As SqlTransaction = con.BeginTransaction
                Try
                    Using cls As New PaymentStatementHistoryClass
                        cls.Insert(obj, tr)
                    End Using

                    'コミット
                    tr.Commit()
                Catch ex As Exception
                    tr.Rollback()
                    Throw New Exception("保存処理に失敗しました。" & vbCrLf & ex.Message, ex)
                End Try
            End Using
        End Using
    End Sub
```
