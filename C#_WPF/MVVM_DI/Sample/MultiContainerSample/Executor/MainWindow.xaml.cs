using Common;
using Prism.Unity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;
using Unity;

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


public class WpfExecuter
{
    private static readonly AssemblyManager _assemblyManager = new();

    public static void ExecuteWithAppInfo(string assemblyName, string windowFullName)
    {
        if (!_assemblyManager.IsAssemblyLoaded(assemblyName))
        {
            _assemblyManager.LoadAssemblyAndRegisterTypes(assemblyName);
        }

        var unityContainer = ((PrismApplication)Application.Current).Container.GetContainer();
        var appInfo = unityContainer.Resolve<IAppInfo>();
        var mainWindow = appInfo.CreateMainWindow(unityContainer);
        mainWindow.Show();
    }

    public static void Execute(string assemblyName, string windowFullName)
    {
        var windowInstanceManager = new WindowInstanceManager(assemblyName, windowFullName);
        windowInstanceManager.ShowWindowInstance();
    }
}


public class AssemblyManager
{
    private readonly IList<string> _loadedAssemblyList = new List<string>();

    public bool IsAssemblyLoaded(string assemblyName)
    {
        return _loadedAssemblyList.Contains(assemblyName);
    }

    public void LoadAssemblyAndRegisterTypes(string assemblyName)
    {
        Assembly assembly = Assembly.Load(new AssemblyName(assemblyName));
        RegisterTypes(assembly, ((PrismApplication)Application.Current).Container.GetContainer());
        _loadedAssemblyList.Add(assemblyName);
    }

    private static void RegisterTypes(Assembly assembly, IUnityContainer unityContainer)
    {
        IAppInfo appInfo = assembly.CreateInstance(assembly.GetName().Name + ".AppInfo") as IAppInfo ?? throw new NullReferenceException("AppInfo is not found.");
        appInfo.RegisterTypes(unityContainer);
        unityContainer.RegisterInstance(appInfo);
    }
}


public class WindowInstanceManager
{
    private readonly string _assemblyName;
    private readonly string _windowFullName;

    public WindowInstanceManager(string assemblyName, string windowFullName)
    {
        _assemblyName = assemblyName;
        _windowFullName = windowFullName;
    }

    public void ShowWindowInstance()
    {
        Window windowInstance = CreateWindowInstance();
        windowInstance.Show();
    }

    private Window CreateWindowInstance()
    {
        Type windowType = GetWindowType() ?? throw new NullReferenceException($"Window not found: {_windowFullName}");
        return Activator.CreateInstance(windowType) as Window;
    }

    public Type? GetWindowType()
    {
        string currentLocation = AppDomain.CurrentDomain.BaseDirectory;
        string dllAssemblyFullPath = Path.Combine(currentLocation, _assemblyName + ".dll");

        if (File.Exists(dllAssemblyFullPath))
        {
            return Assembly.LoadFrom(dllAssemblyFullPath).GetType(_windowFullName);
        }

        return null;
    }
}
