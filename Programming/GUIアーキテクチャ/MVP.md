# MVP(Model View Presenter) パターン

## 概要

MVPはMVCの次。MVVMの前っぽい？

[【Unity】Model-View-(Reactive)Presenterパターンとは何なのか](https://qiita.com/toRisouP/items/5365936fc14c7e7eabf9)  

>Model-View-Presenter（MVP）パターンとは、GUIの設計パターンのうち「Presenter」という概念を用いたものです。  
>
>Model - データの実体。GUIとは直接関係ないアプリケーション本体の要素部分。  
>View - GUIを制御する部分。データを画面に表示したり、逆にユーザからの操作を受け付ける部分。  
>Presenter - ModelとViewをつなげる存在。仲介役。  
>
>MVPパターンで重要な点は1つです。  
>「Presenterが存在しなければ、ViewとModelは完全に独立した状態になる」という点です。  
>ViewとModelをつなげる存在はPresenterのみであるため、Presenterを排除するとこの2つは完全に独立することになります。  
>つまり「ViewはModelを完全に知らない」「ModelはViewを完全に知らない」ということになります。  

→リアルタイム性を求めるなら `Model-View-(Reactive)Presenterパターン` らしい。  

[MVP (Model View Presenter)パターン](http://csharper.blog57.fc2.com/blog-entry-245.html)  
>[MVP パターンとは]
>MVP (Model View Presenter) パターンは、MVC (Model View Controller) パターンの亜種です。
>大きな違いとして、MVC パターンでは Controller がユーザーからの入力イベントを受け取りますが、MVP パターンでは View がユーザーからの入力イベントを受け取り、処理を Presenter に委譲します。

[StackOverFlowの「MVPとMVCの違い」についての回答を読んでみた](https://qiita.com/takahirom/items/597c48ece57b4623cdee)  
![!](https://camo.qiitausercontent.com/120d9a4173c6a9037e0aa63b66ae0c9ce6c933ad/68747470733a2f2f71696974612d696d6167652d73746f72652e73332e616d617a6f6e6177732e636f6d2f302f32373338382f65366465633434362d383563352d656161662d653237662d3138383066363765313163662e706e67)  

→グラフの説明はここが一番わかりやすかった。  
MVPにも Passive View と Supervising Controller なるバージョンの違いがあるらしい。  
今回実装したのはPassiveView。  
これ以上のバージョンがあったとしても素直にMVVM使えってなるので、これ以上は詮索しない。  

---

## サンプル1

[Introducing MVP (Model-View-Presenter) Pattern (WinForms)](https://www.dreamincode.net/forums/topic/342849-introducing-mvp-model-view-presenter-pattern-winforms/)  
[WindowsフォームをMVPっぽくしてみる](https://qiita.com/mono1729/items/d0995f841b8e875b91e4)  
[MVP (Model View Presenter) Example (C# Code)](https://www.youtube.com/watch?v=UgnbIJYUTQY)  
[WinForms Model View Presenter](https://www.codeproject.com/Articles/14660/WinForms-Model-View-Presenter)  

ユーチューブの動画のサンプルをもとに、構造はcodeprojectのモノを組み合わせて作ったやつ。  
ガチガチにMVPしてみたけど、やりすぎな気がするし、ここまでやるならMVVMでいい気がする。  
エントリーポイントまで書き換えるのはやりすぎ。  
お手軽感が足りない。  

プロジェクト : プレーンWPF  
プロジェクトの構造  

``` txt
MVPSample
├─Models
│  └─IRectangleModel.cs
｜  └─RectangleModel.cs
├─Presenters
│  └─RectanglePresenter.cs
└─Views
│  └─IRectangleView.cs
│  └─MainWindow.xaml
└─App.xaml.cs
```

``` C# : Model
namespace MVPSample.Models
{
    public class RectangleModel : IRectangleModel
    {
        public double Length { get; set; }
        public double Breadth { get; set; }
        public double CalculateArea() => Length * Breadth;
    }
}
```

``` C# : Model Interface
namespace MVPSample.Models
{
    public interface IRectangleModel
    {
        double Length { get; set; }
        double Breadth { get; set; }
        double CalculateArea();
    }
}
```

``` C# : Presenter
using MVPSample.Models;
using MVPSample.Views;
using System;

namespace MVPSample.Presenters
{
    public class RectanglePresenter
    {
        readonly IRectangleView rectangleView;
        readonly IRectangleModel rectangleModel;

        public RectanglePresenter(IRectangleView view, IRectangleModel model)
        {
            rectangleView = view;
            rectangleModel = model;
            rectangleView.Calculate += new EventHandler((o,e) => CalculateArea());
        }

        public void CalculateArea()
        {
            rectangleModel.Length = double.Parse(rectangleView.LengthText);
            rectangleModel.Breadth = double.Parse(rectangleView.BreadthText);
            rectangleView.AreaText = rectangleModel.CalculateArea().ToString();
        }
    }
}
```

``` C# : Views
using MVPSample.Views;
using System;
using System.Windows;

namespace MVPSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// https://www.youtube.com/watch?v=UgnbIJYUTQY
    /// </summary>
    public partial class MainWindow : Window, IRectangleView
    {
        public MainWindow() => InitializeComponent();
        public string LengthText { get => txtLength.Text; set => txtLength.Text = value; }
        public string BreadthText { get => txtBreath.Text; set => txtBreath.Text = value; }
        public string AreaText { get => txtBlockArea.Text; set => txtBlockArea.Text = value + " Sq CM"; }
        public event EventHandler Calculate;
        private void Button_Click(object sender, RoutedEventArgs e) => Calculate?.Invoke(sender, e);
    }
}
```

``` C# : View Interface
using System;

namespace MVPSample.Views
{
    public interface IRectangleView
    {
        string LengthText { get; set; }
        string BreadthText { get; set; }
        string AreaText { get; set; }
        event EventHandler Calculate;
        void Show();
    }
}
```

``` XML : UI
<Window
    x:Class="MVPSample.MainWindow"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
    xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
    Title="MainWindow"
    Width="200"
    Height="180"
    mc:Ignorable="d">
    <Grid>
        <Label
            Margin="10,9,0,0"
            HorizontalAlignment="Left"
            VerticalAlignment="Top"
            Content="Length:" />
        <Label
            Margin="10,40,0,0"
            HorizontalAlignment="Left"
            VerticalAlignment="Top"
            Content="Breath:" />
        <TextBlock
            Name="txtBlockArea"
            Width="88"
            Margin="81,71,0,0"
            HorizontalAlignment="Left"
            VerticalAlignment="Top"
            Background="AliceBlue"
            Text="" />
        <Button
            Width="159"
            Height="21"
            Margin="10,92,0,0"
            HorizontalAlignment="Left"
            VerticalAlignment="Top"
            Click="Button_Click"
            Content="Calculate Area" />
        <TextBox
            x:Name="txtLength"
            Width="88"
            Height="26"
            Margin="81,9,0,0"
            HorizontalAlignment="Left"
            VerticalAlignment="Top"
            Text=""
            TextWrapping="Wrap" />
        <TextBox
            x:Name="txtBreath"
            Width="88"
            Height="26"
            Margin="81,40,0,0"
            HorizontalAlignment="Left"
            VerticalAlignment="Top"
            Text=""
            TextWrapping="Wrap" />
    </Grid>
</Window>
```

[C#のWPFでMainメソッドを編集する](https://araramistudio.jimdo.com/2018/04/06/c-%E3%81%AEwpf%E3%81%A7main%E3%83%A1%E3%82%BD%E3%83%83%E3%83%89%E3%82%92%E7%B7%A8%E9%9B%86%E3%81%99%E3%82%8B/)  

エントリーポイントの編集は少し面倒。  
WPFのエントリーポイントは自動生成(obj/Debug/App.g.cs)されるので、それを停止してApp.xamlで編集できるようにする必要がある。  

1. App.xamlを右クリックしてプロパティを選択  
2. ビルドアクションを[アプリケーション定義]から[ページ]に変更する  
3. そして下記コードをコピペする  

``` C# : EntryPoint
using MVPSample.Models;
using MVPSample.Presenters;
using MVPSample.Views;
using System;
using System.Windows;

namespace MVPSample
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
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
}
```

### サンプル所感

やった感じ、インターフェースでプロパティを定義して、定義したプロパティとコントロールを対応させて、各種イベントが発火したらガチャガチャするって感じだった。  
プロパティ増えたらインターフェース増やさないといけない。  
そのインターフェースがビューだけが実装しているならそれでいいのかもしれない。  
面白いと思ったけど、まだちょっと野暮ったい感じがした。  
色ツールくらいだったらちょうどいいかなと思うので、あっちをリファクタするときにMVPに変換して遊んでみるか。  

色ツールをMVVMで作るってなったらちょっとやりすぎだよなぁ。  
コードビハインドで書いた時に感じたのは、コントロールへの値の代入が同じこと書きすぎて無駄に感じることだ。  
確かにあれならプロパティ定義して、それと結びつけるだけでいいだろうとは思う。  

お手軽ツールでコードビハインドでガリガリやるのはちょっとな。  
でもMVVMにするほどでもないかなって時にちょうどいいかなといった印象。  

---

## MVP?

動画で紹介されていた本来のコード。
ViewがPresenterをnewしているのでがっつり依存している。  
調べて出てくるMVPとは構造が違うので、MVPだと言えないような気がする。  
しかし、お手軽ではある。  
ちょっとしたツールを作る時くらいはこのくらいの緩さでもいいような気がしたので残しておく。  

思ったが、MVVMではViewがViewModelをnewしているので、これも似たようなモノではなかろうか？  
いうなれば、お手軽MVVMと言ったところでは？  
xamlのDataContextの記事を見てみたが、`DataContext = new ViewModel();` とすることでバインディングしているので、多分この例もこれに当たる気がする。  

``` C# : Models
namespace MVPSample.Models
{
    public class Rectangle
    {
        public double Length { get; set; }
        public double Breadth { get; set; }
        public double CalculateArea() => Length * Breadth;
    }
}
```

``` C# : Presenters
using MVPSample.Models;
using MVPSample.Views;

namespace MVPSample.Presenters
{
    public class RectanglePresenter
    {
        private IRectangle RectangleView { get; set; }

        public RectanglePresenter(IRectangle rectangleView)
        {
            RectangleView = rectangleView;
            RectangleView.Calculate += new EventHandler((o, e) => CalculateArea());
        }

        private void CalculateArea()
        {
            Rectangle rectangle = new Rectangle()
            {
                Length = double.Parse(RectangleView.LengthText),
                Breadth = double.Parse(RectangleView.BreadthText)
            };
            RectangleView.AreaText = rectangle.CalculateArea().ToString();
        }
    }
}
```

``` C# : Views
using System;

namespace MVPSample.Views
{
    public interface IRectangle
    {
        string LengthText { get; set; }
        string BreadthText { get; set; }
        string AreaText { get; set; }
    }
}


using System.Windows;
using MVPSample.Views;
using MVPSample.Presenters;

namespace MVPSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// https://www.youtube.com/watch?v=UgnbIJYUTQY
    /// </summary>
    public partial class MainWindow : Window, IRectangle
    {
        private RectanglePresenter Presenter { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            // Presenterに依存している。
            Presenter = new RectanglePresenter(this);
        }

        public string LengthText { get => txtLength.Text; set => txtLength.Text = value; }
        public string BreadthText { get => txtBreath.Text; set => txtBreath.Text = value; }
        public string AreaText { get => txtBlockArea.Text; set => txtBlockArea.Text = value + " Sq CM"; }
        
        public event EventHandler Calculate;
        private void Button_Click(object sender, RoutedEventArgs e) => Calculate?.Invoke(sender, e);
    }
}
```
