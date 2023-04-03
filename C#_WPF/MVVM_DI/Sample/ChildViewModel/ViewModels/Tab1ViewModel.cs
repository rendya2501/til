using Prism.Commands;
using Prism.Mvvm;
using System.Windows;

namespace ChildViewModel.ViewModels;

public class Tab1ViewModel : BindableBase
{
    private string _textBoxContent;
    public string TextBoxContent
    {
        get => _textBoxContent;
        set => SetProperty(ref _textBoxContent, value);
    }

    public DelegateCommand ShowMessageCommand { get; }

    public Tab1ViewModel()
    {
        ShowMessageCommand = new DelegateCommand(ShowMessage);
    }

    private void ShowMessage()
    {
        MessageBox.Show(TextBoxContent, "Message from Tab 1");
    }
}
