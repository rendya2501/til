# XAMLまとめ

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
[EventToCommandを使って、ViewModelへEventArgs渡す](https://qiita.com/takanemu/items/efbe7ab1b1272251720a)  
[WPFのMVVMでイベントを処理する方法いろいろ](https://whitedog0215.hatenablog.jp/entry/2020/03/15/173935)  

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

C1MultiSelectコントロールの実装中において、選択状態を初期化をしたいなーって時にUnselectAllというメソッドが用意されているのは分かったので、  
それを直接呼び出せないか調べていたら、CallMethodActionという、まさしくな仕組みがあったのでまとめ。  
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

支払方法変更処理の単体テスト戻りでScrollViewerにフォーカスが当たって点線の枠が表示されてしまう問題が発生した。  
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

``` xml : 文言を変えるサンプル
<Button Command="{Binding ShowHelpPageCommand, Mode=OneWay}">
    <Button.Content>
        <Image Source="{StaticResource White_Help_16}" Stretch="None" />
    </Button.Content>
    <Button.Style>
        <Style BasedOn="{StaticResource WindowButton}" TargetType="Button">
            <Style.Triggers>
                <DataTrigger Binding="{Binding HelpPageURL, Converter={StaticResource NotNullOrEmptyToBoolConverter}, Mode=OneWay}" Value="true">
                    <Setter Property="ToolTip" Value="ヘルプページを表示します。" />
                </DataTrigger>
                <DataTrigger Binding="{Binding HelpPageURL, Converter={StaticResource NotNullOrEmptyToBoolConverter}, Mode=OneWay}" Value="false">
                    <Setter Property="ToolTip" Value="ヘルプページの設定がありません。" />
                </DataTrigger>
            </Style.Triggers>
        </Style>
    </Button.Style>
</Button>
```

ボタンなどのコントロールにマウスを載せた時、説明やヒントをポップアップ表示してくれる機能です。  
[ボタンのツールチップ](https://araramistudio.jimdo.com/2019/11/01/c-%E3%81%AEwpf%E3%81%A7%E3%83%84%E3%83%BC%E3%83%AB%E3%83%81%E3%83%83%E3%83%97%E3%82%92%E8%A1%A8%E7%A4%BA%E3%81%99%E3%82%8B/)  

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

interactionTriggerにKeyTriggerなるものもあるので、そっちも場合によっては使えるかも。

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

## TriggerActionに値を渡すサンプル

2021/10/06 Wed  
C1MultiSelectのカスタムコントロールが全然できないので、そもそも本当に選択状態を反映させることができるのか実験するために、TriggerAction方式でやってみることにした。  
単純な値を渡すだけでもInteractionが必要だったので、ついでにまとめることにした。  
ちなみに選択状態の反映はうまくいった。  

``` C# : Front.DutchTreat.ViewModels.cs
{
    // 科目大区分の選択状態を反映させる
    await Messenger.RaiseAsync(
        new InteractionSetSelectedItemsOfSubjectLargeType(
            "SetSelectedItemsOfSubjectLargeTypeAction",
            SubjectLargeTypeList
                .Where(w => settlementSet.SubjectLargeTypeCDList
                    .Contains(w.SubjectLargeTypeCD))
        )
    );
}
```

``` XML : Front.DutchTreat.Views.EditWindow.xaml
    <i:Interaction.Triggers>
        <l:InteractionMessageTrigger MessageKey="SetSelectedItemsOfSubjectLargeTypeAction" Messenger="{Binding Messenger}">
            <localaction:SetSelectedItemsOfSubjectLargeTypeAction />
        </l:InteractionMessageTrigger>
    </i:Interaction.Triggers>
```

``` C# : Front.DutchTreat.Interaction.cs
    /// <summary>
    /// C1MultiSelectに選択状態を反映させるためのインタラクション
    /// </summary>
    public class InteractionSetSelectedItemsOfSubjectLargeType : InteractionMessage
    {
        /// <summary>
        /// コンテキスト
        /// </summary>
        public IEnumerable<SubjectLargeTypeWithSubjectCDList> List { get; set; }
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="messageKey"></param>
        public InteractionSetSelectedItemsOfSubjectLargeType(string messageKey) : base(messageKey) { }
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="messageKey"></param>
        /// <param name="list"></param>
        public InteractionSetSelectedItemsOfSubjectLargeType(string messageKey, IEnumerable<SubjectLargeTypeWithSubjectCDList> list) : this(messageKey) => List = list;
        /// <summary>
        /// 派生クラスでは必ずオーバーライドしてください。Freezableオブジェクトとして必要な実装です。
        /// 通常このメソッドは、自身の新しいインスタンスを返すように実装します。
        /// </summary>
        /// <returns>自身の新しいインスタンス</returns>
        protected override Freezable CreateInstanceCore() => new InteractionSetSelectedItemsOfSubjectLargeType(MessageKey, List);
    }
```

``` C# : Front.DutchTreat.TriggerAction.cs
    /// <summary>
    /// C1MultiSelectに選択状態を反映させるためのアクション
    /// </summary>
    public class SetSelectedItemsOfSubjectLargeTypeAction : TriggerAction<C1MultiSelect>
    {
        /// <summary>
        /// 選択状態を反映させます。
        /// </summary>
        /// <param name="parameter"></param>
        protected override void Invoke(object parameter)
        {
            if (parameter is InteractionSetSelectedItemsOfSubjectLargeType message)
            {
                // 値が重複したらエラーになるので念のため消してからセットする。
                AssociatedObject.ListBox.SelectedItems.Clear();
                foreach (var item in message.List) AssociatedObject.ListBox.SelectedItems.Add(item);
            }
        }
    }
```

---

## XAMLでBoolを反転するには？

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

---

## 虫眼鏡アイコンをセットしたボタンの実装方法

何のことはない。ただのボタンでスタイルは共通で定義されているものを使っているだけ。  

``` XML : Front.DutchTreat.Views.EditWindow.xaml
<Button : 
    Command="{Binding ShowAttendeeListCommand}"
    IsTabStop="False"
    Style="{StaticResource SearchButton}" />
```

``` XML : Common\Resource\DesignResourceDictionary.xaml
<Style
    x:Key="SearchButton"
    BasedOn="{StaticResource {x:Type Button}}"
    TargetType="{x:Type Button}">
    <Setter Property="ContentTemplate">
        <Setter.Value>
            <DataTemplate>
                <Image Source="{StaticResource Black_Search_24}" Stretch="None" />
            </DataTemplate>
        </Setter.Value>
    </Setter>
    <Setter Property="HorizontalContentAlignment" Value="Center" />
    <Setter Property="VerticalContentAlignment" Value="Center" />
    <Setter Property="Height" Value="30" />
    <Setter Property="Width" Value="30" />
</Style>
```

---

## 自分自身のItemsSourceのCountをXAML上で使用する方法

自分自身の要素が1件も無かったらDataTriggerでEnableをFalseにしたくて調べた。  
やっぱりそれなりに需要はあるみたいで、実現できたのでまとめる。  
ここら辺はRelativeSourceの話になってくるが、そっちでも1記事レベルなのでそれは別でまとめる。1  

[Bind Count of ItemsSource of an ItemsControl in a TextBlock using WPF](https://stackoverflow.com/questions/39482829/bind-count-of-itemssource-of-an-itemscontrol-in-a-textblock-using-wpf)  

``` XML : Wpf.Front.DutchTreat.Views.EditWindow.xaml
<c1:C1MultiSelect.Style>
    <Style TargetType="{x:Type c1:C1MultiSelect}">
        <Style.Triggers>
            <DataTrigger Binding="{Binding RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type metro:MetroWindow}}, Path=DataContext.CanEditDutchTreat, Mode=OneWay}" Value="false">
                <Setter Property="IsEnabled" Value="False" />
            </DataTrigger>
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

## Enumを任意の文字列に変換するコンバーター

Enumのメンバーを任意の文字列に変換するため業務中に作った作品。  
割とうまくできたので、備忘録として乗せておく。  
方針としてはDisplayアノテーションの内容を変換文字列として使う方法。  

``` C# : Common.Converter
    /// <summary>
    /// 
    /// </summary>
    public class EnumToStringConverter : IValueConverter
    {
        /// <summary>
        /// 
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                FieldInfo field = value?.GetType().GetField(value?.ToString());
                DisplayAttribute attr = field.GetCustomAttribute<DisplayAttribute>();
                return attr != null
                    ? attr.Name
                    : value.ToString();
            }
            catch
            {
                return string.Empty;
            }
        }
        /// <summary>
        /// 使わない
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
```

``` C#: Common.Data.Enum
    /// <summary>
    /// 課税区分項目
    /// </summary>
    public enum TaxationType : byte
    {
        /// <summary>
        /// 外税
        /// </summary>
        [Display(Name = "外税")]
        OutsideTax = 1,
        /// <summary>
        /// 内税
        /// </summary>
        [Display(Name = "内税")]
        InsideTax = 2,
        /// <summary>
        /// 非課税
        /// </summary>
        [Display(Name = "非課税")]
        TaxFree = 3
    }
```

``` XML : 使い方
<c1:Column
    Width="65"
    HorizontalAlignment="Left"
    VerticalAlignment="Center"
    Binding="{Binding TaxationType, Converter={StaticResource EnumToStringConverter}, Mode=OneWay}"
    ColumnName="TaxationTypeName"
    Header="課税"
    HeaderHorizontalAlignment="Center"
    HeaderVerticalAlignment="Center" />
```

---

## KeyValuePairConverter

いつぞや、ディクショナリーの値をコンボボックスに配置するために作ったコンバーター。  
うまくできたので置いておく。  

``` C# : KeyValuePairConverter
    /// <summary>
    /// KeyValuePairのKeyをValueに変換するコンバーター
    /// </summary>
    public class KeyValuePairConverter : IValueConverter
    {
        /// <summary>
        /// KeyValuePairのKeyをValueに変換します。
        /// </summary>
        /// <param name="value">バインディング ソースによって生成された値</param>
        /// <param name="targetType">バインディング ターゲット プロパティの型</param>
        /// <param name="parameter">使用するコンバーター パラメーター</param>
        /// <param name="culture">コンバーターで使用するカルチャ</param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // null判定
            if (parameter == null) throw new Exception(string.Format(Message.Invalid, "値"));
            // 型の判定とIListへ変換
            if (!(parameter is IList list)) throw new Exception(string.Format(Message.Invalid, "型"));
            // 要素をループ
            foreach (var item in list)
            {
                // 値が一般的であることを確認
                Type valueType = item.GetType();
                if (valueType.IsGenericType)
                {
                    // ジェネリック型の定義を抽出
                    Type baseType = valueType.GetGenericTypeDefinition();
                    // KeyValuePair型の判定
                    if (baseType == typeof(KeyValuePair<,>))
                    {
                        // KeyとValueの取得
                        var kvpKey = valueType.GetProperty("Key")?.GetValue(item, null);
                        var kvpValue = valueType.GetProperty("Value")?.GetValue(item, null);
                        // Keyと引数valueの比較
                        if (kvpKey?.Equals(value) ?? kvpKey == value)
                        {
                            return kvpValue;
                        }
                    }
                }
            }
            // Keyに合致するものがなければnullを返却。
            return null;
        }

        /// <summary>
        /// OneWayでのBindingでしか使用しません。
        /// </summary>
        /// <param name="value">バインディング ソースによって生成された値</param>
        /// <param name="targetType">バインディング ターゲット プロパティの型</param>
        /// <param name="parameter">使用するコンバーター パラメーター</param>
        /// <param name="culture">コンバーターで使用するカルチャ</param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
```

---

## DependencyProperyのSetterの値がNullになってしまう問題

[Why does my Dependency Property send null to my view model?](https://stackoverflow.com/questions/38958177/why-does-my-dependency-property-send-null-to-my-view-model)  
[MVVMのDataGridまたはListBoxからSelectedItemsにバインド](https://www.webdevqa.jp.net/ja/c%23/mvvm%E3%81%AEdatagrid%E3%81%BE%E3%81%9F%E3%81%AFlistbox%E3%81%8B%E3%82%89selecteditems%E3%81%AB%E3%83%90%E3%82%A4%E3%83%B3%E3%83%89/942024865/amp/)  

MultiSelectComboBoxのSelectedItemsの2WayBindingを実装している時に出くわした問題。  
コントロール側はIList,ViewModel側はIEnumerable<T>で実装していたのだが、コントロール側のSetterまでは値が入っているのに、ViewModelのSetterにはNullが入ってしまう現象に遭遇した。  
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

## アノテーションによるValidationの抑制

[How to suppress validation when nothing is entered](https://stackoverflow.com/questions/1502263/how-to-suppress-validation-when-nothing-is-entered)  

いつぞや、商品台帳でIsEnableにDataのDepartmentをバインドしてF8を実行した時に、Requireのエラーが出て困ったことがあった。  
その時はどうやって調べたいいかわからなかったので、仕方なくDataではなくコントロールのValueをバインドして解決したが、
今回はバリデートを抑制したいということで、`wpf validation suppress`で調べたらいい感じのが出てきた。  
BindingクラスにValidation関係のプロパティがたくさんあったので、それっぽいやつを指定したら、実現できたのでまとめる。  

今回は`ValidatesOnNotifyDataErrors`をFalseにしたらうまく行った。  
読んで字のごとく、Dataにおけるエラー通知をどうするかって意味だと思われる。  
調べてもこのプロパティはこういうものです！って解説をしているところがまったくない。  
まぁ、解決したからいいか。  

``` XML
<im:GcTextBox
    IsReadOnly="{Binding Data.BankCD, Mode=OneWay, Converter={StaticResource NullOrEmptyToBoolConverter}, ValidatesOnNotifyDataErrors=False}"
    IsTabStop="{Binding Data.BankCD, Mode=OneWay, Converter={StaticResource NotNullOrEmptyToBoolConverter}, ValidatesOnNotifyDataErrors=False}"
/>
```

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

### テキストブロックをぐわんぐわん動かしたい

[WPFにおけるGUI構築講座 -座標ベタ書きから脱却しよう-](https://qiita.com/YSRKEN/items/686068a359866f21f956)
