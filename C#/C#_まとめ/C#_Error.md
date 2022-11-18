# C# エラー処理

## キャッチしたエラーの再スロー

- OK  
  - 引数なしでthrow  
  - InnerExceptionにセットしてスロー  
- NG  
  - 引数を付けてスロー  

○引数なしでthorw  

何も考えずthrowすればよろしい。  
下手に引数を付けるとスタックトレースが失われてしまうので何もつけずにthrowする。  

``` cs
try
{
    ErrorHoge();
}
catch (Exception ex)
{
    throw;
}
```

○InnerExceptionにセットしてスロー  

InnerExceptionにセットしてスローするとスタックトレースは失われない。  
別の例外に置き換える必要があるならこの方法を使う。  

``` cs
try
{
    ErrorHoge();
}
catch (Exception ex)
{
    throw new Exception("ReThrow", ex); // 引数付きでスロー
}
```

×引数を付けてスロー  

その地点で新しく例外が発生したと見なされるため、キャッチしたときまでのスタックトレースが失われてしまい、例外を引き起こした「犯人」が分からなくなってしまうのでNG。  

``` cs
try
{
    ErrorHoge();
}
catch (Exception ex)
{
    throw ex; // 引数付きでスロー
}
```

[構文：キャッチした例外をリスローするには？［C#／VB］](https://atmarkit.itmedia.co.jp/ait/articles/1701/11/news023.html)  

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
