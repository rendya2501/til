using Prism.Mvvm;

namespace ChildViewModel.ViewModels;

public class MainWindowViewModel : BindableBase
{
    public Tab1ViewModel Tab1 { get; }
    public Tab2ViewModel Tab2 { get; }

    public MainWindowViewModel(Tab1ViewModel tab1, Tab2ViewModel tab2)
    {
        Tab1 = tab1;
        Tab2 = tab2;
    }
}
