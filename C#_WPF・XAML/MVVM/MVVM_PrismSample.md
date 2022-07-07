# Prismを用いたMVVMサンプル

<https://blog.okazuki.jp/entry/2014/12/23/180413>

## MVVMパターンとは

プログラムの構成をView(見た目),ViewModel(見た目とデータの制御),Model(データの制御)に分けた物。  
基本的に、View,ViewModel間のやり取りはプロパティのBindingを行う。  
プロパティ変更通知はINotifyPropertyChangedインターフェースを使用するが、Prismは便利なサポート  

``` C# : ViewModel
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

ビルドしてView（XAML）を作成します。

``` XML
xmlns:l="clr-namespace:MVVMSample01"
```

そして、DataContextプロパティに先ほど作成したViewModelクラスを設定します。

``` XML
<Window.DataContext>
    <l:MainWindowViewModel />
</Window.DataContext>
```

画面を作成していきます。
入力用のTextBoxと出力用のTextBlockとコマンドを実行するためのButtonを置いて、ViewModelの対応するプロパティとバインディングしています。

``` XML
<!-- ViewModelをXAMLで参照できるように名前空間の定義を行います。 -->
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

---

[.NET standard2.x時代のMVVMライブラリ](https://qiita.com/hqf00342/items/40a753edd8e37286f996)  
Microsoft.Toolkit.Mvvm  
CommunityToolkit relaycommand  

[What is the MVVM pattern, What benefits does MVVM have?](https://www.youtube.com/watch?v=AXpTeiWtbC8)  

アノテーションだけでバインディングできるようになってた。  

---

## WPFでMainWindow.xamlのフォルダを変更する

[WPFでMainWindow.xamlのフォルダを変更する](https://www.paveway.info/entry/2019/07/01/wpf_startupuri)  

App.xamlのStartupUriを変更する。

``` xml : MainWindow.xamlをViewsフォルダに移動した場合
<!-- 修正前 -->
<Application
    ...
    StartupUri="MainWindow.xaml"
/>
<!-- 修正後 -->
<Application
    ...
    StartupUri="Views/MainWindow.xaml"
/>
```

上記だけで起動するが、Viewsに移動させた以上[MainWindow.xaml.cs]と[MainWindow.Xaml]の名前空間も変更しておく。

``` xml : MainWindow.xaml
<!-- 修正前 -->
<Window
    x:Class="WpfApp8.MainWindow"
/>
<!-- 修正後 -->
<window
    x:Class="WpfApp8.Views.MainWindow"
/>
```
