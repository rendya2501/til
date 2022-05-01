# XAML基礎・基本まとめ

[WPFのgitレポジトリ](https://github.com/dotnet/wpf)  

---

## XAMLでNULLの指定の仕方

``` XML
    <!-- "{x:Null}"って指定する -->
    <Setter Property="FocusVisualStyle" Value="{x:Null}" />
```

---

## x:Typeのありなしの違い

東さん的には文字列として認識させるか、型情報として認識させるかの違いって言っていた気がする。  

[when to use {x:Type …}?](https://stackoverflow.com/questions/11167536/when-to-use-xtype)  

効果に違いはありません。どちらの場合もTargetTypeプロパティはtypeof(Border)に設定されます。  
最初のバージョン{x:Type Border}は、WPFの最初のバージョンで必要とされたもので、  
コンパイラが文字列をTypeオブジェクトに変換するためにTypeConverterクラスを使用せず、  
それを行うためにTypeExtensionクラスを指定する必要があったためです。  

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

## Binding ConverterParameterプロパティを使う

<http://cswpf.seesaa.net/article/313843710.html>

今まで地味に謎だった、Converterの第3引数`parameter`に値を入れるサンプル。  
「ConverterParameterもBindingして動的に変更できればいいのですがそれができないのであまり使えません。」  
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

## 左右に分けて配置するテク

結構需要はあるのだが、毎回忘れるのでメモすることにした。  

``` XML
    <Grid Grid.Row="1">
        <!-- 左のまとまり -->
        <StackPanel HorizontalAlignment="Left" Orientation="Horizontal">
            <!-- 内容 -->
        </StackPanel>
        
        <!-- 右のまとまり -->
        <StackPanel HorizontalAlignment="Right" Orientation="Horizontal">
            <!-- 内容 -->
        </StackPanel>
    </Grid>
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

DependencyProperty→バインディングができるようにする。コントロールにそういう機能がある。

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
値を文字列で指定できる（文字列そのもの or 文字列から直接変換可能な型）プロパティの場合はこの構文を使うと便利です。  

``` XML
<TextBox 
  Width = "100" FontSize = "30" Text = "text 1"
  Background = "White" Foreground = "Blue" />
```

### プロパティ要素構文(Property Element Syntax)

要素のinnerText・innerXMLを使用してプロパティを設定する。
では、もっと複雑な型を持つプロパティの場合にはどうすればいいかと言うと、 XML 要素の子要素としてプロパティの値を設定する Property Element Syntax という構文も用意されています。
例えば、上の例を Property Element Syntax で書き直すと以下のようになります。

``` XML
<TextBox>
  <TextBox.Width>100</TextBox.Width>
  <TextBox.FontSize>30</TextBox.FontSize>
  <TextBox.Background>White</TextBox.Background>
  <TextBox.Foreground>Blue</TextBox.Foreground>
  <TextBox.Text>text 1</TextBox.Text>
</TextBox>
```

`<SETTER>`はStyleタグでのみ有効な模様。

---

## DataContextとは?

[データ・バインディングを理解する](https://marikooota.hatenablog.com/entry/2017/05/30/002059)

---

## AssociatedObject

1. インスタンスがアタッチされているオブジェクトを現す  
2. そのBehaviorがアタッチしているUI要素  

Behaviorでの話。  
実際、Behaviorを修正しているときに当然のように出てきて正体が分からなかったので調べたが、thisとかそういった類のものらしい。  

[WPF4.5入門 その59「Behaviorの自作」](https://blog.okazuki.jp/entry/2014/12/22/235048)  
[【WPF】Behaviorで快適WPFライフ](https://anopara.net/2014/06/20/cool-behavior-life/)  

---

## XAMLのコントロール

[C#のWPFのコントロール一覧](https://water2litter.net/rye/post/c_control_list/#my_mokuji10)  
