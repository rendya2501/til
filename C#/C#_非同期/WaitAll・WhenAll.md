# WaitAll・WhenAll

## Task.WaitAllはメインスレッドをブロックするか検証

ふと気になって調べて見た。  

■ **AIの回答**

Task.WaitAll メソッドはメインスレッドをブロックします。  
Task.WaitAll メソッドは、指定された複数のタスクが完了するまでブロックされます。  
これにより、処理の完了を待つ必要がある場合に非同期タスクを同期的に待つことができますが、メインスレッドをブロックするため、UIスレッドなどの場合は注意が必要です。  
そのため、メインスレッドをブロックせずに非同期タスクを待つためには、Task.WhenAll メソッドを使用することができます。  
Task.WhenAll メソッドは、全てのタスクが完了するまでブロックすることなく、全てのタスクが完了した後に次の処理を継続することができます。  

■ **検証結果**

WaitAllを実行した場合、ローディングのクルクルが止まることが確認できる。  
これにより、WaitAllはメインスレッド・UIスレッドをブロックすることが分かった。  

■ **実行環境**

- Windows10  
- .Net6  
- WPF  

`dotnet new wpf -o WaitAllWhenAllDemo -f net6.0`  

■ **実装**

ボタン2とラベルとプログレスバーを配置する。  
プログレスバーはローディングアニメーションが常に走っている状態となる。  

```xml
<Window x:Class="WaitAllWhenAllDemo.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        xmlns:local="clr-namespace:WaitAllWhenAllDemo"
        mc:Ignorable="d"
        Title="MainWindow" Height="450" Width="800">
    <Grid>
        <StackPanel HorizontalAlignment="Center" VerticalAlignment="Center">
            <Button x:Name="waitAllButton" Content="Task.WaitAll" Click="waitAllButton_Click"/>
            <Button x:Name="whenAllButton" Content="Task.WhenAll" Click="whenAllButton_Click"/>
            <StackPanel x:Name="spinnerPanel" HorizontalAlignment="Center" VerticalAlignment="Center" Visibility="Collapsed">
                <TextBlock Text="Waiting for tasks to complete..." Margin="0,0,0,10" />
                <ProgressBar x:Name="progressBar" IsIndeterminate="True" Width="150" Margin="10" Grid.Row="0"/>
            </StackPanel>
        </StackPanel>
    </Grid>
</Window>
```

ボタンそれぞれにWaitAllとWhenAllを記述して動作を確認する。  

``` cs
using System;
using System.Threading.Tasks;
using System.Windows;

namespace WaitAllWhenAllDemo;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private async void waitAllButton_Click(object sender, RoutedEventArgs e)
    {
        // Start the spinner animation
        spinnerPanel.Visibility = Visibility.Visible;
        await Task.Delay(100); // Wait a bit to allow the UI to update

        // Start the tasks
        var tasks = new[]
        {
            Task.Run(() => DummyTask(1)),
            Task.Run(() => DummyTask(2))
        };

        // Wait for all tasks to complete
        Task.WaitAll(tasks);

        // Stop the spinner animation
        spinnerPanel.Visibility = Visibility.Collapsed;
    }

    private async void whenAllButton_Click(object sender, RoutedEventArgs e)
    {
        // Start the spinner animation
        spinnerPanel.Visibility = Visibility.Visible;
        await Task.Delay(100); // Wait a bit to allow the UI to update

        // Start the tasks
        var tasks = new[]
        {
            Task.Run(() => DummyTask(1)),
            Task.Run(() => DummyTask(2))
        };

        // Wait for all tasks to complete
        await Task.WhenAll(tasks);

        // Stop the spinner animation
        spinnerPanel.Visibility = Visibility.Collapsed;
    }

    private void DummyTask(int id)
    {
        // Simulate a long-running task
        Console.WriteLine($"Starting task {id}");
        Task.Delay(5000).Wait();
        Console.WriteLine($"Finished task {id}");
    }
}
```

---

## Task.WaitAll and Exceptions

ASP.Net側のTask.WaitAllで実行しているタスクの中でエラーが発生した場合、エラーが`AggregateException`にラッピングされ、単純なメッセージだけが表示されない現象が発生した。  

下記例であれば`Oops`とエラーダイアログで表示されるのだが、`●●EXCEPTION(Oops)`みたいな感じで表示されてしまう。  
単純にTRY CATCHしてInnerExceptionをThrowしなおせば解決はした。  

``` C#
    Task t1 = Task.Factory.StartNew(() => Task.Delay(1000));
    Task t2 = Task.Factory.StartNew(() => {
        Task.Delay(500);
        throw new Exception("Oops");
    });

    try
    {
        Task.WaitAll(t1, t2);
        System.Diagnostics.Debug.WriteLine("All done");
    }
    catch (AggregateException e)
    {
        System.Diagnostics.Debug.WriteLine("Something went wrong");
        throw e.InnerException;
    }
```

[Task.WaitAll and Exceptions](https://stackoverflow.com/questions/4217643/task-waitall-and-exceptions)  
