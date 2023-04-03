using ChildViewModel.ViewModels;
using ChildViewModel.Views;
using Prism.Ioc;
using Prism.Mvvm;
using System.Windows;

namespace ChildViewModel;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    protected override Window CreateShell()
    {
        return Container.Resolve<MainWindow>();
    }

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.Register<Tab1ViewModel>();
        containerRegistry.Register<Tab2ViewModel>();
        containerRegistry.Register<MainWindowViewModel>();
    }

    protected override void ConfigureViewModelLocator()
    {
        base.ConfigureViewModelLocator();
        ViewModelLocationProvider.Register<MainWindow, MainWindowViewModel>();
    }
}
