# 匿名型

`var mydata1 = new { id = 100, name = "Penguin" }`の形で定義するやつ。  

プロパティは読み取り専用。  

---

## 値の更新

C#8.0までは不可能。  
C#10.0から`With式`を用いることで可能となった。  
with式自体はC#9.0からの登場だが、匿名型に対して有効になったのは10から。

with式を用いた場合の更新というのは、どちらかというと値を変更した新しいインスタンスを作る感じになる。  
元の値は破壊しない。  

``` cs
var src = new {Fuga ="Fuga"};
Console.WriteLine(src.Fuga); // Fuga

var dst = src with {Fuga = "Piyo"};
Console.WriteLine(src.Fuga); // Fuga
Console.WriteLine(dst.Fuga); // Piyo
```

[with 式 - 既存のオブジェクトの変更されたコピーである新しいオブジェクトを作成する | Microsoft Learn](https://learn.microsoft.com/ja-jp/dotnet/csharp/language-reference/operators/with-expression)  

[【C# 機能別】知らん書き方が出てきたらこれを見ろ！C# 10 までの進化を1ページで - OITA: Oika's Information Technological Activities](https://oita.oika.me/2021/12/23/csharp-10-history#class-struct-record)  

---

## 値の追加

既存の値に新しいプロパティを追加することはできない。  
これをやりたいならExpandoObjectを使うべし。  

``` cs
var source1 = new
{
    huga = "huga",
    hoge = "hoge"
};
var source2 = new
{
    piyo = "piyo"
};
// <anonymous type: string huga,string hoge>に'piyo'の定義がありません。
var merged = source1 with { piyo = source2.piyo };
```

---

## 匿名型のマージ

専用のメソッドはないので自作する必要がある。  
参考サイトのメソッドはExpandoObjectとして作りなおしているので、マージ後はインテリセンスが効かなくなるのは注意。  

``` cs
var source1 = new
{
    huga = "huga",
    hoge = "hoge"
};
var source2 = new
{
    piyo = "piyo"
};

var merged = Merge(source1, source2);
// merged
// {
//      huga = "huga",
//      hoge = "hoge",
//      piyo = "piyo"
// }

public dynamic Merge(object item1, object item2)
{
    if (item1 == null || item2 == null)
        return item1 ?? item2 ?? new ExpandoObject();

    dynamic expando = new ExpandoObject();
    var result = expando as IDictionary<string, object>;
    foreach (System.Reflection.PropertyInfo fi in item1.GetType().GetProperties())
    {
        result[fi.Name] = fi.GetValue(item1, null);
    }
    foreach (System.Reflection.PropertyInfo fi in item2.GetType().GetProperties())
    {
        result[fi.Name] = fi.GetValue(item2, null);
    }
    return result;
}
```

因みに、これが必要になったのは、DapperのWhere条件に対するparamを動的に生成したい時であった。  
固定的な値と動的が値があり、固定的な値は最初に1つの匿名型として定義して、動的な部分はループ中に都度、匿名型を作成し、固定の匿名型とmergeして条件を生成することはできないかと考えたためである。  

[c# - Merging anonymous types - Stack Overflow](https://stackoverflow.com/questions/6754967/merging-anonymous-types)  

---

[匿名型 | Microsoft Learn](https://learn.microsoft.com/ja-jp/dotnet/csharp/fundamentals/types/anonymous-types)  
[匿名型_ipentec](https://www.ipentec.com/document/csharp-using-anonymous-type)  
