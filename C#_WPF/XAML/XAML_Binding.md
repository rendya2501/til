# XAML_Binding

---

## DataContext

>データ・バインディングとは、MVVMパターンでいうViewとViewModelを結び付けるために提供されている仕組み。  
>ViewとViewModel間のやりとりは**DataContext**というプロパティを介してやりとりします。  
>Viewで表示したいViewModelのデータ・ソースをDataContextプロパティに渡すだけで、結び付けることができるのです。  
[データ・バインディングを理解する](https://marikooota.hatenablog.com/entry/2017/05/30/002059)  

→  
DataContextってプロパティなんだな。  
でもって、ViewとViewModelを結びつけるための橋ってイメージでいいかな。  

``` XML : Xamlで設定する場合
<!-- 名前空間:クラス名  の形で指定するので名前空間にViewModelを通しておく-->
<!-- 例 xmlns:vm="clr-namespace:WpfApp.ViewModel -->

<Window.DataContext>
    <local:ViewModel />
</Window.DataContext>
```

``` C# : コードビハインドで設定する場合
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ViewModel();
        }
    }
```

---

## 他のコントロールのプロパティを使いたい場合

自分自身のプロパティ  
self

親のコントロールのプロパティ  
`プロパティ = "{Binding RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type 親のコントロール名}}, Path=バインドさせたいプロパティ名}"`  

``` xml
<c1:Column
    HorizontalAlignment="Left"
    VerticalAlignment="Center"
    Binding="{Binding Name}"
    Foreground="Black"
    Header="氏名"
    HeaderHorizontalAlignment="Center"
    HeaderVerticalAlignment="Center"
    IsReadOnly="True">
    <c1:Column.CellTemplate>
        <DataTemplate>
            <Grid>
                <Grid.Style>
                    <Style TargetType="Grid">
                        <Style.Triggers>
                            <DataTrigger Binding="{Binding MessageText, Mode=OneWay, Converter={StaticResource NullOrEmptyToBoolConverter}}" Value="false">
                                <Setter Property="Background" Value="#CAFF9040" />
                            </DataTrigger>
                        </Style.Triggers>
                    </Style>
                </Grid.Style>
                <TextBlock
                    Padding="8,0,0,0"
                    VerticalAlignment="Center"
                    Foreground="{Binding RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type c1:Column}}, Path=Foreground}"
                    Text="{Binding ReservationPlayerName}" />
            </Grid>
        </DataTemplate>
    </c1:Column.CellTemplate>
</c1:Column>
```

[【WPF】RelativeSource(バインディング)の使い方メモ](https://qiita.com/tera1707/items/73cda312b7cd9c4df40d)  
[Q092. Binding.RelativeSource の使い方がよくわからない](https://hilapon.hatenadiary.org/entry/20130405/1365143758)  

---

## ElementNameでバインドする

`Property="{Binding BindValue, ElementName= elementName}"`  

``` xml
<Grid>
    <Grid.ColumnDefinitions>
        <ColumnDefinition Width="Auto"/>
        <ColumnDefinition Width="Auto"/>
    </Grid.ColumnDefinitions>
    <Slider x:Name="slider" Width="300" Height="23" Margin="5"
            HorizontalAlignment="Left" VerticalAlignment="Top"/>
    <TextBlock Grid.Column="1" Width="70" Height="23" Margin="5"
               HorizontalAlignment="Left" VerticalAlignment="Top"
               Text="{Binding Value, ElementName=slider}"/>
</Grid>
```

[[C# WPF]他のコントロールのプロパティをBindする](http://cswpf.seesaa.net/article/313843359.html)  

---

## 自分自身のItemsSourceのCountをXAML上で使用する方法

自分自身の要素が1件も無かったらDataTriggerでEnableをFalseにしたい。  

``` XML
<c1:C1MultiSelect.Style>
    <Style TargetType="{x:Type c1:C1MultiSelect}">
        <Style.Triggers>
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

やっぱりそれなりに需要はある模様。  
[Bind Count of ItemsSource of an ItemsControl in a TextBlock using WPF](https://stackoverflow.com/questions/39482829/bind-count-of-itemssource-of-an-itemscontrol-in-a-textblock-using-wpf)  

---

## BindingのStringFormat

[【WPF】Bindingと文字列を結合して表示する方法4選](https://threeshark3.com/binding-string-concat/)  

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
        Text="{Binding Context.Date, StringFormat={}{0:yyyy/MM/dd}, ElementName=TestDialog, Mode=OneWay}" />
    <TextBlock
        Width="300"
        Margin="10,0,0,0"
        VerticalAlignment="Center"
        Background="AliceBlue"
        Text="{Binding Context.Name, ElementName=TestDialog, Mode=OneWay}" />
</StackPanel>

<!-- MultiBindingを使うことで1つのTextBlockで2つの内容を表示することができる -->
<TextBlock
    Width="300"
    HorizontalAlignment="Left"
    VerticalAlignment="Center">
    <TextBlock.Text>
        <MultiBinding StringFormat="{}{0:yyyy/MM/dd}  {1}">
            <Binding
                ElementName="TestDialog"
                Mode="OneWay"
                Path="Context.Date" />
            <Binding
                ElementName="TestDialog"
                Mode="OneWay"
                Path="Context.Name" />
        </MultiBinding>
    </TextBlock.Text>
</TextBlock>
```

---

## MultiBindingで単純に2つの要素をつなげる例

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

[Binding multiple values with a MultiBinding](https://riptutorial.com/wpf/example/23413/binding-multiple-values-with-a-multibinding)  

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
