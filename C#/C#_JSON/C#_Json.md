# C# Json関連まとめ

C#に置けるjsonの表現方法はクラスそのものか、テキストによる直接の表現か。  
基本的に、デシリアライズすることによってクラスをjsonに変換するのが主な用途で、受け皿はすべてクラス。  
なので、クラスを挟んでシリアライズ、デシリアライズすればjsonは扱える。  

---

## appsettings.json 有無 確認

appsettings.json existance check  

[How to check if Configuration Section exists in .NET Core?](https://stackoverflow.com/questions/44641488/how-to-check-if-configuration-section-exists-in-net-core)  

---

## C#のJSONの取り扱いの歴史

- System.Runtime.Serialization.Json.DataContractJsonSerializer  
- 長く使われてきたサードパーティ製ライブラリのJson.NET (NuGetで取り込むときのライブラリ名はNewtonsoft.Json)  
- C#4.0で導入されたライブラリのDynamic.Json  

ググると上記が入り混じってヒットするかと思います。特にJson.NETが広く使われたのでよくヒットしますね。  

2015/11リリースの .NET Framework 4.6.1 からSystem.Text.Jsonが導入された。  
パッケージ管理のNuGetでインストールすると使えるようになり、実行環境自体が刷新された .NET Core では2017/8リリースの .NET Core 2.0 から標準搭載。  
2021年現在はこの [System.Text.Json] が**デフォルトで推奨**。  

[System.Text.Json でJSONを扱ってみよう](https://iwasiman.hatenablog.com/entry/20210614-CSharp-json)  

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

[C# を使用して JSON をシリアル化および逆シリアル化する方法 - .NET | Microsoft Learn](https://docs.microsoft.com/ja-jp/dotnet/standard/serialization/system-text-json-how-to?pivots=dotnet-6-0)

[C#(.NET)でJsonの操作はJsonSerializerを使いましょう | DevelopersIO](https://dev.classmethod.jp/articles/csharp-dotnet-lets-use-jsonserializer/)  

[C#でJSONを読み書きする方法](https://usefuledge.com/csharp-json.html)  

[JSONの読み込み・デシリアライズを分かりやすく解説【C# Json.NET】](https://tech-and-investment.com/json2/)  

[JSONファイルの作成方法を分かりやすく解説【C# Json.NET】](https://tech-and-investment.com/json3/)  

[【.NET Core】ConfigurationBuilderを使ってC#で設定ファイルや環境変数を扱う | cloud.config Tech Blog](https://tech-blog.cloud-config.jp/2019-7-11-how-to-configuration-builder/)  
