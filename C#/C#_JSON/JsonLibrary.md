# Jsonライブラリ

- NewtonJson  
- System.Text.Json  
- DataContractJsonSerializer

---

## NewtonJson

MITライセンスで公開されている.NET用のオープンソースJSONライブラリ  

``` C# : NewtonJson
using Newtonsoft.Json;

// シリアライズ
string json = JsonConvert.SerializeObject(account, Formatting.Indented);

// デシリアライズ
Person person = JsonConvert.DeserializeObject<Person>(jsonData);
// デシリアライズパターン2 使うことはないだろう
var aa = (Account?)new JsonSerializer().Deserialize(file, typeof(Account));
```

[Newtonsoft.Jsonライブラリの使用方法](https://blog.hiros-dot.net/?p=8766#toc20)  

---

## System.Text.Json

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

---

## DataContractJsonSerializer

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

[[C#] C#でJSONを扱う方法まとめ](https://dev.classmethod.jp/articles/c-sharp-json/)  

---

## どれを使えばいいのか？

|ライブラリ|ユースケース|
|---|---|
|NewtonJson|高機能な検索やパフォーマンスを出したい時|
|System.Text.Json|諸事情によりサードパーティライブラリを使えない時|
|DataContractJsonSerializer|諸事情によりサードパーティライブラリを使えない時だが、わざわざ採用する理由はない|

[[C#] C#でJSONを扱う方法まとめ | DevelopersIO](https://dev.classmethod.jp/articles/c-sharp-json/)  
[【C#】JSONのシリアライザは、System.Text.JSONを使おう。](https://qiita.com/SY81517/items/1cf6246dd99869f7b9c5)  

---

## 各ライブラリにおけるシリアライズ・デシリアライズの書式

>VB/C#でJSONを読み書きするには、JSON.NET(Newtonsoft JSON)またはSystem.Text.Jsonを使用するのが一般的です。  
>JSON.NETは多機能で使われる頻度が高く2020年3月現在ではデファクトスタンダードです。  
>System.Text.Jsonは新しくMicrosoftが開発したライブラリで機能は少なめですがパフォーマンスが優れており、Webアプリケーションのスループットを向上させることができるようです。  
>[VB/C#でJSONの読み込み (System.Text.Json編)](https://www.umayadia.com/Note/Note010VBSystem.Text.Json.htm)  
