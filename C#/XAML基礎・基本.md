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

`<SETTER>`タグはStyle内でのみ有効な模様。  

``` XML
<Style TargetType="TextBlock">
    <Setter Property="Text" Value="{Binding}" />
    <Setter Property="LayoutTransform">
        <Setter.Value>
            <DataTemplate>
                <ScaleTransform ScaleX="{Binding RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type metro:MetroWindow}}, Path=DataContext.Magnification, Mode=TwoWay}" ScaleY="{Binding RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type metro:MetroWindow}}, Path=DataContext.Magnification, Mode=TwoWay}" />
            </DataTemplate>
        </Setter.Value>
    </Setter>
</Style>
```

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

---

## Gridの行列の幅や高さの定義

[C#WPFの道#3！Gridの使い方をわかりやすく解説！](https://anderson02.com/cs/wpf/wpf-3/)  

GridのWidthやHeightに[*]や[Auto]を指定した時の動作が曖昧だったのでまとめ。  

- 数値 : 絶対値でピクセルを指定します。  

- [*] : 均等な比率を割り当てます。  
例)  
3列宣言していて、すべての列の定義が「＊」の場合は、均等に3列の幅が作られます。  
「２＊」などと、数字を＊の前に記述すると、その列のみ2倍などの指定した値の倍率で確保されます。  

- [Auto] : 設置したコントロールの幅で動的に変動します。  
  おそらく、内部のコントロールが動的に変化する場合、Gridの大きさ同じように変化すると思われる。  

---

## Resourceで定義したStyleを当てる方法

[WPF のリソース](http://var.blog.jp/archives/67298406.html)  
[[WPF/xaml]リソースディクショナリを作って、画面のコントロールのstyleを変える](https://qiita.com/tera1707/items/a462678cdfb61a87334b)  
[[WPF] Styleでできることと書き方](https://qiita.com/tera1707/items/cb8ad4c40107ae25b565)  

---

## StaticResourceとDynamicResourceの違い

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
