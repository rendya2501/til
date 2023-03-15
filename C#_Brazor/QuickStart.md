# Blazor QuickStart

## 環境

- Windows10以降  
- .NET6.0  
- VisualStudioCode  
- Blazor WebAssembly  

---

## プロジェクトの作成

ターミナルから`dotnet new`コマンドを実行する。  
`dotnet new blazorwasm -o BlazorQuickStart -f net6.0`  

VSCodeを起動する。  
`code BlazorQuickStart`  

---

## Component

PagesフォルダのCounter.razorをそのまま使わせてもらう。  
以下のようにコーディングすることでWPFのクイックスタートと同じサンプルを実現することができる。  

正直簡単過ぎてクイックスタートとしてまとめる意味を感じない。  
直感的なのも相まって、まとめるまでもなく、すぐにわかるだろうって思ってしまう。  

``` html
@page "/counter"

<PageTitle>Counter</PageTitle>

<h1>Counter</h1>

<p role="status">Current count: @currentCount</p>

<button class="btn btn-primary" @onclick="IncrementCount">Increment</button>
<button class="btn btn-primary" disabled="@IsDisabled" @onclick="DecrementCount">Decrement</button>

@code {
    private int currentCount = 0;
    private bool IsDisabled => currentCount <= 0;

    private void IncrementCount() => currentCount++;
    private void DecrementCount() => currentCount--;
}
```

---

## プロジェクトの起動

VSCodeのターミナルで起動コマンドを実行する。  
`dotnet run`  

---

## WPFで同じことをやった場合

CommunityToolkitを使った場合は以下のように記述する必要がある。  
色々端折ってこれなので、簡潔さに関してはBlazorに遠く及ばない。  

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

---

## 参考

[html - How to enable/disable inputs in blazor - Stack Overflow](https://stackoverflow.com/questions/55002514/how-to-enable-disable-inputs-in-blazor)  
[How are input components enabled/disabled in Blazor?](https://www.syncfusion.com/faq/blazor/forms-and-validation/how-are-input-components-enabled-disabled-in-blazor)  
