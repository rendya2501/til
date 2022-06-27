# XAML_基本まとめ

[WPFのソースコード](https://github.com/dotnet/wpf)  
[コントロールのスタイルとテンプレート](http://msdn.microsoft.com/ja-jp/library/aa970773(v=vs.110).aspx)  
[WPFのコントロール一覧](https://water2litter.net/rye/post/c_control_list/#my_mokuji10)  

[kazuki_WPF入門](https://blog.okazuki.jp/entry/2014/12/27/200015)  
[WPF4.5入門](https://www.slideshare.net/okazuki0130/wpf45-38048141)  

[DataGridView編メニュー](https://dobon.net/vb/dotnet/datagridview/)  
[PrismとLivetで画面を閉じるMVVM](https://redwarrior.hateblo.jp/entry/2020/08/31/090000)  
[Windows Presentation Foundation実践](http://kisuke0303.sakura.ne.jp/blog/wordpress/wp-content/uploads/2016/08/4843696230c1698ad8ff7d086b998344.pdf)

[[C# WPF] なんとかしてWPFの描画を速くしたい「Canvas.Childrenへのオブジェクト追加」](https://www.peliphilo.net/archives/2390)
[WPF で ENTER キーを押したらフォーカス移動するようにする](https://rksoftware.wordpress.com/2016/05/04/001-14/)

[【WPF】【MVVM】GUIのマウス/キー操作処理をコードビハインドから駆逐する](https://qiita.com/hotelmoskva_/items/13ecc724bdad00078c16)  
[ダブルクリックイベントを持っていないコントロールで判定を拾う](https://www.hos.co.jp/blog/20200331/)  

---

## XAMLでNULLの指定の仕方

``` XML
    <!-- "{x:Null}"って指定する -->
    <Setter Property="FocusVisualStyle" Value="{x:Null}" />
```

---

## x:Typeのありなしの違い

azmさん的には文字列として認識させるか、型情報として認識させるかの違いって言っていた気がする。  

[when to use {x:Type …}?](https://stackoverflow.com/questions/11167536/when-to-use-xtype)  

>効果に違いはありません。  
>どちらの場合もTargetTypeプロパティはtypeof(Border)に設定されます。  
>
>最初のバージョン{x:Type Border}は、WPFの最初のバージョンで必要とされたもので、コンパイラが文字列をTypeオブジェクトに変換するためにTypeConverterクラスを使用せず、それを行うためにTypeExtensionクラスを指定する必要があったためです。  

これを考えれば、今は文字列での指定もできるようになったということか。  
まぁ、単純な文字列としてよりは型として認識してもらったほうがより厳密でいいだろうから、そうしてるって程度だろうな。  

x:Type マークアップ拡張機能には、C# の typeof() 演算子や Microsoft Visual Basic の GetType 演算子に似た関数があります。  
x:Type マークアップ拡張機能は、Type 型を受け取るプロパティに対して、文字列変換動作を提供します。  

---

## TextBoxで未入力の場合にBindingしてるソースのプロパティにnullを入れたい

<https://blog.okazuki.jp/entry/20110411/1302529830>  

なんてことはない知識だが、一応MDの練習やTILのために追加。  
テキストボックスの空文字をNULLにしたい場合はどうすればいいのか分からなかったから調べたらドンピシャのがあったので、メモ。  

TargetNullValueプロパティを以下のように書くことで、空文字のときにnullがプロパティに渡ってくるようになります。
TargetNullValueはこの値が来たらnullとして扱うことを設定するためのプロパティの模様。  

``` XML
<TextBox Text="{Binding Path=NumberInput, UpdateSourceTrigger=PropertyChanged, TargetNullValue=''}" />
```

---

## BindingのConverterParameterプロパティ

今まで地味に謎だった、Converterの第3引数`parameter`に値を入れるサンプル。  

<http://cswpf.seesaa.net/article/313843710.html>
>「ConverterParameterもBindingして動的に変更できればいいのですがそれができないのであまり使えません。」  

→  
バインドできないのか・・・。  
これを使うくらいならMultiBinding使ったほうがいいという記事が散見される。  
というわけで、MultiBindの概念を勉強する必要がありそうですね。  

``` XML
<Window.Resources>
    <local:HexConverter x:Key="HexConv"/>
</Window.Resources>
<Grid>
    <Grid.RowDefinitions>
        <RowDefinition Height="Auto"/>
        <RowDefinition Height="Auto"/>
    </Grid.RowDefinitions>
    <TextBox Text="{Binding Value, UpdateSourceTrigger=PropertyChanged}"
         Width="70" Height="23" Margin="5"
         HorizontalAlignment="Left" VerticalAlignment="Top"/>
    <TextBlock Text="{Binding Value, Converter={StaticResource HexConv}, ConverterParameter=4}"
        Grid.Row="1" 
        Width="70" 
        Height="23" 
        Margin="5"
        HorizontalAlignment="Left" 
        VerticalAlignment="Top"/>
</Grid>
```

``` C#
public class HexConverter : IValueConverter
{
    public object Convert(object value, Type type, object parameter, CultureInfo culture)
    {
        int v = (int)value;
        int p = int.Parse((string)parameter);
        return string.Format("0x{0:X" + p.ToString() + "}", v);
    }

    public object ConvertBack(object value, Type type, object parameter, CultureInfo culture)
    {
        string s = (string)value;
        return int.TryParse(s, NumberStyles.AllowHexSpecifier, null, out int v) ? v : 0;
    }
}
```

---

## RelativeSource

[WPFのRelativeSourceのはなし](https://hidari-lab.hatenablog.com/entry/wpf_relativesource_self_and_findancestor)  

---

## 純正のWPFでコマンドを実装する方法

こっちがおすすめ。  
[MVVM:とにかく適当なICommandを実装したい時のサンプル](https://running-cs.hatenablog.com/entry/2016/09/03/211015)  

まったくおすすめしないが、ライブラリ使わないとここまで大変というサンプル  
[WPFのMVVMでコマンドをバインディングする利点](https://takamints.hatenablog.jp/entry/why-using-commands-in-wpf-mvvm)  

---

## 依存関係プロパティの実装方法

[【WPF】依存関係プロパティでユーザーコントロールをバインド対応する！](https://resanaplaza.com/%E3%80%90wpf%E3%80%91%E4%BE%9D%E5%AD%98%E9%96%A2%E4%BF%82%E3%83%97%E3%83%AD%E3%83%91%E3%83%86%E3%82%A3%E3%81%A7%E3%83%A6%E3%83%BC%E3%82%B6%E3%83%BC%E3%82%B3%E3%83%B3%E3%83%88%E3%83%AD%E3%83%BC/)

DependencyProperty→バインディングができるようにする。  
コントロールにそういう機能がある。  

バインディングをしたときに、バインド間で通知をしてくれる。  
この処理は.Net側の処理なので、その先で何をやっているかはわからない。  

---

## 添付プロパティ

例えば、テキストボックスとボタンがあります。
カーソルが当たっている間は色を変えたいけれど、どちらにもその機能はない。
それぞれを継承して拡張した、CustomButton,CustomTextを作ってもいいけど、どちらにも同じコードを書くことになるし作ること自体に手間がかかる。
そういう時に添付プロパティなるものを作って、それをテキストボックスとボタンに実装してあげることで、その機能を実現することができる。
それが、添付プロパティ。

どうやって作るかは後でまとめる。

---

## XAML プロパティの設定方法

[XAML の基本構造（WPF）](https://ufcpp.net/study/dotnet/wpf_xamlbasic.html)  
[XAMLの書き方（１）](https://techinfoofmicrosofttech.osscons.jp/index.php?XAML%E3%81%AE%E6%9B%B8%E3%81%8D%E6%96%B9%EF%BC%88%EF%BC%91%EF%BC%89#ra4d77a1)  

### プロパティ属性構文(Attribute Syntax)

要素の属性にテキストを使用して設定する。  
値を文字列で指定できる（文字列そのもの or 文字列から直接変換可能な型）プロパティの場合はこの構文を使うと便利。  

``` XML : プロパティ属性構文(Attribute Syntax)
<TextBox 
  Width = "100" FontSize = "30" Text = "text 1"
  Background = "White" Foreground = "Blue" />
```

### プロパティ要素構文(Property Element Syntax)

要素のinnerText・innerXMLを使用してプロパティを設定する。  
複雑な型を持つプロパティの場合に有効。  
XML 要素の子要素としてプロパティの値を設定する構文。  

``` XML : プロパティ要素構文(Property Element Syntax)
<TextBox>
  <TextBox.Width>100</TextBox.Width>
  <TextBox.FontSize>30</TextBox.FontSize>
  <TextBox.Background>White</TextBox.Background>
  <TextBox.Foreground>Blue</TextBox.Foreground>
  <TextBox.Text>text 1</TextBox.Text>
</TextBox>
```

### Styleの書き方

Styleまとめを参照されたし。  

---

## DataContext

[データ・バインディングを理解する](https://marikooota.hatenablog.com/entry/2017/05/30/002059)  

>データ・バインディングとは、MVVMパターンでいうViewとViewModelを結び付けるために提供されている仕組み。  
>ViewとViewModel間のやりとりはDataContextというプロパティを介してやりとりします。  
>Viewで表示したいViewModelのデータ・ソースをDataContextプロパティに渡すだけで、結び付けることができるのです。  

→  
DataContextってプロパティなんだな。  
でもって、ViewとViewModelを結びつけるための橋ってイメージでいいかな。  

``` XML : Xamlで設定する場合
<!-- 名前空間:クラス名  の形で指定するので名前空間にViewModelを通しておく-->
<!-- 例 xmlns:vm="clr-namespace:WpfApp.ViewModel -->

<Window.DataContext>
    <local:ViewModel />
</Window.DataContext>
```

``` C# : コードビハインドで設定する場合
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ViewModel();
        }
    }
```

---

## AssociatedObject

1. インスタンスがアタッチされているオブジェクトを現す  
2. そのBehaviorがアタッチしているUI要素  

Behaviorでの話。  
実際、Behaviorを修正しているときに当然のように出てきて正体が分からなかったので調べたが、thisとかそういった類のものらしい。  

[WPF4.5入門 その59「Behaviorの自作」](https://blog.okazuki.jp/entry/2014/12/22/235048)  
[【WPF】Behaviorで快適WPFライフ](https://anopara.net/2014/06/20/cool-behavior-life/)  

---

## Gridの行列の幅や高さの定義

[C#WPFの道#3！Gridの使い方をわかりやすく解説！](https://anderson02.com/cs/wpf/wpf-3/)  
GridのWidthやHeightに[*]や[Auto]を指定した時の動作が曖昧だったのでまとめ。  

### 数値

絶対値でピクセルを指定します。  

### *

均等な比率を割り当てます。  

例)  
3列宣言していて、すべての列の定義が「＊」の場合は、均等に3列の幅が作られます。  
「２＊」などと、数字を＊の前に記述すると、その列のみ2倍などの指定した値の倍率で確保されます。  

### Auto

設置したコントロールの幅で動的に変動します。  
おそらく、内部のコントロールが動的に変化する場合、Gridの大きさ同じように変化すると思われる。  

---

## StaticResourceとDynamicResource

[WPFのStaticResourceとDynamicResourceの違いMSDN](https://social.msdn.microsoft.com/Forums/ja-JP/3bbcdc48-2a47-495e-9406-2555dc515c3a/wpf12398staticresource12392dynamicresource123983694912356?forum=wpfja)  
[WPFのStaticResourceとDynamicResourceの違い](https://tocsworld.wordpress.com/2014/06/26/wpf%E3%81%AEstaticresource%E3%81%A8dynamicresource%E3%81%AE%E9%81%95%E3%81%84/)  
[WPF4.5入門 その51 「リソース」](https://blog.okazuki.jp/entry/2014/09/06/124431)  

### StaticResource

リソースとバインド先の依存関係プロパティの対応付けは起動時の1回のみ。  
ただしクラスは参照なのでリソースのプロパティの変更はバインド先も影響を受ける。  

StaticResourceマークアップ拡張は、実質的にはリソースの値を代入するのと等価です。  

### DynamicResource

リソースとバインド先の依存関係プロパティの対応付けは起動時および起動中(リソースに変更がある度)。  
つまりリソースのオブジェクトが変わってもバインド先は影響を受けるし、当然リソースのプロパティ変更はバインド先も影響を受ける。  

DynamicResourceマークアップ拡張は、設定したキーのリソースが変更されるかどうかを実行時に監視していて、リソースが変わったタイミングで再代入が行われます。  

### 比較・まとめ

DyanmicResourceマークアップ拡張を使うと、例えばアプリケーションのテーマの切り替えといったことが可能になります。  
しかし、必要でない限りStaticResourceマークアップ拡張を使うべきです。  
その理由は、単純にStaticResourceマークアップ拡張のほうがDynamicResourceマークアップ拡張よりもパフォーマンスが良いからです。  

StaticResourceマークアップ拡張を使う時の注意点として、単純な代入という特徴から、使用するよりも前でリソースが定義されてないといけないという特徴があります。  
DynamicResourceマークアップ拡張は、このような制約が無いため、どうしても前方でリソースの宣言が出来ないときもDynamicResourceマークアップ拡張を使う理由になります。  

### ItemsControl製作中の例

StaticResourceとDynamicResourceを勉強した後に追記。  
StaticResourceは性質上、使う時より上で定義していないと使えない。  
その制約がないDynamicResourceであれば、下のような書き方ができるが、接続を常に監視する性質上、パフォーマンスは落ちてしまう模様。  
それならItemsControlをGridで囲み、Gridのリソースとして定義してStaticResourceを指定すべきだと感じた。  
また、自分自身のResourceで完結しているように見えるが、これなら各Templateにそれぞれぶち込んだほうがよさそうに見える。  

まぁ、こういうこともできるよということで書いたけど、実際に使うことはないだろう。  

``` XML : 完成3
<ItemsControl
    ItemTemplate="{DynamicResource ItemTemplate}"
    Template="{DynamicResource MainContentTemplate}">
    <!--  自分自身のリソースに定義してDynamicResourceでバインド  -->
    <ItemsControl.Resources>
        <DataTemplate x:Key="ItemTemplate"/>
        <ControlTemplate x:Key="MainContentTemplate" TargetType="{x:Type ItemsControl}"/>
    </ItemsControl.Resources>
</ItemsControl>
```

---

## UserControl

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

---

## ControlTemplate

[WPF4.5入門 その52 「コントロールテンプレート」](https://blog.okazuki.jp/entry/2014/09/07/195335)  

WPFのコントロールは、見た目を完全にカスタマイズする方法が提供されています。  
コントロールは、TemplateというプロパティにControlTemplateを設定することで、見た目を100%カスタマイズすることが出来るようになっています。  

→  
ItemsControlの時にTemplateプロパティの直下に配置したあれ。  
Templateプロパティ自体、ControlTemplateクラスしか許可していなかったので、そういうものなんだなとしか理解していなかった。  

``` XML : コントロールのTemplateの差し替え例
<!-- WPFのLabelコントロールには、Windows Formと異なりClickイベントが提供されていません。 -->
<!-- ここではClick可能なLabelの実現のために、Buttonコントロールの見た目をLabelにします。 -->

<Button Content="ラベル" Click="Button_Click">
    <Button.Template>
        <ControlTemplate TargetType="{x:Type Button}">
            <Label Content="{TemplateBinding Content}" />
        </ControlTemplate>
    </Button.Template>
</Button>
```

ControlTemplateは、TargetTypeにテンプレートを適用するコントロールの型を指定します。  
そして、ControlTemplateの中に、コントロールの見た目を定義します。  

このとき、TemplateBindingという特殊なBindingを使うことで、コントロールのプロパティをバインドすることが出来ます。  
上記の例ではButtonのContentに設定された値をLabelのContentにBindingしています。  

### 必要な構成要素があるコントロールを上書きする場合

コントロールには、そのコントロールが動作するために必要な構成要素がある場合があります。  
スクロールバーのバーやバーを左右に移動するためのボタンなど、見た目だけでなく、操作することが出来る要素がそれにあたります。  
このようなコントロールは、ControlTemplate内に、コントロールごとに決められた名前で定義する必要があります。  
どのように定義しているかは、MSDNにある、デフォルトのコントロールテンプレートの例を参照してください。  

[コントロールのスタイルとテンプレート](http://msdn.microsoft.com/ja-jp/library/aa970773(v=vs.110).aspx)  

→  
チェックボックスの例を見たときに、やたら複雑に書かれていたのはこういう仕組みがあったからか。  
スタイルを適応するときも、元のコントロールと同じように再定義しないといけないのもこの仕組みのためだろうか。  

---

## INotifyPropertyChanged

[WPF4.5入門 その60「データバインディングを前提としたプログラミングモデル」](https://blog.okazuki.jp/entry/2014/12/23/180413)  

INotifyPropertyChangedインターフェースはPropertyChangedイベントのみをもつシンプルなインターフェースです。  
このイベントを通じてModelからViewModel、ViewModelからViewへの変更通知が行われます。  

[世界で一番短いサンプルで覚えるMVVM入門](https://resanaplaza.com/%E4%B8%96%E7%95%8C%E3%81%A7%E4%B8%80%E7%95%AA%E7%9F%AD%E3%81%84%E3%82%B5%E3%83%B3%E3%83%97%E3%83%AB%E3%81%A7%E8%A6%9A%E3%81%88%E3%82%8Bmvvm%E5%85%A5%E9%96%80/)  

``` C# : INotifyPropertyChangedの最小実装
    // 1. INotifyPropertyChangedの継承
    internal class ViewModel : INotifyPropertyChanged
    {
        // 2. PropertyChangedEventHandlerイベントハンドラの記述
        /// <summary>
        /// INotifyPropertyChangedインターフェース実装イベント
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// カウント数
        /// </summary>
        public int Count
        {
            get { return _Count; }
            set
            {
                _Count = value;
                // 3. プロパティの set 内でのPropertyChangedEventHandlerイベントハンドラの呼び出し
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
            }
        }
        private int _Count;
    }
```

### ヘルパークラス

INotifyPropertyChangedインターフェースの実装をすべてのプロパティに実装するのは負荷が高いため、一般的に以下のようなヘルパークラスが作成されます。  

→  
これが実務でも見るViewModelBaseに記述されてるあれになるわけだ。  

``` C# : INotifyPropertyChangedのヘルパークラス(BindableBase)
using System.ComponentModel;
using System.Runtime.CompilerServices;

    /// <summary>
    /// INotifyPropertyChangedのヘルパークラス
    /// </summary>
    public class BindableBase : INotifyPropertyChanged
    {
        /// <summary>
        /// INotifyPropertyChangedインターフェース実装イベント
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// 値のセットと通知を行う
        /// </summary>
        protected virtual bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            // 同じ値なら処理しない
            if (Equals(field, value))
            {
                return false;
            }
            // 値を反映
            field = value;
            // プロパティ発火
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            // 正常に終了したことを通知
            return true;
        }
    }
```

``` C# : BindableBaseの実装例
    public class ViewModel : BindableBase
    {

        /// <summary>
        /// カウント数
        /// </summary>
        public int Count
        {
            get { return _Count; }
            set { SetProperty(ref _Count, value); }
        }
        private int _Count;
    }
```

---

## ICommand

INotifyPropertyChangedをやったなら、ボタンを押したときの実装もするだろう。  
というわけで、原初のICommandを実装してみたが、クソ面倒くさいなこれ。  
これをプレーンのままで実装するなんて考えられない。  

``` C# : Commandクラスに実装すべき内容
public class CountDownCommand : ICommand
{
    public event EventHandler CanExecuteChanged
    {
         ・・・・・
    }
 
    public bool CanExecute(object parameter)
    {
         ・・・・・
    }
 
    public void Execute(object parameter)
    {
         ・・・・・
    }
 }
```

[MVVM:とにかく適当なICommandを実装したい時のサンプル](https://running-cs.hatenablog.com/entry/2016/09/03/211015)  

``` C# : IComandの最小実装
    internal class ViewModel : INotifyPropertyChanged
    {
        // ICommand で宣言すること
        public ICommand PlaneCountDownCommand => new RelayCommand(() => Count++);
    }

    /// <summary>
    /// MVVMのViewModelから呼び出されるコマンドの定義
    /// コマンド1つにつき1つのクラスを定義する
    /// </summary>
    public class CountDownCommand : ICommand
    {
        /// <summary>
        /// Command実行時に実行するアクション
        /// 引数を受け取りたい場合はこのActionをAction<object>などにする
        /// </summary>
        private Action _action;

        /// <summary>
        /// コンストラクタ
        /// Actionを登録
        /// </summary>
        /// <param name="action"></param>
        public CountDownCommand(Action action) => _action = action;

        #region ICommandインターフェースの必須実装
        /// <summary>
        /// コマンドのルールとして必ず実装しておくイベントハンドラ
        /// 通常、このメソッドを丸ごとコピーすればOK
        /// RaiseCanExecuteChanged が呼び出されたときに生成されます。
        /// </summary>
        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// コマンドの有効／無効を判定するメソッド
        /// コマンドのルールとして必ず実装しておくメソッド
        /// 有効／無効を制御する必要が無ければ、無条件にTrueを返しておく
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object? parameter)
        {
            //とりあえずActionがあれば実行可能
            return _action != null;
        }

        /// <summary>
        /// コマンドの動作を定義するメソッド
        /// コマンドのルールとして必ず実装しておくメソッド
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object? parameter)
        {
            //今回は引数を使わずActionを実行
            _action?.Invoke();
        }
        #endregion
    }
```

そこでPrismになるわけだが、Prismはマイクロソフトが開発しているサポートライブラリなので気にせずどんどん入れよう。  
でもって、VSが新しいためかは知らないが、DelegateCommandって入力して[Ctrl + .]でおすすめを表示させると、Prismをインストールしてusingまで通してくれる選択肢が出てくる。  
簡単にインストールできてしかも軽いので、コマンドの実装するなら使わない手はない。  
nugetから入れようとすると、まず開くので重くて、調べるので重くて、インストールがだるいという3重苦だが、ここまで軽くて簡単に入れられるならマストで入れるべきだ。  

Prismでの実装ならたった1行で済む。控えめに言って神。  

``` C# : Prismでの実装
using Prism.Commands;

    internal class ViewModel : INotifyPropertyChanged
    {
        public DelegateCommand DelegateCountDownCommand => new DelegateCommand(() => Count++);
    }
```

CanExecuteとかFunc\<T>とか実装したいならこのサイトを参考に実装すればいいと思うけど、ならDelegateCommand使えという話。  
[XAMLからViewModelのメソッドにバインドする～RelayCommand～](https://sourcechord.hatenablog.com/entry/2014/01/13/200039)  

---

## ObservableCollection

なんとなく使ってはいるが、なぜ使うのか、どういうものなのかずっとわからなかったのでまとめることにした。  

[マイクロソフト公式](https://docs.microsoft.com/ja-jp/dotnet/api/system.collections.objectmodel.observablecollection-1?view=netcore-3.1)  
>項目が追加または削除されたとき、あるいはリスト全体が更新されたときに通知を行う動的なデータ コレクションを表します。  

[意外と知らない！？ C#の便利なコレクション！](https://qiita.com/hiki_neet_p/items/75bf39838ce580cca92d)  
>コレクションのアイテムに対する、追加、削除、変更、移動操作があった場合、またはリスト全体が更新されたとき、CollectionChanged イベントを発生させることができるコレクションです。  
>「Observable」という名前がついていますが、IObservable\<T> や IObserver\<T> とは直接の関連はありません。  
>むしろ、INotifyPropertyChanged に近いイメージです。  
>ObservableCollection\<T> は INotifyPropertyChanged も実装していますが、そのイベントを直接購読することはできないようになっています。

→  
追加や削除した時にイベントを発生させるので、追加、削除した時に何かやりたい時はObservableCollectionを使う必要がある。  

``` C# : 実装例
    public class ViewModel
    {
        /// <summary>
        /// コレクション本体
        /// </summary>
        public ObservableCollection<Person> People { get; set; } 
            = new ObservableCollection<Person>(Enumerable.Range(1, 10).Select(x => new Person { Name = "tanaka" + x, Age = x }));
        /// <summary>
        /// コレクションに要素を追加する
        /// </summary>
        public DelegateCommand AddItem 
            => new DelegateCommand(() => People.Add(new Person { Name = "追加したtanaka", Age = People.Count + 1 }));
        /// <summary>
        /// コレクションの要素を削除する。
        /// </summary>
        public DelegateCommand RemoveItem 
            => new DelegateCommand(() => People.RemoveAt(People.Count - 1 ));

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ViewModel()
        {
            // ObservableCollectionにイベント登録
            People.CollectionChanged += People_CollectionChanged;
        }

        /// <summary>
        /// ObservableCollectionのAddやRemoveされた時実行される処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void People_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Console.WriteLine("Action for this event: {0}", e.Action);

            switch (e.Action)
            {
                // They added something. 
                case NotifyCollectionChangedAction.Add:
                    // Now show the NEW items that were inserted.
                    Console.WriteLine("Here are the NEW items:");
                    foreach (Person p in e.NewItems)
                    {
                        Console.WriteLine(p.ToString());
                    }
                    break;
                // They removed something. 
                case NotifyCollectionChangedAction.Remove:
                    Console.WriteLine("Here are the OLD items:");
                    foreach (Person p in e.OldItems)
                    {
                        Console.WriteLine(p.ToString());
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Move:
                case NotifyCollectionChangedAction.Reset:
                default:
                    break;
            }

            Console.WriteLine();
        }
    }
```

``` XML
<Window>
    <Window.DataContext>
        <local:ViewModel />
    </Window.DataContext>
    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto" />
            <RowDefinition Height="*" />
        </Grid.RowDefinitions>
        <Menu Grid.Row="0">
            <MenuItem Command="{Binding AddItem}" Header="追加" />
            <MenuItem Command="{Binding RemoveItem}" Header="削除" />
        </Menu>
        <ListBox Grid.Row="1" ItemsSource="{Binding People}">
            <ListBox.ItemTemplate>
                <DataTemplate>
                    <StackPanel Orientation="Horizontal">
                        <TextBox Text="{Binding Name}" />
                        <TextBox Text="{Binding Age}" />
                    </StackPanel>
                </DataTemplate>
            </ListBox.ItemTemplate>
        </ListBox>
    </Grid>
</Window>

```

---

## コードビハインド

VB6時代、Buttonをダブルクリックして飛ばされた先でガリガリ書いてたアレ。  
具体的な定義を言語化できなかったのと、ビヘイビアとの違いを上手く説明できそうになかったのでまとめ。  

### 一番しっくり来た説明

[[UWP]XAMLとコードビハインドの関係とは？](https://colorfulcompany.com/uwp/hello-world)  
>UWPの開発はXAMLが欠かせません。そして、XAMLには連動するソースファイルが存在します。  
>例えば、MainPage.xaml なら、MainPage.xaml.cs という感じで、XAMLファイルと同じ名前を持ったソースが存在します。  
>そして、このソースファイルは、MainPage.xaml に配置されたコントロールに対するプログラムが書かれています。  
>このように、一つのXAMLとソースファイルが連動している仕組みをコードビハインドと言います。  
>XAMLで見た目を作り、C#で動きをプログラムするという流れです。  

「ビハインド:後ろにある」 の意味の通り、見た目に対する制御を記述する裏方的なニュアンスだと思われる。  
やっぱり「コードビハインド」は古い概念で、2004年のASP.NETの記事がヒットしたりする。  
どうやら更に大昔は、見た目と制御を1つのファイルに記述していたみたいだ。考えただけで大変そう。  

[日経Windowsプロ](https://xtech.nikkei.com/it/free/NT/WinReadersOnly/20040810/21/#:~:text=%E3%82%A2%E3%83%97%E3%83%AA%E3%82%B1%E3%83%BC%E3%82%B7%E3%83%A7%E3%83%B3%E3%82%92%E9%96%8B%E7%99%BA%E3%81%99%E3%82%8B%E9%9A%9B,%E3%81%AE1%E3%81%A4%E3%81%A7%E3%81%82%E3%82%8B%E3%80%82)  
>アプリケーションを開発する際に，ユーザー・インターフェース部分とプログラム（ロジック）部分を別のファイルに分けること。  
>ASP.NETによるWebアプリケーション開発の特徴の1つである。  
>これによって，デザインを担うデザイナと，プログラムを担当するプログラマとの作業の切り分けがしやすくなる。  

### その他説明

[WPF における分離コードと XAML](https://docs.microsoft.com/ja-jp/dotnet/desktop/wpf/advanced/code-behind-and-xaml-in-wpf?view=netframeworkdesktop-4.8)  
>コードビハインドとは、XAML ページがマークアップ コンパイルされる際に、マークアップ定義オブジェクトと結合されるコードを表すために使用される用語です。  

[C# WPFアプリ XAMLの基本概念を理解する](https://setonaikai1982.com/xaml_basic/)  
>XAMLと連携させることができるC#のファイルはコードビハインドと呼ばれ、ファイル名は****.xaml.csで保存します。  
>xamlもコードビハインド(xaml.cs)もファイルは別々ですが、どちらも同一のViewクラスの一部です。  
>XAMLとC#ファイルのクラス名を確認すると、どちらも同じ名前のWindowクラスになっています。  

---

## なぜMVVMではコードビハインドに記述することは悪なのか

基本的に、MVVMではコードビハインドにコードを記述することは悪であるという認識が強いみたいだが、なぜなのだろうか。  
それはそれで別途まとめていきたいと思う。  

[MVVM:コードビハインドに記述しても良いと思う](http://gushwell.ldblog.jp/archives/52147035.html)  

---

## ビヘイビア

コードビハインドのイベントべた書きから、イベント発火によって実行する処理部分だけを抜き出した感じのやつ。  

例えば、別々のプログラムで、あるボタンを押したときに同じ処理をさせたいなら、その動作だけをビヘイビアとして切り出し、それぞれのボタンにビヘイビア(振る舞い)を適応させるという芸当が可能になる。  
同じ動作をしているなら、共通の定義としてビヘイビアを抜き出すのもいいかもしれない。  

[かずきのBlog@hatena_ビヘイビア(Behavior)の作り方](https://blog.okazuki.jp/entry/20100823/1282561787)  
>ビヘイビアは、個人的な解釈だとコードビハインドにイベントハンドラを書くことなく何かアクションをさせるための部品と思っています。  

[WPFのビヘイビア](https://qiita.com/flasksrw/items/04818070091fe82495e7)  
>言葉の意味は「振る舞い」。  
>WPFでMVVMに準拠する際、Viewの状態の変化をきっかけにして、Viewで実行される処理の実装方法のことを指す。  
>MVVMに準拠するとコードビハインドが使えないので、その代替手段ということになる。  

### ビヘイビアの最小サンプル

nugetから[Microsoft.Xaml.Behaviors.Wpf]をインストール

```C#
using Microsoft.Xaml.Behaviors;
using System;
using System.Windows;
using System.Windows.Controls;

    public class AlertBehavior : Behavior<Button>
    {
        #region メッセージプロパティ
        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register(
                "Message",
                typeof(string),
                typeof(AlertBehavior),
                new UIPropertyMetadata(null)
            );
        #endregion
        
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AlertBehavior() {}

        // 要素にアタッチされたときの処理。大体イベントハンドラの登録処理をここでやる
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.Click += Alert;
        }

        // 要素にデタッチされたときの処理。大体イベントハンドラの登録解除をここでやる
        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.Click -= Alert;
        }

        // メッセージが入力されていたらメッセージボックスを出す
        private void Alert(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.Message))return;
            MessageBox.Show(this.Message);
        }
    }
```

``` XML
    <Grid>
        <!-- プロパティ属性構文によるビヘイビアの指定は無理な模様。実務でも実績はなかった。 -->
        <Button
            Width="75"
            Margin="8,8,0,0"
            HorizontalAlignment="Left"
            VerticalAlignment="Top"
            Content="Button">
            <i:Interaction.Behaviors>
                <local:AlertBehavior Message="こんにちは世界" />
            </i:Interaction.Behaviors>
        </Button>
    </Grid>
```
