# XAMLまとめ

## XAMLでNULLの指定の仕方

``` XML
    <!-- "{x:Null}"って指定する -->
    <Setter Property="FocusVisualStyle" Value="{x:Null}" />
```

---

## x:Typeのありなしの違い

[when to use {x:Type …}?](https://stackoverflow.com/questions/11167536/when-to-use-xtype)  

x:Type マークアップ拡張機能には、C# の typeof() 演算子や Microsoft Visual Basic の GetType 演算子に似た関数があります。

x:Type マークアップ拡張機能は、Type 型を受け取るプロパティに対して、文字列変換動作を提供します。

---

## イベントをViewModel側で観測する方法

割り勘のマルチセレクトコントロールで選択した値を観測するのに難儀したのでまとめ  
・SelectedItemsはDependencyProperyに登録されていないのでXAMLのマークアップ上ではバインド不可  
・他のSelected系のプロパティでどうにかならないかやってみたが無理。  
・公式を見て方法を探すが、まともな文献が1件しかなくて、しかも面倒くさいやつしかない。  
・SelectionChangedイベントなるものはあるので、コードビハインドで実装して動作を確かめてみたら複数の値を観測できることを確認  
・イベントをDelegateCommandを使ってViemModel側で観測できないかやってみる  
・行けたし、値も観測できたので勝ち  

てか、複数選択されるんだからSelectedItemsのプロパティくらい登録しておけって思うんだけどな。  

Trigger関係はPrismの機能らしい。  

``` XML
<!-- i:Interaction.Triggers、i:EventTrigge、i:InvokeCommandActionの順に設定する -->
<i:Interaction.Triggers>
    <i:EventTrigger EventName="SelectionChanged">
        <i:InvokeCommandAction Command="{Binding TestSelectedChanged, Mode=OneWay}" PassEventArgsToCommand="True" />
    </i:EventTrigger>
</i:Interaction.Triggers>
<!-- 
EventArgsParameterPath
EventArgsParameterPathプロパティにはListViewが持つプロパティのうち、Commandに渡したいプロパティの名前を指定します。
この例では選択されたアイテムが格納されるSelectedItemを指定します。
EventNameもEventArgsParameterPathも、指定した文字列のイベントやプロパティをPrismが探して使用してくれます。

今回の例では、あると逆にエラーになってしまうので外した。
EventArgsの中だけで十分な情報が入っているので、今回は使わないことにする。

PassEventArgsToCommand
PassEventArgsToCommandはEventArgsをViewModelに渡してくれるオプション
 -->
```

---

## ViewModelからコントロールのメソッドを実行する方法

[ビューモデルからビューのメソッドを呼ぶ](https://qiita.com/tera1707/items/d184c85d0c181e6563ea)
CallMethodActionという、まさしくな仕組みがあったのでまとめ。  

``` XML : View
<i:Interaction.Triggers>
    <l:InteractionMessageTrigger MessageKey="SubjectLargeTypeListUnselectAll" Messenger="{Binding Messenger}">
        <i:CallMethodAction MethodName="UnselectAll" />
    </l:InteractionMessageTrigger>
</i:Interaction.Triggers>
```

``` C# : ViewModel
    /// <summary>
    /// 科目大区分一覧の選択をすべて解除します。
    /// </summary>
    private void SubjectLargeTypeListUnselectAll()
    {
        Messenger.Raise(new InteractionMessage("SubjectLargeTypeListUnselectAll"));
    }
```

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

## DataGrid – アクティブセルの枠線を消す（C# WPF)

<http://once-and-only.com/programing/c/datagrid-%E3%82%A2%E3%82%AF%E3%83%86%E3%82%A3%E3%83%96%E3%82%BB%E3%83%AB%E3%81%AE%E6%9E%A0%E7%B7%9A%E3%82%92%E6%B6%88%E3%81%99%EF%BC%88c-wpf/>  

支払方法変更処理の単体テスト戻りでScrollViewerにフォーカスが当たって点線の枠が表示されてしまう問題が発生した。  
この点線をどう表現して調べたらいいかわからなかったところ、ドンピシャな記事があったので、備忘録として残す。  
因みに「wpf scrollviewer　点線」で調べた模様。  

``` XML
<DataGrid.CellStyle>
    <Style TargetType="DataGridCell">
        <Setter Property="BorderThickness" Value="0"/>
        <!-- 
            点線の枠→FocusVisualStyle : キーボードから操作した時にfocusが当たった時のスタイル
            FocusVisualStyle に Value="{x:Null}"を設定すると消せる
        -->
        <Setter Property="FocusVisualStyle" Value="{x:Null}"/>
    </Style>
</DataGrid.CellStyle>
```

---

## XAMLにおけるif文

<https://threeshark3.com/wpf-binding-datatrigger/>  

DataTrigger（データトリガー）とは、Bindingした値に応じてプロパティを変化させる仕組みです。  
Styleでは、通常、「Setter」というオブジェクトを配置してプロパティの値を定義します。  
「Setter」に対し「Triggers」では、条件を記述し、その条件にマッチしたときのみ設定される値を定義できます。  

``` XML
<CheckBox
    x:Name="SetProductCheckBox"
    VerticalAlignment="Center"
    VerticalContentAlignment="Center"
    IsChecked="{Binding Data.Product.SetProductFlag, Mode=TwoWay}">
    <!-- 今回の本題ではないが、チェックボックスの大きさを変えたかったらLayoutTransformするしかないみたい -->
    <CheckBox.LayoutTransform>
        <ScaleTransform ScaleX="1.2" ScaleY="1.2"/>
    </CheckBox.LayoutTransform>
    <CheckBox.Style>
    <!-- TargetTypeを指定しないとなんのプロパティがあるかわからないので指定する -->
        <Style TargetType="CheckBox">
            <Style.Triggers>
            <!-- DataTriggerは他の値をBindして条件として使えるのが強み -->
                <DataTrigger Binding="{Binding Value, ElementName=DepartmentCDBox}" Value="0">
                    <Setter Property="IsEnabled" Value="False" />
                    <Setter Property="IsChecked" Value="False" />
                </DataTrigger>
            </Style.Triggers>
        </Style>
    </CheckBox.Style>
    <!-- TrueとFalse、両方定義する場合 -->
    <DockPanel.Style>
        <Style TargetType="DockPanel">
            <Style.Triggers>
                <DataTrigger Binding="{Binding ReservationPlayerRemarks, Mode=OneWay, Converter={StaticResource NullOrEmptyToBoolConverter}}" Value="true">
                    <Setter Property="Background" Value="Transparent" />
                </DataTrigger>
                <DataTrigger Binding="{Binding ReservationPlayerRemarks, Mode=OneWay, Converter={StaticResource NullOrEmptyToBoolConverter}}" Value="false">
                    <Setter Property="Background" Value="IndianRed" />
                </DataTrigger>
            </Style.Triggers>
        </Style>
    </DockPanel.Style>
</CheckBox>
```

---

## GrapeCityコンポーネントのDataTriggerにおけるTargetTypeの指定の仕方

Labelとかは`<Style TargetType="Label">`でいいんだけど、C1系の指定はどうしたらいいかわからなかったので調べた。  

<https://docs.grapecity.com/help/c1/xaml/xaml_gettingstarted/html/ImplicitandExplicitStyles.htm>  

公式にちゃんと書いてあった。  
予約枠台帳作成処理において遭遇したパターンでまとめる。  

``` XML
     <c1:C1Calendar.Style>
        <!-- {x:Type c1:○○}で指定する -->
        <Style TargetType="{x:Type c1:C1Calendar}">
            <Style.Triggers>
                <DataTrigger Binding="{Binding IsEditMode}" Value="False">
                    <Setter Property="SelectionMode" Value="Multiple" />
                </DataTrigger>
                <DataTrigger Binding="{Binding IsEditMode}" Value="True">
                    <Setter Property="SelectionMode" Value="Single" />
                </DataTrigger>
            </Style.Triggers>
        </Style>
    </c1:C1Calendar.Style>
```

---

### and条件とor条件の書き方

<https://stackoverflow.com/questions/38396419/multidatatrigger-with-or-instead-of-and>  
<http://gootara.org/library/2017/01/wpfao.html>  

StyleのDataTriggerでor条件を指定して状態を変化させる方法。  
MultiDataTriggerなるものを使えば、複数条件は指定できるが、ORの場合はConverterをかまさないといけないらしい。  
そこで見つけたのがこの方法。  
2回も同じこと書くのが気にくわないけど、確かにORになっている。  
セッターの部分をまとめることができないか調べてみたが、Setterだけをまとめることはできなさそうだ。  

``` XML
<!-- 商品台帳のチェックボックスで頑張ったやつ -->
<CheckBox
    x:Name="SetProductCheckBox"
    VerticalAlignment="Center"
    VerticalContentAlignment="Center"
    IsChecked="{Binding Data.Product.SetProductFlag, Mode=TwoWay}">
    <CheckBox.Style>
        <Style TargetType="CheckBox">
            <Style.Triggers>
                <!--  or条件  -->
                <DataTrigger Binding="{Binding Value, ElementName=DepartmentCDBox}" Value="0">
                    <Setter Property="IsEnabled" Value="False" />
                    <Setter Property="IsChecked" Value="False" />
                </DataTrigger>
                <DataTrigger Binding="{Binding Value, ElementName=DepartmentCDBox}" Value="{x:Null}">
                    <Setter Property="IsEnabled" Value="False" />
                    <Setter Property="IsChecked" Value="False" />
                </DataTrigger>
            </Style.Triggers>

            <!-- and条件 -->
            <Style.Triggers>
                <MultiDataTrigger>
                    <MultiDataTrigger.Conditions>
                        <!-- 部門コードが入力されていて、セット商品にチェックが付いていなかったら -->
                        <Condition Binding="{Binding IsChecked, ElementName=SetProductCheckBox}" Value="False" />
                        <Condition Binding="{Binding Value, ElementName=DepartmentCDBox, Converter={StaticResource NotNullOrEmptyToBoolConverter}}" Value="True" />
                    </MultiDataTrigger.Conditions>
                    <Setter Property="IsEnabled" Value="True" />
                    <Setter Property="IsTabStop" Value="True" />
                </MultiDataTrigger>
            </Style.Triggers>
            <!-- And条件に合わない場合の状態も記述する -->
            <Setter Property="IsEnabled" Value="False" />
            <Setter Property="IsTabStop" Value="False" />
        </Style>
    </CheckBox.Style>
</CheckBox>
```

---

## MultiBindingとDataTriggerによるAnd,Or条件の指定の仕方

[StryleのMultiDataTrigger](http://gootara.org/library/2017/01/wpfao.html)

XAML上でAND条件やOR条件を指定したコントロールの制御にも色々あって、  
StyleのMultiDataTriggerなるものと、MultiBindingを使った方法があったのでまとめ。  

MultiDataTriggerはSytleの中に書くのだが、そうすると見た目がおかしくなってしまうので(多分BaseOnとか必要)、  
見た目を崩したくなかったらMultiBindingとConverterを使ってBindさせたほうがよい。  
似ているようで微妙に違って、頭が混乱したのでまとめ。  

どっちがいいという議論もちゃんとある。  
<https://stackoverflow.com/questions/20993293/multidatatrigger-vs-datatrigger-with-multibinding>  
<https://base64.work/so/wpf/171252>  

``` XML
<!-- Style + DataTrigger or MultiDataTrigger -->
<!-- DataTriggerによる制御。BaseOnするものがあれば見た目を崩さずに済むと思う。 -->
<Binding
    Converter="{StaticResource NotNullOrEmptyToBoolConverter}"
    Mode="OneWay"
    Path="Data.Product.ProductCD"
    RelativeSource="{RelativeSource FindAncestor,AncestorType={x:Type metro:MetroWindow}}" />
    <c1:C1FlexGrid.Style>
        <Style TargetType="c1:C1FlexGrid">
            <!-- or条件 -->
            <Style.Triggers>
                <!-- 部門コードが入力されていない、または、セット商品にチェックが付いている-->
                <DataTrigger Binding="{Binding IsChecked, ElementName=SetProductCheckBox}" Value="True">
                    <Setter Property="IsEnabled" Value="False" />
                    <Setter Property="IsTabStop" Value="False" />
                </DataTrigger>
                <DataTrigger Binding="{Binding Value, ElementName=DepaetmentCDBox, Converter={StaticResource NotNullOrEmptyToBoolConverter}}" Value="True">
                    <Setter Property="IsEnabled" Value="False" />
                    <Setter Property="IsTabStop" Value="False" />
                </DataTrigger>
            </Style.Triggers>

            <!-- and条件 -->
            <Style.Triggers>
                <MultiDataTrigger>
                    <MultiDataTrigger.Conditions>
                        <!-- 部門コードが入力されていて、セット商品にチェックが付いていなかったら -->
                        <Condition Binding="{Binding IsChecked, ElementName=SetProductCheckBox}" Value="False" />
                        <Condition Binding="{Binding Value, ElementName=DepartmentCDBox, Converter={StaticResource NotNullOrEmptyToBoolConverter}}" Value="True" />
                    </MultiDataTrigger.Conditions>
                    <MultiDataTrigger.Setters>
                        <Setter Property="IsEnabled" Value="True" />
                        <Setter Property="IsTabStop" Value="True" />
                    </MultiDataTrigger.Setters>
                </MultiDataTrigger>
                <!-- And条件に合わない場合の状態も記述する -->
                <Setter Property="IsEnabled" Value="False" />
                <Setter Property="IsTabStop" Value="False" />
            </Style.Triggers>
        </Style>
    </c1:C1FlexGrid.Style>

<!-- MultiBinding + MultiConverter -->
<!-- Styleに書かないので、見た目を崩さない。AND,ORの指定はMultiConverterを定義することで実現する -->
<ctrl:CustomFlexGrid x:Name="UnitPriceFlexGrid">
    <!-- 1つ1つのプロパティに対してMultiBindingで条件を指定する必要がある -->
    <!-- AND,ORのMultiConverterは必要。既に定義されていたので使わせてもらった -->
    <ctrl:CustomFlexGrid.IsEnabled>
        <MultiBinding Converter="{StaticResource BooleanAndMultiConverter}">
            <Binding
                Converter="{StaticResource NotConverter}"
                ElementName="SetProductCheckBox"
                Path="IsChecked" />
            <Binding
                Converter="{StaticResource NotNullOrEmptyToBoolConverter}"
                ElementName="DepartmentCDBox"
                Path="Value" />
        </MultiBinding>
    </ctrl:CustomFlexGrid.IsEnabled>
    <ctrl:CustomFlexGrid.IsTabStop>
        <MultiBinding Converter="{StaticResource BooleanAndMultiConverter}">
            <Binding
                Converter="{StaticResource NotConverter}"
                ElementName="SetProductCheckBox"
                Path="IsChecked" />
            <Binding
                Converter="{StaticResource NotNullOrEmptyToBoolConverter}"
                ElementName="DepartmentCDBox"
                Path="Value" />
        </MultiBinding>
    </ctrl:CustomFlexGrid.IsTabStop>
</ctrl:CustomFlexGrid>
```

---

## XAMLでConstファイルを読み込んで使いたい

<https://www.it-swarm-ja.com/ja/wpf/%E6%96%87%E5%AD%97%E5%88%97%E3%82%92%E9%9D%99%E7%9A%84%E3%83%AA%E3%82%BD%E3%83%BC%E3%82%B9%E3%81%A8%E3%81%97%E3%81%A6%E5%AE%9A%E7%BE%A9%E3%81%99%E3%82%8B/971295591/>  

FlexGrid操作用のTriggerActionで列名を指定する必要があったので、
XAML上の列名とTriggerActionで指定する列名をConstで定義して参照したほうがいいだろうということで探して見た。  
そしたら意外と探すのに苦労した。  

``` C#
    public class ColumnName
    {
        public const string IsSelected = "IsSelected";
        public const string DutchTreatAmount = "DutchTreatAmount";
    }
```

``` XML
<metro:MetroWindow
    xmlns:localresource="clr-namespace:RN3.Wpf.Front.DutchTreat.Resouce">

    <c1:Column
        ColumnName="{x:Static localresource:ColumnName.DutchTreatAmount}"/>
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
        Text="{Binding Context.AccountsReceivableDate, StringFormat={}{0:yyyy/MM/dd}, ElementName=AccountNoInputDialogControl, Mode=OneWay}" />
    <TextBlock
        Width="300"
        Margin="10,0,0,0"
        VerticalAlignment="Center"
        Background="AliceBlue"
        Text="{Binding Context.AccountsReceivableName, ElementName=AccountNoInputDialogControl, Mode=OneWay}" />
</StackPanel>

<!-- MultiBindingを使うことで1つのTextBlockで2つの内容を表示することができる -->
<TextBlock
    Width="300"
    HorizontalAlignment="Left"
    VerticalAlignment="Center">
    <TextBlock.Text>
        <MultiBinding StringFormat="{}{0:yyyy/MM/dd}  {1}">
            <Binding
                ElementName="AccountNoInputDialogControl"
                Mode="OneWay"
                Path="Context.AccountsReceivableDate" />
            <Binding
                ElementName="AccountNoInputDialogControl"
                Mode="OneWay"
                Path="Context.AccountsReceivableName" />
        </MultiBinding>
    </TextBlock.Text>
</TextBlock>
```

---

## WPFのコードビハインドを介したリソースへのアクセス

<https://www.it-swarm-ja.com/ja/c%23/wpf%E3%81%AE%E3%82%B3%E3%83%BC%E3%83%89%E3%83%93%E3%83%8F%E3%82%A4%E3%83%B3%E3%83%89%E3%82%92%E4%BB%8B%E3%81%97%E3%81%9F%E3%83%AA%E3%82%BD%E3%83%BC%E3%82%B9%E3%81%B8%E3%81%AE%E3%82%A2%E3%82%AF%E3%82%BB%E3%82%B9/968279150/>

xaml resourcedictionary コード 参照  
割り勘の戻り修正をしている時に、FlexGridのセルテンプレート使ってチェックボックスを配置して実装していたが、  
勝手に見た目が変わってどうしようもなかったのでTriggerAction使ってコードビハインドで直接カラムを操作することにした。  
その時、背景色をResourceDictionaryから取得したほうがいいよな～と思って調べた内容。  

``` C#:使用例
    // Application.Current.Resources["resourceName"]
    dutchTreatAmount.Background = (Brush)Application.Current.Resources["IsReadOnlyBackGroundColor"];
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

## WPF + MVVM でキーイベントを定義する

<https://qiita.com/koara-local/items/02d214f0b6fbf26866ec>  

会計NoでF8押しても保存されないって課題を治すときに調べた内容。  
会計NoでF8を観測してしまえないかと思った次第でしたが、ダメでした。  
結果的にCustomBusyIndicatorのプロパティとIMEを抑制するBehaivorを実装する事で解決できましたが、  
各コントロールにおいて、キーイベントによるコマンドの発火ができることがわかったので、これはこれで参考としてまとめておきます。  

``` XML : Ctrl + S でViewModelの特定の内容を保存するコマンドを呼び出したい場合。
    <!-- 実装例1(Gestureを利用) -->
    <Grid>
        <Grid.InputBindings>
            <KeyBinding Gesture="Ctrl+S" Command="{Binding SaveFileCommand}"/>
        </Grid.InputBindings>
    </Grid>
    <!-- 実装例2(KeyとModifiersを利用) -->
    <!-- Gesture の場合の利点としては Ctrl+Alt+S などの２つ以上でもそのまま書けるところです。 -->
    <Grid>
        <Grid.InputBindings>
            <KeyBinding Key="S" Modifiers="Control" Command="{Binding SaveFileCommand}"/>
        </Grid.InputBindings>
    </Grid>
```

---

## 幅いっぱいのやつと中央のやつの違い

白い帯が幅いっぱいになるタイプのダイアログとウィンドウタイプのダイアログ。  
どちらも実装したけれど、具体的に何が違うのかずっと疑問だったのでまとめた。  

InteractionMessage

インタラクショントリガー
ウィンドウがアクティブな状態でF8が押下されたときに実行しますよ。
上から順に実行しますよ。
1行1行がTrrigerAction。
Invokeを必ず持つ。

Invokeは実行する関数。

インタラクショントリガー
→
イベントトリガー
→
アクショントリガー

・ウィンドウ ShowMetroDialogAsync 専用TriggerAction  XAMLあり  
・幅OKのみ   ShowMessageAsync     共通TriggerAction  XAMLなし  
・幅入力あり ShowMetroDialogAsync 共通TriggerAction  XAMLあり  

つまりはそういうことだ。  
何かしらのカスタムをしたかったら、XAML作ってShowMetroDialogAsyncをする必要がある。  
ボタンだけの簡潔なダイアログだけならMetro固有の何かで十分なのだろう。  

### チェックイン入力F7伝票入力(中央のやつ)

ウィンドウタイプの代表としてチェックイン入力のF7伝票入力を採用することにした。  
一番身近で出しやすかったから。  
ウィンドウタイプとして出すためには、出力する画面を設計して、間にトリガーアクションを挟む必要がある。  
いまだにトリガーアクションとコードビハインドの書き方がわからない。  

``` C#  : Front.CheckInTSheet.ViewModels.EditWindowViewModels.cs
    ShowSlipDialogCommand = new DelegateCommand(
        () => Messenger.Raise(new InteractionModalDialogMessage("ShowSlipDialog", "伝票入力", this)), 
        () => !IsBusy
    );
```

``` XML : Front.CheckInTSheet.Views.EditWindow.xaml
    <i:Interaction.Triggers>
        <l:InteractionMessageTrigger MessageKey="ShowSlipDialog" Messenger="{Binding Messenger, Mode=OneWay}">
            <local_ctrl:ShowModalDialogAction DialogWidth="1240" IsBusy="{Binding IsBusy, Mode=OneWay}" />
        </l:InteractionMessageTrigger>
    </i:Interaction.Triggers>
```

``` C#  : Front.CheckInTSheet.Control.ShowModalDialogAction.cs
    /// <summary>
    /// ModalDialogを開くTrrigerAction
    /// </summary>
    public class ShowModalDialogAction : TriggerAction<MetroWindow>
    {
        
        /// <summary>
        /// CustomDialogを開きます
        /// </summary>
        /// <param name="parameter"></param>
        protected override async void Invoke(object parameter)
        {
            if (parameter is InteractionModalDialogMessage message)
            {
                var setting = new MetroDialogSettings()
                {
                    AnimateHide = false,
                    AnimateShow = false
                };
                _Dialog = new ModalDialog(setting)
                {
                    Content = message.DataContext,
                    ContentTemplate = DialogContent,
                    Title = message.Title,
                    Padding = new Thickness(30),
                };
                var content = AssociatedObject.Content as UIElement;
                bool contentIsEnabled = true;
                if (content != null)
                {
                    //値を保持
                    contentIsEnabled = content.IsEnabled;
                    //ダイアログ表示中にウィンドウ下の要素に対しての操作を不能にする
                    content.IsEnabled = false;
                }
                _Dialog.CloseCommand = new DelegateCommand(async () =>
                {
                    await AssociatedObject.HideMetroDialogAsync(_Dialog);
                    //ダイアログを破棄
                    _Dialog = null;
                    if (content != null)
                        content.IsEnabled = contentIsEnabled;
                    // 終了時に行う処理があれば実行(DataContextにIDisposableを持っても良いかもしれない。)
                    if (message.ClosedCallback != null)
                    {
                        var closedCallback = message.ClosedCallback;
                        message.ClosedCallback = null;
                        closedCallback();
                        closedCallback = null;
                    }
                });
                _Dialog.SetBinding(
                    ModalDialog.IsBusyProperty, 
                    new Binding("IsBusy")
                    {
                        Source = this,
                        Mode = BindingMode.OneWay,
                    }
                );
                _Dialog.DialogWidth = DialogWidth;
                await AssociatedObject.ShowMetroDialogAsync(_Dialog);
                var ctrlContent = _Dialog.FindChild<ContentPresenter>("PART_Content");
                var child = ctrlContent.FindVisualChild<UIElement>();
                if (child != null)
                {
                    var focusCtrl = FocusManager.GetFocusedElement(child);
                    if (focusCtrl != null)
                    {
                        focusCtrl.Focus();
                    }
                    else
                    {
                        child.Focus();
                    }
                }
                await _Dialog.WaitUntilUnloadedAsync();
                child = null;
                ctrlContent = null;
                setting = null;
            }
        }
    }
```

``` C#  : Front.CheckInTSheet.Control.SlipDialog.cs
    // ウィンドウのコードビハインド
    /// <summary>
    /// SlipDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class SlipDialog : CustomDialog
    {
        //  MahApps.Metro.Controls.Dialogs.CustomDialog : BaseMetroDialog
    }
```

### 商品台帳のF8保存のバリデーションエラーの時に表示されるやつ(幅いっぱい)

OKボタンしかないタイプのもの。一番よく見かけるやつ。  
横いっぱいのタイプも基本的にはトリガーアクションでMetroのshow系の命令を叩くが、こちらはオリジナルで定義したXAMLは呼び出していない。  

``` C#  : Common.Util.DialogUtil.MessageDialogUtil.cs
    // F8→ViewModelBase→MessageDialogUtil

    /// <summary>
    /// エラーダイアログを表示します。
    /// </summary>
    /// <param name="messenger">メッセンジャー</param>
    /// <param name="message">メッセージ</param>
    public static void ShowWarning(InteractionMessenger messenger, string message)
    {
        messenger.Raise(
            new InteractionDialogMessage(
                "MessageDialog",
                "エラー",
                message,
                MessageDialogStyle.Affirmative,
                response => { }
            )
        );
    }
```

``` XML : Master.Product.Views.EditWindow.xaml
    <i:Interaction.Triggers>
        <l:InteractionMessageTrigger MessageKey="MessageDialog" Messenger="{Binding Messenger}">
            <action:ShowDialogAction />
        </l:InteractionMessageTrigger>
    </i:Interaction.Triggers>
```

``` C#  : Common.TriggerAction.ShowDialogAction.cs
    /// <summary>
    /// Dialogを開くTrrigerAction
    /// </summary>
    public class ShowDialogAction : TriggerAction<MetroWindow>
    {
        /// <summary>
        /// Dialogを開きます
        /// </summary>
        /// <param name="parameter"></param>
        protected override void Invoke(object parameter)
        {
            if (parameter is InteractionDialogMessage message)
            {
                var content = AssociatedObject.Content as UIElement;
                bool contentIsEnabled = true;
                if (content != null)
                {
                    //値を保持
                    contentIsEnabled = content.IsEnabled;
                    //ダイアログ表示中にウィンドウ下の要素に対しての操作を不能にする
                    content.IsEnabled = false;
                }
                // MahApps.Metro.Controls.Dialogs.MetroDialogSettings
                var settings = new MetroDialogSettings()
                {
                    FirstAuxiliaryButtonText = message.FirstAuxiliaryButtonText,
                    SecondAuxiliaryButtonText = message.SecondAuxiliaryButtontext,
                    AnimateHide = false,
                    AnimateShow = false,
                    DefaultButtonFocus = message.DefaultButtonFocus
                };
                if (!string.IsNullOrEmpty(message.AffirmativeButtonText))
                    settings.AffirmativeButtonText = message.AffirmativeButtonText;
                if (!string.IsNullOrEmpty(message.NegativeButtonText))
                    settings.NegativeButtonText = message.NegativeButtonText;

                Dispatcher.Invoke(
                    async () =>
                    {
                        var result = await AssociatedObject.ShowMessageAsync(message.Title, message.Message, message.Style, settings);
                        if (content != null)content.IsEnabled = contentIsEnabled;
                        message.Callback(result);
                        settings = null;
                    }, 
                    System.Windows.Threading.DispatcherPriority.Render
                );
            }
        }
    }
```

### チェックイン入力F3会計No変更の場合のやつ(幅いっぱい)

簡単な入力欄があるタイプのもの。  
基本的に入力があるものはXAMLを設計するらしい。  

``` C#  : Front.CheckInTSheet.ViewModel.EditWindow.cs
    // 会計No入力ダイアログ表示
    ShowAccountNoInputDialogCommand = new DelegateCommand(
        () => Messenger.Raise(
            new InteractionInputTextDialogMessage(
                "ShowAccountNoInputDialog",
                "会計No変更",
                string.Format(Message.RequestInput, "会計No"),
                null,
                4,
                InputMethodType.ImeOff,
                null,
                async value =>
                {
                }
            )
        ),
        () => !IsBusy && !string.IsNullOrEmpty(Data?.AccountNo)
    );
```

``` XML : Front.CheckInTSheet.Views.EditWindow.xaml
    <i:Interaction.Triggers>
        <l:InteractionMessageTrigger MessageKey="ShowAccountNoInputDialog" Messenger="{Binding Messenger, Mode=OneWay}">
            <action:ShowInputTextDialogAction />
        </l:InteractionMessageTrigger>
    </i:Interaction.Triggers>
```

``` C#  : Common.TriggerAction.ShowInputTextDialogAction.cs
    /// <summary>
    /// テキスト入力Dialogを開くTrrigerAction
    /// </summary>
    public class ShowInputTextDialogAction : TriggerAction<MetroWindow>
    {        
        /// <summary>
        /// Dialogを開きます
        /// </summary>
        /// <param name="parameter"></param>
        protected override async void Invoke(object parameter)
        {           
            if(parameter is InteractionInputTextDialogMessage message)
            {
                 // CustomDialogはMahApps.Metro.Controls.Dialogs名前空間のBaseMetroDialogを継承したクラス
                var dialog = new CustomDialog(){ Title = message.Title };
                // カスタムコントロールのインスタンス
                InputTextDialog dialogContent = new InputTextDialog()
                {
                    Message = message.Message,
                    Input = message.Input,
                    MaxLength = message.MaxLength,
                    InputMethod = message.InputMethod,
                    Format = message.Format,
                    AffirmativeCommand = new DelegateCommand<string>(async response =>
                    {
                        await AssociatedObject.HideMetroDialogAsync(dialog);
                        message.Callback(response);
                        dialogContent = null;
                        dialog = null;
                    }),
                    NegativeCommand = new DelegateCommand<string>(async response =>
                    {
                        await AssociatedObject.HideMetroDialogAsync(dialog);
                        message.Callback(null);
                        dialogContent = null;
                        dialog = null;
                    })
                };
                dialog.Content = dialogContent;
                await AssociatedObject.ShowMetroDialogAsync(
                    dialog,
                    new MetroDialogSettings()
                    {
                        AnimateHide = false,
                        AnimateShow = false
                    }
                );
            }
        }
    }
```

``` C#  : Common.Control.InputTextDialog.cs
    /// <summary>
    /// InputTextDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class InputTextDialog : UserControl
    {
    }
```

---

## XAMLでBoolを反転するには?

[bool を反転して Binding したかっただけなんだ](https://usagi.hatenablog.jp/entry/2018/12/05/211311)  

``` XML
<!-- なぜこれがデフォルトで出来ないのか -->
<CheckBox IsChecked="{Binding Awabi}"/>
<CheckBox IsEnabled="{Binding !Awabi}"/>
```

---

## GetOnlyプロパティのバインド

[OneWayToSource binding from readonly property in XAML](https://stackoverflow.com/questions/658170/onewaytosource-binding-from-readonly-property-in-xaml)  
C# wpf dependencyobject getonly bind  

C1MultiSelectコントロールの選択項目のバインド方法がわからなくて調べた。  
そもそもget onlyプロパティをバインドする方法って単純にget onlyにすればいいだけじゃないかと思った。  
