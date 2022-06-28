# EnumToStringConverter

## Enumを任意の文字列に変換するコンバーター

Enumのメンバーを任意の文字列に変換するため業務中に作った作品。  
採用はされなかったが、割とうまくできたので、備忘録として乗せておく。  
方針としてはDisplayアノテーションの内容を変換文字列として使う方法。  

``` C#
    /// <summary>
    /// Enumを任意の文字列に変換するコンバーター
    /// </summary>
    public class EnumToStringConverter : IValueConverter
    {
        /// <summary>
        /// 
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                FieldInfo field = value?.GetType().GetField(value?.ToString());
                DisplayAttribute attr = field.GetCustomAttribute<DisplayAttribute>();
                return attr != null
                    ? attr.Name
                    : value.ToString();
            }
            catch
            {
                return string.Empty;
            }
        }
        /// <summary>
        /// 使わない
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
```

``` C#
    /// <summary>
    /// 課税区分項目
    /// </summary>
    public enum TaxationType : byte
    {
        /// <summary>
        /// 外税
        /// </summary>
        [Display(Name = "外税")]
        OutsideTax = 1,
        /// <summary>
        /// 内税
        /// </summary>
        [Display(Name = "内税")]
        InsideTax = 2,
        /// <summary>
        /// 非課税
        /// </summary>
        [Display(Name = "非課税")]
        TaxFree = 3
    }
```

``` XML : 使い方
<c1:Column
    Width="65"
    HorizontalAlignment="Left"
    VerticalAlignment="Center"
    Binding="{Binding TaxationType, Converter={StaticResource EnumToStringConverter}, Mode=OneWay}"
    ColumnName="TaxationTypeName"
    Header="課税"
    HeaderHorizontalAlignment="Center"
    HeaderVerticalAlignment="Center" />
```
