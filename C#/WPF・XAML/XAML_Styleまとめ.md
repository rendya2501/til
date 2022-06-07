# XAML_Styleまとめ

## Style

[WPF4.5入門 その50 「Style」](https://blog.okazuki.jp/entry/2014/09/04/200304)  

コントロールに設定するプロパティの値のセットを集めるためのもの。  
Styleは、Setterというオブジェクトを使って、どのプロパティにどんな値を設定するか指定できます。  
スタイルを設定するには、コントロールのStyleプロパティにStyleを設定します。  
ResourceやResourceDictionaryに定義することでコントロールのプロパティの設定を複数のコントロールで共通化することが出来ます。  

---

## BaseOn

[[WPF/xaml]BasedOnを使って元のstyleを受け継ぐ](https://qiita.com/tera1707/items/3c4f598c5d022e4987a2)  

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
    </Window.Resources>

    <StackPanel>
        <!-- メイリョウ + 黄 + 大きさ12 -->
        <TextBlock Style="{StaticResource TitleTextStyle}" Text="タイトル" />
        <!-- メイリョウ + 赤 + 大きさ24 -->
        <TextBlock Style="{StaticResource DefaultTextStyle}" Text="デフォルトのテキスト" />
        <!-- メイリョウ + 赤 + 大きさ12 + 斜め -->
        <TextBlock Style="{StaticResource ThirdTextStyle}" Text="第3のテキスト" />
    </StackPanel>
</Window>
```

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

## Resourceで定義したStyleを当てる方法

[WPF のリソース](http://var.blog.jp/archives/67298406.html)  
[[WPF/xaml]リソースディクショナリを作って、画面のコントロールのstyleを変える](https://qiita.com/tera1707/items/a462678cdfb61a87334b)  
[[WPF] Styleでできることと書き方](https://qiita.com/tera1707/items/cb8ad4c40107ae25b565)  

``` XML
<ResourceDictionary xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                    xmlns:AR="clr-namespace:GrapeCity.ActiveReports.Viewer.Wpf;assembly=GrapeCity.ActiveReports.Viewer.Wpf.v12"
                    xmlns:ViewModel="clr-namespace:GrapeCity.ActiveReports.ViewModel;assembly=GrapeCity.ActiveReports.Viewer.Wpf.v12"
                    xmlns:Converters="clr-namespace:GrapeCity.ActiveReports.Converters;assembly=GrapeCity.ActiveReports.Viewer.Wpf.v12"
                    xmlns:View="clr-namespace:GrapeCity.ActiveReports.Viewer.Wpf.View;assembly=GrapeCity.ActiveReports.Viewer.Wpf.v12"
                    xmlns:converters2="clr-namespace:GrapeCity.ActiveReports.Framework.Converters;assembly=GrapeCity.ActiveReports.Viewer.Wpf.v12"
                    xmlns:MultiPage="clr-namespace:GrapeCity.ActiveReports.Viewer.Wpf.View.MultiPage;assembly=GrapeCity.ActiveReports.Viewer.Wpf.v12"
                    xmlns:Framework="clr-namespace:GrapeCity.ActiveReports.Framework;assembly=GrapeCity.ActiveReports.Viewer.Wpf.v12"
                    xmlns:fwi="clr-namespace:GrapeCity.ActiveReports.Framework.Implementation;assembly=GrapeCity.ActiveReports.Viewer.Wpf.v12"
                    xmlns:d="http://schemas.microsoft.com/expression/blend/2008" xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
                    xmlns:toolkit="clr-namespace:GrapeCity.ActiveReports.Viewer.Wpf.Framework.Toolkit;assembly=GrapeCity.ActiveReports.Viewer.Wpf.v12"
                    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                    xmlns:localCommand="clr-namespace:RN3.Report.Command">
    <BitmapImage x:Key="pdfImage" UriSource="/RN3.Report;component/Image/Pdf16x16.ico" />
    <localCommand:PdfExportCommand x:Key="pdfExportCommand" />
    <Style TargetType="View:ErrorPanel">
        <Setter Property="Template">
            <Setter.Value>
                <ControlTemplate TargetType="View:ErrorPanel">
                    <Grid>
                        <Grid.Resources>
                            <!-- globals -->
                            <AR:LocalResources x:Key="res" />
                            <Converters:ResourceImageConverter x:Key="ResourceImageConverter" />
                            <Converters:ErrorInfoIconConverter x:Key="ErrorInfoIconConverter" />
                            <converters2:ExceptionInfoConverter x:Key="ExceptionInfoConverter" />
                            <converters2:ExceptionDetailsConverter x:Key="ExceptionDetailsConverter" />
                            <Converters:CommandConverter x:Key="CommandConverter" />
                            <Converters:ErrorInfoCollectionConverter x:Key="ErrorInfoCollectionConverter" />
                            <ViewModel:VisibilityConverter x:Key="VisibilityConverter" />
                            <!-- sliding panels -->
                            <Style x:Key="SlidingPanelBorder" TargetType="Border">
                                <Setter Property="BorderThickness" Value="1" />
                                <Setter Property="CornerRadius" Value="2" />
                                <Setter Property="Background" Value="LightGray" />
                                <Setter Property="BorderBrush" Value="DarkGray" />
                                <Setter Property="Padding" Value="3" />
                            </Style>
                            <Style x:Key="SlidingPanelButton" TargetType="ButtonBase">
                                <Setter Property="Margin" Value="4,0" />
                                <Setter Property="VerticalAlignment" Value="Center" />
                            </Style>
                            <!-- sliding panels -->
                            <Style x:Key="CloseButton" TargetType="Button">
                                <Setter Property="Template">
                                    <Setter.Value>
                                        <ControlTemplate TargetType="Button">
                                            <Button Command="{TemplateBinding Command}" Margin="0" Padding="0" VerticalAlignment="Center" Height="22">
                                                <Border Margin="4,0" MaxWidth="14" CornerRadius="1">
                                                    <Image Stretch="None" RenderOptions.BitmapScalingMode="Fant" Source="{Binding Source={StaticResource res}, Converter={StaticResource ResourceImageConverter}, Path=Resources.closeerror}" />
                                                </Border>
                                            </Button>
                                        </ControlTemplate>
                                    </Setter.Value>
                                </Setter>
                            </Style>
                            <Style x:Key="ErrorMessageView" TargetType="ScrollViewer">
                                <Setter Property="HorizontalScrollBarVisibility" Value="Auto" />
                                <Setter Property="VerticalScrollBarVisibility" Value="Auto" />
                                <Setter Property="BorderThickness" Value="1" />
                                <Setter Property="BorderBrush" Value="DarkGray" />
                                <Setter Property="Background" Value="White" />
                            </Style>
                            <Style x:Key="ErrorMessageTreeView" TargetType="TreeView">
                                <Setter Property="BorderThickness" Value="0" />
                                <Setter Property="Margin" Value="0" />
                                <Setter Property="Padding" Value="0" />
                                <Setter Property="Background" Value="White" />
                            </Style>
                            <Converters:CommandAdapter x:Key="HideDialog" Command="{Binding HideDialog}" />
                        </Grid.Resources>
                        <Grid.InputBindings>
                            <KeyBinding Key="Escape" Command="{StaticResource HideDialog}" />
                        </Grid.InputBindings>
                        <Border DockPanel.Dock="Top" Style="{StaticResource SlidingPanelBorder}" Visibility="{Binding IsPanelVisible, Converter={StaticResource VisibilityConverter}}">
                            <Border.Resources>
                                <AR:LocalResources x:Key="res" />
                                <Converters:ResourceImageConverter x:Key="ResourceImageConverter" />
                            </Border.Resources>
                            <Grid>
                                <Grid.RowDefinitions>
                                    <RowDefinition Height="Auto" />
                                    <RowDefinition Height="Auto" />
                                </Grid.RowDefinitions>
                                <DockPanel Grid.Row="0">
                                    <Image Source="{Binding Source={StaticResource res}, Converter={StaticResource ResourceImageConverter}, Path=Resources.Attention}" Height="16" Margin="4,0" DockPanel.Dock="Left" Stretch="None" RenderOptions.BitmapScalingMode="Fant"></Image>
                                    <Button DockPanel.Dock="Right" Command="{Binding HidePanel, Converter={StaticResource CommandConverter}}" Style="{StaticResource CloseButton}" AutomationProperties.AutomationId="HideErrorPanelButton" />
                                    <Button Content="{Binding Source={StaticResource res}, Path=Resources.ErrorPanel_ShowDetailsButton}" Command="{Binding ShowDialog, Converter={StaticResource CommandConverter}}" DockPanel.Dock="Right" Style="{StaticResource SlidingPanelButton}" Visibility="{Binding IsDialogVisibleInvert, Converter={StaticResource VisibilityConverter}}" AutomationProperties.AutomationId="ShowDetailsErrorPanelButton" />
                                    <Button x:Name="HideDetailsButton" Content="{Binding Source={StaticResource res}, Path=Resources.ErrorPanel_HideDetailsButton}" Command="{Binding HideDialog, Converter={StaticResource CommandConverter}}" DockPanel.Dock="Right" Style="{StaticResource SlidingPanelButton}" Visibility="{Binding IsDialogVisible, Converter={StaticResource VisibilityConverter}}" AutomationProperties.AutomationId="HideDetailsErrorPanelButton" />
                                    <TextBlock Text="{Binding MessageText}" VerticalAlignment="Center" TextWrapping="NoWrap" TextTrimming="WordEllipsis" AutomationProperties.AutomationId="ErrorMessageTextErrorPanel" />
                                </DockPanel>
                                <Border Grid.Row="1" Visibility="{Binding IsDialogVisible, Converter={StaticResource VisibilityConverter}}" Height="200" Style="{StaticResource SlidingPanelBorder}">
                                    <DockPanel>
                                        <StackPanel Orientation="Horizontal" DockPanel.Dock="Bottom" HorizontalAlignment="Right">
                                            <Button Content="{Binding Source={StaticResource res}, Path=Resources.ErrorPanel_ClearAllButton}" Command="{Binding ClearErrors, Converter={StaticResource CommandConverter}}" Grid.Column="1" Padding="30,2" Margin="4" AutomationProperties.AutomationId="ClearAllErrorPanelButton" />
                                            <Button Content="{Binding Source={StaticResource res}, Path=Resources.ErrorPanel_CopyButton}" Command="{Binding Copy, Converter={StaticResource CommandConverter}}" Grid.Column="1" Padding="10,2" Margin="4" AutomationProperties.AutomationId="CopyErrorPanelButton" />
                                        </StackPanel>
                                        <Grid>
                                            <Grid.ColumnDefinitions>
                                                <ColumnDefinition Width="200" />
                                                <ColumnDefinition Width="Auto " />
                                                <ColumnDefinition Width="*" />
                                            </Grid.ColumnDefinitions>
                                            <ScrollViewer Style="{StaticResource ErrorMessageView}" Grid.Column="0">
                                                <TreeView x:Name="ErrorsTree" ItemsSource="{Binding Errors, Converter={StaticResource ErrorInfoCollectionConverter}}" Style="{StaticResource ErrorMessageTreeView}">
                                                    <TreeView.ItemTemplate>
                                                        <HierarchicalDataTemplate ItemsSource="{Binding Exception}">
                                                            <StackPanel Orientation="Horizontal">
                                                                <Image RenderOptions.BitmapScalingMode="Fant" Source="{Binding Severity, Converter={StaticResource ErrorInfoIconConverter}}" HorizontalAlignment="Left" Margin="0,0,6,0" />
                                                                <TextBlock Text="{Binding Exception, Converter={StaticResource ExceptionInfoConverter}}" />
                                                            </StackPanel>
                                                        </HierarchicalDataTemplate>
                                                    </TreeView.ItemTemplate>
                                                </TreeView>
                                            </ScrollViewer>
                                            <GridSplitter Grid.Column="1" ResizeDirection="Columns" Height="Auto" Width="3" HorizontalAlignment="Stretch" VerticalAlignment="Stretch" />
                                            <ScrollViewer x:Name="ExtendedInfoView" Style="{StaticResource ErrorMessageView}" Grid.Column="2">
                                                <TextBlock Text="{Binding ElementName=ErrorsTree,Path=SelectedItem.Exception, Converter={StaticResource ExceptionDetailsConverter}}" />
                                            </ScrollViewer>
                                        </Grid>
                                    </DockPanel>
                                </Border>
                            </Grid>
                        </Border>
                    </Grid>
                </ControlTemplate>
            </Setter.Value>
        </Setter>
    </Style>
</ResourceDictionary>
```
