# C#メモ

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

## ObservableCollection

[【WPF】ItemsSourceにObservableCollectionを選ぶのはなぜ](https://itwebmemory.com/itemssource-observablecollection-explanation)  
なんとなく使ってはいるが、なぜ使うのか、どういうものなのかずっとわからなかったのでまとめることにした。  

``` txt
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

## コールバックサンプル

``` C#
// Form側
private void EditFormModel_Inquire(string message, Action yesAction, Action noAction)
{
   var dialogResult = MessageBox.Show(
       message,
       "確認",
       MessageBoxButtons.YesNo, MessageBoxIcon.Question
    );
   switch (dialogResult)
   {
       case DialogResult.Yes:
           yesAction();
           break;
       case DialogResult.No:
           noAction();
           break;
       case DialogResult.None:
       case DialogResult.Abort:
       case DialogResult.Retry:
       case DialogResult.Ignore:
       case DialogResult.OK:
       case DialogResult.Cancel:
       default:
           break;
   }
}

// MODEL側
this.OnInquire(
   "保存処理を実行しますか？",
   () =>
   {
       try
       {
           var result = new ServiceClient.EditServiceClient().Save(this._Data);
       }
       catch (Exception ee)
       {
           MessageBox.Show(ee.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
           this.OnStatusMessage("保存に失敗しました。");
           return;
       }
       //オプションで編集が有効の場合、編集モードにする
       var tmpOptionMode = true;
       if (tmpOptionMode)
       {
           //編集モード
           this.SerachBankBranch();
       }
       else
       {
           //初期化モード
           this.Initialize();
       }
       this.OnStatusMessage("データを保存しました。");
   },
   () => {}
);
```

---

## アトリビュートに設定されている文字列の長さを取得する方法

``` C#
/// <summary>
/// ProductCDの桁数を取得
/// </summary>
private static readonly int ProductCDLength = (Attribute.GetCustomAttribute(typeof(TMa_Product).GetProperty(nameof(TMa_Product.ProductCD)), typeof(StringLengthAttribute)) as StringLengthAttribute)?.MaximumLength ?? 20;
```
