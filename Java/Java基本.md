# Java基本まとめ

## final

・クラスに付ける場合→継承の禁止  
・メソッドにつける場合→オーバーライドの禁止  
・変数に付ける場合→再代入の禁止(定数)  

---

## throws

メソッドの宣言に追加するキーワード。  
指定した例外を発生させることを宣言する。  
throwsがついたメソッドを呼び出す場合は例外の対策をしないといけない。  
それだけの宣言みたいだ。  
実際あまり使われないみたいだし、インターフェースと組み合わせた場合、  
抽象が実装に引っ張られるから失敗作だとどこかで効いたことがある。  

---

## implements

javaのインターフェース実装命令。  
C#もこれだったか？  
違う。C#は「:」で済むんだった。であれば継承は?  
継承も「:」か。  
概念は知っているが、あまり使わないからパッと出てこないな。  

## extends

javaにおける継承です。

---

## import static 命令

クラスのメンバーをクラス名を指定せずに呼び出せるようになる。  

``` Java
// Mathクラスの全てのメンバ指定
import static java.lang.Math.*;
public class Sample{
    public static void main(String[] args) {
        // 普通Math.absって呼び出すけどそうする必要がない。
        System.println(abs(-10));
    }
}
```

---

## Enum

[【初心者向け】JavaのEnum型について解説！基本的なメソッドも紹介！](https://www.tech-teacher.jp/blog/java-enum/)  
基本情報でEnumのメソッドを知らないために1問間違えたのでまとめ。  

JavaにおいてはEnumはClassの一種っぽい。  
Objectクラスを継承している。  
とりあえず、JavaのEnumにはOrdinalメソッドなるものが実装されているらしい。  
他にも4つくらいあるが、これはJavaの仕様でそう実装されているみたい。  
この際なので全部まとめる。  

・ordinalメソッド  
Enum型の宣言された順番を取得するメソッド。  
→  
基本情報で出たのはこれ。  
電卓の数字の定義くらいなら順番に取得すれば、その通りになるって寸法か。  

・valueOfメソッド  
指定した文字列と列挙型を比較するときに使います。  

``` Java
public class Test {
    protected enum Name {Tanaka, Sato, Kimura};
    public static void main(String[] args) {
        if(Name.valueOf("Tanaka") == Name.Tanaka) {
            System.out.println("同じ");
        }
    }
}
```

・valuesメソッド  
列挙型のデータすべてを取得する場合に使います。  

---

## HashMap

C#でいうListっぽいことしてるのがこれ。  
Listではaddメソッドで要素を追加するが、JavaのHashMapはput(置く)がそれに当たるらしい。  
直観的にそうだろうとは思ったが、確証がなかったので後学のためにまとめておく。  

---

## toStringが実行されるタイミングは？

``` txt
図1「メソッドmainの出力結果」の通り、"(2 + 5)"と表示するコードです。
Addition クラスの toString メソッドが正解なので add.toString() としたいですが、選択肢にはありません。
Javaの言語仕様上、System.out.println() の引数にオブジェクトを渡すと、内部ではそのオブジェクトの toString() メソッドが呼び出されます。
このため、add オブジェクトのみでも同様の動作となり、整形式が出力されます。よって、解答は「ア」です。
```

なるほどね。まぁ、選択肢をヒントにして考えてみても、そういう動作してくれないと答えにならないからね。  
実行されるタイミングというよりも、何が実行するのかってのが適切かな。  

---

## ==とEqualsの比較の違いは？

オブジェクトの同一性、同値性に関する設問です。  

**同一性**  
同じオブジェクトを参照していることです。「==」で比較します。  

**同値性**  
オブジェクトの値が等しいことです。比較する値は各クラスによって独自に実装します。  
全てのクラスの親であるObjectクラスが実装しているequalsメソッドでのチェックは同一性チェック。  

問題文に「二つのインスタンスを表す値が等しいので true になるべきだが、今のクラス Constant の実装では false になる」という記述があります。  
「値」なので同値性チェックとなる。  
同値性チェックで true にしたい場合、Javaでは equals() メソッドをオーバーライドします。  
equals() メソッドは、java.lang.Object クラスに実装されており、全てのクラスは Object クラスのサブクラスです。  
よって、オーバーライドしない場合、Object クラスの equals() メソッドが呼ばれます。  
そして、Objectクラスのequalsメソッドでのチェックは同一性チェックです。  

new Constant(9) == new Constant(9)
同一性チェックのため問題文に合いません。

new Constant(9).equals(new Constant(9))
正しい。

new Constant(9).evaluate() == new Constant(9).evaluate()
同一性チェックのため問題文に合いません。

new Constant(9).evaluate().equals(new Constant(9).evaluate())
コンパイルエラーです。evaluate() メソッドの戻り値はプリミティブ型(int型)です。

``` Java : Constantクラスにequalsメソッドを実装する場合の例
@Override
public boolean equals(Object obj) {
    if (obj instanceof Constant) {
        return this.evaluate() == ((Constant)obj).evaluate();
    } else {
        return false;
    }
}
```

---

## foreach

[拡張for文(for-each文)を使って要素を順に取得する](https://www.javadrive.jp/start/for/index8.html)  

Javaに置けるforeachは「拡張for文」というものを使って実現するらしい。  
どうやらJava7までは拡張for文がforeach代わりになっていて、Java8からLinqみたいな感じのforEach命令が追加されたらしい。  
C#みたいに書く、foreach文はJavaには存在しない模様。  
JavaScriptのforeachもこんな感じだったような。  
まぁ、どちらにせよ、何やりたいかわかるので別にいいや。  

``` Java : 拡張For文・Foreach
for (データ型 変数名: コレクション){
    // 繰り返しの中で実行される処理
    ...
}

String name[] = {"Suzuki", "Katou", "Yamada"};
for (String str: name){
  System.out.println(str);
}

// foreachで書くならこうなる
name.forEach(s -> System.out.println(s));
```

---

## abstractのメソッドの実行順

基本情報の平成27年春でメソッドの流れを追えなかったのでまとめた。  
全部ソースの中であれこれしているので、そっちを見ればわかる。  
あとどうでもいいけど、stringってS大文字じゃないと認識してくれないんだな。

``` java
abstract public class Encoder{
    // 実装強制。HtmlEncoderクラスに実装されるメソッド。
    abstract protected String encode(char c);
    // 1.テストクラスから呼び出されるencodeメソッド
    public String encode(String s) {
        String result = "";
        for (char c : s.toCharArray()){
            String t = conversionTable.get(c);
            // 2.文字列を1文字ずつ読みこんだ時に、登録された文字では無かったら、abstractを実装したクラスのencodeメソッドを実行する。
            if (t == null) {
                // 3.ここで呼び出すのが、HtmlEncoderのencodeメソッド
                t = encode(c);
            }
            result += t;
        }
        return result;
    }
}

public class HtmlEncoder extends Encoder {
    // 3.というわけで色々やってHtmlEncoderのencodeメソッドが呼び出されるわけである。
    protected String encode(char c){
        // ごにょごにょ
    }
}

public class HtmlEncoderTest {
    static public void main(String[] args){
        // 1.この時呼ばれるencodeメソッドはEncoderクラスのencodeメソッド。その証拠にstring型だ。
        // HtmlEncoderクラスのencodeメソッドを呼び出しているように見えるけど、引数の関係で親クラスのメソッドを呼び出している状態である。
        // C#のAPI側の保存メソッドの時も、こういうからくりで動いているのだろう。
        // abstract側で定義されたメソッドから、各インスタンスのメソッドを呼び出していたわけだ。
        // 引数の型の違いによるオーバーロードはC#では出来ないと思ってたけど、普通に出来たしJavaでもできることを確認した.
        new HtmlEncoder().encode("<script>alert('注意');</script>");
    }
}
```

---

## charについて

これも基本情報の平成27年春をやっていて、int(char)って一体何が出力されるのかわからなかったのでまとめる。  
というか、char全般についてあまりわかってないと思うし、いい機会ではないだろうか。  

因みにint(char)のキャストはその文字コードに変換するという意味らしい。  
→  
Javaに限らずプログラムやコンピュータは、内部ではすべてを数字で表現します。文字や文字列もすべて数字です。  
例えば、‘A’という文字には、Unicodeでは65という数字が割り当てられています。‘あ‘は12354です。  

``` Java
public class Main {
    public static void main(String[] args) throws Exception {
        System.out.println((int)'A');  // 65
        System.out.println((int)'あ'); // 12354
    }
}
```

結局charってのは、文字を格納する型だ。  
stringはcharの配列で出来上がってて、stringの1文字1文字がcharに相当するのだ。  
JavaはUnicode準拠なので、intで変換したらその文字に対応した数字が返されるし、逆に数字を入れたらその数字に対応する文字が帰ってくる。  
とりあえず、これだけで今は十分では無かろうか。  
言語の根幹に関わる部分なので、量が半端ないのだ。  

### 詳しく

[charは文字でStringは文字列! Javaでの文字の扱い方を基礎から解説](https://www.bold.ne.jp/engineer-club/java-char)  

**・Javaのcharは16ビット(2バイト)のプリミティブ型で、Unicodeという文字コード規格での一文字を、0～65,535の範囲の数字で表したものです。**  
・プログラムやコンピュータでは、文字も数字で表しますので、charがJavaで文字を扱う時の最小単位です。  
・charは数字でもあり文字でもあります。文字そのもの、2進数、10進数、16進数、Unicodeのコードポイントなどが使えます。  
・Javaの文字列であるStringは、charが集まってできたものとも言えるので、charとStringは大変関連の深い間柄です。  
・charはプリミティブ型ですので「数字そのもの」です。ですから、何かのクラスのインスタンスではありません。  
・クラスとしてcharを表現する場合は、JavaではCharacterというクラスを使います。  

文字と数字の紐付け方には、たくさんのルールがあります。Javaではその一つであるUnicodeが最初から採用されていて、その中のUTF-16というルールが使われています。
このUTF-16の“16”は、16ビットが処理単位だよということで、だからcharは16ビットなのです。
同じようなルールには、日本語環境ではいわゆるシフトJISやEUC-JP、JISなどがあります。Unicodeの中でも、UTF-8やUTF-32というものもあります。
Javaでもこれらのルールはもちろん扱えますが、Javaのプログラム中で文字・文字列を扱う時は、UnicodeおよびUTF-16を前提にするのが普通です。

``` Java
String s = "ABC"; // ABCという文字列を、
char[] c = s.toCharArray(); // 一文字ずつの文字の配列に変換できる
System.out.println(s); // → ABC
System.out.println(c[0]); // → 1番目の文字はA、
System.out.println(c[1]); // → 2番目の文字はB、
System.out.println(c[2]); // → 3番目の文字はC

char[] c = { 'あ', 'い', 'う', 'え', 'お' }; // あ い う え おの文字の配列を、
String s = new String(c); // 文字列に変換すれば、
System.out.println(s); // → 文字列の"あいうえお"になる

char c = 'A'; // → OK、charに文字を代入する
String s = "A"; // → OK、Stringに文字列を代入する

char c2 = "A"; // → NG(コンパイルエラー)、charに文字列を代入しようとしている
String s2 = 'A'; // → NG(コンパイルエラー)、Stringに文字を代入しようとしている

// char(文字)とString(文字列)は比較もできない
// ↓違うもの同士を比較しているので、コンパイルエラー
if ('A' == "A") { System.out.println("'A' == \"A\"です");}

// ↓これはコンパイルエラーにはならないが、同じモノとは判断されない
if ("A".equals('A')) { System.out.println("\"A\".equals('A')です");}

// charを変数として扱う場合のサンプル
char c1 = 65; // 変数の宣言と初期値(65→A)の代入を同時に行う
System.out.println(c1); // → A

// charを配列として扱う場合のサンプル
char[] charArray1; // 配列型の変数を宣言して、
charArray1 = new char[3]; // 配列の実体をnewして紐付けて、
charArray1[0] = 65; // インデックスへ数字を代入する
charArray1[1] = 66;
charArray1[2] = 67;

char[] charArray2 = {97, 98, 99}; // 宣言と初期化を同時に行う

System.out.println(java.util.Arrays.toString(charArray1)); // → [A, B, C]
System.out.println(java.util.Arrays.toString(charArray2)); // → [a, b, c]

// いずれもコンパイルエラー!!
char c1 = 3.14; // → "doubleからcharには変換できません"
char c2 = -1; // → "intからcharには変換できません"
char c3 = 65536; // → 同上
// これらの数字をcharにキャストすることはできる
char c1 = (char)3.14; // → 3
char c2 = (char)-1; // → 65535
char c3 = (char)65536; // → 0

// いずれも65(文字のA)を、見た目の表現を変えて書いているだけ
char c0 = 65; // 10進数
char c1 = 0b0100_0001; // 2進数(Java 7以降、0b始まり、"_"で区切ることもできる)
char c2 = 0101; // 8進数(0始まり)
char c3 = 0x41; // 16進数(0x始まり)

System.out.println(c0); // → A
System.out.println(c1); // → A
System.out.println(c2); // → A
System.out.println(c3); // → A

char c1 = 'A'; // 数字で65と書く代わりに、'A'でも同じ意味になる
char c2 = 'B'; // 66の代わりに'B'
char c3 = 'あ'; // 12354の代わりに'あ'

System.out.println(c1); // → A
System.out.println(c2); // → B
System.out.println(c3); // → あ

char c1 = 'AB'; // → コンパイルエラー "文字定数が無効です"
char c2 = "A"; // → コンパイルエラー "Stringからcharには変換できません"
String s = 'A'; // → コンパイルエラー "charからStringには変換できません"

// 'X'は内部的には数字なので、数字と比較できる
if ('A' == 65) {
    System.out.println("'A' == 65です"); // こちら!!
} else {
    System.out.println("'A' == 65ではありません");
}

// 'X'は内部的には数字なので、計算ができる
int i1 = -'A';
int i2 = 'a' * -1;
int i3 = 'A' + 'a';
System.out.println(i1); // → -65
System.out.println(i2); // → -97
System.out.println(i3); // → -162、文字列の"Aa"にはならない

/* ２-２-３．特殊な文字はエスケープシーケンスで表す*/
// 印字が難しい文字はエスケープシーケンスでも表せる
char c1 = '\b';
char c2 = '\t';
char c3 = '\n';
char c4 = '\f';
char c5 = '\r';
char c6 = '\"';
char c7 = '\'';
char c8 = '\\';

// Stringリテラルでも同じ書き方ができる
String s1 = "\b";
String s2 = "\t";
String s3 = "\n";
String s4 = "\f";
String s5 = "\r";
String s6 = "\"";
String s7 = "\'";
String s8 = "\\";

/*２-２-４．Unicodeのコードポイントでの文字指定*/
char c1 = '\u3042'; // → 'あ'
char c2 = '\u00C6'; // → 'Æ'(AとEが合わさった文字)

System.out.println('あ' == '\u3042'); // → true
System.out.println(0x3042 == '\u3042'); // → true

System.out.println('Æ' == '\u00C6'); // → true
System.out.println(0x00C6 == '\u00C6'); // → true

String s = "\u3042\u00C6"; // → "あÆ"、Stringリテラルでも同じ書き方ができる

// ?(U+20BB7)はサロゲートペアと呼ばれる範囲にある文字で、1つのcharでは表現できない
char c0 = '?'; // → コンパイルエラー、この文字は0～65535の範囲内ではないため

// サロゲートペアの文字は、JavaのUnicodeエスケープでも表現できない
char c1 = '\u20BB7'; // → コンパイルエラー、これは\u20BBと7に解釈されてしまう
String s1 = "\u20BB7"; // → コンパイルエラー、理由は同上

// サロゲートペアの文字は、2つのcharが「ペア」になって1つの文字になる
char[] c2 = { 0xD842, 0xDFB7 }; // 1つの文字を表すのに長さ2のchar配列が必要
String s2 = "\uD842\uDFB7";
System.out.println(c2); // → ?
System.out.println(s2); // → ?

// サロゲートペアの文字のコードポイントは、32ビットのintで表現する
int i = 0x20BB7;
String s3 = String.format("%c", i); // → "?"
String s4 = Character.toString(i); // → "?"、Java 11より使用可能
```

---

## ジェネリック

[Javaジェネリクス入門、その1](<https://www.gwtcenter.com/java-generics-primer-1>)  
数あるジェネリクスの記事の中でも群を抜いてわかりやすい。  

ジェネリクスがない時代、主にArrayListの話になるが。  
・ArrayListにはなんでも入れられた。なのでstring型しか入れたくないのに、意図しないものが入る可能性があった。  
・取り出すときは毎回キャストが必要だった。  
・stringしか入れたくない場合、それ専用のクラスを作る必要があったが、必要に応じて数が増えるので面倒だった。  

それらを解決したのがジェネリクス  
・使える型を限定できるし、取り出すときにキャストする必要もない。  
・専用のクラスを毎回作る必要もない。  

まさに夢のような話だったのである。  

つまり、ジェネリクスの意味としては、こんな風にかけるだろう。  
・あらかじめ一般的な機能を実現する物を作っておき、使うときにそれを何に使うかを限定する。  

これを、具体的に例えを用いて言えばこうなる。  
・あらかじめ生物が何でも入るリストという機能を作っておき、使うときには人間のリスト、魚のリストと言ったように限定して使う。  

ということである。こうしておけば、わざわざ個別に「人間のリスト」「魚のリスト」と言ったものを作成する必要はなくなる。  

``` java : 簡単なジェネリクスのプログラム
public class Sample {
    static class General<T> {
        T object;
        void put(T object) { this.object = object; }
        T get()            { return object; }
    }
// もしTがString（文字列）の場合、つまり「General<String>」とした場合には、以下のように「変身する」わけである。
//     String object;
//     void put(String object) { this.object = object; }
//     String get()            { return object; }

// 因みに値が決まっていない時のTは実質的にはObjectである。

    public static void main(String[]args) {
        // 文字列を格納するものとして使う
        General<String>foo = new General<>();
        foo.put("this is test");
        String strResult = foo.get();
        System.out.println(strResult);

        // 整数を格納するものとして使う
        General<Integer>bar = new General<>();
        bar.put(123);
        int intResult = bar.get();
        System.out.println(intResult);
    }
}
```

定義  
・型パラメータ付のクラスを定義する。これをジェネリッククラスと言う。  
・その型パラメータ名称を、実際に使われる際の型として、クラス内のあちこちで使う。  

使用時  
・実際に使うときは、型パラメータの代わりに実際の型を指定する。  
・すると、先の型パラメータが、その型に変身したかのような効果を持つ。  

### 結局のところジェネリクスとは何をするものか？

これは結局のところ、  
・どのような型を受け入れるのか、その制約を作るものである。その制約範囲外のものをエラーにしてしまう。  
・制約を作れば、受け入れたものは、その制約内であることがわかっているので、そこで許されることが可能になる。  

例えば  
・哺乳類しか受け入れないという制約を作った。だから哺乳類以外のものはエラーにする。  
・哺乳類という制約があるのだから、その制約内に存在する授乳という機能を使うことができる。  

### 余裕があればまとめなおすかも

とりあえず、ジェネリックが何かはその4まで読めば十分だ。  
その先は中々深い内容なので、余裕があれば別でまとめたいと思います。  

・メソッド単位でジェネリックの指定ができる。  
・複数の型パラメーターを指定することができる。  
・親クラスで定義したジェネリック、子クラスの読みだしのみ可能。→共変  
・親クラスで定義したジェネリック、子クラスの書き込みのみ可能。→反変  
・再帰的ジェネリックなるものがある。→フルーエントインターフェース  

---

## コンストラクタ

平成17年春のJavaでコンストラクタが複数あった場合、コンストラクタ内でthisを使うことで別のコンストラクタを呼び出すことができるルールを地味に知らなかったのでまとめ。
というか、基本中の基本なんだよなぁ。  
C# ではどう書くのかもまとめたいねぇ。  

[Javaのthisを総ざらい! this/this()の意味・使い方](https://www.bold.ne.jp/engineer-club/java-this)  
記事の内容はそれではないが、分量がすごいのと、コンストラクタの部分だけ抜き出してまとめる。  

### 2．メソッドのthis()

２-１．this()は自分自身のコンストラクタを呼ぶ時に使う
Javaでプログラムを書いていると、あるコンストラクタから、オーバーロードした別のコンストラクタを明示的に呼びたい時があります。その時にthis()を使います。
コンストラクタの中から呼べるコンストラクタのメソッド名は“this”で固定なので、オーバーロードしたコンストラクタの内のどれを呼び出すかは、
コンストラクタの引数で区別します。
以下の例で言うと、③→②→①の順番にコンストラクタが呼ばれ、①→②→③の順番で実行されていきます。

``` java
class ThisSample7 {
    String name;
    int amount;

    // ①デフォルトコンストラクタ
    ThisSample7() {
        this.name = "名無しの権兵衛";
        this.amount = 0;
    }

    // ②オーバーロードしたコンストラクタ(その1)
    ThisSample7(String name) {
        this(); // ①のコンストラクタを呼び出す
        this.name = name;
    }

    // ③オーバーロードしたコンストラクタ(その2)
    ThisSample7(String name, int amount) {
        this(name); // ②のコンストラクタを呼び出す
        this.amount = amount;
    }

    public static void main(String[] args) {
        // ③のコンストラクタを呼び出す
        new ThisSample7("山田太郎", 1000);
    }
}
```

２-２．this()の使い方/コンストラクタの使い分け
this()の良くある使い道は、複数の引数を持つ共通的なコンストラクタを用意しておいて、
呼び出すコンストラクタに応じて設定値を変えたりデフォルト値を設定する、というものです。
それによりインスタンスの初期化処理を一つのコンストラクタにまとめられるので、プログラムが分かりやすくなりますし、保守性も高まります。
この例だと、フィールドの実質的な初期化はコンストラクタ③で行っています。
①と②はデフォルト値を引数に、③を呼び出しているだけです。これによりフィールドの初期化処理の記述を③だけに出来ています。

``` java
class ThisSample8 {
    String name;
    int amount;

    // ①デフォルトコンストラクタ
    ThisSample8() {
        this("名無しの権兵衛", 10); // デフォルト値で③を呼び出す
    }

    // ②オーバーロードしたコンストラクタ(その1)
    ThisSample8(String name) {
        this(name, 20); // 引数を使いつつ、③を呼び出す
    }

    // ③オーバーロードしたコンストラクタ(その2)
    ThisSample8(String name, int amount) {
        // フィールドの初期化はこのコンストラクタだけで行う
        this.name = name;
        this.amount = amount;
    }

    void printProperty() {
        System.out.println(String.format("私の名前は%sです。所持金は%d円です。", name, amount));
    }

    public static void main(String[] args) {
        ThisSample8 sample1 = new ThisSample8();
        sample1.printProperty(); // → 名無しの権兵衛、10円

        ThisSample8 sample2 = new ThisSample8("山田太郎");
        sample2.printProperty(); // → 山田太郎、20円

        ThisSample8 sample3 = new ThisSample8("鈴木次郎", 1000);
        sample3.printProperty(); // → 鈴木次郎、1000円
    }
}
```

### C#の場合

[[C#] 複数のコンストラクタを呼び出す(this初期化子)](https://webbibouroku.com/Blog/Article/cs-ctor)  

場合コンストラクタの横に`: this()`の形で書くみたい。  
後でC#側にもまとめると思う。  

``` C#
class Person
{
    public int Age;
    // ①
    public Person() : this(0)
    {
        Console.WriteLine("コンストラクタ - 2");
    }
    // ②
    public Person(int age)
    {
        Console.WriteLine("コンストラクタ - 1");
        this.Age = age;
    }
}

var p = new Person();
// まず①が実行されるが、this(0)とあるので、②に引数「0」が渡されて②が実行される。
// それが終わってから①の処理となるので、以下のような順番で出力される。
// コンストラクタ - 1
// コンストラクタ - 2
```

---

## 配列

### 多次元配列

平成27年秋期のJavaで多次元配列が出てきた。  
多次元配列のlengthは何を返すのかわからなかったのでまとめる。  

[多次元配列の長さを取得する(length)](https://www.javadrive.jp/start/array/index9.html)  
1次元配列の場合は配列の要素数を返しますが、 2次元配列以上の場合は配列変数の length フィールドを参照すると一番外側の配列の要素数を返します。  

``` Java
    byte[][] blocks = new byte[100][512];
    // blocks.length → 100と表示される。
    System.out.println(blocks.length);
    // blocks[0].length → 512と表示される。
    System.out.println(blocks[0].length);
    // blocks[0][0]は何も入っていないので 0と表示される。
    System.out.println(blocks[0][0]);
    // このようなアクセスは出来ない。
    // System.out.println(blocks[0][0].length);

    // こういうこともできるみたい。
    int[][] num = new int[2][];
    num[0] = new int[3];
    num[1] = new int[4];
    System.out.println(num.length);     // 2
    System.out.println(num[0].length);  // 3
    System.out.println(num[1].length);  // 4
```

C#の多次元配列とJavaの多次元配列の宣言の仕方って違うんだな。  
C#の場合はカンマ区切りで指定するらしい。  
lengthで吐き出される値も違う。  
Javaに置ける多次元配列はC#に置けるジャグ配列みたいなものかもしれない。  
微妙にアクセスの仕方が異なる。  
詳しくはC#のほうで。  

[配列の宣言は種類が多いな～JavaとC#の両方で出来るのどれ？](http://juujisya.jugem.jp/?eid=7)  
すごいドンピシャな記事があった。  

``` C#
    // 多次元配列
    // a.length → 51200と表示される。
    // a[0].lengthの指定はできない。
    var a = new byte[100,512];

    // 配列の配列
    // こっちはジャグ配列って奴みたい。
    // こっちならb[0].lengthの指定はできる。エラーになるけど。
    int[][] b = new int[2][];
```

---

## その他小話

Javaに破棄はない。  
Java10からvarによる変数の宣言ができるようになった。  
