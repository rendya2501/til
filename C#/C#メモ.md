# C#メモ

---

## インスタンスの状態

DependencyPropertyのBindingの件でazmさんがインスタンスの状態なんて事を言っていたので調べたわけだが、それとは別にインスタンスについての基礎を紹介しているページがわかりやすかったのでまとめる。  

後日、azmさんがまた使っていたので、意味合いから想像すると、例えばLinqで.Where()を実行したときと、.ToList()を実行したときでは、戻ってくる値が違うわけで、  
azmさんはどうやら帰ってくる型の事をインスタンスの状態と言っている見たいだ。  

[[C# 入門] クラスのインスタンスについて](https://yaspage.com/prog/csharp/cs-instance/)  

---

## varの意味

型を同じにしてくれる。  
暗黙の型指定  
コンパイラは右側の値からデータ型を推測して決定します。  
この仕組みを「型推論」と呼びます。  

---

## 仮想メソッド

virtual修飾子をつけたメソッドのこと。  
主に親クラスで定義するメソッド。  
これがあると、子クラスでoverride修飾子を使うことで処理を上書きできる。  
実装は強制ではないので、virtualがついていても別に何もしなければ親クラスの処理が実行されるだけ。  
インスタンスによって異なる動きを実現する多態性(ポリモーフィズム)を体現する機能。  

2021/10/09 Sat  
MultiSelectComboBoxを実装するにあたって、C1MultiSelectとかXceedのCheckComboBoxとか継承してカスタムする機会がたくさんあったのでまとめ。  
親クラスを継承なんて、こういう機会じゃないと滅多にないから、今まで触れる機会がなかった。  
わかってしまえば大したことないのだがな。  

---

## オーバーロード

引数の型の違いでは、オーバーロード出来ないと思ってたけど、普通に出来たわ。  
わざわざまとめる必要はないだろうけど、一応ね。  

``` C#
public class Hello{
    public static void Main(){
       Print pri = new Print( ); //オブジェクト作成
       pri.maisu( 5 ); //メソッド(1)呼び出し
       pri.maisu( 'a' ); //メソッド(2)呼び出し
    }
}
public class Print
{
    // メソッド(1)の処理
    public void maisu( int a ) => System.Console.WriteLine(a);
    // メソッド(2)の処理
    public void maisu( char a ) => System.Console.WriteLine(a);
}
```

---

## アノテーションとアトリビュート

Javaではアノテーションと呼び、C#,.Netではアトリビュートと呼ぶらしい。  

調べた感じ、C#ではどちらもヒットするので意味的には同じっぽい。  
しかし、実装としてはAttributeClassとして定義されているので、C#ならアトリビュートのほうが強いかも。  

・Annotation : 注釈  
・Attribute : 属性  

<https://elf-mission.net/programming/wpf/episode09/>  
[アノテーション](http://wisdom.sakura.ne.jp/programming/java/java5_9.html)  

■**よくわかっていなかったときの認識**  

恐らくではあるが、属性やバリデーションの為にクラスやフィールドの宣言の上に[]で囲うやつの事全般をこう読んでいるのではないか?  
調べてもそういうのしか出てこなかった。  

---

## 破棄

<https://ufcpp.net/study/csharp/cheatsheet/ap_ver7/#discard>  

型スイッチや分解では、変数を宣言しつつ何らかの値を受け取るわけですが、 特に受け取る必要のない余剰の値が生まれたりします。  
例えば、分解では、複数の値のうち、1つだけを受け取りたい場合があったとします。 こういう場合に、_を使うことで、値を受け取らずに無視することができます。  

``` C#
static (int quotient, int remainder) DivRem(int dividend, int divisor)
    => (Math.DivRem(dividend, divisor, out var remainder), remainder);
static void Deconstruct()
{
    // 商と余りを計算するメソッドがあるけども、ここでは商しか要らない
    // _ を書いたところでは、値を受け取らずに無視する
    var (q, _) = DivRem(123, 11);

    // 逆に、余りしか要らない
    // また、本来「var x」とか変数宣言を書くべき場所にも _ だけを書ける
    (_, var r) = DivRem(123, 11);
}
```

同様の機能は、型スイッチや出力変数宣言でも使えます。  

---

## 名前付き引数 C#7.0

[名前付き引数](https://ufcpp.net/study/csharp/sp4_optional.html)  

なんてことはない。引数の名前をわかりやすくするだけのやつ。  
オプショナルがあっても、全部指定する必要がないっぽい。  
左からnull,false,nullなんてする必要がないので、オプショナルが複数ある場合は便利かも。  
後、src,dstが分かりにくい時とか、呼び出すときにわかりやすくなるのでそういう時も便利。  

``` C#
_ = Sum(x: 1, y: 2, z: 3); // Sum(1, 2, 3); と同じ意味。
_ = Sum(y: 1, z: 2, x: 3); // Sum(3, 1, 2); と同じ意味。
_ = Sum(y: 1);             // Sum(0, 1, 0); と同じ意味。
_ = Sum(1, z: 2, y: 3);    // OK: 前の方は位置指定、後ろの方は名前指定
_ = Sum(1, x: 2, y: 3);    // コンパイル エラー: 前の方の引数を名前指定するのはダメ
_ = Sum(x: 1, 2, 3);       // C# 7.2, 末尾以外でも名前を書けるように
_ = Sum(2, 3, x: 1);       // C# 7.2 でもダメなやつ。末尾以外の引数を名前付きにしたい場合、順序は厳守する必要あり
int Sum(int x = 0 , int y = 0, int z = 0) => x + y + z;
```

要するに、引数の省略や順序変更を目的としているのではなく、 単に「どの実引数が何の意味か」が名前からわかるようにしたいときに使うものです。
例えば、よくある話だと、「Copy(a, b, length)では、aとbのどちらがコピー元でどちらがコピー先かがわからなくて困る」といった問題があったりします。 この際に、以下のように書ければ便利だろうということで名前付き引数の制限が緩和されました。

なんでいまさら？
→多少のリスクがあるから  
一番の問題は、後から名前や値を変えにくい(変えると利用側コードを壊す)という点です。  

また、コンパイルの時にも色々あるみたい。  
詳しくはリンク先参照。今は困ってないのでそこまでまとめる気力がない。  

・引数名や規定値は後から変えると影響でかい。  
・仮想メソッドに対して規定値を与えると混乱の元。  

---

## Cysharp

C#の可能性を切り開いていく専門会社が作ってるOSSライブラリ  

コンソールアプリでCLIプログラムのひな形を紹介している。  
[github_Cysharp](https://github.com/Cysharp/ConsoleAppFramework)  

---

## Parallel.For, Parallel.ForEach

デフラグっぽいグラフィカルな表現で並列処理が確認できて面白い。  
[(C#)Parallel.For, Parallel.ForEach並列処理の挙動確認](https://qiita.com/longlongago_k/items/8f19d84fce6dd677922e)  

---

## PLINQ(Parallel LINQ)

LINQの並列実行版  

---

## メモリ

・変数はメモリのアドレスに名前をつけている？値を入れることで始めてメモリが確保される？それとも変数名を定義した瞬間メモリが確保される？
メモリに名前をつけただけで領域は確保されない？

変数はメモリのアドレスと紐づける。
C#で変数を宣言するだけで使わない場合、コンパイラが宣言しなかったときと同じ扱いにしてくれるのではなかろうか。
だから、使うなら領域を確保するし、しないなら宣言していないのと同じ状態としてくれるのではなかろうか。
そこらへんはコンパイラのさじ加減によるだろう。
親切なコンパイラならわざわざ確保する事はないだろうし、そうでないならアドレスと紐づけはしつつ、内部には何も入れないみたいな感じになるのでは？

いや、違うな。
変数を宣言した場合、そのアドレスと紐づけはするけど、その中身は操作しないことになるのか？
だからコンパイルの段階で紐づけをしないようにしてもらわないと、そのアドレスにアクセスできないことになる可能性があるのか。

まぁ、使わない変数を定義したところで、使っていないのだから定義すべきではないし、それだけの話だった気がする。  

``` C#
    static void Main(string[] args)
    {
        System.Diagnostics.Process hProcess = System.Diagnostics.Process.GetCurrentProcess();

        //Console.WriteLine("BaseAddress     :0x" + Convert.ToString(hProcess.MainModule.BaseAddress.ToInt64(), 16));
        //Console.WriteLine("EntryPoint      :0x" + Convert.ToString(hProcess.MainModule.EntryPointAddress.ToInt64(), 16));
        //Console.WriteLine("ModuleMemorySize:" + hProcess.MainModule.ModuleMemorySize);
        //Console.WriteLine("WorkingSet      :" + hProcess.WorkingSet64);

        //hProcess.Refresh();
        //Console.WriteLine($"物理メモリ使用量: {hProcess.WorkingSet64}\r\n仮想メモリ使用量: {hProcess.VirtualMemorySize64}");
        ////int a;
        //hProcess.Refresh();
        //Console.WriteLine($"物理メモリ使用量: {hProcess.WorkingSet64}\r\n仮想メモリ使用量: {hProcess.VirtualMemorySize64}");

        Console.WriteLine($"現在のメモリ使用量は{Environment.WorkingSet:N0}byteです。");
        Console.WriteLine($"現在のメモリ使用量は{Environment.WorkingSet:N0}byteです。");

        Console.WriteLine($"現在のメモリ使用量は{Environment.WorkingSet:N0}byteです。");
        int a = 0;
        int j;
        Console.WriteLine($"{a:N0}");
        Console.WriteLine($"現在のメモリ使用量は{Environment.WorkingSet:N0}byteです。");

        // aaaaa();
    }

    // unsafe句を使う場合はデバッグオプションでunsafeを許可する必要がある。
    // まぁ、unsafeって書いて「ctrl + .」でヒント出てくるはずだからそれ使えばよろしい。
    static unsafe void aaaaa()
    {
        int a = 100;
        int* b = &a;

        Console.WriteLine(Convert.ToString((int)&a, 16));
        Console.WriteLine("0x" + Convert.ToString((int)b, 16));

        Console.Read();
    }
```

[[変数] 変数とメモリ](https://zenn.dev/antez/books/568dd4d86562a1/viewer/09848c)  
[プログラムがメモリをどう使うかを理解する(1)](https://zenn.dev/rita0222/articles/e6ff75245d79b5)  
[プログラムがメモリをどう使うかを理解する(2)](https://zenn.dev/rita0222/articles/beda4311d9a6bf)  
[プログラムがメモリをどう使うかを理解する(3)](https://zenn.dev/rita0222/articles/f59b79bab45a2a)  
[プログラムがメモリをどう使うかを理解する(4)](https://zenn.dev/rita0222/articles/1f37a5bf910282)  
<https://twitter.com/404death/status/968381431146778624/photo/1>  

---

## 親クラスを実装した子クラスを親クラスの引数で渡しても全てのプロパティが入ってくる

まぁ、それだけなんですけどね。  
親クラスの部分だけでも自動代入出来ればと思ったが、結局リフレクション使ってループして入れるしかない。  

``` cs
using System.Reflection;

var parent = new Child()
{
    Id = 1,
    Name = "name",
    ChildProp = "child1prop"
};
var excon = new ExtendChild(parent);


public class Parent
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
}

public class Child : Parent
{
    public string ChildProp { get; set; }
}

public class ExtendChild : Parent
{
    public string DateTime { get; set; }

    // parentには「Id」「Name」「ChildProp」が入ってくる
    // Parent型なのにChildのプロパティまで含まれる
    // ChildProp [string]:"child1prop"
    // Id [int]:1
    // Name [string]:"name"
    public ExtendChild(Parent parent)
    {
        // 親クラスのプロパティ情報を一気に取得して使用する。
        List<PropertyInfo> props = parent
            .GetType()
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .ToList();
        foreach (var prop in props)
        {
            var propValue = prop.GetValue(parent);
            typeof(ExtendChild).GetProperty(prop.Name)?.SetValue(this, propValue);
        }
    }
}
```

---

## C# if or

C# 9.0(.Net5)からパターンマッチングによって可能になった。  

``` cs
if (i is 2 or 3 or 5 or >= 10){}
```

[【C#】複数のORをまとめて書けないか](https://shozo-gson.com/cs-alt-or/)  

---

## バリデーションチェックに関するあれこれ

愚直に実装するならこうなる。  

``` cs
/// <summary>
/// 実行前バリデーション
/// </summary>
/// <param name="targetList"></param>
/// <returns></returns>
private bool ExecuteValidation(IEnumerable<ListView> targetList)
{
    // リスト事態が存在しない場合 or 1件も指定されていない場合、処理終了
    if (!targetList?.Any() ?? true)
    {
        return false;
    }
    // 何も条件が指定されていない場合、やる意味がないので塞ぐ
    if (!Condition1.HasValue && !Condition2.HasValue && !Condtion3.HasValue)
    {
        MessageDialogUtil.ShowWarning(Messenger, "変更内容が指定されていません。");
        return false;
    }
    // 無効な組み合わせの場合
    if (Condtion2.HasValue && !Condtion1.HasValue)
    {
        MessageDialogUtil.ShowWarning(Messenger, "条件2を指定する場合、条件1も指定してください。");
        return false;
    }

    return true;
}
```

普通のチェック処理なのでいうことはない。  
しかし野暮ったい。で、思った。  

仕組み的に、条件に合致したらメッセージを返却するだけなので、それなら条件とメッセージをリストとして定義しておくことはできないか？と。  

■**改良版**  

まずフィールド変数として条件を定義してしまう。  
メッセージがあれば、warningをshowするだけ。  
2回もフィールドにアクセスしているので、その点が少し心配ではある。  
もう少しいい書き方がありそうな気がするが、思いついたらにしよう。  

``` cs
// 最初に引っかかったものを表示するならfirstを取ればよい。
private string ExecuteErrMsg => new List<(bool flag, string msg)>()
{
    // 何も条件が指定されていない場合、やる意味がないので塞ぐ
    (!Condition1.HasValue && !Condition2.HasValue && !Condtion3.HasValue , "変更内容が指定されていません。" ),
    // 無効な組み合わせの場合
    (Condtion2.HasValue && !Condtion1.HasValue,"条件2を指定する場合、条件1も指定してください。" )
}.FirstOrDefault(f => f.flag).msg;

/// <summary>
/// 実行前バリデーション
/// </summary>
/// <returns></returns>
private bool ShowWarning(string errMsg)
{
    // 1件でもあればアナウンスする
    return string.IsNullOrEmpty(errMsg) ||
        // awaitしていないので、ダイアログが表示されつつ、メインスレッドはfalseを返して処理は終了となるが、それ以上の処理をしないので問題無い。
        // そのあとに処理が控えているなら非同期にしてawaitすべし。
        // それかTaskでResultのwaitをすべし。
        new Func<bool>(() =>
        {
            MessageDialogUtil.ShowWarning(
                Messenger,
                errMsg,
                () => Messenger.Raise(new InteractionMessage("FocusTimeType"))
            );
            return false;
        }).Invoke();
}
```

上記例では、最に合致したメッセージを表示するだけだが、合致しなかった条件を全て表示指せるならこのようになるだろう。  

``` cs
// エラーを全て表示させたい場合はこうなる
private string errMsg => string.Join(
    Environment.NewLine,
    new List<(bool flag, string msg)>()
    {
        // 何も条件が指定されていない場合、やる意味がないので塞ぐ
        (!Condition1.HasValue && !Condition2.HasValue && !Condtion3.HasValue , "変更内容が指定されていません。" ),
        // // 無効な組み合わせの場合
        (Condtion2.HasValue && !Condtion1.HasValue,"条件2を指定する場合、条件1も指定してください。" )
    }.Where(w => w.Key)
);
```

■**試行錯誤の跡**

最初はDictionaryで実装したのだが、2つ以上trueがあるとキーが衝突してしまうので使えなかった。  

``` cs
// DictionaryはKeyは衝突するので使えない。Listでタプルを使うべし。
private Dictionary<bool, string> errMsgDic => new Dictionary<bool, string>() {
    // 何も条件が指定されていない場合、やる意味がないので塞ぐ
    {!Condition1.HasValue && !Condition2.HasValue && !Condtion3.HasValue , "変更内容が指定されていません。" },
    // 無効な組み合わせの場合
    {Condtion2.HasValue && !Condtion1.HasValue,"条件2を指定する場合、条件1も指定してください。" }
};
```

---

## バリデーションエラーだった場合、専用の処理を実行した後、処理を終了したい

やりたいのは以下みたいなこと。  

処理を実行する時のバリデーションに引っかかったら、そのバリデーションに応じたメッセージを表示するとか、特定のコントロールにフォーカスするとか、そういう処理を実行してから全体の処理をreturnする方法。  

ただ抜けるだけならThrowでもいいが、それほどではない。  
かといって、if elseでグダグダ繋げて書くのもスマートではない。  

何かいい書き方はないか悩んだのでまとめる。  

``` cs
public void Execute () {
    if (!condition1) {
        Hoge();
        return;
    } else if (!condition2) {
        Fuga();
        return;
    }

    Do();
}
```

条件と処理はワンセットなので、Listでタプルにまとめることはできる。  
で、バリデーションに引っかかった地点で常にfalseなわけなので、actionを実行しつつ、常にflaseを返してもらいたい。  
つまり `Func<bool>`でアクションを実行しつつ、falseを返すように記述すればいいのだが、毎回return falseと書きたくない。

``` cs
public void Execute () {
    if (!CantExecuteAction) {
        return;
    }

    Do();
}

private bool CantExecuteAction => new List<(bool condition, Func<bool> func)>()
{
    (!Condition1,() => {Hoge(); return false:}), // 常に return falseと書く必要がある。
    (!Condition2,() => {Fuga(); return false:}),
}.FirstOrDefault(f => f.condition).func?.Invoke() ?? true;
```

そもそも、return false以前に、実行すべき関数が存在するなら実質falseであって、実行すべき関数がなければtrueなのだから、常にfalseを返すのはナンセンス。  
null or not nullで十分なのである。  

そういう判定をする関数を通せばいけるけど、actionが存在したら実行しつつ、falseを返す書き方がどうしても1文で収まらない。  
とてもモヤモヤする。  

``` cs
public void Execute () {
    if (Hantei(CantExecuteAction)) {
        return;
    }

    Do();
}

private bool Hantei(Action action)
{
    // nullじゃなければ実行
    action?.Invoke();
    // 関数があれば、常にfalseとなって元の処理を終了できる。
    return action == null;
}

private Action CantExecuteAction => new List<(bool condition, Action action)>()
{
    (!Condition1,() => Hoge()),
    (!Condition2,() => Fuga()),
}.FirstOrDefault(f => f.condition).action;
```

当時、最終的に行きついた考えとしてはis演算子で判定する方法。  
is演算子はnullの場合はfalseとなるので、ifに入らない。  
ifに入るということは関数が存在するということなので、後は関数を実行してreturnする。  
ワンセンテンスとまではいかなかったが、is演算子を使うアイディアが思い浮かんで興奮したのでこれでいくことにした。  

``` cs
public void Execute () {
    // 実行不可条件があれば処理しない
    // nullはisでfalseになるので、actionがnullなら処理されない。
    // actionがあればisはtrueになるので、中に入ってactionが実行される。
    if (CantExecuteAction is Action action)
    {
        action.Invoke();
        return;
    }

    Do();
}

private Action CantExecuteAction => new List<(bool condition, Action action)>()
{
    (!Condition1,() => Hoge()),
    (!Condition2,() => Fuga()),
}.FirstOrDefault(f => f.condition).action;
```

鬼畜の所業。  

``` cs
public void Execute () {
    if (!CantExecuteAction) return;
    Do();
}

private bool CantExecuteAction => new List<(bool condition, Action action)>()
{
    (true,Hoge),
    (false,Fuga),
}.FirstOrDefault(f => f.condition).action is Action action2
    ? (new Func<bool>(() => { action2.Invoke(); return false; })).Invoke()
    : true;
```

---

## readonlyの初期化

[未確認飛行C](https://ufcpp.net/study/csharp/resource/readonlyness/)の以下のコードを見て、「クラス」ではできるのか？と思ってやってみた。  

``` cs
var p = new Point(1, 2);

// p.X = 0; とは書けない。これはちゃんとコンパイル エラーになる

// でも、このメソッドは呼べるし、X, Y が書き換わる
p.Set(3, 4);

Console.WriteLine(p.X); // 3
Console.WriteLine(p.Y); // 4


struct Point
{
    // フィールドに readonly を付けているものの…
    public readonly int X;
    public readonly int Y;

    public Point(int x, int y) => (X, Y) = (x, y);

    // this の書き換えができてしまうので、実は X, Y の書き換えが可能
    public void Set(int x, int y)
    {
        // X = x; Y = y; とは書けない
        // でも、this 自体は書き換えられる
        this = new Point(x, y);
    }
}
```

発端はメソッドの引数で条件を生成したときに、それ以降変更することがないからreadonlyで運用できないか？と思ったからだ。  
コンストラクタで初期化すればいいのだが、DIで運用している & 凄まじい数のサービスがインジェクションされているので、とてもじゃないがコンストラクタが使い物にならない。  
というわけで、メソッドから引数を渡してそれっきりって運用はできないかと調べた。  

一応クラスでも自分自身をnewして返却すれば出来なくはないが、コンストラクタはDI前提で成り立っているので、この方法は使えなかった。  

``` cs
var test = new Test(1);
test = test.Fuga(2);
Console.WriteLine(test._i); // 2

public class Test
{
    public readonly int _i;

    public Test(int i)
    {
        this._i = i;
    }

    public void Hoge()
    {
        // 読み取り専用であるため 'this' に割り当てできません 
        // this = new Test(1);
    }

    // 自分自身を生成して返却すれば出来なくはない。
    public Test Fuga(int i) => new Test(i);
}
```

C# 9.0から追加されたrecord型とwith式なら、ある程度対応できるようだが、そもそも構造が悪い気がしてならない。  
[未確認飛行C_init-only プロパティ](https://ufcpp.net/study/csharp/oo_property.html#init-only)  

``` cs
var test = new Test(1);
test = test.Fuga(2);
Console.WriteLine(test.Y); // 2

public record Test
{
    public int Y { get; init; }

    public Test(int i)
    {
        this.Y = i;
    }
    
    // with式を使った
    public Test Fuga(int i) {
        return this with {Y = i};
    }
}
```

---

## is演算子による式中の変数宣言

``` cs
int hoge = Console.ReadLine() is string huga ? int.Parse(huga) : 0;
```

[特殊な変数宣言 - C# によるプログラミング入門 | ++C++; // 未確認飛行 C](https://ufcpp.net/study/csharp/datatype/declarationexpressions/)  
[今更ながらC# で湯婆婆を単文で実装してみる - Qiita](https://qiita.com/kaedepol/items/7cbc3f7bebb2783f3651)  

---

## チェックボックスの3値判定

チェックボックスで3値の判定をするときの実装。  

リストが1件も無ければチェックを付けない。  
複数件あって、1つでもfalseが混ざっていれば、「-」を表示する。  
これはチェックボックスに置けるnullの状態。  
もちろん全てtrueならチェックを付けるし、全てfalseならチェックを付けない。  

そういう判定をスマートに出来ないかやってみた。  
switch式が使えるバージョンならswitch式かなぁと思う。  

``` cs
var ListData = new List<Hoge>(){
    new Hoge(){
        HogeValue = "hoge1",
        IsSelected = true,
    },
    new Hoge(){
        HogeValue = "hoge2",
        IsSelected = true,
    }
};

class Hoge
{
    public string HogeValue { get; set; }
    public bool IsSelected { get; set; }
}
```

``` cs
// switch 式
var flag = ListData?.Select(a => a.IsSelected).Distinct().OrderBy(o => o).ToList() switch
{
    IEnumerable<bool?> hoge when (hoge?.Count() ?? 0) == 0 => false,
    IEnumerable<bool?> hoge when hoge.Count() == 1 => hoge.First(),
    _ => null,
};

// C#7.3で愚直にやった場合
var uniqueList = ListData?.Select(a => a.IsSelected).Distinct().OrderBy(o => o).ToList();
IsSelectAll = (uniqueList?.Count() ?? 0) == 0
    ? false
    : uniqueList.Count() == 2
        ? null
        : (bool?)uniqueList.First();
```

[【WPF】DataTableバインドなDataGridでSelect Allチェックボックスを作る｜fuqunaga｜note](https://note.com/fuqunaga/n/n62c8d678f249)  
[[C#] switch文をswitch式で表現する | FEELD BLOG](https://feeld-uni.com/?p=1365)  
