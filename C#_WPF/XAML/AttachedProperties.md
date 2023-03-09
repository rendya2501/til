# 添付プロパティ

## 概要

例えば、テキストボックスとボタンがあります。  
カーソルが当たっている間は色を変えたいけれど、どちらにもその機能はない。  
それぞれを継承して拡張した、CustomButton,CustomTextを作ってもいいけど、どちらにも同じコードを書くことになるし作ること自体に手間がかかる。  
そういう時に添付プロパティなるものを作って、それをテキストボックスとボタンに実装してあげることで、その機能を実現することができる。  
それが、添付プロパティ。  

どうやって作るかは後でまとめる。  

---

## 添付プロパティをBindingのPathに指定する場合はカッコを付ける

BindingのPathに添付プロパティを指定する場合、カッコをつけないと「BindingExpression path error」になる。

``` XML
<Window x:Class="Sample.MainView"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:local="clr-namespace:Sample"
    Title="MainView" Height="300" Width="300">
    <Grid>
        <TextBox>
            <TextBox.Style>
                <Style TargerType="TextBox">
                    <Style.Triggers>
                        <!--Pathにカッコを付ける-->
                        <DataTrigger Binding="{Binding Path=(local:AttachedXXX.XXX), RelativeSource={RelativeSource Self}}" Value="True">
                            <Setter Property="Background" Value="Blue"/>
                        </DataTrigger>
                    </Style.Triggers>
                </Style>
            </TextBox.Style>
        </TextBox>
    </Grid>
</Window>
```

[添付プロパティをBindingのPathに指定する場合はカッコを付ける](https://qiita.com/flasksrw/items/7212453de6e7d8f221a1)
