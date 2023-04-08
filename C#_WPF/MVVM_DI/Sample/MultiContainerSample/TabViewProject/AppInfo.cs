using Common;
using System.Windows;
using TabViewProject.Service;
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
        containerRegistry.RegisterType<HogeService>();
    }

    public Window CreateMainWindow(IUnityContainer container)
    {
        var viewModel = container.Resolve<MainWindowViewModel>();
        viewModel.HogeService = container.Resolve<HogeService>();
        return new MainWindow { DataContext = viewModel };
    }
}
