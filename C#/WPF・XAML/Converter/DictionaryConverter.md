# DictionaryConverter

---

## 概要

Enumを任意の文字列に変換するコンバーターを実装しました。(**DictionaryConverter**)  
「EnumToStringConverter」でないのはEnumと紐づいているItemSourceのDictionaryを変換するためです。  

---

## 使い方

1. Bindingには対象Enumを指定してください。
2. ConverterにはDictionaryConverterを指定してください。
3. ConverterParameterにはEnumと紐づいているItemSourceとそのメンバを指定してください。

---

## 実装例

``` XML
<Control
    Binding="{Binding SampleEnum,
        Converter={StaticResource DictionaryConverter},
        ConverterParameter={x:Static isrc:SampleItemSource.SampleTypeList}
    }"
/>
```

``` C# : ItemSource
    /// <summary>
    /// サンプルアイテムソース
    /// </summary>
    public static class SampleItemSource
    {
        public static Dictionary<SampleEnum, string> SampleTypeList
        {
            get => new Dictionary<SampleEnum, string>()
            {
                { SampleEnum.SampleA, "サンプルA" },
                { SampleEnum.SampleB, "サンプルB" },
                { SampleEnum.SampleC, "サンプルC" }
            };
        }
    }
```

``` C# : DictionaryConverter
using System;
using System.Collections;
using System.Globalization;
using System.Windows.Data;

namespace namespace
{
    /// <summary>
    /// DictionaryのKeyをValueに変換するコンバーター
    /// </summary>
    public class DictionaryConverter : IValueConverter
    {
        /// <summary>
        /// DictionaryのKeyをValueに変換します。
        /// </summary>
        /// <param name="value">バインディング ソースによって生成された値</param>
        /// <param name="targetType">バインディング ターゲット プロパティの型</param>
        /// <param name="parameter">使用するコンバーター パラメーター</param>
        /// <param name="culture">コンバーターで使用するカルチャ</param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 値、型チェック
            if (value == null) return null;
            if (!(parameter is IDictionary)) throw new Exception(string.Format(Message.Invalid, "型"));
            // パラメータの型変換
            var dictionary = (IDictionary)parameter;
            // インデクサーで値を取得
            return dictionary[value];
        }
        /// <summary>
        /// OneWayでのBindingでしか使用しません。
        /// </summary>
        /// <param name="value">バインディング ソースによって生成された値</param>
        /// <param name="targetType">バインディング ターゲット プロパティの型</param>
        /// <param name="parameter">使用するコンバーター パラメーター</param>
        /// <param name="culture">コンバーターで使用するカルチャ</param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
```
