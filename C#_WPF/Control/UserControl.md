# UserControl

[簡単なユーザーコントロール(WPF)の作り方](https://qiita.com/tera1707/items/8d24b21a05ad84a1c92f)

右クリック→ユーザーコントロールを作成  

``` C# : SimpleUserControl
/// <summary>
/// SimpleUserControl.xaml の相互作用ロジック
/// </summary>
public partial class SimpleUserControl : UserControl
{
    /// <summary>
    /// 文字列の依存関係プロパティ
    /// </summary>
    public string MyText
    {
        get { return (string)GetValue(MyTextProperty); }
        set { SetValue(MyTextProperty, value); }
    }
    public static readonly DependencyProperty MyTextProperty =
        DependencyProperty.Register(
            "MyText",                               // プロパティ名
            typeof(string),                         // プロパティの型
            typeof(SimpleUserControl),              // プロパティを所有する型＝このクラスの名前
            new PropertyMetadata(string.Empty));    // 初期値

    /// <summary>
    /// コマンドの依存関係プロパティ
    /// </summary>
    public ICommand MyCommand
    {
        get { return (ICommand)GetValue(MyCommandProperty); }
        set { SetValue(MyCommandProperty, value); }
    }
    public static readonly DependencyProperty MyCommandProperty =
        DependencyProperty.Register(
            "MyCommand",                    // プロパティ名
            typeof(ICommand),               // プロパティの型
            typeof(SimpleUserControl),      // プロパティを所有する型＝このクラスの名前
            new PropertyMetadata(null));    // 初期値

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public SimpleUserControl() => InitializeComponent();
}
```

``` XML : UserControlXAML
<UserControl x:Class="WpfApp1.SimpleUserControl"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006" 
             xmlns:d="http://schemas.microsoft.com/expression/blend/2008" 
             xmlns:local="clr-namespace:WpfApp1"
             mc:Ignorable="d" 
             d:DesignHeight="100" d:DesignWidth="150"
             x:Name="root">
    <Grid Width="150" Height="100" Background="#99FF0000">
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width="100"/>
            <ColumnDefinition Width="50"/>
        </Grid.ColumnDefinitions>
        <!-- UserControlに「root」という名前を付けて、下でバインドするときに「ElementName=root」とすることを忘れないこと！ -->
        <Viewbox>
            <TextBlock Text="{Binding MyText, ElementName=root}" Grid.Column="0"/>
        </Viewbox>
        <!-- コマンド(=ボタン押したときの処理)も依存関係プロパティで登録できる -->
        <Button Content="ボタン" Command="{Binding MyCommand, ElementName=root}" Grid.Column="1"/>
    </Grid>
</UserControl>
```

今回の例では階層も何も考えず、同じ名前空間で作成したので、単純なlocalと今の名前空間だけを指定している。  
実際には、ユーザーコントロールはまとめて格納されていると思うので、そこの名前空間とエイリアスを定義すればよろしい。  

``` XML : 使う側のXAML
<Window 略
    xmlns:local="clr-namespace:WpfApp1">
    <Grid>
        <!-- 定義したコントロール名を指定 -->
        <local:SimpleUserControl 
            MyText="メインウインドウ側で指定した文字列"
            MyCommand="{Binding VmMyCommand}"/>
    </Grid>
</Window>
```
