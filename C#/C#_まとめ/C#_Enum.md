# Enum

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

## Enumに付与した属性と属性の値を取得する

[C#でEnumに付与した属性と属性の値を取得する](https://takap-tech.com/entry/2018/12/20/231234)  

Enumのアノテーションから値を取得するのは遅いので、速度が必要なければそれでもいいけど、別にDictionaryでEnumと文字列を定義して、それで取得でもいいのではという話。  

---

## 文字列を enum 型 に変換する方法

[文字列から enum 型への安全な変換](https://qiita.com/masaru/items/a44dc30bfc18aac95015)  

普通に変換させる分にはEnum.TryParseでよろしい。  
数字の文字列をEnumに変換するときは一工夫必要。  

- Parse() : 成功すれば変換された値が返ってくるが、失敗したときに例外を吐くので少々扱いにくい。  
- TryParse() : 変換の成否は戻り値。変換された値は第2引数でoutされる。  

数字からの変換が曲者で、enum 型で定義していない値でも変換に成功したことにして outの変数に入れてしまいます。  
`Enum.TryParse("100", out wd); // true, wd = 100`  

ある値が enum 型で定義されているか検証するには、Enum.IsDefined() を使います。  
これを TryParse() と組み合わせれば、安全な変換が実現できます。  

``` C#
static class EnumExt
{
    static bool TryParse<TEnum>(string s, out TEnum wd) where TEnum : struct
    {
        return Enum.TryParse(s, out wd) && Enum.IsDefined(typeof(TEnum), wd);
    }
}

Weekday wd;
EnumExt.TryParse("Thursday", out wd); // true, wd = Weekday.Thursday
SolarSystem ss; // Sun=0, Mercury, Venus, ...
EnumExt.TryParse("5", out ss); // true, ss = SolarSystem.Jupiter
```

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

``` C# : 最終的な形
public DelegateCommand ButtonCommand => new DelegateCommand(
    () => State = (State == State.Error) ? State.Normal : ++State
);
```

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
