# XAML_Styleまとめ

---

## Style

>コントロールに設定するプロパティの値のセットを集めるためのもの。  
Styleは、Setterというオブジェクトを使って、どのプロパティにどんな値を設定するか指定できます。  
スタイルを設定するには、コントロールのStyleプロパティにStyleを設定します。  
ResourceやResourceDictionaryに定義することでコントロールのプロパティの設定を複数のコントロールで共通化することが出来ます。  
[WPF4.5入門 その50 「Style」](https://blog.okazuki.jp/entry/2014/09/04/200304)  

---

## Styleの書き方

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

## BaseOn

親のスタイルを受け継ぐ。  
受け継いだうえで、指定したプロパティは上書きすることができる。  

``` XML
<Window>
    <Window.Resources>
        <!--  継承元のスタイル  -->
        <Style x:Key="DefaultTextStyle" TargetType="{x:Type TextBlock}">
            <Setter Property="FontFamily" Value="Meiryo UI" />
            <Setter Property="Foreground" Value="Yellow" />
            <Setter Property="FontSize" Value="12" />
        </Style>

        <!--  継承先のスタイル  -->
        <!--  メイリョウ以外全て上書きされる  -->
        <Style
            x:Key="TitleTextStyle"
            BasedOn="{StaticResource DefaultTextStyle}"
            TargetType="{x:Type TextBlock}">
            <Setter Property="FontSize" Value="24" />
            <Setter Property="Foreground" Value="Red" />
        </Style>

        <!--  継承先のスタイルをさらに継承したスタイル  -->
        <!--  色の指定がない場合、直前の継承元が赤なので赤のままになる。 -->
        <!--  指定したプロパティの通り、字体は斜体になり、文字の大きさは継承元より小さくなる  -->
        <Style
            x:Key="ThirdTextStyle"
            BasedOn="{StaticResource TitleTextStyle}"
            TargetType="{x:Type TextBlock}">
            <Setter Property="FontStyle" Value="Oblique" />
            <Setter Property="FontSize" Value="12" />
        </Style>

        <!-- コントロールの指定だけの場合、それがデフォルトのスタイルとなる -->
        <Style TargetType="Label">
            <Setter Property="Foreground" Value="HotPink" />
        </Style>
    </Window.Resources>

    <StackPanel>
        <!-- メイリョウ + 黄 + 大きさ12 -->
        <TextBlock Style="{StaticResource TitleTextStyle}" Text="タイトル" />
        <!-- メイリョウ + 赤 + 大きさ24 -->
        <TextBlock Style="{StaticResource DefaultTextStyle}" Text="デフォルトのテキスト" />
        <!-- メイリョウ + 赤 + 大きさ12 + 斜め -->
        <TextBlock Style="{StaticResource ThirdTextStyle}" Text="第3のテキスト" />
        <!-- デフォルトのスタイルの文字色がピンクなのでピンクになる -->
        <!-- もちろんスタイルの上書きは可能 -->
        <Label Content="aaaa" />
    </StackPanel>
</Window>
```

[[WPF/xaml]BasedOnを使って元のstyleを受け継ぐ](https://qiita.com/tera1707/items/3c4f598c5d022e4987a2)  

---

## Trigger

Styleでは、Triggerを使うことでプロパティの値に応じてプロパティの値を変更することが出来ます。  
例えばマウスが上にあるときにTrueになるIsMouseOverプロパティがTrueの時に、背景色を青にするには以下のようなStyleを記述します。  
TriggerのPropertyに設定可能なプロパティは、依存関係プロパティなので、その点に注意が必要です。  

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

---

## ResourceDictionary

CSSのXAMLバージョン。  
スタイルの定義をまとめ、複数のプロジェクトから参照できるようにしたもの。  

``` XML : ResourceDictionary定義
<ResourceDictionary
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:local="clr-namespace:Wpf_Style">
    <!--  2つ目のチェックボックスのスタイル  -->
    <Style x:Key="MyCheckboxStyle2" TargetType="CheckBox">
        <Setter Property="Background" Value="Red" />
    </Style>
    <Style TargetType="Label">
        <Setter Property="Foreground" Value="HotPink" />
    </Style>
</ResourceDictionary>
```

``` XML : 使う側
<Window.Resources>
    <ResourceDictionary>
    <!-- 複数をまとめたいなら ResourceDictionary の MergedDictionaries プロパティに ResourceDictionary をいれます -->
        <ResourceDictionary.MergedDictionaries>
            <!-- ここで、作ったDictionaryを参照しにいっている -->
            <ResourceDictionary Source="Dictionary1.xaml" />
            <!-- もちろん複数のDictionaryを読み込むことも可能 -->
            <ResourceDictionary Source="Parts2.xaml"/>
            <!-- Source プロパティ付きの ResourceDictionary は子要素にリソースをもつことができないです   -->
            <!-- Source とその場で書いたリソースをまとめたいなら MergedDictionaries で Source 付きと直接リソース定義したものをマージします。   -->
            <!--  リソースディクショナリーを定義した場合、その場で書いたリソースもResourceDictionary内に書く必要があるらしい  -->
            <ResourceDictionary>
                <Style TargetType="Label">
                    <Setter Property="Foreground" Value="HotPink" />
                </Style>
            </ResourceDictionary>
        </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
</Window.Resources>
```

[WPF のリソース](http://var.blog.jp/archives/67298406.html)  
[[WPF/xaml]リソースディクショナリを作って、画面のコントロールのstyleを変える](https://qiita.com/tera1707/items/a462678cdfb61a87334b)  
[[WPF] Styleでできることと書き方](https://qiita.com/tera1707/items/cb8ad4c40107ae25b565)  
