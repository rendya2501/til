
# All

[LINQのAllとAnyが空のシーケンスに対して返す値](https://pdwslmr.netlify.app/posts/language/linq-all-any-empty/)  

シーケンスに条件を満たさない要素が含まれている場合はfalse、それ以外の場合はtrueを返す。  
**空のシーケンス**に条件を満たさない要素は存在しないので**trueを返す**。  

[Allの実装](https://github.com/microsoft/referencesource/blob/4.6.2/System.Core/System/Linq/Enumerable.cs#L1182)

``` C# : Enumerable.cs
public static bool All<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate) {
    if (source == null) throw Error.ArgumentNull("source");
    if (predicate == null) throw Error.ArgumentNull("predicate");
    foreach (TSource element in source) {
        if (!predicate(element)) return false;
    }
    return true;
}
```

---

## 1件もない場合、falseにしたい場合

1件も要素がない場合、foreachは実行されないのでtrueになる。  
ではあるのだが、要素がない場合、falseにしてもらいたい。  
Select,Distinct,OrderBy,FirstOrDefaultを組み合わせることで実現可能であることを発見した。  
ヒントはStackOverFlowから得た。  
[Is there a better way of calling LINQ Any + NOT All?](https://stackoverflow.com/questions/29993814/is-there-a-better-way-of-calling-linq-any-not-all)  
やはり外人ニキ。  

``` C# : できたやつ
    // 混合要素の場合
    // 結果: false
    var tupleList = new List<(int Index, bool? flag)>{(1, false),(2, true),(3, null)}
        .Select(s => s.flag == true)
        .Distinct()
        .OrderBy(o => o)
        .FirstOrDefault();

    // 要素が全てtrueの場合
    // 結果: true
    var tupleList = new List<(int Index, bool? flag)>{(1, true),(2, true),(3, true)};

    // 要素なしの場合
    // 結果: false
    var tupleList = new List<(int Index, bool? flag)>();
```

``` C# : 要素あり解説
    var tupleList = new List<(int Index, bool? flag)>
        {
            (1, true),
            (2, false),
            (3, null),
        };
    // ①nullはfalseに置き換わる。
    // 結果: true,false,false
    var v1 = tupleList.Select(a => a.flag == true);
    // ②重複をなくしてtrueとfalseの2つだけにする
    // 結果: true,false
    var v2 = v1.Distinct();
    // ③OrderByするとfalseが先に来る。trueしかない場合、もちろんtrueが先頭になる。
    // 結果: false,true
    var v3 = v2.OrderBy(o => o);
    // ④一番最初を取得すれば結果になる。
    // 結果: false
    var v4 = v3.FirstOrDefault();
```

``` C# : 要素なし解説
    var nullList = new List<(int Index, bool? flag)>();
    // 結果なし
    var v1 = nullList.Select(a => a.flag == true);
    // 結果なし
    var v2 = v1.Distinct();
    // 結果なし
    var v3 = v2.OrderBy(o => o);
    // 結果がない場合のデフォルトはfalseになる
    var v4 = v3.FirstOrDefault();
```

``` C#
    // nullまたはfalseが1件でもあればtrue、を打ち消す。
    var ee = nullList.Any(a => a.flag == null || a.flag == false);
```
