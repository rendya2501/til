using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Dispatch;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private async void TestButtonWithInvoke_Click(object sender, RoutedEventArgs e)
    {
        TestButtonWithInvoke.IsEnabled = false;
        TestButtonNoInvoke.IsEnabled = false;

        await Task.Run(() =>
        {
            for (int i = 1; i <= 5; i++)
            {
                Thread.Sleep(1000); // Simulate work
                
                Application.Current.Dispatcher.Invoke(() =>
                {
                    OutputTextBox.Text = $"Count: {i} (With Invoke)";
                });
            }
        });

        TestButtonWithInvoke.IsEnabled = true;
        TestButtonNoInvoke.IsEnabled = true;
    }

    private async void TestButtonNoInvoke_Click(object sender, RoutedEventArgs e)
    {
        TestButtonWithInvoke.IsEnabled = false;
        TestButtonNoInvoke.IsEnabled = false;

        int count = 0;
        await Task.Run(async () =>
        {
            for (int i = 1; i <= 5; i++)
            {
                Thread.Sleep(1000); // Simulate work
                OutputTextBox.Text = $"Count: {i} (No Invoke)";
            }
        });

        // OutputTextBox.Text = $"Count: {count} (No Invoke)";

        TestButtonWithInvoke.IsEnabled = true;
        TestButtonNoInvoke.IsEnabled = true;
    }
}
