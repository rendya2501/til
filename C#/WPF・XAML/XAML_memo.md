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

## イベントを観測してコントロールのメソッドを実行したりプロパティを変更する方法

[Blend SDKのChangePropertyActionを使ってみる](http://nineworks2.blog.fc2.com/blog-entry-33.html?sp)  
[【WPF】EventTriggerとChangePropertyAction](http://pro.art55.jp/?eid=1303828)  

いつぞや解決したと思っていたFlexGridの幅ギリギリまで列を定義した時に横スクロールが表示されてしまう問題に対応するため色々やった。  
最初から横スクロールを無効にして、Resizeイベントが発生したら横スクロールを有効にすればいいかなって思ったのでイベントを観測してプロパティを切り替えられないか探した。  
実際にできたけど、ItemsSourceが変わった瞬間Resizeも走るので意味がないことが分かった。  
検索し終わった後にトリガーを引くのも遅すぎるので、結局Invalidateで解決した。  

``` xml : 基本
<i:Interaction.Triggers>
    <!-- いつも使っているやり方 -->
    <l:InteractionMessageTrigger MessageKey="FlexGridInvalidateAction" Messenger="{Binding Messenger}">
        <i:CallMethodAction MethodName="ArrangeScroll" />
    </l:InteractionMessageTrigger>

    <!-- イベントを観測する場合はEventTriggerを使う -->
    <i:EventTrigger EventName="GotFocus">
        <!-- プロパティを変更したい場合ChangePropertyActionを使う -->
        <i:ChangePropertyAction PropertyName="HorizontalScrollBarVisibility" Value="Hidden" />
        <!-- 同じ要領でコントロールのメソッドを実行することも可能 -->
        <i:CallMethodAction MethodName="OnSizeChanged" />
    </i:EventTrigger>
</i:Interaction.Triggers>
```

``` XML : バグに対応するためにあれこれやったやつ
<i:Interaction.Triggers>
    <l:InteractionMessageTrigger MessageKey="FlexGridInvalidateAction" Messenger="{Binding Messenger}">
        <!-- CustomFlexGridにスクロールバーの大きさを変えるっぽいメソッドがあったので呼び出してみたが駄目だった。 -->
        <i:CallMethodAction MethodName="ArrangeScroll" TargetObject="{Binding RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type ctrl:CustomFlexGrid}}}" />
        <!-- CustomFlexGrid側でもItemsSourceChangedの最後に実行しているが、それだけでは駄目で、描画された後にDispatcher.Invokeで改めて実行したら行けた -->
        <i:CallMethodAction MethodName="InvalidateVisual" />
        <!-- データをセットしたらスクロールバーを消して、もう一度設定しなおしたらいけないか試したけど駄目だった。 -->
        <i:ChangePropertyAction PropertyName="HorizontalScrollBarVisibility" Value="Hidden" />
        <i:ChangePropertyAction PropertyName="HorizontalScrollBarVisibility" Value="Auto" />
    </l:InteractionMessageTrigger>

    <!-- 中身が変わったタイミングでいけないか色々やってみた -->
    <i:EventTrigger EventName="ItemsSourceChanging">
        <!-- これは上でも書いたけど駄目 -->
        <i:ChangePropertyAction PropertyName="HorizontalScrollBarVisibility" Value="Hidden" />
        <i:ChangePropertyAction PropertyName="HorizontalScrollBarVisibility" Value="Auto" />
        <!-- ソートしたら治るので、疑似的なソートに関係する処理を実行させたらどうかと思ったが駄目だった。 -->
        <i:CallMethodAction MethodName="Clear" TargetObject="{Binding RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type ctrl:CustomFlexGrid}}, Path=CollectionView.SortDescriptions}" />
        <!-- アイテムが変わっているタイミングでの再描画は駄目だった。もちろんChangedでも駄目。 -->
        <i:CallMethodAction MethodName="InvalidateVisual" />
    </i:EventTrigger>
</i:Interaction.Triggers>
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

## GetOnlyプロパティのバインド

[OneWayToSource binding from readonly property in XAML](https://stackoverflow.com/questions/658170/onewaytosource-binding-from-readonly-property-in-xaml)  
C# wpf dependencyobject getonly bind  

C1MultiSelectコントロールの選択項目のバインド方法がわからなくて調べた。  
そもそもget onlyプロパティをバインドする方法って単純にget onlyにすればいいだけじゃないかと思った。  

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

## タブコントロールのタブを幅いっぱいにする修正

元々ビヘイビアでやっていたところをスタイルから修正することにした。  
添付プロパティや添付プロパティを含めたDataTriggerの分岐のやり方とか色々発見があったのでまとめる。  

### 最終的な作品

``` XML
    <ControlTemplate TargetType="{x:Type TabControl}">
        <Grid>
            <Grid.ColumnDefinitions>
                <ColumnDefinition x:Name="ColumnDefinition0" />
                <ColumnDefinition x:Name="ColumnDefinition1" Width="0" />
            </Grid.ColumnDefinitions>
            <Grid.RowDefinitions>
                <RowDefinition x:Name="RowDefinition0" Height="Auto" />
                <RowDefinition x:Name="RowDefinition1" Height="*" />
            </Grid.RowDefinitions>
            <Grid
                x:Name="HeaderPanelGrid"
                Grid.Row="0"
                Grid.Column="0"
                Panel.ZIndex="1">
                <mah:Underline
                    x:Name="Underline"
                    Background="{DynamicResource MahApps.Brushes.WindowTitle}"
                    BorderBrush="{TemplateBinding mah:TabControlHelper.UnderlineBrush}"
                    LineThickness="{TemplateBinding BorderThickness}"
                    Placement="Bottom"
                    SnapsToDevicePixels="{TemplateBinding SnapsToDevicePixels}"
                    Visibility="Collapsed" />
                <!-- 今回実装したところ -->
                <UniformGrid
                    x:Name="HeaderPanel"
                    Columns="{TemplateBinding helper:TabControlHelper.Colums}"
                    IsItemsHost="true"
                    KeyboardNavigation.TabIndex="1"
                    Rows="{TemplateBinding helper:TabControlHelper.Rows}" />
            </Grid>
        </Grid>
    </ControlTemplate>
```

``` C#
using System.Windows;

namespace RN3.Wpf.Common.Control.Helper
{
    /// <summary>
    /// タブコントロールヘルパー
    /// </summary>
    public class TabControlHelper
    {
        #region ColumsProperty
        /// <summary>
        /// Columsプロパティ
        /// </summary>
        public static readonly DependencyProperty ColumsProperty =
            DependencyProperty.RegisterAttached(
                "Colums",
                typeof(int),
                typeof(TabControlHelper),
                new FrameworkPropertyMetadata()
            );
        /// <summary>
        /// Columsを取得します。
        /// </summary>
        /// <param name="dp"></param>
        /// <returns></returns>
        public static int GetColums(DependencyObject dp)
        {
            return (int)dp.GetValue(ColumsProperty);
        }
        /// <summary>
        /// Columsを設定します。
        /// </summary>
        /// <param name="dp"></param>
        /// <param name="value"></param>
        public static void SetColums(DependencyObject dp, int value)
        {
            dp.SetValue(ColumsProperty, value);
        }
        #endregion

        #region RowsProperty
        /// <summary>
        /// Rowsプロパティ
        /// </summary>
        public static readonly DependencyProperty RowsProperty =
            DependencyProperty.RegisterAttached(
                "Rows",
                typeof(int),
                typeof(TabControlHelper),
                new FrameworkPropertyMetadata(1)
            );
        /// <summary>
        /// Rowsを取得します。
        /// </summary>
        /// <param name="dp"></param>
        /// <returns></returns>
        public static int GetRows(DependencyObject dp)
        {
            return (int)dp.GetValue(RowsProperty);
        }
        /// <summary>
        /// Rowsを設定します。
        /// </summary>
        /// <param name="dp"></param>
        /// <param name="value"></param>
        public static void SetRows(DependencyObject dp, int value)
        {
            dp.SetValue(RowsProperty, value);
        }
        #endregion
    }
}
```

``` XML
    <TabControl
        x:Name="FeeTabControl"
        Width="970"
        Height="320"
        behavior:MoveFocusBehavior.IsSkip="All"
        ItemsSource="{Binding SeasonList, Mode=OneWay}"
        SelectedItem="{Binding SeasonSelectedItem, Mode=TwoWay}">
        <!-- 添付プロパティの設定 -->
        <!-- 行が0なら1にするTrigger。ここでもあれこれあった。できれば直接添付プロパティを参照したかったけどそうしたらStackOverFlowしやがった。 -->
        <TabControl.Style>
            <Style BasedOn="{StaticResource {x:Type TabControl}}" TargetType="{x:Type TabControl}">
                <Setter Property="helper:TabControlHelper.Rows" Value="{Binding Path=Items.Count, RelativeSource={RelativeSource Self}, Converter={StaticResource DivideConverter}, ConverterParameter=4}" />
                <Style.Triggers>
                    <DataTrigger Binding="{Binding Path=Items.Count, RelativeSource={RelativeSource Self}, Converter={StaticResource DivideConverter}, ConverterParameter=4}" Value="0">
                        <Setter Property="helper:TabControlHelper.Rows" Value="1" />
                    </DataTrigger>
                </Style.Triggers>
            </Style>
        </TabControl.Style>
        <TabControl.ItemTemplate>
            <DataTemplate>
                <TextBlock Text="{Binding SeasonName}" />
            </DataTemplate>
        </TabControl.ItemTemplate>
        <TabControl.ContentTemplate>
            <DataTemplate>
            </DataTemplate>
        </TabControl.ContentTemplate>
    </TabControl>
```

### ボツ作品

総数を列の数で割って行を求めようとした。  
ほぼできたし動くのも確認できたけど、現存するConverterだけでは無理そうだったのと、そもそも2行にするプログラムは料金台帳しかないから、View及びViewModel側から値を設定できたほうが楽でいいんじゃないかと気が付いてやめた。  
まぁ、色々検証できたのでこれはこれでよかったけど。  

``` XML
<Grid
    x:Name="HeaderPanelGrid"
    Grid.Row="0"
    Grid.Column="0"
    Panel.ZIndex="1">
    <mah:Underline
        x:Name="Underline"
        Background="{DynamicResource MahApps.Brushes.WindowTitle}"
        BorderBrush="{TemplateBinding mah:TabControlHelper.UnderlineBrush}"
        LineThickness="{TemplateBinding BorderThickness}"
        Placement="Bottom"
        SnapsToDevicePixels="{TemplateBinding SnapsToDevicePixels}"
        Visibility="Collapsed" />
    <!--<TabPanel
        x:Name="HeaderPanel"
        IsItemsHost="true"
        KeyboardNavigation.TabIndex="1" />-->
    <UniformGrid
        x:Name="HeaderPanel"
        Columns="{TemplateBinding helper:TabControlHelper.Colums}"
        IsItemsHost="true"
        KeyboardNavigation.TabIndex="1">
        <UniformGrid.Style>
            <Style TargetType="{x:Type UniformGrid}">
                <Setter Property="Rows" Value="1" />
                <Style.Triggers>
                    <DataTrigger Binding="{Binding Path=Columns, RelativeSource={RelativeSource Self}, Converter={StaticResource NegativeToBoolConverter}}">
                        <Setter Property="Rows">
                            <Setter.Value>
                                <MultiBinding Converter="{StaticResource DivideMultiConverter}" ConverterParameter="{x:Type sys:Int32}">
                                    <MultiBinding.Bindings>
                                        <Binding Path="Items.Count" RelativeSource="{RelativeSource FindAncestor, AncestorType={x:Type TabControl}}" />
                                        <Binding Path="Columns" RelativeSource="{RelativeSource Self}" />
                                    </MultiBinding.Bindings>
                                </MultiBinding>
                            </Setter.Value>
                        </Setter>
                    </DataTrigger>
                </Style.Triggers>
            </Style>
        </UniformGrid.Style>
        <!--<UniformGrid.Rows>
            <MultiBinding Converter="{StaticResource DivideMultiConverter}" ConverterParameter="{x:Type sys:Int32}">
                <MultiBinding.Bindings>
                    <Binding Path="Items.Count" RelativeSource="{RelativeSource FindAncestor, AncestorType={x:Type TabControl}}" />
                    <Binding Path="Columns" RelativeSource="{RelativeSource Self}" />
                </MultiBinding.Bindings>
            </MultiBinding>
        </UniformGrid.Rows>-->
    </UniformGrid>
</Grid>
```

``` xml : なんかYr君が最初に提示してくれた案
ItemsPanel
    <Setter Property="ItemsPanel">
        <Setter.Value>
            <ItemsPanelTemplate>
                <Grid></Grid>
            </ItemsPanelTemplate>
        </Setter.Value>
    </Setter>
```

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

---

## 画像の追加でミスった話

代表者・幹事対応の時に遭遇した問題。  

・binとobjを削除する必要があった模様。  
・フォルダに追加するだけだと認識されない。  
・その場合はすべて表示にして、右クリックして追加する。  

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
