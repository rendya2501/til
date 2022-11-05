# SqlConnection

---

## SqlConnectionをUsingした場合、Disposeと同時にCloseされるのでFinallyで明示的にCloseする必要はない

SqlConnection using close  

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

>次の例では、作成、 SqlConnection開き、そのプロパティの一部を表示します。  
>**接続はブロックの最後で自動的に using 閉じられます**。  
[Microsoft公式_SqlConnection.Close メソッド](https://docs.microsoft.com/ja-jp/dotnet/api/system.data.sqlclient.sqlconnection.close?redirectedfrom=MSDN&view=netframework-4.7.2#System_Data_SqlClient_SqlConnection_Close)  

<!--  -->
>SqlConnection.Close()にはClose と Dispose は、機能的に同じです。  
>「DbDataReader.Dispose()にこのメソッドは Close を呼び出します。」と書かれています。  
[SqlConnectionとSqlDataReaderをusingで囲った場合Closeは必要？](https://social.msdn.microsoft.com/Forums/ja-JP/c2a0c8b2-7743-4cfa-869c-f26293b0250f/sqlconnection12392sqldatareader12434using123912225812387123832258021512close?forum=csharpgeneralja)  

<!--  -->
>using文は、try {} finally {XXX.Dispose();}をわざわざ書かなくてすむようにする糖衣構文ということです。  
→へぇ～そうだったんだ。  
>functionally equivalentとあるように、 DbConnectionクラスにとっては、機能的にはCloseもDisposeもどちらも変わらない ということです。  
>なので、using文ではDisposeメソッドを必ず呼ぶので、結果、Closeメソッドは基本的に不要ということです。  
>しかし、1つ異なる点があります。  
>Closeメソッドは再度そのインスタンスを再Openできるのに、対し、Disposeメソッドは一度実行されたら、そのインスタンスにはアクセスできない。  
[using文で初期化したDbConnection、Closeを書くべき？書かなくていい？](https://qiita.com/momotaro98/items/c4fe0fff0c173e879f2d)  

---

## なぜCloseを書くのか

>It doesn't affect the behaviour of the code, but it does aid readability.  
>つまり、 可読性のため。(既存のコードに影響を与えないし) とのこと。  
>複数のスコープがあるとどこでインスタンスがCloseされているのかがわかりにくくなるので、Closeを書くとよいということです。  
[Should I call Close() or Dispose() for stream objects?](https://stackoverflow.com/questions/7524903/should-i-call-close-or-dispose-for-stream-objects/7525134#7525134)  
