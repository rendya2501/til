# MVP(Model View Presenter) パターン

## 概要

[【Unity】Model-View-(Reactive)Presenterパターンとは何なのか](https://qiita.com/toRisouP/items/5365936fc14c7e7eabf9)  

MVPはMVCの次。MVVMの前。

[MVP (Model View Presenter)パターン](http://csharper.blog57.fc2.com/blog-entry-245.html)  

[MVP パターンとは]
MVP (Model View Presenter) パターンは、MVC (Model View Controller) パターンの亜種です。
大きな違いとして、MVC パターンでは Controller がユーザーからの入力イベントを受け取りますが、MVP パターンでは View がユーザーからの入力イベントを受け取り、処理を Presenter に委譲します。


[Model]
Model は、ドメインモデルを表します。
ドメインとは、業務固有の問題領域のことです。

Model → View
Model は、View に依存しません。
Model → Presenter
Model は、Presenter に依存しません。


[View]
View は、Presenter の要求インターフェイスを実装し、ユーザーインターフェイスを直接操作します。
View は、極力無能にします。そのためには、UI コントロールとの直接的なやり取り以外をできるだけ行わないようにします。

View → Presenter
View は、Presenter に自分自身を関連付けます。
View は、ユーザーからの入力イベントを受け取り、処理を Presenter に委譲します。
View は、上記以外の目的で Presenter を操作しません。
View → Model
View は、Presenter から Model を受け取って出力することができます。
View は、Model を生成しません。
View は、出力に必要な操作以外で、Model を操作しません。


[Presenter]
Presenter は、View を通じて、ユーザーからの入力イベントを受け取ります。その後、Model を操作したり View を操作したりします。

Presenter → View
Presenter は、View から入力値を取得することができます。
Presenter は、View を操作することができます。
Presenter → Model
Presenter は、Model を生成したり操作することができます。
Presenter → その他
Presenter は、Model や View に属さないもの (データストアやハードウェア等) に関する処理を行うことができます。


[View と Presenter の関係]
前述の通り、View は、Presenter の要求インターフェイスを実装します。この要求インターフェイスは、View が極力無能になるように考慮して定義します。
Presenter と View は１対１の関係で、一つの Presenter を複数の View に関連付けることは通常しません。


[Model の変更通知について]
MVP パターンも MVC パターンと同じく、Model が View に変更を通知することができます。ただし、これは必須ではなく、この記事でもこれを含めていません。(つか、そっちはよく知りません。)
こちらの記事によると、このように Model の変更通知を無くして Presenter が完全に View 操作を行うことを、「慎ましいビュー (Humble View)」 と呼ぶそうです。

[StackOverFlowの「MVPとMVCの違い」についての回答を読んでみた](https://qiita.com/takahirom/items/597c48ece57b4623cdee)

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
