# 匿名型

---

## 匿名型（匿名クラス）を動的に作成する

[.NET Core (C#) 匿名型（匿名クラス）を動的に作成する](https://marock.tokyo/2021/01/24/net-core-%E5%8C%BF%E5%90%8D%E5%9E%8B%EF%BC%88%E5%8C%BF%E5%90%8D%E3%82%AF%E3%83%A9%E3%82%B9%EF%BC%89%E3%82%92%E5%8B%95%E7%9A%84%E3%81%AB%E4%BD%9C%E6%88%90%E3%81%99%E3%82%8B/)  
[匿名型の動的生成に関して](https://dobon.net/vb/bbs/log3-54/31793.html)  
[匿名型を動的に作成しますか？](https://www.web-dev-qa-db-ja.com/ja/c%23/%E5%8C%BF%E5%90%8D%E5%9E%8B%E3%82%92%E5%8B%95%E7%9A%84%E3%81%AB%E4%BD%9C%E6%88%90%E3%81%97%E3%81%BE%E3%81%99%E3%81%8B%EF%BC%9F/970777402/)  
[新しい匿名クラスを動的にするには？](https://www.web-dev-qa-db-ja.com/ja/c%23/%E6%96%B0%E3%81%97%E3%81%84%E5%8C%BF%E5%90%8D%E3%82%AF%E3%83%A9%E3%82%B9%E3%82%92%E5%8B%95%E7%9A%84%E3%81%AB%E3%81%99%E3%82%8B%E3%81%AB%E3%81%AF%EF%BC%9F/970949625/)  

匿名型（匿名クラス）を動的に作成するには、System.DynamicのExpandoObjectクラスを使用する必要がある。  
newしたインスタンスに対して愚直にaddするしかない模様。  
というか、思った以上に深い内容だった。  

``` C#
    // 動的に作成するプロパティ、値の組み合わせをDictionaryに登録
    var dic = new Dictionary<string, string>
    {
        { "key1", "キー1" },
        { "key2", "キー2" }
    };
```

``` C# : パターン1
    // 匿名型をDictionaryから作成
    dynamic anonymousType = new ExpandoObject();
    foreach (var item in dic)
    {
        ((IDictionary<string, object>)anonymousType).Add(item.Key, item.Value);
    }
    ((IDictionary<string, object>)anonymousType).Add("Key3", "キー3");
    // 動的生成はdynamicなのでインテリセンスは効かない
    Console.WriteLine(anonymousType.key1);  // キー1
    Console.WriteLine(anonymousType.key2);  // キー2
    Console.WriteLine(anonymousType.key3);  // キー3

    // 通常の匿名型はインテリセンスが効く
    var test = new { key1 = "キー1", key2 = "キー2" };
    Console.WriteLine(test.key1);  // キー1
    Console.WriteLine(test.key2);  // キー2
```

``` C# : パターン2
    IDictionary<string, object> expando = new ExpandoObject();
    foreach (var item in dic)
    {
        expando[item.Key] = item.Value;
    }
    dynamic d = expando;
    Console.WriteLine(d.key1);
    Console.WriteLine(d.key2);
```

``` C# : パターン3
    IDictionary<string, object> expando = new ExpandoObject();
    foreach (var item in dic)
    {
        expando.Add(item.Key, item.Value);
    }
    dynamic d = expando;
    Console.WriteLine(d.key1);
    Console.WriteLine(d.key2);
```

``` C#
    dynamic expando = new ExpandoObject();
    IDictionary<string, object> dictionary = (IDictionary<string, object>)expando;
    dictionary.Add("FirstName", "Bob");
    dictionary.Add("LastName", "Smith");
    Console.WriteLine(expando.FirstName + " " + expando.LastName);
```

直接プロパティを指定することで生成することも可能な模様。  

``` C#
dynamic employee = new ExpandoObject();
employee.Name = "John Smith";
employee.Age = 33;
foreach (var property in (IDictionary<string, object>)employee)
{
    Console.WriteLine(property.Key + ": " + property.Value);
}
// Name: John Smith
// Age: 33
```

次のようなExpandoObjectを作成できます。

``` C#
IDictionary<string,object> expando = new ExpandoObject();
expando["Name"] = value;
```

そして、動的にキャストした後、これらの値はプロパティのようになります。

``` C#
dynamic d = expando;
Console.WriteLine(d.Name);
```

ただし、これらは実際のプロパティではなく、Reflectionを使用してアクセスすることはできません。  
したがって、次のステートメントはnullを返します。  

``` C#
d.GetType().GetProperty("Name") 
```

どうしても1行で済ませたいならFuncデリゲートを使うしかない。  

``` C#
 dynamic d = new Func<IDictionary<string, object>>(() =>
    {
        IDictionary<string, object> expando = new ExpandoObject();
        foreach (var item in dic)
        {
            expando.Add(item.Key, item.Value);
        }
        return expando;
    }).Invoke();
    Console.WriteLine(d.key1);
    Console.WriteLine(d.key2);
```

こういう芸当もできる。  
Dapperに渡す条件を生成するときに使えるだろう。  

``` C#
    Student student = new Student()
    {
        StudentId = "1",
        StudentName = "Cnillincy"
    };
    IDictionary<string, object> anonymousType = new ExpandoObject();
    foreach (var (key, value) in student
        .GetType()
        .GetProperties(BindingFlags.Instance | BindingFlags.Public))
    {
        anonymousType.Add(s.Name,  s.GetValue(condition));
    }
    dynamic dynamic = anonymousType;
    Console.WriteLine(dynamic.key1);  // キー1
    Console.WriteLine(dynamic.key2);  // キー2
```

動的生成は前提としてExpandoObjectをnewしたインスタンスに対して愚直にaddしないといけないからLinqで都度returnは機能しない。  
だから絶対にlinqでは実現不能だ。  

``` C# : ダメなやつ
    dynamic d = (IDictionary<string, object>)dic.ToDictionary(s => s.Key, aa => (object)aa.Value);
    // これはダメ
    Console.WriteLine(d.key1);
    Console.WriteLine(d.key2);
    // これはいける。
    // Linqで返されるのは、結局Dictionary型でしかないという事だろう。  
    Console.WriteLine(d["key1"]);
    Console.WriteLine(d["key2"]);
```

---

## Dapperと匿名型の動的生成を用いた例

日付でBETWEENしたい条件が複数ある場合とか使えるかもしれん。  
まぁ、そんなことするくらいなら文字列補完で直接クエリにぶち込めって思うけどね。  

``` C#
    Condition student = new Condition()
    {
        Code = 1,
        DateList = new List<(DateTime from, DateTime to)>() {
            (new DateTime(2022, 3, 20),new DateTime(2022, 3, 22)),
            (new DateTime(2022, 5, 50),new DateTime(2022, 6, 20))
        }
    };

private SampleData GetSample(Condition condition)
{
    IDictionary<string, object> anonymousType = new ExpandoObject();
    foreach (var s in condition
        .GetType()
        .GetProperties(BindingFlags.Instance | BindingFlags.Public))
    {
        anonymousType.Add(s.Name,  s.GetValue(condition));
    }

    StringBuilder selectQuery = new StringBuilder()
        .AppendLine("SELECT ")
        .AppendLine(" ~~~ ")
        .AppendLine("FROM [Table]")
        .AppendLine("WHERE [Code] = Code");
    
    StringBuilder whereQuery = new StringBuilder();
    foreach(var item in condition.ExclusionDateList.Select((v,i) => (v,i))) {
        whereQuery.AppendLine($"AND Date BETWEEN @from{i} AND @to{i}");
        anonymousType.Add($"from{i}", v.from);
        anonymousType.Add($"to{i}", v.to);
    }

    return _DapperAction.GetFirstDataByQuery<SampleData>(
        ConnectionTypes.Data,
        selectQuery.AppendLine(whereQuery).ToString(),
        (dynamic)anonymousType
    );
}
```
