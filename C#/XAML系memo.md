# XAML勉強

## TextBoxで未入力の場合にBindingしてるソースのプロパティにnullを入れたい

<https://blog.okazuki.jp/entry/20110411/1302529830>  

なんてことはない知識だが、一応MDの練習やTILのために追加。  
テキストボックスの空文字をNULLにしたい場合はどうすればいいのか分からなかったから調べたらドンピシャのがあったので、メモ。  

TargetNullValueプロパティを以下のように書くことで、空文字のときにnullがプロパティに渡ってくるようになります。

``` XML
<TextBox Text="{Binding Path=NumberInput, UpdateSourceTrigger=PropertyChanged, TargetNullValue=''}" />
```

TargetNullValueはこの値が来たらnullとして扱うことを設定するためのプロパティの模様。  

---

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

---

## xaml datatrigger or

<https://stackoverflow.com/questions/38396419/multidatatrigger-with-or-instead-of-and>  
<http://gootara.org/library/2017/01/wpfao.html>  

StyleのDataTriggerでor条件のやり方。  
MultiDataTriggerなるものを使えば、複数条件は指定できるが、ORの場合はConverterをかまさないといけないので面倒。  
そこで見つけたのがこの手法。  
2回も同じこと書くのが気にくわないけど、確かにORになっている。  
セッターの部分をまとめることができないか調べてみたが、Setterだけをまとめることはできなさそうだ。  

``` XML
<Style.Triggers>
    <DataTrigger Binding="{Binding CCTVPath}" Value="">
        <Setter Property="Visibility" Value="Hidden"/>
    </DataTrigger>
    <DataTrigger Binding="{Binding PermissionsFlag}" Value="False">
        <Setter Property="Visibility" Value="Hidden"/>
    </DataTrigger>
</Style.Triggers>
    
<c1:C1FlexGrid.Style>
    <Style TargetType="c1:C1FlexGrid">
        <Style.Triggers>
            <DataTrigger Binding="{Binding IsChecked, ElementName=SetProductCheckBox}">
                <Setter Property="IsEnabled" Value="False" />
                <Setter Property="IsTabStop" Value="False" />
            </DataTrigger>
            <DataTrigger Binding="{Binding Value, ElementName=DepaetmentCDBox}" Value="{x:Null}">
                <Setter Property="IsEnabled" Value="False" />
                <Setter Property="IsTabStop" Value="False" />
            </DataTrigger>
        </Style.Triggers>
    </Style>
</c1:C1FlexGrid.Style>
```

---

## MultiBindingとMultiDataTrigger

XAML上でのAND条件OR条件を指定したコントロールの制御にも色々あって、  
Styleの中に各MultiDataTriggerなるものと、MultiBindingを使った方法があったのでまとめ。  

MultiDataTriggerはSytleの中に書くのだが、そうすると見た目がおかしくなってしまうので(多分BaseOnとか必要)、  
見た目を崩したくなかったらMultiBindingとConverterを使ってBindさせたほうがよい。  
ちょっとまだまとめきれていないので、後でまとめる。  

``` XML
<CheckBox.IsChecked>
    <Binding
        ConverterParameter="Consignment"
        Mode="TwoWay"
        Path="Data.Product.ConsignmentPurchaseType">
        <Binding.Converter>
            <conv:EnumToBoolConverter FalseValue="Purchase" />
        </Binding.Converter>
    </Binding>
</CheckBox.IsChecked>

<Button
    Command="{Binding RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type metro:MetroWindow}}, Path=DataContext.ShowPlayerCustomerCommand, Mode=OneWay}"
    CommandParameter="{Binding Mode=OneWay}"
    IsTabStop="False"
    Style="{StaticResource SearchButton}" />
    <MultiBinding Converter="{StaticResource BooleanAndMultiConverter}">
        <Binding ElementName="SetProductCheckBox" Path="IsChecked" />
        <Binding
            Converter="{StaticResource NotNullOrEmptyToBoolConverter}"
            ElementName="DepartmentCDBox"
            Path="Value" />
    </MultiBinding>

<Binding
    Converter="{StaticResource NotNullOrEmptyToBoolConverter}"
    Mode="OneWay"
    Path="Data.Product.ProductCD"
    RelativeSource="{RelativeSource FindAncestor,
    AncestorType={x:Type metro:MetroWindow}}" />
    <!--
        IsEnabled="{Binding IsChecked, Converter={StaticResource NotConverter}, ElementName=SetProductCheckBox}"
        IsTabStop="{Binding IsChecked, Converter={StaticResource NotConverter}, ElementName=SetProductCheckBox}"
    -->
    c1:C1FlexGrid.Style>
        <Style TargetType="c1:C1FlexGrid">
            <Style.Triggers>
                <DataTrigger Binding="{Binding IsChecked, Converter={StaticResource NotConverter}, ElementName=SetProductCheckBox}">
                    <Setter Property="IsEnabled" Value="False" />
                    <Setter Property="IsTabStop" Value="False" />
                </DataTrigger>
                <DataTrigger Binding="{Binding Value, ElementName=DepaetmentCDBox}" Value="{x:Null}">
                    <Setter Property="IsEnabled" Value="False" />
                    <Setter Property="IsTabStop" Value="False" />
                </DataTrigger>
                <MultiDataTrigger>
                    <MultiDataTrigger.Conditions>
                        <Condition Binding="{Binding IsChecked, Converter={StaticResource NotConverter}, ElementName=SetProductCheckBox}" Value="True" />
                        <Condition Binding="{Binding Data.Product.ProductCD, Mode=OneWay, Converter={StaticResource NotNullOrEmptyToBoolConverter}}" Value="True" />
                    </MultiDataTrigger.Conditions>
                    <Setter Property="IsEnabled" Value="True" />
                    <Setter Property="IsTabStop" Value="True" />
                </MultiDataTrigger>
            </Style.Triggers>
        </Style>
    </c1:C1FlexGrid.Style>
```

---

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
