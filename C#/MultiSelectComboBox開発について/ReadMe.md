# MultiSelectComboBox開発について

いろいろやったのでまとめ。  

---

## コントロールのスタイルを作成する方法 (WPF .NET)

[コントロールのスタイルを作成する方法 (WPF .NET)](https://docs.microsoft.com/ja-jp/dotnet/desktop/wpf/controls/how-to-create-apply-style?view=netdesktop-5.0)  
[WPF4.5入門 その50 「Style」](https://blog.okazuki.jp/entry/2014/09/04/200304)  

因みにXAMLをデバッグ中に変更できる機能は「エディットコンテニュー」というらしい。  
VisualStudio2017から使えるようになったようなので、比較的新しい技術みたいだ。  


Styleで指定した値はデフォルト値になる。

スタイルの暗黙的な適応  
スタイルの TargetType を TextBlock 型に設定し、x:Key属性を省略すると、
そのスタイルにスコープ指定されているすべての TextBlock 要素 (通常、XAML ファイル自体) にスタイルが適用されます。

``` XML : スタイルの暗黙的な適応
<Window.Resources>
    <!-- x:Key属性が省略されているのでTextBlock型全てに設定が適応される -->
    <Style TargetType="TextBlock">
        <Setter Property="HorizontalAlignment" Value="Center" />
        <Setter Property="FontFamily" Value="Comic Sans MS"/>
        <Setter Property="FontSize" Value="14"/>
    </Style>
</Window.Resources>
<StackPanel>
    <TextBlock>My Pictures</TextBlock>
    <TextBlock>Check out my new pictures!</TextBlock>
</StackPanel>
```

スタイルの明示的な適応  
値が含まれる x:Key 属性をスタイルに追加すると、そのスタイルは TargetType のすべての要素に暗黙的に適用されなくなります。  

``` XML : スタイルの明示的な適応
<Window.Resources>
    <!-- x:Key属性を定義 -->
    <Style x:Key="TitleText" TargetType="TextBlock">
        <Setter Property="HorizontalAlignment" Value="Center" />
        <Setter Property="FontFamily" Value="Comic Sans MS"/>
        <Setter Property="FontSize" Value="14"/>
    </Style>
</Window.Resources>
<StackPanel>
    <!-- x:Key属性の使用を宣言することでMyPicturesのみに設定が適応される -->
    <TextBlock Style="{StaticResource TitleText}">My Pictures</TextBlock>
    <TextBlock>Check out my new pictures!</TextBlock>
</StackPanel>
```

スタイルの継承  
Styleは、別のスタイルを元にして新しいStyleを作ることが出来ます。  
BaseOnというプロパティに元になるStyleを指定することで実現出来ます。  

``` XML : スタイルの継承
<Window.Resources>
    <Style TargetType="TextBlock">
        <Setter Property="HorizontalAlignment" Value="Center" />
        <Setter Property="FontFamily" Value="Comic Sans MS"/>
        <Setter Property="FontSize" Value="14"/>
    </Style>
    <!-- BasedOnで元となるスタイルを定義 -->
    <Style BasedOn="{StaticResource {x:Type TextBlock}}"
           TargetType="TextBlock"
           x:Key="TitleText">
        <Setter Property="FontSize" Value="26"/>
        <Setter Property="Foreground">
            <Setter.Value>
                <LinearGradientBrush StartPoint="0.5,0" EndPoint="0.5,1">
                    <LinearGradientBrush.GradientStops>
                        <GradientStop Offset="0.0" Color="#90DDDD" />
                        <GradientStop Offset="1.0" Color="#5BFFFF" />
                    </LinearGradientBrush.GradientStops>
                </LinearGradientBrush>
            </Setter.Value>
        </Setter>
    </Style>
</Window.Resources>
<StackPanel>
    <!-- x:Key属性の使用を宣言することでMyPicturesのみに設定が適応される -->
    <TextBlock Style="{StaticResource TitleText}" Name="textblock1">My Pictures</TextBlock>
    <TextBlock>Check out my new pictures!</TextBlock>
</StackPanel>
```

トリガー  
Styleでは、Triggerを使うことでプロパティの値に応じてプロパティの値を変更することが出来ます。  
例えばマウスが上にあるときにTrueになるIsMouseOverプロパティがTrueの時に、背景色を青にするには以下のようなStyleを記述します。  

``` XML
<Style x:Key="DefaultTextStyle" TargetType="{x:Type TextBlock}">
    <Setter Property="FontFamily" Value="Meiryo UI" />
    <Setter Property="FontSize" Value="12" />
    <Style.Triggers>
        <Trigger Property="IsMouseOver" Value="True">
            <Setter Property="Background" Value="Blue" />
        </Trigger>
    </Style.Triggers>
</Style>
```

・Template

・TemplateBinding
親の設定を子にも適応させるやつ。

・StaticResource  
対象のプロパティに1度だけ設定が行われます。
リソースとバインド先の依存関係プロパティの対応付けは起動時の1回のみ、ただしクラスは参照なのでリソースのプロパティの変更はバインド先も影響を受ける。

・DynamicResource
リソースの内容が変更されたら対象のプロパティも変更されます。
リソースとバインド先の依存関係プロパティの対応付けは起動時および起動中(リソースに変更がある度)。つまりリソースのオブジェクトが変わってもバインド先は影響を受けるし、当然リソースのプロパティ変更はバインド先も影響を受ける。


・ControlTemplate
コントロールのためのテンプレート。
なんかControlTemplate定義してるところは全部Gridから始まっているけど何なのだろうか。そういうものなのか？  

このコントロール テンプレートは単純です。
コントロールのルート要素である Grid
ボタンの丸みのある外観を描画するための Ellipse
ユーザー指定のボタンの内容を表示する ContentPresenter

ControlTemplate単体で定義して、x:keyで指定することも可能な模様。  

・ContentControl
どんなに頑張ってもチェックボックスのForegroundが変わってくれなかった。
元のソースを見てみると、CheckBoxのContentプロパティのContentControlなるものを操作していたので、
そちらのForegroundを変更したら行けた。

・何個も入れ子になっているStyleのTemplateBindingは本当に全部親の設定を引き継いでいるという認識でいいのか？

・いろいろなコントロールが組み合わさってモノができているのはわかるけど、その中のこのコントロールの大しての設定！みたいな指定ってどこでやってるんだ？

[WPFのStaticResourceとDynamicResourceの違い](https://tocsworld.wordpress.com/2014/06/26/wpf%E3%81%AEstaticresource%E3%81%A8dynamicresource%E3%81%AE%E9%81%95%E3%81%84/)  
[MSDN_WPFのStaticResourceとDynamicResourceの違い](https://social.msdn.microsoft.com/Forums/ja-JP/3bbcdc48-2a47-495e-9406-2555dc515c3a/wpf12398staticresource12392dynamicresource123983694912356?forum=wpfja)  
[コントロールのためのテンプレートを作成する方法 (WPF.NET)](https://docs.microsoft.com/ja-jp/dotnet/desktop/wpf/controls/how-to-create-apply-template?view=netdesktop-5.0)  
[テンプレート（WPF）](https://ufcpp.net/study/dotnet/wpf_template.html)  
[[WPF]ComboBoxのControlTemplateを使ってシンプルかつMouseOrver時に色が変わるComboBoxを作ってみた](https://qiita.com/nori0__/items/61bc195ff6e07ff1daa5)  



・SnapsToDevicePixels

エッジをシャープにするオプションらしい。
これがないとぼやけるっぽいので常につけておけばいいんじゃないかな。