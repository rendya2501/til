# ItemsControlまとめ

ItemsControlとは、複数のオブジェクトを指定された表示方法で並べるためのコントロールです。  
コントロールを並べるためのコントロールとでも言えばいいか。  

[ItemsControl 攻略 ～ 外観のカスタマイズ](http://grabacr.net/archives/1240)  
[WPF Reference ScrollContentPresenter of ScrollViewer Template](https://stackoverflow.com/questions/945542/wpf-reference-scrollcontentpresenter-of-scrollviewer-template)  
[How to place a ScrollViewer on top of the content it should scroll](https://stackoverflow.com/questions/490784/how-to-place-a-scrollviewer-on-top-of-the-content-it-should-scroll)  

[A Custom WPF ScrollViewer](https://www.codeproject.com/Tips/1271095/A-Custom-WPF-ScrollViewer)  
[WPF Header/Multi-Column ListBox Control](http://epsg27700.blogspot.com/2010/01/wpf-headeredmulti-column-listbox_21.html)  
[Wpf – Grid/ScrollViewer- Freeze grid header row vertical scrolling, but not horizontal scrolling](https://itecnote.com/tecnote/wpf-grid-scrollviewer-freeze-grid-header-row-vertical-scrolling-but-not-horizontal-scrolling/)  
[How to freeze grid column in ScrollViewer or achieve something similar to that?](https://docs.microsoft.com/en-us/answers/questions/537119/how-to-freeze-grid-column-in-scrollviewer-or-achie.html)  
[グループ化したコレクションのヘッダーをスクロール時に常に表示する](https://qiita.com/ambleside138/items/a25ab909cdbf6f307f54)  
[Show WPF ScrollViewer Vertical scroll bar on right always](https://stackoverflow.com/questions/31511009/show-wpf-scrollviewer-vertical-scroll-bar-on-right-always)  
[WPF の ListBox でスクロールバーの下に隙間ができる](http://var.blog.jp/archives/67425961.html)
[【WPF】ScrollViewerを入れ子にした時の挙動をいい感じにする](https://threeshark3.com/scrcoll-viewer-wheel/)  

[How to implement WPF custom grid with scrolling support](https://stackoverflow.com/questions/42552869/how-to-implement-wpf-custom-grid-with-scrolling-support)  
[WPF - ScrollViewer with fixed header/row](https://stackoverflow.com/questions/10505875/wpf-scrollviewer-with-fixed-header-row)  

---

## ItemsControlの基本的な使い方

[WPF】ItemsControlの基本的な使い方](https://qiita.com/ebipilaf/items/c3e9e501eb0560a12ce8)  
[ItemsControlのテンプレートをカスタマイズする](https://mvrck.jp/documents/dotnet/wpf/itemscontrol-template/)  
[WPFのListBoxをカスタムする](https://qiita.com/tera1707/items/363d2a33eadcb3eb275a)  

``` txt
プロパティ名        | 概要                                         | 設定で使用するコントロール
--------------------|----------------------------------------------|----------------------------
Template            | ItemsControlそのものを指定します。           | ControlTemplate
ItemsPanel          | 要素の並べ方について指定します。             | ItemsPanaleTemplate
ItemTemplate        | 要素を表示する際のテンプレートを指定します。 | DataTemplate
ItemContainierStyle | 要素に適応するStyleを指定します。            | Style
```

### Template

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

### ItemsPanel

ItemsPanelTemplate でコレクションをどう並べるかを指定します。
指定できるのはPanelクラスの派生クラスである以下の3つです。
なお、デフォルトで StackPanel が指定されているので、何も指定されていない場合は要素が縦に並びます。

StacPanel : 縦に並ぶ  
WrapPanel : 横に並ぶ  
Grid      : 指定はできるが子要素を並べる機能がないためすべて重なる  

``` XML
<ItemsControl.ItemsPanel>
    <ItemsPanelTemplate>
        <WrapPanel Orientation="Vertical"/>
    </ItemsPanelTemplate>
</ItemsControl.ItemsPanel>
```

### ItemContainerStyle

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

### ItemTemplate

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

## ItemsControlをDataGridみたいに使う

[WPF ItemsControlをDataGridみたいに使う](https://nomoredeathmarch.hatenablog.com/entry/2019/01/21/003825)  
[ItemsControl with row and column headers WPF](https://stackoverflow.com/questions/51396486/itemscontrol-with-row-and-column-headers-wpf)  

[[WPF] GridSplitter 画面を分割して境界線をドラッグしてリサイズする](https://note.dokeep.jp/post/wpf-gridsplitter/)  
[世界で一番短いサンプルで覚えるMVVM入門](https://resanaplaza.com/%E4%B8%96%E7%95%8C%E3%81%A7%E4%B8%80%E7%95%AA%E7%9F%AD%E3%81%84%E3%82%B5%E3%83%B3%E3%83%97%E3%83%AB%E3%81%A7%E8%A6%9A%E3%81%88%E3%82%8Bmvvm%E5%85%A5%E9%96%80/)  

[WPF: How to prevent scrollbar from overlaying headers in list type views](https://stackoverflow.com/questions/52725066/wpf-how-to-prevent-scrollbar-from-overlaying-headers-in-list-type-views)  

このサンプルを作るだけでも結構な量の発見があった。  
簡単な一覧だけならこのサンプルを適応するだけで色々解決するのではないかと思った。  
最小のMVVMサンプルはかなりいいサンプルなので、これはこれで別にまとめる。  
まだ全体として完成したわけではないし、まだ全然ItemsControlを理解できていないので、今後も習得していく。  

チェックアウトで作ったやつと何か違うと思ったら、参考にしたサンプルはヘッダー部分はGridをそのまま使ってるのか。  

``` XML : サンプルの写経
<Window 略>
    <Window.DataContext>
        <local:MainWindowViewModel />
    </Window.DataContext>
    <Grid Grid.IsSharedSizeScope="True">
        <Grid.Resources>
            <Style x:Key="HorizontalGridSplitter" TargetType="{x:Type GridSplitter}">
                <Setter Property="Height" Value="1" />
                <Setter Property="HorizontalAlignment" Value="Stretch" />
                <Setter Property="Background" Value="Red" />
            </Style>
            <Style x:Key="VerticalGridSplitter" TargetType="{x:Type GridSplitter}">
                <Setter Property="HorizontalAlignment" Value="Stretch" />
                <Setter Property="Width" Value="1" />
                <Setter Property="Background" Value="Red" />
            </Style>
        </Grid.Resources>
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto" />
            <RowDefinition Height="Auto" />
            <RowDefinition Height="*" />
        </Grid.RowDefinitions>
        <!--  ヘッダー部分  -->
        <Grid Grid.Row="0" Background="AliceBlue">
            <Grid.ColumnDefinitions>
                <!--  列幅を自動的に調整する  -->
                <ColumnDefinition Width="Auto" SharedSizeGroup="Seq" />
                <ColumnDefinition Width="Auto" />
                <ColumnDefinition Width="Auto" SharedSizeGroup="Name" />
                <ColumnDefinition Width="Auto" />
                <ColumnDefinition Width="Auto" SharedSizeGroup="BirthDay" />
                <ColumnDefinition Width="Auto" />
                <ColumnDefinition Width="Auto" SharedSizeGroup="Age" />
                <ColumnDefinition Width="Auto" />
            </Grid.ColumnDefinitions>
            <TextBlock
                Grid.Column="0"
                Margin="5,0,10,0"
                HorizontalAlignment="Center"
                Text="連番" />
            <GridSplitter Grid.Column="1" Style="{StaticResource VerticalGridSplitter}" />
            <TextBlock
                Grid.Column="2"
                Margin="5,0,10,0"
                HorizontalAlignment="Center"
                Text="氏名" />
            <GridSplitter Grid.Column="3" Style="{StaticResource VerticalGridSplitter}" />
            <TextBlock
                Grid.Column="4"
                Margin="5,0,10,0"
                HorizontalAlignment="Center"
                Text="生年月日" />
            <GridSplitter Grid.Column="5" Style="{StaticResource VerticalGridSplitter}" />
            <TextBlock
                Grid.Column="6"
                Margin="5,0,10,0"
                HorizontalAlignment="Center"
                Text="年齢" />
            <GridSplitter Grid.Column="7" Style="{StaticResource VerticalGridSplitter}" />
        </Grid>
        <!--  ヘッダーとデータの間の線  -->
        <GridSplitter Grid.Row="1" Style="{StaticResource HorizontalGridSplitter}" />
        <!--  データ部分  -->
        <ItemsControl
            Grid.Row="2"
            Focusable="False"
            ItemsSource="{Binding Persons}"
            ScrollViewer.CanContentScroll="True"
            ScrollViewer.HorizontalScrollBarVisibility="Auto"
            ScrollViewer.IsDeferredScrollingEnabled="True"
            ScrollViewer.VerticalScrollBarVisibility="Auto"
            VirtualizingPanel.IsVirtualizing="True">
            <ItemsControl.Template>
                <ControlTemplate TargetType="{x:Type ItemsControl}">
                    <!--  スクロールバーを表示する  -->
                    <ScrollViewer Focusable="False">
                        <!--  パネルを仮想化する  -->
                        <VirtualizingStackPanel IsItemsHost="True" />
                    </ScrollViewer>
                </ControlTemplate>
            </ItemsControl.Template>
            <ItemsControl.ItemTemplate>
                <DataTemplate DataType="{x:Type local:Person}">
                    <Grid Background="AntiqueWhite">
                        <Grid.ColumnDefinitions>
                            <ColumnDefinition Width="Auto" SharedSizeGroup="Seq" />
                            <ColumnDefinition Width="Auto" />
                            <ColumnDefinition Width="Auto" SharedSizeGroup="Name" />
                            <ColumnDefinition Width="Auto" />
                            <ColumnDefinition Width="Auto" SharedSizeGroup="BirthDay" />
                            <ColumnDefinition Width="Auto" />
                            <ColumnDefinition Width="Auto" SharedSizeGroup="Age" />
                            <ColumnDefinition Width="Auto" />
                        </Grid.ColumnDefinitions>
                        <TextBlock
                            Grid.Column="0"
                            Margin="5,0,10,0"
                            Text="{Binding Seq}" />
                        <GridSplitter Grid.Column="1" Style="{StaticResource VerticalGridSplitter}" />
                        <TextBlock
                            Grid.Column="2"
                            Margin="5,0,10,0"
                            Text="{Binding Name}" />
                        <GridSplitter Grid.Column="3" Style="{StaticResource VerticalGridSplitter}" />
                        <TextBlock
                            Grid.Column="4"
                            Margin="5,0,10,0"
                            Text="{Binding BirthDay}" />
                        <GridSplitter Grid.Column="5" Style="{StaticResource VerticalGridSplitter}" />
                        <TextBlock
                            Grid.Column="6"
                            Margin="5,0,10,0"
                            Text="{Binding Age}" />
                        <GridSplitter Grid.Column="7" Style="{StaticResource VerticalGridSplitter}" />
                    </Grid>
                </DataTemplate>
            </ItemsControl.ItemTemplate>
        </ItemsControl>
    </Grid>
</Window>
```

``` C# : このサンプルにおけるViewModel
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
    }

    public sealed class Person
    {
        public int Seq { get; }
        public string Name { get; }
        public string BirthDay { get; }
        public int Age { get; }
        public Person(int seq, string name, string birthDay, int age)
        {
            this.Seq = seq;
            this.Name = name;
            this.BirthDay = birthDay;
            this.Age = age;
        }
    }

    public class MainWindowViewModel
    {
        public ReadOnlyObservableCollection<Person> Persons { get; set; } =
            new ReadOnlyObservableCollection<Person>(
                new ObservableCollection<Person>
                {
                    new Person(1,  "北野久子"  , "1973/11/25" , 48),
                    new Person(2,  "松崎和馬"  , "1993/11/15" , 28),
                    new Person(3,  "水谷義哉"  , "1962/07/21" , 59),
                    new Person(4,  "長野佳奈"  , "2001/03/13" , 21),
                    new Person(5,  "藤島凪"    , "1974/04/18" , 47),
                    new Person(6,  "荻野亜実"  , "1968/03/02" , 54),
                    new Person(7,  "岩本葵愛"  , "1997/12/06" , 24),
                    new Person(8,  "小山田大貴", "1991/10/10" , 30),
                    new Person(9,  "宮原美明"  , "1977/10/26" , 44),
                    new Person(10, "田端利之"  , "1974/08/16" , 47),
                    new Person(11, "大下靖"    , "1968/09/14" , 53),
                    new Person(12, "那須瑞紀"  , "2000/11/15" , 21),
                    new Person(13, "門脇忠吉"  , "1978/03/25" , 44),
                    new Person(14, "冨永和奏"  , "1988/03/14" , 34),
                    new Person(15, "島本彰三"  , "1988/09/07" , 33),
                    new Person(16, "首藤和男"  , "1995/10/28" , 26),
                    new Person(17, "氏家覚"    , "1972/05/28" , 49),
                    new Person(18, "古川幸次郎", "1977/08/06" , 44),
                    new Person(19, "安藤浩俊"  , "1965/10/16" , 56),
                    new Person(20, "佐伯誠治"  , "1985/03/21" , 37),
                }
            );
    }
```

### チェックアウトのItemsControl

比較参考としてチェックアウトにおいてあったItemsControlも載せておく

``` XML : チェックアウトのItemsControl
<ItemsControl
    MinWidth="120"
    BorderBrush="{StaticResource MahApps.Brushes.Gray5}"
    Focusable="False"
    ItemsSource="{Binding DepositTypeList, Mode=OneWay}">
    <ItemsControl.Template>
        <ControlTemplate TargetType="{x:Type ItemsControl}">
            <Border BorderBrush="{StaticResource MahApps.Brushes.Accent}" BorderThickness="1">
                <ScrollViewer
                    Focusable="False"
                    HorizontalScrollBarVisibility="Auto"
                    VerticalScrollBarVisibility="Auto">
                    <i:Interaction.Triggers>
                        <l:InteractionMessageTrigger MessageKey="ResetPayMethodScrollAction" Messenger="{Binding RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type Window}}, Path=DataContext.Messenger, Mode=OneWay}">
                            <action:ReserScrollViewerAction />
                        </l:InteractionMessageTrigger>
                    </i:Interaction.Triggers>
                    <Grid>
                        <ItemsPresenter Margin="0,26,0,0" />
                        <Border VerticalAlignment="Top" IsHitTestVisible="False">
                            <Grid Background="White">
                                <Grid.ColumnDefinitions>
                                    <ColumnDefinition Width="3*" MinWidth="140" />
                                    <ColumnDefinition Width="4*" MinWidth="100" />
                                </Grid.ColumnDefinitions>
                                <Label
                                    Grid.Column="0"
                                    Height="Auto"
                                    Margin="0"
                                    HorizontalContentAlignment="Center"
                                    BorderBrush="{TemplateBinding BorderBrush}"
                                    BorderThickness="0,0,1,1"
                                    Content="精算方法"
                                    FontSize="15"
                                    Style="{StaticResource AccentLabel}" />
                                <Label
                                    Grid.Column="1"
                                    Height="Auto"
                                    Margin="0"
                                    HorizontalContentAlignment="Center"
                                    BorderBrush="{TemplateBinding BorderBrush}"
                                    BorderThickness="0,0,0,1"
                                    Content="精算金額"
                                    FontSize="15"
                                    Style="{StaticResource AccentLabel}" />
                            </Grid>
                        </Border>
                    </Grid>
                </ScrollViewer>
            </Border>
        </ControlTemplate>
    </ItemsControl.Template>
    <ItemsControl.ItemTemplate>
        <DataTemplate>
            <Border BorderBrush="{Binding RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type ItemsControl}}, Path=BorderBrush, Mode=OneWay}" BorderThickness="0,0,1,1">
                <Grid Height="Auto">
                    <Grid.ColumnDefinitions>
                        <ColumnDefinition Width="3*" MinWidth="140" />
                        <ColumnDefinition Width="4*" MinWidth="100" />
                    </Grid.ColumnDefinitions>
                    <Button
                        HorizontalAlignment="Stretch"
                        VerticalAlignment="Stretch"
                        HorizontalContentAlignment="Stretch"
                        VerticalContentAlignment="Stretch"
                        Command="{Binding RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type Window}}, Path=DataContext.SelectDepositTypeCommand, Mode=OneWay}"
                        CommandParameter="{Binding Mode=OneWay}"
                        Focusable="False"
                        Style="{StaticResource MahApps.Styles.Button.Flat}">
                        <DockPanel VerticalAlignment="Center">
                            <TextBlock DockPanel.Dock="Left" Text="{Binding DepositTypeName, Mode=OneWay}" />
                            <TextBlock
                                Margin="0,0,8,0"
                                HorizontalAlignment="Right"
                                DockPanel.Dock="Right"
                                Text="▼"
                                Visibility="{Binding DetailDisplayFlag, Mode=OneWay, Converter={StaticResource VisibleConverter}}" />
                        </DockPanel>
                    </Button>
                    <im:GcNumber
                        Grid.Column="1"
                        HorizontalAlignment="Stretch"
                        HorizontalContentAlignment="Right"
                        AllowSpin="False"
                        BorderThickness="0"
                        DisabledBackground="Transparent"
                        DisabledForeground="Black"
                        DisplayFieldSet="###,###,##0,,,-,"
                        FieldSet="###,###,##0,,,-,"
                        IsReadOnly="{Binding RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type Window}}, Path=DataContext.CanEditPayment, Mode=OneWay, Converter={StaticResource NotConverter}}"
                        SpinButtonVisibility="NotShown"
                        Value="{Binding Amount, Mode=TwoWay}">
                        <im:GcNumber.Style>
                            <Style BasedOn="{StaticResource {x:Type im:GcNumber}}" TargetType="{x:Type im:GcNumber}">
                                <Setter Property="Background" Value="Transparent" />
                                <Style.Triggers>
                                    <Trigger Property="IsReadOnly" Value="True">
                                        <Setter Property="HighlightText" Value="False" />
                                        <Setter Property="Background" Value="Transparent" />
                                    </Trigger>
                                    <Trigger Property="IsActive" Value="True">
                                        <Setter Property="Background" Value="{StaticResource MahApps.Brushes.Accent4}" />
                                    </Trigger>
                                </Style.Triggers>
                            </Style>
                        </im:GcNumber.Style>
                    </im:GcNumber>
                </Grid>
            </Border>
        </DataTemplate>
    </ItemsControl.ItemTemplate>
</ItemsControl>
```

---

## ItemsControlをDataGridみたいに使うをアレンジ

サンプルを参考にFlexGridみたいにできないかいろいろやって見た。  
2つの完成品ができたので、まとめる。  

選択している要素が必要ないならItemsControl単品でよさそう。
選択している要素が必要ならListBoxで囲むといいかも。

``` XML : ItemsControlバージョン
    <Grid>
        <Grid.Resources>
            <Style x:Key="HorizontalGridSplitter" TargetType="{x:Type GridSplitter}">
                <Setter Property="Height" Value="1" />
                <Setter Property="HorizontalAlignment" Value="Stretch" />
                <Setter Property="Background" Value="Red" />
            </Style>
            <Style x:Key="VerticalGridSplitter" TargetType="{x:Type GridSplitter}">
                <Setter Property="HorizontalAlignment" Value="Stretch" />
                <Setter Property="Width" Value="1" />
                <Setter Property="Background" Value="Red" />
            </Style>

            <!--  ヘッダーレイアウト定義  -->
            <Border
                x:Key="HeaderTemplate"
                Background="Yellow"
                BorderBrush="Black"
                BorderThickness="1,1,1,0">
                <Grid>
                    <Grid.ColumnDefinitions>
                        <ColumnDefinition Width="Auto" SharedSizeGroup="DepositTypeName" />
                        <ColumnDefinition Width="Auto" />
                        <ColumnDefinition Width="Auto" SharedSizeGroup="Amount" />
                    </Grid.ColumnDefinitions>
                    <Label
                        Grid.Column="0"
                        HorizontalContentAlignment="Center"
                        Content="精算方法"
                        FontSize="15" />
                    <GridSplitter Grid.Column="1" Style="{StaticResource VerticalGridSplitter}" />
                    <Label
                        Grid.Column="2"
                        HorizontalContentAlignment="Center"
                        Content="精算金額"
                        FontSize="15" />
                </Grid>
            </Border>

            <!--  データレイアウト定義  -->
            <DataTemplate x:Key="ItemTemplate">
                <Border BorderBrush="Black" BorderThickness="1,0,1,1">
                    <Grid>
                        <Grid.ColumnDefinitions>
                            <ColumnDefinition Width="Auto" SharedSizeGroup="DepositTypeName" />
                            <ColumnDefinition Width="Auto" />
                            <ColumnDefinition Width="Auto" SharedSizeGroup="Amount" />
                        </Grid.ColumnDefinitions>
                        <TextBlock
                            Grid.Column="0"
                            Margin="5,0,10,0"
                            Text="{Binding DepositTypeName}" />
                        <GridSplitter Grid.Column="1" Style="{StaticResource VerticalGridSplitter}" />
                        <TextBlock
                            Grid.Column="2"
                            Margin="5,0,10,0"
                            Text="{Binding Amount}" />
                    </Grid>
                </Border>
            </DataTemplate>

            <!--  ヘッダーとデータを載せるテンプレートの定義  -->
            <ControlTemplate x:Key="MainContentTemplate" TargetType="{x:Type ItemsControl}">
                <!--  ヘッダーとデータをScrollViewerに収める  -->
                <!-- ScrollBarVisibilityプロパティの設定は必須。じゃないと横スクロールができない。1敗 -->
                <ScrollViewer
                    Grid.IsSharedSizeScope="True"
                    HorizontalScrollBarVisibility="Auto"
                    VerticalScrollBarVisibility="Auto">
                    <i:Interaction.Behaviors>
                        <local:ScrollSyncronizingBehavior Orientation="Horizontal" ScrollGroup="Group1" />
                    </i:Interaction.Behaviors>
                    <ScrollViewer.Template>
                        <ControlTemplate TargetType="{x:Type ScrollViewer}">
                            <Grid>
                                <Grid.ColumnDefinitions>
                                    <ColumnDefinition Width="*" />
                                    <ColumnDefinition Width="Auto" />
                                </Grid.ColumnDefinitions>
                                <Grid.RowDefinitions>
                                    <RowDefinition Height="*" />
                                    <RowDefinition Height="Auto" />
                                </Grid.RowDefinitions>
                                <Grid Grid.Row="0" Grid.Column="0">
                                    <Grid.RowDefinitions>
                                        <!--  ヘッダー部分のHeightはAutoじゃないと縦スクロールが利かない。1敗  -->
                                        <RowDefinition Height="Auto" />
                                        <RowDefinition Height="Auto" />
                                        <RowDefinition Height="*" />
                                    </Grid.RowDefinitions>
                                    <!--  ヘッダー部分  -->
                                    <ScrollViewer
                                        Grid.Row="0"
                                        Content="{StaticResource HeaderTemplate}"
                                        HorizontalScrollBarVisibility="Hidden"
                                        VerticalScrollBarVisibility="Hidden">
                                        <i:Interaction.Behaviors>
                                            <local:ScrollSyncronizingBehavior Orientation="Horizontal" ScrollGroup="Group1" />
                                        </i:Interaction.Behaviors>
                                    </ScrollViewer>
                                    <!--  ヘッダーとデータの間の線  -->
                                    <GridSplitter
                                        Grid.Row="1"
                                        IsHitTestVisible="False"
                                        Style="{StaticResource HorizontalGridSplitter}" />
                                    <!--  ScrollViewerのコンテンツ  -->
                                    <!--  ここにデータ部分が乗っているはず  -->
                                    <ScrollContentPresenter
                                        Name="PART_ScrollContentPresenter"
                                        Grid.Row="2"
                                        KeyboardNavigation.DirectionalNavigation="Local" />
                                </Grid>
                                <!--  右端の縦スクロールバー  -->
                                <ScrollBar
                                    Name="PART_VerticalScrollBar"
                                    Grid.Row="0"
                                    Grid.Column="1"
                                    Maximum="{TemplateBinding ScrollableHeight}"
                                    ViewportSize="{TemplateBinding ViewportHeight}"
                                    Visibility="{TemplateBinding ComputedVerticalScrollBarVisibility}"
                                    Value="{TemplateBinding VerticalOffset}" />
                                <!--  最下段の横スクロールバー  -->
                                <ScrollBar
                                    Name="PART_HorizontalScrollBar"
                                    Grid.Row="1"
                                    Grid.Column="0"
                                    Maximum="{TemplateBinding ScrollableWidth}"
                                    Orientation="Horizontal"
                                    ViewportSize="{TemplateBinding ViewportWidth}"
                                    Visibility="{TemplateBinding ComputedHorizontalScrollBarVisibility}"
                                    Value="{TemplateBinding HorizontalOffset}" />
                                <!--  右下の大きさを変えるアイコン  -->
                                <ResizeGrip
                                    Grid.Row="1"
                                    Grid.Column="1"
                                    HorizontalAlignment="Right"
                                    VerticalAlignment="Stretch"
                                    Background="{DynamicResource {x:Static SystemColors.WindowBrushKey}}" />
                            </Grid>
                        </ControlTemplate>
                    </ScrollViewer.Template>
                    <!--  データ部分  -->
                    <ItemsPresenter />
                </ScrollViewer>
            </ControlTemplate>
        </Grid.Resources>

        <ItemsControl
            Width="200"
            Height="100"
            Margin="0,10,0,0"
            Focusable="False"
            ItemTemplate="{StaticResource ItemTemplate}"
            ItemsSource="{Binding DepositTypeList, Mode=OneWay}"
            Template="{StaticResource MainContentTemplate}"
            VirtualizingPanel.IsVirtualizing="True" />
    </Grid>
```

``` XML : ListBoxバージョン
    <Grid>
        <Grid.Resources>
            <Style x:Key="HorizontalGridSplitter" TargetType="{x:Type GridSplitter}">
                <Setter Property="Height" Value="1" />
                <Setter Property="HorizontalAlignment" Value="Stretch" />
                <Setter Property="Background" Value="Red" />
            </Style>


            <Style x:Key="VerticalGridSplitter" TargetType="{x:Type GridSplitter}">
                <Setter Property="HorizontalAlignment" Value="Stretch" />
                <Setter Property="Width" Value="1" />
                <Setter Property="Background" Value="Red" />
            </Style>

            <!--  ヘッダーレイアウト定義  -->
            <Border
                x:Key="HeaderTemplate"
                Background="Yellow"
                BorderBrush="Black"
                BorderThickness="1,1,1,0">
                <Grid>
                    <Grid.ColumnDefinitions>
                        <ColumnDefinition Width="Auto" SharedSizeGroup="DepositTypeName" />
                        <ColumnDefinition Width="Auto" />
                        <ColumnDefinition Width="Auto" SharedSizeGroup="Amount" />
                    </Grid.ColumnDefinitions>
                    <Label
                        Grid.Column="0"
                        HorizontalContentAlignment="Center"
                        VerticalContentAlignment="Center"
                        Content="精算方法"
                        FontSize="15" />
                    <GridSplitter Grid.Column="1" Style="{StaticResource VerticalGridSplitter}" />
                    <Label
                        Grid.Column="2"
                        HorizontalContentAlignment="Center"
                        VerticalContentAlignment="Center"
                        Content="精算金額"
                        FontSize="15" />
                </Grid>
            </Border>

            <!--  データレイアウト定義  -->
            <DataTemplate x:Key="ItemTemplate">
                <Border BorderBrush="Black" BorderThickness="1,0,1,1">
                    <Grid>
                        <Grid.ColumnDefinitions>
                            <ColumnDefinition Width="Auto" SharedSizeGroup="DepositTypeName" />
                            <ColumnDefinition Width="Auto" />
                            <ColumnDefinition Width="Auto" SharedSizeGroup="Amount" />
                        </Grid.ColumnDefinitions>
                        <TextBlock
                            Grid.Column="0"
                            Margin="10,0,10,0"
                            Text="{Binding DepositTypeName}" />
                        <GridSplitter
                            Grid.Column="1"
                            IsHitTestVisible="False"
                            Style="{StaticResource VerticalGridSplitter}" />
                        <TextBlock
                            Grid.Column="2"
                            Margin="10,0,10,0"
                            Text="{Binding Amount}" />
                    </Grid>
                </Border>
            </DataTemplate>

            <!--  ヘッダーとデータを載せるテンプレートの定義  -->
            <ControlTemplate x:Key="MainContentTemplate" TargetType="{x:Type ListBox}">
                <!--  ヘッダーとデータをScrollViewerに収める  -->
                <ScrollViewer Grid.IsSharedSizeScope="True">
                    <i:Interaction.Behaviors>
                        <local:ScrollSyncronizingBehavior Orientation="Horizontal" ScrollGroup="Group2" />
                    </i:Interaction.Behaviors>
                    <ScrollViewer.Template>
                        <ControlTemplate TargetType="{x:Type ScrollViewer}">
                            <Grid>
                                <Grid.ColumnDefinitions>
                                    <ColumnDefinition Width="*" />
                                    <ColumnDefinition Width="Auto" />
                                </Grid.ColumnDefinitions>
                                <Grid.RowDefinitions>
                                    <RowDefinition Height="*" />
                                    <RowDefinition Height="Auto" />
                                </Grid.RowDefinitions>
                                <Grid Grid.Row="0" Grid.Column="0">
                                    <Grid.RowDefinitions>
                                        <!--  ヘッダー部分のHeightはAutoじゃないと縦スクロールが利かない。1敗  -->
                                        <RowDefinition Height="Auto" />
                                        <RowDefinition Height="Auto" />
                                        <RowDefinition Height="*" />
                                    </Grid.RowDefinitions>
                                    <!--  ヘッダー部分  -->
                                    <ScrollViewer
                                        Grid.Row="0"
                                        Content="{StaticResource HeaderTemplate}"
                                        HorizontalScrollBarVisibility="Hidden"
                                        VerticalScrollBarVisibility="Hidden">
                                        <i:Interaction.Behaviors>
                                            <local:ScrollSyncronizingBehavior Orientation="Horizontal" ScrollGroup="Group2" />
                                        </i:Interaction.Behaviors>
                                    </ScrollViewer>
                                    <!--  ヘッダーとデータの間の線  -->
                                    <GridSplitter
                                        Grid.Row="1"
                                        IsHitTestVisible="False"
                                        Style="{StaticResource HorizontalGridSplitter}" />
                                    <!--
                                        ScrollViewerのコンテンツ
                                        ここにデータ部分が乗っているはず
                                    -->
                                    <ScrollContentPresenter
                                        Name="PART_ScrollContentPresenter"
                                        Grid.Row="2"
                                        KeyboardNavigation.DirectionalNavigation="Local" />
                                </Grid>
                                <!--  右端の縦スクロールバー  -->
                                <ScrollBar
                                    Name="PART_VerticalScrollBar"
                                    Grid.Row="0"
                                    Grid.Column="1"
                                    Maximum="{TemplateBinding ScrollableHeight}"
                                    ViewportSize="{TemplateBinding ViewportHeight}"
                                    Visibility="{TemplateBinding ComputedVerticalScrollBarVisibility}"
                                    Value="{TemplateBinding VerticalOffset}" />
                                <!--  最下段の横スクロールバー  -->
                                <ScrollBar
                                    Name="PART_HorizontalScrollBar"
                                    Grid.Row="1"
                                    Grid.Column="0"
                                    Maximum="{TemplateBinding ScrollableWidth}"
                                    Orientation="Horizontal"
                                    ViewportSize="{TemplateBinding ViewportWidth}"
                                    Visibility="{TemplateBinding ComputedHorizontalScrollBarVisibility}"
                                    Value="{TemplateBinding HorizontalOffset}" />
                                <!--  右下の大きさを変えるアイコン  -->
                                <ResizeGrip
                                    Grid.Row="1"
                                    Grid.Column="1"
                                    HorizontalAlignment="Right"
                                    VerticalAlignment="Stretch"
                                    Background="{DynamicResource {x:Static SystemColors.WindowBrushKey}}" />
                            </Grid>
                        </ControlTemplate>
                    </ScrollViewer.Template>
                    <!--  データ部分  -->
                    <ItemsPresenter />
                </ScrollViewer>
            </ControlTemplate>

            <!--  ItemContainerStyleとItemTemplateの両方で同じプロパティを設定するとItemTemplateが勝つっぽい  -->
            <!--  コンテナー要素のカスタマイズ  -->
            <!--  項目ごとに区切り線を入れたり、項目が選択されたときの外観などを定義  -->
            <Style x:Key="ItemContainerStyle" TargetType="ListBoxItem">
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
                <Style.Triggers>
                    <Trigger Property="IsSelected" Value="True">
                        <Setter Property="Background" Value="Plum" />
                    </Trigger>
                    <Trigger Property="IsMouseOver" Value="True">
                        <Setter Property="Background" Value="LightGray" />
                    </Trigger>
                </Style.Triggers>
            </Style>
        </Grid.Resources>

        <ListBox
            Width="200"
            Height="100"
            Margin="0,10,0,10"
            ItemContainerStyle="{StaticResource ItemContainerStyle}"
            ItemTemplate="{StaticResource ItemTemplate}"
            ItemsSource="{Binding DepositTypeList}"
            Template="{StaticResource MainContentTemplate}"
            VirtualizingPanel.IsVirtualizing="True" />
    </Grid>
```

``` C# : このサンプルにおけるViewModel
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
    }

    public sealed class Deposit
    {
        public string DepositTypeName { get; set; }
        public decimal Amount { get; set; }
    }

    public class MainWindowViewModel
    {
        public List<Deposit> DepositTypeList { get; set; } =
            new List<Deposit>()
            {
                new Deposit(){DepositTypeName="test1",Amount=100 },
                new Deposit(){DepositTypeName="test2",Amount=200 },
                new Deposit(){DepositTypeName="test3",Amount=300 },
                new Deposit(){DepositTypeName="test4",Amount=400 },
                new Deposit(){DepositTypeName="test5",Amount=500 },
                new Deposit(){DepositTypeName="test6",Amount=600 },  
            };
    }
```

### 試行錯誤の跡

全体の形はできているが、スクロールが甘い状態。  

``` xml : 完成形の途中
<ItemsControl
    Width="200"
    Height="100"
    Margin="0,10,0,0"
    Background="Red"
    BorderBrush="Gray"
    Focusable="False"
    ItemsSource="{Binding DepositTypeList, Mode=OneWay}">
    <ItemsControl.Resources>
        <Style x:Key="VerticalGridSplitter" TargetType="{x:Type GridSplitter}">
            <Setter Property="HorizontalAlignment" Value="Stretch" />
            <Setter Property="Width" Value="1" />
            <Setter Property="Background" Value="Red" />
            <Setter Property="Padding" Value="0" />
            <Setter Property="Margin" Value="0" />
        </Style>
    </ItemsControl.Resources>
    <ItemsControl.Template>
        <ControlTemplate TargetType="{x:Type ItemsControl}">
            <ScrollViewer
                Focusable="False"
                HorizontalScrollBarVisibility="Auto"
                VerticalScrollBarVisibility="Auto">
                <Grid>
                    <ItemsPresenter Margin="0,30,0,0" />
                    <Border
                        VerticalAlignment="Top"
                        BorderBrush="Black"
                        BorderThickness="1,1,1,1"
                        IsHitTestVisible="False">
                        <Grid Background="Yellow" Grid.IsSharedSizeScope="True">
                            <Grid.ColumnDefinitions>
                                <ColumnDefinition Width="3*" MinWidth="100" />
                                <ColumnDefinition Width="Auto" />
                                <ColumnDefinition Width="4*" MinWidth="140" />
                            </Grid.ColumnDefinitions>
                            <Label
                                Grid.Column="0"
                                HorizontalContentAlignment="Center"
                                Content="精算方法"
                                FontSize="15" />
                            <GridSplitter Grid.Column="1" Style="{StaticResource VerticalGridSplitter}" />
                            <Label
                                Grid.Column="2"
                                HorizontalContentAlignment="Center"
                                Content="精算金額"
                                FontSize="15" />
                        </Grid>
                    </Border>
                </Grid>
            </ScrollViewer>
        </ControlTemplate>
    </ItemsControl.Template>
    <ItemsControl.ItemTemplate>
        <DataTemplate>
            <Border BorderBrush="Black" BorderThickness="1,0,1,1">
                <Grid Height="Auto">
                    <Grid.ColumnDefinitions>
                        <ColumnDefinition Width="3*" MinWidth="100" />
                        <ColumnDefinition Width="Auto" />
                        <ColumnDefinition Width="4*" MinWidth="140" />
                    </Grid.ColumnDefinitions>
                    <TextBlock Grid.Column="0" Text="{Binding DepositTypeName}" />
                    <GridSplitter Grid.Column="1" Style="{StaticResource VerticalGridSplitter}" />
                    <TextBlock Grid.Column="2" Text="{Binding Amount}" />
                </Grid>
            </Border>
        </DataTemplate>
    </ItemsControl.ItemTemplate>
</ItemsControl>
```

一番最初に完成したと思ったやつ。  
ヘッダーもスクロールできることに気が付いたのでボツになった。  

``` XML : 完成1
<ItemsControl
    Width="200"
    Height="100"
    Margin="0,10,0,0"
    Focusable="False"
    Grid.IsSharedSizeScope="True"
    ItemsSource="{Binding DepositTypeList, Mode=OneWay}"
    ScrollViewer.CanContentScroll="True"
    VirtualizingPanel.IsVirtualizing="True">
    <ItemsControl.Resources>
        <Style x:Key="HorizontalGridSplitter" TargetType="{x:Type GridSplitter}">
            <Setter Property="Height" Value="1" />
            <Setter Property="HorizontalAlignment" Value="Stretch" />
            <Setter Property="Background" Value="Red" />
        </Style>
        <Style x:Key="VerticalGridSplitter" TargetType="{x:Type GridSplitter}">
            <Setter Property="HorizontalAlignment" Value="Stretch" />
            <Setter Property="Width" Value="1" />
            <Setter Property="Background" Value="Red" />
        </Style>
    </ItemsControl.Resources>
    <ItemsControl.Template>
        <ControlTemplate TargetType="{x:Type ItemsControl}">
            <ScrollViewer
                Focusable="False"
                HorizontalScrollBarVisibility="Auto"
                VerticalScrollBarVisibility="Auto">
                <Grid>
                    <Grid.RowDefinitions>
                        <RowDefinition Height="Auto" />
                        <RowDefinition Height="Auto" />
                        <RowDefinition Height="*" />
                    </Grid.RowDefinitions>
                    <!--  ヘッダー部分  -->
                    <Border
                        Grid.Row="0"
                        VerticalAlignment="Top"
                        BorderBrush="Black"
                        BorderThickness="1,1,1,0">
                        <Grid Background="Yellow">
                            <Grid.ColumnDefinitions>
                                <ColumnDefinition Width="Auto" SharedSizeGroup="DepositTypeName" />
                                <ColumnDefinition Width="Auto" />
                                <ColumnDefinition Width="Auto" SharedSizeGroup="Amount" />
                            </Grid.ColumnDefinitions>
                            <Label
                                Grid.Column="0"
                                HorizontalContentAlignment="Center"
                                Content="精算方法"
                                FontSize="15" />
                            <GridSplitter Grid.Column="1" Style="{StaticResource VerticalGridSplitter}" />
                            <Label
                                Grid.Column="2"
                                HorizontalContentAlignment="Center"
                                Content="精算金額"
                                FontSize="15" />
                        </Grid>
                    </Border>
                    <!--  ヘッダーとデータの間の線  -->
                    <GridSplitter
                        Grid.Row="1"
                        IsHitTestVisible="False"
                        Style="{StaticResource HorizontalGridSplitter}" />
                    <!--  データ部分定義  -->
                    <!--<ItemsPresenter Grid.Row="2" />-->
                    <!-- 仮想化に対応 -->
                    <VirtualizingStackPanel Grid.Row="2" IsItemsHost="True" />
                </Grid>
            </ScrollViewer>
        </ControlTemplate>
    </ItemsControl.Template>
    <!--  データ部分本体  -->
    <ItemsControl.ItemTemplate>
        <DataTemplate>
            <Border
                BorderBrush="Black"
                BorderThickness="1,0,1,1"
                IsHitTestVisible="False">
                <Grid Height="Auto">
                    <Grid.ColumnDefinitions>
                        <ColumnDefinition Width="Auto" SharedSizeGroup="DepositTypeName" />
                        <ColumnDefinition Width="Auto" />
                        <ColumnDefinition Width="Auto" SharedSizeGroup="Amount" />
                    </Grid.ColumnDefinitions>
                    <TextBlock
                        Grid.Column="0"
                        Margin="5,0,10,0"
                        Text="{Binding DepositTypeName}" />
                    <GridSplitter Grid.Column="1" Style="{StaticResource VerticalGridSplitter}" />
                    <TextBlock
                        Grid.Column="2"
                        Margin="5,0,10,0"
                        Text="{Binding Amount}" />
                </Grid>
            </Border>
        </DataTemplate>
    </ItemsControl.ItemTemplate>
</ItemsControl>
```

---

## 横スクロール問題

[ItemsControlをDataGridみたいに使う]研究の一環としてまとめ。  

縦スクロールにヘッダーを含めるのはできたけど、それだと、データ部分とヘッダー部分で別れるので、横スクロールが同期しない問題が発生した。  
その時にいろいろまとめた。  

wpf scrollviewer overlap  

[WPF: How to prevent scrollbar from overlaying headers in list type views](https://stackoverflow.com/questions/52725066/wpf-how-to-prevent-scrollbar-from-overlaying-headers-in-list-type-views)
[GridViewHeaderRowPresenter scrolls with custom TreeListView control](https://stackoverflow.com/questions/28605052/gridviewheaderrowpresenter-scrolls-with-custom-treelistview-control)  
[【WPF】ListBoxからListViewもどきを作る（再考）](http://pro.art55.jp/?eid=1075922)  

ScrollViewで囲むのはデータ部分だけにして、スタイルをいじってヘッダーまで長さを延長することで、疑似的にDataGridみたいにした案。  
また完璧にできたと思ったけど、今度は横スクロールが聞かなくなってしまった。  
これからわかるに、どう頑張ってもコードビハインドは必要そうだということだ。  
XAML上で2つのスクロールを連動させるための添付プロパティみたいなのがあれば話は別ですが、どこのサイトもビヘイビアでやってるのでそんなものはないのでしょう。  

---

## ItemsControlでIsSharedSizeScopeを適応させる方法

[ItemsControlをDataGridみたいに使う]研究の一環としてまとめ。  
理想の動作を実現する過程において、ヘッダーのGritSpliterを操作してもデータ本体まで伝播しない問題が解決できなかった。  
元祖DataGridみたいに使うサイトでやっているサンプルでは出来ていたのだが、そもそもの構造が違っていたのでどのように適応していいのか分からなかった。  
GridのIsSharedSizeScopeなるものがキーであるのは間違いなかったので、それを素直にItemsControlに適応することはできないか探していたら、ズバリな記事に行き当たり、その通りに実装したら見事に解決できたのでまとめる。  

結論としてItemsControl自信に Grid.IsSharedSizeScope="True" を適応すればよいだけの話であった。  

[WPF Grid.IsSharedSizeScope in ItemsControl and ScrollViewer](https://stackoverflow.com/questions/54922513/wpf-grid-issharedsizescope-in-itemscontrol-and-scrollviewer)  
wpf itemscontrol issharedsizescope  

``` XML
<!-- ItemsControl本体にGrid.IsSharedSizeScope="True" は適応可能 -->
<ItemsControl ItemsSource="{Binding}" Grid.IsSharedSizeScope="True" IsTabStop="False">
    <ItemsControl.ItemTemplate>
        <DataTemplate>
            <Grid >
                <Grid.ColumnDefinitions>
                    <ColumnDefinition Width="30"/>
                    <ColumnDefinition Width="40"/>
                    <!-- 後は動きを共有したい列をグループ名でまとめればOK -->
                    <ColumnDefinition Width="Auto" SharedSizeGroup="sharedWidth"/>
                    <ColumnDefinition Width="*"/>
                </Grid.ColumnDefinitions>
```

---

## WPFのScrollViewerやScrollBarのスクロール位置を同期させる

[WPFのScrollViewerやScrollBarのスクロール位置を同期させる](https://days-of-programming.blogspot.com/2015/01/wpfscrollviewerscrollbar.html)  

ItemsControlとはずれるが、DataGridサンプルで使う必要があったのでこちらにまとめる。  

``` XML : 使い方
<ScrollViewer HorizontalScrollBarVisibility="Hidden" VerticalScrollBarVisibility="Hidden">
    <!-- 中略 -->
    <i:Interaction.Behaviors>
        <!-- スクロールを同期させたいモノをScrollGroupに同じ名前で登録する -->
        <b:ScrollSyncronizingBehavior ScrollGroup="Group1" Orientation="Vertical" />
    </i:Interaction.Behaviors>
</ScrollViewer>
```

``` C# : ScrollSyncronizingBehavior
// Behaviorを使うためにはnugetからMicrosoft.Xaml.Behaviorsをインストールしないといけない
// <https://livealoneblog.com/wpf-behavior/>
using Microsoft.Xaml.Behaviors;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace ItemsControl2
{
    /// <summary>
    /// ScrollViewerのスクロール位置を同期するビヘイビア
    /// </summary>
    public class ScrollSyncronizingBehavior : Behavior<Control>
    {
        static readonly Dictionary<string, List<Control>> SyncGroups = new Dictionary<string, List<Control>>();

        private static readonly DependencyProperty ScrollGroupProperty = DependencyProperty.Register(
            "ScrollGroup",
            typeof(string),
            typeof(ScrollSyncronizingBehavior),
            new FrameworkPropertyMetadata((d, e) =>
            {
                if (d is ScrollSyncronizingBehavior me)
                {
                    me.RemoveSyncGroup((string)e.OldValue);
                    me.AddSyncGroup((string)e.NewValue);
                }
            })
        );
        /// <summary>
        /// スクロールグループ
        /// </summary>
        public string ScrollGroup
        {
            get { return (string)GetValue(ScrollGroupProperty); }
            set { SetValue(ScrollGroupProperty, value); }
        }


        private static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
            "Orientation",
            typeof(Orientation),
            typeof(ScrollSyncronizingBehavior),
            new FrameworkPropertyMetadata()
        );
        /// <summary>
        /// スクロールの向き
        /// </summary>
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AddSyncGroup(ScrollGroup);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            RemoveSyncGroup(ScrollGroup);
        }

        /// <summary>
        /// 同期グループに追加するメソッド
        /// </summary>
        /// <param name="GroupName">グループ名</param>
        /// <returns>成功したかどうか</returns>
        private bool AddSyncGroup(string GroupName)
        {
            if (string.IsNullOrEmpty(ScrollGroup))
            {
                return false;
            }

            if (AssociatedObject is ScrollViewer sv)
            {
                add();
                sv.ScrollChanged += ScrollViewerScrolled;
                return true;
            }
            if (AssociatedObject is ScrollBar sb)
            {
                add();
                sb.ValueChanged += ScrollBarScrolled;
                return true;
            }
            return false;

            void add()
            {
                if (!SyncGroups.ContainsKey(GroupName))
                {
                    SyncGroups.Add(GroupName, new List<Control>());
                }
                SyncGroups[GroupName].Add(AssociatedObject);
            }
        }

        /// <summary>
        /// 同期グループから削除するメソッド
        /// </summary>
        /// <param name="GroupName">グループ名</param>
        /// <returns>成功したかどうか</returns>
        private bool RemoveSyncGroup(string GroupName)
        {
            if (!string.IsNullOrEmpty(ScrollGroup)
                && (AssociatedObject is ScrollViewer || AssociatedObject is ScrollBar))
            {
                if (AssociatedObject is ScrollViewer sv)
                {
                    sv.ScrollChanged -= ScrollViewerScrolled;
                }
                if (AssociatedObject is ScrollBar sb)
                {
                    sb.ValueChanged -= ScrollBarScrolled;
                }

                SyncGroups[GroupName].Remove(AssociatedObject);
                if (SyncGroups[GroupName].Count == 0)
                {
                    SyncGroups.Remove(GroupName);
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// ScrollViewerの場合の変更通知イベントハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScrollViewerScrolled(object sender, ScrollChangedEventArgs e)
        {
            UpdateScrollValue(
                sender,
                Orientation == Orientation.Horizontal ? e.HorizontalOffset : e.VerticalOffset
            );
        }

        /// <summary>
        /// ScrollBarの場合の変更通知イベントハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScrollBarScrolled(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UpdateScrollValue(sender, e.NewValue);
        }

        /// <summary>
        /// スクロール値を設定するメソッド
        /// </summary>
        /// <param name="sender">スクロール値を更新してきたコントロール</param>
        /// <param name="NewValue">新しいスクロール値</param>
        private void UpdateScrollValue(object sender, double NewValue)
        {
            IEnumerable<Control> others = SyncGroups[ScrollGroup].Where(p => p != sender);

            foreach (ScrollBar sb in others.OfType<ScrollBar>().Where(p => p.Orientation == Orientation))
            {
                sb.Value = NewValue;
            }
            foreach (ScrollViewer sv in others.OfType<ScrollViewer>())
            {
                if (Orientation == Orientation.Horizontal)
                {
                    sv.ScrollToHorizontalOffset(NewValue);
                }
                else
                {
                    sv.ScrollToVerticalOffset(NewValue);
                }
            }
        }
    }
}
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
