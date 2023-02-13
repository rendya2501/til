# C#のアクセス修飾子

---

## 早見表

- public    : あらゆる所からアクセスできる  
- private   : 同じクラス内のみアクセスできる  
- protected : 同じクラスと派生したクラスのみアクセスできる  
- internal  : 同じアセンブリならアクセスできる。つまり同一プロジェクト内部。  
- protected internal : 同じアセンブリと別アセンブリの派生クラスでアクセスできる  
- private protected  : 同じクラスと同じアセンブリの派生したクラスのみアクセスできる  

<https://www.fenet.jp/dotnet/column/language/6153/>

---

## クラスにアクセス修飾子を書かか無かった時のデフォルト

公式では`internal`と言っているが、`private`になるという人もいる。  
実際に確かめないと分からんなぁ。  

>クラス、レコード、構造体には、public または internal を指定できます。  
>アクセス修飾子が指定されなかった場合は、既定で internal が適用されます  
>
>クラスや構造体と同様、インターフェイスの既定のアクセス レベルは internal になります。  
>
>[アクセス修飾子 - C# プログラミング ガイド | Microsoft Learn](https://learn.microsoft.com/ja-jp/dotnet/csharp/programming-guide/classes-and-structs/access-modifiers)  

<!--  -->
>```cs
>namespace MyCsharp
>{
>    class csharprogram
>    {
>        void method1() {}
>        class csharpin {}
>    }
>}
>```
>
>上記のコードでは、クラスとそのフィールドおよびメソッドにアクセス修飾子を割り当てていません。  
>したがって、デフォルトでは、internal アクセス修飾子はクラス csharpprogram に割り当てられ、private アクセス修飾子はそのフィールドとメソッドに割り当てられます。  
>
>したがって、コードは次のコードと同じように機能します。  
>
>``` cs
>namespace MyCsharp
>{
>    internal class csharprogram
>    {
>        private void method1() {}
>        private class csharpin {}
>    }
>}
>```
>
>[C# のデフォルトのアクセス修飾子 | Delft スタック](https://www.delftstack.com/ja/howto/csharp/csharp-default-access-modifier/)  

<!--  -->
>C#でクラスのメンバに対して、アクセス修飾子を省略した場合、privateになります。  
>
>[C#でprivate書きますか？〜JavaとC#でクラスメンバにアクセス修飾子を書かない時の違い〜インターフェースについても - Qiita](https://qiita.com/RyotaMurohoshi/items/fe28abc91e24bc90a637)  

<!--  -->
>それはさておき省略した場合のアクセス修飾子は何になっているのでしょうか？  
>調べた所privateになるようです。  
>
>[C#のclassでアクセス修飾子を省略する | 迷惑堂本舗](https://maywork.net/computer/csharp-class-access-control-omit/)

---

## internal

同一アセンブリ内(同一exe/同一dll)のクラスからのみアクセス可能な修飾子。  
他のプロジェクトからは、参照設定がされていてもinternalの場合はアクセス不可となります。  

※アセンブリ : exeやdllのこと
※同じアセンブリ内とは同じプロジェクト内ということ  

Aプロジェクト,public Aクラス,internal BクラスでDLLを作成する。  
BプロジェクトでAプロジェクトDLLを参照する。  
BプロジェクトからAプロジェクトのAクラスは参照できるが、Bクラスは参照できない。  

[C#のinternalについて分かりやすく解説！｜C#のinternalを正しく使いこなそう](https://www.fenet.jp/dotnet/column/language/6153/)  

---

## protected , internal , protected internal , private protected

<https://ufcpp.net/study/csharp/oo_conceal.html#protected-internal>

```C#
public class Base
{
    public int Public { get; set; } // どこからでも
    protected int Protected { get; set; } // 派生クラスからだけ
    internal int Internal { get; set; } // 同一アセンブリ(同一 exe/同一 dll)内からだけ
    protected internal int ProtectedInternal { get; set; } // 派生クラス "もしくは" 同一アセンブリ内 から
    private protected int PrivateProtected { get; set; } // 派生クラス "かつ" 同一アセンブリ内 から(C# 7.2 以降)
    private int Private { get; set; } // クラス内からだけ

    public void Method()
    {
        // 同一クラス内
        // 全部 OK
        Public = 0;
        Protected = 0;
        Internal = 0;
        ProtectedInternal = 0;
        Private = 0;
        PrivateProtected = 0;
    }
}

internal class Derived : Base
{
    public void MethodInDerived()
    {
        // 同一アセンブリ内の派生クラス
        // コメントアウトしてないやつだけ OK
        Public = 0;
        Protected = 0;
        Internal = 0;
        ProtectedInternal = 0;
        //Private = 0;
        PrivateProtected = 0;
    }
}

internal class OtherClass
{
    public void Method()
    {
        // 同一アセンブリ内の他のクラス
        // コメントアウトしてないやつだけ OK
        var x = new Base();

        x.Public = 0;
        //x.Protected = 0;
        x.Internal = 0;
        x.ProtectedInternal = 0;
        //x.Private = 0;
        //x.PrivateProtected = 0;
    }
}
```

このコードとは別のプロジェクト内では、以下のような制限がかかります。

```C#
public class Derived : ClassLibrary1.Base
{
    public void MethodInDerived()
    {
        // 他のアセンブリ内の派生クラス
        // コメントアウトしてないやつだけ OK

        Public = 0;
        Protected = 0;
        //Internal = 0;
        ProtectedInternal = 0;
        //Private = 0;
        //PrivateProtected = 0; // ここが protected internal との差
    }
}

internal class OtherClass
{
    public void Method()
    {
        // 他のアセンブリ内の他のクラス
        // public 以外全滅

        var x = new ClassLibrary1.Base();

        x.Public = 0;
        //x.Protected = 0;
        //x.Internal = 0;
        //x.ProtectedInternal = 0;
        //x.Private = 0;
        //x.PrivateProtected = 0;
    }
}
```

ちなみに、protected internal と private protected では、語順は自由です。  
protected internalとinternal protected、private protectedとprotected privateはそれぞれ同じ意味になります。  

```C#
// どちらの順序でも同じ意味
protected internal int A1;
internal protected int A2;

private protected int B1;
protected private int B2;
```
