# Prismを用いたMVVMサンプル1

<https://blog.okazuki.jp/entry/2014/12/23/180413>

RNに入る前のサンプルとしては最適だったかもしれないが、先にRNで組んでしまってからだと、当たり前のことしか言っていない。
なんてことはない。改めてサンプルとしてまとめる必要性も皆無だ。
でも、自分で説明するとなったらあれなんだよなぁ。

### MVVMパターンとは？

プログラムの構成をView(見た目),ViewModel(見た目とデータの制御),Model(データの制御)に分けた物。
基本的に、View,ViewModel間のやり取りはプロパティのBindingを行う。
プロパティ変更通知はINotifyPropertyChangedインターフェースを使用するが、Prismは便利なサポート

```C# : ViewModel
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;

namespace MVVMSample1
{
    public class MainWindowViewModel : BindableBase
    {
        #region プロパティ
        private string input;
        /// <summary>
        /// 入力値
        /// </summary>
        public string Input
        {
            get { return this.input; }
            set
            {
                this.SetProperty(ref this.input, value);
                // 入力値に変かがある度にコマンドのCanExecuteの状態が変わったことを通知する
                this.ConvertCommand.RaiseCanExecuteChanged();
            }
        }

        private string output;
        /// <summary>
        /// 出力値
        /// </summary>
        public string Output
        {
            get { return this.output; }
            set { this.SetProperty(ref this.output, value); }
        }
        /// <summary>
        /// 変換コマンド
        /// </summary>
        public DelegateCommand ConvertCommand { get; private set; }
        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainWindowViewModel()
        {
            this.ConvertCommand = new DelegateCommand(
                this.ConvertExecute,
                this.CanConvertExecute
            );
        }
        /// <summary>
        /// 大文字に変換
        /// </summary>
        private void ConvertExecute()
        {
            this.Output = this.Input.ToUpper();
        }
        /// <summary>
        /// 何か入力されてたら実行可能
        /// </summary>
        /// <returns></returns>
        private bool CanConvertExecute()
        {
            return !string.IsNullOrWhiteSpace(this.Input);
        }
    }
}
```

ビルドしてView（XAML）を作成します。ViewModelをXAMLで参照できるように名前空間の定義を行います。

```C# : Xaml
xmlns:l="clr-namespace:MVVMSample01"
```

そして、DataContextプロパティに先ほど作成したViewModelクラスを設定します。

```C# : Xaml
<Window.DataContext>
    <l:MainWindowViewModel />
</Window.DataContext>
```

画面を作成していきます。入力用のTextBoxと出力用のTextBlockとコマンドを実行するためのButtonを置いて、ViewModelの対応するプロパティとバインディングしています。

```C# : Xaml
<Window
    x:Class="MVVMSample01.MainWindow"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:l="clr-namespace:MVVMSample01"
    Title="MainWindow" Height="350" Width="525">
    <Window.DataContext>
        <l:MainWindowViewModel />
    </Window.DataContext>
    <StackPanel>
        <TextBox Text="{Binding Input, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}" />
        <Button Content="Convert" Command="{Binding ConvertCommand}" />
        <TextBlock Text="{Binding Output}" />
    </StackPanel>
</Window>
```
