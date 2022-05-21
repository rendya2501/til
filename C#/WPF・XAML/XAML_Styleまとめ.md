# XAML_Styleまとめ

[[WPF] Styleでできることと書き方](https://qiita.com/tera1707/items/cb8ad4c40107ae25b565)  

## BaseOn

[[WPF/xaml]BasedOnを使って元のstyleを受け継ぐ](https://qiita.com/tera1707/items/3c4f598c5d022e4987a2)  

---

## Resourceで定義したStyleを当てる方法

[WPF のリソース](http://var.blog.jp/archives/67298406.html)  
[[WPF/xaml]リソースディクショナリを作って、画面のコントロールのstyleを変える](https://qiita.com/tera1707/items/a462678cdfb61a87334b)  
[[WPF] Styleでできることと書き方](https://qiita.com/tera1707/items/cb8ad4c40107ae25b565)  

---

## スタイルの定義

``` XML
    <StackPanel Margin="20" Orientation="Vertical">
        <StackPanel.Resources>
            <Style
                x:Key="TitleLabel"
                BasedOn="{StaticResource {x:Type ctrl:CustomLabel}}"
                TargetType="ctrl:CustomLabel">
                <Setter Property="Margin" Value="0,0,10,0" />
                <Setter Property="HorizontalContentAlignment" Value="Right" />
                <Setter Property="VerticalAlignment" Value="Center" />
            </Style>
            <Style
                x:Key="TitleLabelCol3"
                BasedOn="{StaticResource TitleLabel}"
                TargetType="ctrl:CustomLabel">
                <Setter Property="Width" Value="65" />
            </Style>
        </StackPanel.Resources>
    </StackPanel>

    <!-- 使うとき1 -->
      <ctrl:CustomLabel Content="委託" Style="{StaticResource TitleLabelCol3}" />
```

``` XML
    <Grid.Resources>
        <Style x:Key="TitleLabel" TargetType="{x:Type Button}">
            <Style.Triggers>
                <Trigger Property="IsMouseOver" Value="True">
                    <Setter Property="Background" Value="{StaticResource MahApps.Brushes.WindowButtonCommands.Background.MouseOver}" />
                </Trigger>
                <Trigger Property="IsPressed" Value="True">
                    <Setter Property="Background" Value="{StaticResource MahApps.Brushes.AccentBase}" />
                    <Setter Property="Foreground" Value="{StaticResource MahApps.Brushes.IdealForeground}" />
                </Trigger>
                <Trigger Property="IsEnabled" Value="False">
                    <Setter Property="Foreground" Value="{StaticResource MahApps.Brushes.IdealForegroundDisabled}" />
                </Trigger>
            </Style.Triggers>
            <Setter Property="Background" Value="{StaticResource MahApps.Brushes.Transparent}" />
            <Setter Property="Foreground" Value="{Binding RelativeSource={RelativeSource AncestorType={x:Type FrameworkElement}}, Path=(TextElement.Foreground)}" />
            <Setter Property="HorizontalContentAlignment" Value="Center" />
            <Setter Property="Padding" Value="0" />
            <Setter Property="BorderThickness" Value="0" />
            <Setter Property="FocusVisualStyle" Value="{x:Null}" />
            <Setter Property="Focusable" Value="False" />
            <Setter Property="IsTabStop" Value="False" />
        </Style>
    </Grid.Resources>

    <!-- 使うとき2 -->
    <Button
        Grid.Column="0"
        Width="60"
        Command="{Binding ShowReservationSearchWindowCommand}"
        ToolTip="予約検索">
        <Button.Style>
            <Style BasedOn="{StaticResource TitleLabel}" TargetType="{x:Type Button}">
                <Setter Property="Template">
                    <Setter.Value>
                        <ControlTemplate TargetType="{x:Type Button}">
                            <Border Background="{TemplateBinding Background}">
                                <Image Source="{StaticResource White_Search_24}" Stretch="None" />
                            </Border>
                        </ControlTemplate>
                    </Setter.Value>
                </Setter>
            </Style>
        </Button.Style>
    </Button>
```
