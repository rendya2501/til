# MVP(Model View Presenter) パターン

## 概要

[【Unity】Model-View-(Reactive)Presenterパターンとは何なのか](https://qiita.com/toRisouP/items/5365936fc14c7e7eabf9)  

MVPはMVCの次。MVVMの前。

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
        public double Length { get; set; }
        public double Breadth { get; set; }
        public double CalculateArea();
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
紹介されているMVPとは言えないような気がするが、お手軽ではある。  
ちょっとしたツールを作る時くらいはこのくらいの緩さでもいいような気がしたので残しておく。  

思ったが、MVVMではViewがViewModelをnewしているので、これも似たようなモノではなかろうか？  
お手軽MVVMに片足突っ込んでる気がする。  

``` C#
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVPSample.Models
{
    public class Rectangle
    {
        public double Length { get; set; }
        public double Breadth { get; set; }
        public double CalculateArea() => Length * Breadth;
    }
}


using MVPSample.Models;
using MVPSample.Views;

namespace MVPSample.Presenters
{
    public class RectanglePresenter
    {
        readonly IRectangle rectangleView {get; private set;};

        public RectanglePresenter(IRectangle rectangleView)
        {
            rectangleView = rectangleView;
        }

        public void CalculateArea()
        {
            Rectangle rectangle = new()
            {
                Length = double.Parse(rectangleView.LengthText),
                Breadth = double.Parse(rectangleView.BreadthText)
            };
            rectangleView.AreaText = rectangle.CalculateArea().ToString();
        }
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        readonly RectanglePresenter presenter {get; private set;};
        public MainWindow()
        {
            InitializeComponent();
            presenter = new RectanglePresenter(this);
        }

        public string LengthText { get => txtLength.Text; set => txtLength.Text = value; }
        public string BreadthText { get => txtBreath.Text; set => txtBreath.Text = value; }
        public string AreaText { get => txtBlockArea.Text; set => txtBlockArea.Text = value + " Sq CM"; }
        private void Button_Click(object sender, RoutedEventArgs e) => presenter.CalculateArea();
    }
}
```
