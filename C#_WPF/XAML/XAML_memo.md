# XAMLまとめ

---

## 参考リンク集

[WPFのソースコード](https://github.com/dotnet/wpf)  
[コントロールのスタイルとテンプレート](http://msdn.microsoft.com/ja-jp/library/aa970773(v=vs.110).aspx)  
[WPFのコントロール一覧](https://water2litter.net/rye/post/c_control_list/#my_mokuji10)  

[kazuki_WPF入門](https://blog.okazuki.jp/entry/2014/12/27/200015)  
[WPF4.5入門](https://www.slideshare.net/okazuki0130/wpf45-38048141)  

[DataGridView編メニュー](https://dobon.net/vb/dotnet/datagridview/)  
[Windows Presentation Foundation実践](http://kisuke0303.sakura.ne.jp/blog/wordpress/wp-content/uploads/2016/08/4843696230c1698ad8ff7d086b998344.pdf)

[[C# WPF] なんとかしてWPFの描画を速くしたい「Canvas.Childrenへのオブジェクト追加」](https://www.peliphilo.net/archives/2390)
[WPF で ENTER キーを押したらフォーカス移動するようにする](https://rksoftware.wordpress.com/2016/05/04/001-14/)

[【WPF】【MVVM】GUIのマウス/キー操作処理をコードビハインドから駆逐する](https://qiita.com/hotelmoskva_/items/13ecc724bdad00078c16)  
[ダブルクリックイベントを持っていないコントロールで判定を拾う](https://www.hos.co.jp/blog/20200331/)  
[WPFのRelativeSourceのはなし](https://hidari-lab.hatenablog.com/entry/wpf_relativesource_self_and_findancestor)  

---

## WPFでコンソールログを出力する

``` C#
using System.Diagnostics;
Trace.WriteLine("text");
```

[No output to console from a WPF application?](https://stackoverflow.com/questions/160587/no-output-to-console-from-a-wpf-application)  

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

## WPFのコードビハインドを介したリソースへのアクセス

<https://www.it-swarm-ja.com/ja/c%23/wpf%E3%81%AE%E3%82%B3%E3%83%BC%E3%83%89%E3%83%93%E3%83%8F%E3%82%A4%E3%83%B3%E3%83%89%E3%82%92%E4%BB%8B%E3%81%97%E3%81%9F%E3%83%AA%E3%82%BD%E3%83%BC%E3%82%B9%E3%81%B8%E3%81%AE%E3%82%A2%E3%82%AF%E3%82%BB%E3%82%B9/968279150/>

コードビハインドで、背景色をResourceDictionaryから取得して設定する方法がわからなかったのでまとめた。  

``` C#:使用例
    // Application.Current.Resources["resourceName"]
   Amount.Background = (Brush)Application.Current.Resources["IsReadOnlyBackGroundColor"];
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

## StaticResourceが使えない

>その場合はWPFのプロジェクトを作成したときに自動生成されるApp.xamlファイルに記述することで、プロジェクト内のすべての画面から参照することが可能です。  
>App.xamlファイルのApplication.Resourcesエリアにリソースを定義します。  
[複数の画面で定義を共有したいとき](https://anderson02.com/cs/wpf/wpf-6/)

StaticResourceを使いたかったらApp.xamlの`<Application.Resources>`要素に`<ResourceDictionary>`をだらっと追加しないといけない模様。

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

## DataTrigger + Enum

DataTriggerでEnum使うときって、ValueにそのままEnumの文字列を入れても認識されるらしい。  
厳密に指定するなら`x:Static`と名前空間からEnumを参照することでアクセス可能。  

``` C#
public enum State
{
    Normal,
    Warning,
    Error
}
```

``` xml : 文字列で直接指定
    <Button Command="{Binding ButtonCommand}">
        <Button.Style>
            <Style TargetType="{x:Type Button}">
                <Style.Triggers>
                    <DataTrigger Binding="{Binding State}" Value="Normal">
                        <Setter Property="Background" Value="Blue" />
                    </DataTrigger>
                    <DataTrigger Binding="{Binding State}" Value="Warning">
                        <Setter Property="Background" Value="Yellow" />
                    </DataTrigger>
                    <DataTrigger Binding="{Binding State}" Value="Error">
                        <Setter Property="Background" Value="Red" />
                    </DataTrigger>
                </Style.Triggers>
            </Style>
        </Button.Style>
    </Button>
```

``` xml : x:Staticによる指定
<window xmlns:localenum="clr-namespace:Wpf_DataTrigger_Enum.Enum">

    <Button Command="{Binding ButtonCommand}">
        <Button.Style>
            <Style TargetType="{x:Type Button}">
                <Style.Triggers>
                    <DataTrigger Binding="{Binding State}" Value="{x:Static localenum:State.Normal}">
                        <Setter Property="Background" Value="Blue" />
                    </DataTrigger>
                    <DataTrigger Binding="{Binding State}" Value="{x:Static localenum:State.Warning}">
                        <Setter Property="Background" Value="Yellow" />
                    </DataTrigger>
                    <DataTrigger Binding="{Binding State}" Value="{x:Static localenum:State.Error}">
                        <Setter Property="Background" Value="Red" />
                    </DataTrigger>
                </Style.Triggers>
            </Style>
        </Button.Style>
    </Button>
```

[datatrigger on enum to change image](https://stackoverflow.com/questions/13917033/datatrigger-on-enum-to-change-image)  

---

## ViewModelからViewModelへの通知

1. Modelを介して行う。  
2. PrismのEventAggregatorを使う。  

モデルを介する場合のイメージはこう。  
この場合、容易に他のViewへの操作を依頼できるが、Modelに依存してしまう。  

``` txt
ViewModel  ViewModel
   ↓         ↑
       Model
```

依存の問題を解決する仕組みがPublisher/Subscriberパターンであり、それをPrismで実装したものがEventAggregator。  
そちらでの実装方法は「Prism.EventAggregator」でまとめているので、そちらを参照されたし。  

[C# WPFアプリ MVVMモデル内の連携方法](https://setonaikai1982.com/mvvm-comm/)  

---

## 参照 'RelativeSource FindAncestor'を使用したバインディングのソースが見つかりません

ContextMenuからFindAncestorがうまく行かない。  
そもそもRelativeSourceでFindAncestorした時、Bindのタイミングが遅かったりしてうまくいかないことが多い印象。  
そのための機能としてBindingProxyなる方法が用意されている模様。  

### 1. BindingProxyを使う方法

やってみたが、これだとインスタンスが生成されていない状態で実行されてエラーとなってしまった。  
これはこれで生成タイミングが早すぎるのかもしれない。  

``` XML
<metro:MetroWindow.Resources>
    <data:BindingProxy x:Key="Proxy" Data="{Binding RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type metro:MetroWindow}}, Path=DataContext, Mode=OneWay}" />
</metro:MetroWindow.Resources>

<MenuItem
    Command="{Binding Source={StaticResource Proxy}, Path=Data.DeleteUseTaxDetailCommand}"
    CommandParameter="{Binding Source={StaticResource Proxy}, Path=Data.UseTaxDetailSelectedItem, Mode=TwoWay}"
    Header="{Binding Source={StaticResource Proxy}, Path=Data.UseTaxDetailSelectedIndex, Mode=TwoWay, TargetNullValue={x:Static sys:String.Empty}, Converter={StaticResource AdditionConverter}, ConverterParameter=1}"
    HeaderStringFormat="{}{0:N0} 行目を削除" />
```

[参照 'RelativeSource FindAncestor'を使用したバインディングのソースが見つかりません](https://www.web-dev-qa-db-ja.com/ja/wpf/%E5%8F%82%E7%85%A7-%27relativesource-findancestor%27%E3%82%92%E4%BD%BF%E7%94%A8%E3%81%97%E3%81%9F%E3%83%90%E3%82%A4%E3%83%B3%E3%83%87%E3%82%A3%E3%83%B3%E3%82%B0%E3%81%AE%E3%82%BD%E3%83%BC%E3%82%B9%E3%81%8C%E8%A6%8B%E3%81%A4%E3%81%8B%E3%82%8A%E3%81%BE%E3%81%9B%E3%82%93/1072766554/)  

### 2. PlacementTargetプロパティを使う方法

結局こっちに落ち着いた。  
でもって何行目を削除って出さないだけでだいぶ機能的に楽になったのでそれも辞めた。  
インデックスなんて内部でわかってるんだから別に渡す必要なんてなかったんだ。  

``` XML
        <!-- <ResourceDictionary>
            <Style
                x:Key="RightClickMenuItem"
                BasedOn="{StaticResource {x:Type MenuItem}}"
                TargetType="{x:Type MenuItem}">
                <Setter Property="Command" Value="{Binding DeleteUseTaxDetailCommand}" />
                <Setter Property="CommandParameter" Value="{Binding UseTaxDetailSelectedItem, Mode=TwoWay}" />
                <Setter Property="Header" Value="{Binding UseTaxDetailSelectedIndex, Mode=TwoWay, TargetNullValue={x:Static sys:String.Empty}, Converter={StaticResource AdditionConverter}, ConverterParameter=1}" />
                <Setter Property="HeaderStringFormat" Value="{}{0:N0} 行目を削除" />
            </Style>
        </ResourceDictionary>-->
```

[【WPF】ContextMenuからFindAncestorする方法](https://threeshark3.com/contextmenu-findancestor/)  

---
