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

## xaml datatrigger or条件 and条件

<https://stackoverflow.com/questions/38396419/multidatatrigger-with-or-instead-of-and>  
<http://gootara.org/library/2017/01/wpfao.html>  

StyleのDataTriggerでor条件を指定して状態を変化させる方法。  
MultiDataTriggerなるものを使えば、複数条件は指定できるが、ORの場合はConverterをかまさないといけないらしい。  
そこで見つけたのがこの方法。  
2回も同じこと書くのが気にくわないけど、確かにORになっている。  
セッターの部分をまとめることができないか調べてみたが、Setterだけをまとめることはできなさそうだ。  

``` XML
<!-- 商品台帳のチェックボックスで頑張ったやつ -->
<CheckBox
    x:Name="SetProductCheckBox"
    VerticalAlignment="Center"
    VerticalContentAlignment="Center"
    IsChecked="{Binding Data.Product.SetProductFlag, Mode=TwoWay}">
    <CheckBox.Style>
        <Style TargetType="CheckBox">
            <Style.Triggers>
                <!--  or条件  -->
                <DataTrigger Binding="{Binding Value, ElementName=DepartmentCDBox}" Value="0">
                    <Setter Property="IsEnabled" Value="False" />
                    <Setter Property="IsChecked" Value="False" />
                </DataTrigger>
                <DataTrigger Binding="{Binding Value, ElementName=DepartmentCDBox}" Value="{x:Null}">
                    <Setter Property="IsEnabled" Value="False" />
                    <Setter Property="IsChecked" Value="False" />
                </DataTrigger>

                <!-- and条件 -->
                <Style.Triggers>
                    <MultiDataTrigger>
                        <MultiDataTrigger.Conditions>
                            <!-- 部門コードが入力されていて、セット商品にチェックが付いていなかったら -->
                            <Condition Binding="{Binding IsChecked, ElementName=SetProductCheckBox}" Value="False" />
                            <Condition Binding="{Binding Value, ElementName=DepartmentCDBox, Converter={StaticResource NotNullOrEmptyToBoolConverter}}" Value="True" />
                        </MultiDataTrigger.Conditions>
                        <Setter Property="IsEnabled" Value="True" />
                        <Setter Property="IsTabStop" Value="True" />
                    </MultiDataTrigger>
                </Style.Triggers>
                <!-- And条件に合わない場合の状態も記述する -->
                <Setter Property="IsEnabled" Value="False" />
                <Setter Property="IsTabStop" Value="False" />
            </Style.Triggers>
        </Style>
    </CheckBox.Style>
</CheckBox>
```

---

## MultiBindingとDataTriggerによるAnd,Or条件の指定の仕方

[StryleのMultiDataTrigger](http://gootara.org/library/2017/01/wpfao.html)

XAML上でAND条件やOR条件を指定したコントロールの制御にも色々あって、  
StyleのMultiDataTriggerなるものと、MultiBindingを使った方法があったのでまとめ。  

MultiDataTriggerはSytleの中に書くのだが、そうすると見た目がおかしくなってしまうので(多分BaseOnとか必要)、  
見た目を崩したくなかったらMultiBindingとConverterを使ってBindさせたほうがよい。  
似ているようで微妙に違って、頭が混乱したのでまとめ。  

``` XML
<!-- Style + DataTrigger or MultiDataTrigger -->
<!-- DataTriggerによる制御。BaseOnするものがあれば見た目を崩さずに済むと思う。 -->
<Binding
    Converter="{StaticResource NotNullOrEmptyToBoolConverter}"
    Mode="OneWay"
    Path="Data.Product.ProductCD"
    RelativeSource="{RelativeSource FindAncestor,AncestorType={x:Type metro:MetroWindow}}" />
    <c1:C1FlexGrid.Style>
        <Style TargetType="c1:C1FlexGrid">
            <Style.Triggers>
                <!-- or条件 -->
                <!-- 部門コードが入力されていない、または、セット商品にチェックが付いている-->
                <DataTrigger Binding="{Binding IsChecked, ElementName=SetProductCheckBox}" Value="True">
                    <Setter Property="IsEnabled" Value="False" />
                    <Setter Property="IsTabStop" Value="False" />
                </DataTrigger>
                <DataTrigger Binding="{Binding Value, ElementName=DepaetmentCDBox, Converter={StaticResource NotNullOrEmptyToBoolConverter}}" Value="True">
                    <Setter Property="IsEnabled" Value="False" />
                    <Setter Property="IsTabStop" Value="False" />
                </DataTrigger>

                <!-- and条件 -->
                <MultiDataTrigger>
                    <MultiDataTrigger.Conditions>
                        <!-- 部門コードが入力されていて、セット商品にチェックが付いていなかったら -->
                        <Condition Binding="{Binding IsChecked, ElementName=SetProductCheckBox}" Value="False" />
                        <Condition Binding="{Binding Value, ElementName=DepartmentCDBox, Converter={StaticResource NotNullOrEmptyToBoolConverter}}" Value="True" />
                    </MultiDataTrigger.Conditions>
                    <Setter Property="IsEnabled" Value="True" />
                    <Setter Property="IsTabStop" Value="True" />
                </MultiDataTrigger>
                <!-- And条件に合わない場合の状態も記述する -->
                <Setter Property="IsEnabled" Value="False" />
                <Setter Property="IsTabStop" Value="False" />
            </Style.Triggers>
        </Style>
    </c1:C1FlexGrid.Style>

<!-- MultiBinding + MultiConverter -->
<!-- Styleに書かないので、見た目を崩さない。AND,ORの指定はMultiConverterを定義することで実現する -->
<ctrl:CustomFlexGrid x:Name="UnitPriceFlexGrid">
    <!-- 1つ1つのプロパティに対してMultiBindingで条件を指定する必要がある -->
    <!-- AND,ORのMultiConverterは必要。既に定義されていたので使わせてもらった -->
    <ctrl:CustomFlexGrid.IsEnabled>
        <MultiBinding Converter="{StaticResource BooleanAndMultiConverter}">
            <Binding
                Converter="{StaticResource NotConverter}"
                ElementName="SetProductCheckBox"
                Path="IsChecked" />
            <Binding
                Converter="{StaticResource NotNullOrEmptyToBoolConverter}"
                ElementName="DepartmentCDBox"
                Path="Value" />
        </MultiBinding>
    </ctrl:CustomFlexGrid.IsEnabled>
    <ctrl:CustomFlexGrid.IsTabStop>
        <MultiBinding Converter="{StaticResource BooleanAndMultiConverter}">
            <Binding
                Converter="{StaticResource NotConverter}"
                ElementName="SetProductCheckBox"
                Path="IsChecked" />
            <Binding
                Converter="{StaticResource NotNullOrEmptyToBoolConverter}"
                ElementName="DepartmentCDBox"
                Path="Value" />
        </MultiBinding>
    </ctrl:CustomFlexGrid.IsTabStop>
</ctrl:CustomFlexGrid>
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
