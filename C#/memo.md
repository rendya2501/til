# 雑記

## キャスト

「is」,「as」「前かっこ」による変換の違い。  

・前かっこは変換に失敗するとエラー(System.InvalidCastException)になる。  
・as は変換に失敗するとnullになる。エラーにはならない。  
・is はif文で使うので、失敗したらelseに流れるだけ。  

## タプルのリストを簡単に初期化する方法

<https://cloud6.net/so/c%23/1804197>

``` C#
    // List ver
    var tupleList = new List<(int Index, string Name)>
    {
        (1, "cow"),
        (5, "chickens"),
        (1, "airplane")
    };
    // 配列 ver
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

## Taskのラムダ式の定義の仕方

async/await を使った非同期ラムダ式を変数に代入する方法とも言うか。  
<https://qiita.com/go_astrayer/items/352c34b8db72cf2f6ca5>  
いつもの癖でAction型に入れようとして少し苦戦したので備忘録として残すことにした。  

``` C#
    Func<Task> AsyncFunc = async () =>
    {
        await Task.Delay(1);
    };
```

## is演算子によるnullチェック

クラスAのインスタンスを作ります。  
インスタンス変数にnullを代入します。  
それをオブジェクト型変数bに代入します。  
bをAにキャストした場合どうなるか。  
→  
nullの場合は失敗するが、nullを代入しないと成功する。  
でもなんで？  

``` C#
public class Hello{
    class A {
    }

    public static void Main(){
        A a = new A();
        a = null; // ※
        object b = a;
        
        if (b is A aa) {
            // nullを代入しないとこちら
            System.Console.WriteLine("Success");
        } else {
            // a = nullの場合はこちら
            System.Console.WriteLine("Fail");
        }
    }
}
```

<https://ufcpp.net/study/csharp/datatype/typeswitch/>  
元々のis演算子の仕様でもあるんですが、nullには型がなくて常にisに失敗します(falseを返す)。  
なるほどね。is演算子は実行時の変数の中身を見るが、nullは型がないので、エラーになるわけか。  
納得。  
しかし、言語仕様で困ったらマイクロソフトではなく未確認飛行Cを見るのが一番だな。  
こっちのほうがわかりやすい。  

``` C#
string x = null;

if (x is string)
{
    // x の変数の型は string なのに、is string は false
    // is 演算子は変数の実行時の中身を見る ＆ null には型がない
    Console.WriteLine("ここは絶対通らない");
}
```

## TextBoxで未入力の場合にBindingしてるソースのプロパティにnullを入れたい

<https://blog.okazuki.jp/entry/20110411/1302529830>

なんてことはない知識だが、一応MDの練習やTILのために追加。  
テキストボックスの空文字をNULLにしたい場合はどうすればいいのか分からなかったから調べたらドンピシャのがあったので、メモ。  

TargetNullValueプロパティを以下のように書くことで、空文字のときにnullがプロパティに渡ってくるようになります。

``` XML
<TextBox Text="{Binding Path=NumberInput, UpdateSourceTrigger=PropertyChanged, TargetNullValue=''}" />
```

TargetNullValueはこの値が来たらnullとして扱うことを設定するためのプロパティの模様。  

## SignalR

非同期でリアルタイムな双方向通信を実現するライブラリ。  
WebSocketのASP.Netバージョンみたい。  
2012年あたりから記事が見つかるので、結構古い技術なのかもしれない。  

概要はそれだけなので、そこから先は実際にサンプルを組んで実践していくしかない。  
そもそも、WebSocketプログラミングすらしたことないからな。  
一番身近な例が株価出はなかろうか。  
あれは更新せずとも刻一刻とチャートが変化する。  
それはサーバー側からプッシュ通信がされるからだろう。  
それのASP.Netバージョンでしか本当にないのだろう。  

## DataGrid – アクティブセルの枠線を消す（C# WPF)

<http://once-and-only.com/programing/c/datagrid-%E3%82%A2%E3%82%AF%E3%83%86%E3%82%A3%E3%83%96%E3%82%BB%E3%83%AB%E3%81%AE%E6%9E%A0%E7%B7%9A%E3%82%92%E6%B6%88%E3%81%99%EF%BC%88c-wpf/>  

支払方法変更処理の単体テスト戻りでScrollViewerにフォーカスが当たって点線の枠が表示されてしまう問題が発生した。  
この点線をどう表現して調べたらいいかわからなかったところ、ドンピシャな記事があったので、備忘録として残す。  
因みに「wpf scrollviewer　点線」で調べた模様。  

``` XML
<DataGrid.CellStyle>
    <Style TargetType="DataGridCell">
        <Setter Property="BorderThickness" Value="0"/>
        <!-- 
            点線の枠→FocusVisualStyle : キーボードから操作した時にfocusが当たった時のスタイル
            FocusVisualStyle に Value="{x:Null}"を設定すると消せる
        -->
        <Setter Property="FocusVisualStyle" Value="{x:Null}"/>
    </Style>
</DataGrid.CellStyle>
```

## シリアライズとデシリアライズを繰り返すと？

<https://www.jpcert.or.jp/java-rules/ser10-j.html>  
支払方法変更処理において、F4の初期化を実行するとメモリーがどんどん増えていくことに気が付いた。  
そこでは、まっさらなデータをDeepCopy(シリアライズとデシリアライズ)して代入する処理をしていたのだが、  
もしかしてこの場合、ガベージコレクションされないのかなと思ったらされない模様。  
おとなしくループさせて、必要な項目だけを初期化したらメモリーが増えることはなくなった。  

シリアライズがどういうことをするのか説明できないので、その記事も参考に置いておきますね。  
<http://funini.com/kei/java/serialize.shtml>  

シリアライズとは、メモリ上のデータをバイトに変換すること。  
メモリ上のそのインスタンスが保持するデータ全てがバイト列に置き変わる。  
バイト列なので、ポインタで指し示すデータではなく、実データとなる。  
それを元に戻すのがデシリアライズ。  
デシリアライズすると、バイト列のデータが元に戻るので、全く同じデータを持つインスタンスを作ることができる。  
もちろんこの時、メモリーのアドレスやポインタ等は新しくなっている。  
しかし、これを延々と繰り返すと、同じデータが無限に増やせるので、メモリリークの原因になるらしい。  

## async voidのラムダ式

<https://stackoverflow.com/questions/61827597/async-void-lambda-expressions>  

async void ○○ await △△ みたいな非同期処理を1行で書けないか探したが、全然そんなこと書いてるところがない。  
「async void lambda c#」で調べてようやくそれっぽいところにたどり着いたが、本当にあってるのかはわからない。  

``` C#
    private async void Hoge()
    {
        await Task.Delay(1000);
    }
    private void Main()
    {
        // async void HogeはTask.Runのように書いても動くけど、厳密には少し違うみたい。
        Hoge();
        // これは正確にはWait1000みたいな意味合いらしい。
        Task.Run(async () => await Task.Delay(1000));
    }
    private async Task Wait1000() {
        await Task.Delay(1000);
    }
    Task.Run(Wait1000);
```
