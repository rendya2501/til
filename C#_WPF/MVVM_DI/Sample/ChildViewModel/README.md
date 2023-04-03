# Prism DI サンプル

## 仕様

1つのViewと1つのViewModelがあるとします。  

ViewにはTabControlがあり、Tab1とTab2があります。  
Tab1にはテキストボックス1つとボタンがあり、ボタンを押したらテキストボックスの中身がメッセージボックスで表示される。  
Tab2にはテキストボックスが2つあり、2つの入力を結合して出力するようなテキストブロックも配置。  

そのような仕様の中で、TabごとにChildViewModelをインジェクションするような実装を行う。

---

## プロジェクトの作成

`dotnet new wpf -n ChildViewModel -f net6.0`  

---

## Prism ライブラリのインストール

`dotnet add package Prism.Unity`  

---

## 各種実装

### App.xaml

``` xml
<prism:PrismApplication
    x:Class="ChildViewModel.App"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:local="clr-namespace:ChildViewModel"
    xmlns:prism="http://prismlibrary.com/">
    <Application.Resources />
</prism:PrismApplication>
```

### App.xaml.cs

``` cs
using ChildViewModel.ViewModels;
using ChildViewModel.Views;
using Prism.Ioc;
using Prism.Mvvm;
using System.Windows;

namespace ChildViewModel;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    protected override Window CreateShell()
    {
        return Container.Resolve<MainWindow>();
    }

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.Register<Tab1ViewModel>();
        containerRegistry.Register<Tab2ViewModel>();
        containerRegistry.Register<MainWindowViewModel>();
    }

    protected override void ConfigureViewModelLocator()
    {
        base.ConfigureViewModelLocator();
        ViewModelLocationProvider.Register<MainWindow, MainWindowViewModel>();
    }
}
```

### MainWindow.xaml

``` xml
<Window
    x:Class="ChildViewModel.Views.MainWindow"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
    xmlns:local="clr-namespace:ChildViewModel"
    xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
    xmlns:prism="http://prismlibrary.com/"
    Title="MainWindow"
    Width="800"
    Height="450"
    prism:ViewModelLocator.AutoWireViewModel="True"
    mc:Ignorable="d">
    <Grid>
        <TabControl>
            <TabItem Header="Tab 1">
                <Grid>
                    <Grid.RowDefinitions>
                        <RowDefinition Height="Auto" />
                        <RowDefinition Height="Auto" />
                    </Grid.RowDefinitions>
                    <TextBox Grid.Row="0" Text="{Binding Tab1.TextBoxContent, UpdateSourceTrigger=PropertyChanged}" />
                    <Button
                        Grid.Row="1"
                        Command="{Binding Tab1.ShowMessageCommand}"
                        Content="Show Message" />
                </Grid>
            </TabItem>
            <TabItem Header="Tab 2">
                <Grid>
                    <Grid.RowDefinitions>
                        <RowDefinition Height="Auto" />
                        <RowDefinition Height="Auto" />
                        <RowDefinition Height="Auto" />
                    </Grid.RowDefinitions>
                    <TextBox Grid.Row="0" Text="{Binding Tab2.TextBox1Content, UpdateSourceTrigger=PropertyChanged}" />
                    <TextBox Grid.Row="1" Text="{Binding Tab2.TextBox2Content, UpdateSourceTrigger=PropertyChanged}" />
                    <TextBlock Grid.Row="2" Text="{Binding Tab2.CombinedText}" />
                </Grid>
            </TabItem>
        </TabControl>
    </Grid>
</Window>
```

### MainViewModel.cs

``` cs
public class MainViewModel : BindableBase
{
    public Tab1ViewModel Tab1 { get; }
    public Tab2ViewModel Tab2 { get; }

    public MainViewModel(Tab1ViewModel tab1, Tab2ViewModel tab2)
    {
        Tab1 = tab1;
        Tab2 = tab2;
    }
}
```

### Tab1ViewModel.cs

``` cs
using Prism.Commands;
using Prism.Mvvm;
using System.Windows;

public class Tab1ViewModel : BindableBase
{
    private string _textBoxContent;
    public string TextBoxContent
    {
        get => _textBoxContent;
        set => SetProperty(ref _textBoxContent, value);
    }

    public DelegateCommand ShowMessageCommand { get; }

    public Tab1ViewModel()
    {
        ShowMessageCommand = new DelegateCommand(ShowMessage);
    }

    private void ShowMessage()
    {
        MessageBox.Show(TextBoxContent, "Message from Tab 1");
    }
}
```

### Tab2ViewModel.cs

``` cs
using Prism.Mvvm;

public class Tab2ViewModel : BindableBase
{
    private string _textBox1Content;
    private string _textBox2Content;
    private string _combinedText;

    public string TextBox1Content
    {
        get => _textBox1Content;
        set
        {
            SetProperty(ref _textBox1Content, value);
            CombineText();
        }
    }

    public string TextBox2Content
    {
        get => _textBox2Content;
        set
        {
            SetProperty(ref _textBox2Content, value);
            CombineText();
        }
    }

    public string CombinedText
    {
        get => _combinedText;
        set => SetProperty(ref _combinedText, value);
    }

    private void CombineText()
    {
        CombinedText = $"{TextBox1Content} {TextBox2Content}";
    }
}
```
