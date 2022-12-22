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
