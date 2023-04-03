using Prism.Mvvm;

namespace ChildViewModel.ViewModels;

public class Tab2ViewModel : BindableBase
{
    private string _textBox1Content;
    private string _textBox2Content;
    private string _combinedText;

    public string TextBox1Content
    {
        get => _textBox1Content;
        set
        {
            SetProperty(ref _textBox1Content, value);
            CombineText();
        }
    }

    public string TextBox2Content
    {
        get => _textBox2Content;
        set
        {
            SetProperty(ref _textBox2Content, value);
            CombineText();
        }
    }

    public string CombinedText
    {
        get => _combinedText;
        set => SetProperty(ref _combinedText, value);
    }

    private void CombineText()
    {
        CombinedText = $"{TextBox1Content} {TextBox2Content}";
    }
}