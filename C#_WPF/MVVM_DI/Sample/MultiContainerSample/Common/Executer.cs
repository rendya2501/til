using Prism.Unity;
using System.Reflection;
using System.Windows;
using Unity;

namespace Common;

/// <summary>
/// WPFアプリケーションの実行をサポートするクラスです。
/// </summary>
public class WpfExecuter
{
    private static readonly AssemblyManager _assemblyManager = new();

    /// <summary>
    /// IAppInfoを使用してアセンブリをロードし、指定されたウィンドウを表示します。
    /// </summary>
    /// <param name="assemblyName">アセンブリ名</param>
    /// <param name="windowFullName">表示するウィンドウの完全名</param>
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

    /// <summary>
    /// 指定されたウィンドウを表示します。
    /// </summary>
    /// <param name="assemblyName">アセンブリ名</param>
    /// <param name="windowFullName">表示するウィンドウの完全名</param>
    public static void Execute(string assemblyName, string windowFullName)
    {
        var windowInstanceManager = new WindowInstanceManager(assemblyName, windowFullName);
        windowInstanceManager.ShowWindowInstance();
    }
}


/// <summary>
/// アセンブリのロードと登録を管理するクラスです。
/// </summary>
public class AssemblyManager
{
    private readonly IList<string> _loadedAssemblyList = new List<string>();

    /// <summary>
    /// アセンブリが既にロードされているかどうかを判断します。
    /// </summary>
    /// <param name="assemblyName">アセンブリ名</param>
    /// <returns>アセンブリがロードされている場合はtrue、そうでない場合はfalse</returns>
    public bool IsAssemblyLoaded(string assemblyName)
    {
        return _loadedAssemblyList.Contains(assemblyName);
    }

    /// <summary>
    /// アセンブリをロードし、型を登録します。
    /// </summary>
    /// <param name="assemblyName">アセンブリ名</param>
    public void LoadAssemblyAndRegisterTypes(string assemblyName)
    {
        Assembly assembly = Assembly.Load(new AssemblyName(assemblyName));
        RegisterTypes(assembly, ((PrismApplication)Application.Current).Container.GetContainer());
        _loadedAssemblyList.Add(assemblyName);
    }

    /// <summary>
    /// アセンブリ内の型を登録します。
    /// </summary>
    /// <param name="assembly">登録する型が含まれるアセンブリ</param>
    /// <param name="unityContainer">型を登録するためのUnityコンテナ</param>
    private static void RegisterTypes(Assembly assembly, IUnityContainer unityContainer)
    {
        IAppInfo appInfo = assembly.CreateInstance(assembly.GetName().Name + ".AppInfo") as IAppInfo ?? throw new NullReferenceException("AppInfo is not found.");
        appInfo.RegisterTypes(unityContainer);
        unityContainer.RegisterInstance(appInfo);
    }
}

/// <summary>
/// ウィンドウインスタンスを管理するクラスです。
/// </summary>
public class WindowInstanceManager
{
    private readonly string _assemblyName;
    private readonly string _windowFullName;

    /// <summary>
    /// ウィンドウインスタンスマネージャを初期化します。
    /// </summary>
    /// <param name="assemblyName">アセンブリ名</param>
    /// <param name="windowFullName">表示するウィンドウの完全名</param>
    public WindowInstanceManager(string assemblyName, string windowFullName)
    {
        _assemblyName = assemblyName;
        _windowFullName = windowFullName;
    }

    /// <summary>
    /// ウィンドウインスタンスを表示します。
    /// </summary>
    public void ShowWindowInstance()
    {
        Window windowInstance = CreateWindowInstance();
        windowInstance.Show();
    }

    /// <summary>
    /// ウィンドウインスタンスを作成します。
    /// </summary>
    /// <returns>作成されたウィンドウインスタンス</returns>
    private Window CreateWindowInstance()
    {
        Type windowType = GetWindowType() ?? throw new NullReferenceException($"Window not found: {_windowFullName}");
        return Activator.CreateInstance(windowType) as Window;
    }

    /// <summary>
    /// 指定されたウィンドウの型を取得します。
    /// </summary>
    /// <returns>ウィンドウの型、存在しない場合はnull</returns>
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
