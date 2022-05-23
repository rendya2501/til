# KeyValuePairConverter

いつぞや、ディクショナリーの値をコンボボックスに配置するために作ったコンバーター。  
うまくできたので置いておく。  

確か、商品台帳作ってて課税区分のItemsSourceを文字列に変換するために作ったっけか。  
当時は無理だと思っていたけど、探せばそれなりにサンプルがあったのでちょっと形を整えて無事に行けた。  
と、思ったらちゃんとwikiに記事を残していたので引っ張ってきます。  

[C# Reflection - How can I tell if object o is of type KeyValuePair and then cast it?](https://stackoverflow.com/questions/2729614/c-sharp-reflection-how-can-i-tell-if-object-o-is-of-type-keyvaluepair-and-then)  

---

## 概要

ItemsSourceのIList<KeyValuePair<K, V>>を変換するコンバーターを実装しました。  
当初はEnumを任意の文字列に変換したかったために開発したものでしたが、IList<KeyValuePair<K, V>>の性質上、Enum以外の変換も可能なはずです。(未検証)  

DictionaryConverterを実装した時、「特別な理由がない限り、**IList**<**KeyValuePair**<**Key,Value**>>から**Dictionary**<**Enum,String**>にしてください。」とアナウンスしましたが、これを取り消します。  
KeyValuePairConverterが完成したのでDictionaryにする必要がなくなったためです。  
今後もItemsSourceはIList<KeyValuePair<Key,Value>>での実装をお願いします。  

---

## 使い方(Enumの場合)

1. Bindingには対象Enumを指定してください。  
2. ConverterにはKeyValuePairConverterを指定してください。  
3. ConverterParameterにはEnumと紐づいているItemSourceとそのメンバを指定してください。  

---

## 実装例

参考 : 商品台帳 TaxationTypeItemSource  

``` XML
<Control
    Binding="{Binding SampleEnum,
        Converter={StaticResource KeyValuePairConverter},
        ConverterParameter={x:Static isrc:SampleItemSource.SampleTypeList}
    }"
/>
```

``` C#
    /// <summary>
    /// サンプルアイテムソース
    /// </summary>
    public static class SampleItemSource
    {
        /// <summary>
        /// サンプル一覧
        /// </summary>
        public static IList<KeyValuePair<SampleEnum, string>> SampleTypeList
        {
            get
            {
                return new List<KeyValuePair<SampleEnum, string>>()
                {
                    new KeyValuePair<SampleEnum, string>( SampleEnum.SampleA, "サンプルA"),
                    new KeyValuePair<SampleEnum, string>( SampleEnum.SampleB, "サンプルB"),
                    new KeyValuePair<SampleEnum, string>( SampleEnum.SampleC, "サンプルC")
                };
            }
        }
    }
```

``` C# : KeyValuePairConverter
    /// <summary>
    /// KeyValuePairのKeyをValueに変換するコンバーター
    /// </summary>
    public class KeyValuePairConverter : IValueConverter
    {
        /// <summary>
        /// KeyValuePairのKeyをValueに変換します。
        /// </summary>
        /// <param name="value">バインディング ソースによって生成された値</param>
        /// <param name="targetType">バインディング ターゲット プロパティの型</param>
        /// <param name="parameter">使用するコンバーター パラメーター</param>
        /// <param name="culture">コンバーターで使用するカルチャ</param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // null判定
            if (parameter == null) throw new Exception(string.Format(Message.Invalid, "値"));
            // 型の判定とIListへ変換
            if (!(parameter is IList list)) throw new Exception(string.Format(Message.Invalid, "型"));
            // 要素をループ
            foreach (var item in list)
            {
                // 値が一般的であることを確認
                Type valueType = item.GetType();
                if (valueType.IsGenericType)
                {
                    // ジェネリック型の定義を抽出
                    Type baseType = valueType.GetGenericTypeDefinition();
                    // KeyValuePair型の判定
                    if (baseType == typeof(KeyValuePair<,>))
                    {
                        // KeyとValueの取得
                        var kvpKey = valueType.GetProperty("Key")?.GetValue(item, null);
                        var kvpValue = valueType.GetProperty("Value")?.GetValue(item, null);
                        // Keyと引数valueの比較
                        if (kvpKey?.Equals(value) ?? kvpKey == value)
                        {
                            return kvpValue;
                        }
                    }
                }
            }
            // Keyに合致するものがなければnullを返却。
            return null;
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
```

---

## 実際の実装例

``` C#
    /// <summary>
    /// 曜日アイテムソース
    /// </summary>
    public static class DayOfTheWeekTypeItemSource
    {
        /// <summary>
        /// 曜日一覧
        /// </summary>
        public static IList<KeyValuePair<DayOfTheWeekType, string>> DayOfTheWeekTypeList
        {
            get
            {
                return new List<KeyValuePair<DayOfTheWeekType, string>>()
                {
                    new KeyValuePair<DayOfTheWeekType, string>( DayOfTheWeekType.Sun, "日" ),
                    new KeyValuePair<DayOfTheWeekType, string>( DayOfTheWeekType.Mon, "月" ),
                    new KeyValuePair<DayOfTheWeekType, string>( DayOfTheWeekType.Tue, "火" ),
                    new KeyValuePair<DayOfTheWeekType, string>( DayOfTheWeekType.Wed ,"水" ),
                    new KeyValuePair<DayOfTheWeekType, string>( DayOfTheWeekType.Thu, "木" ),
                    new KeyValuePair<DayOfTheWeekType, string>( DayOfTheWeekType.Fri, "金" ),
                    new KeyValuePair<DayOfTheWeekType, string>( DayOfTheWeekType.Sat, "土" ),
                };
            }
        }
    }
```

``` XML
    <c1:Column
        Width="50"
        HorizontalAlignment="Center"
        VerticalAlignment="Center"
        Binding="{Binding DayOfTheWeek, Converter={StaticResource KeyValuePairConverter}, ConverterParameter={x:Static isrc:DayOfTheWeekTypeItemSource.DayOfTheWeekTypeList}, Mode=OneWay}"
        ColumnName="DayOfTheWeek"
        Header="曜日"
        HeaderHorizontalAlignment="Center"
        HeaderVerticalAlignment="Center"
        Visible="True" />
```

---
---

## DictionaryConverter実装の時の内容

DictionaryConverter自体は残っているので使おうと思えば使えます。
まぁ使うことはないだろうし、ボツだけど一応残しておく。

## 概要

Enumを任意の文字列に変換するコンバーターを実装しました。(**DictionaryConverter**)
「EnumToStringConverter」でないのはEnumと紐づいているItemSourceのDictionaryを変換するためです。

※今後ItemsSourceは特別な理由がない限り、**IList**<**KeyValuePair**<**Key,Value**>>から**Dictionary**<**Enum,String**>にしてください。
また、余力があるようでしたら、可能な限りIListからDictionaryへの変換作業も行ってください。

---

## 使い方

1. Bindingには対象Enumを指定してください。
2. ConverterにはDictionaryConverterを指定してください。
3. ConverterParameterにはEnumと紐づいているItemSourceとそのメンバを指定してください。

---

## 実装例

参考 : 商品台帳 TaxationTypeItemSource  

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
using RN3.Wpf.Common.Resource;
using System;
using System.Collections;
using System.Globalization;
using System.Windows.Data;

namespace RN3.Wpf.Common.Converter
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
