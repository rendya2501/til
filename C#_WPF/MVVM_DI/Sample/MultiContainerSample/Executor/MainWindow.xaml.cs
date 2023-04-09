using Common;
using System.Windows;

namespace Executor;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    /// <summary>
    /// IAppInfoを使用したChildViewModelへのインジェクションサンプル
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnLaunchTabProjectClicked(object sender, RoutedEventArgs e)
    {
        const string assemblyName = "TabViewProject";
        const string windowFullName = "TabViewProject.Views.MainWindow";
        WpfExecuter.ExecuteWithAppInfo(assemblyName, windowFullName);
    }

    /// <summary>
    /// CommunityToolKitとPrism.Unityを使ったインジェクションサンプル
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnLaunchToolKitProjectClicked(object sender, RoutedEventArgs e)
    {
        const string assemblyName = "ToolkitFugaProject";
        const string windowFullName = "ToolkitFugaProject.Views.MainWindow";
        WpfExecuter.Execute(assemblyName, windowFullName);
    }
}
