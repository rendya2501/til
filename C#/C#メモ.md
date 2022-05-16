# C#メモ

## アノテーションを使った、リストに1件もない場合のバリデーション

<https://stackoverflow.com/questions/5146732/viewmodel-validation-for-a-list>  
画面にエラー状態は表示したくないけど、警告は出したい場合があったのでその備忘録。  

``` C#
// 最小値、最大値、エラーメッセージ
[Range(1, int.MaxValue, ErrorMessage = "At least one item needs to be selected")]
// GetOnlyの省略形の書き方
public int ItemCount => Items != null ? Items.Length : 0;

// boolの判定もいける
// こっちは型の指定、最小値、最大値、エラーメッセージ
[Range(typeof(bool), "true", "true", ErrorMessage = "残高は0で保存してください。")]
public bool IsZeroBalanceAmount => BalanceAmount == 0;
```

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
>元々のis演算子の仕様でもあるんですが、nullには型がなくて常にisに失敗します(falseを返す)。  

なるほどね。  
is演算子は実行時の変数の中身を見るが、nullは型がないので、エラーになるわけか。  

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

``` C# : 少し応用してキーバリューで出力するサンプル
    var props = condition
        .GetType()
        .GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public)
        .Select(s => (s.Name ,s.GetValue(condition)))
        .ToList();
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

## プレースホルダー(文字列補完)

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

## プレースホルダー中におけるToStringFormatの指定

[【C#6.0～】文字列補間（$を使った文字列書式設定）](https://imagingsolution.net/program/string_interpolation/)  

いつぞや、いつも`.ToString("N0")`ってフォーマット書くところ`:N0`で書けることを発見したのでまとめ。  
プレースホルダー中でのフォーマットの指定はコロン指定ができるができるらしい。便利。  
C#Ver6からの機能みたい。結構実装されてから経っているのね。  

因みにToStringするときに文字列で指定するこれは、「書式設定」というらしい。  

``` C#
// SettlementAmount.ToString("N0") → SettlementAmount:N0
$"支払額{SettlementAmount:N0}円を人数{TargetPlayerCount}人で均等に割り付けます。{Environment.NewLine}よろしいですか？"
```

---

## インターフェースのインスタンス

東さんの小話で出てきた話題。  
Javaだと匿名クラスでのみInterfaceのインスタンスを作成できるって話だったが、聞いているだけではちょっとイメージできなかったのでまとめてみた。  

[[Java] インターフェースをnewする違和感が解決した](https://qiita.com/imanishisatoshi/items/f73abc8206f405970d4f)  

ネタばれすると「インターフェースを継承した名前の無いクラスをnewしていただけ」という落ちだったが、面白い発見だった。  
後日、基本情報の勉強をしているとこれが出てきた。  
普通に基礎レベルの内容だったらしい。  

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
        // 多分普通に(officeCD,businessDate)でいいと思う。
        return await _Accessor.GetResultAsync<TRe_ReservationBasicInfo>(PATH + "/get/basic_info", new ValueTuple<string, DateTime>(officeCD, businessDate));
        // こっちでも普通にいけた
        // return await _Accessor.GetResultAsync<TRe_ReservationBasicInfo>(PATH + "/get/basic_info", (officeCD, businessDate));
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

---

## Tuple,ValueTuple

ValueTupleがある今、Tupleを使う意味はないけれど、知れば知るほどValueTupleとの違いがわからなくなったので、今のうちにまとめておく。  
正直ValueTupleだけ知っていれば十分だ。  
Tupleはまとめるのだるいのでリンクだけ貼っておく。  
いちいち「Tuple.」って付けないといけないからわかりやすいだろう。  

[C# - ValueTuple](https://www.tutorialsteacher.com/csharp/valuetuple)  
[C# - Tuple](https://www.tutorialsteacher.com/csharp/csharp-tuple)  

``` C# : ValueTuple初期化
// 要素が全てitemになるタイプの初期化
var person = (1, "Bill", "Gates");

ValueTuple<int, string, string> person = (1, "Bill", "Gates");

(int, string, string) person = (1, "Bill", "Gates");


// 要素名を付けるタイプの初期化
(int Id, string FirstName, string LastName) person = (1, "Bill", "Gates");

var person = (Id:1, FirstName:"Bill", LastName: "Gates");

(int Id, string FirstName, string LastName) person = (PersonId:1, FName:"Bill", LName: "Gates");

(string, string, int) person = (PersonId:1, FName:"Bill", LName: "Gates");

string firstName = "Bill", lastName = "Gates";
var per = (FirstName: firstName, LastName: lastName);
```

### Tuple?ValueTuple?引数に渡す方法

WebAPIに渡すパラメーターでタプルを使ったのはいいけれど、本当にタプルなのか？バリュータプルと何が違うのか？  
そもそも渡し方と受け取り型ってこれでいいの？ってのがわからなかったのでまとめた。  
だけど、もっとまとめる必要がありそう。まだ全然まとめきれていない。  
とりあえず、下の渡し方は全てOKだった。  
他にもやらないといけないから、今はこれで勘弁して。  

``` C#
    ValueTupleArgumentTest(("a", 1));
    ValueTupleArgumentTest((str: "b", i: 2));
    ValueTupleArgumentTest(new ValueTuple<string, int>("c", 3));
    ValueTupleArgumentTest(ValueTuple.Create("d", 4));
    // これはダメ
    // ValueTupleArgumentTest(Tuple.Create("d", 4));

    // 引数はValueTuple<string,int>型
    private static void ValueTupleArgumentTest((string str, int i) key)
    {
        Console.WriteLine(key.str);
        Console.WriteLine(key.i);
    }
```

---

## Switch式でメソッドを実行できないか色々やった結果

<https://stackoverflow.com/questions/59729459/using-blocks-in-c-sharp-switch-expression>

if elseif でがりがり書くなら、switchの出番なんじゃないかと思ってやってみた。  
結果的に出来なかった。  
文法上は怒られないのだが、実際に動かしてみると"Operation is not valid due to the current state of the object."と言われて動かない。  

switch式は絶対にreturnがないといけない形式なので、Actionとして受け取る必要がある。  
若干わかりにくいけど、スマートだから妥協できないかと思ったけど、そもそも動かないのであれば、switchの1行表示で妥協するしかなさそうだ。  

``` C#
/// <summary>
/// データプロパティ変更時イベント
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
private void DataPropertyChanged(object sender, PropertyChangedEventArgs e)
{
    // 元のコード。{}がない分まだすっきりしているが、e.PropertyNameを毎回書かないといけないなら,それはswitchを使うべき。
    // 部門コード
    if (e.PropertyName == nameof(Data.DepartmentCD))
        SetDepartmentCD();
    // 商品コード
    else if (e.PropertyName == nameof(Data.ProductCD))
        SetProductCD();
    // 商品分類コード
    else if (e.PropertyName == nameof(Data.ProductClsCD))
        SearchProductCls();

    // 1行にまとめたswitch。普通のよりは断然いいが、breakを毎回書かないといけない。
    // 言語仕様的に仕方がないが・・・。
    switch (e.PropertyName)
    {
        // 部門コード
        case nameof(Data.DepartmentCD): SetDepartmentCD(); break;
        // 商品コード
        case nameof(Data.ProductCD): break;
        // 商品分類コード
        case nameof(Data.ProductClsCD): break;
        default: break;
    }

    // 本命。文法上は怒られないけど実行するとエラーになる。
    // 実行するのではなく,Actionデリゲートとして受け取り、最後に実行する。
    Action result = e.PropertyName switch
    {
        // 部門コード
        nameof(Data.DepartmentCD) => SetDepartmentCD,
        // 商品コード
        nameof(Data.ProductCD) => SetProductCD,
        // 商品分類コード
        nameof(Data.ProductClsCD) => SearchProductCls,
        _ => throw new InvalidOperationException()
    };
    result();
}
```

後日。改めてやったら出来たのでまとめる。

``` C#
    /// <summary>
    /// スイッチ式サンプル1
    /// スイッチ式使ってActionを受け取って実行って事ができると、処理をifで切り替えずに済むのでは？と思ってやってみた。
    /// 昔やったときは出来かったはずだが、できるようになってるっぽい。
    /// </summary>
    /// <param name="args"></param>
    static void Main(string[] args)
    {
        // 適当な分岐のつもり
        int flag = 1;

        // 1.Actionデリゲートとして受け取るタイプ
        Action result = flag switch
        {
            1 => Test1,
            2 => Test2,
            _ => throw new InvalidOperationException()
        };
        result.Invoke();

        // 2.そもそも関数として切り離したタイプ
        Action(flag).Invoke();

        // 3.どれか1つでもキャストするとvarでもいけることがわかった。
        // でもって、こっちだと.Invoke()で起動できる。
        // 勘違いだ。a()とするかa.Invoke()とするかの違いでしかなかった。
        var a = flag switch
        {
            1 => (Action)Test1,
            2 => Test2,
            _ => throw new InvalidOperationException()
        };
        a.Invoke();

        // 4.そもそもActionとして受け取らないで即時実行するタイプ
        (flag switch
        {
            1 => (Action)Test1,
            2 => Test2,
            _ => throw new InvalidOperationException()
        }).Invoke();
        // 何とか1行に出来なくはないが・・・。ないだろうなぁ。
        (flag switch { 1 => (Action)Test1, 2 => Test2, _ => throw new Exception() }).Invoke();
    }

    /// <summary>
    /// 2.そもそも関数として切り離したタイプ
    /// </summary>
    /// <param name="flag"></param>
    /// <returns></returns>
    static Action Action(int flag) => flag switch
    {
        1 => Test1,
        2 => Test2,
        _ => throw new InvalidOperationException()
    };

    /// <summary>
    /// テストメソッド1
    /// </summary>
    static void Test1() => Console.WriteLine("test1");
    /// <summary>
    /// テストメソッド2
    /// </summary>
    static void Test2() => Console.WriteLine("test2");
```

---

## アトリビュートに設定されている文字列の長さを取得する方法

``` C#
/// <summary>
/// ProductCDの桁数を取得
/// </summary>
private static readonly int ProductCDLength = (Attribute.GetCustomAttribute(typeof(TMa_Product).GetProperty(nameof(TMa_Product.ProductCD)), typeof(StringLengthAttribute)) as StringLengthAttribute)?.MaximumLength ?? 20;
```

---

## インスタンスの状態

[[C# 入門] クラスのインスタンスについて](https://yaspage.com/prog/csharp/cs-instance/)  

DependencyPropertyのBindingの件で東さんがインスタンスの状態なんて事を言っていたので調べたわけだが、  
それとは別にインスタンスについての基礎を紹介しているページがわかりやすかったのでまとめる。  

後日、東さんがまた使っていたので、意味合いから想像すると、例えばLinqで.Where()を実行したときと、.ToList()を実行したときでは、戻ってくる値が違うわけで、  
東さんはどうやら帰ってくる型の事をインスタンスの状態と言っている見たいだ。  

---

## マニフェストファイルとは？

アプリケーション マニフェスト [application manifest]  
アプリケーションおよびそのすべての構成ファイルを記述するファイル。ClickOnce アプリケーションによって使用されます。  

俺が調べたい内容はこんな事だっただろうか。  
発行の時に追加されるあれについてだった気がする。  
それ以外の意味では、見た目に関する事だとか、管理者権限を付与するものだとか、他のアセンブリとの接続情報を保持するものだとか、  
そういったメタデータの集まり的なファイルの模様。  
まぁ、でも意味合い的にはそれだけで十分な気がするけどね。  

[C#メモ Manifestファイルを追加してフォームの表示がぼやっとしているのをはっきりさせてみる](https://www.tetsuyanbo.net/tetsuyanblog/45990)  
どういう仕組みなのかは知らないが、マニフェストファイルを追加するだけで文字のぼやけが解消するらしい。  
ここら辺は必要になったらまた調べることに鳴るだろう。  

---

## ジェネリックにnullを指定する方法

東さんに質問された内容。ジェネリックは弱いのでまとめる。  

Javaの時にやったジェネリックに基づけば、ArrayList等においてジェネリックは入れる型を限定し、取り出すときはキャストいなくていい仕組みであったはず。  
あらかじめ実行する処理は定義しておいて、型はその時々で柔軟に切り替える仕組みがジェネリック。  
なので、絶対に型は必要。  
しかし、nullは型がない。  
いつだかis演算子でnullを判定した場合、nullには型がないので常にfalseになる、ってまとめた気がする。  
型が必要なのに型がないのでそもそも無理な話なのだ。  
結局、Objectを指定することで解決したらしい。  

C#8.0からではあるが、null許容型が追加されたらしい。  
Framerwork4.8の7.3どまりのフロントではもちろんエラーになる。  

``` C#
    static void Class<T>() where T : class { }
    static void NullableClass<T>() where T : class? { }
    
    static void Main()
    {
        Class<string>();
        // C#8.0では警告みたいなのが出るが普通に行けることを確認した。
        NullableClass<string?>();
        // 結局渡したい物が何もないからnullにしたいだけであって、それならということでobjectを渡して何とかしたみたい。
        Class<object>();
    }
```

---

## decimalに1をかけて意味があるのか？

2021/11/13 Sat  
福田さんがおもむろに1を掛けているコードを発見した。  
意味があるようには見えなかったので、聞いてみたが、どうやら数量を掛けているつもりだったらしい。  
しかし、小数点があったら四捨五入されるなんて言っていたが、本当だろうかと確かめてみた。  
されなかった。  
やっぱり意味がなかった。  

``` C#
    decimal? test = (decimal?)Math.PI;
    test = test * 1;
    test = (decimal?)2.5252525252;
    test = test * 1;
```

---

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

## イミディエイト クリップボード コピー

`Clipboard.SetText(コピーしたい文字列)`  

[クリップボードに文字列をコピーする、クリップボードから文字列を取得する](https://dobon.net/vb/dotnet/string/clipboard.html)  
Clipboard.SetTextメソッドを使えば、簡単に文字列をクリップボードにコピーできます。  

VB6で長いSQLを取得する時にお世話になったでそ。  
最新の環境でも普通に使えたのでメモ。  
これでStringBuilderをToStringしたときに、長ったらしく横に展開されるあれも楽に取ることができる。  
内容的にVisualStudio側に置いてもいいのだが、とりあえずC#のほうに置いておく。  

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

## C#でURLを既定のブラウザで開く

[C#でURLを既定のブラウザで開く](https://qiita.com/tsukasa_labz/items/80a94d202f5e88f1ddc0)  

なんかうまいこといかない場合もある模様。  
次必要になったらまとめる。  

``` C#
Process.Start(
    new ProcessStartInfo()
    {
        FileName = "https://www.google.co.jp",
        UseShellExecute = true,
    }
);
```

---

## StringBuilderで先頭にWHEREを追加する方法

``` C#
            var whereQuery = new StringBuilder();
            if (!string.IsNullOrEmpty(settlementPlayerNo))
            {
                whereQuery.AppendLine("AND [SettlementPlayerNo] = @settlementPlayerNo");
            }
            if (!string.IsNullOrEmpty(settlementID))
            {
                whereQuery.AppendLine("AND [SettlementID] = @settlementID");
            }
            // 現金振替マイナスの伝票は除く
            if (isExclueCashAdvanceMinus)
            {
                whereQuery.AppendLine("AND [DetailType] <> @cashAdvanceMinus");
            }
            // 先頭のAND消してWHEREにする。
            whereQuery.Remove(0, 3).Insert(0, "WHERE");
```

---

## asyncのデリゲート定義

[async/await を使った非同期ラムダ式を変数に代入する方法](https://qiita.com/go_astrayer/items/352c34b8db72cf2f6ca5)  

``` C#
 Func<ListReplyContext, Task> callback = async res => {}
```

---

## Foreachでnullが来ても大丈夫な書き方はないか

[foreachの時のNullReferenceExceptionを回避する](https://tiratom.hatenablog.com/entry/2018/12/16/foreach%E3%81%AE%E6%99%82%E3%81%AENullReferenceException%E3%82%92%E5%9B%9E%E9%81%BF%E3%81%99%E3%82%8B)  

foreachをやる前にifでnullチェックするのが野暮ったく感じたし、インデントが深くなってしまうのでなんかいい書き方はないかなということで探した。  
前にも調べたはずだが、そこそこ需要あると思うので備忘録としてまとめる。  

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

## DateTimeで年月日だけ取得する

[日時（DateTimeオブジェクト）の情報を取得する](https://dobon.net/vb/dotnet/system/datetime.html)  

GrapeCityのGcDateのFormatが効かず、時間も入ってきてうまく検索できない問題が発生して、年月日だけ取得する書き方が分からなかったので調べた。  
こういうのって地味にわからない。  

``` C#
//2000年9月30日13時15分30秒を表すDateTimeオブジェクトを作成する
DateTime dt = new DateTime(2000, 9, 30, 13, 15, 30);
//「2000/09/30 0:00:00」のDateTimeオブジェクトを作成する
DateTime dtd = dt.Date;

//nullの場合も対応できる
DateTime? dt = null;
var test = dt?.Date;
```

---

## Like検索(曖昧検索)

[LINQ：文字列コレクションで「LIKE検索」（部分一致検索）をするには？［C#、VB］](https://atmarkit.itmedia.co.jp/ait/articles/1412/02/news129.html)  

``` C#
    // LIKE '%ぶた%'：String.Containsを使う
    var 前後パーセント = sampleData.Where(item => item.Contains("ぶた"));
    WriteItems("LIKE '%ぶた%'", 前後パーセント);
    // → LIKE '%ぶた%': ぶた, こぶた, ぶたまん, ねぶたまつり
    
    // LIKE 'ぶた%'：String.StartsWithを使う
    var 後パーセント = sampleData.Where(item => item.StartsWith("ぶた"));
    WriteItems("LIKE 'ぶた%'", 後パーセント);
    // → LIKE 'ぶた%': ぶた, ぶたまん

    // LIKE '%ぶた'：String.EndsWithを使う
    var 前パーセント = sampleData.Where(item => item.EndsWith("ぶた"));
    WriteItems("LIKE '%ぶた'", 前パーセント);
    // → LIKE '%ぶた': ぶた, こぶた
```

---

## プロパティのSwap奮闘記

FrmaeのPlayer1とPlayer2の入れ替えを実現するために色々やったのでまとめ。  
普通のスワップなら気にする必要もないのだが、プロパティの中身を全て入れ替える場合は単純なSwapではうまく行かなかった。  

``` C# : 最初に作った駄目な奴
    private void CopyRepre(ReservationPlayerView obj)
    {
        if (obj == null)
        {
            return;
        }

        void Swap(ref ReservationPlayerView x, ref ReservationPlayerView y)
        {
            ReservationPlayerView tmp = y;
            y = x;
            x = tmp;
        }

        // temp代表者
        ReservationPlayerView TempRepre = null;
        // 代表者を押した場所にする
        foreach (var frame in SelectedReservation.ReservationFrameList)
        {
            Func<ReservationPlayerView, ReservationPlayerView> Selected = (select) =>
            {
                if (frame.ReservationPlayer1 == select)
                {
                    return frame.ReservationPlayer1;
                }
                if (frame.ReservationPlayer2 == select)
                {
                    return frame.ReservationPlayer2;
                }
                if (frame.ReservationPlayer3 == select)
                {
                    return frame.ReservationPlayer3;
                }
                if (frame.ReservationPlayer4 == select)
                {
                    return frame.ReservationPlayer4;
                }
                return null;
            };


            if (frame.ReservationPlayer1 != obj && (frame.ReservationPlayer1?.ReservationRepreFlag ?? false))
            {
                //TempRepre = frame.ReservationPlayer1;
                //frame.ReservationPlayer1 = obj;
                TempRepre = frame.ReservationPlayer1;
                var TempSelect = Selected(obj);
                frame.ReservationPlayer1 = TempSelect;
                TempSelect = TempRepre;

                frame.ReservationPlayer1.ReservationRepreFlag = false;
            }
            if (frame.ReservationPlayer2 != obj && (frame.ReservationPlayer2?.ReservationRepreFlag ?? false))
            {
                //TempRepre = frame.ReservationPlayer2;
                //frame.ReservationPlayer2 = obj;
                TempRepre = frame.ReservationPlayer2;
                var TempSelect = Selected(obj);
                frame.ReservationPlayer2 = TempSelect;
                TempSelect = TempRepre;

                frame.ReservationPlayer2.ReservationRepreFlag = false;
            }
            if (frame.ReservationPlayer3 != obj && (frame.ReservationPlayer3?.ReservationRepreFlag ?? false))
            {
                if (frame.ReservationPlayer1 == obj)
                {
                    TempRepre = frame.ReservationPlayer3;
                    frame.ReservationPlayer3 = frame.ReservationPlayer1;
                    frame.ReservationPlayer1 = TempRepre;
                }
                if (frame.ReservationPlayer2 == obj)
                {
                    TempRepre = frame.ReservationPlayer3;
                    frame.ReservationPlayer3 = frame.ReservationPlayer2;
                    frame.ReservationPlayer2 = TempRepre;
                }
                if (frame.ReservationPlayer3 == obj)
                {
                    TempRepre = frame.ReservationPlayer3;
                    frame.ReservationPlayer3 = frame.ReservationPlayer3;
                    frame.ReservationPlayer2 = TempRepre;
                }
                if (frame.ReservationPlayer4 == obj)
                {
                    TempRepre = frame.ReservationPlayer3;
                    frame.ReservationPlayer3 = frame.ReservationPlayer4;
                    frame.ReservationPlayer4 = TempRepre;
                }
                frame.ReservationPlayer3.ReservationRepreFlag = false;
            }
            if (frame.ReservationPlayer4 != obj && (frame.ReservationPlayer4?.ReservationRepreFlag ?? false))
            {
                //TempRepre = frame.ReservationPlayer4;
                //frame.ReservationPlayer4 = obj;
                TempRepre = frame.ReservationPlayer4;
                var TempSelect = Selected(obj);
                frame.ReservationPlayer4 = TempSelect;
                TempSelect = TempRepre;

                frame.ReservationPlayer4.ReservationRepreFlag = false;
            }
        }
    }
```

``` C# : 愚直にやって何とか形にしたやつ
    /// <summary>
    /// 代表者入れ替え
    /// </summary>
    private void SwapRepre(ReservationPlayerView obj)
    {
        if (obj == null)
        {
            return;
        }
        // 今の代表者と押した場所を入れ替える
        foreach (var repre in SelectedReservation.ReservationFrameList)
        {
            // 今の代表者を観測する
            if (repre.ReservationPlayer1 != obj && (repre.ReservationPlayer1?.ReservationRepreFlag ?? false))
            {
                // 今の代表者を観測したら、押された場所を観測する
                foreach (var pushFrom in SelectedReservation.ReservationFrameList)
                {
                    // 押された場所を観測できたら入れ替えを実行する
                    if (pushFrom.ReservationPlayer1 == obj)
                    {
                        (repre.ReservationPlayer1, pushFrom.ReservationPlayer1) = (pushFrom.ReservationPlayer1, repre.ReservationPlayer1);
                    }
                    else if (pushFrom.ReservationPlayer2 == obj)
                    {
                        (repre.ReservationPlayer1, pushFrom.ReservationPlayer2) = (pushFrom.ReservationPlayer2, repre.ReservationPlayer1);
                    }
                    else if (pushFrom.ReservationPlayer3 == obj)
                    {
                        (repre.ReservationPlayer1, pushFrom.ReservationPlayer3) = (pushFrom.ReservationPlayer3, repre.ReservationPlayer1);
                    }
                    else if (pushFrom.ReservationPlayer4 == obj)
                    {
                        (repre.ReservationPlayer1, pushFrom.ReservationPlayer4) = (pushFrom.ReservationPlayer4, repre.ReservationPlayer1);
                    }
                }
                break;
            }
            if (repre.ReservationPlayer2 != obj && (repre.ReservationPlayer2?.ReservationRepreFlag ?? false))
            {
                foreach (var pushFrom in SelectedReservation.ReservationFrameList)
                {
                    if (pushFrom.ReservationPlayer1 == obj)
                    {
                        (repre.ReservationPlayer2, pushFrom.ReservationPlayer1) = (pushFrom.ReservationPlayer1, repre.ReservationPlayer2);
                    }
                    else if (pushFrom.ReservationPlayer2 == obj)
                    {
                        (repre.ReservationPlayer2, pushFrom.ReservationPlayer2) = (pushFrom.ReservationPlayer2, repre.ReservationPlayer2);
                    }
                    else if (pushFrom.ReservationPlayer3 == obj)
                    {
                        (repre.ReservationPlayer2, pushFrom.ReservationPlayer3) = (pushFrom.ReservationPlayer3, repre.ReservationPlayer2);
                    }
                    else if (pushFrom.ReservationPlayer4 == obj)
                    {
                        (repre.ReservationPlayer2, pushFrom.ReservationPlayer4) = (pushFrom.ReservationPlayer4, repre.ReservationPlayer2);
                    }
                }
                break;
            }
            if (repre.ReservationPlayer3 != obj && (repre.ReservationPlayer3?.ReservationRepreFlag ?? false))
            {
                foreach (var pushFrom in SelectedReservation.ReservationFrameList)
                {
                    if (pushFrom.ReservationPlayer1 == obj)
                    {
                        (repre.ReservationPlayer3, pushFrom.ReservationPlayer1) = (pushFrom.ReservationPlayer1, repre.ReservationPlayer3);
                    }
                    else if (pushFrom.ReservationPlayer2 == obj)
                    {
                        (repre.ReservationPlayer3, pushFrom.ReservationPlayer2) = (pushFrom.ReservationPlayer2, repre.ReservationPlayer3);
                    }
                    else if (pushFrom.ReservationPlayer3 == obj)
                    {
                        (repre.ReservationPlayer3, pushFrom.ReservationPlayer3) = (pushFrom.ReservationPlayer3, repre.ReservationPlayer3);
                    }
                    else if (pushFrom.ReservationPlayer4 == obj)
                    {
                        (repre.ReservationPlayer3, pushFrom.ReservationPlayer4) = (pushFrom.ReservationPlayer4, repre.ReservationPlayer3);
                    }
                }
                break;
            }
            if (repre.ReservationPlayer4 != obj && (repre.ReservationPlayer4?.ReservationRepreFlag ?? false))
            {
                foreach (var pushFrom in SelectedReservation.ReservationFrameList)
                {
                    if (pushFrom.ReservationPlayer1 == obj)
                    {
                        (repre.ReservationPlayer4, pushFrom.ReservationPlayer1) = (pushFrom.ReservationPlayer1, repre.ReservationPlayer4);
                    }
                    else if (pushFrom.ReservationPlayer2 == obj)
                    {
                        (repre.ReservationPlayer4, pushFrom.ReservationPlayer2) = (pushFrom.ReservationPlayer2, repre.ReservationPlayer4);
                    }
                    else if (pushFrom.ReservationPlayer3 == obj)
                    {
                        (repre.ReservationPlayer4, pushFrom.ReservationPlayer3) = (pushFrom.ReservationPlayer3, repre.ReservationPlayer4);
                    }
                    else if (pushFrom.ReservationPlayer4 == obj)
                    {
                        (repre.ReservationPlayer4, pushFrom.ReservationPlayer4) = (pushFrom.ReservationPlayer4, repre.ReservationPlayer4);
                    }
                }
                break;
            }
        }
    }
```

``` C# : 愚直に作った後、家で閃いたやつ
// プロパティを特定したうえでの代入はできるんだったら、デリゲートをフルに使って何とかできなかったのか？と今更ながらに思う。
var repreTemp = null;
var sourceTemp = null;
Action repre = null;
Action source = null;

foreach(frame){
    // 現在の代表者の場所を求める
    if (player1) {
        repreTemp = player1;
        repre = (obj) => player1 = obj;
    }
    // 今回押された場所を特定する
    if (player3 == obj) {
        sourceTemp = player3
        source = (repre) => player3 = repre;
    }
}

// 今回押された場所を代表者にする
repre(obj); //or repre(sourceTemp);
// 代表者だった場所に押された情報をいれる。
source(repreTemp);
```

``` C# : 閃いたアイデアを取り入れて実現できたパターン
    /// <summary>
    /// 代表者入れ替え
    /// </summary>
    /// <param name="obj">押された場所のプレーヤー情報。ReservationPlayer[n]</param>
    private void SwapRepre(ReservationPlayerView obj)
    {
        if (obj == null)
        {
            return;
        }
        // 一時的な代表者を格納
        ReservationPlayerView tempRepre = null;
        // 「押された場所→代表者」にするアクション
        Action<ReservationPlayerView> beRepreAction = null;
        // 「代表者→押された場所」にするアクション
        Action<ReservationPlayerView> beSourceAction = null;
        // 枠の1行単位でループ
        foreach (ReservationFrameView frame in SelectedReservation.ReservationFrameList)
        {
            // 今の代表者を特定する
            if (frame.ReservationPlayer1 != obj && (frame.ReservationPlayer1?.ReservationRepreFlag ?? false))
            {
                tempRepre = frame.ReservationPlayer1;
                // 今の代表者は押された場所と入れ替えるのでbeSourceActionを登録する
                beSourceAction = (source) => frame.ReservationPlayer1 = source;
            }
            else if (frame.ReservationPlayer2 != obj && (frame.ReservationPlayer2?.ReservationRepreFlag ?? false))
            {
                tempRepre = frame.ReservationPlayer2;
                beSourceAction = (source) => frame.ReservationPlayer2 = source;
            }
            else if (frame.ReservationPlayer3 != obj && (frame.ReservationPlayer3?.ReservationRepreFlag ?? false))
            {
                tempRepre = frame.ReservationPlayer3;
                beSourceAction = (source) => frame.ReservationPlayer3 = source;
            }
            else if (frame.ReservationPlayer4 != obj && (frame.ReservationPlayer4?.ReservationRepreFlag ?? false))
            {
                tempRepre = frame.ReservationPlayer4;
                beSourceAction = (source) => frame.ReservationPlayer4 = source;
            }
            // 押された場所を特定する
            switch (obj)
            {
                case ReservationPlayerView n when n == frame.ReservationPlayer1:
                    // 押された場所は今の代表者と入れ替えるのでbeRepreActionを登録する
                    beRepreAction = (repre) => frame.ReservationPlayer1 = repre;
                    break;
                case ReservationPlayerView n when n == frame.ReservationPlayer2:
                    beRepreAction = (repre) => frame.ReservationPlayer2 = repre;
                    break;
                case ReservationPlayerView n when n == frame.ReservationPlayer3:
                    beRepreAction = (repre) => frame.ReservationPlayer3 = repre;
                    break;
                case ReservationPlayerView n when n == frame.ReservationPlayer4:
                    beRepreAction = (repre) => frame.ReservationPlayer4 = repre;
                    break;
                default:
                    break;
            }
        }
        // 代表者がnullということは同じ場所をクリックしたことになるので、そのときは処理しない。
        if (tempRepre != null)
        {
            // 今回押された場所を代表者にする。
            beSourceAction(obj);
            // 代表者だった場所に押された場所の情報をいれる。
            beRepreAction(tempRepre);
        }
    }
```

``` C# : 場合によっては無駄なループが発生することに気が付いたので、平行して検索すればよくね？って思って作ったTask並列実行パターン
    /// <summary>
    /// 代表者入れ替え
    /// </summary>
    /// <param name="obj">押された場所のプレーヤー情報。ReservationPlayer[n]</param>
    private void SwapRepre(ReservationPlayerView obj)
    {
        if (obj == null)
        {
            return;
        }
        // 一時的な代表者を格納
        ReservationPlayerView tempRepre = null;
        // 「押された場所→代表者」にするアクション
        Action<ReservationPlayerView> beRepreAction = null;
        // 「代表者→押された場所」にするアクション
        Action<ReservationPlayerView> beSourceAction = null;

        // 今の代表者を特定するためのタスク
        Task findRepre = Task.Run(() =>
        {
            foreach (ReservationFrameView frame in SelectedReservation.ReservationFrameList)
            {
                // 今の代表者を特定する
                if (frame.ReservationPlayer1 != obj && (frame.ReservationPlayer1?.ReservationRepreFlag ?? false))
                {
                    tempRepre = frame.ReservationPlayer1;
                    // 今の代表者は押された場所と入れ替えるのでbeSourceActionを登録する
                    beSourceAction = (source) => frame.ReservationPlayer1 = source;
                    break;
                }
                else if (frame.ReservationPlayer2 != obj && (frame.ReservationPlayer2?.ReservationRepreFlag ?? false))
                {
                    tempRepre = frame.ReservationPlayer2;
                    beSourceAction = (source) => frame.ReservationPlayer2 = source;
                    break;
                }
                else if (frame.ReservationPlayer3 != obj && (frame.ReservationPlayer3?.ReservationRepreFlag ?? false))
                {
                    tempRepre = frame.ReservationPlayer3;
                    beSourceAction = (source) => frame.ReservationPlayer3 = source;
                    break;
                }
                else if (frame.ReservationPlayer4 != obj && (frame.ReservationPlayer4?.ReservationRepreFlag ?? false))
                {
                    tempRepre = frame.ReservationPlayer4;
                    beSourceAction = (source) => frame.ReservationPlayer4 = source;
                    break;
                }
            }
        });
        // 押された場所を特定するためのタスク
        Task findSource = Task.Run(() =>
        {
            foreach (ReservationFrameView frame in SelectedReservation.ReservationFrameList)
            {
                // 押された場所を特定する
                switch (obj)
                {
                    case ReservationPlayerView n when n == frame.ReservationPlayer1:
                        // 押された場所は今の代表者と入れ替えるのでbeRepreActionを登録する
                        beRepreAction = (repre) => frame.ReservationPlayer1 = repre;
                        break;
                    case ReservationPlayerView n when n == frame.ReservationPlayer2:
                        beRepreAction = (repre) => frame.ReservationPlayer2 = repre;
                        break;
                    case ReservationPlayerView n when n == frame.ReservationPlayer3:
                        beRepreAction = (repre) => frame.ReservationPlayer3 = repre;
                        break;
                    case ReservationPlayerView n when n == frame.ReservationPlayer4:
                        beRepreAction = (repre) => frame.ReservationPlayer4 = repre;
                        break;
                    default:
                        break;
                }
            }
        });

        // 代表者と押した場所を検索する処理を並行実行
        Task.WaitAll(findRepre, findSource);
        // 代表者がnullということは同じ場所をクリックしたことになるので、そのときは処理しない。
        if (tempRepre != null)
        {
            // 今回押された場所を代表者にする。
            beSourceAction(obj);
            // 代表者だった場所に押された場所の情報をいれる。
            beRepreAction(tempRepre);
        }
    }
```

``` C# : Taskで変数やデリゲートまで取れれば一時変数も必要なくね？と思って作った最終パターン
    /// <summary>
    /// 代表者入れ替え
    /// </summary>
    /// <param name="obj">押された場所のプレーヤー情報。ReservationPlayer[n]</param>
    private void SwapRepre(ReservationPlayerView obj)
    {
        if (obj == null)
        {
            return;
        }

        // 今の代表者を特定し、代表者の一時変数と押された場所にするためのActionを返却するタスク
        Task<(ReservationPlayerView tempRepre, Action<ReservationPlayerView> beSourceAction)> findRepre =
            Task<(ReservationPlayerView, Action<ReservationPlayerView>)>.Factory.StartNew(() =>
            {
                foreach (ReservationFrameView frame in SelectedReservation.ReservationFrameList)
                {
                    // 今の代表者を特定する
                    if (frame.ReservationPlayer1 != obj && (frame.ReservationPlayer1?.ReservationRepreFlag ?? false))
                    {
                        // 一時変数と今の代表者は押された場所と入れ替えるのでbeSourceActionを登録する
                        return (frame.ReservationPlayer1, (source) => frame.ReservationPlayer1 = source);
                    }
                    else if (frame.ReservationPlayer2 != obj && (frame.ReservationPlayer2?.ReservationRepreFlag ?? false))
                    {
                        return (frame.ReservationPlayer2, (source) => frame.ReservationPlayer2 = source);
                    }
                    else if (frame.ReservationPlayer3 != obj && (frame.ReservationPlayer3?.ReservationRepreFlag ?? false))
                    {
                        return (frame.ReservationPlayer3, (source) => frame.ReservationPlayer3 = source);
                    }
                    else if (frame.ReservationPlayer4 != obj && (frame.ReservationPlayer4?.ReservationRepreFlag ?? false))
                    {
                        return (frame.ReservationPlayer4, (source) => frame.ReservationPlayer4 = source);
                    }
                }
                return (null, null);
            });
        // 押された場所を特定し、押されたプレーヤーの一時変数と代表者にするためのActionを返却するタスク
        Task<(ReservationPlayerView tempSource, Action<ReservationPlayerView> beRepreAction)> findSource =
            Task<(ReservationPlayerView, Action<ReservationPlayerView>)>.Factory.StartNew(() =>
            {
                foreach (ReservationFrameView frame in SelectedReservation.ReservationFrameList)
                {
                    // 押された場所を特定する
                    switch (obj)
                    {
                        // switchのパターンマッチング
                        case ReservationPlayerView n when n == frame.ReservationPlayer1:
                            // 一時変数と押された場所は今の代表者と入れ替えるのでbeRepreActionを登録する
                            return (frame.ReservationPlayer1, (repre) => frame.ReservationPlayer1 = repre);
                        case ReservationPlayerView n when n == frame.ReservationPlayer2:
                            return (frame.ReservationPlayer2, (repre) => frame.ReservationPlayer2 = repre);
                        case ReservationPlayerView n when n == frame.ReservationPlayer3:
                            return (frame.ReservationPlayer3, (repre) => frame.ReservationPlayer3 = repre);
                        case ReservationPlayerView n when n == frame.ReservationPlayer4:
                            return (frame.ReservationPlayer4, (repre) => frame.ReservationPlayer4 = repre);
                        default:
                            break;
                    }
                }
                return (null, null);
            });

        // 代表者と押した場所を検索する処理を並行実行
        Task.WaitAll(findRepre, findSource);
        
        // 代表者がnullということは同じ場所をクリックしたことになるので処理しない。
        if (findRepre.Result.tempRepre != null)
        {
            // 今回押された場所を代表者にする。引数は押された場所の情報。
            findRepre.Result.beSourceAction(findSource.Result.tempSource);
            // 代表者だった場所に押された場所の情報をいれる。引数は代表者の情報。
            findSource.Result.beRepreAction(findRepre.Result.tempRepre);
        }
    }

    // // 代表者と押した場所を検索する処理を並行実行
    // Task.WaitAll(findRepre, findSource);
    // // これでも結果を取得できる。
    // var aa = Task.WhenAll(findRepre, findSource);
    // var a = aa.Result[0];
    // var b = aa.Result[1];
```

---

## switch文のパターンマッチング

[C#のアプデでめちゃくちゃ便利になったswitch文（パターンマッチング）の紹介](https://qiita.com/toRisouP/items/18b31b024b117009137a)

swapの時にサラっと使ったけど改めてまとめ。  
if文で愚直にobjがPlaer1か2か判定してたけど、型で判定できないか探してみた。  
結果的にif文より短くかけたけど、思ってたのと若干違った。  

``` C#
public string NankaMethod3(object obj)
{
    // Object型をそれぞれの型で判定し、その中身も同時に判定する
    switch (obj)
    {
        // objがint型 かつ 0より大きい
        case int x when x > 0:
            return x.ToString();
        // objがint型 かつ 0以下
        case int x when x <= 0:
            return (-x).ToString();
        // objがfloat型
        case float f:
            return ((int) f).ToString();
        // objが1文字以上のstring型
        case string s when s.Length > 0:
            return s;
        // どれにもマッチしなかった
        default:
            throw new ArgumentOutOfRangeException(nameof(obj));
    }
}
```

``` C#
private void SwapRepre(ReservationPlayerView obj)
{
    // これがswitchだと下のようになる
    // if (pushFrom.ReservationPlayer1 == obj)
    // {
    //     (repre.ReservationPlayer4, pushFrom.ReservationPlayer1) = (pushFrom.ReservationPlayer1, repre.ReservationPlayer4);
    // }
    // else if (pushFrom.ReservationPlayer2 == obj)
    // {
    //     (repre.ReservationPlayer4, pushFrom.ReservationPlayer2) = (pushFrom.ReservationPlayer2, repre.ReservationPlayer4);
    // }
    // else if (pushFrom.ReservationPlayer3 == obj)
    // {
    //     (repre.ReservationPlayer4, pushFrom.ReservationPlayer3) = (pushFrom.ReservationPlayer3, repre.ReservationPlayer4);
    // }
    // else if (pushFrom.ReservationPlayer4 == obj)
    // {
    //     (repre.ReservationPlayer4, pushFrom.ReservationPlayer4) = (pushFrom.ReservationPlayer4, repre.ReservationPlayer4);
    // }

    // 押された場所を特定する
    switch (obj)
    {
        // switchのパターンマッチング
        case ReservationPlayerView n when n == frame.ReservationPlayer1:
            // 一時変数と押された場所は今の代表者と入れ替えるのでbeRepreActionを登録する
            return (frame.ReservationPlayer1, (repre) => frame.ReservationPlayer1 = repre);
        case ReservationPlayerView n when n == frame.ReservationPlayer2:
            return (frame.ReservationPlayer2, (repre) => frame.ReservationPlayer2 = repre);
        case ReservationPlayerView n when n == frame.ReservationPlayer3:
            return (frame.ReservationPlayer3, (repre) => frame.ReservationPlayer3 = repre);
        case ReservationPlayerView n when n == frame.ReservationPlayer4:
            return (frame.ReservationPlayer4, (repre) => frame.ReservationPlayer4 = repre);
        default:
            break;
    }
}
```

---

## C#からDB接続でSQLServerに接続してSELECT文を実行する方法

[C#からDB接続でSQLServerに接続してSELECT文を実行する方法](https://rainbow-engine.com/csharp-dbconnection-sqlserver/)

---

## 演算の優先順位の確認

decimal?型にnullが入って来た場合、null合体演算子と反転処理は同居させてもエラーにならないか気になったので調査。  
`-1 × null` でエラーになりそうに見える。  
なのでかっこでくくって演算の優先順位を決めてあげたほうがよさそうに見えた。  
→`-1 × (aa ?? decimal.zero)`  

結果はかっこで括らなくても「0」と表示されたので、内部的にちゃんと評価されている模様。  

後日判明したが、そもそもnullとの演算は全てnullになるので、`-1 * null`がエラーになるという視点はちょっと残念だった。  

``` C#
    decimal? aa = null;
    // -(aa ?? decimal.zero)と括って上げたほうがよさそうに見える。
    var aac = -aa ?? decimal.Zero;
    // 「0」と表示されたので内部的に評価されている模様。
    System.Console.WriteLine(aac);

    // 後日これを実行してみたらaacはnullになったので、
    // -aa ?? decimal.Zero は -1 * (aa ?? decimal.zero) ではなく (null ?? decimal.zero)の状態になっていたことが分かった。
    var aac = -aa;

    // これがnullになるので、そもそもnullとの演算は全てnullになる模様。
    var aac = -1 * aa;
```

タプルで先頭の要素に入れた値を後の要素で使えないか気になったので調査。  
ダメだった。  
同時に値を入れるので、先頭とか関係ない模様。  

``` C#
    // Main.cs(10,20): error CS0103: The name `Disp' does not exist in the current context
    var www = (
        Disp: aa,
        Calc: -Disp
    );    
    System.Console.WriteLine(www.Calc);
```

---

## イミディエイトウィンドウでオブジェクトの中身を出力する方法

`?object.ToString()` で行ける。  

sqlのパラメータを取得したい時、カーソルを当てたときのメニューでは長すぎると「...」で切れてしまうので、全部取得したくなった。  
真っ先に浮かんだのはjsonにシリアライズすることだったが、どうSystem.~~~からやろうとしてもエラーになる。  
Usingはビルド時にそのクラスの一番上で定義されていなければ無理な模様。  
結果的にToStringだけで出力できることを発見して、事実それで解決できた。  

一応、シリアライズを頑張ったやつも乗せておく。  

[.NET 内で JSON のシリアル化と逆シリアル化 (マーシャリングとマーシャリングの解除) を行う方法](https://docs.microsoft.com/ja-jp/dotnet/standard/serialization/system-text-json-how-to?pivots=dotnet-6-0)  
→  
`System.Text.Json.JsonSerializer.Serialize(param)`  

[C# Object引数の中身をtextに羅列したい](https://oshiete.goo.ne.jp/qa/5988834.html)  
→  
>nullでないことを条件として，ToStringメソッドを呼び出せば，何らかの文字列が得られるはずです。  
>他にも，GetTypeメソッドを呼べば元の型が (ほぼ) 得られますし，リフレクションを使えば元の型を知らなくてもフィールド等の値を得られますから，頑張ればそちらでも情報を得られると思います。  

---

## C#リフレクションTIPS 55連発

[C#リフレクションTIPS 55連発](https://qiita.com/gushwell/items/91436bd1871586f6e663)  

### 47.プロパティの値を取得する

``` C#
// 1行で完結させたやつ
object value = obj.GetType().GetProperty("MyProperty").GetValue(obj);

// ばらしたやつ
Type type = obj.GetType();
PropertyInfo prop = type.GetProperty("MyProperty");
object value = prop.GetValue(obj);
Console.WriteLine(value);   
```

---

## EnumのGetValueOrDefault

EnumのGetValueOrDefaultは何が帰ってくるのか気になった。  
Byteなので普通に0だが、面白いのは定義したEnumが1から始まるものでもデフォルトは0とされること。  

``` C#
    CaddyType? caddyType = null;
    // Enumが1から始まっていても0となる。
    var aa  = (int)caddyType.GetValueOrDefault();

    enum CaddyType : byte
    {
        None = 1,
        Use =2,
        UseTwo = 3,
    }
```

---

## Listにnullのオブジェクトをいれる

``` C#
    Person person = null;
    // a1.Count = 1
    // 1番目の要素はnull
    var a1 = new List<Person>() { person };

    person = new Person();
    // a2.Count = 1
    // 1番目の要素はPersonインスタンス
    var a2 = new List<Person>() { person };

    // a1がnullだとエラーになるのでnew Listにnullをいれるのは罠
    foreach(var aa in a1)
    {

    }
```
