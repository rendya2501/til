# CheckBox

## チェックボックスの3値判定

チェックボックスで3値の判定をするときの実装。  

リストが1件も無ければチェックを付けない。  
複数件あって、1つでもfalseが混ざっていれば、「-」を表示する。  
これはチェックボックスに置けるnullの状態。  
もちろん全てtrueならチェックを付けるし、全てfalseならチェックを付けない。  

そういう判定をスマートに出来ないかやってみた。  

■データ用意  

こういうデータがあったとする  

``` cs
var ListData = new List<Hoge>(){
    new Hoge(){
        HogeValue = "hoge1",
        IsSelected = true,
    },
    new Hoge(){
        HogeValue = "hoge2",
        IsSelected = true,
    }
};

class Hoge
{
    public string HogeValue { get; set; }
    public bool IsSelected { get; set; }
}
```

■ **switch式 Ver1**

1行で書ける。  
型変換処理を2回も書かないといけないのがちょっと気になるが、それ以外はスマート。  
switch式が使えるバージョンなら素直にswitch式を使うべし。  

``` cs
// false,trueの並びにする。
bool? flag = ListData?.Select(a => a.IsSelected).Distinct().OrderBy(o => o).ToList() switch
{
    // nullは0件とする。0件ということは、そもそもプレーヤーが1人もいないのでチェックボックスは空白(false)とする。
    IEnumerable<bool?> hoge when (hoge?.Count() ?? 0) == 0 => false,
    // 1件ということは、全部trueかfalseなので、そのまま通す。
    IEnumerable<bool?> hoge when hoge.Count() == 1 => hoge.First(),
    // 0でも1もない(実質2)ということは、trueとfalseが混在している状態なので、中間状態(null)とする。
    _ => null,
};
```

■ **switch式 Ver2**

is式を使うことで`when`による型変換を無くして、更にシンプルでわかりやすく記述することが出来た。  

``` cs
// false,trueの並びにする。
bool? flag = ListData?.Select(a => a.IsSelected).Distinct().OrderBy(o => o).ToList() is IEnumerable<bool> hoge
    ? hoge.Count() switch
    {
        // 0件ということは、そもそもプレーヤーが1人もいないのでチェックボックスは空白(false)とする。
        0 => false,
        // 1件ということは、全部trueかfalseなので、そのまま通す。
        1 => (bool?)hoge.First(),
        // 2件ということは、trueとfalseが混在している状態なので、中間状態(null)とする。
        2 => null,
        // あり得ないケースなのでthrowする。
        _ => throw new ArgumentException()
    }
    // nullはisで変換できないので空白(false)とする。
    : false;
```

■ **C# 7.3 で愚直にやった**

一回Listに取らないといけないので1行で書けない。  
AllやAnyを駆使すれば行けなくは無いのだろうけど、なんか違う。  

``` cs
// まずListにとる
var uniqueList = ListData?.Select(a => a.IsSelected).Distinct().OrderBy(o => o).ToList();
// countが0ということは、そもそもプレーヤーが1人もいないのでチェックボックスは空白(false)
bool? flag = (uniqueList?.Count() ?? 0) == 0
    ? false
    // countが2ということは、trueとfalseが混在している状態なので、中間状態(null)とする。
    : uniqueList.Count() == 2
        ? null
        // 全部trueならtrue,全部falseならfalseなのでそのまま通す。
        : (bool?)uniqueList.FirstOrDefault();
```

■ **C# 7.3 でワンセンテンスで行けた Ver**

こちらもis式を使うことでワンセンテンスで行くことが出来た。  
switch式がないので、三項演算子のネストになってしまうが、仕方がないか。  

``` cs
// false,trueの並びにする。
bool? flag = ListData?.Select(a => a.IsSelected).Distinct().OrderBy(o => o).ToList() is IEnumerable<bool> uniqueList
    // countが0ということは、そもそもプレーヤーが1人もいないのでチェックボックスは空白(false)とする。
    ? uniqueList.Count() == 0
        ? false
        // countが1ということは、全部trueかfalseなので、そのまま通す。
        : uniqueList.Count() == 1
            ? (bool?)uniqueList.First()
            // countが0でも1もない(実質2)ということは、trueとfalseが混在している状態なので、中間状態(null)とする。
            : null
    // nullはisでfalseとなるのでこちらに来る。そのまま空白(false)とする。
    : false;
```

[【WPF】DataTableバインドなDataGridでSelect Allチェックボックスを作る｜fuqunaga｜note](https://note.com/fuqunaga/n/n62c8d678f249)  
[[C#] switch文をswitch式で表現する | FEELD BLOG](https://feeld-uni.com/?p=1365)  
