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

intはstructなので値型。
objectはclassなので参照型。
intをobject型に代入する場合、値型から参照型への変換が必要。
その時の変換処理がBox化(Boxing)。
値型はスタック領域に配置される。
参照型はヒープ領域に配置される。
intをobject型に代入すると、ヒープ領域に新しい領域が確保され、その領域にスタックの値をコピーする。元の値が何型だったのかの情報も含まれる。
スタック領域には新しく確保したヒープ領域へのポインタ情報を持ったスタックが確保される。
Box化解除(Unboxing)はポインタの参照先から値を取り出してスタックを新しく確保する。

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

### プロパティってそもそも何？

そのクラスのプライベートフィールドの値の読み取り、書き込み等を行うメンバー。  
Javaにはプロパティは存在しないらしい。  
なので、クラスのプライベートフィールドへのアクセスはGetterとSetterを自分で実装しないといけない。  
そうすると、コード量がすごいことになるのでそれを軽減するための仕組みがプロパティという印象を受けた。  

<https://teratail.com/questions/304645>  

``` txt
(1) オブジェクト指向の概念の一つ「カプセル化」を実現するため、通常クラス内の各フィールドへの直接アクセスは禁止するようにしておき、
外部からはパブリックプロパティで各フィールドの値を取得したり設定したりするということがもともとのプロパティの目的です。 

(2) プロパティを使う目的には、開発者が意図した規則に基づいてフィールドを正しく使用できるよう保証するということもあります。

(3) プロパティでなければダメというケースもあります。
例えば、Entity Framework Code First でのモデルを定義を行う場合はフィールドではダメで、プロパティの定義が必要です。
他には、ASP.NET Web Forms アプリのデータバインド式でもプロパティでないとダメです。
```

<https://qiita.com/toshi0607/items/801a0d37fb48313cbdbd>  
1. フィールド
- オブジェクト指向について「クラスは、データと振る舞いをカプセル化したものである」と説明されるときの「データ」の部分です。
- オブジェクトが持つデータをフィールドとして定義します。
- フィールドはクラスのメンバ（クラスなど、型を構成する内部要素の総称）として宣言された変数で、インスタンスと直接結び付けられます。
- メンバ変数とも呼ばれます。
- フィールドはインスタンスからアクセスします。
- **非公開にし、プロパティで操作するのが原則です。**
- フィールドと3.プロパティの混乱を避けるためにアンダースコア（_）をつけて定義することがあります。
→  
答えがあったぞ。特に理由が無ければプロパティ経由でアクセスしておけって話か。  

3. プロパティ
- オブジェクト内にあるフィールドの値を取得、または設定するための手段です。
- クラス外部から見るとメンバー変数のように振る舞い、 クラス内部から見るとメソッドのように振舞います。
- メンバー変数の値の取得・変更を行うためのメソッドのことをアクセサー(accessor)といいます。
- setterに渡す値にはsetter内からvalueという変数でアクセスできます。

### 内部で使用する場合、メンバ変数に直接アクセスしていいのか、プロパティからアクセスすればいいのかどっちがいいの？

<https://teratail.com/questions/304645>  
「クラス内からであっても、private のメンバー変数には直接アクセスせず、 プロパティを通してアクセスする方が後々の保守がしやすかったりします」  
→  
クラスの内側でも「プロパティ」による抽象化の恩恵に与りたいのか否か，程度の話なんじゃないかな，と。  
→  
日次帳票でやった、内部クラス(印刷データ生成クラス)なら、直接フィールドでやり取りしていいのではないだろうか。  
外部に公開するわけでもないし、内部で使うだけだし、抽象化する必要性もないし、そういう場合はフィールドでよさそうな気はする。  
それ以外はプロパティ自体をprivateで宣言して使えば、フィールドを使っているのと同じ様なものでは無かろうか。  
しかし、プロパティのほうが参照元を表示してくれるので、プロパティを使ったほうが便利といえば便利。  

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

---

## varの意味

型を同じにしてくれる。  
暗黙の型指定
コンパイラは右側の値からデータ型を推測して決定します。
この仕組みを「型推論」と呼びます。

---

## ObservableCollection

[【WPF】ItemsSourceにObservableCollectionを選ぶのはなぜ](https://itwebmemory.com/itemssource-observablecollection-explanation)  
なんとなく使ってはいるが、なぜ使うのか、どういうものなのかずっとわからなかったのでまとめることにした。  

```
項目が追加または削除されたとき、あるいはリスト全体が更新されたときに通知を行う動的なデータ コレクションを表します。
https://docs.microsoft.com/ja-jp/dotnet/api/system.collections.objectmodel.observablecollection-1?view=netcore-3.1
```

ざっくりいうと、INotifyPropertyChangedを実装したリストっぽい。  
正確には、INotifyCollectionChangedインターフェイスを実装したデータコレクションの組み込み実装クラス。  
というわけで、View側からの項目の追加、削除、内容の変更を観測できるし、ViewModel側からView側へ逆に反映させることができるわけだ。  
だから、項目の追加や削除が必要な場合には、ObservableCollectionを採用するというわけね。  

---

## プレースホルダー(文字列補完)

まさかこんな基本的な事がわかっていなかったなんて・・・  
割とショックである。  

基本は`{インデックス}`で置き換え場所を指定する事。  
中かっこ自体を表示する方法も地味にわからなかったが、それは`{{`でエスケープすればよかった。  

<https://buralog.jp/csharp-string-interpolation/>  

``` C# 5.0以前
string stationaries = "Pen";
string fruits = "Pineapple Apple";
// 表示するインデックスが多すぎると、わけわからなくなる。
MessageBox.Show(string.Format("PPAPとは{0} {1} {2}の略である。", stationaries, fruits, stationaries));
// string.Formatを噛まさなくても、Console.Write直でいける。
Console.Write("PPAPとは{0} {1} {2}の略である。", stationaries, fruits, stationaries));
```

``` C# 6.0以降
string stationaries = "Pen";
string fruits = "Pineapple Apple";
// ${}で補完を行う。業務で使ってる形がこれ。こちらのほうが直観的でよい。
// これができるならインデックスで指定する方法は使わなくていいだろう。
// もちろんConsole.Writeでも同じように記述可能。
MessageBox.Show($"PPAPとは{stationaries} {fruits} {stationaries}の略である。");
```

``` C# {}を文字列内に含める
// {{、または}}とする。
string ppap = "Pen Pineapple Apple Pen ";
MessageBox.Show($"{{{ppap}}}");
// 結果は{Pen Pineapple Apple Pen}と表示されます。
```

---

## インターフェースのインスタンス

東さんの小話で出てきた話題。  
Javaだと匿名クラスでのみInterfaceのインスタンスを作成できるって話だったが、聞いているだけではちょっとイメージできなかったのでまとめてみた。  

[[Java] インターフェースをnewする違和感が解決した](https://qiita.com/imanishisatoshi/items/f73abc8206f405970d4f)  

ネタばれすると「インターフェースを継承した名前の無いクラスをnewしていただけ」という落ちだったが、面白い発見だった。  

---

## 三項演算子で同じインタフェースを実装したクラスがなぜ暗黙的変換といわれるのか

java5以降改善されたらしい。その前までは同様の現象が起こっていた模様。  
C#も今後改善されるのかな。  

---

## ASP.NetのWeb APIのパラメーターでタプルを渡す

予約枠台帳の修正やってる時に、事業者コードと日付だけのパラメーターのためにクラスを用意するのが面倒で、タプルとか匿名型を渡せないか調べた。  
匿名型は型が分からないのであれだが、せめてタプルと思ったら行けたのでまとめる。いいかどうかはしらん。  

``` C#
    /// <summary>
    /// フロント側
    /// </summary>
    public async Task<TRe_ReservationBasicInfo> GetBasicInfo(string officeCD, DateTime businessDate)
    {
        return await _Accessor.GetResultAsync<TRe_ReservationBasicInfo>(PATH + "/get/basic_info", new ValueTuple<string, DateTime>(officeCD, businessDate));
    }

    /// <summary>
    /// API側
    /// </summary>
    [HttpPost("get/basic_info")]
    public TRe_ReservationBasicInfo GetBasicInfo([FromBody] (string OfficeCD, DateTime BusinessDate) key)
    {
        return _ReservationFrameModel.GetBasicInfo(key.OfficeCD, key.BusinessDate);
    }
```
