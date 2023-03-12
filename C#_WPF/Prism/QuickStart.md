# Prismを用いたMVVMサンプル

## 環境

- Windows10以降  
- .NET6.0  
- VisualStudioCode  
- WPF  
- Prism.Core --version 8.1.97  

---

## プロジェクトの構成

``` txt
/PrismQuickStart
    /ViewModels
        MainWindowViewModel.cs
    /Views
        MainWindow.xaml
        MainWindow.xaml.cs
    App.xaml
    App.xaml.cs
```

- Views : 画面表示に関するファイルを格納するフォルダ  
- ViewModels : 画面と処理を繋ぐViewModelファイルを格納するフォルダ  
- Models : ビジネスロジックに関する処理を格納するフォルダ  
- Common : 共通的なロジックを格納するファルダ  

今回はビジネスロジックはないためModelは省略。  
Prism.CoreではBindableBase等の共通コードは提供されるので、Commonも省略。  

---

## プロジェクトの作成

ターミナルから`dotnet new`コマンドを実行する。  
`dotnet new wpf -o PrismQuickStart -f net6.0`  

VSCodeを起動する。  
`code PrismQuickStart`  

VSCodeのターミナルを起動。  
`ctrl + j`  

CommunityToolkitをnugetからインストール  
`dotnet add package Prism.Core --version 8.1.97`  

---

## ViewModels

`ViewModels`フォルダを作成し、`MainWindowViewModel.cs`を新規作成する。  

以下のコードをコピペする。  

``` cs
using Prism.Mvvm;
using Prism.Commands;

namespace PrismQuickStart.ViewModels;

public class MainWindowViewModel : BindableBase
{
    /// <summary>
    /// インクリメントコマンド
    /// </summary>
    /// <value></value>
    public DelegateCommand IncrementCommand { get; private set; }
    
    /// <summary>
    /// デクリメントコマンド
    /// </summary>
    /// <value></value>
    public DelegateCommand DecrementCommand { get; private set; }

    /// <summary>
    /// カウントプロパティ
    /// </summary>
    /// <value></value>
    public int Count
    {
        get { return _Count; }
        set { SetProperty(ref _Count, value); }
    }
    private int _Count;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public MainWindowViewModel()
    {
        IncrementCommand = new DelegateCommand(Increment);
        DecrementCommand = new DelegateCommand(Decrement,() => Count > 0);
    }

    /// <summary>
    /// インクリメントコマンドの処理
    /// カウント数を増やす
    /// </summary>
    private void Increment()
    {
        Count++;
        DecrementCommand.RaiseCanExecuteChanged();
    }

    /// <summary>
    /// デクリメントコマンドの処理
    /// カウント数を減らす
    /// </summary>
    private void Decrement()
    {
        Count--;
        DecrementCommand.RaiseCanExecuteChanged();
    }
}
```

---

## Views

`Views`フォルダを作成する。  

`MainWindow.xaml`と`MainWindow.xaml.cs`を`Views`フォルダに移動させる。  

### MainWindow.xaml

`x:Class`を`x:Class="PrismQuickStart.MainWindow"`から`x:Class="PrismQuickStart.Views.MainWindow"`に変更。  

`xmlns:vm="clr-namespace:PrismQuickStart.ViewModels"`を追加。  

`Window.DataContext`プロパティに`vm:MainWindowViewModel`を追加。  

ボタンとテキストブロックを追加する。  

最終形は以下の通り。  

``` xml
<Window x:Class="PrismQuickStart.Views.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        xmlns:local="clr-namespace:PrismQuickStart"
        xmlns:vm="clr-namespace:PrismQuickStart.ViewModels"
        mc:Ignorable="d"
        Title="MainWindow" Height="450" Width="800">
    <Window.DataContext>
        <vm:MainWindowViewModel />
    </Window.DataContext>
    <Grid>
        <StackPanel Orientation="Vertical" >
            <Button Content="Increment" Command="{Binding IncrementCommand}" />
            <Button Content="Decrement" Command="{Binding DecrementCommand}" />
            <TextBlock Text="{Binding Count}" />
        </StackPanel>
    </Grid>
</Window>
```

### MainWindow.xaml.cs

namespaceを`PrismQuickStart`から`PrismQuickStart.Views`に変更。  

``` cs
using System.Windows;

namespace PrismQuickStart.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
    }
}
```

### App.xaml

`StartupUri`を`MainWindow.xaml`から`Views/MainWindow.xaml`に変更する。  

``` xml
<Application x:Class="PrismQuickStart.App"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:local="clr-namespace:PrismQuickStart"
             StartupUri="Views/MainWindow.xaml">
    <Application.Resources>
         
    </Application.Resources>
</Application>
```

---

## プロジェクトの起動

VSCodeのターミナルで起動コマンドを実行する。  
`dotnet run`  

---

[WPF4.5入門 その60「データバインディングを前提としたプログラミングモデル」 - かずきのBlog@hatena](https://blog.okazuki.jp/entry/2014/12/23/180413)  
