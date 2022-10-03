# C#メモ

---

## シリアライズとデシリアライズを繰り返すと？

実務において、初期化を繰り返すとメモリーがどんどん増えていくことに気が付いた。  
そこでは、まっさらなデータをDeepCopy(シリアライズとデシリアライズ)して代入する処理をしていたのだが、もしかしてこの場合、ガベージコレクションされないのかなと思ったらされない模様。  
おとなしくループさせて、必要な項目だけを初期化したらメモリーが増えることはなくなった。  

>オブジェクトをシリアライズすると、ガベージコレクタに回収されず、生存期間が延長されることがある。  
ガベージコレクタはよそから参照されているオブジェクトインスタンスを回収することはできないため、参照の一覧表が存在するかぎり、シリアライズしたオブジェクトはガベージコレクタに回収されない。  
<https://www.jpcert.or.jp/java-rules/ser10-j.html>  

シリアライズとは、メモリ上のデータをバイトに変換すること。  
メモリ上のそのインスタンスが保持するデータ全てがバイト列に置き変わる。  
バイト列なので、ポインタで指し示すデータではなく、実データとなる。  
それを元に戻すのがデシリアライズ。  
デシリアライズすると、バイト列のデータが元に戻るので、全く同じデータを持つインスタンスを作ることができる。  
もちろんこの時、メモリーのアドレスやポインタ等は新しくなっている。  
しかし、これを延々と繰り返すと、同じデータが無限に増やせるので、メモリリークの原因になるらしい。  

[Java のシリアライズ (serializer, 直列化) について](http://funini.com/kei/java/serialize.shtml)

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

## json deserialize object to int

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

[C# で数字を object 型にキャストした値型の扱いについて](https://cms.shise.net/2014/10/csharp-object-cast/)  
[【C#】Boxing / Unboxing ってどこで使われてるのか調べてみた](https://mslgt.hatenablog.com/entry/2017/11/18/132025)  

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

DependencyPropertyのBindingの件でazmさんがインスタンスの状態なんて事を言っていたので調べたわけだが、それとは別にインスタンスについての基礎を紹介しているページがわかりやすかったのでまとめる。  

後日、azmさんがまた使っていたので、意味合いから想像すると、例えばLinqで.Where()を実行したときと、.ToList()を実行したときでは、戻ってくる値が違うわけで、  
azmさんはどうやら帰ってくる型の事をインスタンスの状態と言っている見たいだ。  

[[C# 入門] クラスのインスタンスについて](https://yaspage.com/prog/csharp/cs-instance/)  

---

## try catchのスコープの話

tryで宣言した変数を外部で使うためには、tryの外で予め宣言しておいてからでないと使えないわけで、もっといい方法ないの？ってことで調べた。  

結局、これだ！っていう感じの答えは見つからなかった。  
更にtryで囲むとか？  
まぁ、理由があってこの形なのだから愚直にやるのが一番いいような気もする。  

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
>[「catch」または「finally」のスコープの「try」で変数が宣言されないのはなぜですか？](https://www.webdevqa.jp.net/ja/c%23/%E3%80%8Ccatch%E3%80%8D%E3%81%BE%E3%81%9F%E3%81%AF%E3%80%8Cfinally%E3%80%8D%E3%81%AE%E3%82%B9%E3%82%B3%E3%83%BC%E3%83%97%E3%81%AE%E3%80%8Ctry%E3%80%8D%E3%81%A7%E5%A4%89%E6%95%B0%E3%81%8C%E5%AE%A3%E8%A8%80%E3%81%95%E3%82%8C%E3%81%AA%E3%81%84%E3%81%AE%E3%81%AF%E3%81%AA%E3%81%9C%E3%81%A7%E3%81%99%E3%81%8B%EF%BC%9F/957401549/)  

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
