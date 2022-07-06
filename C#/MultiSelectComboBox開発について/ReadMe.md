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

[コントロールのためのテンプレートを作成する方法 (WPF.NET)](https://docs.microsoft.com/ja-jp/dotnet/desktop/wpf/controls/how-to-create-apply-template?view=netdesktop-5.0)  
[テンプレート（WPF）](https://ufcpp.net/study/dotnet/wpf_template.html)  

[[WPF]ComboBoxのControlTemplateを使ってシンプルかつMouseOrver時に色が変わるComboBoxを作ってみた](https://qiita.com/nori0__/items/61bc195ff6e07ff1daa5)  

・SnapsToDevicePixels

エッジをシャープにするオプションらしい。
これがないとぼやけるっぽいので常につけておけばいいんじゃないかな。

---

## 自作マルチセレクトコンボボックス

[【C#】ListBoxで項目を追加、取得する方法(CheckedListBoxも解説)](https://www.sejuku.net/blog/57045)  

[CheckBox Styles and Templates 公式ソース](https://docs.microsoft.com/en-us/previous-versions/windows/silverlight/dotnet-windows-silverlight/cc278078(v=vs.95))

・ディスプレイメンバー
・セレクトバリュー
・中間状態の見た目
・そもそもの解析
