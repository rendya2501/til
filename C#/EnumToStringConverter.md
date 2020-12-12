# Enumを任意の文字列に変換する方法

Enumのメンバーを任意の文字列に変換するため業務中に作った作品。  
割とうまくできたので、備忘録として乗せておく。  
方針としてはDisplayアノテーションの内容を変換文字列として使う方法。  

```C#:Converter
using System;
using System.Globalization;
using System.Windows.Data;
using System.Reflection;
using System.ComponentModel.DataAnnotations;

namespace RN3.Wpf.Common.Converter
{
    /// <summary>
    /// 
    /// </summary>
    public class EnumToStringConverter : IValueConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                FieldInfo field = value?.GetType().GetField(value?.ToString());
                DisplayAttribute attr = field.GetCustomAttribute<DisplayAttribute>();
                if (attr != null)
                {
                    return attr.Name;
                }
                return value.ToString();
            }
            catch
            {
                return string.Empty;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
```

```C#:Enum
using System.ComponentModel.DataAnnotations;

namespace RN3.Wpf.Common.Data.Enum
{
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
}
```

```C#:XAML
<c1:Column
    Width="65"
    HorizontalAlignment="Left"
    VerticalAlignment="Center"
    Binding="{Binding TaxationType,
              Converter={StaticResource EnumToStringConverter},
              Mode=OneWay}"
    ColumnName="TaxationTypeName"
    Header="課税"
    HeaderHorizontalAlignment="Center"
    HeaderVerticalAlignment="Center" />
```
