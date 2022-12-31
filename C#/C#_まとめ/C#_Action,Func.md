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

[C#でJavascriptみたいな即時関数を実行する](https://yuzutan-hnk.hatenablog.com/entry/2017/01/15/022643)  

タプルでの受け取りは無理だった。
エラーにはならなかったが、全部初期値が入って使い物にならなかった。
匿名型はそもそも型だけを定義するってのが無理なので選択肢に上がらない。  
これは地味に知らなかったので、別でまとめる。

<https://twitter.com/neuecc/status/1430737738593017859>
このツイートで匿名関数の即時実行は無理って言ってるけど、できたんだよな。
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

---

## 匿名関数の即時実行でyield returnは使えない

[コンパイラ エラー CS1621](https://docs.microsoft.com/ja-jp/dotnet/csharp/misc/cs1621)  
>yield ステートメントは、匿名メソッドまたはラムダ式の内部では使用できません。  

[In C#, why can't an anonymous method contain a yield statement?](https://stackoverflow.com/questions/1217729/in-c-why-cant-an-anonymous-method-contain-a-yield-statement)  

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

[イテレータはラムダ式、匿名メソッド内では使えない](http://blogs.wankuma.com/gshell/archive/0001/01/01/IteratorInFunc.aspx)  

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

---

## Actionとローカル関数のどちらを使うべきか調査

ローカル関数のほうが必要な処理が少ないので、特段の理由が無ければローカル関数を使うべし。  

ローカル関数は2ステップだが、Actionは4ステップ必要。  

Actionを定義してから実行するのと、Actionをnewして即時実行する時のILは同じ。  

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

[sharplab.io](https://sharplab.io/#v2:EYLgHgbALAPgAgJgIwFgBQ64GYAEicDCO6A3ujhXrnFDgLIAUAlMWpTmW+5XEgjgEMcAXmbCAfCTxIAnAwBEACwD2AcwCm8pgG4cAX23luFAc13HW7PUYo2qeWgDlmdzhYc5gzKbzlK1mjr6hlzGXjp21qGWPNS0APIu0W7uAHbqAO7SCAzMIuLSfioaWkwAdACSqQBuygDW6maUkezoUUA=)  

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
