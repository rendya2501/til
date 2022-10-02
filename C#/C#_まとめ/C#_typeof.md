# typeof

---

[[C# クラス] キャストで型変換（基底クラス⇔派生クラス）](https://yaspage.com/prog/csharp/cs-class-cast/)  
アップキャストダウンキャストの例としてはわかりにくいけど、typeofの使い方は参考になったので、それはそれでまとめる。  

`型を厳密にチェックしたい場合は、typeof を使います。`  
ほーんって感じ。  
Typeofってnameofと合わせて使ってた気がする。  
そんなことなかったわ。  

``` C#
    /// <summary>
    /// 指定されたインスタンスを複写します。
    /// </summary>
    /// <typeparam name="T">型</typeparam>
    /// <param name="item">複写元</param>
    /// <returns></returns>
    private T GetCopy<T>(T item) where T : class
    {
        return JsonConvert.DeserializeObject(JsonConvert.SerializeObject(item), typeof(T)) as T;
    }
```

``` C#
using System;

// 基底クラス
class BaseClass { }
// 派生クラス
class ChildClass : BaseClass { }
// 派生クラスの派生クラス
class GrandChildClass : ChildClass { }

// メインプログラム
class Program
{
    public static void Main()
    {
        BaseClass b = new BaseClass();
        Console.WriteLine("BaseClass b = new BaseClass();");
        Console.WriteLine($"bはBaseClass型？       = {b.GetType() == typeof(BaseClass)}"); // True
        Console.WriteLine($"bはChildClass型？      = {b.GetType() == typeof(ChildClass)}"); // False
        Console.WriteLine($"bはGrandChildClass型？ = {b.GetType() == typeof(GrandChildClass)}"); // False

        b = new ChildClass();
        Console.WriteLine("\nBaseClass b = new ChildClass();");
        Console.WriteLine($"bはBaseClass型？       = {b.GetType() == typeof(BaseClass)}"); // False
        Console.WriteLine($"bはChildClass型？      = {b.GetType() == typeof(ChildClass)}"); // True
        Console.WriteLine($"bはGrandChildClass型？ = {b.GetType() == typeof(GrandChildClass)}"); // False

        b = new GrandChildClass();
        Console.WriteLine("\nBaseClass b = new GrandChildClass();");
        Console.WriteLine($"bはBaseClass型？       = {b.GetType() == typeof(BaseClass)}"); // False
        Console.WriteLine($"bはChildClass型？      = {b.GetType() == typeof(ChildClass)}"); // False
        Console.WriteLine($"bはGrandChildClass型？ = {b.GetType() == typeof(GrandChildClass)}"); // True
    }
}
// BaseClass b = new BaseClass();
// bはBaseClass型？       = True
// bはChildClass型？      = False
// bはGrandChildClass型？ = False

// BaseClass b = new ChildClass();
// bはBaseClass型？       = False
// bはChildClass型？      = True
// bはGrandChildClass型？ = False

// BaseClass b = new GrandChildClass();
// bはBaseClass型？       = False
// bはChildClass型？      = False
// bはGrandChildClass型？ = True
```
