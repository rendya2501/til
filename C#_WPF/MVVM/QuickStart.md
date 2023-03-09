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
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WPF_QuickStart.ViewModels;

public class ViewModel : BindableBase
{

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

`MainWindow.xaml`に`xmlns:vm="clr-namespace:WPF_QuickStart.ViewModels"`を追加。  

``` xml
    <Window.DataContext>
        <vm:ViewModel/>
    </Window.DataContext>
```

`dotnet run`  

---

[【C#】WPFでMVVMをフレームワークなしでシンプルに構築する](https://zenn.dev/takuty/articles/b12f4011871058)  
[世界で一番短いサンプルで覚えるMVVM入門 | 趣味や仕事に役立つ初心者DIYプログラミング入門](https://resanaplaza.com/%E4%B8%96%E7%95%8C%E3%81%A7%E4%B8%80%E7%95%AA%E7%9F%AD%E3%81%84%E3%82%B5%E3%83%B3%E3%83%97%E3%83%AB%E3%81%A7%E8%A6%9A%E3%81%88%E3%82%8Bmvvm%E5%85%A5%E9%96%80/)  
