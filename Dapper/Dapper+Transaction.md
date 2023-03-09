# Dapperとトランザクション

## 基本

Executeメソッドの第三引数にSqlTransactionインスタンスを渡す。  

``` cs
// Dapperの基本となるトランザクション処理
var con_str = @"Server=<sv>;Database=<db>;User ID=<id>;Password=<passwd>;Trust Server Certificate=true;";
using (var connection = new SqlConnection(con_str))
{
    connection.Open();
    using (var tran = connection.BeginTransaction())
    {
        try
        {
            var result = connection.Execute("CREATE TABLE __HOGE1 (id int,name nvarchar)", null, tran);
            tran.Commit();
        }
        catch (Exception e)
        {
            tran.Rollback();
        }
        finally
        {
            connection.Close();
        }
    }
}
```

[C# DapperでDB接続する方法（トランザクション編）](https://learning-collection.com/c-dapper%E3%81%A7db%E6%8E%A5%E7%B6%9A%E3%81%99%E3%82%8B%E6%96%B9%E6%B3%95%EF%BC%88%E3%83%88%E3%83%A9%E3%83%B3%E3%82%B6%E3%82%AF%E3%82%B7%E3%83%A7%E3%83%B3%E7%B7%A8%EF%BC%89/)  

---

## 分散トランザクション

複数のインスタンスに跨ったトランザクションスコープは機能しないことを確認した。  
代わりに`SqlTransaction`を使った自前での分散トランザクションっぽいことは実装出来たので備忘録として残す。  

■ **TransactionScope**

`TransactionScope`を使って、複数のインスタンスに対するトランザクションはできない。  
2つ目のコネクションのオープン、もといDapperの実行でエラーとなる。  

``` cs
string con_str1 = @"Server=192.168.0.1;Database=<db1>;User ID=<id>;Password=<passwd1>;Trust Server Certificate=true;";
string con_str2 = @"Server=172.168.150.24;Database=<db2>;User ID=<id>;Password=<passwd2>;Trust Server Certificate=true;";

using (TransactionScope ts = new TransactionScope())
{
    using (var connection = new SqlConnection(con_str1))
    {
        string query = "CREATE TABLE __HOGE1 (id int,name nvarchar)";
        var result = connection.Execute(query);
    }

    using (var connection = new SqlConnection(con_str2))
    {
        string query = "CREATE TABLE __HOGE2 (id int,name nvarchar)";
        // 型 'System.PlatformNotSupportedException' のハンドルされていない例外が Microsoft.Data.SqlClient.dll で発生しました: 'This platform does not support distributed transactions.'
        var result = connection.Execute(query);
    }

    ts.Dispose();
}
```

■ **SqlTransactionを使った自前での分散トランザクション**

[Stack Overflow(Transactionscope throwing exception this platform does not support distributed transactions while opening connection object)](https://stackoverflow.com/questions/56328832/transactionscope-throwing-exception-this-platform-does-not-support-distributed-t)の内容を参考に`SqlTransaction`を使った分散トランザクションを実装してうまくいくことを確認した。  

``` cs
string con_str1 = @"Server=192.168.0.1;Database=<db1>;User ID=<id>;Password=<passwd1>;Trust Server Certificate=true;";
string con_str2 = @"Server=172.168.150.24;Database=<db2>;User ID=<id>;Password=<passwd2>;Trust Server Certificate=true;";

SqlTransaction tran1 = null;
SqlTransaction tran2 = null;

try
{
    using var connection1 = new SqlConnection(con_str1);
    connection1.Open();
    tran1 = connection1.BeginTransaction();
    _ = connection1.Execute("CREATE TABLE __HOGE1 (id int,name nvarchar)", null, tran1);

    using var connection2 = new SqlConnection(con_str2);
    connection2.Open();
    tran2 = connection2.BeginTransaction();
    _ = connection2.Execute("CREATE TABLE __HOGE2 (id int,name nvarchar)", null, tran2);

    tran1.Commit();
    tran2.Commit();
}
catch (Exception e)
{
    tran1?.Rollback();
    tran2?.Rollback();
}
```

■ **プログラム全文**

以下、検証プログラムも含めた全検証プログラム。  

``` cs
using System.Transactions;
using Dapper;
using Microsoft.Data.SqlClient;


public class TransactionScopeTest
{
    // インスタンスを跨いだトランザクションスコープは機能しない
    public static void Execute()
    {
        string con_str1 = @"Server=192.168.0.1;Database=<db1>;User ID=<id>;Password=<passwd1>;Trust Server Certificate=true;";
        string con_str2 = @"Server=172.168.150.24;Database=<db2>;User ID=<id>;Password=<passwd2>;Trust Server Certificate=true;";

        using (TransactionScope ts = new TransactionScope())
        {
            using (var connection = new SqlConnection(con_str1))
            {
                string query = "CREATE TABLE __HOGE1 (id int,name nvarchar)";
                var result = connection.Execute(query);
            }

            using (var connection = new SqlConnection(con_str2))
            {
                string query = "CREATE TABLE __HOGE2 (id int,name nvarchar)";
                // 型 'System.PlatformNotSupportedException' のハンドルされていない例外が Microsoft.Data.SqlClient.dll で発生しました: 'This platform does not support distributed transactions.'
                var result = connection.Execute(query);
            }

            ts.Dispose();
        }
    }

    public static void OriginalDistributeTransactionTest()
    {
        string con_str1 = @"Server=192.168.0.1;Database=<db1>;User ID=<id>;Password=<passwd1>;Trust Server Certificate=true;";
        string con_str2 = @"Server=172.168.150.24;Database=<db2>;User ID=<id>;Password=<passwd2>;Trust Server Certificate=true;";

        SqlTransaction tran1 = null;
        SqlTransaction tran2 = null;

        try
        {
            using var connection1 = new SqlConnection(con_str1);
            connection1.Open();
            tran1 = connection1.BeginTransaction();
            _ = connection1.Execute("CREATE TABLE __HOGE1 (id int,name nvarchar)", null, tran1);

            using var connection2 = new SqlConnection(con_str2);
            connection2.Open();
            tran2 = connection2.BeginTransaction();
            _ = connection2.Execute("CREATE TABLE __HOGE2 (id int,name nvarchar)", null, tran2);

            tran1.Rollback();
            tran2.Rollback();
        }
        catch (Exception e)
        {
            tran1?.Rollback();
            tran2?.Rollback();
        }
    }


    // Dapperの基本となるトランザクション処理。
    // 問題なくうまく行った。
    public static void BasicDapperTransaction()
    {
        string con_str1 = @"Server=192.168.0.1;Database=<db1>;User ID=<id>;Password=<passwd1>;Trust Server Certificate=true;";

        using (var connection1 = new SqlConnection(con_str1))
        {
            connection1.Open();
            using (var tran1 = connection1.BeginTransaction())
            {
                try
                {
                    //クエリ実行
                    var retCd = connection1.Execute("CREATE TABLE __HOGE1 (id int,name nvarchar)", null, tran1);
                    tran1.Rollback();
                }
                catch (Exception e)
                {
                    tran1.Rollback();
                }
                finally
                {
                    connection1.Close();
                }
            }
        }
    }

    // トランザクションをネストさせた実験
    // 問題なくうまくいった。
    public static void BasicDapperTransaction2()
    {
        string con_str1 = @"Server=192.168.0.1;Database=<db1>;User ID=<id>;Password=<passwd1>;Trust Server Certificate=true;";
        string con_str2 = @"Server=172.168.150.24;Database=<db2>;User ID=<id>;Password=<passwd2>;Trust Server Certificate=true;";

        using (var connection1 = new SqlConnection(con_str1))
        {
            connection1.Open();
            using (var tran1 = connection1.BeginTransaction())
            {
                try
                {
                    //クエリ実行
                    _ = connection1.Execute("CREATE TABLE __HOGE1 (id int,name nvarchar)", null, tran1);

                    using (var connection2 = new SqlConnection(con_str2))
                    {
                        connection2.Open();
                        using (var tran2 = connection2.BeginTransaction())
                        {
                            try
                            {
                                _ = connection2.Execute("CREATE TABLE __HOGE2 (id int,name nvarchar)", null, tran2);
                                tran1.Rollback();
                                tran2.Rollback();
                            }
                            catch (Exception e)
                            {
                                tran1.Rollback();
                                tran2.Rollback();
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    tran1.Rollback();
                }
            }
        }
    }

    // トランザクションを外に出してロールバックできるか調査。
    // 問題なくうまくいった。
    public static void TransactionHasei()
    {
        string con_str1 = @"Server=192.168.0.1;Database=<db1>;User ID=<id>;Password=<passwd1>;Trust Server Certificate=true;";
        string con_str2 = @"Server=172.168.150.24;Database=<db2>;User ID=<id>;Password=<passwd2>;Trust Server Certificate=true;";

        SqlTransaction tran1 = null;
        SqlTransaction tran2 = null;

        using (var connection1 = new SqlConnection(con_str1))
        {
            connection1.Open();
            tran1 = connection1.BeginTransaction();
            _ = connection1.Execute("CREATE TABLE __HOGE1 (id int,name nvarchar)", null, tran1);

            try
            {
                using (var connection2 = new SqlConnection(con_str2))
                {
                    connection2.Open();
                    tran2 = connection2.BeginTransaction();
                    try
                    {
                        _ = connection2.Execute("CREATE TABLE __HOGE2 (id int,name nvarchar)", null, tran2);
                        tran1.Rollback();
                        tran2.Rollback();
                    }
                    catch (Exception e)
                    {
                        tran1.Rollback();
                        tran2.Rollback();
                    }
                }
            }
            catch (Exception e)
            {
                tran1.Rollback();
            }
        }
    }

    
    // 最終形
    // うまくいった。
    public static void Saishuukei()
    {
        string con_str1 = @"Server=192.168.0.1;Database=<db1>;User ID=<id>;Password=<passwd1>;Trust Server Certificate=true;";
        string con_str2 = @"Server=172.168.150.24;Database=<db2>;User ID=<id>;Password=<passwd2>;Trust Server Certificate=true;";

        SqlTransaction tran1 = null;
        SqlTransaction tran2 = null;

        try
        {
            using var connection1 = new SqlConnection(con_str1);
            connection1.Open();
            tran1 = connection1.BeginTransaction();
            _ = connection1.Execute("CREATE TABLE __HOGE1 (id int,name nvarchar)", null, tran1);

            using var connection2 = new SqlConnection(con_str2);
            connection2.Open();
            tran2 = connection2.BeginTransaction();
            _ = connection2.Execute("CREATE TABLE __HOGE2 (id int,name nvarchar)", null, tran2);

            tran1.Rollback();
            tran2.Rollback();
        }
        catch (Exception e)
        {
            tran1?.Rollback();
            tran2?.Rollback();
        }
    }
}
```

---

## クエリでもトランザクションしたらどうなるか？

`SqlConnection`や`TransactionScope`でコード中でトランザクションを張っておきながら、SQLでも`BEGIN TRAN`、`COMMIT TRAN`した場合どうなるのか実験した。  

実行するクエリを、以下のように記述する分には普通にロールバックされることを確認した。  

``` cs
{
var con_str = @"Server=.\SQLEXPRESS;Database=SandBox;Integrated Security=True;";
using (var connection = new SqlConnection(con_str))
{
    connection.Open();
    using (var tran = connection.BeginTransaction())
    {
        string query = """
        BEGIN TRAN;
        CREATE TABLE __HOGE1 (id int,name nvarchar)
        COMMIT TRAN;
        """;
        var result = connection.Execute(query,null,tran);
        tran.Rollback();
    }
}
}
```

しかし、以下のように`query`を記述した場合、コード中でトランザクションをロールバックしても適応されることを確認した。  
しかもエラーにならない。  

``` cs
string query = """
BEGIN TRAN;
CREATE TABLE __HOGE1 (id int,name nvarchar)
COMMIT TRAN;
COMMIT TRAN;
""";
```

このように、3つもトランザクションを並べると流石にエラーになったが、それでもテーブルは出来上がっていた。  
エラーになってもロールバックが効いていないことになる。  

``` cs
string query = """
BEGIN TRAN;
CREATE TABLE __HOGE1 (id int,name nvarchar)
COMMIT TRAN;
COMMIT TRAN;
COMMIT TRAN;
""";
```

当たり前ではあるが、コードでトランザクションを張っているなら、クエリ中で余計なことはしない方がよい。  
因みにTransactionScopeでも同様の結果となった。  

``` cs
{
var con_str = @"Server=.\SQLEXPRESS;Database=SandBox;Integrated Security=True;";
using (TransactionScope ts = new TransactionScope())
{
    using (var connection = new SqlConnection(con_str))
    {
        // queryを同じように変更して実行していくと同じ結果となる
        string query = """
        BEGIN TRAN;
        CREATE TABLE __HOGE1 (id int,name nvarchar)
        COMMIT TRAN;
        COMMIT TRAN;
        """;
        var result = connection.Execute(query);
    }
}
}
```

---

[C#(.NET系)からPostgreSQLへ接続(TransactionScopeや分散トランザクションについて) - Qiita](https://qiita.com/mimitaro/items/fde451da2f722fe12072)  

[c# - Transactionscope throwing exception this platform does not support distributed transactions while opening connection object - Stack Overflow](https://stackoverflow.com/questions/56328832/transactionscope-throwing-exception-this-platform-does-not-support-distributed-t)  

[C#のSQL Serverでトランザクション使用する - プログラムを書こう！](https://www.paveway.info/entry/2020/01/08/csharp_sqlservertransaction)  
