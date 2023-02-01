# WhereIf

ifでwhereを実行するかしないかを実現できないか調査した。  

``` cs
var list = new List<int>(){1,2,3,4,5,6,7,8,9};
if (Condition)
{
    list = list.Where(predicate).toList();
}
```

これを改善したい。  
Linqメソッドのチェーンとして実現させたかった。  

---

## 実装

拡張メソッド定義

``` cs
public static class LinqExtension
{
    public static IEnumerable<TSource> WhereIf<TSource>
        (this IEnumerable<TSource> source, bool condition, Func<TSource, bool> predicate)
            => condition ? source.Where(predicate) : source;

    public static IEnumerable<TSource> WhereIfElse<TSource>
        (this IEnumerable<TSource> source, bool condition, Func<TSource, bool> truePredicate, Func<TSource, bool> falsePredicate)
            => condition ? source.Where(truePredicate) : source.Where(falsePredicate);
}
```

使用例

``` cs
var list = new List<int>(){1,2,3,4,5,6,7,8,9};

// trueなのでwhereが実行される。
// 5,6,7,8,9
list = list.WhereIf(true, w => w >= 5).ToList();

// falseなのでwhereは実行されない。
// 1,2,3,4,5,6,7,8,9
list = list.WhereIf(false, w => w <= 5).ToList();
```

---

## IQueryableでの実装

参考リンクでの実装はIQueryableとなっていた。  
AsQueryableとすれば使用可能。  

``` cs
public static class LinqEx
{
    public static IQueryable<TSource> WhereIf<TSource>(
        this IQueryable<TSource> Source, bool Condition, Expression<Func<TSource, bool>> Predicate)
            => Condition ? Source.Where(Predicate) : Source;
}
```

``` cs
var list = new List<int>(){1,2,3,4,5,6,7,8,9};

list = list.AsQueryable().WhereIf(true, w => w > 2).ToList();
```

[C# LINQで動的なWhereを実現する](https://heinlein.hatenablog.com/entry/2018/08/15/101552)  

---

## 参考

[C# LINQで動的なWhereを実現する](https://heinlein.hatenablog.com/entry/2018/08/15/101552)  
[Use WhereIf for multiple condition in c#](https://stackoverflow.com/questions/61269629/use-whereif-for-multiple-condition-in-c-sharp)  
[WhereIf](https://www.extensionmethod.net/csharp/ienumerable-t/whereif)  

- c# linq where 複数 動的  
- linq 拡張メソッド 自作  
