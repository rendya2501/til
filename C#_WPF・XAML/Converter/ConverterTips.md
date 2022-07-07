# ConverterTips

---

## BindingのConverterParameterプロパティ

今まで地味に謎だった、Converterの第3引数`parameter`に値を入れるサンプル。  

<http://cswpf.seesaa.net/article/313843710.html>
>「ConverterParameterもBindingして動的に変更できればいいのですがそれができないのであまり使えません。」  

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
