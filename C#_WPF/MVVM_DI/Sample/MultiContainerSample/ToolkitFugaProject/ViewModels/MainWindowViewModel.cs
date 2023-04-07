using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using ToolkitFugaProject.Common;
using ToolkitFugaProject.Service;

namespace ToolkitFugaProject.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(DecrementCommand))]
    public int count;

    [ObservableProperty]
    public List<StatusEnum> statuses;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Result))]
    public StatusEnum selectedStatus;

    public string Result => $"This is the selected Status: {SelectedStatus}";

    [RelayCommand]
    private void Increment() => Count++;

    [RelayCommand(CanExecute = nameof(CanDecrement))]
    private void Decrement() => Count--;
    private bool CanDecrement() => Count > 0;

    [Unity.Dependency]
    public FugaService FugaService { get; set; }

    [RelayCommand]
    private void ShowFuga() => MessageBox.Show(FugaService.FugaContent, "Message from ShowFugaCommand");

    public MainWindowViewModel()
    {
        statuses = new()
        {
            StatusEnum.Waiting,
            StatusEnum.Running,
            StatusEnum.Paused,
            StatusEnum.Success,
            StatusEnum.Failure
        };
        SelectedStatus = Statuses.First();
    }
}