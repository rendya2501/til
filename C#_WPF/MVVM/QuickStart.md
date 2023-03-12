# QuickStart

## 目的

プレーンなWPFをMVVMでサクッと動かすためのサンプル。  
加算ボタン、減算ボタン、カウントの表示だけを行うシンプルな構成。  
開発はVSCodeで行う。  

~~VSCodeでの開発では、XAMLの編集ヒントが全く出ないのでガリガリやっていくのには向かないが、VisualStudioなんて立ち上げていたら重くてたまらないので致し方なし。~~  

---

## 環境

- Windows10以降  
- .NET6.0  
- VisualStudioCode  
- WPF  

---

## プロジェクトの構成

``` txt
/WPF_QuickStart
    /Common
        BindableBase.cs
        RelayCommand.cs
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

今回はビジネスロジックはないためModelは省略  

---

## プロジェクトの作成

ターミナルから`dotnet new`コマンドを実行する。  
`dotnet new wpf -o WPF_QuickStart -f net6.0`  

VSCodeを起動する。  
`code WPF_QuickStart`  

---

## Common

`Common`フォルダを作成し、`BindableBase.cs`と`RelayCommand.cs`を新規作成する。  
以下のコードをそれぞれコピペする。  

### BindableBase

``` cs
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WPF_QuickStart.Common;

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
```

### RelayCommand

``` cs
using System;
using System.Windows.Input;

namespace WPF_QuickStart.Common;

public class RelayCommand : ICommand
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

    /// <summary>
    /// <see cref="CanExecuteChanged"/> イベントを発生させるために使用されるメソッド
    /// <see cref="CanExecute"/> の戻り値を表すために
    /// メソッドが変更されました。
    /// </summary>
    public void RaiseCanExecuteChanged()
    {
        var handler = CanExecuteChanged;
        if (handler != null)
        {
            handler(this, EventArgs.Empty);
        }
    }
}
```

---

## ViewModels

`ViewModels`フォルダを作成し、`MainWindowViewModel.cs`を新規作成する。  
以下のコードをコピペする。  

``` cs
using System.Windows.Input;
using WPF_QuickStart.Common;

namespace WPF_QuickStart.ViewModels;

public class MainWindowViewModel : BindableBase
{
    /// <summary>
    /// インクリメントコマンド
    /// </summary>
    /// <value></value>
    public RelayCommand IncrementCommand { get; set; }

    /// <summary>
    /// デクリメントコマンド
    /// </summary>
    /// <value></value>
    public RelayCommand DecrementCommand { get; set; }

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
        IncrementCommand = new RelayCommand(Increment);
        DecrementCommand = new RelayCommand(Decrement,() => Count > 0);
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

`x:Class`を`x:Class="WPF_QuickStart.MainWindow"`から`x:Class="WPF_QuickStart.Views.MainWindow"`に変更。  

`xmlns:vm="clr-namespace:WPF_QuickStart.ViewModels"`を追加。  

`Window.DataContext`プロパティに`vm:MainWindowViewModel`を追加。  

ボタンとテキストブロックを追加する。  

最終形は以下の通り。  

``` xml
<Window x:Class="WPF_QuickStart.Views.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        xmlns:local="clr-namespace:WPF_QuickStart"
        xmlns:vm="clr-namespace:WPF_QuickStart.ViewModels"
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

namespaceを`WPF_QuickStart`から`WPF_QuickStart.Views`に変更。  

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

### App.xaml

`StartupUri`を`MainWindow.xaml`から`Views/MainWindow.xaml`に変更する。  

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

---

## プロジェクトの起動

VSCodeのターミナルを起動。  
`ctrl + j`  

ターミナルでプロジェクト起動コマンドを実行する。  
`dotnet run`  

---

## 参考

ここが一番よくまとまっている。  
[【C#】MVVMプロジェクトの作り方【初心者向け】 | すぽてく](https://shinshin-log.com/mvvm-project/)  

参考になるが、少し凝りすぎてうるさい。  
[【C#】WPFでMVVMをフレームワークなしでシンプルに構築する](https://zenn.dev/takuty/articles/b12f4011871058)  
[世界で一番短いサンプルで覚えるMVVM入門 | 趣味や仕事に役立つ初心者DIYプログラミング入門](https://resanaplaza.com/%E4%B8%96%E7%95%8C%E3%81%A7%E4%B8%80%E7%95%AA%E7%9F%AD%E3%81%84%E3%82%B5%E3%83%B3%E3%83%97%E3%83%AB%E3%81%A7%E8%A6%9A%E3%81%88%E3%82%8Bmvvm%E5%85%A5%E9%96%80/)  

MainWindowを移動させたときに変更が必要
[WPFでMainWindow.xamlのフォルダを変更する](https://www.paveway.info/entry/2019/07/01/wpf_startupuri)  
