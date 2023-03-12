# Blazor_QuickStart

---

## 環境

- Windows10以降  
- .NET6.0  
- VisualStudioCode  
- blazorserver  
- CommunityToolkit.Mvvm --version 8.1.0  

---

## プロジェクトの構成

``` txt
/ToolkitQuickStart
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
また、CommunityToolkitを使用するとBindableBase等の共通コードも必要なくなるので、Commonも省略。  

---

## プロジェクトの作成

ターミナルから`dotnet new`コマンドを実行する。  
`dotnet new blazorserver -o BlazorQuickStart -f net6.0`  

VSCodeを起動する。  
`code BlazorQuickStart`  

VSCodeのターミナルを起動。  
`ctrl + j`  

---

## ViewModels

`ViewModels`フォルダを作成し、`MainWindowViewModel.cs`を新規作成する。  

`ObservableObject`クラスを継承する。  
`partial class`とする。  
プロパティ名は小文字とする。  

以下のコードをコピペする。  

``` cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ToolkitQuickStart.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(DecrementCommand))]
    public int count;

    [RelayCommand]
    private void Increment() => Count++;

    [RelayCommand(CanExecute = nameof(CanDecrement))]
    private void Decrement() => Count--;

    private bool CanDecrement() => Count > 0;
}
```

---

## Views

`Views`フォルダを作成する。  

`MainWindow.xaml`と`MainWindow.xaml.cs`を`Views`フォルダに移動させる。  

### MainWindow.xaml

`x:Class`を`x:Class="ToolkitQuickStart.MainWindow"`から`x:Class="ToolkitQuickStart.Views.MainWindow"`に変更。  

`xmlns:vm="clr-namespace:ToolkitQuickStart.ViewModels"`を追加。  

`Window.DataContext`プロパティに`vm:MainWindowViewModel`を追加。  

ボタンとテキストブロックを追加する。  

最終形は以下の通り。  

``` xml
<Window x:Class="ToolkitQuickStart.Views.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        xmlns:local="clr-namespace:ToolkitQuickStart"
        xmlns:vm="clr-namespace:ToolkitQuickStart.ViewModels"
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

namespaceを`ToolkitQuickStart`から`ToolkitQuickStart.Views`に変更。  

``` cs
using System.Windows;

namespace ToolkitQuickStart.Views
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
<Application x:Class="ToolkitQuickStart.App"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:local="clr-namespace:ToolkitQuickStart"
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

## 参考

[MVVM ソース ジェネレーター: 二度と MVVM ボイラープレート コードを書く必要はありません。 - YouTube](https://www.youtube.com/watch?v=aCxl0z04BN8)  

[MVVM ツールキットの概要 - .NET Community Toolkit | Microsoft Learn](https://learn.microsoft.com/ja-jp/dotnet/communitytoolkit/mvvm/)  
[CommunityToolkit/MVVM-Samples: Sample repo for MVVM package](https://github.com/CommunityToolkit/MVVM-Samples)  
