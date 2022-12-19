# SelectIf

---

## 概要

if文でselectを実行するかしないかをチェーンで実現する拡張機能。  

selectの中でifを噛ましてもいいが、ループの度にifの判定がされるので、オーダー量が心配。  
その点、拡張メソッドでifなら実行しないようにすれば、効率的。  

---

## Selectの自前実装

``` cs
public static IEnumerable<TResult> MySelect<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
{
    foreach (var item in source)
    {
        yield return selector(item);
    }
}
```

---

## 実装

拡張メソッド定義  

``` cs
public static class LinqExtension
{
    public static IEnumerable<TResult> SelectIf<TSource, TResult>(this IEnumerable<TSource> source, bool condition, Func<TSource, TResult> selector)
        => condition ? source.Select(selector) : (IEnumerable<TResult>)source;
}
```

---

[【C#】自作LINQオペレータの作り方](https://qiita.com/yutorisan/items/2c796841c6f047358c03)  
