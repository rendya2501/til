# C#基本まとめ

## .NetのバージョンとC#のバージョン早見表

[【C# 機能別】知らん書き方が出てきたらこれを見ろ！C# 10 までの進化を1ページで](https://oita.oika.me/2021/12/23/csharp-10-history)  
[C# の歴史](https://docs.microsoft.com/ja-jp/dotnet/csharp/whats-new/csharp-version-history)  
[.NET Framework のバージョン対応表](https://qiita.com/nskydiving/items/3af8bab5a0a63ccb9893)  

``` txt : 早見表
C#   | year  | .NET      | .NET | .NET | Visual Studio
     |       | Framework | Core |      |
-----+-------+-----------+------+------+----------------
1.0  | 2002  |  1.0      |      |      | 2002
1.2  | 2003  |  1.1      |      |      | 2003
2.0  | 2005  |  2.0      |      |      | 2005
3.0  | 2007  |  3.5      |      |      | 2008
4.0  | 2010  |  4.0      |      |      | 2010
5.0  | 2012  |  4.5      |      |      | 2012
6.0  | 2015  |  4.6      | 1.0  |      | 2015
7.0  | 2017  |  4.7      |      |      | 2017
7.1  | 2017  |  2.0      |      |      | 2017
7.2  | 2017  |  2.1      |      |      | 2017
7.3  | 2018  |  4.8      | 2.2  |      | 2017
8.0  | 2019  |           | 3.0  |      | 2019
9.0  | 2020  |           |      | 5    | 2019
10.0 | 2021  |           |      | 6    | 2022
11.0 | 2022  |           |      |      | 2022
```

---

## 演算子の名称

``` txt
null合体演算子 : ??
null条件演算子 : ?.
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

## イベント

[C#のイベント機能](https://dobon.net/vb/dotnet/vb2cs/event.html)  

イベントは要するにデリゲートである。  
そのイベントが発火したときに何をやらせたいのか、その処理を登録するだけ。  

``` C# : 基本的なイベント
Console.WriteLine("ボタンを押した体");
SleepClass clsSleep = new();
// ① イベントハンドラの追加
// イベントが発生した時に実行したい処理を登録する。
clsSleep.Time += new EventHandler(SleepClass_Time);
// ② 処理開始
clsSleep.Start();

/// <summary>
/// イベントが発生したときに呼び出されるメソッド
/// </summary>
void SleepClass_Time(object sender, EventArgs e)
{
    // ⑥ 登録された処理として、画面にハローワールドが表示される。
    Console.WriteLine("Hello, World!");
}

public class SleepClass
{
    // データを持たないイベントデリゲートの宣言
    // ここでは"Time"というイベントデリゲートを宣言する
    public event EventHandler? Time;

    /// <summary>
    /// イベントを発火させるメソッド
    /// </summary>
    /// <param name="e"></param>
    protected virtual void OnTime(EventArgs e)
    {
        // ⑤ Timeに紐づけられた処理が実行される。
        // 今回の例の場合SleepClass_Timeが発動する。
        Time?.Invoke(this, e);
    }

    /// <summary>
    /// 処理を開始する。
    /// 1秒後にイベント発火
    /// </summary>
    public void Start()
    {
        // ③ 1秒待つ
        Thread.Sleep(1000);
        // ④ イベントを発火させるメソッド
        OnTime(EventArgs.Empty);
        // 別にここにこう書いてもいい
        //  Time?.Invoke(this, e);
    }
}
```

``` C# : 値が帰ってくるサンプル
Console.WriteLine("値が帰ってくるサンプル");
SleepClass2 SleepClass2 = new();
SleepClass2.Time += new SleepClass2.TimeEventHandler(SleepClass_Time2);
SleepClass2.Start();

/// <summary>
/// 値が帰ってくるイベントのサンプルクラス
/// </summary>
public class SleepClass2
{
    //デリゲートの宣言
    //TimeEventArgs型のオブジェクトを返すようにする
    public delegate void TimeEventHandler(object sender, TimeEventArgs e);

    //イベントデリゲートの宣言
    public event TimeEventHandler? Time;

    protected virtual void OnTime(TimeEventArgs e) => Time?.Invoke(this, e);

    public void Start()
    {
        Thread.Sleep(1000);
        //返すデータの設定
        TimeEventArgs e = new();
        e.Message = "終わったよ。";
        //イベントの発生
        OnTime(e);
    }
}

/// <summary>
/// Timeイベントで返されるデータ
/// ここではstring型のひとつのデータのみ返すものとする
/// </summary>
/// <remarks>
/// EventArgsの派生クラスを用いてデータを返していたが、必ずしもそうする必要はない。
/// しかし、EventArgsを使った方法が.NETでは推奨されているので余程のことがない限りは従うべき。
/// </remarks>
public class TimeEventArgs : EventArgs{ public string Message; }
```

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

東さんに三項演算子でnull許可のboolを受け取るとき、「片方を変換しないといけないのキモイね」って言われたので、「受け取り側をvarじゃなくてbool?にすれば行けますよ」って言ったけどエラーになった。  
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

## 構造体

- 構造体は値型、クラスは参照型  
- 構造体はスタック領域に展開され、クラスはヒープ領域に展開される。  
- int等と同じなので、メソッドを抜けたらメモリから解放される。  
- もちろんクラスより軽量。newの処理も早いし、メモリも食わない。  
- 構造体の初期状態は0初期化状態という。→構造体の既定値(default value)と呼ぶ。クラスの初期状態はnull。  
- 値型なので、newする必要はない。でもnewできる。  
  - int型をnewしているのと同じ意味になる。  
  - newはコンストラクタを呼ぶ。  
  - newしない場合、コンストラクタを呼ばない。  

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

---

## ジェネリック

[【c#】ジェネリック制約まとめ](https://qiita.com/daria_sieben/items/aa28a014656c9a0990ed)  

### ジェネリック制約とは

ジェネリックを使う際に宣言の後ろに

`[where ジェネリックで使う型 : 制限したい型]`

をつけることで使える型を制限出来ることです。  
こうすることによってこのクラスでしか使えないようにしたいとか、このクラスでは使ってほしくないなどの使い方が出来るようになります。  

``` C# : ジェネリックの対象をクラスのみにしたい場合
public class MyClass<T> where T : class
{
}
```

### 一覧

``` txt
ジェネリック制約                説明
where T : class                 class(参照型)のみ制約
where T : struct                Nullableを除く全ての値型のみ制約
where T : <クラス名>            指定したクラスのみで制約
where T : <インターフェース名>  指定したインターフェースのみで制約
where T : new()                 引数なしのパブリックコンストラクタがある型のみで制約
where T : U                     Uに基づいた型で制約される
```

---

## Equalsメソッドと ==演算子 の違い

[==演算子とEqualsメソッドの違いとは？［C#］](https://atmarkit.itmedia.co.jp/ait/articles/1802/28/news028.html)  
[2つの値が等しいか調べる、等値演算子(==)とEqualsメソッドの違い](https://dobon.net/vb/dotnet/beginner/equality.html)  
[【C#】文字列を比較する（== 演算子、Equalメソッド、Compareメソッド）](https://nyanblog2222.com/programming/c-sharp/193/)  

Javaの時にもまとめたかもしれないが、改めて==とEqualsの違いをまとめる。  

事の発端は萬君がEqualsで文字列の比較をしていて、左辺の文字列がnullで.Equalsメソッドで比較しようとしてエラーになるのを発見したためだ。  
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

//参照型の等価を調べる
//o1とo2は別のインスタンス
object o1 = new object();
object o2 = new object();
Console.WriteLine(o1 == o2); //false
Console.WriteLine(o1.Equals(o2)); //false

//o1とo2は同じインスタンス
o2 = o1;
Console.WriteLine(o1 == o2); //true
Console.WriteLine(o1.Equals(o2)); //true

//String型の等価を調べる
//s1とs2は同じ値だが、別のインスタンス
string s1 = new string('a', 10);
string s2 = new string('a', 10);
Console.WriteLine(s1 == s2); //true
Console.WriteLine(s1.Equals(s2)); //true
Console.WriteLine(object.ReferenceEquals(s1, s2)); //false
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

## out・ref・in

[C# out と ref](https://qiita.com/muro/items/f88b17b5fea3b4537ba7)  

どちらも参照渡しのためのパラメーター修飾子です。  

### out

out修飾子はreturn以外でメソッド内からメソッド外へデータを受け渡す場合で使用されます。  
よく使われるものとしてはTryParseメソッドがあります。  

``` C#
    // 呼び出し側
    int val;

    // trueが返りつつ、valは10となる。
    if (OutHoge(out val))
    {
        // C# 7.0から out 引数の利用時に同時に変数宣言できるようになり、あらかじめ変数を宣言しておく必要がなくなりました。
        // if (OutHoge(out int val)) こうできる
        Console.WriteLine(val);
    }

    // 呼び出し先
    bool OutHoge(out int x)
    {
        x = 10;
        return true;
    }
```

### ref

ref修飾子はメソッド外からメソッド内へデータを渡し、変更を外部へ反映させる必要がある場合に使用します。  

``` C#
    int x = 0;
    // RefPiyoを抜けた後、xは10になっている。
    RefPiyo(ref x);

    void RefPiyo(ref int x)
    {
        x = 10;
    }
```

#### 参照型の参照渡し

``` C#
    // Age = 20 で初期化
    var p = new Person("John", 20);
    // Ageに10足す
    Hoge(p);
    // 反映される
    Console.WriteLine($"Name={p.Name}, Age={p.Age}"); // Name=John, Age=30

    void Hoge(Person p) => p.Age += 10;
```

``` C#
    // Age = 20 で初期化
    var p = new Person("John", 20);
    // Hogeの中でpに新しいインスタンスを代入し、Ageに10足す
    Hoge(p);
    // 反映されない
    Console.WriteLine($"Name={p.Name}, Age={p.Age}"); // Name=John, Age=20

    void Hoge(Person p)
    {
        p = new Person("Mike", 33);
        p.Age += 10;
    }
```

``` C#
    // Age = 20 で初期化
    var p = new Person("John", 20);
    // p に新しいインスタンスを代入し、Ageに10足す
    Hoge(ref p);
    // 反映される
    Console.WriteLine($"Name={p.Name}, Age={p.Age}"); // Name=Mike, Age=43

    void Hoge(ref Person p)
    {
        p = new Person("Mike", 33);
        p.Age += 10;
    }
```

### in

一言で言うならば、読み取り専用の参照渡し。  

引数に渡した値がメソッド内で変更されないためには、値渡しを利用していました。  
しかし値渡しは変数をコピーするため、巨大な配列などはコピーに時間がかかるデメリットもありました。  
このinキーワードは、処理の早い参照渡しでありながら読み取り専用であるため、パフォーマンス向上が期待できます。  

C#7.2から利用可能。  

``` C#
    int x = 1;
    // ref 引数と違って修飾不要
    F(x);
    // 明示的に in と付けてもいい
    F(in x);
    // リテラルに対しても呼べる
    F(10);
    // 右辺値(式の計算結果)に対しても呼べる
    int y = 2;
    F(x + y);

    void F(in int x)
    {
        // 読み取り可能
        Console.WriteLine(x);
        // 書き換えようとするとコンパイル エラー
        x = 2;
    }

    // 補足: in 引数はオプションにもできる
    void G(in int x = 1) { }
```
