# DependencyInjection

---

## DependencyInjectionとは？

クラスの内部でnewするのではなく、外部でnewしてそれをセッターやコンストラクタから渡してしてあげること = 注入して上げること。  

依存性の注入に関しては考え方や意味が全て。  
プログラミング的には大したことはほとんどしていない。  

---

## DIのお堅い概要

>DI (Dependency Injection、依存の注入)とは、設計方法の一つ。  
部品(クラス)同士の直接的な依存をなくし、部品をより抽象的なものに依存させる。  
これによりテストしやすくなったり、部品(クラス)の変更が容易になったりする。  
[DI (Dependency Injection) 依存の注入とは](https://demi-urge.com/dependency-injection/)  

---

## DIの考え方

クラス（実体）に依存させるのはやめて、インタフェース（抽象、ルール）に依存させようということ。  
部品(クラス)同士を直接繋ぐのをやめて、インタフェース(抽象、ルール)を介して繋ぐことで、インタフェースを満たす部品(クラス)との交換が可能となる。  

---

## DIにおける「依存性」と「注入」の意味

- 依存性  
(大雑把に)とあるクラスに、固定の定数、変数、インスタンスが入っちゃっている状態  
つまりそのクラスは、その定数、変数、インスタンスに依存している  

- 注入  
そのクラスの外から定数、変数、インスタンスをあるクラスにぶちこむこと  

注意) クラスだけに限らないという話もあるけど、ほとんどの場合クラスになる。

>DIという言葉のうち「Dependency（依存性）」という単語は、「オブジェクトが成立するために必要な要件」という意味を持っています。  
この要件とは、オブジェクトの持つ属性や関連するオブジェクトなどです。  
[atmarkit](https://atmarkit.itmedia.co.jp/ait/articles/0504/29/news022.html)  

「注入（Injection）」とは「外部からの設定（Configuration）」を意味しています。  
設定ファイルやWebアプリケーションのデプロイメントディスクリプタ（web.xmlなど）での設定を「注入」と呼んでいるのです。  

これらのことから「DI」という言葉を言い表すと「オブジェクトの成立要件に必要な情報を外部設定すること」となります。  
情報を外部に切り出すことで、たとえオブジェクトを利用する状況が変わったとしても、設定を変更するだけでそのオブジェクトを利用することができるようになります。  
つまり再利用性の高い「部品」としてオブジェクトを実装しやすくなるのです。  
このような再利用性の高いソフトウェア部品のことを「コンポーネント」と呼びます。  

---

## DIしないと何が問題なのか？依存していると何が嫌なのか?

とあるクラスが、固定した他の(定数、変数、クラスなど)に依存していると  

- 外から動的に動作を変更できないので、テストしづらい  
- 決め打ちなので、柔軟性がなくカスタマイズしにくい  

具体的には、あるクラスだけテストしたいのに  

- 中に別のクラスが入っているとテストしにくい  
- テストに時間がかかるメソッドが中にあってテスト終了に時間がかかる  

具体的に言うと、車クラスが内部で夏タイヤクラスをnewしていると、冬タイヤに変えるためには車ごと変えないといけない、という意味になってしまう。  

---

## それを解決するのがDI

依存性をなくすために、動的に動作を注入しようぜ! ってこと。  
あるクラス内の決め打ち定数、変数、インスタンスを排除して、外から注入することで、動的に動作を変えられるようにする。  
それを解決する方法が引数やコンストラクタで、クラスや変数を外から受け取れるようにする事 = DI  

車とタイヤの例に戻るが、そういうのはインターフェース(タイヤ接続のためのボルトとか位置とか大きさとかそういうやつ)を挟んでやると、そのインターフェスにあったタイヤとは付け替えできるよねって意味にできる。  

---

## メリット  

- ソフトウエアの階層をきれいに分離した設計が容易になる  
- コードが簡素になり、開発期間が短くなる  
- テストが容易になり、「テスト・ファースト」による開発スタイルを取りやすくなる  
- 特定のフレームワークへの依存性が極小になるため、変化に強いソフトウエアを作りやすくなる（＝フレームワークの進化や、他のフレームワークへの移行に対応しやすくなる）  

---

## デメリット  

- クラスをたくさん作るので大抵の場合はじめに工数がかかる場合が多いと思われる  
- プログラムの実行スピードが遅くなる可能性が高い  
- クラスやファイルが沢山できる  

---

## IoC(inversion of control)

制御の反転  
DIとセットで登場する用語。  

>「あるクラスA」が「あるサービスクラスB」を利用する場合を想>定します。  
>何も考えなければ、クラスAがクラスBを生成して（newして）使>用します。  
>
>``` cs
>public class A {
>  public A()
>  {
>    B b = new B();
>  }
>}
>```
>
>これに対してIoCの考えを適用した場合、「クラスBを欲するクラ>スA」に対して外部から「クラスBのインスタンス」を注入しま>す。
>
>``` cs
>public class A {
>  public A(B b)
>  {
>  }
>}
>```
>
>つまり、自ら生成するのに対して、外部から注入されるようにな>った、つまり制御が反転した、ということです。  
>[ryuichi111stdの技術日記](https://ryuichi111std.hatenablog.com/entry/2016/07/17/102129)  

---

クラスAがクラスBに依存している状況  

``` mermaid
classDiagram

ClassA ..> ClassB : 依存
```

``` cs
class B 
{
    public string GetHowToGreet()
    {
        return "こんにちは";
    }
}

class A 
{
    public void Greet() 
    {
        // BをAの内部でインスタンス化
        var b = new B();
        console.WriteLine(b.GetHowToGreet());
    }
}

var a = new A();
a.Greet();
```

依存性の逆転を行った場合

``` mermaid
classDiagram

ClassA ..> InterfaceB : 依存
InterfaceB <|.. ClassB : 実装
```

``` cs
interface InterfaceB 
{
    string GetHowToGreet();
}

class ClassA
{
    // Bのインターフェイスに依存する
    private InterfaceB _InterfaceB;

    // コンストラクタで外からBの実体を受け取る
    public ClassA(InterfaceB interfaceB) 
    {
        _InterfaceB = interfaceB;
    } 

    public void Greet() 
    {
        // 受け取ったBの実体を用いる
        console.WriteLine(_InterfaceB.GetHowToGreet());
    }
}

class ClassB : InterfaceB 
{
    public string GetHowToGreet()
    {
        return "こんにちは";
    }
}

// Bは外で生成して渡す
InterfaceB b = new ClassB();
ClassA a = new ClassA(b);
a.Greet();
```

[依存性の逆転のいちばんわかりやすい説明](https://zenn.dev/naas/articles/c743a3d046fa78)  
[【SOLID原則】依存性逆転の原則 - DIP](https://zenn.dev/chida/articles/e46a66cd9d89d1)  

---

## クラス図

``` mermaid
classDiagram

ClassA ..> InterfaceB : 依存
InterfaceB <|.. ClassB : 実装
```

``` cs
class ClassA 
{
    private InterfaceB _InterfaceB;

    public ClassA(InterfaceB interfaceB)
    {
        _InterfaceB = interfaceB;
    }
}

public interface InterfaceB
{
    public void MethodB();
}

class ClassB : InterfaceB
{
    public void MethodB(){
        // ..
    }
}
```

[[デザインパターン] Dependency injection(依存性の注入)を実装付きで解説 [初心者向け]](https://www.12-technology.com/2021/06/didependency-injection.html)  

---

## インジェクションの種類

- フィールドインジェクション  
- セッターインジェクション  
- コンストラクタインジェクション  

手動実装  
DIコンテナを用いた実装  

とりあえず、DIコンテナ使ってコンストラクタインジェクションで実装していれば間違いはない。  

---

## DIの実装例_手動のDI

wikiの例を参考。  
コンストラクタを用いた手動のDIを紹介。  
コンストラクターインジェクション  

``` java : DIを用いない状態
public class VerySimpleStockTraderImpl implements IAutomatedStockTrader {
    // 別のクラスを直接Newしている
    private IStockAnalysisService analysisService = new StockAnalysisServiceImpl();
    private IOnlineBrokerageService brokerageService = new NewYorkStockExchangeBrokerageServiceImpl();

    public void executeTrades() {}
}

// エントリーポイント
public class MyApplication {
    public static void main(String[] args) {
        IAutomatedStockTrader stockTrader = new VerySimpleStockTraderImpl();
        stockTrader.executeTrades();
    }
}
```

``` java : 手動でのDI
public class VerySimpleStockTraderImpl implements IAutomatedStockTrader {
    private IStockAnalysisService analysisService;
    private IOnlineBrokerageService brokerageService;
    // コンストラクタからインスタンスを取得
    public VerySimpleStockTraderImpl(IStockAnalysisService analysisService,IOnlineBrokerageService brokerageService) {
        this.analysisService = analysisService;
        this.brokerageService = brokerageService;
    }

    public void executeTrades() {}
}

// エントリーポイント
public class MyApplication {
    public static void main(String[] args) {
        // MyApplication.main()が依存性の注入を行っており、VerySimpleStockTraderImpl自体は特定の実装に依存しなくなっている。
        IStockAnalysisService analysisService = new StockAnalysisServiceImpl();
        IOnlineBrokerageService brokerageService = new NewYorkStockExchangeBrokerageServiceImpl();
        // この実装ではコンストラクタ注入の手法が用いられている。
        IAutomatedStockTrader stockTrader = new VerySimpleStockTraderImpl(analysisService,brokerageService);
        stockTrader.executeTrades();
    }
}
```

---

## DIの実装例_DIコンテナを用いた実装

[C#でDIコンテナを使用してみる](https://remix-yh.net/1332/)  
[Unity Container: Constructor Injection](https://www.tutorialsteacher.com/ioc/constructor-injection-using-unity-container)  
[.NET 系の DI コンテナ](https://qiita.com/okazuki/items/239ca5ef46e5a085e085)  

MVPSampleがいい例だったのでそのまま流用した。  

### DIなしで実装した場合

``` C# : DIなしで実装した場合
    public partial class App : Application
    {
        [STAThread]
        public static void Main()
        {
            IRectangleModel model = new RectangleModel();
            IRectangleView view = new MainWindow();
            _ = new RectanglePresenter(view, model);
            view.Show();
            new App().Run();
        }
    }
```

### IUnityContainerで実装した場合

``` C# : IUnityContainerで実装した場合
    public partial class App : Application
    {
        [STAThread]
        public static void Main()
        {
            IUnityContainer container = new UnityContainer();
            container.RegisterType<IRectangleView, MainWindow>();
            container.RegisterType<IRectangleModel, RectangleModel>();
            // DIコンテナを用いた場合、自動的に引数に注入してくれる模様。
            container.Resolve<RectanglePresenter>();
            container.Resolve<IRectangleView>().Show();
            container.Resolve<App>().Run();

            // 依存性を注入する部分はこういう風にもかける。だけど、ほぼ意味はないでしょう。
            //① これでもいける
            //container.RegisterInstance(new RectanglePresenter(view, container.Resolve<IRectangleModel>()));
            //② これでもいける
            //container.RegisterType<RectanglePresenter>(
            //    new InjectionConstructor(
            //        new ResolvedParameter<IRectangleView>(),
            //        new ResolvedParameter<IRectangleModel>()
            //    )
            //);
        }
    }
```

### Microsoft.Extensions.DependencyInjectionでの実装

ASP.NET Core などで何も考えない場合に使うことになる、事実上の標準の DI コンテナです。  
非常にシンプルで DI コンテナとして最低限これくらいは持ってるだろうと思われる機能だけ持ってます。  

[コンストラクタインジェクションはここを参考にした](http://surferonwww.info/BlogEngine/post/2021/01/01/dependency-injection-for-dotnet-core-application.aspx)  

AddTransient で登録するとコンテナから取得するたびに別のインスタンスを返します。  
AddSingleton で登録すると毎回同じインスタンスになります。  
AddScoped で登録すると同じスコープ内だと同じインスタンスになります。  
スコープを作るには ServiceCollection に BuildServiceProvider をした結果の ServiceProvider の CreateScope メソッドを使います。  

``` C# : Microsoft.Extensions.DependencyInjectionでの実装
    public partial class App : Application
    {
        [STAThread]
        public static void Main()
        {
            // Microsoft.Extensions.DependencyInjectionでの実装
            IServiceCollection services = new ServiceCollection();
            services.AddTransient<App>();
            services.AddScoped<IRectangleView, MainWindow>();
            services.AddScoped<IRectangleModel, RectangleModel>();
            services.AddTransient<RectanglePresenter>();
            // インスタンスを提供してくれる人を作る
            using var provider = services.BuildServiceProvider();
            provider.GetService<RectanglePresenter>();
            provider.GetService<IRectangleView>()?.Show();
            provider.GetService<App>()?.Run();
        }
    }
```

---

## 参考

[DI って何でするのかわからない人向けに頑張って説明してみる「本来の意味」](https://qiita.com/okazuki/items/0c17a161a921847cd080)  
[DI (依存性注入) って何のためにするのかわからない人向けに頑張って説明してみる](https://qiita.com/okazuki/items/a0f2fb0a63ca88340ff6)  
[「なぜDI(依存性注入)が必要なのか？」についてGoogleが解説しているページを翻訳した](https://qiita.com/mizunowanko/items/53eed059fc044c5aa5dc)  
[猿でも分かる! Dependency Injection: 依存性の注入](https://qiita.com/hshimo/items/1136087e1c6e5c5b0d9f)  
[atmarkit](https://atmarkit.itmedia.co.jp/ait/articles/0504/29/news022.html)  

大体、サルでも分かるリスペクト  
