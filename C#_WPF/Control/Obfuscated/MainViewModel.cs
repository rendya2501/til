using System.ComponentModel;

namespace Obfuscated;

internal class MainViewModel : INotifyPropertyChanged
{
    private string _inputText;
    public string InputText
    {
        get { return _inputText; }
        set
        {
            _inputText = value;
            OnPropertyChanged(nameof(InputText));
        }
    }

    private bool _isObfuscatedMode;
    public bool IsObfuscatedMode
    {
        get { return _isObfuscatedMode; }
        set
        {
            _isObfuscatedMode = value;
            OnPropertyChanged(nameof(IsObfuscatedMode));
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
