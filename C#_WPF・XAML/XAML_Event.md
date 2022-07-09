# XAML イベント

---

## イベントをViewModel側で観測する方法

マルチセレクトコントロールで選択した値を観測するのに難儀したのでまとめ  

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

## WPF + MVVM でキーイベントを定義する

<https://qiita.com/koara-local/items/02d214f0b6fbf26866ec>  

各コントロールにおいて、キーイベントによるコマンドの発火ができることがわかったので、これはこれで参考としてまとめておきます。  

``` XML : Ctrl + S でViewModelの特定の内容を保存するコマンドを呼び出したい場合
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
