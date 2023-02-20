# 匿名型

---

## 匿名型（匿名クラス）

`var mydata1 = new { id = 100, name = "Penguin" }`の形で作る匿名型は、`System.Dynamic.ExpandoObject`クラスに該当する。  
変数に直接代入せず、動的に匿名型を生成するためには、`System.Dynamic.ExpandoObject`クラスを使用する必要がある。  

``` cs
dynamic employee = new ExpandoObject();
employee.Name = "John Smith";
Console.WriteLine(employee.Name);
// John Smith
```

---

## リフレクション

実際のプロパティではないため、Reflectionでアクセスできない。  
以下の場合はnullとなる。  

``` cs
dynamic employee = new ExpandoObject();
employee.Name = "John Smith";
Console.WriteLine(employee.GetType().GetProperty("Name") ?? "nullです。");
// nullです。
```

---

## インテリセンス

通常の匿名型はインテリセンスが効くが、`ExpandoObject`から生成した場合はインテリセンスは利かない。  

``` cs
var test = new { key1 = "キー1", key2 = "キー2" };
// 「test.」と入力したら 「key1」か「key2」の候補が出てくる。
Console.WriteLine(test.key1);  // キー1
Console.WriteLine(test.key2);  // キー2
```

``` cs
dynamic employee = new ExpandoObject();
employee.Name = "John Smith";
// 「employee.」と入力しても何も候補が表示されないが、しっかり中身は入っている。
Console.WriteLine(employee.Name); // John Smith
```

---

## 匿名クラスの動的生成の基本

`IDictionary<string, object>`に格納することで実現可能。  
`dic`にaddしているが、使用するのは`expando`変数であることが分かる。  

``` cs
dynamic expando = new ExpandoObject();
IDictionary<string, object> dic = (IDictionary<string, object>)expando;

dic.Add("FirstName", "Bob");
dic.Add("LastName", "Smith");
Console.WriteLine(expando.FirstName + " " + expando.LastName);
// Bob Smith
```

`IDictionary<string,object>`型にnewして直接代入でも問題ない。  
ただ、使用の際は`dynamic`へのキャストが必要となる。  

``` C#
IDictionary<string,object> expando = new ExpandoObject();
expando["Name"] = "Hoge";

// expandoをdynamicに変換する
dynamic d = expando;
Console.WriteLine(d.Name); // Hoge

// 詰まる所、やっていることはこれ
Console.WriteLine(((dynamic)expando).Name); // Hoge
```

---

## 匿名クラスの動的生成の実践例

``` cs
// 動的に作成するプロパティ、値の組み合わせをDictionaryに登録
var dic = new Dictionary<string, string>
{
    { "key1", "キー1" },
    { "key2", "キー2" }
};

// IDictionary宣言
IDictionary<string, object> expando = new ExpandoObject();

// IDictionaryにAdd
foreach (var item in dic)
{
    expando.Add(item.Key, item.Value);
}

// 使用する場合はdynamicに変換
dynamic d = expando;
Console.WriteLine(d.key1);
Console.WriteLine(d.key2);
```

毎回`(IDictionary<string, object>)`にキャストする事でもAddできるがオススメはしない。  
あくまでこういう事もできるよっていう紹介。  

``` cs
var dic = new Dictionary<string, string>
{
    { "key1", "キー1" },
    { "key2", "キー2" }
};

// 匿名型宣言
dynamic anonymousType = new ExpandoObject();

// 匿名型にAdd
foreach (var item in dic)
{
    ((IDictionary<string, object>)anonymousType).Add(item.Key, item.Value);
}
((IDictionary<string, object>)anonymousType).Add("Key3", "キー3");

// 表示
Console.WriteLine(anonymousType.key1);  // キー1
Console.WriteLine(anonymousType.key2);  // キー2
Console.WriteLine(anonymousType.key3);  // キー3
```

---

こういう芸当もできる。  
Dapperに渡す条件を生成するときに使えるだろう。  

``` C#
Student student = new Student()
{
    StudentId = "1",
    StudentName = "Cnillincy"
};

// 匿名型定義
IDictionary<string, object> anonymousType = new ExpandoObject();

// 匿名型に追加
foreach (var (key, value) in student
    .GetType()
    .GetProperties(BindingFlags.Instance | BindingFlags.Public))
{
    anonymousType.Add(s.Name,  s.GetValue(condition));
}
// dynamicに変換
dynamic dynamic = anonymousType;

// アクセス
Console.WriteLine(dynamic.key1);  // キー1
Console.WriteLine(dynamic.key2);  // キー2
```

---

## Linqで動的生成

linqでは実現不能。  
newしたインスタンスに対して愚直にaddするしかない模様。  

動的生成はExpandoObjectをnewしたインスタンスに対してaddすることで追加するので、Linqによるyield returnは機能しない。  

``` cs : ダメなやつ
dynamic d = (IDictionary<string, object>)dic.ToDictionary(s => s.Key, aa => (object)aa.Value);

// これはダメ
Console.WriteLine(d.key1);
Console.WriteLine(d.key2);

// これはいける。
// Linqで返されるのは、結局Dictionary型でしかないという事だろう。  
Console.WriteLine(d["key1"]);
Console.WriteLine(d["key2"]);
```

どうしても1行で済ませたいならFuncデリゲートを使うしかない。  

``` cs
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

---

## 参考

[匿名型_ipentec](https://www.ipentec.com/document/csharp-using-anonymous-type)  

[.NET Core (C#) 匿名型（匿名クラス）を動的に作成する](https://marock.tokyo/2021/01/24/net-core-%E5%8C%BF%E5%90%8D%E5%9E%8B%EF%BC%88%E5%8C%BF%E5%90%8D%E3%82%AF%E3%83%A9%E3%82%B9%EF%BC%89%E3%82%92%E5%8B%95%E7%9A%84%E3%81%AB%E4%BD%9C%E6%88%90%E3%81%99%E3%82%8B/)  

[匿名型の動的生成に関して](https://dobon.net/vb/bbs/log3-54/31793.html)  

[匿名型を動的に作成しますか？](https://www.web-dev-qa-db-ja.com/ja/c%23/%E5%8C%BF%E5%90%8D%E5%9E%8B%E3%82%92%E5%8B%95%E7%9A%84%E3%81%AB%E4%BD%9C%E6%88%90%E3%81%97%E3%81%BE%E3%81%99%E3%81%8B%EF%BC%9F/970777402/)  

[新しい匿名クラスを動的にするには？](https://www.web-dev-qa-db-ja.com/ja/c%23/%E6%96%B0%E3%81%97%E3%81%84%E5%8C%BF%E5%90%8D%E3%82%AF%E3%83%A9%E3%82%B9%E3%82%92%E5%8B%95%E7%9A%84%E3%81%AB%E3%81%99%E3%82%8B%E3%81%AB%E3%81%AF%EF%BC%9F/970949625/)  
