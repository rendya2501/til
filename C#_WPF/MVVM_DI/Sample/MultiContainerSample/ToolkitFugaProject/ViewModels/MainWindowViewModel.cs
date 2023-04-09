using Common;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Livet.Messaging;
using Livet.Messaging.Windows;
using Prism.Commands;
using Prism.Events;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ToolkitFugaProject.Common;

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
    public FugaViewModel FugaViewModel { get; init; }

    [RelayCommand]
    public void ShowTabView()
    {
        const string assemblyName = "TabViewProject";
        const string windowFullName = "TabViewProject.Views.MainWindow";
        WpfExecuter.ExecuteWithAppInfo(assemblyName, windowFullName);
    }

    private IEventAggregator _eventAggregator { get; init; }

    public DelegateCommand CloseCommand { get; }

    [RelayCommand]
    public void CloseWindow()
    {
        string aaa = "aaa";
        _eventAggregator.GetEvent<PubSubEvent<string>>().Publish(aaa);
        CloseCommand.Execute();
    }

    public InteractionMessenger Messenger { get; } = new InteractionMessenger();


    public MainWindowViewModel(IEventAggregator eventAggregator)
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

        CloseCommand = new DelegateCommand(() => Messenger.Raise(new WindowActionMessage(WindowAction.Close, "WindowAction")));

        this._eventAggregator = eventAggregator;
        _eventAggregator.GetEvent<PubSubEvent<string>>().Subscribe(OnDataReceived);
    }


    private void OnDataReceived(string data)
    {
        Debug.WriteLine(data);
    }
}