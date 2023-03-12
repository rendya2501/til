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

---

## xaml display enum description

``` cs
public enum StatusEnum
{
    [Description("No status yet")]
    None = 0,

    [Description("Waiting to run")]
    Waiting = 1,

    [Description("Run in progress")]
    Running = 2,

    [Description("Paused by user")]
    Paused = 3,

    [Description("Run is a success")]
    Success = 4,

    [Description("Run finished with error")]
    Failure = 5
}
```

``` cs
public class EnumDescriptionConverter : IValueConverter
{
    private static string GetEnumDescription(Enum enumObj)
    {
        foreach (var att in enumObj.GetType().GetField(enumObj.ToString()).GetCustomAttributes(false))
        {
            return att is DescriptionAttribute attrib ? attrib.Description : enumObj.ToString();
        }
        return enumObj.ToString();
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is Enum myEnum ? GetEnumDescription(myEnum) : string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
```

``` cs

public class MainWindowViewModel : BindableBase
{
    private List<StatusEnum> _statuses;
    public List<StatusEnum> Statuses
    {
        get { return _statuses; }
        set { SetProperty(ref _statuses, value); }
    }

    private StatusEnum _selectedStatus;
    public StatusEnum SelectedStatus
    {
        get { return _selectedStatus; }
        set
        {
            SetProperty(ref _selectedStatus, value);
            Result = $"This is the selected Status: {value}";
        }
    }

    private string _result;
    public string Result
    {
        get { return _result; }
        set { SetProperty(ref _result, value); }
    }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public MainWindowViewModel()
    {
        IncrementCommand = new DelegateCommand(Increment);
        DecrementCommand = new DelegateCommand(Decrement, () => Count > 0);

        Statuses = new List<StatusEnum>
            {
                StatusEnum.Waiting,
                StatusEnum.Running,
                StatusEnum.Paused,
                StatusEnum.Success,
                StatusEnum.Failure
            };
        SelectedStatus = Statuses.First();
    }
}
```

``` xml
    xmlns:common="clr-namespace:PrismQuickStart.Common"

    <Window.Resources>
        <common:EnumDescriptionConverter x:Key="enumConverter" />
    </Window.Resources>

    <Grid>
        <StackPanel Orientation="Vertical">
            <!--  DISPLAY THE COMBOBOX WITH OUR SELECTED ENUMS  -->
            <ComboBox
                Width="200"
                Height="25"
                Margin="5"
                ItemsSource="{Binding Statuses}"
                SelectedItem="{Binding SelectedStatus}">
                <ComboBox.ItemTemplate>
                    <DataTemplate>
                        <TextBlock Text="{Binding Converter={StaticResource enumConverter}}" />
                    </DataTemplate>
                </ComboBox.ItemTemplate>
            </ComboBox>
            <!--  DISPLAY THE RESULT OF THE SELECTION  -->
            <Label
                Width="200"
                Height="25"
                Margin="5"
                Background="AliceBlue"
                Content="{Binding Result}" />
        </StackPanel>
    </Grid>
```

[WPF - Enum Binding With Description in a ComboBox - Code4Noobz](https://code.4noobz.net/wpf-enum-binding-with-description-in-a-combobox/)  
