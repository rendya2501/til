# QuickStart

## 環境

- Windows10以降  
- .NET6.0  
- Visual Studio Community 2022  
- WPF  

---

`dotnet new wpf -o WPF_QuickStart -f net6.0`  
`code WPF_QuickStart`  

---

## ViewModels

`ViewModels`フォルダを作成し、`ViewModel.cs`を新規作成する。  
以下のコードをコピペする。  

``` cs
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace WPF_QuickStart.ViewModels;

public class ViewModel : BindableBase
{
    //コマンド
    public ICommand Button1_Pushed { get; set; }

    public ViewModel()
    {
        //コマンド
        Button1_Pushed = new RelayCommand(Button1_Command);
    }

    //コマンドの処理内容
    private void Button1_Command()
    {
        //カウント数を増やす
        Count++;
    }

    //変数(カウント数)
    private int _Count;
    public int Count
    {
        get { return _Count; }
        set { SetProperty(ref _Count, value); }
    }
}

/// <summary>
/// INotifyPropertyChangedのヘルパークラス
/// </summary>
public class BindableBase : INotifyPropertyChanged
{
    /// <summary>
    /// INotifyPropertyChangedインターフェース実装イベント
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// 値のセットと通知を行う
    /// </summary>
    protected virtual bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
        // 同じ値なら処理しない
        if (Equals(field, value))
        {
            return false;
        }
        // 値を反映
        field = value;
        // プロパティ発火
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        // 正常に終了したことを通知
        return true;
    }
}

class RelayCommand : ICommand
{
    private readonly Action _execute;
    private readonly Func<bool> _canExecute;

    /// <summary>
    /// RaiseCanExecuteChanged が呼び出されたときに生成されます。
    /// </summary>
    public event EventHandler CanExecuteChanged;

    /// <summary>
    /// 常に実行可能な新しいコマンドを作成します。
    /// </summary>
    /// <param name="execute">実行ロジック。</param>
    public RelayCommand(Action execute)
        : this(execute, null)
    {
    }

    /// <summary>
    /// 新しいコマンドを作成します。
    /// </summary>
    /// <param name="execute">実行ロジック。</param>
    /// <param name="canExecute">実行ステータス ロジック。</param>
    public RelayCommand(Action execute, Func<bool> canExecute)
    {
        if (execute == null)
            throw new ArgumentNullException("execute");
        _execute = execute;
        _canExecute = canExecute;
    }


    /// <summary>
    /// 現在の状態でこの <see cref="RelayCommand"/> が実行できるかどうかを判定します。
    /// </summary>
    /// <param name="parameter">
    /// コマンドによって使用されるデータ。コマンドが、データの引き渡しを必要としない場合、このオブジェクトを null に設定できます。
    /// </param>
    /// <returns>このコマンドが実行可能な場合は true、それ以外の場合は false。</returns>
    public bool CanExecute(object parameter)
    {
        return _canExecute == null ? true : _canExecute();
    }

    /// <summary>
    /// 現在のコマンド ターゲットに対して <see cref="RelayCommand"/> を実行します。
    /// </summary>
    /// <param name="parameter">
    /// コマンドによって使用されるデータ。コマンドが、データの引き渡しを必要としない場合、このオブジェクトを null に設定できます。
    /// </param>
    public void Execute(object parameter)
    {
        _execute();
    }

    public void RaiseCanExcuteChanged()
    {
        var handler = CanExecuteChanged;
        if (handler != null)
        {
            handler(this, EventArgs.Empty);
        }
    }
}
```

## Views

`Views`フォルダを作成する。  

`MainWindow.xaml`と`MainWindow.xaml.cs`を`Views`フォルダに移動させる。  

`App.xaml`の`StartupUri`を`MainWindow.xaml`から`Views/MainWindow.xaml`に変更する。  

``` xml
<Application x:Class="WPF_QuickStart.App"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:local="clr-namespace:WPF_QuickStart"
             StartupUri="Views/MainWindow.xaml">
    <Application.Resources>
         
    </Application.Resources>
</Application>
```

`MainWindow.xaml`  

`x:Class="WPF_QuickStart.MainWindow"`から`x:Class="WPF_QuickStart.Views.MainWindow"`に変更。  
`xmlns:vm="clr-namespace:WPF_QuickStart.ViewModels"`を追加。  
`Window.DataContext`プロパティに`vm:ViewModel`を追加。  

``` xml
<Window
    x:Class="WPF_QuickStart.Views.MainWindow"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
    xmlns:local="clr-namespace:WPF_QuickStart"
    xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
    xmlns:vm="clr-namespace:WPF_QuickStart.ViewModels"
    Title="MainWindow"
    Width="800"
    Height="450"
    mc:Ignorable="d">
    <Window.DataContext>
        <vm:ViewModel />
    </Window.DataContext>
    <Grid>
        <StackPanel Orientation="Vertical" >
            <Button Content="Button1" Command="{Binding Button1_Pushed}" />
            <TextBlock Text="{Binding Count}" />
        </StackPanel>
    </Grid>
</Window>
```

`MainWindow.xaml.cs`のnamespaceを`WPF_QuickStart`から`WPF_QuickStart.Views`に変更。  

``` cs
using System.Windows;

namespace WPF_QuickStart.Views
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

`dotnet run`  

---

[【C#】MVVMプロジェクトの作り方【初心者向け】 | すぽてく](https://shinshin-log.com/mvvm-project/)  

[【C#】WPFでMVVMをフレームワークなしでシンプルに構築する](https://zenn.dev/takuty/articles/b12f4011871058)  
[世界で一番短いサンプルで覚えるMVVM入門 | 趣味や仕事に役立つ初心者DIYプログラミング入門](https://resanaplaza.com/%E4%B8%96%E7%95%8C%E3%81%A7%E4%B8%80%E7%95%AA%E7%9F%AD%E3%81%84%E3%82%B5%E3%83%B3%E3%83%97%E3%83%AB%E3%81%A7%E8%A6%9A%E3%81%88%E3%82%8Bmvvm%E5%85%A5%E9%96%80/)  
