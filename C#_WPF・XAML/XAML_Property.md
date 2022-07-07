# XAML プロパティ

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

---

## DependencyProperty (依存関係プロパティ)

[【WPF】依存関係プロパティでユーザーコントロールをバインド対応する！](https://resanaplaza.com/%E3%80%90wpf%E3%80%91%E4%BE%9D%E5%AD%98%E9%96%A2%E4%BF%82%E3%83%97%E3%83%AD%E3%83%91%E3%83%86%E3%82%A3%E3%81%A7%E3%83%A6%E3%83%BC%E3%82%B6%E3%83%BC%E3%82%B3%E3%83%B3%E3%83%88%E3%83%AD%E3%83%BC/)

DependencyProperty→バインディングができるようにする。  
コントロールにそういう機能がある。  

バインディングをしたときに、バインド間で通知をしてくれる。  
この処理は.Net側の処理なので、その先で何をやっているかはわからない。  

---

## 依存プロパティをバインドする方法

[Binding to custom dependency property](https://stackoverflow.com/questions/13956767/binding-to-custom-dependency-property-again)  

マルチセレクトコンボボックス開発の時に直面。  
全選択コンボボックスのcontextを設定できたほうが便利だと思って依存プロパティを作ったはいいものの、素直にバインドできなかったのでまとめ。  
ただバインドさせるだけでは警告が出てうまく動作しない。

結論としてはRelativeSourceで自分自身のコントロールの親を指定する必要がある。  
Windowに乗っているならAncestorTypeはWindowだし、UsercontrolならAncestorTypeはUserControl。  

``` XML
<!-- SelectAllContentが依存プロパティとして外部公開し、実行時に反映する値 -->
<CheckBox
    Margin="5,1,0,1"
    HorizontalAlignment="Left"
    VerticalContentAlignment="Center"
    Content="{Binding SelectAllContent, RelativeSource={RelativeSource FindAncestor, AncestorType=UserControl}}"
    IsChecked="{x:Null}"
    IsThreeState="True" />
```

``` C#
    public static readonly DependencyProperty SelectAllContentProperty = DependencyProperty.Register(
        "SelectAllContent",
        typeof(string),
        typeof(MultiSelectComboBox1),
        new UIPropertyMetadata("Select All")
    );
    public string SelectAllContent
    {
        get { return (string)GetValue(SelectAllContentProperty); }
        set { SetValue(SelectAllContentProperty, value); }
    }
```

---

## 添付プロパティ

例えば、テキストボックスとボタンがあります。
カーソルが当たっている間は色を変えたいけれど、どちらにもその機能はない。
それぞれを継承して拡張した、CustomButton,CustomTextを作ってもいいけど、どちらにも同じコードを書くことになるし作ること自体に手間がかかる。
そういう時に添付プロパティなるものを作って、それをテキストボックスとボタンに実装してあげることで、その機能を実現することができる。
それが、添付プロパティ。

どうやって作るかは後でまとめる。

---

## 添付プロパティをBindingのPathに指定する場合はカッコを付ける

[添付プロパティをBindingのPathに指定する場合はカッコを付ける](https://qiita.com/flasksrw/items/7212453de6e7d8f221a1)

BindingのPathに添付プロパティを指定する場合、カッコをつけないと「BindingExpression path error」になる。

``` XML
<Window x:Class="Sample.MainView"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:local="clr-namespace:Sample"
    Title="MainView" Height="300" Width="300">
    <Grid>
        <TextBox>
            <TextBox.Style>
                <Style TargerType="TextBox">
                    <Style.Triggers>
                        <!--Pathにカッコを付ける-->
                        <DataTrigger Binding="{Binding Path=(local:AttachedXXX.XXX), RelativeSource={RelativeSource Self}}" Value="True">
                            <Setter Property="Background" Value="Blue"/>
                        </DataTrigger>
                    </Style.Triggers>
                </Style>
            </TextBox.Style>
        </TextBox>
    </Grid>
</Window>
```
