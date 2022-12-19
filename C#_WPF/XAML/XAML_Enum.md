# XAML_Enum

## DataTrigger + Enum

DataTriggerでEnum使うときって、ValueにそのままEnumの文字列を入れても認識されるらしい。  
厳密に指定するなら`x:Static`と名前空間からEnumを参照することでアクセス可能。  

``` C#
public enum State
{
    Normal,
    Warning,
    Error
}
```

``` xml : 文字列で直接指定
    <Button Command="{Binding ButtonCommand}">
        <Button.Style>
            <Style TargetType="{x:Type Button}">
                <Style.Triggers>
                    <DataTrigger Binding="{Binding State}" Value="Normal">
                        <Setter Property="Background" Value="Blue" />
                    </DataTrigger>
                    <DataTrigger Binding="{Binding State}" Value="Warning">
                        <Setter Property="Background" Value="Yellow" />
                    </DataTrigger>
                    <DataTrigger Binding="{Binding State}" Value="Error">
                        <Setter Property="Background" Value="Red" />
                    </DataTrigger>
                </Style.Triggers>
            </Style>
        </Button.Style>
    </Button>
```

``` xml : x:Staticによる指定
<window xmlns:localenum="clr-namespace:Wpf_DataTrigger_Enum.Enum">

    <Button Command="{Binding ButtonCommand}">
        <Button.Style>
            <Style TargetType="{x:Type Button}">
                <Style.Triggers>
                    <DataTrigger Binding="{Binding State}" Value="{x:Static localenum:State.Normal}">
                        <Setter Property="Background" Value="Blue" />
                    </DataTrigger>
                    <DataTrigger Binding="{Binding State}" Value="{x:Static localenum:State.Warning}">
                        <Setter Property="Background" Value="Yellow" />
                    </DataTrigger>
                    <DataTrigger Binding="{Binding State}" Value="{x:Static localenum:State.Error}">
                        <Setter Property="Background" Value="Red" />
                    </DataTrigger>
                </Style.Triggers>
            </Style>
        </Button.Style>
    </Button>
```

[datatrigger on enum to change image](https://stackoverflow.com/questions/13917033/datatrigger-on-enum-to-change-image)  
