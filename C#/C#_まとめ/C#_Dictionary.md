# C#_Dictionary

---

## 初期化方法

### 1. コレクションイニシャライザを使用する方法

``` cs
var dic = new Dictionary<string, int> {
    { "a", 1 },
    { "b", 2 }
};
```

### 2. オブジェクトイニシャライザを使用する方法

``` cs
var dic = new Dictionary<string, int> { 
    ["a"] = 1,
    ["b"] = 2
};
```

---

## DictionaryはRemoveしてAddすると順番が保証されない

addし続けるなら問題ないが、途中でRemoveすると順番が保証されない模様。  
OrderByすればソートされるけど、文字列の場合は文字列順にソートされてしまうので、純粋に追加した順番にはなってくれない。  

そこで考えたのが`List<KeyValuePair<K,V>>`  
Listは順番を保証してくれる。  
事実、途中でRemoveしてAddしても追加した順番になっている。  
問題点は要素の追加が面倒くさいことだろうか。  

``` C#
var dic1 = new Dictionary<string, string>()
{
    {"a","A" },
    {"b","B" },
    {"c","C" },
    {"d","D" },
};
foreach (var item in dic1)
{
    Console.WriteLine($"{item.Key},{item.Value}");
}
Console.WriteLine("---");
dic1.Remove("b");
dic1.Remove("c");
dic1.Add("e", "E");
dic1.Add("a1", "A1");
foreach (var item in dic1)
{
    Console.WriteLine($"{item.Key},{item.Value}");
}

// a,A
// b,B
// c,C
// d,D
// ---
// a,A
// a1,A1
// e,E
// d,D
```

``` cs
var dic2 = new List<KeyValuePair<string, string>>()
{
    new KeyValuePair<string, string>("a","A"),
    new KeyValuePair<string, string>("b","B"),
    new KeyValuePair<string, string>("c","C"),
    new KeyValuePair<string, string>("d","D"),
};
foreach (var item in dic2)
{
    Console.WriteLine($"{item.Key},{item.Value}");
}
Console.WriteLine("---");
dic2.RemoveAt(2);
dic2.RemoveAt(2);
dic2.Add(new KeyValuePair<string, string>("e", "E"));
dic2.Add(new KeyValuePair<string, string>("a1", "A1"));
foreach (var item in dic2)
{
    Console.WriteLine($"{item.Key},{item.Value}");
}

// a,A
// b,B
// c,C
// d,D
// ---
// a,A
// b,B
// e,E
// a1,A1
```

[C# Dictionary add entry with wrong order](https://stackoverflow.com/questions/24223344/c-sharp-dictionary-add-entry-with-wrong-order)  

---

[Dictionaryの拡張メソッド 36選](https://qiita.com/soi/items/6ce0e0ddefdd062c026a)  
