# XAML_分岐処理まとめ

## XAMLにおけるif文

[さんさめ_【WPF】Binding入門5。DataTriggerの活用](https://threeshark3.com/wpf-binding-datatrigger/)  

DataTrigger（データトリガー）とは、Bindingした値に応じてプロパティを変化させる仕組みです。  
Styleでは、通常、「Setter」というオブジェクトを配置してプロパティの値を定義します。  
「Setter」に対し「Triggers」では、条件を記述し、その条件にマッチしたときのみ設定される値を定義できます。  

``` XML : 基本形
<Button>
    <Button.Style>
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
    </Button.Style>
</Button>
```

---

## デフォルトの状態を設定する上で注意すること

デフォルトの状態を設定したい場合、それはSetterタグで記述しないとDataTriggerで変化させることができない。  
デフォルトの状態をプロパティ属性構文で記述すると変化してくれないことを発見したので注意する。  

また、Styleで定義するので、BaseOnでスタイル元を継承すること。  
そうしないと、レイアウトが崩れてしまう。  

``` XML : これは問題ない
<Button>
    <Button.Style>
        <Style TargetType="{x:Type Button}">
            <!-- セッターでデフォルトの状態を定義する分には動く -->
            <Setter Property="Background" Value="Red"/> 
            <Style.Triggers>
                <DataTrigger Binding="{Binding Name}" Value="">
                    <Setter Property="Background" Value="Blue"/>
                </DataTrigger>
            </Style.Triggers>
        </Style>
    </Button.Style>
</Button>
```

``` XML : これはダメ
<!-- プロパティ属性構文でデフォルトの状態を定義すると変化しない -->
<Button Background="Red">
    <Button.Style>
        <Style TargetType="{x:Type Button}">
            <Style.Triggers>
                <DataTrigger Binding="{Binding Name}" Value="">
                    <Setter Property="Background" Value="Blue"/>
                </DataTrigger>
            </Style.Triggers>
        </Style>
    </Button.Style>
</Button>
```

---

## 別のコントロールの値を条件として利用する場合

ElementNameとBindingしたい値を指定することで実現可能  

``` XML
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

---

## True,Falseどちらの場合も定義する必要性

両方の状態を定義できるけど、そうする必要はない。  
デフォルトの状態を定義しておいて、残ったほうで状態を変化させるだけで十分。  

``` XML : TrueとFalse、両方定義する場合
<DockPanel.Style>
    <Style TargetType="{x:Type DockPanel}">
        <Style.Triggers>
            <DataTrigger Binding="{Binding Name, Mode=OneWay, Converter={StaticResource NullOrEmptyToBoolConverter}}" Value="true">
                <Setter Property="Background" Value="Transparent" />
            </DataTrigger>
            <DataTrigger Binding="{Binding Name, Mode=OneWay, Converter={StaticResource NullOrEmptyToBoolConverter}}" Value="false">
                <Setter Property="Background" Value="IndianRed" />
            </DataTrigger>
        </Style.Triggers>
    </Style>
</DockPanel.Style>
```

どちらかでも意味は同じ。  

``` XML
<DockPanel.Style>
    <Style TargetType="{x:Type DockPanel}">
        <Setter Property="Background" Value="Transparent" />
        <Style.Triggers>
            <DataTrigger Binding="{Binding Name, Mode=OneWay, Converter={StaticResource NullOrEmptyToBoolConverter}}" Value="false">
                <Setter Property="Background" Value="IndianRed" />
            </DataTrigger>
        </Style.Triggers>
    </Style>
</DockPanel.Style>

<DockPanel.Style>
    <Style TargetType="{x:Type DockPanel}">
        <Setter Property="Background" Value="IndianRed" />
        <Style.Triggers>
            <DataTrigger Binding="{Binding Name, Mode=OneWay, Converter={StaticResource NullOrEmptyToBoolConverter}}" Value="true">
                <Setter Property="Background" Value="Transparent" />
            </DataTrigger>
        </Style.Triggers>
    </Style>
</DockPanel.Style>

```

---

## And条件とOr条件の書き方

[MultiDataTrigger with OR instead of AND](https://stackoverflow.com/questions/38396419/multidatatrigger-with-or-instead-of-and)  
[StryleのMultiDataTrigger](http://gootara.org/library/2017/01/wpfao.html)

- DataTrigger  
- MultiDataTrigger  
- 各プロパティ + MultiBinding  

の組み合わせで実現可能。  

### Or条件

Or条件はDataTriggerで実現可能。  

2回も同じこと書くのが気にくわないけど仕方なし。  
セッターの部分をまとめることができないか調べてみたが、できなさそうだった。  

``` XML : or条件
<CheckBox>
    <CheckBox.Style>
        <Style TargetType="CheckBox">
            <!-- or条件に合わない場合の状態を記述する -->
            <Setter Property="IsEnabled" Value="False" />
            <Setter Property="IsTabStop" Value="False" />
            <!-- or条件はDataTriggerで当てはまってほしい条件を全て記述する -->
            <Style.Triggers>
                <DataTrigger Binding="{Binding Name}" Value="">
                    <Setter Property="IsEnabled" Value="False" />
                    <Setter Property="IsChecked" Value="False" />
                </DataTrigger>
                <DataTrigger Binding="{Binding Name}" Value="{x:Null}">
                    <Setter Property="IsEnabled" Value="False" />
                    <Setter Property="IsChecked" Value="False" />
                </DataTrigger>
            </Style.Triggers>
        </Style>
    </CheckBox.Style>
</CheckBox>
```

### And

And条件はMultiDataTriggerによって実現する。  

MultiDataTriggerでOr条件を実現させる場合Converterが必要。  
なのでOr条件を実現させるなら素直にDataTriggerを使うべし。  

``` XML : and条件
<CheckBox>
    <CheckBox.Style>
        <Style TargetType="CheckBox">
            <!-- And条件に合わない場合の状態も記述する -->
            <Setter Property="IsEnabled" Value="False" />
            <Setter Property="Content" Value="デフォルト" />
            <!-- And条件はMultiDataTriggerで定義する -->
            <Style.Triggers>
                <MultiDataTrigger>
                    <MultiDataTrigger.Conditions>
                        <Condition Binding="{Binding Flag1}" Value="True" />
                        <Condition Binding="{Binding Flag2}" Value="True" />
                    </MultiDataTrigger.Conditions>
                    <MultiDataTrigger.Setters>
                        <Setter Property="IsEnabled" Value="True" />
                        <Setter Property="Content" Value="第一" />
                    </MultiDataTrigger.Setters>
                </MultiDataTrigger>
            </Style.Triggers>
        </Style>
    </CheckBox.Style>
</CheckBox>
```

MultiDataTriggerは複数定義可能なので、And条件のOr条件みたいなこともできる。  

``` XML : and条件 複数のMultiDataTrigger
<CheckBox>
    <CheckBox.Style>
        <Style TargetType="CheckBox">
            <!-- デフォルトの状態 -->
            <Setter Property="IsEnabled" Value="False" />
            <Setter Property="Content" Value="デフォルト" />
            <!-- MultiDataTriggerは複数定義することができる -->
            <Style.Triggers>
                <!-- 第一の状態 -->
                <MultiDataTrigger>
                    <MultiDataTrigger.Conditions>
                        <Condition Binding="{Binding Flag1}" Value="True" />
                        <Condition Binding="{Binding Flag2}" Value="True" />
                    </MultiDataTrigger.Conditions>
                    <MultiDataTrigger.Setters>
                        <Setter Property="IsEnabled" Value="True" />
                        <Setter Property="Content" Value="第一" />
                    </MultiDataTrigger.Setters>
                </MultiDataTrigger>
                <!-- 第二の状態 -->
                <MultiDataTrigger>
                    <MultiDataTrigger.Conditions>
                        <Condition Binding="{Binding Flag1}" Value="True" />
                        <Condition Binding="{Binding Flag2}" Value="False" />
                    </MultiDataTrigger.Conditions>
                    <MultiDataTrigger.Setters>
                        <Setter Property="IsEnabled" Value="False" />
                        <Setter Property="Content" Value="第二" />
                    </MultiDataTrigger.Setters>
                </MultiDataTrigger>
            </Style.Triggers>
        </Style>
    </CheckBox.Style>
</CheckBox>
```

どっちがいいかという議論  
[MultiDataTrigger vs DataTrigger with multibinding](https://stackoverflow.com/questions/20993293/multidatatrigger-vs-datatrigger-with-multibinding)  

---

## MultiBindingによるAnd,Or条件の実現

プロパティの中でMultiBindingを使うことでもAnd,Orを実現できる。  
しかし、その場合、MultiConverterを使わないといけない。  

Styleに定義しないので、見た目を崩す心配がないことくらいが利点だろうか。  
MultiConverterを使わないといけない地点で手軽さが失われる。  

``` XML
<CheckBox>
    <CheckBox.IsEnabled>
        <MultiBinding Converter="{StaticResource BooleanAndMultiConverter}">
            <Binding Path="Flag1" />
            <Binding Path="Flag2" />
        </MultiBinding>
    </CheckBox.IsEnabled>
</CheckBox>
```

---

## DataTriggerとMultiDataTriggerの共存

DataTriggerとMultiDataTriggerは共存可能であるという例。  

右クリックした時、その場所が灰色なら、右クリックメニューの1つだけをEnable Falseにするときに色々苦労したのでまとめ。  
どちらかというと、Triggerの制御より灰色にしているプロパティがどのクラスのものなのかさっぱりわからなかったのが問題ではあったが、通常はTrueで、ある条件の時のみFalseにする構文を発見できたのでそれをまとめる。  

``` XML
    <Style>
        <Style.Triggers>
            <!-- デフォルトはTrueとして設定する -->
            <DataTrigger Binding="{Binding UniqueNo, Mode=OneWay}" Value="4">
                <Setter Property="CommandParameter" Value="{Binding Source={StaticResource ContextProxy}, Path=Data.RightClickedContent, Mode=OneWay}" />
                <Setter Property="IsEnabled" Value="True" />
            </DataTrigger>
            <MultiDataTrigger>
                <!--  操作不能な枠だったら無効にする  -->
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
