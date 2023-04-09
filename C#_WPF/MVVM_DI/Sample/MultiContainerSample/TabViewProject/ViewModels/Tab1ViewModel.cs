using Common;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System.Diagnostics;
using System.Windows;

namespace TabViewProject.ViewModels;

public class Tab1ViewModel : BindableBase
{
    private string _textBoxContent;
    public string TextBoxContent
    {
        get => _textBoxContent;
        set => SetProperty(ref _textBoxContent, value);
    }

    public DelegateCommand ShowMessageCommand { get; }

    public DelegateCommand ShowToolKitCommand { get; }


    private readonly IEventAggregator _eventAggregator;


    public Tab1ViewModel(IEventAggregator eventAggregator)
    {
        ShowMessageCommand = new DelegateCommand(ShowMessage);
        ShowToolKitCommand = new DelegateCommand(ShowToolKit);

        _eventAggregator = eventAggregator;
        _eventAggregator.GetEvent<PubSubEvent<string>>().Subscribe(OnDataReceived);
    }

    private void ShowMessage()
    {
        MessageBox.Show(TextBoxContent, "Message from Tab 1");
    }

    private void ShowToolKit()
    {
        const string assemblyName = "ToolkitFugaProject";
        const string windowFullName = "ToolkitFugaProject.Views.MainWindow";
        WpfExecuter.Execute(assemblyName, windowFullName);

        string dataToSend = "Sample data";
        _eventAggregator.GetEvent<PubSubEvent<string>>().Publish(dataToSend);
    }


    private void OnDataReceived(string data)
    {
        Debug.WriteLine(data);
    }
}
