# C# Json関連まとめ

[Newtonsoft.Jsonライブラリの使用方法](https://blog.hiros-dot.net/?p=8766#toc20)  
[C#でJSONを読み書きする方法](https://usefuledge.com/csharp-json.html)  
[Microsoft公式](https://docs.microsoft.com/ja-jp/dotnet/standard/serialization/system-text-json-how-to?pivots=dotnet-6-0)  
[JSONの読み込み・デシリアライズを分かりやすく解説【C# Json.NET】](https://tech-and-investment.com/json2/)  
[JSONファイルの作成方法を分かりやすく解説【C# Json.NET】](https://tech-and-investment.com/json3/)  

C#に置けるjsonの表現方法はクラスそのものか、テキストによる直接の表現か。  
基本的に、デシリアライズすることによってクラスをjsonに変換するのが主な用途で、受け皿はすべてクラス。  
なので、クラスを挟んでシリアライズ、デシリアライズすればjsonは扱える。  

---

## 各ライブラリにおけるシリアライズ・デシリアライズの書式

[【C#】JSONのシリアライザは、System.Text.JSONを使おう。](https://qiita.com/SY81517/items/1cf6246dd99869f7b9c5)  

[VB/C#でJSONの読み込み (System.Text.Json編)](https://www.umayadia.com/Note/Note010VBSystem.Text.Json.htm)  
VB/C#でJSONを読み書きするには、JSON.NET(Newtonsoft JSON)またはSystem.Text.Jsonを使用するのが一般的です。  
JSON.NETは多機能で使われる頻度が高く2020年3月現在ではデファクトスタンダードです。  
System.Text.Jsonは新しくMicrosoftが開発したライブラリで機能は少なめですがパフォーマンスが優れており、Webアプリケーションのスループットを向上させることができるようです。  

### NewtonJson

NewtonJsonはMITライセンスで公開されている.NET用のオープンソースJSONライブラリで、以下の特徴を謳っています。  

- High Performance  
  - DataContractJsonSerializerより50%高速  
- LINQ to JSON  
  - JSONの作成、パース、検索、変更をJObject、JArray、JValueオブジェクトを使って行える  
- Easy To Use  
- Run Anywhere  
  - WindowsでもMonoでもXamarinでも使える  
- JSON Path  
  - XPathライクな書き方でJSONのクエリが行える  
- XML Support  
  - XMLとJSONの相互変換をサポート

``` C# : NewtonJson
using Newtonsoft.Json;

// シリアライズ
string json = JsonConvert.SerializeObject(account, Formatting.Indented);

// デシリアライズ
Person person = JsonConvert.DeserializeObject<Person>(jsonData);
// デシリアライズパターン2 使うことはないだろう
var aa = (Account?)new JsonSerializer().Deserialize(file, typeof(Account));
```

### System.Text.Json

.NET Core 3.0以降から標準搭載されたMicrosoft公式のJsonシリアライザー。  
公式推奨ライブラリ。  

``` C# : System.Text.Json
using System.Text.Json;

// シリアライズ
var options = new JsonSerializerOptions { WriteIndented = true };
string jsonString = JsonSerializer.Serialize(weatherForecast, options);

// デシリアライズ
WeatherForecast? weatherForecast = JsonSerializer.Deserialize<WeatherForecast>(jsonString);
```

### DataContractJsonSerializer

[[C#] C#でJSONを扱う方法まとめ](https://dev.classmethod.jp/articles/c-sharp-json/)  

.NET Frameworkで提供されている、オブジェクトをJSONにシリアライズ、JSONをオブジェクトにデシリアライズするためのクラスです。  
名前空間はSystem.Runtime.Serialization.Json、アセンブリはSystem.Runtime.Serialization.dllです。  

- Microsoft社はDataContractJsonSerializerを推奨していない。  
- 一番古いJsonライブラリなので、今更採用する意味はない。  

``` C# : System.Runtime.Serialization.Json
using System.Runtime.Serialization.Json;

public static class JsonUtility
{
    /// <summary>
    /// 任意のオブジェクトをJSON文字列にシリアライズします。
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static string Serialize(object obj)
    {
        using (var stream = new MemoryStream())
        {
            var serializer = new DataContractJsonSerializer(obj.GetType());
            serializer.WriteObject(stream, obj);
            return Encoding.UTF8.GetString(stream.ToArray());
        }
    }
    /// <summary>
    /// JSON文字列を任意のオブジェクトにデシリアライズします。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="message"></param>
    /// <returns></returns>
    public static T Deserialize<T>(string message)
    {
        using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(message)))
        {
            var serializer = new DataContractJsonSerializer(typeof(T));
            return (T)serializer.ReadObject(stream);
        }
    }
}
```

### 結局どれを使えばいいの？

``` json
System.Text.Json 諸事情によりサードパーティライブラリを使えない時。
NewtonJson       高機能な検索やパフォーマンスを出したい時。
DataContractJsonSerializer 諸事情によりサードパーティライブラリを使えない時だが、わざわざ採用する理由はない。
```

### C#のJSONの取り扱いの歴史

[System.Text.Json でJSONを扱ってみよう](https://iwasiman.hatenablog.com/entry/20210614-CSharp-json)  

- System.Runtime.Serialization.Json.DataContractJsonSerializer  
- 長く使われてきたサードパーティ製ライブラリのJson.NET (NuGetで取り込むときのライブラリ名はNewtonsoft.Json)  
- C#4.0で導入されたライブラリのDynamic.Json  

ググると上記が入り混じってヒットするかと思います。特にJson.NETが広く使われたのでよくヒットしますね。  

2015/11リリースの .NET Framework 4.6.1 からSystem.Text.Jsonが導入されパッケージ管理のNuGetでインストールすると使えるようになり、実行環境自体が刷新された .NET Core では2017/8リリースの .NET Core 2.0 から標準搭載。  
2021年現在はこの [System.Text.Json] が**デフォルトで推奨**となっています。  

---

## ファイル読み書きを含めたサンプルコード

``` C# : Read
    // exeと同じ階層にあるtest.json を UTF-8 で開く
    using StreamReader sr = new StreamReader("test2.json", Encoding.UTF8);
    // ファイルの内容をデシリアライズして person にセット
    return JsonConvert.DeserializeObject<Person>(sr.ReadToEnd());


    // jsonファイルを読み込みます
    using StreamReader file = File.OpenText(@"C:\Users\rendy\Desktop\CSharpSample1\CSharpSample1\Json\test.json");
    // デシリアライズ関数に読み込んだファイルと、データ用クラスの名称(型)を指定します。
    // デシリアライズされたデータは、自動的にaccountのメンバ変数に格納されます 
    return (Account?)new JsonSerializer().Deserialize(file, typeof(Account));
```

``` C# : Write
    // exeと同じ階層にあるtest2.json を UTF-8 で書き込み用でオープン
    using var sw = new StreamWriter("test2.json", false, Encoding.UTF8);
    // JSON データをファイルに書き込み
    sw.Write(JsonConvert.SerializeObject(jsonObject));


    // 作成したオブジェクトのデータをJSONにシリアライズします
    // 一つ目に引数に、シリアライズするオブジェクトを指定します
    // 二つ目に引数に、インデントの有無を指定します。
    string json = JsonConvert.SerializeObject(account, Formatting.Indented);
    // シリアライズ済みデータ(文字列)をファイルに書き込みます
    File.WriteAllText("Account.json", json);
```

``` C#
using System.Text.Json;

    //ParseメソッドでJSON文字列をJson.NETのオブジェクトに変換
    var obj = JObject.Parse(@"{
        'people' : [
            {
                'name' : 'Kato Jun',
                'age' : 31
            },
            {
                'name' : 'PIKOTARO',
                'age' : 53
            },
            {
                'name' : 'Kosaka Daimaou',
                'age' : 43
            }
        ]
    }");
    //Json.Netのオブジェクトに変換することでLINQが使えるようになる
    var oldestPersonName = obj["people"].OrderByDescending(p => p["age"]).FirstOrDefault();
    WriteLine(oldestPersonName);
```

---

``` C# : <https://blog.hiros-dot.net/?p=8766#toc19>
    public static void Read()
    {
        Person person = new Func<Person>(() =>
        {
            // exeと同じ階層にあるtest.json を UTF-8 で開く
            using var sr = new StreamReader("test2.json", Encoding.UTF8);
            // ファイルの内容をデシリアライズして person にセット
            return JsonConvert.DeserializeObject<Person>(sr.ReadToEnd());
        })();
        Console.WriteLine("Name : " + person?.Name);
        Console.WriteLine("Age: " + person?.Age);
        Console.WriteLine("Weight  : " + person?.Weight);
    }

    public static void Write()
    {
        Person person = new Person()
        {
            Name = "takahiro",
            Age = 46,
            Weight = 1163.5,
        };
        // JSON データにシリアライズ
        var jsonData = JsonConvert.SerializeObject(person);
        new Action(() =>
        {
            // exeと同じ階層にあるtest2.json を UTF-8 で書き込み用でオープン
            using var sw = new StreamWriter("test2.json", false, Encoding.UTF8);
            // JSON データをファイルに書き込み
            sw.Write(jsonData);
        })();
        Console.WriteLine("書き込みました。");
    }

    //[JsonObject("Person")]
    class Person
    {
        //[JsonProperty("Person Name")]
        public string Name { get; set; }
        //[JsonProperty("Person Age")]
        public int Age { get; set; }
        //[JsonProperty("Person Weight")]
        public double Weight { get; set; }
    }
```

``` C# : <https://tech-and-investment.com/json3/>
    /// <summary>
    /// 読み込みサンプル
    /// https://tech-and-investment.com/json2/
    /// </summary>
    public static void Read()
    {
        try
        {
            var account = new Func<Account?>(() =>
            {
                // jsonファイルを読み込みます
                using StreamReader file = File.OpenText(@"C:\Users\rendy\Desktop\CSharpSample1\CSharpSample1\Json\test.json");
                // デシリアライズ関数に読み込んだファイルと、データ用クラスの名称(型)を指定します。
                // デシリアライズされたデータは、自動的にaccountのメンバ変数に格納されます 
                return (Account?)new JsonSerializer().Deserialize(file, typeof(Account));
            })();
            // 表示
            Console.WriteLine("Email : " + account?.Email);
            Console.WriteLine("Active: " + account?.Active);
            Console.WriteLine("Date  : " + account?.CreatedDate);
            foreach (string role in account?.Roles ?? Enumerable.Empty<string>())
            {
                Console.WriteLine("Roles : " + role);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ファイル読み込み時に例外が発生しました。:{ex.Message}");
        }
    }

    /// <summary>
    /// 書き込みサンプル
    /// https://tech-and-investment.com/json3/
    /// </summary>
    public static void Write()
    {
        // データ用のクラスオブジェクトを作成します。
        Account account = new Account
        {
            Email = "james@example.com",
            Active = true,
            CreatedDate = new DateTime(2013, 1, 20, 0, 0, 0, DateTimeKind.Utc),
            Roles = new List<string>
            {
                "User",
                "Admin"
            }
        };
        // 作成したオブジェクトのデータをJSONにシリアライズします
        // 一つ目に引数に、シリアライズするオブジェクトを指定します
        // 二つ目に引数に、インデントの有無を指定します。
        string json = JsonConvert.SerializeObject(account, Formatting.Indented);
        // シリアライズ済みデータ(文字列)をファイルに書き込みます
        File.WriteAllText("Account.json", json);
    }

    /// <summary>
    /// デシリアライズ用のデータクラスです
    /// </summary>
    private class Account
    {
        public string Email { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedDate { get; set; }
        public IList<string> Roles { get; set; }
    }
```

---

## マイクロソフト公式

``` C# : .NET オブジェクトを JSON として書き込む方法 (シリアル化)
using System.Text.Json;

namespace SerializeBasic
{
    public class WeatherForecast
    {
        public DateTimeOffset Date { get; set; }
        public int TemperatureCelsius { get; set; }
        public string? Summary { get; set; }
    }

    public class Program
    {
        public static void Main()
        {
            var weatherForecast = new WeatherForecast
            {
                Date = DateTime.Parse("2019-08-01"),
                TemperatureCelsius = 25,
                Summary = "Hot"
            };

            string jsonString = JsonSerializer.Serialize(weatherForecast);
            // 前の例では、シリアル化する型に型の推定を使用しています。 Serialize() のオーバーロードでは、ジェネリック型パラメーターを受け取ります。
            // string jsonString = JsonSerializer.Serialize<WeatherForecast>(weatherForecast);

            // 同期処理
            // string fileName = "WeatherForecast.json"; 
            // string jsonString = JsonSerializer.Serialize(weatherForecast);
            // File.WriteAllText(fileName, jsonString);

            // 非同期処理
            // string fileName = "WeatherForecast.json";
            // using FileStream createStream = File.Create(fileName);
            // await JsonSerializer.SerializeAsync(createStream, weatherForecast);
            // await createStream.DisposeAsync();

            Console.WriteLine(File.ReadAllText(fileName));
        }
    }
}
// output:
//{"Date":"2019-08-01T00:00:00-07:00","TemperatureCelsius":25,"Summary":"Hot"}
```

``` C# : シリアル化の例
using System.Text.Json;

namespace SerializeExtra
{
    public class WeatherForecast
    {
        public DateTimeOffset Date { get; set; }
        public int TemperatureCelsius { get; set; }
        public string? Summary { get; set; }
        public string? SummaryField;
        public IList<DateTimeOffset>? DatesAvailable { get; set; }
        public Dictionary<string, HighLowTemps>? TemperatureRanges { get; set; }
        public string[]? SummaryWords { get; set; }
    }

    public class HighLowTemps
    {
        public int High { get; set; }
        public int Low { get; set; }
    }

    public class Program
    {
        public static void Main()
        {
            var weatherForecast = new WeatherForecast
            {
                Date = DateTime.Parse("2019-08-01"),
                TemperatureCelsius = 25,
                Summary = "Hot",
                SummaryField = "Hot",
                DatesAvailable = new List<DateTimeOffset>() 
                    { DateTime.Parse("2019-08-01"), DateTime.Parse("2019-08-02") },
                TemperatureRanges = new Dictionary<string, HighLowTemps>
                    {
                        ["Cold"] = new HighLowTemps { High = 20, Low = -10 },
                        ["Hot"] = new HighLowTemps { High = 60 , Low = 20 }
                    },
                SummaryWords = new[] { "Cool", "Windy", "Humid" }
            };

            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(weatherForecast, options);

            Console.WriteLine(jsonString);
        }
    }
}
// output:
//{
//  "Date": "2019-08-01T00:00:00-07:00",
//  "TemperatureCelsius": 25,
//  "Summary": "Hot",
//  "DatesAvailable": [
//    "2019-08-01T00:00:00-07:00",
//    "2019-08-02T00:00:00-07:00"
//  ],
//  "TemperatureRanges": {
//    "Cold": {
//      "High": 20,
//      "Low": -10
//    },
//    "Hot": {
//    "High": 60,
//      "Low": 20
//    }
//  },
//  "SummaryWords": [
//    "Cool",
//    "Windy",
//    "Humid"
//  ]
//}
```

## JSON を .NET オブジェクトとして読み取る方法 (逆シリアル化)

JSON を逆シリアル化する一般的な方法は、まず、1 つまたは複数の JSON プロパティを表すプロパティとフィールドを持つクラスを作成することです。 その後、文字列またはファイルから逆シリアル化するには、JsonSerializer.Deserialize メソッドを呼び出します。 ジェネリック オーバーロードの場合は、作成したクラスの型をジェネリック型パラメーターとして渡します。 非ジェネリック オーバーロードの場合は、作成したクラスの型をメソッド パラメーターとして渡します。 同期または非同期のいずれかで逆シリアル化することができます。 クラスで表されていない JSON プロパティはすべて無視されます。

次の例では、JSON 文字列を逆シリアル化する方法を示します。

``` C#
using System.Text.Json;

namespace DeserializeExtra
{
    public class WeatherForecast
    {
        public DateTimeOffset Date { get; set; }
        public int TemperatureCelsius { get; set; }
        public string? Summary { get; set; }
        public string? SummaryField;
        public IList<DateTimeOffset>? DatesAvailable { get; set; }
        public Dictionary<string, HighLowTemps>? TemperatureRanges { get; set; }
        public string[]? SummaryWords { get; set; }
    }

    public class HighLowTemps
    {
        public int High { get; set; }
        public int Low { get; set; }
    }

    public class Program
    {
        public static void Main()
        {
            string jsonString =
@"{
  ""Date"": ""2019-08-01T00:00:00-07:00"",
  ""TemperatureCelsius"": 25,
  ""Summary"": ""Hot"",
  ""DatesAvailable"": [
    ""2019-08-01T00:00:00-07:00"",
    ""2019-08-02T00:00:00-07:00""
  ],
  ""TemperatureRanges"": {
                ""Cold"": {
                    ""High"": 20,
      ""Low"": -10
                },
    ""Hot"": {
                    ""High"": 60,
      ""Low"": 20
    }
            },
  ""SummaryWords"": [
    ""Cool"",
    ""Windy"",
    ""Humid""
  ]
}
";
                
            WeatherForecast? weatherForecast = 
                JsonSerializer.Deserialize<WeatherForecast>(jsonString);

            Console.WriteLine($"Date: {weatherForecast?.Date}");
            Console.WriteLine($"TemperatureCelsius: {weatherForecast?.TemperatureCelsius}");
            Console.WriteLine($"Summary: {weatherForecast?.Summary}");
        }
    }
}
// output:
//Date: 8/1/2019 12:00:00 AM -07:00
//TemperatureCelsius: 25
//Summary: Hot
```

``` C#
using System.Text.Json;

namespace DeserializeFromFile
{
    public class WeatherForecast
    {
        public DateTimeOffset Date { get; set; }
        public int TemperatureCelsius { get; set; }
        public string? Summary { get; set; }
    }

    public class Program
    {
        public static void Main()
        {
            string fileName = "WeatherForecast.json";
            string jsonString = File.ReadAllText(fileName);
            WeatherForecast weatherForecast = JsonSerializer.Deserialize<WeatherForecast>(jsonString)!;

            Console.WriteLine($"Date: {weatherForecast.Date}");
            Console.WriteLine($"TemperatureCelsius: {weatherForecast.TemperatureCelsius}");
            Console.WriteLine($"Summary: {weatherForecast.Summary}");
        }
    }
}
// output:
//Date: 8/1/2019 12:00:00 AM -07:00
//TemperatureCelsius: 25
//Summary: Hot
```

``` C# : 非同期デシリアライズ
using System.Text.Json;

namespace DeserializeFromFileAsync
{
    public class WeatherForecast
    {
        public DateTimeOffset Date { get; set; }
        public int TemperatureCelsius { get; set; }
        public string? Summary { get; set; }
    }

    public class Program
    {
        public static async Task Main()
        {
            string fileName = "WeatherForecast.json";
            using FileStream openStream = File.OpenRead(fileName);
            WeatherForecast? weatherForecast = 
                await JsonSerializer.DeserializeAsync<WeatherForecast>(openStream);

            Console.WriteLine($"Date: {weatherForecast?.Date}");
            Console.WriteLine($"TemperatureCelsius: {weatherForecast?.TemperatureCelsius}");
            Console.WriteLine($"Summary: {weatherForecast?.Summary}");
        }
    }
}
// output:
//Date: 8/1/2019 12:00:00 AM -07:00
//TemperatureCelsius: 25
//Summary: Hot
```

## 書式設定された JSON へのシリアル化

JSON 出力を整形するには、JsonSerializerOptions.WriteIndented を true に設定します。

``` C#

using System.Text.Json;

namespace SerializeWriteIndented
{
    public class WeatherForecast
    {
        public DateTimeOffset Date { get; set; }
        public int TemperatureCelsius { get; set; }
        public string? Summary { get; set; }
    }

    public class Program
    {
        public static void Main()
        {
            var weatherForecast = new WeatherForecast
            {
                Date = DateTime.Parse("2019-08-01"),
                TemperatureCelsius = 25,
                Summary = "Hot"
            };

            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(weatherForecast, options);

            Console.WriteLine(jsonString);
        }
    }
}
// output:
//{
//  "Date": "2019-08-01T00:00:00-07:00",
//  "TemperatureCelsius": 25,
//  "Summary": "Hot"
//}
```

## フィールドを含める

次の例で示されているように、シリアル化または逆シリアル化のときにフィールドを含めるには、JsonSerializerOptions.IncludeFields グローバル設定または JsonSerializerOptions.IncludeFields 属性を使用します。

``` C#
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Fields
{
    public class Forecast
    {
        public DateTime Date;
        public int TemperatureC;
        public string? Summary;
    }

    public class Forecast2
    {
        [JsonInclude]
        public DateTime Date;
        [JsonInclude]
        public int TemperatureC;
        [JsonInclude]
        public string? Summary;
    }

    public class Program
    {
        public static void Main()
        {
            var json =
                @"{""Date"":""2020-09-06T11:31:01.923395"",""TemperatureC"":-1,""Summary"":""Cold""} ";
            Console.WriteLine($"Input JSON: {json}");

            var options = new JsonSerializerOptions
            {
                IncludeFields = true,
            };
            var forecast = JsonSerializer.Deserialize<Forecast>(json, options)!;

            Console.WriteLine($"forecast.Date: {forecast.Date}");
            Console.WriteLine($"forecast.TemperatureC: {forecast.TemperatureC}");
            Console.WriteLine($"forecast.Summary: {forecast.Summary}");

            var roundTrippedJson =
                JsonSerializer.Serialize<Forecast>(forecast, options);

            Console.WriteLine($"Output JSON: {roundTrippedJson}");

            var forecast2 = JsonSerializer.Deserialize<Forecast2>(json)!;

            Console.WriteLine($"forecast2.Date: {forecast2.Date}");
            Console.WriteLine($"forecast2.TemperatureC: {forecast2.TemperatureC}");
            Console.WriteLine($"forecast2.Summary: {forecast2.Summary}");

            roundTrippedJson = JsonSerializer.Serialize<Forecast2>(forecast2);
            
            Console.WriteLine($"Output JSON: {roundTrippedJson}");
        }
    }
}

// Produces output like the following example:
//
//Input JSON: { "Date":"2020-09-06T11:31:01.923395","TemperatureC":-1,"Summary":"Cold"}
//forecast.Date: 9/6/2020 11:31:01 AM
//forecast.TemperatureC: -1
//forecast.Summary: Cold
//Output JSON: { "Date":"2020-09-06T11:31:01.923395","TemperatureC":-1,"Summary":"Cold"}
//forecast2.Date: 9/6/2020 11:31:01 AM
//forecast2.TemperatureC: -1
//forecast2.Summary: Cold
//Output JSON: { "Date":"2020-09-06T11:31:01.923395","TemperatureC":-1,"Summary":"Cold"}
```

## UTF-8 にシリアル化する

UTF-8 バイト配列へのシリアル化は、文字列ベースのメソッドを使用するより約 5 から 10% 高速です。 違いは、バイト (UTF-8) を文字列 (UTF-16) に変換する必要がないことから生じます。

UTF-8 バイト配列にシリアル化するには、JsonSerializer.SerializeToUtf8Bytes メソッドを呼び出します。

``` C#
byte[] jsonUtf8Bytes =JsonSerializer.SerializeToUtf8Bytes(weatherForecast);
```

## UTF-8 からの逆シリアル化

UTF-8 から逆シリアル化するには、次の例に示すように、ReadOnlySpan<byte> または Utf8JsonReader を受け取る JsonSerializer.Deserialize オーバーロードを呼び出します。 この例では、JSON が jsonUtf8Bytes という名前のバイト配列内にあることを想定しています。

```C#
var readOnlySpan = new ReadOnlySpan<byte>(jsonUtf8Bytes);
WeatherForecast deserializedWeatherForecast = 
    JsonSerializer.Deserialize<WeatherForecast>(readOnlySpan)!;
```

```C#
var utf8Reader = new Utf8JsonReader(jsonUtf8Bytes);
WeatherForecast deserializedWeatherForecast = 
    JsonSerializer.Deserialize<WeatherForecast>(ref utf8Reader)!;
```

---

``` C# : 多分実務のコード
var request = new HttpRequestMessage()
{
    Method = HttpMethod.Post,
    RequestUri = new UriBuilder(_Host + _Domain + path).Uri,
    Content = new StringContent(param, Encoding.UTF8, @"application/json")
};

private TResponse Send<TRequest, TResponse>(string url, string xApiKey, TRequest request)
{
    string dataString = Newtonsoft.Json.JsonConvert.SerializeObject(request);
    byte[] dataBytes = Encoding.UTF8.GetBytes(dataString);

    WebRequest webRequest = HttpWebRequest.Create(url);
    webRequest.ContentType = "application/json";
    webRequest.Method = "POST";
    webRequest.ContentLength = dataBytes.Length;
    webRequest.Headers.Add("x-api-key", xApiKey);

    using (Stream reqStream = webRequest.GetRequestStream())
    {
        reqStream.Write(dataBytes, 0, dataBytes.Length);
        reqStream.Close();
    }

    string responseString = new Func<string>(() => 
    {
        using (Stream stream = webRequest.GetResponse().GetResponseStream())
        using (StreamReader streamReader = new StreamReader(stream, Encoding.UTF8))
        return streamReader.ReadToEnd();
    })();
    return Newtonsoft.Json.JsonConvert.DeserializeObject<TResponse>(responseString);
}
```

``` C#
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using static System.Console;

namespace Json1
{
    public static class JsonUtility
    {
        /// <summary>
        /// 任意のオブジェクトをJSON文字列にシリアライズします。
        /// </summary>
        /// <param name="graph"></param>
        /// <returns></returns>
        public static string Serialize(object graph)
        {
            using (var stream = new MemoryStream())
            {
                var serializer = new DataContractJsonSerializer(graph.GetType());
                serializer.WriteObject(stream, graph);
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }
        /// <summary>
        /// JSON文字列を任意のオブジェクトにデシリアライズします。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <returns></returns>
        public static T Deserialize<T>(string message)
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(message)))
            {
                var serializer = new DataContractJsonSerializer(typeof(T));
                return (T)serializer.ReadObject(stream);
            }
        }
    }

    class Sample2
    {
        public void Start()
        {
            //データの作成
            var p_1 = new Person2()
            {
                ID = 0,
                Name = "Taka",
            };
            p_1.Attributes.Add("key1", "value1");
            p_1.Attributes.Add("key2", "value2");
            p_1.Attributes.Add("key3", "value3");

            var p_2 = new Person2()
            {
                ID = 1,
                Name = "PG",
            };
            p_2.Attributes.Add("keyAA", "valueAA");
            p_2.Attributes.Add("keyBB", "valueBB");
            p_2.Attributes.Add("keyCC", "valueCC");
            //リストをシリアライズ
            string json = JsonUtility.Serialize(new List<Person2>() { p_1, p_2 });
            WriteLine(json);
            //デシリアライズ
            var pDeserializeList = JsonUtility.Deserialize<IList<Person2>>(json);
            //内容の出力
            foreach (var p in pDeserializeList)
            {
                WriteLine("ID = " + p.ID);
                WriteLine("Name = " + p.Name);
                foreach (var att in p.Attributes)
                {
                    WriteLine(att.Key + " = " + att.Value);
                }
            }
        }
    }

    [DataContract]
    public class Person2
    {
        [DataMember]
        public int ID { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public IDictionary<string, string> Attributes { get; private set; }

        public Person2()
        {
            this.Attributes = new Dictionary<string, string>();
        }
    }
}
```
