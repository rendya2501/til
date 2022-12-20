# Builder

---

## Builderパターンとは？

主に2つのパターンで使用される？

複雑な手順でオブジェクトを生成する場合のパターン。  
数多くの初期化パラメータを組み合わせて、少し性質の異なるオブジェクトをたくさん生成した場合。  

引数が多すぎて分かりにくい場合のパターン。  

---

## 複雑な手順でオブジェクトを生成するパターン

[【GoFのデザインパターン】「Builder」ってどんなパターン？](http://www.code-magagine.com/?p=2674)  

---

## 引数が分かりにくいパターン

>クラスを作ってみたが、コンストラクタの引数が多すぎて『順番わかりづらい。。。』ことがある。  
>
>例えば、『各教科の評価点を設定するクラス』を作ったけど、インスタンスを作ってみると・・・
>
>``` java
>//どの教科の設定なのかわかりづらい！！
>StudentGrade studentGrade = new StudentGrade("A","A","E","D","A","B","B");
>```
>
>引数が多すぎて、『どの教科の評価点なのか？』がわかりにくい。。。  
>こんなときは、コンストラクタではなくbuilderを提供する方法もある。  
>
>[コンストラクタの引数が多すぎる場合の対応方法。Builderを検討しよう。](http://java-study.blog.jp/archives/1030064889.html)  

---

## 名前付き引数があればビルダーパターンは必要ない？

ビルダーパターンは引数が多すぎる時、どの引数に何を指定しているのか分かりにくいので、それを解決するためのパターン。  
C#では名前付き引数があるので、引数が分かりにくい問題は解決している。  

GoFの時代には名前付き引数はなかったはずなので、言語の不足している機能を補うためのパターンだったということか。  

>builderパターンも名前付き引数あれば不要だし、デザインパターンは言語仕様の不備を補うものでしかないんですよねぇ。  
そもそも、人間がパターンを覚えて使って実装しないといけないというのは、ライブラリ化できてないという点でソフトウェア技術の負けではないかと思う。  
<https://twitter.com/kis/status/1362273897370972160>  

``` cs
StudentGrade grade = new StudentGrade(
    total: 100,
    arithmetic: 100,
    chemistry: 100,
    language: 100,
    english: 100,
    social: 100,
    history: 100
);

public class StudentGrade
{
    //総合評価
    private readonly int Total;
    //算数
    private readonly int Arithmetic;
    //化学
    private readonly int Chemistry;
    //国語
    private readonly int Language;
    //英語
    private readonly int English;
    //社会
    private readonly int Social;
    //歴史
    private readonly int History;

    public StudentGrade(
        int total,
        int arithmetic,
        int chemistry,
        int language,
        int english,
        int social,
        int history)
    {
        this.Total = total;
        this.Arithmetic = arithmetic;
        this.Chemistry = chemistry;
        this.Language = language;
        this.English = english;
        this.Social = social;
        this.History = history;
    }
}
```
