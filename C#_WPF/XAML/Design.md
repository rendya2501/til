# XAML 見た目に関するまとめ

---

## 虫眼鏡アイコンをセットしたボタンの実装方法

何のことはない。ただのボタンでスタイルは共通で定義されているものを使っているだけ。  

``` XML
<Button : 
    Command="{Binding ShowAttendeeListCommand}"
    IsTabStop="False"
    Style="{StaticResource SearchButton}" />
```

``` XML
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

## 画像の追加

プロジェクトに画像を追加して、パスを通したにも関わらず表示されない場合があったので、その時対処したことをまとめる。  

- binとobjを削除する必要があった模様。  
- フォルダに追加するだけだと認識されない。  
- その場合はすべて表示にして、右クリックして追加する。  

---

## 縦横の大きさを動的に変動させる方法

[WPF ScrollViewer with Grid inside and dynamic size](https://stackoverflow.com/questions/48529723/wpf-scrollviewer-with-grid-inside-and-dynamic-size)  

FlexGridやScrollViewer等、コントロールの大きさが画面の大きさと連動して欲しい時にどのように書けばいいのか、テンプレートとなる知識がなかったのでまとめ。  
やっぱりGrid先輩が強すぎる。  
とりあえずGridで囲んでおけみたいなところあるな。  

``` XML
<Window x:Class="WpfTest.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Width="200"
        Height="300">

  <!-- Layout Root -->
  <Grid>
    <Grid.RowDefinitions>
      <RowDefinition Height="Auto" />
      <RowDefinition Height="*" />
      <RowDefinition Height="Auto" />
    </Grid.RowDefinitions>

    <!-- Header Panel -->
    <Border Grid.Row="0" Background="#CCCCCC" Padding="11">
      <!-- Replace this TextBlock with your header content. -->
      <TextBlock Text="Header Content" TextAlignment="Center" />
    </Border>

    <!-- Body Panel -->
    <Grid Grid.Row="1" Background="#CCCCFF">
      <Grid.RowDefinitions>
        <RowDefinition Height="Auto" />
        <RowDefinition Height="*" />
      </Grid.RowDefinitions>

      <Border Grid.Row="0" Background="#FFCCCC" Padding="11">
        <!-- Replace this TextBlock with your upper body content. -->
        <TextBlock Text="Upper Body Content" TextAlignment="Center" />
      </Border>

      <ScrollViewer Grid.Row="1" Padding="11"
                    VerticalScrollBarVisibility="Auto">

        <!-- Replace this Border with your scrollable content. -->
        <Border MinHeight="200">
          <Border.Background>
            <RadialGradientBrush RadiusY="1" RadiusX="1" Center="0.5,0.5">
              <GradientStop Color="White" Offset="0" />
              <GradientStop Color="Black" Offset="1" />
            </RadialGradientBrush>
          </Border.Background>
        </Border>

      </ScrollViewer>
    </Grid>

    <!-- Footer Panel -->
    <Border Grid.Row="2" Background="#CCFFCC" Padding="11">
      <!-- Replace this TextBlock with your footer content. -->
      <TextBlock Text="Footer Content" TextAlignment="Center" />
    </Border>
  </Grid>

</Window>
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

## DataGrid – アクティブセルの枠線を消す（C# WPF)

<http://once-and-only.com/programing/c/datagrid-%E3%82%A2%E3%82%AF%E3%83%86%E3%82%A3%E3%83%96%E3%82%BB%E3%83%AB%E3%81%AE%E6%9E%A0%E7%B7%9A%E3%82%92%E6%B6%88%E3%81%99%EF%BC%88c-wpf/>  

ScrollViewerにフォーカスが当たって点線の枠が表示されてしまう問題が発生した。  
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
