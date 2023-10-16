# ItemsControl

ItemsControlとは、複数のオブジェクトを指定された表示方法で並べるためのコントロール。  
コントロールを並べるためのコントロールとでも言えばいいか。  

---

## ItemsControlの主要プロパティ

|プロパティ名 | 概要 | 設定で使用するコントロール|
|-|-|-|
|Template            | ItemsControlそのものを指定します。           | ControlTemplate|
|ItemsPanel          | 要素の並べ方について指定します。             | ItemsPanaleTemplate|
|ItemTemplate        | 要素を表示する際のテンプレートを指定します。 | DataTemplate|
|ItemContainierStyle | 要素に適応するStyleを指定します。            | Style|

---

## Template

コントロール全体の設定。  
全体をScrollViewerで囲ったり、Borderで装飾したりするときはここで設定する。  
ItemsPresenterには、ItemsPanelの内容が入る。  

``` XML
<ItemsControl.Template>
    <ControlTemplate TargetType="ItemsControl">
        <ScrollViewer Focusable="False">
            <ItemsPresenter/>
        </ScrollViewer>
    </ControlTemplate>
</ItemsControl.Template>
```

---

## ItemsPanel

ItemsPanelTemplate でコレクションをどう並べるかを指定します。
指定できるのはPanelクラスの派生クラスである以下の3つです。
なお、デフォルトで StackPanel が指定されているので、何も指定されていない場合は要素が縦に並びます。

``` txt
StacPanel : 縦に並ぶ  
WrapPanel : 横に並ぶ  
Grid      : 指定はできるが子要素を並べる機能がないためすべて重なる。選ぶ意味はほぼない。  
```

``` XML
<ItemsControl.ItemsPanel>
    <ItemsPanelTemplate>
        <WrapPanel Orientation="Vertical"/>
    </ItemsPanelTemplate>
</ItemsControl.ItemsPanel>
```

---

## ItemContainerStyle

コレクションの各オブジェクトを格納するコンテナーの設定。  
これがItemsPanelに乗る。  
ContentPresenterには、ItemTemplateの内容が入る。  

``` XML
<ItemsControl.ItemContainerStyle>
    <Style TargetType="ListBoxItem">
        <Setter Property="OverridesDefaultStyle" Value="True"/>
        <Setter Property="BorderThickness" Value="1"/>

        <Setter Property="Template">
            <Setter.Value>
                <ControlTemplate TargetType="ContentControl">
                    <Border
                        Background="{TemplateBinding Background}"
                        BorderThickness="{TemplateBinding BorderThickness}"
                        BorderBrush="{TemplateBinding BorderBrush}">
                        <!-- ContentPresenterには、ItemTemplateの内容が入る -->
                        <ContentPresenter/>
                    </Border>
                </ControlTemplate>
            </Setter.Value>
        </Setter>
        <Style.Triggers>
            <Trigger Property="IsSelected" Value="True">
                <Setter Property="Background" Value="#9999CCFF"/>
                <Setter Property="BorderBrush" Value="#3399FF"/>
            </Trigger>
        </Style.Triggers>
    </Style>
</ItemsControl.ItemContainerStyle>
```

---

## ItemTemplate

各オブジェクトの表示形式を指定。  
下記の例では、各オブジェクトのNameプロパティを表示する。  

``` XML
<ItemsControl.ItemTemplate>
    <DataTemplate>
        <TextBlock Text="{Binding Name}"/>
    </DataTemplate>
</ItemsControl.ItemTemplate>
```

---

## サンプル

``` XML
<ListBox ItemsSource="{Binding Mall}">
    <!--  コントロール全体の設定  -->
    <ListBox.Template>
        <ControlTemplate TargetType="{x:Type ListBox}">
            <Border
                Background="LightGray"
                BorderBrush="Red"
                BorderThickness="5">
                <ItemsPresenter Margin="5" />
            </Border>
        </ControlTemplate>
    </ListBox.Template>
    <!--  ItemsPanelTemplate でコレクションをどう並べるかを指定します。  -->
    <ListBox.ItemsPanel>
        <ItemsPanelTemplate>
            <WrapPanel Orientation="Horizontal" />
        </ItemsPanelTemplate>
    </ListBox.ItemsPanel>
    <!--  DataTemplate でコレクションの項目をどのように表示するかを指定します。  -->
    <ListBox.ItemTemplate>
        <DataTemplate>
            <StackPanel>
                <TextBlock>
                    <TextBlock.Text>
                        <MultiBinding StringFormat="【{0}】{1}">
                            <Binding Path="Prefecture" />
                            <Binding Path="Name" />
                        </MultiBinding>
                    </TextBlock.Text>
                </TextBlock>
                <TextBlock Text="{Binding FavoriteCount, StringFormat=お気に入り：{0}}" />
            </StackPanel>
        </DataTemplate>
    </ListBox.ItemTemplate>
    <!--  Style を指定します。ItemTemplate と同じく要素ごとの表示方法を指定するプロパティです。  -->
    <ListBox.ItemContainerStyle>
        <Style TargetType="ListBoxItem">
            <Setter Property="OverridesDefaultStyle" Value="True" />
            <Setter Property="Template">
                <Setter.Value>
                    <ControlTemplate TargetType="{x:Type ContentControl}">
                        <Border Background="{TemplateBinding Background}">
                            <ContentPresenter />
                        </Border>
                    </ControlTemplate>
                </Setter.Value>
            </Setter>
            <Setter Property="Margin" Value="10" />
            <!--  追加したStykle その1  -->
            <Setter Property="Width" Value="100" />
            <!--  追加したStykle その2  -->
            <Setter Property="Height" Value="50" />
            <!--  追加したStykle その3  -->
            <Style.Triggers>
                <Trigger Property="IsMouseOver" Value="True">
                    <Setter Property="Background" Value="LightBlue" />
                </Trigger>
                <Trigger Property="IsSelected" Value="True">
                    <Setter Property="Background" Value="LightGreen" />
                </Trigger>
            </Style.Triggers>
        </Style>
    </ListBox.ItemContainerStyle>
</ListBox>
```

``` C# : このサンプルにおけるViewModel
public MainWindow()
{
    InitializeComponent();
    DataContext = new MainViewModel();
}

class MainViewModel
{
    public List<Store> Mall { get; set; } = Enumerable.Range(1, 20).Select(x => new Store()
    {
        Name = "お店" + x,
        Prefecture = "東京都",
        FavoriteCount = x * 10,
    }).ToList();
}

class Store
{
    public string Name { get; set; }
    public string Prefecture { get; set; }
    public int FavoriteCount { get; set; }
}
```

[WPF】ItemsControlの基本的な使い方](https://qiita.com/ebipilaf/items/c3e9e501eb0560a12ce8)  
[ItemsControlのテンプレートをカスタマイズする](https://mvrck.jp/documents/dotnet/wpf/itemscontrol-template/)  
[WPFのListBoxをカスタムする](https://qiita.com/tera1707/items/363d2a33eadcb3eb275a)  
