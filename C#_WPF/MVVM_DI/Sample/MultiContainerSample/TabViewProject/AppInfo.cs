using Common;
using System.Windows;
using TabViewProject.ViewModels;
using TabViewProject.Views;
using Unity;

namespace TabViewProject;

public class AppInfo : IAppInfo
{
    public void RegisterTypes(IUnityContainer containerRegistry)
    {
        containerRegistry.RegisterType<MainWindowViewModel>();
        containerRegistry.RegisterType<Tab1ViewModel>();
        containerRegistry.RegisterType<Tab2ViewModel>();
    }

    public Window CreateMainWindow(IUnityContainer container)
    {
        var viewModel = container.Resolve<MainWindowViewModel>();
        return new MainWindow { DataContext = viewModel };
    }
}
