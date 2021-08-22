# 雑記

## キャスト

「is」,「as」「前かっこ」による変換の違い。  

・前かっこは変換に失敗するとエラー(System.InvalidCastException)になる。  
・as は変換に失敗するとnullになる。エラーにはならない。  
・is はif文で使うので、失敗したらelseに流れるだけ。  

## タプルのリストを簡単に初期化する方法

<https://cloud6.net/so/c%23/1804197>

``` C#
    // List ver
    var tupleList = new List<(int Index, string Name)>
    {
        (1, "cow"),
        (5, "chickens"),
        (1, "airplane")
    };
    // 配列 ver
    var tupleList = new (int Index, string Name)[]
    {
        (1, "cow"),
        (5, "chickens"),
        (1, "airplane")
    };
```

---

## アノテーションを使った、リストに1件もない場合のバリデーション

<https://stackoverflow.com/questions/5146732/viewmodel-validation-for-a-list>  
画面にエラー状態は表示したくないけど、警告は出したい場合があったのでその備忘録。  

``` C#
[Range(1, int.MaxValue, ErrorMessage = "At least one item needs to be selected")]
public int ItemCount
{
    get => Items != null ? Items.Length : 0;
}
```

---

## C#のアクセス修飾子

<https://www.fenet.jp/dotnet/column/language/6153/>

`internal`がわからなかったのでついでに調べた。  

- public    : あらゆる所からアクセスできる  
- private   : 同じクラス内のみアクセスできる  
- protected : 同じクラスと派生したクラスのみアクセスできる  
- internal  : 同じアセンブリならアクセスできる  
- protected internal : 同じアセンブリと別アセンブリの派生クラスでアクセスできる  
- private protected  : 同じクラスと同じアセンブリの派生したクラスのみアクセスできる  

---

## is演算子によるnullチェック

クラスAのインスタンスを作ります。  
インスタンス変数にnullを代入します。  
それをオブジェクト型変数bに代入します。  
bをAにキャストした場合どうなるか。  
→  
nullの場合は失敗するが、nullを代入しないと成功する。  
でもなんで？  

``` C#
public class Hello{
    class A {
    }

    public static void Main(){
        A a = new A();
        a = null; // ※
        object b = a;
        
        if (b is A aa) {
            // nullを代入しないとこちら
            System.Console.WriteLine("Success");
        } else {
            // a = nullの場合はこちら
            System.Console.WriteLine("Fail");
        }
    }
}
```

<https://ufcpp.net/study/csharp/datatype/typeswitch/>  
元々のis演算子の仕様でもあるんですが、nullには型がなくて常にisに失敗します(falseを返す)。  
なるほどね。is演算子は実行時の変数の中身を見るが、nullは型がないので、エラーになるわけか。  
納得。  
しかし、言語仕様で困ったらマイクロソフトではなく未確認飛行Cを見るのが一番だな。  
こっちのほうがわかりやすい。  

``` C#
string x = null;

if (x is string)
{
    // x の変数の型は string なのに、is string は false
    // is 演算子は変数の実行時の中身を見る ＆ null には型がない
    Console.WriteLine("ここは絶対通らない");
}
```

---

## SignalR

非同期でリアルタイムな双方向通信を実現するライブラリ。  
要はサーバーからの通知を実現する技術の.Net版だ。  

通常、サーバーとクライアントのやり取りはクライアントからのリクエストがトリガーとなる。  
それだけしかないので、サーバーからクライアントへ通知する手段は存在しない。  
システムの性質上、どうしてもサーバーからの状態を通知したい場合はクライアントから定期的にリクエストを飛ばすしかない。  
それに対応するための技術といえるだろう。  
2012年あたりから記事が見つかるので、結構古い技術なのかもしれない。  

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

---

## const,readonly,static readonlyの違い

<https://qiita.com/4_mio_11/items/203c88eb5299e4a45f31>

- const  
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

- readonly  
コンストラクタでのみ書き込み可能。  
それ以降は変更不可。  

- static readonly  
コンパイル時ではなく、実行時に値が定まり、以後不変となる。  
コンパイルした後の話なので、メソッドを実行した結果をプログラム実行中の定数として扱うことができる。  
バージョニング問題的な観点から、constよりstatic readonlyが推奨される。  
ちなみにconstは暗黙的にstaticに変換されるので、staticを嫌悪する必要はない。  

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

## Boxing Unboxing

[【C#】Boxing / Unboxing ってどこで使われてるのか調べてみた](https://mslgt.hatenablog.com/entry/2017/11/18/132025)  
[Boxing and Unboxing (C# Programming Guide)](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/types/boxing-and-unboxing)

json deserialize object to intの時にはまったので、こちらの概念もまとめておく。  
あちらは、intをObjectに入れるまではよかったが、intとして取り出せなかった問題。  
Jsonの数値型がlong型(int64)だから内部で勝手に変換されていたのが原因らしい。  
intをObjectに入れたときの動作がBoxingで、調べていたらバンバンヒットしたからまとめたいって思ったわけ。  

Boxing は雑にまとめると int などの値型を Boxing という仕組みを使って object 型にすることで、
参照型として扱えるようにする、ということです( Unboxing は object型から intを取り出す)。  

今回のように、Objectとして、何でもAPI側に渡せるようにする場合、この概念を知っておかないといけない。  

``` C#
    int i = 123;
    // Boxing copies the value of i into object o.
    object o = i;
    // Change the value of i.
    i = 456;
    /* Output:
        The value-type value = 456
        The object-type value = 123
    */
    // 参照は別になる模様
```

[オブジェクトをintにキャストするより良い方法](https://www.it-swarm-ja.com/ja/c%23/%E3%82%AA%E3%83%96%E3%82%B8%E3%82%A7%E3%82%AF%E3%83%88%E3%82%92int%E3%81%AB%E3%82%AD%E3%83%A3%E3%82%B9%E3%83%88%E3%81%99%E3%82%8B%E3%82%88%E3%82%8A%E8%89%AF%E3%81%84%E6%96%B9%E6%B3%95/957907480/)  
[【C#】いろんな型変換（キャスト）Convert vs Parse vs ToString](https://kuroeveryday.blogspot.com/2014/04/convert-vs-parse-vs-tostring.html)  
ついでにこちらもどうぞ。  
Object型の適切なキャスト方法がまとめられています。  

2021/08/13 Fri 追記  
<https://ufcpp.net/study/csharp/RmBoxing.html>  
やはり未確認飛行C。わかりやすい。  

・int型等、値型をobjectに代入するとbox化  
・代入したobjectから(int)objectってやって値を取り出すのがunbox化  
・スタックとヒープがある。スタックが値型、ヒープが参照型。  
・基本的にスタックのほうが軽い。  
・box化をするとヒープに領域が生成され、スタックにポインタを持つことで結びつく。  
・ヒープに領域が確保される処理は思い。  
・そこから(int)Objectで取り出した時、新しくスタックが詰まれるので、更にメモリを消費する。  
・コピーされて生成されるので、中身的には別物扱い。  
簡単にいうとそういうことらしい。  

---

## アノテーション

Annotation:注釈  
https://elf-mission.net/programming/wpf/episode09/  
恐らくではあるが、属性やバリデーションの為にクラスやフィールドの宣言の上に[]で囲うやつの事全般をこう読んでいるのではないか?  
調べてもそういうのしか出てこなかった。  
後は彼らが何を指してそう読んでいるのか、一括にしているのかなど、聞いてみないとわからない。  

---

## 破棄

https://ufcpp.net/study/csharp/cheatsheet/ap_ver7/#discard  
・discard : 破棄  

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

## プロパティ

<https://www.fenet.jp/infla/column/technology/c%E3%81%AE%E3%83%97%E3%83%AD%E3%83%91%E3%83%86%E3%82%A3%E3%82%92%E4%BD%BF%E3%81%84%E3%81%93%E3%81%AA%E3%81%9D%E3%81%86%EF%BC%81%E3%81%95%E3%81%BE%E3%81%96%E3%81%BE%E3%81%AA%E5%AE%9F%E8%A3%85%E6%96%B9/>  

ゲッターセッター→ありすぎると見づらい→自動生成してくれる。それを提供する仕組みがプロパティ。  
クラス外部から見るとメンバー変数のように振る舞い、 クラス内部から見るとメソッドのように振舞う。  

内部で使用する場合、メンバ変数に直接アクセスしていいのか、プロパティからアクセスすればいいのかどちらかわからなくなる。  
<https://teratail.com/questions/304645>  
一番それっぽい質問してる人がいたけど、全然回答になっていない。  
まぁ、どちらでもいいと言えばいいのだが、プロパティを通すと処理が走るので、その必要がないなら直接メンバ変数へのアクセスでいいのではないでしょうか？  
一番わかりやすいのは、外部に公開する必要がない場合∧内部でのみ保持できればいい場合。  
この場合はプロパティを定義せず、メンバ変数を定義して使えばいいだろう。  
それこそ、日次帳票でやった、印刷データ生成クラスのような感じだ。  

---

## セマフォ

<https://e-words.jp/w/%E3%82%BB%E3%83%9E%E3%83%95%E3%82%A9.html#:~:text=%E3%82%BB%E3%83%9E%E3%83%95%E3%82%A9%E3%81%A8%E3%81%AF%E3%80%81%E3%82%B3%E3%83%B3%E3%83%94%E3%83%A5%E3%83%BC%E3%82%BF%E3%81%A7,%E3%82%92%E8%A1%A8%E3%81%99%E5%80%A4%E3%81%AE%E3%81%93%E3%81%A8%E3%80%82>

``` txt
セマフォとは、コンピュータで並列処理を行う際、同時に実行されているプログラム間で資源（リソース）の排他制御や同期を行う仕組みの一つ。
当該資源のうち現在利用可能な数を表す値のこと。
```

セマフォを資源を使っているかどうか、その状態を表す信号機のようなものだと認識していたが、  
本来は並列処理がメインの概念みたいだ。  

萬君からセマフォって何？って聞かれたのと、「セマフォがタイムアウトしました。」ってエラーを見せられたので、調べてみた次第です。  
ドンピシャな回答があったので乗せておく。  
「tcp プロバイダー セマフォがタイムアウトしました。」  
<https://detail.chiebukuro.yahoo.co.jp/qa/question_detail/q14134357014>  

要約すると、A、B、Cってプロセスが並行してて、  
ある資源が全然解放されないって時にセマフォがタイムアウトエラーを発生させるってことらしい。  

---

## WPFのコードビハインドを介したリソースへのアクセス

<https://www.it-swarm-ja.com/ja/c%23/wpf%E3%81%AE%E3%82%B3%E3%83%BC%E3%83%89%E3%83%93%E3%83%8F%E3%82%A4%E3%83%B3%E3%83%89%E3%82%92%E4%BB%8B%E3%81%97%E3%81%9F%E3%83%AA%E3%82%BD%E3%83%BC%E3%82%B9%E3%81%B8%E3%81%AE%E3%82%A2%E3%82%AF%E3%82%BB%E3%82%B9/968279150/>

xaml resourcedictionary コード 参照  
割り勘の戻り修正をしている時に、FlexGridのセルテンプレート使ってチェックボックスを配置して実装していたが、  
勝手に見た目が変わってどうしようもなかったのでTriggerAction使って直接カラムを操作することにした。  
その時、背景色をResourceDictionaryから取得したほうがいいよな～と思って調べた内容。  
以下の1文で問題なく取得できたので備忘録として残しておく。  
`Application.Current.Resources["resourceName"];`  

``` C#
    // 使用例
    dutchTreatAmount.Background = (Brush)Application.Current.Resources["IsReadOnlyBackGroundColor"];
```

---

## virtual

virtualは継承先でoverride修飾子を使うことで処理を上書きできることを明示するための修飾子。  

---

## abstractとinterfaceの棲み分け

interfaceは外部向け。abstractは内部向け。  
interfaceはpublicしか記述できない。  
abstractはprotectedが使える。  
なので、外部に公開するわけでは無い処理において、実装を強制する際に用いるといいかも。  

---

## 親クラスの全プロパティの値を子クラスにコピーする方法

<https://qiita.com/microwavePC/items/54f0082f3d76922a6259>  

``` C#
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="parent"></param>
    public ExtendSearchCondition(SearchCondition parent)
    {
        // 親クラスのプロパティ情報を一気に取得して使用する。
        List<PropertyInfo> props = parent
            .GetType()
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            ?.ToList();
        foreach (var prop in props)
        {
            var propValue = prop.GetValue(parent);
            typeof(SearchCondition).GetProperty(prop.Name).SetValue(this, propValue);
        }
    }
```

---

## if (true) return; , if (true) hoge(); としてはいけない理由。

①2行に変更するとき漏れが生じる  
②returnを見落としがちになる。  
③条件が長いと後ろの文章が見えなくなる。  

---

## string.CompareTo

文字列のBETWEEN比較をするのに便利そうだなと思ってまとめ。  
とりあえずサンプルのように大小関係を取ればBETWEENになる。  

``` C#
// a.CompareTo(b)とした時
// aがbより小さい : -1 : ("9000").CompareTo("9001"),
// aとbが同じ     :  0 : ("9000").CompareTo("9000"),
// aがbより大きい :  1 : ("9000").CompareTo("8999"),
// 8999 9000 9001 ~~~ 9998 9999 10000
//  -1    0    1        1    0    -1
{
    if (accountNoRange.AccountNoFrom.CompareTo(accountNo) <= 0 && accountNoRange.AccountNoTo.CompareTo(accountNo) >= 0)
    {
    }
}
```

---

## プロパティーを参照渡ししてメソッド先で値を変更したい場合

<https://takap-tech.com/entry/2014/08/13/143232>  

施設売上報告書を作っている時に体験した事。  
SearchSimple○○系の処理はどれも似ている。  
FromToで使いまわしたいが、プロパティは固定で指定しないといけないので、本来ならFrom用、To用と作らないといけない。  
そんなことしたくないので、プロパティを渡そうとしたらエラーになった。  
`プロパティ、インデクサー、または動的メンバー アクセスを out または ref のパラメーターとして渡すことはできません。`  
参考URLでは「この挙動は自動実装プロパティが実際はsetter/getterメソッドを隠ぺいした存在という事に起因する。」ということらしい。  

仕方がないので、Actionを渡すことで対応出来た。  
参考にしたURLでは拡張メソッドで対応しているっぽいが、影響範囲がでかすぎるのでActionで済ませた。  
いいかは知らない。多分よくないはず。  
リフレクション使ってプロパティ名を渡して動的に対応してもらうってのもいいかも。  

<https://atmarkit.itmedia.co.jp/fdotnet/csharp30/csharp30_04/csharp30_04_01.html>  
まとめ終わった後に見つけた。  
似たようなことしてる。やっぱり苦肉の策っぽいですね。  

``` C#
// 自動実装プロパティ
public int No { get; set; }

// 呼び出し元
private Hoge() {
    Fuga(No);
}

// 参照渡し
// プロパティ、インデクサー、または動的メンバー アクセスを out または ref のパラメーターとして渡すことはできません。
private Fuga(ref int no){
    no = 1;
}
```

``` C#
// 自動実装プロパティ
public int No { get; set; }

// 呼び出し元
private Hoge() {
    Fuga((no) => No = no);
}

// Actionで実現
// プロパティ、インデクサー、または動的メンバー アクセスを out または ref のパラメーターとして渡すことはできません。
private Fuga(Action<int> action){
    action(1);
}
```
