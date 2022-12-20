# ToolTip

## ToolTipとは

>ボタンなどのコントロールにマウスを載せた時、説明やヒントをポップアップ表示してくれる機能です。  
[ボタンのツールチップ](https://araramistudio.jimdo.com/2019/11/01/c-%E3%81%AEwpf%E3%81%A7%E3%83%84%E3%83%BC%E3%83%AB%E3%83%81%E3%83%83%E3%83%97%E3%82%92%E8%A1%A8%E7%A4%BA%E3%81%99%E3%82%8B/)  

「マウスオーバー」「ポップアップ」等が真っ先に思い浮かんだが、WPFでは「ツールチップ」な模様。  

---

## IsEnableがFalseなコントロールでもツールチップを表示させる

>ToolTipService.ShowOnDisabledをTrueにする  
[【WPF】無効なコントロールにツールチップを表示させる方法](https://threeshark3.com/show-on-disabled/)  

---

## 実装

``` XML
<!-- キーを定義 -->
<DockPanel.Resources>
    <Style x:Key="ToolTipTextBlock" TargetType="TextBlock">
        <Setter Property="LayoutTransform">
            <Setter.Value>
                <ScaleTransform 
                ScaleX="{Binding RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type metro:MetroWindow}}, Path=DataContext.Magnification, Mode=TwoWay}" 
                ScaleY="{Binding RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type metro:MetroWindow}}, Path=DataContext.Magnification, Mode=TwoWay}" />
            </Setter.Value>
        </Setter>
    </Style>
</DockPanel.Resources>

<!-- 定義したキーを使う場合 -->
<c1:Column.CellTemplate>
    <DataTemplate>
        <Grid ToolTipService.IsEnabled="{Binding Player1Remarks, Mode=OneWay, Converter={StaticResource NotNullOrEmptyToBoolConverter}}">
            <Grid.ColumnDefinitions>
                <ColumnDefinition Width="*" />
                <ColumnDefinition Width="Auto" />
            </Grid.ColumnDefinitions>
            <ToolTipService.ToolTip>
                <TextBlock Style="{DynamicResource ToolTipTextBlock}" Text="{Binding Player1Remarks, Mode=OneWay}" />
            </ToolTipService.ToolTip>
            <TextBlock
                Grid.Column="0"
                Focusable="False"
                Text="{Binding Player1Name, Mode=OneWay}"
                TextWrapping="Wrap" />
            <TextBlock
                Grid.Column="1"
                Margin="0,0,5,0"
                Focusable="False"
                Text="※"
                Visibility="{Binding Player1Remarks, Mode=OneWay, Converter={StaticResource StringToVisibilityConverter}}" />
        </Grid>
    </DataTemplate>
</c1:Column.CellTemplate>
```

---

## 試行錯誤のあと

``` xml
<!-- 試行錯誤のあと -->
                            <Setter Property="LayoutTransform" TargetName="grid">
                                <Setter.Value>
                                    <TransformGroup>
                                        <ScaleTransform ScaleX="1" ScaleY="1" />
                                        <SkewTransform AngleX="0" AngleY="0" />
                                        <RotateTransform Angle="90" />
                                        <TranslateTransform X="0" Y="0" />
                                    </TransformGroup>
                                </Setter.Value>
                            </Setter>

<!--<Style.Resources>
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
</Style.Resources>-->
```

---

[C#のWPFでツールチップを表示する](https://araramistudio.jimdo.com/2019/11/01/c-%E3%81%AEwpf%E3%81%A7%E3%83%84%E3%83%BC%E3%83%AB%E3%83%81%E3%83%83%E3%83%97%E3%82%92%E8%A1%A8%E7%A4%BA%E3%81%99%E3%82%8B/)  
[wpf : ToolTipの幅](http://pieceofnostalgy.blogspot.com/2013/05/wpf-tooltip.html)
[ToolTip Style ToolTipService.ShowDuration](https://stackoverflow.com/questions/32288529/tooltip-style-tooltipservice-showduration)
[キーを指定したスタイルの使い方参考](https://qiita.com/tera1707/items/cb8ad4c40107ae25b565)
