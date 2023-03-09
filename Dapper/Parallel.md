# Dapperの並列実行

## 基本

``` cs
public static void Execute()
{
    var con_str = @"Server=.\SQLEXPRESS;Database=SandBox;Integrated Security=True;";

    using (var connection = new ThreadLocal<SqlConnection>(
        () =>
        {
            var conn = new SqlConnection(con_str);
            conn.Open();
            return conn;
        },
        trackAllValues: true
    ))
    {
        Parallel.For(1, 100000, x =>
        {
            var res = connection.Value.Query<dynamic>("SELECT * FROM items").First();
        });
        // 生成された全てのConnectionを一括Dispose
        foreach (var item in connection.Values.OfType<IDisposable>())
            item.Dispose();
    }
}
```

---

## 解説

`SqlConnection`はスレッドセーフではないので、そのまま`Parallel.For`すると死ぬ。  

``` cs
var con_str = @"Server=.\SQLEXPRESS;Database=SandBox;Integrated Security=True;";
using (var connection = new SqlConnection(con_str))
{
    connection.Open();
    Parallel.For(1, 1000, x =>
    {
        _ = connection.Query<DateTime>("select current_timestamp").First();
    });
}

//例外が発生しました: CLR/System.InvalidOperationException
//型 'System.InvalidOperationException' の例外が System.Data.SqlClient.dll で発生しましたが、ユーザー コード内ではハンドルされませんでした: 'The requested operation cannot be completed because the connection has been broken.'
```

`connection.Open();`を`Parallel.For`の中に入れればこのエラーは発生しなくなるが、毎回オープンしてクローズしたくはない。  
一回開いたら終わるまで開きっぱなしにしたい。  

``` cs
var con_str = @"Server=.\SQLEXPRESS;Database=SandBox;Integrated Security=True;";
using (var connection = new SqlConnection(con_str))
{
    Parallel.For(1, 1000, x =>
    {
        connection.Open();
        _ = connection.Query<DateTime>("select current_timestamp").First();
    });
}
```

`ThreadLocal`なるものを使用することで、スレッドセーフな`SqlConnection`を作ることができる。  

``` cs
var con_str = @"Server=.\SQLEXPRESS;Database=SandBox;Integrated Security=True;";
using (var connection = new ThreadLocal<SqlConnection>(
    () => 
    {
        var conn = new SqlConnection(con_str);
        conn.Open(); 
        return conn; 
    }
))
{
    Parallel.For(1, 1000, x =>
    {
        _ = connection.Value.Query<DateTime>("select current_timestamp").First();
    });
}
```

ThreadLocalを使用したコネクションのオープンではDisposeを明示する必要がある。  
通常のusingによるDisposeはThreadLocalのDisposeであって、その中身のDisposeまでは行ってくれない模様。  
Dispose対策までしたものが基本の形となる。  

---

## DisposableThreadLocal

毎回書くのはだるいし、内部のDisposeくらい自動的にやってくれるような仕組みを作ってもいい。  
というわけで、内部のDisposeまで実行してくれる`ThreadLocal`をラップしたクラスを作ると、シンプルに書けていいよっていうやつ。  

``` cs
var con_str = @"Server=.\SQLEXPRESS;Database=SandBox;Integrated Security=True;";
using (var connection = DisposableThreadLocal.Create(
    () =>
    {
        var conn = new SqlConnection(con_str);
        conn.Open();
        return conn;
    }
))
{
    Parallel.For(1, 1000, x =>
    {
        _ = connection.Value.Query<DateTime>("select current_timestamp").First();
    });
}

public static class DisposableThreadLocal
{
    public static DisposableThreadLocal<T> Create<T>(Func<T> valueFactory)
        where T : IDisposable
        => new DisposableThreadLocal<T>(valueFactory);

}

public class DisposableThreadLocal<T> : ThreadLocal<T>
    where T : IDisposable
{
    public DisposableThreadLocal(Func<T> valueFactory)
        : base(valueFactory, trackAllValues: true)
    {
    }

    protected override void Dispose(bool disposing)
    {
        var exceptions = new List<Exception>();
        foreach (var item in this.Values.OfType<IDisposable>())
        {
            try
            {
                item.Dispose();
            }
            catch (Exception e)
            {
                exceptions.Add(e);
            }
        }

        base.Dispose(disposing);

        if (exceptions.Any())
            throw new AggregateException(exceptions);
    }
}
```

---

## 速度検証

ループ回数が1,000回までの場合、通常のForが早いが、10万回になってくると圧倒的にParallelForの方が早いことが分かった。  
というわけで、並列の効果はあるものと思われる。  

しかし、本当に10万回もループして結果が帰ってきているのか確証がなかったので、確認のためにListにAddしてCountを見てみたら、キッチリ10万になっていなかった。  
毎回微妙に数字が変わるし、本当に大丈夫なのかと思ってしまう。  

``` cs
public static void Execute()
{
    var for_count = 100000;
    var con_str = @"Server=.\SQLEXPRESS;Database=SandBox;Integrated Security=True;";

    // Parallel For
    var stopWatch = Stopwatch.StartNew();
    using (var connection = new ThreadLocal<SqlConnection>(
        () =>
        {
            var conn = new SqlConnection(con_str);
            conn.Open();
            return conn;
        },
        trackAllValues: true
    ))
    {
        var list = new List<dynamic>();
        Parallel.For(1, for_count, x =>
        {
            var res = connection.Value.Query<dynamic>("SELECT * FROM items").First();
            list.Add(res);
        });
        foreach (var item in connection.Values.OfType<IDisposable>())
            item.Dispose();
        Console.WriteLine(list.Count());
    }
    stopWatch.Stop();
    Console.WriteLine(stopWatch.Elapsed);


    // Normal For
    stopWatch = Stopwatch.StartNew();
    using (var connection = new SqlConnection(con_str))
    {
        var list = new List<dynamic>();
        connection.Open();
        for (var i = 1; i <= for_count; i++)
        {
            var res = connection.Query<dynamic>("SELECT * FROM items").First();
            list.Add(res);
        };
        Console.WriteLine(list.Count());
    }
    stopWatch.Stop();
    Console.WriteLine(stopWatch.Elapsed);
}

// 1,000回
// Parallel.For Count   : 970              ←？？？
// Parallel.For Elapsed : 00:00:00.2176943
// Normal For Count     : 1000
// Normal For Elapsed   : 00:00:00.0981057 ← 早い

// 100,000回
// Parallel.For Count   : 95709            ←？？？
// Parallel.For Elapsed : 00:00:01.2726733 ← 早い
// Normal For Count     : 100000
// Normal For Elapsed   : 00:00:07.1138585
```

---

[neue cc - 並列実行とSqlConnection](https://neue.cc/2013/03/09_400.html)  
