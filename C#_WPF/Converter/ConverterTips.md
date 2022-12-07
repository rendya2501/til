# ConverterTips

---

## BindingのConverterParameterプロパティ

今まで地味に謎だった、Converterの第3引数`parameter`に値を入れるサンプル。  

>「ConverterParameterもBindingして動的に変更できればいいのですがそれができないのであまり使えません。」  
[[C# WPF]Binding ConverterParameterプロパティを使う](http://cswpf.seesaa.net/article/313843710.html)  

→  
バインドできないのか・・・。  
これを使うくらいならMultiBinding使ったほうがいいという記事が散見される。  

``` XML
<Window.Resources>
    <local:HexConverter x:Key="HexConv"/>
</Window.Resources>
<Grid>
    <Grid.RowDefinitions>
        <RowDefinition Height="Auto"/>
        <RowDefinition Height="Auto"/>
    </Grid.RowDefinitions>
    <TextBox Text="{Binding Value, UpdateSourceTrigger=PropertyChanged}"
         Width="70" Height="23" Margin="5"
         HorizontalAlignment="Left" VerticalAlignment="Top"/>
    <TextBlock Text="{Binding Value, Converter={StaticResource HexConv}, ConverterParameter=4}"
        Grid.Row="1" 
        Width="70" 
        Height="23" 
        Margin="5"
        HorizontalAlignment="Left" 
        VerticalAlignment="Top"/>
</Grid>
```

``` C#
public class HexConverter : IValueConverter
{
    public object Convert(object value, Type type, object parameter, CultureInfo culture)
    {
        int v = (int)value;
        int p = int.Parse((string)parameter);
        return string.Format("0x{0:X" + p.ToString() + "}", v);
    }

    public object ConvertBack(object value, Type type, object parameter, CultureInfo culture)
    {
        string s = (string)value;
        return int.TryParse(s, NumberStyles.AllowHexSpecifier, null, out int v) ? v : 0;
    }
}
```

---

## CustomConverterのプロパティにバインドする方法

``` cs
public class DependencyObjectConverter : DependencyObject, IValueConverter
{
    public static readonly DependencyProperty IsReverseProperty
        = DependencyProperty.Register("IsReverse", typeof(bool), typeof(DependencyObjectConverter));
    public bool IsReverse
    {
        get { return (bool)GetValue(IsReverseProperty); }
        set { SetValue(IsReverseProperty, value); }
    }

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        var format = IsReverse ? "-##,#;##,#" : "##,#";
        return string.Format("{0:" + format + "}", value);
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
```

``` xml
<Window.Resources>
    <FrameworkElement x:Key="proxyElement" DataContext="{Binding}"/>

    <local:DependencyObjectConverter 
        x:Key="DependencyObjectNormal"
        IsReverse="{Binding Path=DataContext.Normal, Source={StaticResource ResourceKey=proxyElement}}"/>
    <local:DependencyObjectConverter 
        x:Key="DependencyObjectReverse"
        IsReverse="{Binding Path=DataContext.Reverse, Source={StaticResource ResourceKey=proxyElement}}"/>
</Window.Resources>

<TextBlock Text="{Binding Path=Quantity, Converter={StaticResource ResourceKey=DependencyObjectReverse}}"/>
```

[C# + WPFで、Converter + DependencyObject / MuliBinding を使って動的にViewの書式設定をする](https://thinkami.hatenablog.com/entry/2014/10/27/063202)  
