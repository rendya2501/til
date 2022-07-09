# XAMLまとめ

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
[WPFのRelativeSourceのはなし](https://hidari-lab.hatenablog.com/entry/wpf_relativesource_self_and_findancestor)  

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

## ViewModelからコントロールのメソッドを実行する方法

C1MultiSelectコントロールの実装中において、選択状態を初期化をしたいなーって時にUnselectAllというメソッドが用意されているのは分かったので、それを直接呼び出せないか調べていたら、CallMethodActionという、まさしくな仕組みがあったのでまとめ。  
[ビューモデルからビューのメソッドを呼ぶ](https://qiita.com/tera1707/items/d184c85d0c181e6563ea)  

``` XML : View
<i:Interaction.Triggers>
    <l:InteractionMessageTrigger MessageKey="SubjectLargeTypeListUnselectAll" Messenger="{Binding Messenger}">
        <!-- C1MultiSelectというコントロールにはUnselectAllという選択状態を全て解除するメソッドが用意されており、それをXAML上から直接呼び出せる -->
        <!-- 呼び出しはいつものアレ→Messenger.Raise(new InteractionMessage("SubjectLargeTypeListUnselectAll")); -->
        <i:CallMethodAction MethodName="UnselectAll" />

        <!-- FlexGrid関連にもまとめたが、自分自身のプロパティが持つメソッドをたどって実行させることもできる -->
        <i:CallMethodAction MethodName="Clear" TargetObject="{Binding RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type ctrl:CustomFlexGrid}}, Path=CollectionView.SortDescriptions}" />
    </l:InteractionMessageTrigger>
</i:Interaction.Triggers>
```

CallMethodActionはWPF側が用意しているものらしい。  
当然の如く、LivetにもLivetCallMethodActionなるものが用意されているので、機会があったらこっちも使うかも。  
こっちは簡単な引数も渡せる模様。  

``` XML
<l:LivetCallMethodAction
    MethodName="Add"
    MethodParameter="{Binding SelectedIndex}"
    MethodTarget="{Binding RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type c1:C1MultiSelect}}, Path=ListBox.SelectedItems}" />
```

---

## DataGrid – アクティブセルの枠線を消す（C# WPF)

<http://once-and-only.com/programing/c/datagrid-%E3%82%A2%E3%82%AF%E3%83%86%E3%82%A3%E3%83%96%E3%82%BB%E3%83%AB%E3%81%AE%E6%9E%A0%E7%B7%9A%E3%82%92%E6%B6%88%E3%81%99%EF%BC%88c-wpf/>  

ScrollViewerにフォーカスが当たって点線の枠が表示されてしまう問題が発生した。  
この点線をどう表現して調べたらいいかわからなかったところ、ドンピシャな記事があったので、備忘録として残す。  
因みに「wpf scrollviewer　点線」で調べた模様。  

``` XML
<DataGrid.CellStyle>
    <Style TargetType="DataGridCell">
        <Setter Property="BorderThickness" Value="0"/>
        <!-- 点線の枠→FocusVisualStyle : キーボードから操作した時にfocusが当たった時のスタイル -->
        <!-- FocusVisualStyle に Value="{x:Null}"を設定すると消せる -->
        <Setter Property="FocusVisualStyle" Value="{x:Null}"/>
    </Style>
</DataGrid.CellStyle>
```

---

## XAMLでConstファイルを読み込んで使いたい

<https://www.it-swarm-ja.com/ja/wpf/%E6%96%87%E5%AD%97%E5%88%97%E3%82%92%E9%9D%99%E7%9A%84%E3%83%AA%E3%82%BD%E3%83%BC%E3%82%B9%E3%81%A8%E3%81%97%E3%81%A6%E5%AE%9A%E7%BE%A9%E3%81%99%E3%82%8B/971295591/>  

XAML上の列名とTriggerActionで指定する列名をConstで定義して参照したほうがいいだろうということで探して見た。  
そしたら意外と探すのに苦労した。  

XAMLは参照を追加して [クラス名.フィールド名] でアクセスすればよい。  

``` C#
    public class ColumnName
    {
        public const string IsSelected = "IsSelected";
        public const string Amount = "Amount";
    }
```

``` XML
<metro:MetroWindow
    xmlns:localresource="clr-namespace:namespace.ColumnName">

    <c1:Column
        ColumnName="{x:Static localresource:ColumnName.Amount}"/>
</metro:MetroWindow>
```

---

## TextBlockのFormatの指定の仕方と2つの文字を繋げる方法

TextBlockで日付を表示するとyyyy/mm/dd hh:mm:ssの表示になってしまうので、(金額(decimal)も小数点が表示されてしまう)  
Formatがないか調べたらあったのでまとめる。  
BindingのStringFormatを使うことで実現可能であった。
<https://qiita.com/koara-local/items/815eb5146b3ddc48a8c3>  

また、MultiBindingを使用することで、  
2つのTextBlockで表示していたものを1つTextBlockで表示することが出来ることも発見したので同時にまとめる。  
<http://nineworks2.blog.fc2.com/blog-entry-10.html>  

``` XML
<!-- StringFormat={}{0:yyyy/MM/dd}で日付の表示を操作可能 -->
<StackPanel HorizontalAlignment="Left" Orientation="Horizontal">
    <TextBlock
        Width="90"
        VerticalAlignment="Center"
        Background="AliceBlue"
        Text="{Binding Context.Date, StringFormat={}{0:yyyy/MM/dd}, ElementName=TestDialog, Mode=OneWay}" />
    <TextBlock
        Width="300"
        Margin="10,0,0,0"
        VerticalAlignment="Center"
        Background="AliceBlue"
        Text="{Binding Context.Name, ElementName=TestDialog, Mode=OneWay}" />
</StackPanel>

<!-- MultiBindingを使うことで1つのTextBlockで2つの内容を表示することができる -->
<TextBlock
    Width="300"
    HorizontalAlignment="Left"
    VerticalAlignment="Center">
    <TextBlock.Text>
        <MultiBinding StringFormat="{}{0:yyyy/MM/dd}  {1}">
            <Binding
                ElementName="TestDialog"
                Mode="OneWay"
                Path="Context.Date" />
            <Binding
                ElementName="TestDialog"
                Mode="OneWay"
                Path="Context.Name" />
        </MultiBinding>
    </TextBlock.Text>
</TextBlock>
```

---

## WPFのコードビハインドを介したリソースへのアクセス

<https://www.it-swarm-ja.com/ja/c%23/wpf%E3%81%AE%E3%82%B3%E3%83%BC%E3%83%89%E3%83%93%E3%83%8F%E3%82%A4%E3%83%B3%E3%83%89%E3%82%92%E4%BB%8B%E3%81%97%E3%81%9F%E3%83%AA%E3%82%BD%E3%83%BC%E3%82%B9%E3%81%B8%E3%81%AE%E3%82%A2%E3%82%AF%E3%82%BB%E3%82%B9/968279150/>

コードビハインドで、背景色をResourceDictionaryから取得して設定する方法がわからなかったのでまとめた。  

``` C#:使用例
    // Application.Current.Resources["resourceName"]
   Amount.Background = (Brush)Application.Current.Resources["IsReadOnlyBackGroundColor"];
```

---

## XAMLでBoolを反転するには？

[bool を反転して Binding したかっただけなんだ](https://usagi.hatenablog.jp/entry/2018/12/05/211311)  

どう頑張っても、普通にやるならConverterが必要らしい。  
が、こんなことのためにあんな面倒くさいConverterを噛まさなければいけないのは絶対にいやだ。  

``` XML
<!-- なぜこれがデフォルトで出来ないのか -->
<CheckBox IsChecked="{Binding Awabi}"/>
<CheckBox IsEnabled="{Binding !Awabi}"/>
```

世の中には便利なライブラリがあるらしい。  
[Alex141/CalcBinding](https://github.com/Alex141/CalcBinding)  
Manage NuGet Packages から CalcBindingをインストールしてWindowに参照を追加することでほぼ理想的な書き方が実現できる。  

``` XML
<Window 
    xmlns:c="clr-namespace:CalcBinding;assembly=CalcBinding">

    <!-- かなり理想郷に近いぞ！！！ -->
    <CheckBox IsChecked="{Binding Awabi}"/>
    <CheckBox IsEnabled="{c:Binding !Awabi}"/>
</Window>
```

---

## 自分自身のItemsSourceのCountをXAML上で使用する方法

自分自身の要素が1件も無かったらDataTriggerでEnableをFalseにしたくて調べた。  
やっぱりそれなりに需要はあるみたいで、実現できたのでまとめる。  
ここら辺はRelativeSourceの話になってくるが、そっちでも1記事レベルなのでそれは別でまとめる。1  

[Bind Count of ItemsSource of an ItemsControl in a TextBlock using WPF](https://stackoverflow.com/questions/39482829/bind-count-of-itemssource-of-an-itemscontrol-in-a-textblock-using-wpf)  

``` XML
<c1:C1MultiSelect.Style>
    <Style TargetType="{x:Type c1:C1MultiSelect}">
        <Style.Triggers>
            <!-- C1MultiSelectは内部にItemsプロパティがあって、ItemsのクラスにはCountがある -->
            <!-- それをRelativeSource Selfを指定することでアクセスできるようになる模様 -->
            <DataTrigger Binding="{Binding RelativeSource={RelativeSource Self}, Path=Items.Count}" Value="0">
                <Setter Property="IsEnabled" Value="False" />
            </DataTrigger>
        </Style.Triggers>
        <Setter Property="IsEnabled" Value="true" />
    </Style>
</c1:C1MultiSelect.Style>
```

---

## DependencyProperyのSetterの値がNullになってしまう問題

[Why does my Dependency Property send null to my view model?](https://stackoverflow.com/questions/38958177/why-does-my-dependency-property-send-null-to-my-view-model)  
[MVVMのDataGridまたはListBoxからSelectedItemsにバインド](https://www.webdevqa.jp.net/ja/c%23/mvvm%E3%81%AEdatagrid%E3%81%BE%E3%81%9F%E3%81%AFlistbox%E3%81%8B%E3%82%89selecteditems%E3%81%AB%E3%83%90%E3%82%A4%E3%83%B3%E3%83%89/942024865/amp/)  

MultiSelectComboBoxのSelectedItemsの2WayBindingを実装している時に出くわした問題。  
コントロール側はIList,ViewModel側は`IEnumerable<T>`で実装していたのだが、コントロール側のSetterまでは値が入っているのに、ViewModelのSetterにはNullが入ってしまう現象に遭遇した。  
コントロール側の型とViewModel側の型を合わせれば値は届くが、2Wayにしたい以上、コントロールはIList,ViewModelはIEnumerableで受け取りたい願望がある。  
その線でいろいろ探してみたが、どうやら無理らしい。  

XAMLはジェネリックのバインドをサポートしてないっぽい。  
なので非ジェネリックIListとIListを同じ意味で使用はできない模様。  
どのサンプルでもObservableCollectionを使っていたり、イベントやビヘイビアで実現している。  
現状では、ジェネリックのバインディングは実現できない模様。  
実質的には疑似的な2wayどまりで、ViewModelからの通知はOneWayになってしまうだろう。  

対処法  
1.コントロールの型とViewModelの型を合わせる→IListで受け取ってOfTypeで変換して使う。  
2.ObservableCollectionのCollectionChangedイベントを観測する。  
3.イベントやビヘイビアで観測する。  

いろいろな実現方法もまとめておく  
[ListBoxやDataGridなどのItemsControlでSelectedItemsやIsSelectedをBindingする](https://qiita.com/mkuwan/items/7372b4b602fdabc3358c)  

---

## 同じ値が入力されても実行されるようにする

①変更をすべて取得。  
②フラグで値が変更されたかを管理。  
③LostFocusで検索実行。  
同じ値を許可しないといけないので、SetPropertyは使えない。  
キーバインドを全て取得して、いちいちフラグを管理する方法で何とかしたが、面倒くさくなるのは確か。  

Interaction.TriggersのEventTriggerでどんなイベントがあるのか調べた時の内容  
eventtrigger eventname 一覧  
[【WPF】【MVVM】GUIのマウス/キー操作処理をコードビハインドから駆逐する](https://qiita.com/hotelmoskva_/items/13ecc724bdad00078c16)  

大抵の場合はBindingの`UpdateSourceTrigger=PropertyChanged`に設定すれば、  
値が更新された瞬間にPropertyChangedが走るようになる。  
伝票入力のほうはこちらで実装していた。  
なぜ今回はこれが駄目だったのか。  
伝票入力の場合では、入力して検索した後、入力を消すので直前と同じ値を入力しても検索が走ったが、今回は仕様上、値が残ったままにしないといけなかった。  
値が入力されている状態で同じ値を入力しても、変更の観測はされるが、前回と同じなので検索は実行されない。  
だからこの方法は使えなくてKeyDownとフラグで制御した。  

[Bindingの各プロパティ(UpdateSourceTrigger,Delay,NotifyOnSourceUpdated)の動作について](https://qiita.com/furugen/items/6201f74cf5d5a823d92b)  
[[C# WPF]Bindingソースの更新タイミングを変える -UpdateSourceTrigger-](http://cswpf.seesaa.net/article/313839819.html)  

``` XML
    <im:GcNumber
        Name="RankCDBox"
        Width="70"
        HorizontalAlignment="Left"
        HorizontalContentAlignment="Center"
        Style="{StaticResource RankCD}"
        Value="{Binding Data.RankCD, Mode=TwoWay, TargetNullValue={x:Null}}">
        <i:Interaction.Triggers>
            <!-- ③LostFocusで検索実行 -->
            <i:EventTrigger EventName="LostFocus">
                <i:InvokeCommandAction Command="{Binding SearchRankAndWageCommand, Mode=OneWay}" />
            </i:EventTrigger>
            <!-- ①変更を全て取得 -->
            <i:EventTrigger EventName="KeyDown">
                <i:InvokeCommandAction Command="{Binding SetUpRankFlagCommand, Mode=OneWay}" />
            </i:EventTrigger>
        </i:Interaction.Triggers>
    </im:GcNumber>
```

``` C# : ViewModel
        // ②フラグで値が変更されたかを管理
        SetUpRankFlagCommand = new DelegateCommand(() => { IsChangedRankCD = true; }, () => !IsBusy);

        private void DataPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // ②フラグで値が変更されたかを管理。
            else if (e.PropertyName == nameof(Data.RankCD))
            {
                IsChangedRankCD = true;
            }
        }

        // ③LostFocusで検索実行
        SearchRankAndWageCommand = new DelegateCommand(async () => await SearchRankAndWage(), () => !IsBusy);
        /// <summary>
        /// ランクとランク賃金検索処理
        /// </summary>
        private async Task SearchRankAndWage()
        {
            if (IsChangedRankCD)
            {
                await ServiceErrorHandlingAsync(
                    async () =>
                    {
                        await SearchSimpleRankAsync();
                        await RankWageSearchImpl();
                    },
                    nameof(IsBusy)
                );
                IsChangedRankCD = false;
            }
        }
```

---

## MetroWindowでタイトルバーにボタンを配置する

ヘルプページを表示するときに採用した案。  
オプションの歯車を出すアレを?アイコンに置き換えてそれをヘルプページとした。  
しかし、意外にも出し方が分からなかったのでまとめ。  

[MahApps.Metroを使ってみた](https://sourcechord.hatenablog.com/entry/2016/02/23/012511)  

RidhtWindowCommandsを使う。  

``` xml
<Controls:MetroWindow
    x:Class="MahAppsMetroTest.MainWindow"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:Controls="clr-namespace:MahApps.Metro.Controls;assembly=MahApps.Metro"
    xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
    xmlns:local="clr-namespace:MahAppsMetroTest"
    xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
    Title="MainWindow"
    Width="500"
    Height="300"
    BorderThickness="0"
    GlowBrush="Black"
    mc:Ignorable="d">
    <Controls:MetroWindow.RightWindowCommands>
        <Controls:WindowCommands>
            <Button Content="settings" />
            <Button Content="acount" />
        </Controls:WindowCommands>
    </Controls:MetroWindow.RightWindowCommands>
    以下略
```

![!](http://cdn-ak.f.st-hatena.com/images/fotolife/m/minami_SC/20160223/20160223003354.png)  

---

## 2つテキストブロックを左右に分けて左側だけリサイズできるようにしたい

FlexGridで1つのセルにテキストを2つ配置して左右にわけて、左側の大きさだけが変わって、片方はそのままって機能を実装したかったんだけど、全然できなかったので色々調べた。  
DataGridでは再現できたので、それをFlexGridにも当てたら最終的にできた。  
できなかった原因はColumnのTextAlighnにLeftが設定されていたから。  
これを外したらあっさりできた。  

[WPFにおけるGUI構築講座 -座標ベタ書きから脱却しよう-](https://qiita.com/YSRKEN/items/686068a359866f21f956)
[WPF DataGridへのBindingに関する基本設計](https://oita.oika.me/2014/11/03/wpf-datagrid-binding/)  
[DataGridTemplateColumn内のTextBoxのフォーカスについて](https://social.msdn.microsoft.com/Forums/ja-JP/cc028d67-7ed6-406e-99a9-4c876a06647c/datagridtemplatecolumn2086912398textbox12398125011245712540124591247312395?forum=wpfja)  

``` XML : DataGridで試したやりかた
<DataGrid>
    <DataGrid.Columns>
        <!-- Grid案 ○ -->
        <!-- DataGridでセルテンつかってもいけた -->
        <DataGridTemplateColumn Width="80">
            <DataGridTemplateColumn.CellTemplate>
                <DataTemplate>
                    <Grid>
                        <Grid.ColumnDefinitions>
                            <ColumnDefinition Width="*" />
                            <ColumnDefinition Width="Auto" />
                        </Grid.ColumnDefinitions>
                        <TextBlock
                            Grid.Column="0"
                            Text="{Binding Name}"
                            ToolTip="test" />
                        <TextBlock Grid.Column="1" Text="{Binding No}" />
                    </Grid>
                </DataTemplate>
            </DataGridTemplateColumn.CellTemplate>
        </DataGridTemplateColumn>
        <!-- TextBlockの中にTextBlockを入れる案 × -->
        <!-- これだと駄目だった -->
        <DataGridTemplateColumn Width="80">
            <DataGridTemplateColumn.CellTemplate>
                <DataTemplate>
                    <TextBlock
                        HorizontalAlignment="Left"
                        Text="{Binding Name}"
                        ToolTip="test">
                        <TextBlock HorizontalAlignment="Right" Text="{Binding No}" />
                    </TextBlock>
                </DataTemplate>
            </DataGridTemplateColumn.CellTemplate>
        </DataGridTemplateColumn>
    </DataGrid.Columns>
</DataGrid>
```

``` XML : 実際にGridに当てたときの書き方
    <!-- 自動調整あり 左側が大きくなるパターン -->
    <Grid>
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width="*" />
            <ColumnDefinition Width="Auto" />
        </Grid.ColumnDefinitions>
        <TextBlock
            Grid.Column="0"
            Focusable="False"
            Text="{Binding Player1Name, Mode=OneWay}"
            ToolTip="{Binding Player1Remarks, Mode=OneWay}" />
        <TextBlock
            Grid.Column="1"
            Focusable="False"
            Text="*"
            Visibility="{Binding Player1Remarks, Mode=OneWay, Converter={StaticResource StringToVisibilityConverter}}" />
    </Grid>

    <!-- 多分これでもうまくいくだろうパターン自動調整ないのでほぼ使う意味ない-->
     <StackPanel Orientation="Horizontal">
        <StackPanel HorizontalAlignment="Left" Orientation="Horizontal">
            <TextBlock/>
        </StackPanel>
        <StackPanel HorizontalAlignment="Right" Orientation="Horizontal">
            <TextBlock/>
        </StackPanel>
     </StackPanel>
```

---

## ツールチップ開発の色々

マウスオーバー
プレースホルダー
ポップアップ
ToolTip

[ボタンのツールチップ](https://araramistudio.jimdo.com/2019/11/01/c-%E3%81%AEwpf%E3%81%A7%E3%83%84%E3%83%BC%E3%83%AB%E3%83%81%E3%83%83%E3%83%97%E3%82%92%E8%A1%A8%E7%A4%BA%E3%81%99%E3%82%8B/)  
>ボタンなどのコントロールにマウスを載せた時、説明やヒントをポップアップ表示してくれる機能です。  

[C#のWPFでツールチップを表示する](https://araramistudio.jimdo.com/2019/11/01/c-%E3%81%AEwpf%E3%81%A7%E3%83%84%E3%83%BC%E3%83%AB%E3%83%81%E3%83%83%E3%83%97%E3%82%92%E8%A1%A8%E7%A4%BA%E3%81%99%E3%82%8B/)  
[wpf : ToolTipの幅](http://pieceofnostalgy.blogspot.com/2013/05/wpf-tooltip.html)
[ToolTip Style ToolTipService.ShowDuration](https://stackoverflow.com/questions/32288529/tooltip-style-tooltipservice-showduration)
[キーを指定したスタイルの使い方参考](https://qiita.com/tera1707/items/cb8ad4c40107ae25b565)

[【WPF】無効なコントロールにツールチップを表示させる方法](https://threeshark3.com/show-on-disabled/)  
>ToolTipService.ShowOnDisabledをTrueにする  

``` XML
<!-- キーを定義 -->
<DockPanel.Resources>
    <Style x:Key="ToolTipTextBlock" TargetType="TextBlock">
        <Setter Property="LayoutTransform">
            <Setter.Value>
                <ScaleTransform 
                ScaleX="{Binding RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type metro:MetroWindow}}, Path=DataContext.Magnification, Mode=TwoWay}" 
                ScaleY="{Binding RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type metro:MetroWindow}}, Path=DataContext.Magnification, Mode=TwoWay}" />
            </Setter.Value>
        </Setter>
    </Style>
</DockPanel.Resources>

<!-- 定義したキーを使う場合 -->
<c1:Column.CellTemplate>
    <DataTemplate>
        <Grid ToolTipService.IsEnabled="{Binding Player1Remarks, Mode=OneWay, Converter={StaticResource NotNullOrEmptyToBoolConverter}}">
            <Grid.ColumnDefinitions>
                <ColumnDefinition Width="*" />
                <ColumnDefinition Width="Auto" />
            </Grid.ColumnDefinitions>
            <ToolTipService.ToolTip>
                <TextBlock Style="{DynamicResource ToolTipTextBlock}" Text="{Binding Player1Remarks, Mode=OneWay}" />
            </ToolTipService.ToolTip>
            <TextBlock
                Grid.Column="0"
                Focusable="False"
                Text="{Binding Player1Name, Mode=OneWay}"
                TextWrapping="Wrap" />
            <TextBlock
                Grid.Column="1"
                Margin="0,0,5,0"
                Focusable="False"
                Text="※"
                Visibility="{Binding Player1Remarks, Mode=OneWay, Converter={StaticResource StringToVisibilityConverter}}" />
        </Grid>
    </DataTemplate>
</c1:Column.CellTemplate>

<!-- 試行錯誤のあと -->
                            <Setter Property="LayoutTransform" TargetName="grid">
                                <Setter.Value>
                                    <TransformGroup>
                                        <ScaleTransform ScaleX="1" ScaleY="1" />
                                        <SkewTransform AngleX="0" AngleY="0" />
                                        <RotateTransform Angle="90" />
                                        <TranslateTransform X="0" Y="0" />
                                    </TransformGroup>
                                </Setter.Value>
                            </Setter>

<!--<Style.Resources>
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
</Style.Resources>-->

```

---

## StaticResourceが使えない

[複数の画面で定義を共有したいとき](https://anderson02.com/cs/wpf/wpf-6/)

>その場合はWPFのプロジェクトを作成したときに自動生成されるApp.xamlファイルに記述することで、プロジェクト内のすべての画面から参照することが可能です。  
>App.xamlファイルのApplication.Resourcesエリアにリソースを定義します。  

StaticResourceを使いたかったらApp.xamlの`<Application.Resources>`要素に`<ResourceDictionary>`をだらっと追加しないといけない模様。

---

## MultiBindingで単純に2つの要素をつなげる例

[](https://riptutorial.com/wpf/example/23413/binding-multiple-values-with-a-multibinding)  

``` XML
<TextBlock>
    <TextBlock.Text>
        <MultiBinding StringFormat="{}{0} {1}">
            <Binding Path="User.Forename"/>
            <Binding Path="User.Surname"/>
        </MultiBinding>
    </TextBlock.Text>
</TextBlock>
```

---

## DataTemplateをResourceで定義する方法

[How to create a static resource of DataTemplate?](https://social.msdn.microsoft.com/Forums/vstudio/en-US/0de6f454-bcde-4aa9-843d-ead2ad9d6d61/how-to-create-a-static-resource-of-datatemplate?forum=wpf)  

``` XML
<Window>
  <Window.Resources>
    <DataTemplate x:Key="Header">
      <TextBlock Foreground="Orange" Background="Yellow" Text="{Binding}"/>
    </DataTemplate>
    <DataTemplate x:Key="Name">
      <TextBlock Foreground="White" Background="Black" Text="{Binding Name}"/>
    </DataTemplate>
    <DataTemplate x:Key="Age">
      <TextBlock Foreground="Red" Background="Green" Text="{Binding Age}"/>
    </DataTemplate>
    <x:Array x:Key="Office" Type="{x:Type local:Person}">
      <local:Person Name="Michael" Age="40"/>
      <local:Person Name="Jim" Age="30"/>
      <local:Person Name="Dwight" Age="30"/>
    </x:Array>
  </Window.Resources>
  <ListView ItemsSource="{Binding Source={StaticResource Office}}">
    <ListView.View>
      <GridView>
        <GridView.Columns>
          <GridViewColumn Width="140" Header="Column 1" HeaderTemplate="{StaticResource Header}" CellTemplate="{StaticResource Name}"/>
          <GridViewColumn Width="140" Header="Column 2" HeaderTemplate="{StaticResource Header}" CellTemplate="{StaticResource Age}"/>
        </GridView.Columns>
      </GridView>
    </ListView.View>
  </ListView>
</Window>
```

---

## XAMLで定義したデータをバインドする方法

[ListBoxコントロールにデータをバインディングする - XAMLのXMLをバインディングする (WPFプログラミング)](https://www.ipentec.com/document/csharp-wpf-listbox-data-bind-from-xaml-xml)  

ItemsControl研究で発見した。  
MVVMバインドさせるのが面倒な時にいいかも。  

``` XML : StaticResourceとして指定する方法
    <Grid>
        <Grid.Resources>
            <!-- バインドデータを定義 -->
            <XmlDataProvider x:Key="BlogData" XPath="Blogs/Blog">
                <x:XData>
                    <Blogs xmlns="">
                        <Blog>
                            <BlogSite>simplegeek.com</BlogSite>
                            <Blogger OnlineStatus="Offline">Chris Anderson</Blogger>
                        </Blog>
                        <Blog>
                            <BlogSite>fortes.com</BlogSite>
                            <Blogger OnlineStatus="Offline">Fil Fortes</Blogger>
                        </Blog>
                        <Blog>
                            <BlogSite>Longhorn Blogs</BlogSite>
                            <Blogger OnlineStatus="Online">Rob Relyea</Blogger>
                        </Blog>
                    </Blogs>
                </x:XData>
            </XmlDataProvider>
            <!-- XmlDataProviderで定義した内容をコントロールにバインドする場合 -->
            <DataTemplate x:Key="BlogDataTemplate">
                <Grid TextBlock.FontSize="20">
                    <TextBlock Text="{Binding XPath=Blogger}" />
                    <TextBlock Text="{Binding XPath=BlogSite}" />
                    <!-- XPath指定は必要な模様。 -->
                    <!-- Blogger要素にあるOnlineStatusをバインドする場合は/@でアクセス可能な模様 -->
                    <TextBlock Text="{Binding XPath=Blogger/@OnlineStatus}" />
                </Grid>
            </DataTemplate>
        </Grid.Resources>

        <ListBox
            ItemTemplate="{StaticResource BlogDataTemplate}"
            ItemsSource="{Binding Source={StaticResource BlogData}}"/>
    </Grid>
```

直接指定もできなくはないが、基本的にリソースに定義することをおすすめする。  

``` XML : 直接指定する方法
<ListBox>
    <ListBox.ItemsSource>
        <Binding>
            <Binding.Source>
                <XmlDataProvider XPath="Blogs/Blog">
                    <x:XData>
                        <Blogs xmlns="">
                            <Blog>
                                <BlogSite>simplegeek.com</BlogSite>
                                <Blogger OnlineStatus="Offline">Chris Anderson</Blogger>
                                <Url>http://simplegeek.com</Url>
                            </Blog>
                            <Blog>
                                <BlogSite>fortes.com</BlogSite>
                                <Blogger OnlineStatus="Offline">Fil Fortes</Blogger>
                                <Url>http://fortes.com/work</Url>
                            </Blog>
                            <Blog>
                                <BlogSite>Longhorn Blogs</BlogSite>
                                <Blogger OnlineStatus="Online">Rob Relyea</Blogger>
                                <Url>http://www.longhornblogs.com/rrelyea/</Url>
                            </Blog>
                        </Blogs>
                    </x:XData>
                </XmlDataProvider>
            </Binding.Source>
        </Binding>
    </ListBox.ItemsSource>
</ListBox>
```

DataTemplateをResourceで定義する方法でもまとめた内容だが、全体的によくできているのでそのまま持ってくる

``` XML : Arrayを使用した場合
<Window>
  <Window.Resources>
    <DataTemplate x:Key="Header">
      <TextBlock Foreground="Orange" Background="Yellow" Text="{Binding}"/>
    </DataTemplate>
    <DataTemplate x:Key="Name">
      <TextBlock Foreground="White" Background="Black" Text="{Binding Name}"/>
    </DataTemplate>
    <DataTemplate x:Key="Age">
      <TextBlock Foreground="Red" Background="Green" Text="{Binding Age}"/>
    </DataTemplate>
    <x:Array x:Key="Office" Type="{x:Type local:Person}">
      <local:Person Name="Michael" Age="40"/>
      <local:Person Name="Jim" Age="30"/>
      <local:Person Name="Dwight" Age="30"/>
    </x:Array>
  </Window.Resources>

  <ListView ItemsSource="{Binding Source={StaticResource Office}}">
    <ListView.View>
      <GridView>
        <GridView.Columns>
          <GridViewColumn Width="140" Header="Column 1" HeaderTemplate="{StaticResource Header}" CellTemplate="{StaticResource Name}"/>
          <GridViewColumn Width="140" Header="Column 2" HeaderTemplate="{StaticResource Header}" CellTemplate="{StaticResource Age}"/>
        </GridView.Columns>
      </GridView>
    </ListView.View>
  </ListView>
</Window>
```

---

## 

[【C#/WPF】EventTriggerを使って、Buttonでなくてもクリック時のCommandをかけるようにする](https://qiita.com/tera1707/items/7ecde6e97a19437cbf72)

Interaction.Triggersを使うまでに必要なこと

- System.Windows.Interactivity.dllを参照に追加  
- Microsoft.Expression.Interactionsを参照に追加  
- xmlns:i="http://schemas.microsoft.com/expression/2010/interactivity" xamlの上に追加  

xaml コントロールにフォーカス
[[C#][WPF]ViewModel側からコントロールのフォーカスを指定する方法](https://alfort.online/827)  

これができないとかぬかしやがる。
マジでふざけてる。
死ねXAML

・エンターキーを検知する方法
・XAML上で別のコントロールにフォーカスさせる方法

xaml focus method
CallMethodAction FocusManager.FocusedElement
WPFEventTrigger set focus
EventTrigger 
https://stackoverflow.com/questions/3870214/eventtrigger-with-setter-in-wpf
interractiontritter datatrriger
https://www.infragistics.com/community/forums/f/ultimate-ui-for-wpf/25409/binding-content-of-another-field-using-datatrigger

[How to Set Focus on a TextBox Using Triggers in XAML](https://spin.atomicobject.com/2013/03/06/xaml-wpf-textbox-focus/)  
[C#/WPF]ビューモデルからビューのメソッドを呼ぶ代わりに、EventTriggerでMouseDown等のイベントを拾ってビューのメソッドを呼ぶ](https://qiita.com/tera1707/items/d184c85d0c181e6563ea)  

[Invoke Command When "ENTER" Key Is Pressed In XAML](https://stackoverflow.com/questions/4834227/invoke-command-when-enter-key-is-pressed-in-xaml)  
eventtrigger enter key
[WPF+LivetのMVVMで、TextBoxでEnterキーが押された時のイベントを指定したい](https://teratail.com/questions/296691)  
[C# WPF-Tips-TextBox-EnterでCommand実行](https://dasuma20.hatenablog.com/entry/cs/wpf/tips/textbox-enter-command)  
[【C#/WPF】EventTriggerを使って、Buttonでなくてもクリック時のCommandをかけるようにする](https://qiita.com/tera1707/items/7ecde6e97a19437cbf72)  

``` xml
            <i:Interaction.Triggers>
                <i1:KeyTrigger Key="Enter" FocusManager.FocusedElement="{Binding ElementName=TextBox2}">
                    <i1:CallMethodAction MethodName="Focus" TargetObject="{Binding ElementName=TextBox2}" />
                    <!--<i1:CallMethodAction FocusManager.FocusedElement="{Binding ElementName=TextBox2}" />-->

                </i1:KeyTrigger>
                <!--<Setter Property="FocusManager.FocusedElement" Value="{Binding ElementName=TextBox1}" />-->
                <!--<i1:CallMethodAction MethodName="Focus" TargetObject="{Binding ElementName=TextBox2}" />-->

            </i:Interaction.Triggers>
```

``` xml
<Window
    x:Class="WpfApp4.MainWindow"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
    xmlns:i="http://schemas.microsoft.com/expression/2010/interactivity"
    xmlns:i1="http://schemas.microsoft.com/expression/2010/interactions"
    xmlns:local="clr-namespace:WpfApp4"
    xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
    Title="MainWindow"
    Width="483"
    Height="268"
    FocusManager.FocusedElement="{Binding ElementName=TextBox2}"
    mc:Ignorable="d">
    <Grid>
        <TextBox
            Width="172"
            Height="23"
            Margin="83,75,0,0"
            HorizontalAlignment="Left"
            VerticalAlignment="Top"
            Text="TextBox"
            TextWrapping="Wrap">
            <i:Interaction.Triggers>
                <i1:KeyTrigger Key="Enter" FocusManager.FocusedElement="{Binding ElementName=TextBox2}">
                    <i1:CallMethodAction MethodName="Focus" TargetObject="{Binding ElementName=TextBox2}" />
                    <!--<i1:CallMethodAction FocusManager.FocusedElement="{Binding ElementName=TextBox2}" />-->

                </i1:KeyTrigger>
                <!--<Setter Property="FocusManager.FocusedElement" Value="{Binding ElementName=TextBox1}" />-->
                <!--<i1:CallMethodAction MethodName="Focus" TargetObject="{Binding ElementName=TextBox2}" />-->

            </i:Interaction.Triggers>
            <!--<TextBox.Style>
                <Style>
                    <Style.Triggers>
                        <EventTrigger RoutedEvent="MouseEnter">
                            <Setter Property="FocusManager.FocusedElement" Value="{Binding ElementName=CodeDigit2}" />
            -->
            <!--<EventTrigger.Actions>
                                <BeginStoryboard>
                                    <Storyboard>
                                        <DoubleAnimation
                                            Storyboard.TargetProperty="FontSize"
                                            To="28"
                                            Duration="0:0:0.300" />
                                    </Storyboard>
                                </BeginStoryboard>
                            </EventTrigger.Actions>-->
            <!--
                        </EventTrigger>
            -->
            <!--<Trigger Property="" Value="True">
                            <Setter Property="Background" Value="Blue" />
                        </Trigger>
                        <DataTrigger Binding="{Binding ElementName=CodeDigit1, Path=Text.Length}" Value="1">
                            <Setter Property="FocusManager.FocusedElement" Value="{Binding ElementName=CodeDigit2}" />
                        </DataTrigger>-->
            <!--
                    </Style.Triggers>
                </Style>
            </TextBox.Style>-->
            <TextBox.Style>
                <Style TargetType="TextBox">
                    <Style.Triggers>
                        <Trigger Property="IsMouseOver" Value="True">
                            <Setter Property="FocusManager.FocusedElement" Value="{Binding ElementName=TextBox2}" />
                        </Trigger>
                        <!--<EventTrigger RoutedEvent="PreviewMouseLeftButtonUp">
                            <EventTrigger.EnterActions>
                                <BeginStoryboard>
                                    <Storyboard>
                                        <Setter Property="FocusManager.FocusedElement" Value="{Binding ElementName=TextBox2}" />
                                    </Storyboard>
                                </BeginStoryboard>

                            </EventTrigger.EnterActions>
                        </EventTrigger>-->
                    </Style.Triggers>
                </Style>
            </TextBox.Style>
        </TextBox>
        <TextBox
            Name="TextBox2"
            Width="172"
            Height="23"
            Margin="83,103,0,0"
            HorizontalAlignment="Left"
            VerticalAlignment="Top"
            Text="TextBox"
            TextWrapping="Wrap" />
    </Grid>
</Window>

```

---

## 縦横の大きさを動的に変動させる方法

[WPF ScrollViewer with Grid inside and dynamic size](https://stackoverflow.com/questions/48529723/wpf-scrollviewer-with-grid-inside-and-dynamic-size)  

FlexGridやScrollViewer等、コントロールの大きさが画面の大きさと連動して欲しい時にどのように書けばいいのか、テンプレートとなる知識がなかったのでまとめ。  
やっぱりGrid先輩が強すぎる。  
とりあえずGridで囲んでおけみたいなところあるな。  

``` XML
<Window x:Class="WpfTest.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Width="200"
        Height="300">

  <!-- Layout Root -->
  <Grid>
    <Grid.RowDefinitions>
      <RowDefinition Height="Auto" />
      <RowDefinition Height="*" />
      <RowDefinition Height="Auto" />
    </Grid.RowDefinitions>

    <!-- Header Panel -->
    <Border Grid.Row="0" Background="#CCCCCC" Padding="11">
      <!-- Replace this TextBlock with your header content. -->
      <TextBlock Text="Header Content" TextAlignment="Center" />
    </Border>

    <!-- Body Panel -->
    <Grid Grid.Row="1" Background="#CCCCFF">
      <Grid.RowDefinitions>
        <RowDefinition Height="Auto" />
        <RowDefinition Height="*" />
      </Grid.RowDefinitions>

      <Border Grid.Row="0" Background="#FFCCCC" Padding="11">
        <!-- Replace this TextBlock with your upper body content. -->
        <TextBlock Text="Upper Body Content" TextAlignment="Center" />
      </Border>

      <ScrollViewer Grid.Row="1" Padding="11"
                    VerticalScrollBarVisibility="Auto">

        <!-- Replace this Border with your scrollable content. -->
        <Border MinHeight="200">
          <Border.Background>
            <RadialGradientBrush RadiusY="1" RadiusX="1" Center="0.5,0.5">
              <GradientStop Color="White" Offset="0" />
              <GradientStop Color="Black" Offset="1" />
            </RadialGradientBrush>
          </Border.Background>
        </Border>

      </ScrollViewer>
    </Grid>

    <!-- Footer Panel -->
    <Border Grid.Row="2" Background="#CCFFCC" Padding="11">
      <!-- Replace this TextBlock with your footer content. -->
      <TextBlock Text="Footer Content" TextAlignment="Center" />
    </Border>
  </Grid>

</Window>
```
