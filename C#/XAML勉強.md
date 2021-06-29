# XAML勉強

## DataTrigger

<https://threeshark3.com/wpf-binding-datatrigger/>

Styleでは、通常、「Setter」というオブジェクトを配置してプロパティの値を定義します。  
「Setter」に対し「Triggers」では、条件を記述し、その条件にマッチしたときのみ設定される値を定義できます。  
DataTrigger（データトリガー）とは、Bindingした値に応じてプロパティを変化させる仕組みです。  

``` XML
<CheckBox
    x:Name="SetProductCheckBox"
    VerticalAlignment="Center"
    VerticalContentAlignment="Center"
    IsChecked="{Binding Data.Product.SetProductFlag, Mode=TwoWay}">
    <!-- どうでもいいがチェックボックスの大きさを変えたかったらLayoutTransformするしかないみたい -->
    <CheckBox.LayoutTransform>
        <ScaleTransform ScaleX="1.2" ScaleY="1.2"/>
    </CheckBox.LayoutTransform>
    <CheckBox.Style>
    <!-- TargetTypeを指定しないとなんのプロパティがあるかわからないので指定する -->
        <Style TargetType="CheckBox">
            <Style.Triggers>
            <!-- DataTriggerは他の値をBindして条件として使えるのが強み -->
                <DataTrigger Binding="{Binding Value, ElementName=DepartmentCDBox}" Value="0">
                    <Setter Property="IsEnabled" Value="False" />
                    <Setter Property="IsChecked" Value="False" />
                </DataTrigger>
            </Style.Triggers>
        </Style>
    </CheckBox.Style>
</CheckBox>
```

## 予約枠のいつぞやのやつ

``` XML
<ControlTemplate.Triggers>
    <!--<DataTrigger Binding="{Binding Text, ElementName=DayOfWeek}" Value="土">
        <Setter Property="Foreground" Value="blue" />
    </DataTrigger>
    <DataTrigger Binding="{Binding Text, ElementName=DayOfWeek}" Value="日">
        <Setter Property="Foreground" Value="red" />
    </DataTrigger>-->
    <DataTrigger Binding="{Binding DayOfWeek}" Value="Saturday">
        <Setter TargetName="DayOfWeek" Property="Foreground" Value="blue" />
    </DataTrigger>
    <DataTrigger Binding="{Binding DayOfWeek}" Value="Sunday">
        <Setter TargetName="DayOfWeek" Property="Foreground" Value="red" />
    </DataTrigger>
</ControlTemplate.Triggers>

<ControlTemplate.Triggers>
    <!--<DataTrigger Binding="{Binding Empty}" Value="True">
        <Setter Property="Visibility" Value="Hidden" />
    </DataTrigger>
    <DataTrigger Binding="{Binding Text, ElementName=Day}" Value="土">
        <Setter Property="Foreground" Value="blue" />
        <Setter TargetName="Border" Property="BorderThickness" Value="1,0,1,1" />
    </DataTrigger>
    <DataTrigger Binding="{Binding Text, ElementName=Day}" Value="日">
        <Setter Property="Foreground" Value="red" />
    </DataTrigger>
    <DataTrigger Binding="{Binding IsSelected}" Value="True">
        <Setter TargetName="Base" Property="Background" Value="{StaticResource MahApps.Brushes.Accent4}" />
    </DataTrigger>-->
    <DataTrigger Binding="{Binding Empty}" Value="True">
        <Setter TargetName="Count" Property="Visibility" Value="Hidden" />
    </DataTrigger>
    <DataTrigger Binding="{Binding DayOfWeek}" Value="Saturday">
        <Setter TargetName="Day" Property="Foreground" Value="blue" />
        <Setter TargetName="Border" Property="BorderThickness" Value="1,0,1,1" />
    </DataTrigger>
    <DataTrigger Binding="{Binding DayOfWeek}" Value="Sunday">
        <Setter TargetName="Day" Property="Foreground" Value="red" />
    </DataTrigger>
    <DataTrigger Binding="{Binding IsSelected}" Value="True">
        <Setter TargetName="Base" Property="Background" Value="{StaticResource MahApps.Brushes.Accent4}" />
    </DataTrigger>
</ControlTemplate.Triggers>
```
