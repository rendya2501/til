# C#_Foreach

---

## Foreachでnullが来ても大丈夫な書き方

foreachをやる前にifでnullチェックするのが野暮ったく感じたし、インデントが深くなってしまうのでなんかいい書き方はないかなということで探した。  
前にも調べたはずだが、忘れたっぽいので改めてまとめる。  

null合体演算子[??]と`Enumerable.Empty<T>()`か`new List<T>()`の2つ組み合わせで実現できる模様。  
Enumerable.EmptyはLinqで空のシーケンスを取得するための構文の模様。  

``` C#
    // 例1
    foreach (string msg in msgList ?? Enumerable.Empty<String>()){}

    // 例2
    foreach (string msg in msgList ?? new List<string>()){}
```

[【C#,LINQ】Empty～空のシーケンスがほしいとき～](https://www.urablog.xyz/entry/2018/06/02/070000)  
[foreachの時のNullReferenceExceptionを回避する](https://tiratom.hatenablog.com/entry/2018/12/16/foreach%E3%81%AE%E6%99%82%E3%81%AENullReferenceException%E3%82%92%E5%9B%9E%E9%81%BF%E3%81%99%E3%82%8B)  

---

## Enumerable.Empty vs new List

Foreachでnullが来ても大丈夫な書き方でEnumerableとListの2パターンを示したが、ではどちらがいいのかという問題が発生する。  
2つの単語を並べただけでどちらがいいのか？という記事はたくさんヒットした。  

>たとえ空の配列や空のリストを使ったとしても、それらはオブジェクトであり、メモリに保存されています。  
>ガーベッジコレクタはそれらの面倒を見なければなりません。  
>高スループットのアプリケーションを扱っている場合、それは顕著な影響を与える可能性があります。  
>Enumerable.Emptyは、呼び出しごとにオブジェクトを作成しないので、GCへの負荷が少なくなります。  
[Is it better to use Enumerable.Empty\<T>() as opposed to new List\<T>() to initialize an IEnumerable\<T>?](https://stackoverflow.com/questions/1894038/is-it-better-to-use-enumerable-emptyt-as-opposed-to-new-listt-to-initial)  

<!--  -->
> Because Enumerable.Empty caches the zero-element array, it can provide a slight performance advantage in some programs.
> Enumerable.Emptyはゼロ要素配列をキャッシュするため、一部のプログラムではパフォーマンスがわずかに向上する可能性があります。  
[C# Enumerable.Empty](https://thedeveloperblog.com/empty)  

ということで、Enumerableに軍配が上がる模様。  

---

## foreach index

``` C#
foreach (var (item, index) in items.Select((item, index) => (item, index)))
{
    Console.WriteLine($"index: {index}, value: {item}");
}
```

毎回書くのがだるいなら拡張メソッドを作ってもいいかもね。

``` C#
public static partial class TupleEnumerable
{
    public static IEnumerable<(T itme, int index)> Indexed<T> (this IEnumerable<T> source)
    {
        if(source == null) throw new ArumentNullExeption(nameof(source));

        IEnumerable<(T item, int index)> impl(){
            var i = 0;
            foreach(var item in source) 
            {
                yield return (item,i);
                ++i;
            }
        }

        return impl();
    }
}
```

``` C# : 使用例
foreach (var (item,index) in items.Indexed())
{
    Console.WriteLine($"index: {index}, value: {item}");
}
```

[未確認飛行C](https://ufcpp.net/blog/2016/12/tipsindexedforeach/)  
