# NullableDictionary

仕様フラグでnull全ての場合をコンバートしたいがためにDictionaryのKeyにnullを指定できないかいろいろ試したやつがこれ。  
結局、KeyValuePairのnullをコンバートできる方法を見つけたので、これはお蔵入りになったが備忘録として残す。  

<https://qiita.com/Temarin/items/27614d879e9376421aae>  
<https://stackoverflow.com/questions/4632945/dictionary-with-null-key>  

NullableDictionary  
<https://qiita.com/chocolamint/items/9f13fe7e3c6343f898c2>  
<https://qiita.com/RyotaMurohoshi/items/03937297810e7c9aaf8b>  

## NullableStruct

``` C# : 詳細
    /// <summary>
    /// Dictionaryのキーにnullを許容させるための構造体
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>
    /// 構造体はnullになりえない性質なので、nullを構造体でラップすることでDictionaryを騙す。
    /// </remarks>
    public struct Nullable<T>
    {
        /// <summary>
        /// 入力値
        /// </summary>
        public T Value { get; }
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="value"></param>
        /// <remarks>
        /// 不要なNewを避けるためPrivateで宣言する。
        /// Dictionaryからコンストラクタが呼び出されるのではなく、implicit operator経由でたどり着く。
        /// </remarks>
        public Nullable(T value) => Value = value;

        /// <summary>
        /// 演算子のオーバーロード。
        /// == 演算子を使ってxとyの比較を行った場合に呼び出される。
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator ==(Nullable<T> x, Nullable<T> y) => x.Equals(y);
        /// <summary>
        /// 演算子のオーバーロード。
        /// =! 演算子を使ってxとyの比較を行った場合に呼び出される。
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator !=(Nullable<T> x, Nullable<T> y) => !x.Equals(y);
        /// <summary>
        /// Nullable<T>型をT型に変換します。
        /// 「var b1 = new Nullable<double>(1); var b2 = (int)b1;」等のキャストを行ったときに呼び出される。
        /// explicit:明示的にキャストしないとエラー。new Dictionary<Nullable<int?>, string>(){(Nullable<int?>)null, "null" }→こうしないといけない。
        /// implicit:明示的にキャストしなくてもエラーにならない。 new Dictionary<Nullable<int?>, string>(){null, "null" }→これでもOK。
        /// </summary>
        /// <param name="source"></param>
        public static implicit operator T(Nullable<T> source) => source.Value;
        /// <summary>
        /// T型をNullable<T>型に変換します。
        /// 変換するT型の値をコンストラクタの引数としてNullable<T>型を生成することでT型をNullable<T>型に変換します。
        /// 「(Nullable<T>)null」 等のキャストを行ったときに呼び出される。
        /// </summary>
        /// <param name="source"></param>
        public static implicit operator Nullable<T>(T source) => new Nullable<T>(source);
        /// <summary>
        /// ToStringのオーバーロード。
        /// Dictionaryへの入力以外に使う予定はないので、これは実装しない。
        /// </summary>
        /// <returns></returns>
        public override string ToString() => Value?.ToString();

        /// <summary>
        /// ハッシュコードを取得します。
        /// nullの場合は0を返却します。
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// int 0の場合とハッシュ値が同じになるのはなぁ。
        /// でもハッシュが同じでもちゃんと動作するしな。なんなんだろう。
        /// </remarks>
        public override int GetHashCode() => Value == null ? 0 : Value.GetHashCode();

        public override bool Equals(object obj)
        {
            if (obj is Nullable<T> nullable)
            {
                //return Equals(nullable);
                return ReferenceEquals(Value, nullable.Value) || Value.Equals(nullable.Value);
            } else
            {
                return false;
            }

            // 1行にできる。
            //public override bool Equals(object obj) => obj is Nullable<T> nullable && Equals(nullable);
        }
    }
```

---

## ユースケース

``` C# : 動作チェック
    foreach (var item in ValidFlagItemSource.ValidFlagListHasBlank)
    {
        Console.WriteLine(Convert(item.Key, ValidFlagItemSource.ValidFlagListHasBlank));
    }
```

``` C# : ItemsSource
    /// <summary>
    /// 使用フラグアイテムソース
    /// </summary>
    public static class ValidFlagItemSource
    {
        /// <summary>
        /// すべてを含む一覧
        /// </summary>
        public static IList<KeyValuePair<bool?, string>> ValidFlagListHasBlank
        {
            get
            {
                return new List<KeyValuePair<bool?, string>>()
                {
                    new KeyValuePair<bool?, string>(null, "すべて"),
                    new KeyValuePair<bool?, string>(true, "使用する"),
                    new KeyValuePair<bool?, string>(false, "使用しない"),
                };
            }
        }
    }
```

``` C# : Converter
    public static class DictionaryConverter
    {
        /// <summary>
        /// DictionaryのKeyをValueに変換します。
        /// </summary>
        /// <param name="value">バインディング ソースによって生成された値</param>
        /// <param name="targetType">バインディング ターゲット プロパティの型</param>
        /// <param name="parameter">使用するコンバーター パラメーター</param>
        /// <param name="culture">コンバーターで使用するカルチャ</param>
        /// <returns></returns>
        public static object Convert(object value, object parameter)
        {
            // if (!(parameter is IDictionary)) throw new Exception("型");
            // パラメータの型変換
            var dictionary = (IDictionary)parameter;
            // インデクサーで値を取得
            return dictionary[value];
        }
    }
```

``` C# : リリース用ソース
    /// <summary>
    /// Dictionaryのキーにnullを許容させるための構造体
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>
    /// 構造体はnullになりえない性質なので、nullを構造体でラップすることでDictionaryを騙す。
    /// </remarks>
    public struct Nullable<T>
    {
        /// <summary>
        /// 入力値
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="value">入力値</param>
        private Nullable(T value) => Value = value;

        /// <summary>
        /// 演算子のオーバーロード。
        /// == 演算子を使ってxとyの比較を行った場合に呼び出される。
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator ==(Nullable<T> x, Nullable<T> y) => x.Equals(y);
        /// <summary>
        /// 演算子のオーバーロード。
        /// != 演算子を使ってxとyの比較を行った場合に呼び出される。
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator !=(Nullable<T> x, Nullable<T> y) => !x.Equals(y);
        /// <summary>
        /// キャストのオーバーロード
        /// Nullable<T>型をT型に変換する時に呼び出されます。
        /// </summary>
        /// <param name="source"></param>
        public static implicit operator T(Nullable<T> source) => source.Value;
        /// <summary>
        /// キャストのオーバーロード。
        /// T型をNullable<T>型に変換する時に呼び出されます。
        /// </summary>
        /// <param name="source"></param>
        public static implicit operator Nullable<T>(T source) => new Nullable<T>(source);

        /// <summary>
        /// ハッシュコードを取得します。
        /// nullの場合は0を返却します。
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => Value == null ? 0 : Value.GetHashCode();
        /// <summary>
        /// 2つのオブジェクト インスタンスが等しいかどうかを判断します。
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj) => 
            obj is Nullable<T> nullable
            && (ReferenceEquals(Value, nullable.Value) || Value.Equals(nullable.Value));
    }
```

---

## 速度エビデンス

[構造体を定義すると Equals が自動的に実装されるが、IEquatable<T> を実装した方がよい](http://noriok.hatenadiary.jp/entry/2017/07/17/233146)  

こんな記事を見つけたので、ついでに実装して速度を図ってみたら、OverLoadのほうが早かったというやつ。  

``` txt
Point             = 1928ms
Point(OverLoad)   = 252ms
Point(IEquatable) = 493ms
```

OverLoadしたソースはこれ。  

``` C#
    /// <summary>
    /// 速度比較用
    /// オーバーロードのみ
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct NullableOverLoad<T>
    {
        public T Value { get; }
        public NullableOverLoad(T value) => Value = value;
        public static bool operator ==(NullableOverLoad<T> x, NullableOverLoad<T> y) => x.Equals(y);
        public static bool operator !=(NullableOverLoad<T> x, NullableOverLoad<T> y) => !x.Equals(y);
        public static implicit operator T(NullableOverLoad<T> source) => source.Value;
        public static implicit operator NullableOverLoad<T>(T source) => new NullableOverLoad<T>(source);
        public override int GetHashCode() => Value == null ? 0 : Value.GetHashCode();
        public override bool Equals(object obj) => obj is Nullable<T> nullable && Equals(nullable);
    }
```

一番速度出なかった奴はこれ。  

``` C#
    /// <summary>
    /// 速度比較用 IEquatableなしVer
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct NullableNotEquatable<T>
    {
        public T Value { get; }
        public NullableNotEquatable(T value) => Value = value;
        public static implicit operator T(NullableNotEquatable<T> source) => source.Value;
        public static implicit operator NullableNotEquatable<T>(T source) => new NullableNotEquatable<T>(source);
        public override int GetHashCode() => Value == null ? 0 : Value.GetHashCode();
    }
```
