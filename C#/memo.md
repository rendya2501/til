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

## アノテーションを使った、リストに1件もない場合のバリデーション

<https://stackoverflow.com/questions/5146732/viewmodel-validation-for-a-list>  
画面にエラー状態は表示したくないけど、警告は出したい場合があったのでその備忘録。  

``` C#
[Range(1, int.MaxValue, ErrorMessage = "At least one item needs to be selected")]
public int ItemCount
{
    get => Items != null ? Items.Length : 0;
}
```

## C#のアクセス修飾子

<https://www.fenet.jp/dotnet/column/language/6153/>

`internal`がわからなかったのでついでに調べた。  

- public    : あらゆる所からアクセスできる  
- private   : 同じクラス内のみアクセスできる  
- protected : 同じクラスと派生したクラスのみアクセスできる  
- internal  : 同じアセンブリならアクセスできる  
- protected internal : 同じアセンブリと別アセンブリの派生クラスでアクセスできる  
- private protected  : 同じクラスと同じアセンブリの派生したクラスのみアクセスできる  
