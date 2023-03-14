# const,readonly

---

## 読み取り専用のローカル変数

C#に読み取り専用のローカル変数は存在しない。  
`readonly`キーワードはローカル変数に対応していない。  

初期代入可能で変更不可なローカル変数はC#では定義できない。  
Javaはできる。(final修飾子)  

読み取り専用のローカル変数は定義できないが、ローカル定数は定義できる。  
しかし、この変数は初期代入ができない。  

``` cs
void method1() 
{
    // ローカル定数を定義
    const int ID = 1;
    ID = 2; // エラー
}

void method2(int id) 
{
    // ローカル定数は初期代入ができない
    const int ID = id; // エラー
}
```

Javaは`final`修飾子で初期代入+変更不可のローカル変数を定義可能。  

>``` java
>void method() {
>    final int a; // 初期化されていないが、コンパイルエラーにはならない
>    System.out.println(a); // コンパイルエラー!! finalだが値が設定されていないため使用できない
>    a = 10;
>    System.out.println(a); // 変数に値が設定されたため参照できる
>    a = 123; // コンパイルエラー!! finalなので再代入はできない。
>}
>```
>
>[Javaのfinalを大解剖 finalの全てがここにある!!](https://www.bold.ne.jp/engineer-club/java-final)  

こちらの記事のコメントで言及がある。  

>C#のローカル変数にreadonlyがない理由は、おそらく明示しなくてもコンパイラにはわかることだからです。  
>ローカル変数は基本的にそのメソッド内でしかインスタンスを差し替えることができないので、  
>
>- メソッド内で再代入していない  
>- refつきパラメータとして他のメソッドに渡していない  
>
>この2つを満たしていれば明示しなくてもコンパイラはreadonly的な変数として扱います。  
>[【Unity】ローカル変数のreadonly（読み取り専用）を実現する方法【C#】 - Qiita](https://qiita.com/su10/items/602e89acfa0439c707ae)  

---

## const,readonly,static readonly

## const

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

---

## readonly

コンストラクタでのみ書き込み可能。  
それ以降は変更不可。  

---

## static readonly

コンパイル時ではなく、実行時に値が定まり、以後不変となる。  
コンパイルした後の話なので、メソッドを実行した結果をプログラム実行中の定数として扱うことができる。  
バージョニング問題的な観点から、constよりstatic readonlyが推奨される。  
ちなみにconstは暗黙的にstaticに変換されるので、staticを嫌悪する必要はない。  

---

[constとreadonlyとstatic readonly、それぞれの特徴と使い分け方](https://qiita.com/4_mio_11/items/203c88eb5299e4a45f31)  
