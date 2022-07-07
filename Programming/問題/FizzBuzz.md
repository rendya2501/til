
# FizzBuzz

wiki
>最初のプレイヤーは「1」と数字を発言する。次のプレイヤーは直前のプレイヤーの発言した数字に1を足した数字を発言していく。  
ただし、3の倍数の場合は「Fizz」（Bizz Buzzの場合は「Bizz」）、5の倍数の場合は「Buzz」、3の倍数かつ5の倍数の場合（すなわち15の倍数の場合）は「Fizz Buzz」（Bizz Buzzの場合は「Bizz Buzz」）を数の代わりに発言しなければならない。  
>発言を間違えた者や、ためらった者は脱落となる。  
>
>FizzBuzz問題  
>このゲームをコンピュータ画面に表示させるプログラムとして作成させることで、コードが書けないプログラマ志願者を見分ける手法をジェフ・アトウッドがFizzBuzz問題 (FizzBuzz Question) として提唱した。  
>その提唱はインターネットの様々な場所で議論の対象になっている。  

---

## 普通のやつ

``` C# : 一番最初に自分が考えたやつ
    foreach (var s in Enumerable.Range(1, 30))
    {
        if (s % 15 == 0) Console.WriteLine("FizzBuzz");
        else if (s % 3 == 0) Console.WriteLine("Fizz");
        else if (s % 5 == 0) Console.WriteLine("Buzz");
        else Console.WriteLine(s);
    }
```

``` C# : 次に思いついたやつ
Console.WriteLine(
    string.Join(
        Environment.NewLine,
        Enumerable.Range(1, 30).Select(i =>
            i % 15 == 0 ? "FizzBuzz" :
            i % 3 == 0 ? "Fizz" :
            i % 5 == 0 ? "Buzz" :
            i.ToString())
    )
);
```

``` C# : https://codereview.stackexchange.com/questions/49058/single-line-fizzbuzz-solution-in-linq
    Enumerable.Range(1, 30).Select(i =>
        i % 15 == 0 ? "FizzBuzz" :
        i % 3 == 0 ? "Fizz" :
        i % 5 == 0 ? "Buzz" :
        i.ToString())
    .ToList()
    .ForEach(Console.WriteLine);
```

``` C# : https://unity-indie-dou.hatenablog.com/entry/2017/08/23/080000
    for (int i = 1; i <= 30; i++)
    {
        Console.Write(i % 3 == 0 ? "Fizz" : "");
        if (i % 3 == 0 && i % 5 != 0) { Console.WriteLine(); continue; }
        Console.WriteLine(i % 5 == 0 ? "Buzz" : i.ToString());
    }
```

---

## 剰余演算を用いない方法

### 最大公約数を使った方法

[C#:最大公約数を求める （ユークリッドの互除法）](https://qiita.com/gushwell/items/e9614b4ac2bea3fc6486)  

２つ以上の正の整数に共通な約数（公約数）のうち最大のものを最大公約数といいます．
例 12 と 18 の公約数は，1,2,3,6 で， 6 が最大公約数

２つ以上の正の整数の共通な倍数（公倍数）のうち最小のものを最小公倍数といいます．
例 2 と 3 の公倍数は，6,12,18,24,... で， 6 が最小公倍数

``` C# : 最大公約数で判定する方法
for (int i = 1; i <= 30; i++)
{
   if (Gcd(i, 15) == 15) Console.WriteLine("FizzBuzz");
   else if (Gcd(i, 15) == 3) Console.WriteLine("Fizz");
   else if (Gcd(i, 15) == 5) Console.WriteLine("Buzz");
   else Console.WriteLine(i.ToString());
}

public static int Gcd(int a, int b)
{
    int gcd(int x, int y) => y == 0 ? x : gcd(y, x % y);
    return a > b ? gcd(a, b) : gcd(b, a);
}
```

### 商を用いた方法

``` C# : https://739j.hatenablog.com/entry/2015/03/15/183519
int num3 = 1;
int num5 = 1;
for (int i = 1; i <= 30; i++)
{
    if (i / 3 == num3 && i / 5 == num5)
    {
        Console.WriteLine("FizzBuzz");
        num3++;
        num5++;
    }
    else if (i / 3 == num3)
    {
        Console.WriteLine("Fizz");
        num3++;
    }
    else if (i / 5 == num5)
    {
        Console.WriteLine("Buzz");
        num5++;
    }
    else
    {
        Console.WriteLine(i.ToString());
    }
}
```

### 小数点以下に数字が存在するかどうかで判定する方法

一番最初に考えたやつだけど、小数点第一が0だった場合アウトなのでダメ。

``` C#
    for (decimal i = 1M; i <= 30; i++)
    {
       var a = (i / 3).ToString("0.0");
       var b = (i / 5).ToString("0.0");
       if (a.Substring(a.Length - 1, 1) == "0" && b.Substring(b.Length - 1, 1) == "0") Console.WriteLine("FizzBuzz");
       else if (a.Substring(a.Length - 1, 1) == "0") Console.WriteLine("Fizz");
       else if (b.Substring(b.Length - 1, 1) == "0") Console.WriteLine("Buzz");
       else Console.WriteLine(i.ToString());
    }
```

小数点でsplitした結果、配列の数が1なら割り切れたと判定。  
2なら割り切れなかったと判定する方法。  
ToStringしたら0.333333とかがなくなってしまうのに苦労した。  
double型なら小数点まで見てくれる模様。  

``` C#
    for (double i = 1.0D; i <= 30; i++)
    {
        int a = (i / 3.0D).ToString().Split('.').Length;
        int b = (i / 5.0D).ToString().Split('.').Length;

        if (a == 1 && b == 1) Console.WriteLine("FizzBuzz");
        else if (a == 1) Console.WriteLine("Fizz");
        else if (b == 1) Console.WriteLine("Buzz");
        else Console.WriteLine(i.ToString());
    }
```
