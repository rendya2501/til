# Take,Skip

- Take : 指定した数だけ要素を読み出す  
- Skip : 指定した数だけ読み飛ばす  

リストがあって、入力した人より下の人を対象にしたい場合があった。  
Linqでなんかないか探したらありました。  

最初はTakeしか見つけられなくて、一番上を飛ばすために、Reserse().Take().Reverse()なんてくそ面倒くさいことをする始末だった。  
そこで見つけたSkip。まさに神だったね。  

---

■ **単純なTakeの例**

先頭から2つの要素を取得する。  

``` C#
int[] src = new int[]{1, 2, 3, 4, 5};
foreach(int item in src.Take(2)) {
    Console.WriteLine(item);
}
// →1,2
```

■ **単純なSkipの例**

先頭から2つの要素を読み飛ばす。  
Skipした以降の全要素が取得できる。  

``` cs
int[] src = new int[]{1, 2, 3, 4, 5};
foreach(int item in src.Skip(2)) {
    Console.WriteLine(item);
}
// →3,4,5
```

■ **1つ読み飛ばし、先頭から2つの要素を取得する例**

Skipした後、Takeする。  

``` cs
int[] src = new int[]{1, 2, 3, 4, 5};
foreach(int item in src.Skip(1).Take(2)) {
    Console.WriteLine(item);
}
// →2,3
```

■ **先頭から2つの要素を取得し、1つ読み飛ばす例**

Takeした後、Skipする。  

``` cs
int[] src = new int[]{1, 2, 3, 4, 5};
foreach(int item in src.Take(2).Skip(1)) {
    Console.WriteLine(item);
}
// →2
```

---

[C# LINQ の使い方(Count, Take, Skip, First, Last, Max, Min, Contains, All, Any, Distinct, Sum) - Symfoware](https://symfoware.blog.fc2.com/blog-entry-1927.html)  
