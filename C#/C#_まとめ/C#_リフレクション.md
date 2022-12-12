# C#_リフレクション

---

## アトリビュートに設定されている文字列の長さを取得する方法

``` C#
/// <summary>
/// Codeの桁数を取得
/// </summary>
private static readonly int CodeLength = (Attribute.GetCustomAttribute(typeof(TestClass).GetProperty(nameof(TestClass.Code)), typeof(StringLengthAttribute)) as StringLengthAttribute)?.MaximumLength ?? 20;
```

---

## 親クラスの全プロパティの値を子クラスにコピーする方法

``` C#
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="parent"></param>
    public Child(Parent parent)
    {
        // 親クラスのプロパティ情報を一気に取得して使用する。
        List<PropertyInfo> props = parent
            .GetType()
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .ToList();
        foreach (var prop in props)
        {
            var propValue = prop.GetValue(parent);
            typeof(Child).GetProperty(prop.Name).SetValue(this, propValue);
        }
    }
```

``` C# : 少し応用してキーバリューで出力するサンプル
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="parent"></param>
    public Child(Parent parent)
    {
        // 親クラスのプロパティ情報を一気に取得して使用する。
        var props = parent
            .GetType()
            .GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public)
            .Select(s => (key: s.Name ,value: s.GetValue(parent)))
            .ToList();
        foreach (var (key,value) in props)
        {
            typeof(Child).GetProperty(key).SetValue(this, value);
        }
    }
```

[【C#】親クラスの全プロパティの値を子クラスに簡単にコピーできるようにする方法](https://qiita.com/microwavePC/items/54f0082f3d76922a6259)  

---

## RefrectionからNullableのEnumを判定する方法

c# reflection property nullable enum

``` C# : NullableのEnumも判定できる関数
    public static bool IsNullableEnum(Type t)
    {
        Type u = Nullable.GetUnderlyingType(t);
        return (u != null) && u.IsEnum;
    }
```

``` C# : 判定例
    var aa = new HogeClass();
    // nullableのenumではないのでfalse
    Console.WriteLine(IsNullableEnum(aa.GetType().GetProperty(nameof(HogeClass.Hoge)).PropertyType)); // false
    // nullableのenumなのでtrue
    Console.WriteLine(IsNullableEnum(aa.GetType().GetProperty(nameof(HogeClass.NullHoge)).PropertyType)); // true

    public class HogeClass
    {
        public HogeEnum Hoge { get; set; }
        public HogeEnum? NullHoge { get; set; }
    }

    public enum HogeEnum
    {
        Fuga,
        Hoge
    }
```

[How can I get the Enum type with reflection?](https://www.daniweb.com/programming/software-development/threads/333539/how-can-i-get-the-enum-type-with-reflection)  

---

## RefrectionでEnumに代入する

このようなクラスがあったとして、

``` C#
    enum SomeEnum
    {
        FOO = 1,
        BAR = 2,
        BAZ = 3,
    }

    class Hoge
    {
        public SomeEnum SomeEnum { get; set; }
        public SomeEnum? NullSomeEnum { get; set; }
    }
```

リフレクションでEnumに代入する方法はこんな感じ。  
Enum.ToObjectによる変換が有効。  
Nullableの場合は`Nullable.GetUnderlyingType`

``` C#
    var hoge = new Hoge();

    // 通常、nullableのTypeを取得
    var hogeEnumType = hoge.GetType().GetProperty(nameof(Hoge.SomeEnum)).PropertyType;
    var nullHogeEnumType = hoge.GetType().GetProperty(nameof(Hoge.NullSomeEnum)).PropertyType;

    // 通常のEnumへの代入
    hoge.GetType()
        .GetProperty(nameof(Hoge.SomeEnum))
        .SetValue(
            hoge,
            Enum.ToObject(hogeEnumType, 1)
        );
    // nullableのEnumへの代入
    hoge.GetType()
        .GetProperty(nameof(Hoge.NullSomeEnum))
        .SetValue(
            hoge,
            Enum.ToObject(Nullable.GetUnderlyingType(nullHogeEnumType), 3)
        );

    Console.WriteLine(hoge.SomeEnum); // FOO
    Console.WriteLine(hoge.NullSomeEnum); // BAZ
```

---

## 47.プロパティの値を取得する

``` C#
// 1行で完結させたやつ
object value = obj.GetType().GetProperty("MyProperty").GetValue(obj);

// ばらしたやつ
Type type = obj.GetType();
PropertyInfo prop = type.GetProperty("MyProperty");
object value = prop.GetValue(obj);
Console.WriteLine(value);   
```

[C#リフレクションTIPS 55連発](https://qiita.com/gushwell/items/91436bd1871586f6e663)  
