using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using ToolkitFugaProject.Service;
using Unity;

namespace ToolkitFugaProject.ViewModels;

public partial class FugaViewModel : ObservableObject
{
    [Dependency]
    public FugaService FugaService { get; init; }

    [RelayCommand]
    private void ShowFuga() => MessageBox.Show(FugaService.FugaContent, "Message from ShowFugaCommand");
}
