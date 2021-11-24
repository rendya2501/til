# C#基本まとめ

## C#のバージョン早見表

[.NET Framework のバージョン対応表](https://qiita.com/nskydiving/items/3af8bab5a0a63ccb9893)  
公式を見るよりよっぽどいい。  

- .NetFramework 4.8 : C# 7.3  
- .NetCore3.0 : C# 8.0
- .NetCore5 : C#9.0

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

## インデクサ(indexer)

大昔にまとめたんですけど、結局どういう機能かわからないままなんですよね。  
というわけで、そういう基本的なところからまとめていこうと思います。  
まぁ、ほとんどwikiの内容なんですけどね。  

### とりあえず言葉の意味から

indexer  
①索引作成者  
②データベースの索引を作るプログラムや人の事。  

indexって付くくらいなので、まぁ、そうなるよねって感じ。  
容易に添え字が使われるのだろう事は予想できる。  

### とりあえず定義から

``` txt : wiki
クラスや構造体のインスタンスに配列を同様に沿え字を指定してアクセスするための構文
```

配列って添え字でアクセスするじゃん？  
インスタンスも同じように添え字でアクセスできるようにすると便利なんじゃね？って機能  
まぁ、やればわかるけど、本当にクラスも配列見たいに添え字でアクセスできるんだ  

### 実装例

インデクサーをサポートしていないJavaにおいて、配列リストを表すコレクションの要素へのアクセスは、Listインターフェースのget/setメソッドによって提供される。  

``` Java
var list = new java.util.ArrayList<Integer>(java.util.Collections.nCopies(10, 0));
// index 番目の要素に値を設定。
// void set(int index, E element)
list.set(2, 100);
// index 番目の要素を取得。
// E get(int index)
int val = list.get(2);
```

C#のインデクサでは、配列リストの要素へのアクセスを配列のアクセスと同じように記述することができる。  

``` C#
var list = new System.Collections.Generic.List<int>(new int[10]);
list[2] = 100;
int val = list[2];
```

あれ？もしかしてindexofとかでアクセスしてたけどそんなことする必要なかったのか？  
いや、indexofは用途が違う。  
stringで値を検索して、その場所をインデックスで返してくれる関数だから違う。  

インデクサを定義する際、インデックスとして整数以外の値 (文字列やオブジェクトなど) も使用することができ、ハッシュテーブルなどの連想配列を表すコレクションに使用されている。  

``` C#
var map = new System.Collections.Generic.Dictionary<string, double>();
map["key1"] = 0.1;
double val = map["key1"];
```

一番恩恵がディクショナリーかもしれない。  
キーでバリューにアクセスできるのはインデクサのおかげだったんだな。  
割と感動した。  

因みにインデクサを提供するインターフェースはIListらしい。→
[列挙可能から完全なるモノまで – IEnumerableの探索 – C# Advent Calendar 2014](https://www.kekyo.net/2014/12/14/4587)  

``` txt
IListインターフェイスには「インデクサ」が定義されています。このインデクサを使用可能にするため、敢えてIListインターフェイスを実装しているのではないかと推測しています。
```

萬君がそういってた。よくそんなこと知ってるね。  
まぁ、調べたらそういう文献がちらほら見つかるので、本当のことなのだろう。  
DictionaryはIDctionaryが提供していたので、厳密にIListで無ければいけないというわけではないみたいだ。  

### 他参考にしたサイト等  

[Java使いがC#を勉強する　その④　インデクサ](https://shironeko.hateblo.jp/entry/2017/02/09/202843)

### 結論？

これがなかったらDictionaryは使い物にならなさそうなことわかった。  
Listでも、配列のようにアクセスできるのはインデクサーのおかげということがわかった。  
実装する機会があるかどうかは知らないけど、作る時は調べながら作るでしょう。  
本当に、配列のようにアクセスできる仕組みを提供する機能なんだなーってことがわかりました。  

---

## キャスト

「is」,「as」「前かっこ」による変換の違い。  

・前かっこは変換に失敗するとエラー(System.InvalidCastException)になる。  
・as は変換に失敗するとnullになる。エラーにはならない。  
・is はif文で使うので、失敗したらelseに流れるだけ。  

## TryParseとConvertの違い

[C#のint.ParseとConvert.ToInt32の違いをマスター](https://www.wareko.jp/blog/master-difference-between-csharp-int-parse-and-convert-toint32)  

・空文字列 → int.Parse(“”), Convert.ToInt32(“”) は共に例外になる  
・null → int.Parse(null) は例外になる。Convert.ToInt32(null) はゼロになる。  

``` C#
// TryParseを1文で済ませたかったらこう書く。因みにTryParseには文字列しか渡せない。なのでうまくできたが、使えなかった。
FrontIsPrintCopy = bool.TryParse(_BasicInfo.CopyPrintType.GetValueOrDefault(), out bool frontIsPrintCopy) && frontIsPrintCopy;
// _BasicInfo.CopyPrintTypeがnullの場合はGetValueOrDefaultで0になってToBooleanでfalseになる。
FrontIsPrintCopy = System.Convert.ToBoolean(_BasicInfo.CopyPrintType.GetValueOrDefault());
```

---

## C#のアクセス修飾子

<https://www.fenet.jp/dotnet/column/language/6153/>

`internal`がわからなかったのでついでに調べた。  

- public    : あらゆる所からアクセスできる  
- private   : 同じクラス内のみアクセスできる  
- protected : 同じクラスと派生したクラスのみアクセスできる  
- internal  : 同じアセンブリならアクセスできる。つまり同一プロジェクト内部。  
- protected internal : 同じアセンブリと別アセンブリの派生クラスでアクセスできる  
- private protected  : 同じクラスと同じアセンブリの派生したクラスのみアクセスできる  

<https://www.fenet.jp/dotnet/column/language/4831/#internal>  

同一アセンブリ内(同一exe/同一dll)のクラスからのみアクセス可能な修飾子です。  
他のプロジェクトからは、参照設定がされていてもinternalの場合はアクセス不可となります。  
publicと違い修正範囲が限定されているため、同じアセンブリ内でのみ使用するならば、こちらを使う方が良いでしょう。  

---

## abstractとinterfaceの棲み分け

interfaceは外部向け。abstractは内部向け。  
interfaceはpublicしか記述できない。  
abstractはprotectedが使える。  
なので、外部に公開する場合はinterface。  
内部において、実装を強制する場合はabstractを用いるいいかも。  

---

## アノテーション

Annotation:注釈  
<https://elf-mission.net/programming/wpf/episode09/>  
恐らくではあるが、属性やバリデーションの為にクラスやフィールドの宣言の上に[]で囲うやつの事全般をこう読んでいるのではないか?  
調べてもそういうのしか出てこなかった。  
後は彼らが何を指してそう読んでいるのか、一括にしているのかなど、聞いてみないとわからない。  

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

1.フィールド  
・オブジェクト指向について「クラスは、データと振る舞いをカプセル化したものである」と説明されるときの「データ」の部分です。  
・オブジェクトが持つデータをフィールドとして定義します。  
・フィールドはクラスのメンバ（クラスなど、型を構成する内部要素の総称）として宣言された変数で、インスタンスと直接結び付けられます。  
・メンバ変数とも呼ばれます。  
・フィールドはインスタンスからアクセスします。  
・**非公開にし、プロパティで操作するのが原則です。**  
・フィールドと3.プロパティの混乱を避けるためにアンダースコア（_）をつけて定義することがあります。  
→  
答えがあったぞ。特に理由が無ければプロパティ経由でアクセスしておけって話か。  

3.プロパティ  
・オブジェクト内にあるフィールドの値を取得、または設定するための手段です。  
・クラス外部から見るとメンバー変数のように振る舞い、 クラス内部から見るとメソッドのように振舞います。  
・メンバー変数の値の取得・変更を行うためのメソッドのことをアクセサー(accessor)といいます。  
・setterに渡す値にはsetter内からvalueという変数でアクセスできます。  

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

## protected、internal、protected internal と private protected

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

ちなみに、protected internal と private protected では、語順は自由です。 protected internalとinternal protected、private protectedとprotected privateはそれぞれ同じ意味になります。

```C#
// どちらの順序でも同じ意味
protected internal int A1;
internal protected int A2;

private protected int B1;
protected private int B2;
```

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

## シャローコピーとディープコピー

[シャローコピーとディープコピーの違い](https://itsakura.com/it-program-shallow)  
[C# DeepCopyする方法](https://tomisenblog.com/c-sharp-deepcopy/)  

シンボリックリンクとハードリンクみたいなやつ。  
シャローコピーは明らかにシンボリックリンクだな。  
いや、多分正確には違うと思うけど。  

### シャローコピー

・オブジェクトの参照先をコピーします。  
・コピー元のオブジェクトとコピー先のオブジェクトが同じアドレスの値(インスタンス)を参照します。  
・片方のオブジェクトの値を変更すると、もう一方のオブジェクトの値も変更されます。  
・英語ではshallow copyです。shallowは、浅いという意味です。  

変数だけ別の用意して、参照先は同じやつ。  

``` C# : シャローコピー
    var srcMember = new Member { Name = "鈴木", Address = "東京都" };
    var dstMember = srcMember;
    dstMember.Address = "千葉県";
    // src側も変わってる
```

### ディープコピー

・オブジェクトの値(インスタンス)をコピーします。  
・コピー元のオブジェクトとコピー先のオブジェクトがそれぞれ別のアドレスの値(インスタンス)を参照します。  
・片方のオブジェクトの値を変更しても、もう一方のオブジェクトの値は変更されません。  
・英語ではdeep copyです。deepは、深いという意味です。  

変数も別で参照先も別のやつ。  
でも、どっちも入ってる値は同じ。  
C#ではデフォルトでやってくれるやつはないので自分で実装する必要がある。  
といっても、Newして片方のデータをそのまま新しいほうにグルグルセットするだけだけどね。  
シリアライズとデシリアライズでやる方法もあるか。  

ソースはシャローコピーのdst側を千葉県ってやってもsrcの値はそのままになるだけだから省略するよ。  

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

## イベントのOverride

・Overrideしたイベントは 「-=」や「+=」で登録、解除はできない。  
なので発動させたくなかったらフラグ使って、returnしたりして実行されないようにする必要がある。  

[C# eventのオーバーライドと基底クラスで発生するイベントの処理](https://opcdiary.net/c-event%E3%81%AE%E3%82%AA%E3%83%BC%E3%83%90%E3%83%BC%E3%83%A9%E3%82%A4%E3%83%89%E3%81%A8%E5%9F%BA%E5%BA%95%E3%82%AF%E3%83%A9%E3%82%B9%E3%81%A7%E7%99%BA%E7%94%9F%E3%81%99%E3%82%8B%E3%82%A4%E3%83%99/)  
もしかしてこの方法の通りにやればそんなことする必要ないかも？  
余裕があれば次の機会に照らし合わせて見てみたい。  

---

## アップキャスト/ダウンキャスト

[C#：アップキャストとダウンキャストについて、サンプルソース付きで解説してみる](https://www.kakistamp.com/entry/2018/04/29/010258)  

**子から親へのキャスト→アップキャスト**  
**親から子へのキャスト→ダウンキャスト**  

・アップキャストは、常に安全に行える  
・ダウンキャストは、エラーが発生する事がある。  
→
・キャストは型を変更するだけで、中身が消える訳ではない。（アクセスできる範囲が変わるだけで、実態は存在している）  
・インスタンス作成時、メモリに領域が割り当てられる。  
・子クラスのインスタンスは、親クラスのインスタンスより多くの領域を必用とする。  
・「親→子」とキャストした場合、子クラスが必要とする領域に、メモリが割り当てられていない状態となる。（そのため、エラーとして弾いている。）  

``` C#
//親クラス
class ParentClass { }
//子クラス
class ChildClass : ParentClass { }

private void Test()
{
    //===========================
    //      アップキャスト
    //===========================
    //子クラスのインスタンスを生成
    ChildClass child01 = new ChildClass();
    //子クラスのインスタンスを、親クラスに代入可能。
    ParentClass parent01 = child01;

    //===========================
    //  ダウンキャスト（エラー）
    //===========================
    ChildClass child02;
    //親クラスのインスタンスを生成
    ParentClass parent02 = new ParentClass();
    //キャストできるかどうか確認（falseになります）
    if (parent02 is ChildClass)
    {
        //キャスト時にエラーが発生する
        child02 = (ChildClass)parent02;
    }
    //asを使えば キャストできない場合 nullが入る
    child02 = parent02 as ChildClass;

    //===========================
    //     ダウンキャスト
    //===========================
    //親クラスのインスタンスを、子クラスで生成
    ParentClass parent03 = new ChildClass();
    //ダウンキャスト可。
    ChildClass child03 = (ChildClass)parent03;
}
```

ダウンキャストがうまくいかない理由はここが詳しく解説している。  
[なぜアップキャストは安全で、ダウンキャストは危険なのか](https://qiita.com/RYO-4947123/items/eaeb48b6fcf97c02710f)  

アップキャストとは？  
基底（親）クラス型の変数に派生（子）クラスのインスタンスを代入する際に行われる型変換です。  

ダウンキャストとは？  
派生（子）クラス型の変数に基底（親）クラスのインスタンスを代入する際に行われる型変換です。  

子クラスを親クラスにアップキャストして問題ないのは、親クラスで定義した領域を確保している状態での代入になるから。  
親クラスを子クラスにダウンキャストして問題なのは、子クラスが実装している分の領域を確保しない状態で代入する事になるから。  
こんなところだよな。  
だから、親クラスを子クラスにダウンキャストしたかったら、親クラスの変数に子クラスのインスタンスを代入して、それを子クラスにキャストするって操作が必要になる。  
そうすれば、子クラスで必要な領域を確保しつつ親クラスで実装した機能のみが使える制限された状態になり、そのまま子クラスに代入しても、  
領域は確保しているから問題ないし、子クラスにキャストする事で制限された機能を解放する事になるから。  

ここも面白い。  
[【Java】アップキャストとダウンキャスト](https://se-tomo.com/2019/09/16/%E3%80%90java%E3%80%91%E3%82%A2%E3%83%83%E3%83%97%E3%82%AD%E3%83%A3%E3%82%B9%E3%83%88%E3%81%A8%E3%83%80%E3%82%A6%E3%83%B3%E3%82%AD%E3%83%A3%E3%82%B9%E3%83%88/)  

アップキャストをすることによって、そのインスタンスでできることが制限されます。
子クラスは
・親クラスの機能
・子クラス独自の機能
の２つを持っていますが、親クラスにアップキャストしたことにより

・親クラスの機能
・（使用不可能）子クラスの機能
では、アップキャストにより親クラスで定義された機能しか利用できなくなる例を見てみましょう。  

``` Java : アップキャスト
package try_catch_01;

class A {}
class B extends A{
    public void method() {
        System.out.println("methodB");
    }
}

public class Main {
    public static void main(String[] args) {
        B b = new B();
        b.method();
        
        // アップキャスト
        A b2a = (A) b;
        b2a.method();//未定義エラー
    }
}
```

つまり、
・アップキャスト後の実態はあくまでサブクラス
・アップキャストにより、アップキャストで定義されたものしか利用できなくなる

``` Java : ダウンキャスト
class A {}
class B extends A{
    public void method() {
        System.out.println("methodB");
    }
}

public class Main {
    public static void main(String[] args) {
        // *****ダメな例******//
        // Aのインスタンス
        A a = new A();
        // Aがもつ機能を無理やり拡張はできない
        B a2b = (B)a;

        // *****OKな例******//
        // BのインスタンスをAの機能に制限した状態
        A a = new B();
        // 制限を解除した。無理やり拡張している訳でない。
        B a2b = (B)a;
    }
}
```

上記のように、

・実態がBクラスであるが、型はAクラス
・つまり、変数aは型Bを型Aに暗黙的にアップキャストした状態
・それは、実態はBクラスだが、Aの範囲に使用制限をかけた状態
・変数aをBクラスに戻し（ダウンキャスト）、アップキャストによる使用制限を解除

---

## typeof

[[C# クラス] キャストで型変換（基底クラス⇔派生クラス）](https://yaspage.com/prog/csharp/cs-class-cast/)  
アップキャストダウンキャストの例としてはわかりにくいけど、typeofの使い方は参考になったので、それはそれでまとめる。  

`型を厳密にチェックしたい場合は、typeof を使います。`  
ほーんって感じ。  
TypeofってRN3だとnameofと合わせて使ってた気がする。  
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

## 配列

今更感ある。  
これを書いた時は基本情報の試験間近なので、Javaの書き方と比較してついでにという事でまとめる事にした。  
とりあえずこことここを見ておけば十分です。  

[未確認飛行C 配列](https://ufcpp.net/study/csharp/st_array.html)  
[配列の宣言は種類が多いな～JavaとC#の両方で出来るのどれ？](http://juujisya.jugem.jp/?eid=7)  

種類としては、通常の配列と多次元配列とジャグ配列の3種類がある。  
宣言の仕方も色々合ってややこしい。  
とりあえず、宣言だけまとめる。  

``` C#
// 基本
型名[] 変数名 = new 型名[] {値1, 値2, .....};
int[] a = new int[] {1, 3, 5, 7, 9};
int[] b = new int[] { 1, 3, 5, 7, 9, }; //最後にカンマがあっても問題ない
int[] a = {1, 3, 5, 7, 9};
var a = new[] {1, 3, 5, 7, 9};

int[] ary1; // JavaでもC#でもOK
int[] ary1 = new int[]{1, 2}; // JavaでもC#でもOK
int[] ary2 = new int[2]{1, 2}; // JavaはNGだけど、C#はOK
int[] ary2 = new int[3]{1, 2}; // これはエラー


// 多次元配列
変数名 = new 型名[長さ1, 長さ2]; // 2次元配列の場合
変数名 = new 型名[長さ1, 長さ2, 長さ3]; // 3次元配列の場合
double[,] a = new double[,]{{1, 2}, {2, 1}, {0, 1}}; // 3行2列の行列
double[,] b = new double[,]{{1, 2, 0}, {0, 1, 2}};   // 2行3列の行列
double[,] c = new double[3, 3];                      // 3行3列の行列

int[,] ary = new int[4, 5];
int[,] ary = { { 1, 2 }, { 3, 4 }, { 5, 6 } };


// 配列の配列
double[][] a = new double[][]{  // 3行2列の行列
  new double[]{1, 2},
  new double[]{2, 1},
  new double[]{0, 1}
};
double[][] b = new double[][]{  // 2行3列の行列
  new double[]{1, 2, 0},
  new double[]{0, 1, 2}
};
double[][] c = new double[3][]; // 3行3列の行列


// ジャグ配列→いわゆる凸凹の配列
int[][] array = new int[2][];
array[0] = new int[3];
array[1] = new int[2];
//C#ではエラーになる。[ ][ ]を使うときは、2次元目をnewを使って後入れしなきゃだって
int[ ][ ]  array ={ {1,2,3},{1,2} };
```

---

## 条件演算子のターゲット型推論の強化

東さんに三項演算子でnull許可のboolを受け取るとき、「片方を変換しないといけないのキモイね」って言われたので、  
「受け取り側をvarじゃなくてbool?にすれば行けますよ」って言ったけどエラーになった。  
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

## C# 構造体

値型なので、newする必要はない。でもnewもできる。  
newはコンストラクタを呼ぶ。  
newしない場合コンストラクタを呼ばない。  
そういう棲み分けができる。  

言語仕様に近いので中々濃い内容になっている。  
とりあえず、ざっくりとした特徴と宣言の仕方くらいをまとめられれば十分かな。  
実務ではまず使わないからガッツリやったところでねぇって感じはする。  

- 構造体は値型、クラスは参照型  
- 構造体はスタック領域に展開され、クラスはヒープ領域に展開される。  
- int等と同じなので、メソッドを抜けたらメモリから解放される。  
- もちろんクラスより軽量。newの処理も早いし、メモリも食わない。  
- 構造体の初期状態は0初期化状態という。→構造体の既定値(default value)と呼ぶ。クラスの初期状態はnull。  
- newできる。でもint等と同じなので、newしなくても使える。  

``` C#
    public struct Circle
    {
        public double r;
        public double CalcCircum(double r) => 3.14 * 2 * r;
        public double CalcArea(double r) => 3.14 * r * r;
    }
    
    // 方法1.new演算子を使う方法
    Circle c1 = new Circle();
    c1.r = 10.0;
    Console.WriteLine("半径{0}の円周は{1}、面積は{2}", c1.r, c1.CalcCircum(c1.r), c1.CalcArea(c1.r));
    
    // 方法2.new演算子を使わない方法
    Circle c2;
    c2.r = 20.0;
    Console.WriteLine("半径{0}の円周は{1}、面積は{2}", c2.r, c2.CalcCircum(c2.r), c2.CalcArea(c2.r));
    
    // 方法3.インスタンス化と同時に初期化
    Circle c3 = new Circle() {r = 30.0};
    Console.WriteLine("半径{0}の円周は{1}、面積は{2}", c3.r, c3.CalcCircum(c3.r), c3.CalcArea(c3.r));

    // 方法4.default演算子を使った方法 C# 2.0～9.0 まで、p1と同じ意味っぽい
    var p4 = default(Circle);
```
