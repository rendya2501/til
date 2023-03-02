# Enum

---

## 基本

- 列挙子の標準の型はint  

- 別の型を指定することも可能  
  - 指定できるデータ : byte、sbyte、short、ushort、int、uint、long、ulong  

[【ソースコード有】列挙型とは？C#のenumの使い方を知ろう | 株式会社パソナ（旧パソナテック）｜ITエンジニア・ものづくりエンジニアの求人情報・転職情報](https://www.pasonatech.co.jp/workstyle/column/detail.html?p=7559)  

---

## EnumのGetValueOrDefault

EnumのGetValueOrDefaultは何が帰ってくるのか気になった。  
Byteなので普通に0だが、面白いのは定義したEnumが1から始まるものでもデフォルトは0とされること。  

``` C#
CaddyType? caddyType = null;
// Enumが1から始まっていても0となる。
var aa  = (int)caddyType.GetValueOrDefault();

enum CaddyType : byte
{
    None = 1,
    Use =2,
    UseTwo = 3,
}
```

---

## EnumのDisplayAtributeを取得する

``` C#
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine(Gender.Unknown.GetDisplayName()); // 不明Display
        Console.WriteLine(Gender.Unknown.GetDescription()); // 不明Description

        Console.WriteLine(Gender.Male.GetDisplayName());   // Male
        Console.WriteLine(Gender.Male.GetDescription());   // 男性Description

        Console.WriteLine(Gender.Female.GetDisplayName()); // 女性Display
        Console.WriteLine(Gender.Female.GetDescription()); // Female
    }
}

public enum Gender
{
    [Display(Name = "不明Display")]
    [Description("不明Description")]
    Unknown,

    [Description("男性Description")]
    Male,

    [Display(Name = "女性Display")]
    Female,
}

/// <summary>
/// Enum拡張クラス
/// </summary>
public static class EnumExtentions
{
    /// <summary>
    /// Enumに定義してあるDisplay属性を取得する。
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string GetDisplayName(this Enum value) =>
        Enum.IsDefined(value.GetType(), value)
            ? value.GetEnumAttribute<DisplayAttribute>()?.Name ?? value.ToString()
            : string.Empty;

    /// <summary>
    /// Enumに定義してあるDescription属性を取得する。
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string GetDescription(this Enum value) =>
        Enum.IsDefined(value.GetType(), value)
            ? value.GetEnumAttribute<DescriptionAttribute>()?.Description ?? value.ToString()
            : string.Empty;

    /// <summary>
    /// Attributeの値を取得する共通処理
    /// </summary>
    /// <typeparam name="TAttribute"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    private static TAttribute GetEnumAttribute<TAttribute>(this Enum value) where TAttribute : Attribute =>
        value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(TAttribute), false)?.OfType<TAttribute>()?.FirstOrDefault();
}
```

[Enumに定義してあるDisplay属性を表示する。リソースファイルがある場合、リソースから取得する拡張メソッド](https://qiita.com/mak_in/items/7909e51d249826115403)  
[Enumの値の属性を取得する](https://www.web-dev-qa-db-ja.com/ja/c%23/enum%E3%81%AE%E5%80%A4%E3%81%AE%E5%B1%9E%E6%80%A7%E3%82%92%E5%8F%96%E5%BE%97%E3%81%99%E3%82%8B/968546402/)  

---

## Enumに付与した属性と属性の値を取得する

Enumのアノテーションから値を取得するのは遅いので、速度が必要なければそれでもいいけど、別にDictionaryでEnumと文字列を定義して、それで取得でもいいのではという話。  

[C#でEnumに付与した属性と属性の値を取得する](https://takap-tech.com/entry/2018/12/20/231234)  

---

## 文字列 → Enum変換

- Parse() : 成功すれば変換された値が返ってくるが、失敗したときに例外を吐くので少々扱いにくい。  
- TryParse() : 変換の成否は戻り値。変換された値は第2引数でoutされる。  

普通に変換させる分にはEnum.TryParseで問題ない。  
数字からの変換が曲者。  
enum 型で定義していない値でも変換に成功したことにして outの変数に入れてしまう模様。  

``` C#
enum Weekday
{
    Sunday = 0,
    Monday = 1,
    Tuesday = 2,
    //...
    Saturday = 6
}

Weekday wd;
Enum.TryParse("2", out wd); // true, wd = Weekday.Tuesday →わかる
Enum.TryParse("Tuesday", out wd); // true, wd = Weekday.Tuesday →わかる
Enum.TryParse("April", out wd); // false, wd = Weekday.Saturday →わかる
Enum.TryParse("100", out wd); // true, wd = 100 →!!!!!!!!
```

ある値が enum 型で定義されているか検証するには、`Enum.IsDefined()` を使う。  
これを TryParse() と組み合わせれば、安全な変換が実現できる。  

``` C#
/// <summary>
/// Enum拡張クラス
/// </summary>
public static class EnumExtentions
{
    /// <summary>
    /// 拡張TryParse
    /// Enumに定義されていない値をfalseとします。
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <param name="s"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public static bool TryParse<TEnum>(string s, out TEnum result) where TEnum : struct =>
        Enum.TryParse(s, out result) && Enum.IsDefined(typeof(TEnum), result);
}
```

``` cs
EnumExtentions.TryParse("2", out wd); // true, wd = Weekday.Tuesday
EnumExtentions.TryParse("Tuesday", out wd); // true, wd = Weekday.Tuesday
EnumExtentions.TryParse("April", out wd); // false, wd = Weekday.Saturday
EnumExtentions.TryParse("100", out wd); // false, wd = 100 →falseになった
```

[文字列から enum 型への安全な変換](https://qiita.com/masaru/items/a44dc30bfc18aac95015)  

---

## 数値 → Enum変換

1. 単純なキャスト  
2. Enum.ToObject  

``` C#
enum SomeEnum {
  FOO = 1,
  BAR = 2,
  BAZ = 3,
}
```

()によるキャスト  

``` C#
int x = 1;
SomeEnum ex = (SomeEnum) x;
Console.WriteLine(ex); //=> FOO
```

Enum.ToObjectによるキャスト  
`typeof`によるキャストが可能なので、リフレクションで元の値に変換しなおしたい時にはこちらを使うべし。  

``` C#
int x = 1;
SomeEnum ex = (SomeEnum) Enum.ToObject(typeof(SomeEnum), x);
Console.WriteLine(ex); //=> FOO

// ()によるキャストをしないとobject型となってしまうが、値はしっかり反映されている。
var ex2 = Enum.ToObject(typeof(SomeEnum), x);
Console.WriteLine(ex2); //=> FOO
```

[[C#] 数値→enum値に変換する（Enum.ToObject）](https://csharp.programmer-reference.com/convert-int-enum/)  
[Type変数を使用して変数をキャストする](https://www.web-dev-qa-db-ja.com/ja/c%23/type%E5%A4%89%E6%95%B0%E3%82%92%E4%BD%BF%E7%94%A8%E3%81%97%E3%81%A6%E5%A4%89%E6%95%B0%E3%82%92%E3%82%AD%E3%83%A3%E3%82%B9%E3%83%88%E3%81%99%E3%82%8B/957671824/)  

---

## Enumのインクリメント

DataTrigger + Enum のサンプルを作っている時に、ボタンを押す度にEnumをインクリメントして、それに応じて色を変えるってサンプルをやっている時に、そこそこの試行錯誤といい感じにまとまったのでまとめ。  
もちろんずっとインクリメントするわけには行かないので、上限に達したら戻してやらないといけない。  
その塩梅をうまいこと書けなかったが、何とかワンセンテンスにまとめることができた。  

``` C#
public enum State
{
    Normal,
    Warning,
    Error
}
```

Stateが最大だったらもとに戻して、そうでなければインクリメント。  
参考演算子で ++Stateしたらインクリメントされた結果が戻ってくるのね。  
具体的にどうなってるんだろう。  
なんとなくこうかなって感じでやったらできたので、余裕があったら解析したい。  

■**最終的な形**  

``` C# : 最終的な形
public DelegateCommand ButtonCommand => new DelegateCommand(
    () => State = (State == State.Error) ? State.Normal : ++State
);
```

■**ボツ1**  

インクリメントした結果が最大値を超えていたらもとに戻す。  
一番愚直かもしれないが、ifを切らないといけないので、中括弧が絶対に必要。  
ここまでできるんだったらなんかあるだろうということで探求の旅に出た。

``` C# : ボツ1
public DelegateCommand ButtonCommand => new DelegateCommand(
    () =>
    {
        if (++State > State.Error) State = State.Normal;
    }
);
```

■**ボツ2**  

全部参考演算子で判定する。  
まぁ、中括弧はいらないが、毎回こんなことしてられないのでボツ。  

``` C# : ボツ2
public DelegateCommand ButtonCommand => new DelegateCommand(
    () => State = (State == State.Normal) 
           ? State.Warning 
           : (State == State.Warning)
               ? State.Error
               : (State == State.Error)
                   ? State.Normal
                   : throw new Exception("ありえん");
);
```

---

## Enum バックからの受け取り

asp側の定義

``` cs
public enum ResultStatus
{
    Success,
    Error
}
```

wpf側の定義  

これだとasp側からSuccessで帰ってきてもExecutionになってしまう。  
暗黙的に降られている番号的に一致するからだと思われる。  

``` cs
public enum ResultStatus
{
    Execution,
    Success,
    Error
}
```

合わせたければこうする事

``` cs
public enum ResultStatus
{
    Success,
    Error,
    Execution,
}
```

web側は結果を返すだけだが、フロントは実行中というステータスがあるのでこんな感じになっている。
