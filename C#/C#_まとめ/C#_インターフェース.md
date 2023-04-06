# インターフェース

---

``` cs
interface I
{
    // 今までも書けたもの（publicな非静的関数メンバーのみ）
    void PublicMethod();
    int PublicProperty { get; }

    // 今まで書けなかったけども書けるようになるもの
    protected void ProtectedMethod();
    public void ImplementedMethod() { }
    public int ImplementedProperty => 0;
    public const int Constant = 2;
    public static readonly int StaticField = Math.Exp(Constant);
    public static int StaticMethod(int x) => StaticField * x;

    // 今後も書けないもの（インスタンスフィールドとコンストラクター）
    int InstanceField;
    public I(int value) => InstanceField = value;
}
```

[デフォルト実装の導入がもたらす影響 ― C#への「インターフェースのデフォルト実装」の導入（中編） - Build Insider](https://www.buildinsider.net/column/iwanaga-nobuyuki/014)  

---

## インターフェースのインスタンス

Javaだと匿名クラスでのみInterfaceのインスタンスを作成できるって話だったが、聞いているだけではちょっとイメージできなかったのでまとめてみた。  

[[Java] インターフェースをnewする違和感が解決した](https://qiita.com/imanishisatoshi/items/f73abc8206f405970d4f)  

ネタばれすると「インターフェースを継承した名前の無いクラスをnewしていただけ」という落ちだったが、面白い発見だった。  
後日、基本情報の勉強をしているとこれが出てきた。  
普通に基礎レベルの内容だったらしい。  

---

## インターフェイスにインターフェイスを実装できるのか?

実装はしないけど継承？みたいなことはできる。  
その際、インターフェイスの中で継承元のインターフェイスの実装を強制されることはない。  
しかし、そのインターフェイスを実装したクラスでは、積み上げたインターフェイス全てを実装しないといけないらしい。  

``` C#
    interface ITest1
    {
        void Test1();
    }

    interface ITest2 : ITest1
    {
        void Test2();
    }

    interface ITest3 : ITest2
    {
        void Test3();
    }

    public class Test : ITest3
    {
        public void Test1()
        {
            throw new NotImplementedException();
        }
        public void Test2()
        {
            throw new NotImplementedException();
        }
        public void Test3()
        {
            throw new NotImplementedException();
        }
    }

    public class Test2 : ITest2
    {
        public void Test1()
        {
            throw new NotImplementedException();
        }
        void ITest2.Test2()
        {
            throw new NotImplementedException();
        }
    }
```

---

## ダイアモンド継承

``` C#
    interface IA
    {
        void M() => Console.WriteLine("A.M");
    }

    interface IB : IA
    {
        void IA.M() => Console.WriteLine("B.M");
    }

    interface IC : IA
    {
        void IA.M() => Console.WriteLine("C.M");
    }

    // IB にも IC にも M の実装があって、どちらを使いたいのか不明瞭(コンパイル エラー)。
    // インターフェイスメンバー'IA.M()'には最も固有な実装がありません。'IB.IM.M()'と'IC.IA.M()'のどちらも最も固有なものではありません。
    class C : IB, IC
    {
        // これなら IB.M でも IC.M でもなく、この M が呼ばれるので明瞭
        //public void M() => Console.WriteLine("new implementation");
    }
```

---

## トレイト

[C# 8のデフォルトインターフェースメソッド](https://www.infoq.com/jp/articles/default-interface-methods-cs8/)  
C# 8の新機能としてデフォルトインターフェースメソッドが提案されている。  
これはトレイトというプログラミングテクニックを可能にするものである。  
トレイトとは、関連のないクラス間でメソッドを再利用できるオブジェクト指向プログラミング技術である。  

``` C#
    interface IDefaultInterfaceMethod
    {
        public void DefaultMethod()
        {
            Console.WriteLine("I am a default method in the interface!");
        }
    }

    class AnyClass : IDefaultInterfaceMethod { }

    class Program
    {
        static void Main()
        {
            IDefaultInterfaceMethod anyClass = new AnyClass();
            anyClass.DefaultMethod();
        }
    }
```

---

## インターフェースに拡張メソッドを追加

``` C#
// ITestインターフェース
public interface ITest
{
    string TestStr { get; set; }
}

// ITest実装クラス
public class Test : ITest
{
    public string TestStr { get; set; }
}

// ITestインターフェース拡張クラス
public static class ITestExtension
{
    // 拡張メソッド
    public static string Print(this ITest test)
    {
        return test.TestStr + "_test";
    }
}

// 
public class InterfaceExtensionMethod
{
    public static void Execute()
    {
        ITest test = new Test() { TestStr = "1234" };
        Console.WriteLine(test.Print());
    }
}
```

---

## 拡張メソッドの意義

>前節の通り、実を言うと、拡張メソッドは両手ばなしによろこべる機能ではなかったりします。  
>インスタンス メソッドでの実装が可能ならば素直にクラスのインスタンス メソッドとして定義すべきです。
>
>拡張メソッドは、「クラスを作った人とは全くの別人がメソッドを足せる」という点が最大のメリットです。  
>このメリットは、特にインターフェイスに対して需要があります。  
>多くの場合、インターフェイスを作る人と、そのインターフェイスを使った処理を書く人は別です。  
>通常、この「インターフェイスを使った処理」は静的メソッドになりがちです。  
>そして、拡張メソッドの真骨頂は「（本来は前置き記法である）静的メソッドを後置き記法で書ける」という部分にあると思っています。  
>
>例えば、下図のような、データ列に対するパイプライン処理を考えてみます。
>`入力 → 条件で絞る x > 10 → 値を加工する x*x → 出力`  
>
>まず、条件付けや値の加工のために以下のような静的メソッドを用意します。
>
>``` C#
>static class Extensions
>{
>    public static IEnumerable<int> Where(this IEnumerable<int> array, Func<int, bool> pred)
>    {
>        foreach (var x in array)
>            if (pred(x))
>                yield return x;
>    }
>    public static IEnumerable<int> Select(this IEnumerable<int> array, Func<int, int> filter)
>    {
>        foreach (var x in array)
>            yield return filter(x);
>    }
>}
>```
>
>これを、静的メソッド呼び出しの構文で書くと以下のようになります。
>
>``` C#
>var input = new[] { 8, 9, 10, 11, 12, 13 };
>
>var output =
>    Extensions.Select(
>        Extensions.Where(
>            input,
>            x => x > 10),
>        x => x * x);
>```
>
>やりたいパイプライン処理の順序と、語順が逆になります。  
>また、「Where とそれに対する条件式 x > 10」や 「Select とそれに対する加工式 x * x」の位置が離れてしまいます。  
>
>これに対して、拡張メソッド構文を使うと、以下のようになります。  
>
>``` C#
>var input = new[] { 8, 9, 10, 11, 12, 13 };
>var output = input
>    .Where(x => x > 10)
>    .Select(x => x * x);
>```
>
>ただ語順が違うだけなんですが、 こちらの方がやりたいことの意図が即座に伝わります。  
>すなわち、パイプライン処理（フィルタリング処理）は、 後置きの語順が好ましい処理です。  
>というように、 語順的に後置きの方がしっくりくる場合に （というか、むしろその場合のみに）、 静的メソッドを拡張メソッド化することをお勧めします。  
>
>[mikakuninn](https://ufcpp.net/study/csharp/sp3_extension.html#interface)  
