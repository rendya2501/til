# XAML_分岐処理まとめ

## XAMLにおけるif文

[さんさめ_【WPF】Binding入門5。DataTriggerの活用]<https://threeshark3.com/wpf-binding-datatrigger/>  

DataTrigger（データトリガー）とは、Bindingした値に応じてプロパティを変化させる仕組みです。  
Styleでは、通常、「Setter」というオブジェクトを配置してプロパティの値を定義します。  
「Setter」に対し「Triggers」では、条件を記述し、その条件にマッチしたときのみ設定される値を定義できます。  

``` XML : 基本形
<Button>
    <!-- TargetTypeを指定しないと設定できるプロパティに何があるかわからないので指定する -->
    <Style TargetType="{x:Type Button}">
        <!-- 通常は背景色、赤 -->
        <Setter Property="Background" Value="Red"/> 
        <Style.Triggers>
            <!-- Nameプロパティが空文字かnullなら背景色を青に -->
            <DataTrigger Binding="{Binding Name}" Value="">
                <Setter Property="Background" Value="Blue"/>
            </DataTrigger>
            <DataTrigger Binding="{Binding Name}" Value="{x:Null}">
                <Setter Property="Background" Value="Blue"/>
            </DataTrigger>
        </Style.Triggers>
    </Style>
</Button>
```

``` XML : 別のコントロールの値を利用する場合
<CheckBox>
    <CheckBox.Style>
        <Style TargetType="{x:Type CheckBox}">
            <Style.Triggers>
                <DataTrigger Binding="{Binding Value, ElementName=TestTextBox}" Value="0">
                    <Setter Property="IsEnabled" Value="False" />
                    <Setter Property="IsChecked" Value="False" />
                </DataTrigger>
            </Style.Triggers>
        </Style>
    </CheckBox.Style>
</CheckBox>
```

``` XML : TrueとFalse、両方定義する場合
    <DockPanel.Style>
        <Style TargetType="{x:Type DockPanel}">
            <Style.Triggers>
                <DataTrigger Binding="{Binding ReservationPlayerRemarks, Mode=OneWay, Converter={StaticResource NullOrEmptyToBoolConverter}}" Value="true">
                    <Setter Property="Background" Value="Transparent" />
                </DataTrigger>
                <DataTrigger Binding="{Binding ReservationPlayerRemarks, Mode=OneWay, Converter={StaticResource NullOrEmptyToBoolConverter}}" Value="false">
                    <Setter Property="Background" Value="IndianRed" />
                </DataTrigger>
            </Style.Triggers>
        </Style>
    </DockPanel.Style>
```

``` xml : 文言を変えるサンプル
<Button Command="{Binding ShowHelpPageCommand, Mode=OneWay}">
    <Button.Content>
        <Image Source="{StaticResource White_Help_16}" Stretch="None" />
    </Button.Content>
    <Button.Style>
        <Style BasedOn="{StaticResource WindowButton}" TargetType="Button">
            <Style.Triggers>
                <DataTrigger Binding="{Binding HelpPageURL, Converter={StaticResource NotNullOrEmptyToBoolConverter}, Mode=OneWay}" Value="true">
                    <Setter Property="ToolTip" Value="ヘルプページを表示します。" />
                </DataTrigger>
                <DataTrigger Binding="{Binding HelpPageURL, Converter={StaticResource NotNullOrEmptyToBoolConverter}, Mode=OneWay}" Value="false">
                    <Setter Property="ToolTip" Value="ヘルプページの設定がありません。" />
                </DataTrigger>
            </Style.Triggers>
        </Style>
    </Button.Style>
</Button>
```

ボタンなどのコントロールにマウスを載せた時、説明やヒントをポップアップ表示してくれる機能です。  
[ボタンのツールチップ](https://araramistudio.jimdo.com/2019/11/01/c-%E3%81%AEwpf%E3%81%A7%E3%83%84%E3%83%BC%E3%83%AB%E3%83%81%E3%83%83%E3%83%97%E3%82%92%E8%A1%A8%E7%A4%BA%E3%81%99%E3%82%8B/)  

---

## and条件とor条件の書き方

<https://stackoverflow.com/questions/38396419/multidatatrigger-with-or-instead-of-and>  
<http://gootara.org/library/2017/01/wpfao.html>  

StyleのDataTriggerでor条件を指定して状態を変化させる方法。  
MultiDataTriggerなるものを使えば、複数条件は指定できるが、ORの場合はConverterをかまさないといけないらしい。  
そこで見つけたのがこの方法。  
2回も同じこと書くのが気にくわないけど、確かにORになっている。  
セッターの部分をまとめることができないか調べてみたが、Setterだけをまとめることはできなさそうだ。  

``` XML : チェックボックスで頑張ったやつ
<CheckBox
    x:Name="SetProductCheckBox"
    VerticalAlignment="Center"
    VerticalContentAlignment="Center"
    IsChecked="{Binding Data.Product.SetProductFlag, Mode=TwoWay}">
    <CheckBox.Style>
        <Style TargetType="CheckBox">
            <!--  or条件  -->
            <Style.Triggers>
                <DataTrigger Binding="{Binding Value, ElementName=DepartmentCDBox}" Value="0">
                    <Setter Property="IsEnabled" Value="False" />
                    <Setter Property="IsChecked" Value="False" />
                </DataTrigger>
                <DataTrigger Binding="{Binding Value, ElementName=DepartmentCDBox}" Value="{x:Null}">
                    <Setter Property="IsEnabled" Value="False" />
                    <Setter Property="IsChecked" Value="False" />
                </DataTrigger>
            </Style.Triggers>

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

どっちがいいという議論もちゃんとある。  
<https://stackoverflow.com/questions/20993293/multidatatrigger-vs-datatrigger-with-multibinding>  
<https://base64.work/so/wpf/171252>  

``` XML :  Style + DataTrigger or MultiDataTrigger
<!-- DataTriggerによる制御。BaseOnするものがあれば見た目を崩さずに済むと思う。 -->
<Binding
    Converter="{StaticResource NotNullOrEmptyToBoolConverter}"
    Mode="OneWay"
    Path="Data.Product.ProductCD"
    RelativeSource="{RelativeSource FindAncestor,AncestorType={x:Type metro:MetroWindow}}" />
    <c1:C1FlexGrid.Style>
        <Style TargetType="c1:C1FlexGrid">
            <!-- or条件 -->
            <Style.Triggers>
                <!-- 部門コードが入力されていない、または、セット商品にチェックが付いている-->
                <DataTrigger Binding="{Binding IsChecked, ElementName=SetProductCheckBox}" Value="True">
                    <Setter Property="IsEnabled" Value="False" />
                    <Setter Property="IsTabStop" Value="False" />
                </DataTrigger>
                <DataTrigger Binding="{Binding Value, ElementName=DepaetmentCDBox, Converter={StaticResource NotNullOrEmptyToBoolConverter}}" Value="True">
                    <Setter Property="IsEnabled" Value="False" />
                    <Setter Property="IsTabStop" Value="False" />
                </DataTrigger>
            </Style.Triggers>

            <!-- and条件 -->
            <Style.Triggers>
                <MultiDataTrigger>
                    <MultiDataTrigger.Conditions>
                        <!-- 部門コードが入力されていて、セット商品にチェックが付いていなかったら -->
                        <Condition Binding="{Binding IsChecked, ElementName=SetProductCheckBox}" Value="False" />
                        <Condition Binding="{Binding Value, ElementName=DepartmentCDBox, Converter={StaticResource NotNullOrEmptyToBoolConverter}}" Value="True" />
                    </MultiDataTrigger.Conditions>
                    <MultiDataTrigger.Setters>
                        <Setter Property="IsEnabled" Value="True" />
                        <Setter Property="IsTabStop" Value="True" />
                    </MultiDataTrigger.Setters>
                </MultiDataTrigger>
            </Style.Triggers>
            <!-- And条件に合わない場合の状態も記述する -->
            <Setter Property="IsEnabled" Value="False" />
            <Setter Property="IsTabStop" Value="False" />
        </Style>
    </c1:C1FlexGrid.Style>
```

``` XML : MultiBinding + MultiConverter
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

## ある条件の時のみFalseにする方法

[MultiDataTrigger with OR instead of AND](https://stackoverflow.com/questions/38396419/multidatatrigger-with-or-instead-of-and)  

チェックインから予約入力を開く修正の話。  
右クリックした時、その場所が灰色なら、右クリックメニューの1つだけをEnable Falseにするときに色々苦労したのでまとめ。  
どちらかというと、Triggerの制御より灰色にしているプロパティがどのクラスのものなのかさっぱりわからなかったのが問題ではあったが、通常はTrueで、ある条件の時のみFalseにする構文を発見できたのでそれをまとめる。  

DataTriggerとMultiDataTriggerは共存可能な模様。  

``` XML
    <Style>
        <Style.Triggers>
            <!-- デフォルトはTrueとして設定する -->
            <DataTrigger Binding="{Binding UniqueNo, Mode=OneWay}" Value="4">
                <Setter Property="CommandParameter" Value="{Binding Source={StaticResource ContextProxy}, Path=Data.RightClickedContent, Mode=OneWay}" />
                <Setter Property="IsEnabled" Value="True" />
            </DataTrigger>
            <MultiDataTrigger>
                <!--  操作不能な枠だったら予約入力は無効にする  -->
                <MultiDataTrigger.Conditions>
                    <!-- 4番目の項目 かつ オペレーション不可の場所だった場合 -->
                    <Condition Binding="{Binding UniqueNo, Mode=OneWay}" Value="4" />
                    <Condition Binding="{Binding Source={StaticResource ContextProxy}, Path=Data.RightClickedContent.IsOperatable, Mode=OneWay}" Value="False" />
                </MultiDataTrigger.Conditions>
                <MultiDataTrigger.Setters>
                    <!-- EnableをFalseにする。 -->
                    <Setter Property="IsEnabled" Value="False" />
                </MultiDataTrigger.Setters>
            </MultiDataTrigger>
        </Style.Triggers>
    </Style>
```
