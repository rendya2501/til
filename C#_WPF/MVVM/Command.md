# Command

## 引数があるCommandを実行する

ItemsControlでその行番号を引数として受け取り、その行の項目に対して操作を行いたい時があったので調べた。  

```xml
<TextBox 
    Height="30" Width="80" Margin="5,5,5,5"
    Text="{Binding Message, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}"
/>
<Button 
    Height="30" Width="80"
    Content="表示"
    Command="{Binding ShowMessageCommand}" CommandParameter="{Binding Message}"
/>
```

```cs
namespace sample
{
    public class MainWindowViewModel : ViewModelsBase
    {
        private string _message;
        /// <summary>
        /// 画面のテキストボックスとバインディング
        /// </summary>
        public string Message
        {
            get { return this._message; }
            set
            {
                this._message = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainWindowViewModel()
        {
            this.ShowMessageCommand = new DelegateCommand<string>(ShowMessage);
        }

        public ICommand ShowMessageCommand { get; private set; }

        private void ShowMessage(string message)
        {
            MessageBox.Show(message);
        }
    }
}
```

[MVVMで引数があるCommandを実装する](https://yoshinorin.net/articles/2016/05/21/MVVM-command-hasArgument/)  
