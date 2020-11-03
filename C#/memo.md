#　const,readonly,static readonlyの違い
https://qiita.com/4_mio_11/items/203c88eb5299e4a45f31

- const
コンパイル時に定義する。
なので、メソッドの結果など、実行しなければ確定しないものは定数として定義できない。
これによって、バージョニング問題が発生するとか。

class Hoge
{
    public const double PI = 3.14;     // OK   
    public const double piyo = PI * PI;     // OK
    public const double payo = Math.Sqrt(10);   // NG

    void Piyo(){
    //コンパイルで生成される中間言語では下の条件式はmyData == 3.14となる
    if(Moge == PI)
        //処理
 }
 
 
- readonly

コンストラクタでのみ書き込み可能。
それ以降は変更不可。


- static readonly
実行時に定まってほしい定数を定義する。
バージョニング問題的な観点から、constよりstatic readonlyが推奨される。
ちなみにconstも裏ではstatic扱いになっているので、staticを嫌悪する必要はない。

