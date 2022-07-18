# C#メモ

[今日からできるC#のパフォーマンス改善小ネタ10選](https://qiita.com/shun-shun123/items/cb6689a9f210e90b9833)
[匿名型_ipentec](https://www.ipentec.com/document/csharp-using-anonymous-type)  

---

## シリアライズとデシリアライズを繰り返すと？

<https://www.jpcert.or.jp/java-rules/ser10-j.html>  
支払方法変更処理において、F4の初期化を実行するとメモリーがどんどん増えていくことに気が付いた。  
そこでは、まっさらなデータをDeepCopy(シリアライズとデシリアライズ)して代入する処理をしていたのだが、  
もしかしてこの場合、ガベージコレクションされないのかなと思ったらされない模様。  
おとなしくループさせて、必要な項目だけを初期化したらメモリーが増えることはなくなった。  

シリアライズがどういうことをするのか説明できないので、その記事も参考に置いておきますね。  
<http://funini.com/kei/java/serialize.shtml>  

シリアライズとは、メモリ上のデータをバイトに変換すること。  
メモリ上のそのインスタンスが保持するデータ全てがバイト列に置き変わる。  
バイト列なので、ポインタで指し示すデータではなく、実データとなる。  
それを元に戻すのがデシリアライズ。  
デシリアライズすると、バイト列のデータが元に戻るので、全く同じデータを持つインスタンスを作ることができる。  
もちろんこの時、メモリーのアドレスやポインタ等は新しくなっている。  
しかし、これを延々と繰り返すと、同じデータが無限に増やせるので、メモリリークの原因になるらしい。  

## シリアライズしたデータを見てみたい

<https://social.msdn.microsoft.com/Forums/vstudio/ja-JP/4fb1e972-c19b-4bbd-b828-82cb783c13e8/12458125021247212455124631248812434124711252212450125211245212?forum=csharpgeneralja>  
<http://funini.com/kei/java/serialize.shtml>  

シリアル化。  
XMLの他にJsonもある。多分バイト列にするやつもある。  
なるほど。  
参照先の情報をXMLやJsonにまとめるのね。  
そうすれば、通信相手にデータを送ることができる。  

参照情報だけでは、参照先のアドレスしかわからない。ただの数字でしかないからね。  
シリアル化は、そういった参照先の情報を全て持ってきて、XMLなり、Jsonなり、バイトなりに変換する。  
そうすることで、全てのデータをそれらファイルにまとめることができる。  
極端な話、全て文字列にするわけだ。  

そうしてしまえば、まぁただのデータなので、相手に送ることもできるし、参照なんて関係なく文字列として扱うことが出来る。  
もちろん、受け取ったデータから複合する事で新しい参照にそいつらデータをぶち込んで、元のように使うことが出来るわけだ。  
まぁ、参照先は違って当然だけど、別にそれが本質ではないしね。  

だから、JsonSerializerでJson化してDeserializeするとディープコピーができるわけか。  
納得。  

だからシリアル化、一連のデータとするって意味でシリアル化っていうわけか。  
なるほど。なるほど。  

``` C#
    //シリアライズ対象のクラス
    public class SampleClass
    {
        public int Number;
        public string Message;
    }

    class Program
    {
        static void Main(string[] args)
        {
            SampleClass cls = new SampleClass
            {
                Message = "テスト",
                Number = 123
            };

            var serializer = new XmlSerializer(typeof(SampleClass));
            var sw = new StringWriter();
            serializer.Serialize(sw, cls);
            Console.WriteLine(sw.ToString());
            sw.Close();

            var ms = new MemoryStream();
            serializer.Serialize(ms, cls);
            ms.Position = 0;
            var i = (SampleClass)serializer.Deserialize(ms);
            Console.WriteLine(i.ToString());
            ms.Close();
        }
    }
```

``` XML : Console.WriteLineした結果
<?xml version="1.0" encoding="utf-16"?>
<SampleClass xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <Number>123</Number>
  <Message>テスト</Message>
</SampleClass>
```

---

## json deserialize object to int

[C# で数字を object 型にキャストした値型の扱いについて](https://cms.shise.net/2014/10/csharp-object-cast/)  
[【C#】Boxing / Unboxing ってどこで使われてるのか調べてみた](https://mslgt.hatenablog.com/entry/2017/11/18/132025)  

サーチボックスで主キーが2つある場合に、主キーを送る仕組みがなかったので、objectに格納して送信するようにした時に、  
キャストエラーになったので色々調べた。  
そもそもobject型に変換されたものはConvert.ToInt32系のメソッドを使って変換しないとエラーになってしまう模様。  
後は、jsonは数値型しかなく、値の劣化の心配がないlong型(int64)に自動的に変換される模様。  
Boxing,Unboxingという仕組みもあり、なかなか奥が深かった。  
実際に、jsonにシリアライズしてデシリアライズしたときにエラーになるので、そういう物と認識したほうがいいかもしれない。  

``` C#
    int i = 3;
    object obj = i;
    // シリアライズして送信してデシリアライズして受け取った体
    var de_json = JsonConvert.DeserializeObject<object>(JsonConvert.SerializeObject(obj));
    // System.InvalidCastException: 'Unable to cast object of type 'System.Int64' to type 'System.Int32'.'
    var de_i = (int)de_json;
```

こっちだとInvalidCastExceptionになるのでJsonに変換する場合とはまた違うのかもしれない。  

``` C#
    double d = 1.23456;
    object o = d;
    int i = (int)o;
    Console.WriteLine(i);
```

---

## 三項演算子で同じインタフェースを実装したクラスがなぜ暗黙的変換といわれるのか

java5以降改善されたらしい。その前までは同様の現象が起こっていた模様。  
C#も今後改善されるのかな。  

## 条件演算子のターゲット型推論の強化

azmさんに三項演算子でnull許可のboolを受け取るとき、「片方を変換しないといけないのキモイね」って言われたので、「受け取り側をvarじゃなくてbool?にすれば行けますよ」って言ったけどエラーになった。  
どうやらこれが有効なのはC#9からみたいで、Framework4.8のC#7.3では無理だった。  
家でやるサンプルは基本的に最新版なので、バージョンを意識することがない。  
それを意識するいい体験だったのでまとめた。  

``` C#
// この記述が許されるのはC#9から。
// これはC#9の条件演算子のターゲット型推論の強化に当たるらしい。
bool? aa = true ? false : null;

// C#9以前はこのように書くしかない。
var aa = true ? (bool?)false : null;
```

---

## インスタンスの状態

[[C# 入門] クラスのインスタンスについて](https://yaspage.com/prog/csharp/cs-instance/)  

DependencyPropertyのBindingの件でazmさんがインスタンスの状態なんて事を言っていたので調べたわけだが、  
それとは別にインスタンスについての基礎を紹介しているページがわかりやすかったのでまとめる。  

後日、azmさんがまた使っていたので、意味合いから想像すると、例えばLinqで.Where()を実行したときと、.ToList()を実行したときでは、戻ってくる値が違うわけで、  
azmさんはどうやら帰ってくる型の事をインスタンスの状態と言っている見たいだ。  

---

## try catchのスコープの話

tryで宣言した変数を外部で使うためには、tryの外で予め宣言しておいてからでないと使えないわけで、  
それってダサくね？もっといい方法ないの？ってことで調べた。  

結局、これだ！っていう感じの答えは見つからなかった。  
更にtryで囲むとか？  
まぁ、理由があってこの形なのだから愚直にやるのが一番いいような気もする。  

[「catch」または「finally」のスコープの「try」で変数が宣言されないのはなぜですか？](https://www.webdevqa.jp.net/ja/c%23/%E3%80%8Ccatch%E3%80%8D%E3%81%BE%E3%81%9F%E3%81%AF%E3%80%8Cfinally%E3%80%8D%E3%81%AE%E3%82%B9%E3%82%B3%E3%83%BC%E3%83%97%E3%81%AE%E3%80%8Ctry%E3%80%8D%E3%81%A7%E5%A4%89%E6%95%B0%E3%81%8C%E5%AE%A3%E8%A8%80%E3%81%95%E3%82%8C%E3%81%AA%E3%81%84%E3%81%AE%E3%81%AF%E3%81%AA%E3%81%9C%E3%81%A7%E3%81%99%E3%81%8B%EF%BC%9F/957401549/)  

>2つのこと：  
>
>1. 通常、Javaのスコープは2つのレベルのみです：グローバルと関数。ただし、try/catchは例外です（しゃれは意図していません）。
> 例外がスローされ、例外オブジェクトに変数が割り当てられた場合それに対して、そのオブジェクト変数は「catch」セクション内でのみ使用可能であり、catchが完了するとすぐに破棄されます。
>
>2. （そして更に重要なことに）。 tryブロックのどこで例外がスローされたかを知ることはできません。変数が宣言される前の可能性があります。したがって、catch/finally句で使用できる変数を指定することはできません。スコーピングが提案されたとおりである次のケースを検討してください。
>
>``` C#
>try
>{
>    throw new ArgumentException("some operation that throws an exception");
>    string s = "blah";
>}
>catch (e as ArgumentException)
>{  
>    Console.Out.WriteLine(s);
>}
>```
>
>これは明らかに問題です。例外ハンドラに到達すると、sは宣言されません。キャッチは例外的な状況とfinallys must executeを処理することを意図しているので、安全であり、コンパイル時に問題を宣言することは実行時よりもはるかに優れています。
>

---

## Foreachでnullが来ても大丈夫な書き方

[foreachの時のNullReferenceExceptionを回避する](https://tiratom.hatenablog.com/entry/2018/12/16/foreach%E3%81%AE%E6%99%82%E3%81%AENullReferenceException%E3%82%92%E5%9B%9E%E9%81%BF%E3%81%99%E3%82%8B)  

foreachをやる前にifでnullチェックするのが野暮ったく感じたし、インデントが深くなってしまうのでなんかいい書き方はないかなということで探した。  
前にも調べたはずだが、忘れたっぽいので改めてまとめる。  

null合体演算子[??]と`Enumerable.Empty<T>()`か`new List<T>()`の2つ組み合わせで実現できる模様。  
Enumerable.EmptyはLinqで空のシーケンスを取得するための構文の模様。  

[【C#,LINQ】Empty～空のシーケンスがほしいとき～](https://www.urablog.xyz/entry/2018/06/02/070000)  

``` C#
    // 例1
    foreach (string msg in msgList ?? Enumerable.Empty<String>()){}

    // 例2
    foreach (string msg in msgList ?? new List<string>()){}
```

---

## Enumerable.Empty vs new List

Foreachでnullが来ても大丈夫な書き方でEnumerableとListの2パターンを示したが、ではどちらがいいのかという問題が発生する。  
2つの単語を並べただけでどちらがいいのか？という記事はたくさんヒットした。  

[Is it better to use Enumerable.Empty\<T>() as opposed to new List\<T>() to initialize an IEnumerable\<T>?](https://stackoverflow.com/questions/1894038/is-it-better-to-use-enumerable-emptyt-as-opposed-to-new-listt-to-initial)  

>たとえ空の配列や空のリストを使ったとしても、それらはオブジェクトであり、メモリに保存されています。  
>ガーベッジコレクタはそれらの面倒を見なければなりません。  
>高スループットのアプリケーションを扱っている場合、それは顕著な影響を与える可能性があります。  
>Enumerable.Emptyは、呼び出しごとにオブジェクトを作成しないので、GCへの負荷が少なくなります。  

[C# Enumerable.Empty](https://thedeveloperblog.com/empty)  

> Because Enumerable.Empty caches the zero-element array, it can provide a slight performance advantage in some programs.
> Enumerable.Emptyはゼロ要素配列をキャッシュするため、一部のプログラムではパフォーマンスがわずかに向上する可能性があります。  

ということで、Enumerableに軍配が上がるみたい。  

---

## using

usingの省略範囲はforeachやwhileのように直前の1つだけじゃなくて、そのブロックの終わりまで続くらしい。  
割と初めて知った。  

using宣言は8.0からの機能である模様。  

``` C# : 省略前
    string constr = @"接続文字列";
    using (SqlConnection con = new SqlConnection(constr))
    {
        con.Open();

        string sqlstr = "select * from products";
        SqlCommand com = new SqlCommand(sqlstr, con);

        using (SqlDataReader sdr = com.ExecuteReader())
        {
            while (sdr.Read())
            {
                _TextBox1.Text += $"{(string)sdr["name"]:s}:{(int)sdr["price"]:d} \r\n";
            }
        }
    }
```

``` C# : 省略後
    string constr = @"接続文字列";
    using SqlConnection con = new SqlConnection(constr);
    con.Open();

    string sqlstr = "select * from products";
    SqlCommand com = new SqlCommand(sqlstr, con);

    using SqlDataReader sdr = com.ExecuteReader();
    while (sdr.Read())
        _TextBox1.Text += $"{sdr["name"].ToString():s}:{(int)sdr["price"]:d} {Environment.NewLine}";
```

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

## const,readonly,static readonlyの違い

<https://qiita.com/4_mio_11/items/203c88eb5299e4a45f31>

### const

コンパイル時に定義する。  
なので、メソッドの結果など、実行しなければ確定しないものは定数として定義できない。  
これによって、バージョニング問題が発生するとか。  

``` C#
    class Hoge
    {
        public const double PI = 3.14;     // OK
        public const double piyo = PI \* PI;     // OK
        public const double payo = Math.Sqrt(10);   // NG

        void Piyo(){
        //コンパイルで生成される中間言語では下の条件式はmyData == 3.14となる
        if(Moge == PI)
            //処理
    }
```

### readonly

コンストラクタでのみ書き込み可能。  
それ以降は変更不可。  

### static readonly

コンパイル時ではなく、実行時に値が定まり、以後不変となる。  
コンパイルした後の話なので、メソッドを実行した結果をプログラム実行中の定数として扱うことができる。  
バージョニング問題的な観点から、constよりstatic readonlyが推奨される。  
ちなみにconstは暗黙的にstaticに変換されるので、staticを嫌悪する必要はない。  

---

## アノテーション

Annotation:注釈  
<https://elf-mission.net/programming/wpf/episode09/>  

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

## using static

<https://ufcpp.net/study/csharp/oo_static.html>  

using static ディレクティブを書くことで、クラス名を省略して、直接静的メソッドを呼べるようになります。  
例えば、Math クラス(System 名前空間)中のメソッド呼び出しであれば、以下のように書けます。  

``` C#
using System;
using static System.Math;

class Program
{
    static void Main()
    {
        // using static を使わないならMath.Asin(1)
        var pi = 2 * Asin(1);
        Console.WriteLine(PI == pi);
    }
}
```

ちなみに、using static は任意のクラスに対して使えます(静的クラスでないとダメとかの制限はありません)。  
たとえば以下の例では、TimeSpan構造体やTaskクラスを using static していますが、これらは static 修飾子がついていない普通のクラスです。  

``` C#
using System.Threading.Tasks;
using static System.Threading.Tasks.Task;
using static System.TimeSpan;

class UsingStaticNormalClass
{
    public async Task XAsync()
    {
        // TimeSpan.FromSeconds
        var sec = FromSeconds(1);

        // Task.Delay
        await Delay(sec);
    }
}
```

### using staticと列挙型

列挙型のメンバーも静的なので、using staticを使って、型名を省略して参照できます。  

``` C#
using static Color;

class UsingStaticEnum
{
    public void X()
    {
        // enum のメンバーも using static で参照できる
        var cyan = Blue | Green;
        var purple = Red | Blue;
        var yellow = Red | Green;
    }
}

enum Color
{
    Red = 1,
    Green = 2,
    Blue = 4,
}
```

### using staticと拡張メソッド

using static を使う場合でも、そのクラス中の拡張メソッドはあくまで拡張メソッドとしてだけ使えます。  
using static だけでは、拡張メソッドを普通の静的メソッドと同じ呼び方で呼べません。  

``` C#
using static System.Linq.Enumerable;

class UsingStaticSample
{
    public void X()
    {
        // 普通の静的メソッド
        // Enumerable.Range が呼ばれる
        var input = Range(0, 10);

        // 拡張メソッド
        // Enumerable.Select が呼ばれる
        var output1 = input.Select(x => x * x);

        // 拡張メソッドを普通の静的メソッドとして呼ぼうとすると
        // コンパイル エラー
        var output2 = Select(input, x => x * x);
    }
}
```

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

## typeof

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
        Console.WriteLine($"bはBaseClass型？       = {b.GetType() == typeof(BaseClass)}");
        Console.WriteLine($"bはChildClass型？      = {b.GetType() == typeof(ChildClass)}");
        Console.WriteLine($"bはGrandChildClass型？ = {b.GetType() == typeof(GrandChildClass)}");

        b = new ChildClass();
        Console.WriteLine("\nBaseClass b = new ChildClass();");
        Console.WriteLine($"bはBaseClass型？       = {b.GetType() == typeof(BaseClass)}");
        Console.WriteLine($"bはChildClass型？      = {b.GetType() == typeof(ChildClass)}");
        Console.WriteLine($"bはGrandChildClass型？ = {b.GetType() == typeof(GrandChildClass)}");

        b = new GrandChildClass();
        Console.WriteLine("\nBaseClass b = new GrandChildClass();");
        Console.WriteLine($"bはBaseClass型？       = {b.GetType() == typeof(BaseClass)}");
        Console.WriteLine($"bはChildClass型？      = {b.GetType() == typeof(ChildClass)}");
        Console.WriteLine($"bはGrandChildClass型？ = {b.GetType() == typeof(GrandChildClass)}");
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

---

## 範囲アクセス

`a[i..j]` という書き方で「i番目からj番目の要素を取り出す」というような操作ができるようになりました。  
C# 8.0からの機能なので、.NetFramework(C# 7.3)では使えません。  

``` C#
class Program
{
    static void Main()
    {
        var a = new[] { 1, 2, 3, 4, 5 };
         // 前後1要素ずつ削ったもの
        var middle = a[1..^1];
         // 2, 3, 4 が表示される
        foreach (var x in middle)
            Console.WriteLine(x);
    }
}
```

---

## Equalsメソッドと ==演算子 の違い

[==演算子とEqualsメソッドの違いとは？［C#］](https://atmarkit.itmedia.co.jp/ait/articles/1802/28/news028.html)  
[2つの値が等しいか調べる、等値演算子(==)とEqualsメソッドの違い](https://dobon.net/vb/dotnet/beginner/equality.html)  
[【C#】文字列を比較する（== 演算子、Equalメソッド、Compareメソッド）](https://nyanblog2222.com/programming/c-sharp/193/)  

Javaの時にもまとめたかもしれないが、改めて==とEqualsの違いをまとめる。  

事の発端はY君がEqualsで文字列の比較をしていて、左辺の文字列がnullで.Equalsメソッドで比較しようとしてエラーになるのを発見したためだ。  
文字列の比較にEqualsを使う必要性は何だったのかわからなかったので、そもそもどういう違いがあるのか、==ではダメだったのかを調べた。  

[バグのもと!?”==”と”Equals”の使い分け](https://fledglingengineer.xyz/equals/)  
>C#ではNULLがあり得る単純な文字列比較の場合、“==”を使用した方が良いです。  

早速結論が出た。  

### == 値の等価

値の等価とは、比較する2つのオブジェクトの中身が同じであるということです。  

``` C#
string a = new string("Good morning!");
string b = new string("Good morning!");

if(a == b) // True
{
    Console.WriteLine("True!");
}
```

こちらは”a”と”b”の中身の文字列を比較しており、中身の文字列が一致しているためTrueとなります。  
![a](https://fledglingengineer.xyz/wp-content/uploads/2020/09/image-24.png)  

### Equals 参照の等価

一方で、参照の等価とは、比較する両者が同じインスタンスを参照しているということです。  

``` C#
string a = new string("Good morning!");
string b = new string("Good morning!");

if (a.Equals(b)) // True
{
    Console.WriteLine("True!");
}
```

![q](https://fledglingengineer.xyz/wp-content/uploads/2020/09/image-22.png)

### オブジェクトの比較

先程の値の参照に”(object)”を付けた場合どうなるか?  
その場合、オブジェクトの比較となり、“a”と”b”はそれぞれ異なるオブジェクトのため、Falseとなる。  

``` C#
string a = new string("Good morning!");
string b = new string("Good morning!");

if ((object)a == (object)b) // False
{
    Console.WriteLine("True!");
}
```

![s](https://fledglingengineer.xyz/wp-content/uploads/2020/09/image-25.png)

### NULLの判定は”==”を使うべき

``` C#
string name1 = "Mike";
string name2 = "Mike";

if(name1 == name2) // True
{
    Console.WriteLine("True!");
}
```

上記の場合は、特にインスタンスを生成しているわけではないので、“==”を使用しても、”Equals”を使用しても値の等価となり、一見問題ないように考えられます。  

しかし、以下の場合はどうなるでしょうか?

``` C#
// “name1″にnullが入るかもしれないため、nullチェックを設けたとする。  
string name1 = null;

// “==”を使用したif文は正常に動作し、Trueを返す。  
 if(name1 == null) // True
{
    Console.WriteLine("True!");
}

// 一方で、”Equals”を使用したif文では例外が発生する。  
// こちらはコンパイル時に、エラーとはならないため、思わぬバグを生んでしまう可能性があるため単純な文字列の比較は==が無難。  
if (name1.Equals(null)) // System.NullReferenceException: 'Object reference not set to an instance of an object.'
{
    Console.WriteLine("True!");
}
```

### 等値演算子とEqualsメソッドの違い

C#では、値型の比較に==演算子を使うと「値の等価」を調べることになります。  
参照型の比較に==演算子を使うと、通常は「参照の等価」を調べます。  
しかし、String型のように、クラスで等値演算子がオーバーロードされているならば、参照型でも==演算子で「値の等価」を調べます。  

Equalsメソッドは、値型の比較に使うと、「値の等価」を調べます。  
参照型の比較に使うと、通常は「参照の等価」を調べます。  
しかし、String型のように、クラスのEqualsメソッドがオーバーライドされていれば、参照型でも「値の等価」を調べます。  

「参照の等価」を調べるためには、Object.ReferenceEqualsメソッドを使用することもできます。  
さらにC#では、Object型にキャストしてから==演算子で比較することでも、確実に参照の等価を調べることができます。  

### Stringクラス

等値演算子とEqualsメソッドで値の等価を調べることができるクラス（等値演算子がオーバーロードされ、かつ、Equalsメソッドがオーバーライドされているクラス）は多くありません。  

その代表は、Stringクラスです。  
その他にもVersionクラスなどもそのようですが、とりあえずStringクラスはこのように特別なクラスであることを覚えておいてください。  
このようなクラスでは、参照型にもかかわらず、等値演算子やEqualsメソッドで「値の等価」を調べることができます。  

### 結局、どちらを使うべきか

値型の等価は==演算子で調べるのが良いでしょう。  
参照型で値の等価を調べるには、Equalsメソッドを使うのが確実でしょう。  
参照型で明確に参照の等価を調べたいならば、Object.ReferenceEqualsメソッド（またはObject型にキャストしてから==演算子）を使います。

``` C#
//値型の等価を調べる
int i1 = 1;
int i2 = i1 * i1;
Console.WriteLine(i1 == i2); //true
Console.WriteLine(i1.Equals(i2)); //true
Console.WriteLine(object.Equals(i1, i2)); //true

//参照型の等価を調べる
//o1とo2は別のインスタンス
object o1 = new object();
object o2 = new object();
Console.WriteLine(o1 == o2); //false
Console.WriteLine(o1.Equals(o2)); //false
Console.WriteLine(object.Equals(o1, o2)); //false

//o1とo2は同じインスタンス
o2 = o1;
Console.WriteLine(o1 == o2); //true
Console.WriteLine(o1.Equals(o2)); //true
Console.WriteLine(object.Equals(o1, o2)); //true

//String型の等価を調べる
//s1とs2は同じ値だが、別のインスタンス
string s1 = new string('a', 10);
string s2 = new string('a', 10);
Console.WriteLine(s1 == s2); //true
Console.WriteLine(s1.Equals(s2)); //true
Console.WriteLine(object.ReferenceEquals(s1, s2)); //false
Console.WriteLine(object.Equals(s1, s2)); //true
```

``` C# : なんか個人でいろいろ頑張った後
    string A = "AA";
    string B = "BB";

    Console.WriteLine(A.Equals(B));
    Console.WriteLine( A == B);
    Console.WriteLine(Equals(A, B));

    B = "AA";

    Console.WriteLine(A.Equals(B));
    Console.WriteLine(A == B);
    Console.WriteLine(Equals(A, B));

    A = null;
    B = null;

    Console.WriteLine(A == B);
    Console.WriteLine(A?.Equals(B) ?? false);
    Console.WriteLine(Equals(A, B));
```

---

## ToListしないで済ませる

``` C#
var code = "aaaaaa";
var stringList = _Dapper.Execute(
    // ①Dapperでデータベースからコード一覧を取得する。
    // WHERE Code = @code
);
// ②addメソッドを使いたいのでToListする
stringList.ToList();
// ③検索条件に使ったcodeをAddすることで1つのコード一覧とする。
stringList.Add(code);
```

上の方法だと、Where条件として使ってるのに、もう一度追加する必要があるのでなんか無駄に感じる。

``` C#
var code = "aaaaaa";
var stringList = _Dapper.Execute(
    // SELECT @code AS code
    // UNION
    // SELECT code FROM table
    // Where Code = @code
)
```

Dapperに流すクエリの中でUNIONしてやればそのひと手間をなくせるのでは？というサンプル。  

---

## 匿名型（匿名クラス）を動的に作成する

[.NET Core (C#) 匿名型（匿名クラス）を動的に作成する](https://marock.tokyo/2021/01/24/net-core-%E5%8C%BF%E5%90%8D%E5%9E%8B%EF%BC%88%E5%8C%BF%E5%90%8D%E3%82%AF%E3%83%A9%E3%82%B9%EF%BC%89%E3%82%92%E5%8B%95%E7%9A%84%E3%81%AB%E4%BD%9C%E6%88%90%E3%81%99%E3%82%8B/)  
[匿名型の動的生成に関して](https://dobon.net/vb/bbs/log3-54/31793.html)  
[匿名型を動的に作成しますか？](https://www.web-dev-qa-db-ja.com/ja/c%23/%E5%8C%BF%E5%90%8D%E5%9E%8B%E3%82%92%E5%8B%95%E7%9A%84%E3%81%AB%E4%BD%9C%E6%88%90%E3%81%97%E3%81%BE%E3%81%99%E3%81%8B%EF%BC%9F/970777402/)  
[新しい匿名クラスを動的にするには？](https://www.web-dev-qa-db-ja.com/ja/c%23/%E6%96%B0%E3%81%97%E3%81%84%E5%8C%BF%E5%90%8D%E3%82%AF%E3%83%A9%E3%82%B9%E3%82%92%E5%8B%95%E7%9A%84%E3%81%AB%E3%81%99%E3%82%8B%E3%81%AB%E3%81%AF%EF%BC%9F/970949625/)  

匿名型（匿名クラス）を動的に作成するには、System.DynamicのExpandoObjectクラスを使用する必要がある。  
newしたインスタンスに対して愚直にaddするしかない模様。  
というか、思った以上に深い内容だった。  

``` C#
    // 動的に作成するプロパティ、値の組み合わせをDictionaryに登録
    var dic = new Dictionary<string, string>
    {
        { "key1", "キー1" },
        { "key2", "キー2" }
    };
```

``` C# : パターン1
    // 匿名型をDictionaryから作成
    dynamic anonymousType = new ExpandoObject();
    foreach (var item in dic)
    {
        ((IDictionary<string, object>)anonymousType).Add(item.Key, item.Value);
    }
    ((IDictionary<string, object>)anonymousType).Add("Key3", "キー3");
    // 動的生成はdynamicなのでインテリセンスは効かない
    Console.WriteLine(anonymousType.key1);  // キー1
    Console.WriteLine(anonymousType.key2);  // キー2
    Console.WriteLine(anonymousType.key3);  // キー3

    // 通常の匿名型はインテリセンスが効く
    var test = new { key1 = "キー1", key2 = "キー2" };
    Console.WriteLine(test.key1);  // キー1
    Console.WriteLine(test.key2);  // キー2
```

``` C# : パターン2
    IDictionary<string, object> expando = new ExpandoObject();
    foreach (var item in dic)
    {
        expando[item.Key] = item.Value;
    }
    dynamic d = expando;
    Console.WriteLine(d.key1);
    Console.WriteLine(d.key2);
```

``` C# : パターン3
    IDictionary<string, object> expando = new ExpandoObject();
    foreach (var item in dic)
    {
        expando.Add(item.Key, item.Value);
    }
    dynamic d = expando;
    Console.WriteLine(d.key1);
    Console.WriteLine(d.key2);
```

``` C#
    dynamic expando = new ExpandoObject();
    IDictionary<string, object> dictionary = (IDictionary<string, object>)expando;
    dictionary.Add("FirstName", "Bob");
    dictionary.Add("LastName", "Smith");
    Console.WriteLine(expando.FirstName + " " + expando.LastName);
```

直接プロパティを指定することで生成することも可能な模様。  

``` C#
dynamic employee = new ExpandoObject();
employee.Name = "John Smith";
employee.Age = 33;
foreach (var property in (IDictionary<string, object>)employee)
{
    Console.WriteLine(property.Key + ": " + property.Value);
}
// Name: John Smith
// Age: 33
```

次のようなExpandoObjectを作成できます。

``` C#
IDictionary<string,object> expando = new ExpandoObject();
expando["Name"] = value;
```

そして、動的にキャストした後、これらの値はプロパティのようになります。

``` C#
dynamic d = expando;
Console.WriteLine(d.Name);
```

ただし、これらは実際のプロパティではなく、Reflectionを使用してアクセスすることはできません。  
したがって、次のステートメントはnullを返します。  

``` C#
d.GetType().GetProperty("Name") 
```

どうしても1行で済ませたいならFuncデリゲートを使うしかない。  

``` C#
 dynamic d = new Func<IDictionary<string, object>>(() =>
    {
        IDictionary<string, object> expando = new ExpandoObject();
        foreach (var item in dic)
        {
            expando.Add(item.Key, item.Value);
        }
        return expando;
    }).Invoke();
    Console.WriteLine(d.key1);
    Console.WriteLine(d.key2);
```

こういう芸当もできる。  
Dapperに渡す条件を生成するときに使えるだろう。  

``` C#
    Student student = new Student()
    {
        StudentId = "1",
        StudentName = "Cnillincy"
    };
    var props = student
        .GetType()
        .GetProperties(BindingFlags.Instance | BindingFlags.Public)
        .Select(s => (key: s.Name, value: s.GetValue(student)))
        .ToList();
    IDictionary<string, object> anonymousType = new ExpandoObject();
    foreach (var (key, value) in props)
    {
        anonymousType.Add(key, value);
    }
    dynamic dynamic = anonymousType;
    Console.WriteLine(dynamic.key1);  // キー1
    Console.WriteLine(dynamic.key2);  // キー2
```

動的生成は前提としてExpandoObjectをnewしたインスタンスに対して愚直にaddしないといけないからLinqで都度returnは機能しない。  
だから絶対にlinqでは実現不能だ。  

``` C# : ダメなやつ
    dynamic d = (IDictionary<string, object>)dic.ToDictionary(s => s.Key, aa => (object)aa.Value);
    // これはダメ
    Console.WriteLine(d.key1);
    Console.WriteLine(d.key2);
    // これはいける。
    // Linqで返されるのは、結局Dictionary型でしかないという事だろう。  
    Console.WriteLine(d["key1"]);
    Console.WriteLine(d["key2"]);
```
