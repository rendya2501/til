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

ローカル関数のほうが早いので、特段の理由が無ければローカル関数を使うべし。  

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
