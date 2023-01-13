# Switch

switch式 は `C# 8.0 (.NetCore 3.1)` からの機能。  

---

## C# 8.0 の switch式でORを再現する

switch式中で複数条件の判定を行いたい。  
C#9.0 (.Net5)なら`or演算子`で可能だが、C# 8.0では使えない。  
代替案を見つけたのでメモする。  

■**C# 8.0での対応**  

一度変数受けして、`||`で振り分ける。  

``` cs
int switchValue = 1;
var resultText = switchValue switch
{
    var x when x == 0 || x == 2 => "zero or two",
    var x when x == 1 || x == 3 => "one or three",
    _ => "unknown",
};
```

■**C# 9.0での対応**  

C# 9.0ならorで記述可能。  

``` cs
int switchValue = 1;
var resultText = switchValue switch
{
    0 or 2 => "zero or two",
    1 or 3 => "one or three",
    _ => "unknown",
};
```

[c# 8.0 - C# 8 switch expression with multiple cases with same result - Stack Overflow](https://stackoverflow.com/questions/56676260/c-sharp-8-switch-expression-with-multiple-cases-with-same-result)  

---

## switch文のパターンマッチング

プレーヤーの席を交換したい。  
プレーヤーはそれぞれの型を持っているので、そのまま代入すると参照を入れ替えるだけで意味がない。  

if文で愚直にobjがPlayer1か2か判定してたけど、型で判定できないか探してみた。  
switch文で型の判定ができる模様。  

``` cs
private void SwapRepre(ReservationPlayerView obj)
{
    if (pushFrom.ReservationPlayer1 == obj)
    {
        (repre.ReservationPlayer4, pushFrom.ReservationPlayer1) = (pushFrom.ReservationPlayer1, repre.ReservationPlayer4);
    }
    else if (pushFrom.ReservationPlayer2 == obj)
    {
        (repre.ReservationPlayer4, pushFrom.ReservationPlayer2) = (pushFrom.ReservationPlayer2, repre.ReservationPlayer4);
    }
    else if (pushFrom.ReservationPlayer3 == obj)
    {
        (repre.ReservationPlayer4, pushFrom.ReservationPlayer3) = (pushFrom.ReservationPlayer3, repre.ReservationPlayer4);
    }
    else if (pushFrom.ReservationPlayer4 == obj)
    {
        (repre.ReservationPlayer4, pushFrom.ReservationPlayer4) = (pushFrom.ReservationPlayer4, repre.ReservationPlayer4);
    }
}
```

結果的にif文より短くかけたけど、思ってたのと若干違った。  

``` C#
private void SwapRepre(ReservationPlayerView obj)
{
    // 押された場所を特定する
    switch (obj)
    {
        // switchのパターンマッチング
        case ReservationPlayerView n when n == frame.ReservationPlayer1:
            // 一時変数と押された場所は今の代表者と入れ替えるのでbeRepreActionを登録する
            return (frame.ReservationPlayer1, (repre) => frame.ReservationPlayer1 = repre);
        case ReservationPlayerView n when n == frame.ReservationPlayer2:
            return (frame.ReservationPlayer2, (repre) => frame.ReservationPlayer2 = repre);
        case ReservationPlayerView n when n == frame.ReservationPlayer3:
            return (frame.ReservationPlayer3, (repre) => frame.ReservationPlayer3 = repre);
        case ReservationPlayerView n when n == frame.ReservationPlayer4:
            return (frame.ReservationPlayer4, (repre) => frame.ReservationPlayer4 = repre);
        default:
            break;
    }
    
    // これがswitchだと下のようになる
    // if (pushFrom.ReservationPlayer1 == obj)
    // {
    //     (repre.ReservationPlayer4, pushFrom.ReservationPlayer1) = (pushFrom.ReservationPlayer1, repre.ReservationPlayer4);
    // }
    // else if (pushFrom.ReservationPlayer2 == obj)
    // {
    //     (repre.ReservationPlayer4, pushFrom.ReservationPlayer2) = (pushFrom.ReservationPlayer2, repre.ReservationPlayer4);
    // }
    // else if (pushFrom.ReservationPlayer3 == obj)
    // {
    //     (repre.ReservationPlayer4, pushFrom.ReservationPlayer3) = (pushFrom.ReservationPlayer3, repre.ReservationPlayer4);
    // }
    // else if (pushFrom.ReservationPlayer4 == obj)
    // {
    //     (repre.ReservationPlayer4, pushFrom.ReservationPlayer4) = (pushFrom.ReservationPlayer4, repre.ReservationPlayer4);
    // }
}
```

``` C#
public string NankaMethod3(object obj)
{
    // Object型をそれぞれの型で判定し、その中身も同時に判定する
    switch (obj)
    {
        // objがint型 かつ 0より大きい
        case int x when x > 0:
            return x.ToString();
        // objがint型 かつ 0以下
        case int x when x <= 0:
            return (-x).ToString();
        // objがfloat型
        case float f:
            return ((int) f).ToString();
        // objが1文字以上のstring型
        case string s when s.Length > 0:
            return s;
        // どれにもマッチしなかった
        default:
            throw new ArgumentOutOfRangeException(nameof(obj));
    }
}
```

[C#のアプデでめちゃくちゃ便利になったswitch文（パターンマッチング）の紹介](https://qiita.com/toRisouP/items/18b31b024b117009137a)

---

## Switch式でメソッドを実行できないか色々やった結果

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
    // 元のコード。
    // {}がない分まだすっきりしているが、e.PropertyNameを毎回書かないといけないなら,それはswitchを使うべき。

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

    // 本命。
    // 文法上は怒られないけど実行するとエラーになる。
    // System.Runtime.CompilerServices.SwitchExpressionException: 'Non-exhaustive switch expression failed to match its input.'

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
    result.invoke();
}
```

C#8.0以降ではスイッチ式が利用可能。  

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

        // 4を1行に出来なくはないが・・・。ないだろうなぁ。
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

[c# 8.0 - Using blocks in C# switch expression? - Stack Overflow](https://stackoverflow.com/questions/59729459/using-blocks-in-c-sharp-switch-expression)  
