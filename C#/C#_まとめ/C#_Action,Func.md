# ラムダ式(匿名関数)

---

## デリゲートとは？

>メソッドを参照するための型  
C++の関数ポインターのようなもの  
関数ポインターや関数オブジェクトをオブジェクト指向に適するように拡張したもの  
他のメソッドに処理を丸投げするためのオブジェクト  
[未確認飛行C](https://ufcpp.net/study/csharp/sp_delegate.html)  

「メソッドを格納するための型」といえるかもしれない。  

基本的な使い方  
`宣言⇒インスタンス化⇒メソッド代入`

``` C#
// ①宣言
delegate void del_func(int val);

public static void Main(){
    // ②インスタンス化
    // ③メソッド代入
    del_func hoge = new del_func(hoge_func);

    hoge.Invoke();
}

// 代入する関数
static void hoge_func(int val){
    _ = val;
}
```

---

## Action,Func

Action,Funcは通常のdelegateを使いやすくした型。  
通常のdelegateような宣言が必要なくなった。  

delegateの基本的な使い方 : `宣言⇒インスタンス化⇒メソッド代入` →面倒くさい。  
Action : ``  

``` C#
public static void Main(){
    Action<int> hoge = hoge_func;

    hoge.Invoke();
}

// 代入する関数
static void hoge_func(int val){
    _ = val;
}
```

[【C#】delegate, Action, Funcについて 多分一番易しい解説](https://hikotech.net/post-449/)  

---

## 匿名関数の即時実行

VB.Netの時、usingした結果だけを受け取りたい場合によく利用したが、C#になってからあまり使っていなかった。  
でもって、すぐにやり方がわからなかったのでまとめ。  

``` C# : 最も単純な例
int x = new Func<int>(() => 1)();

new Action(() => 処理)();
```

``` C#
try
{
    // デシリアライズしたデータを入れる、入れ物です
    Account account = null;
    // jsonファイルを読み込みます
    using (StreamReader file = File.OpenText(@"C:\test.json"))
    {
        account = (Account)new JsonSerializer().Deserialize(file, typeof(Account));
    }
    // 略
}
```

``` C#
try
{
    var account = new Func<Account>(() =>
    {
        using StreamReader file = File.OpenText(@"C:\test.json");
        return (Account)new JsonSerializer().Deserialize(file, typeof(Account));
    })();

    Func<Account?> account = () =>
    {
        using StreamReader file = File.OpenText(@"C:\test.json");
        return (Account)new JsonSerializer().Deserialize(file, typeof(Account));
    };
    _ = account();
}
```

タプルでの受け取りは無理だった。  
エラーにはならなかったが、全部初期値が入って使い物にならなかった。  
匿名型はそもそも型だけを定義するってのが無理なので選択肢に上がらない。  
これは地味に知らなかったので、別でまとめる。  

[このツイート](https://twitter.com/neuecc/status/1430737738593017859)で匿名関数の即時実行は無理って言ってるけど、できたんだよな。
![aa](https://pbs.twimg.com/media/E9sAV0jVoAEHVUs?format=png&name=360x360)  

``` C#
public class Foo
{
    public int X { get; set; }
    public int Y { get; set; }

    public Foo(int x , int y)
    {
        // 即時実行無理なので、ローカル関数で定義して、直後に実行。
        async void Init(){
            await Task.Delay(1000);
            X = x;
        }
        Init();

        // Actionをnewすれば即時実行できるよ。
        new Action(async () =>
        {
            await Task.Delay(1000);
            X = x;
        })();

        Y = y;
    }
}
```

[C#でJavascriptみたいな即時関数を実行する](https://yuzutan-hnk.hatenablog.com/entry/2017/01/15/022643)  

---

## 匿名関数の即時実行でyield returnは使えない

>yield ステートメントは、匿名メソッドまたはラムダ式の内部では使用できません。  
[コンパイラ エラー CS1621](https://docs.microsoft.com/ja-jp/dotnet/csharp/misc/cs1621)  

実務で、「これくらいなら即時関数でまとめたほうがきれいだな」って思ったやつがあって、「Enumerableで返すならyieldだっけ？」ってことで実装したらエラーになった。  

後日調査した結果、ローカル関数だといける。  
謎である。  

``` C#
// ローカル関数 ○
IEnumerable<int> Generate()
{
    for (int i = 0; i < 10; i++)
    {
        yield return i;
    }
}

// 匿名関数 ×
var aa = new Func<IEnumerable<int>>(() =>
{
    for (int i = 0; i < 10; i++)
    {
        // コンパイラ エラー CS1621
        // yield ステートメントは、匿名メソッドまたはラムダ式の内部では使用できません
        yield return i;
    }
})();
```

どうしても匿名関数でやりたいならこうしないといけないみたい。  
やる意味はない。  
どうしてもやるにしても、LinqのSelect内部で頑張るべき。  

``` C#
public class Test
{
    // 逐次処理をする関数オブジェクト
    private static Func<IEnumerable<int>> iterateFunc;
    // 1 メソッドによる実装
    private static IEnumerable<int> Iterate()
    {
        for (int i = 0; i < 10; ++i)
        {
            Thread.Sleep(1000);
            yield return i;
        }
    }
    // スタティックコンストラクタ
    static Test()
    {
        // 2 【コンパイルエラー】ラムダ式バージョン
        //iterateFunc = () =>
        //    {
        //        for (int i = 0; i < 10; ++i)
        //        {
        //            Thread.Sleep(1000);
        //            yield return i;
        //        }
        //    };
        iterateFunc = new Func<IEnumerable<int>>(Iterate); 
    }
}
```

[コンパイラ エラー CS1621](https://docs.microsoft.com/ja-jp/dotnet/csharp/misc/cs1621)  
[イテレータはラムダ式、匿名メソッド内では使えない](http://blogs.wankuma.com/gshell/archive/0001/01/01/IteratorInFunc.aspx)  
[In C#, why can't an anonymous method contain a yield statement?](https://stackoverflow.com/questions/1217729/in-c-why-cant-an-anonymous-method-contain-a-yield-statement)  

---

## Actionとローカル関数のどちらを使うべきか？

ローカル関数のほうが必要な処理が少ないので、特段の理由が無ければローカル関数を使うべし。  
ローカル関数は2ステップだが、Actionは4ステップ必要。  
Actionを定義してから実行するのと、Actionをnewして即時実行する時のIL(中間言語)は同じ。  

[sharplab.io](https://sharplab.io)で以下のコードを入力する。  

``` cs
public class C 
{
    public void M() 
    {
        Action a =()=>{ Console.WriteLine("hoge"); };
        a();
    }
    
    public void N()
    {
        void b(){ Console.WriteLine("hoge"); };
        b();
    }
    
    public void O()
    {
        new Action(() => Console.WriteLine("hoge")).Invoke();
    }
}
```

上記コードによる[sharplab.io](https://sharplab.io/#v2:EYLgHgbALAPgAgJgIwFgBQ64GYAEicDCO6A3ujhXrnFDgLIAUAlMWpTmW+5XEgjgEMcAXmbCAfCTxIAnAwBEACwD2AcwCm8pgG4cAX23luFAc13HW7PUYo2qeWgDlmdzhYc5gzKbzlK1mjr6hlzGXjp21qGWPNS0APIu0W7uAHbqAO7SCAzMIuLSfioaWkwAdACSqQBuygDW6maUkezoUUA=)でのILは以下の通り。  

``` cs
public class C
{
    [Serializable]
    [CompilerGenerated]
    private sealed class <>c
    {
        public static readonly <>c <>9 = new <>c();

        public static Action <>9__0_0;

        public static Action <>9__2_0;

        internal void <M>b__0_0()
        {
            Console.WriteLine("hoge");
        }

        internal void <O>b__2_0()
        {
            Console.WriteLine("hoge");
        }
    }

    public void M()
    {
        (<>c.<>9__0_0 ?? (<>c.<>9__0_0 = new Action(<>c.<>9.<M>b__0_0)))();
    }

    public void N()
    {
        <N>g__b|1_0();
    }

    public void O()
    {
        (<>c.<>9__2_0 ?? (<>c.<>9__2_0 = new Action(<>c.<>9.<O>b__2_0)))();
    }

    [CompilerGenerated]
    internal static void <N>g__b|1_0()
    {
        Console.WriteLine("hoge");
    }
}
```

[C#のActionとローカル関数のどちらを使うべきか調査](https://shibuya24.info/entry/action_or_local_method)  

---

## デリゲートにはローカル関数を代入可能

当時は珍しく感じたが、デリゲートは関数を格納する型なので、普通に考えていける。  

``` C#
    {
        // ローカル関数
        IEnumerable<int> GetIntList(IEnumerable<string> stringList)
            => this.GetIntList(stringList, arg1, arg2, arg3, arg4);

        // デリゲート,第2引数にローカル関数を渡す
        var result = GetGroupSettlementSet(customer, GetIntList);
    }

    private Result GetPackSettlementSet(
        Customer customer,
        Func<IEnumerable<string>, IEnumerable<int>> getIntList)
    {
        // この処理の中で文字列一覧を生成
        var stringList = new List<string>();
        // デリゲートの実行
        var intList = getIntList(stringList);
    }

    private IEnumerable<int> GetIntList(
        IEnumerable<string> stringList,
        string arg1,string arg2,string arg3,string arg4)
    {
    }
```

---

## デリゲートのターゲット推論

三項演算子でActionの内容を切り替えたい場合、宣言で明確にActionを定義していても、エラーとなってしまう。  
9.0では解決されているらしいが、8.0以下でも一応書ける事を発見したので残しておく。  

愚直にやるならこうなる。

``` C#
bool flag = true;
Action<int, string> modeAction = null;
if (flag)
{
    modeAction = (id, name) => Console.WriteLine($"{id} {name} 様");
}
else
{
    modeAction = (id, name) => Console.WriteLine($"{id} {name} さん");
}
```

三項演算子で分岐させるとエラーになる。

``` C#
// 型定義
Action<int, string> modeAction = flag
    ? (id, name) => Console.WriteLine($"{id} {name} 様")
    : (id, name) => Console.WriteLine($"{id} {name} さん");

// キャスト
Action<int, string> modeAction = (Action<int, string>)(flag
    ? (id, name) => Console.WriteLine($"{id} {name} 様")
    : (id, name) => Console.WriteLine($"{id} {name} さん"));

// CS8957:ラムダ式とラムダ式の間に共通の型が見つからないため、言語バージョン8.0で条件式が無効です。
// ターゲットにより型指定された変換を使用するには、言語バージョン9.0以上にアップグレードしてください。
```

片方で良いので、`new Action`するかActionでキャストするといける。  
不格好ではあるが、出来なくはないのでお好みでって感じではある。  

``` C#
// new Action案
Action<int, string> modeAction = flag
    ? new Action<int, string>((id, name) => Console.WriteLine($"{id} {name} 様"))
    : (id, name) => Console.WriteLine($"{id} {name} さん");

// Actionキャスト案
Action<int, string> modeAction = flag
    ? (Action<int, string>)((id, name) => Console.WriteLine($"{id} {name} 様"))
    : (id, name) => Console.WriteLine($"{id} {name} さん");

// 上記2点ならvarでも問題ない
var modeAction = flag
    ? new Action<int, string>((id, name) => Console.WriteLine($"{id} {name} 様"))
    : (id, name) => Console.WriteLine($"{id} {name} さん");
var modeAction = flag
    ? (Action<int, string>)((id, name) => Console.WriteLine($"{id} {name} 様"))
    : (id, name) => Console.WriteLine($"{id} {name} さん");
```

即時実行も出来たりする。  
条件分岐して即時実行するサンプルはswitch式の方でもやってたのでそちらも参考にされたし。  

``` C#
// こういうこともできるよ
new Action<int, string>(flag
    ? (Action<int, string>)((id, name) => Console.WriteLine($"{id} {name} 様"))
    : (id, name) => Console.WriteLine($"{id} {name} さん")).Invoke(1,"2");
(flag
    ? (Action<int, string>)((id, name) => Console.WriteLine($"{id} {name} 様"))
    : (id, name) => Console.WriteLine($"{id} {name} さん")).Invoke(1,"2");
```

---

## ループ処理においてローカル関数を定義した場合のパフォーマンスへの影響

foreachの中でローカル関数を定義するのと、foreach外でローカル関数を定義するのとで何か違うのか調査。  
ぱっと見では、ループ中に定義処理を書くと、何回も再定義されそうに見える。  

結論としては問題ないと思われる。  
ローカル関数は、中間言語では、そのクラスにおけるstaticなメソッドとして定義され直すので、ループ中に記述しても問題ない。  
そして、どちらの場合でも同じ中間言語となったので、ループ中で定義してもパフォーマンスへの影響はないように見える。  

``` cs
using System;

public class Class1
{
    public void Method()
    {
        var strings = new System.Collections.Generic.List<string>(){
            "Hoge",
            "Fuga",
            "Piyo"
        };
        foreach (var str in strings) 
        {
            void innerHoge(string arg)
            {
                Console.WriteLine(arg);
            }

            innerHoge(str);
        }
    } 
}

public class Class2 {
    public void Method()
    {
        var strings = new System.Collections.Generic.List<string>(){
            "Hoge",
            "Fuga",
            "Piyo"
        };

        void outerHoge(string arg)
        {
            Console.WriteLine(arg);
        }

        foreach (var str in strings) 
        {
            outerHoge(str); 
        }
    }
}
```

生成された中間言語。  
[difff](https://difff.jp/)などで比較してみると、クラス名やメソッド名以外の違いが確認できない。
そして、ローカル関数は一番最後のブロックでstaticなクラスメソッドとして定義されているのが分かる。  

[sharplab.ioでのサンプル](https://sharplab.io/#v2:EYLgxg9gTgpgtADwGwBYA0AXEBLANgHwAEAmARgFgAoKwgZgAIT6BhXAQwGcOLKBvK+oMYNCKegFkYGABYQAJgAoAlAKH9KQzfQBubKI1IAGDvQC89AHYwA7gZQA6ADLYOGADyEjAPmXqt/+gAiAAkIAHMYQLRVAKFAgDEAVzC2KJjYoIAFbABPCED0zQBfAG5CoQAzaBg2MGl6BV19V31sCwNjJXpywT8MwVF6NqsoUIiFT0N6PTCVDX76PoWhTwBOBRmlMvn+oqoerWGYUfCYBRatg72d+iLuymuaEWIWdi4XpeFGMUkZeWV0p9NE0OiZzFZbJ4HM5XB5vL4DpoQqc0jdYglkqlomiAoFsnkCjjBKV9kTvvQIIkMMcxmdJtMoLMDkCAmsNozLmTHmSqrBavVGnp6C0hu1JhwuszEUJKdSTuMLiV7rFrsUqEUgA)  

<div class="column-left">

``` cs
.class public auto ansi beforefieldinit Class1
    extends [System.Runtime]System.Object
{
    // Methods
    .method public hidebysig 
        instance void Method () cil managed 
    {
        // Method begins at RVA 0x20a0
        // Code size 84 (0x54)
        .maxstack 3
        .locals init (
            [0] valuetype [System.Collections]System.Collections.Generic.List`1/Enumerator<string>
        )

        IL_0000: newobj instance void class [System.Collections]System.Collections.Generic.List`1<string>::.ctor()
        IL_0005: dup
        IL_0006: ldstr "Hoge"
        IL_000b: callvirt instance void class [System.Collections]System.Collections.Generic.List`1<string>::Add(!0)
        IL_0010: dup
        IL_0011: ldstr "Fuga"
        IL_0016: callvirt instance void class [System.Collections]System.Collections.Generic.List`1<string>::Add(!0)
        IL_001b: dup
        IL_001c: ldstr "Piyo"
        IL_0021: callvirt instance void class [System.Collections]System.Collections.Generic.List`1<string>::Add(!0)
        IL_0026: callvirt instance valuetype [System.Collections]System.Collections.Generic.List`1/Enumerator<!0> class [System.Collections]System.Collections.Generic.List`1<string>::GetEnumerator()
        IL_002b: stloc.0
        .try
        {
            // sequence point: hidden
            IL_002c: br.s IL_003a
            // loop start (head: IL_003a)
                IL_002e: ldloca.s 0
                IL_0030: call instance !0 valuetype [System.Collections]System.Collections.Generic.List`1/Enumerator<string>::get_Current()
                IL_0035: call void Class1::'<Method>g__innerHoge|0_0'(string)

                IL_003a: ldloca.s 0
                IL_003c: call instance bool valuetype [System.Collections]System.Collections.Generic.List`1/Enumerator<string>::MoveNext()
                IL_0041: brtrue.s IL_002e
            // end loop

            IL_0043: leave.s IL_0053
        } // end .try
        finally
        {
            // sequence point: hidden
            IL_0045: ldloca.s 0
            IL_0047: constrained. valuetype [System.Collections]System.Collections.Generic.List`1/Enumerator<string>
            IL_004d: callvirt instance void [System.Runtime]System.IDisposable::Dispose()
            IL_0052: endfinally
        } // end handler

        IL_0053: ret
    } // end of method Class1::Method

    .method public hidebysig specialname rtspecialname 
        instance void .ctor () cil managed 
    {
        // Method begins at RVA 0x2110
        // Code size 7 (0x7)
        .maxstack 8

        IL_0000: ldarg.0
        IL_0001: call instance void [System.Runtime]System.Object::.ctor()
        IL_0006: ret
    } // end of method Class1::.ctor

    .method assembly hidebysig static 
        void '<Method>g__innerHoge|0_0' (
            string arg
        ) cil managed 
    {
        .custom instance void System.Runtime.CompilerServices.NullableContextAttribute::.ctor(uint8) = (
            01 00 01 00 00
        )
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
            01 00 00 00
        )
        // Method begins at RVA 0x2118
        // Code size 7 (0x7)
        .maxstack 8

        IL_0000: ldarg.0
        IL_0001: call void [System.Console]System.Console::WriteLine(string)
        IL_0006: ret
    } // end of method Class1::'<Method>g__innerHoge|0_0'

} // end of class Class1
```

</div>
<div class="column-right">

``` cs
.class public auto ansi beforefieldinit Class2
    extends [System.Runtime]System.Object
{
    // Methods
    .method public hidebysig 
        instance void Method () cil managed 
    {
        // Method begins at RVA 0x2120
        // Code size 84 (0x54)
        .maxstack 3
        .locals init (
            [0] valuetype [System.Collections]System.Collections.Generic.List`1/Enumerator<string>
        )

        IL_0000: newobj instance void class [System.Collections]System.Collections.Generic.List`1<string>::.ctor()
        IL_0005: dup
        IL_0006: ldstr "Hoge"
        IL_000b: callvirt instance void class [System.Collections]System.Collections.Generic.List`1<string>::Add(!0)
        IL_0010: dup
        IL_0011: ldstr "Fuga"
        IL_0016: callvirt instance void class [System.Collections]System.Collections.Generic.List`1<string>::Add(!0)
        IL_001b: dup
        IL_001c: ldstr "Piyo"
        IL_0021: callvirt instance void class [System.Collections]System.Collections.Generic.List`1<string>::Add(!0)
        IL_0026: callvirt instance valuetype [System.Collections]System.Collections.Generic.List`1/Enumerator<!0> class [System.Collections]System.Collections.Generic.List`1<string>::GetEnumerator()
        IL_002b: stloc.0
        .try
        {
            // sequence point: hidden
            IL_002c: br.s IL_003a
            // loop start (head: IL_003a)
                IL_002e: ldloca.s 0
                IL_0030: call instance !0 valuetype [System.Collections]System.Collections.Generic.List`1/Enumerator<string>::get_Current()
                IL_0035: call void Class2::'<Method>g__outerHoge|0_0'(string)

                IL_003a: ldloca.s 0
                IL_003c: call instance bool valuetype [System.Collections]System.Collections.Generic.List`1/Enumerator<string>::MoveNext()
                IL_0041: brtrue.s IL_002e
            // end loop

            IL_0043: leave.s IL_0053
        } // end .try
        finally
        {
            // sequence point: hidden
            IL_0045: ldloca.s 0
            IL_0047: constrained. valuetype [System.Collections]System.Collections.Generic.List`1/Enumerator<string>
            IL_004d: callvirt instance void [System.Runtime]System.IDisposable::Dispose()
            IL_0052: endfinally
        } // end handler

        IL_0053: ret
    } // end of method Class2::Method

    .method public hidebysig specialname rtspecialname 
        instance void .ctor () cil managed 
    {
        // Method begins at RVA 0x2110
        // Code size 7 (0x7)
        .maxstack 8

        IL_0000: ldarg.0
        IL_0001: call instance void [System.Runtime]System.Object::.ctor()
        IL_0006: ret
    } // end of method Class2::.ctor

    .method assembly hidebysig static 
        void '<Method>g__outerHoge|0_0' (
            string arg
        ) cil managed 
    {
        .custom instance void System.Runtime.CompilerServices.NullableContextAttribute::.ctor(uint8) = (
            01 00 01 00 00
        )
        .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
            01 00 00 00
        )
        // Method begins at RVA 0x2118
        // Code size 7 (0x7)
        .maxstack 8

        IL_0000: ldarg.0
        IL_0001: call void [System.Console]System.Console::WriteLine(string)
        IL_0006: ret
    } // end of method Class2::'<Method>g__outerHoge|0_0'

} // end of class Class2
```

</div>

<style>
.column-left{
  float: left;
  width: 47.5%;
  text-align: left;
}
.column-right{
  float: right;
  width: 47.5%;
  text-align: left;
}
</style>
