# Linq Cast

## LinqのCast

Castって失敗した時の挙動ってどうなのか知らなかったので調べた。  
→  
失敗すると例外を吐く模様。  

それはそうと、CastはFirstと同じくらい罠らしく、CastではなくOfTypeを使うべきって記事を見つけたのでまとめる。  

OfType\<T>
キャストできなかった時 : 要素はフィルタリングされる
要素がnullだった時 : 要素はフィルタリングされる

Cast\<T>
キャストできなかった時 : 例外になる
要素がnullだった時 : 例外になる or 要素はフィルタリングされない

OfType\<T>は挙動の一貫性があり、予想外の値が来た時も安定してフィルタリングしてくれますね。  
以上の点から、Cast\<T>を使う必要はほぼなく、OfType\<T>を使うことを強くお勧めします。  

``` C#
    object[] hogeArray = new object[] { 3, "apple", 42 };
    foreach (var item in hogeArray.OfType<int>())
    {
        // キャストできたものだけforeachの中にくるので、
        // 3 と 42 だけが出力される
        Console.WriteLine(item);
    }
    // 2個目の要素を列挙しようとした時に例外発生
    foreach (var item in hogeArray.Cast<int>())
    {
        // 3 はintにキャストできるので出力されるが、
        // "apple"をキャストしようとしたタイミングで例外発生!
        Console.WriteLine(item);
    }

    // もちろんこれもエラーになる。
    // System.InvalidCastException: '指定されたキャストは有効ではありません。'
    foreach (var item in hogeArray.Select(s => (int)s))
    {
        Console.WriteLine(item);
    }
```

``` C#
    // こうやるのと
    if (args?.AddedItems?.Count > 0
        && args.AddedItems.OfType<Hoge>() is IEnumerable<Hoge> addItems)
    {
        HogeList.AddRange(addItems);
    }
    // こうやるのとだったら、こっちのほうがいいのでは
    foreach (var addItem in args.AddedItems.OfType<Hoge>())
    {
        HogeList.Add(addItem);
    }
```

[【LINQ】Cast\<T>は使わない方が良い？OfType\<T>を使おう](https://threeshark3.com/castistrap/)  
