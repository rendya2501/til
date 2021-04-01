# 雑記

「前かっこ」と「as」による変換の違い。  
前かっこは変換に失敗するとエラーになる。  
as は変換に失敗するとnullになる。エラーにはならない。  

## タプルのリストを簡単に初期化する方法

<https://cloud6.net/so/c%23/1804197>

``` C#
    var tupleList = new List<(int Index, string Name)>
    {
        (1, "cow"),
        (5, "chickens"),
        (1, "airplane")
    };
    var tupleList = new (int Index, string Name)[]
    {
        (1, "cow"),
        (5, "chickens"),
        (1, "airplane")
    };
```
