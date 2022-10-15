# タプル

---

## Tuple,ValueTuple

- Tupleは参照型  
- ValueTupleは値型(構造体)  

ValueTupleがある今、Tupleを使う意味はないけれど、知れば知るほどValueTupleとの違いがわからなくなったので、今のうちにまとめておく。  
正直ValueTupleだけ知っていれば十分だ。  
Tupleはまとめるのだるいのでリンクだけ貼っておく。  
いちいち「Tuple.」って付けないといけないからわかりやすいだろう。  

[C# - ValueTuple](https://www.tutorialsteacher.com/csharp/valuetuple)  
[C# - Tuple](https://www.tutorialsteacher.com/csharp/csharp-tuple)  

---

## ValueTuple初期化

``` C# : 要素が全てitemになるタイプの初期化
var person = (1, "Bill", "Gates");

ValueTuple<int, string, string> person = (1, "Bill", "Gates");

(int, string, string) person = (1, "Bill", "Gates");
```

``` C# : 要素名を付けるタイプの初期化
(int Id, string FirstName, string LastName) person = (1, "Bill", "Gates");

var person = (Id:1, FirstName:"Bill", LastName: "Gates");

(int Id, string FirstName, string LastName) person = (PersonId:1, FName:"Bill", LName: "Gates");

(string, string, int) person = (PersonId:1, FName:"Bill", LName: "Gates");

string firstName = "Bill", lastName = "Gates";
var per = (FirstName: firstName, LastName: lastName);
```

---

## Tupleを引数に渡す方法

WebAPIに渡すパラメーターでタプルを使ったのはいいけれど、本当にタプルなのか？バリュータプルと何が違うのか？  
そもそも渡し方と受け取り型ってこれでいいの？ってのがわからなかったのでまとめた。  
だけど、もっとまとめる必要がありそう。まだ全然まとめきれていない。  
とりあえず、下の渡し方は全てOKだった。  
他にもやらないといけないから、今はこれで勘弁して。  

``` C#
    ValueTupleArgumentTest(("a", 1));
    ValueTupleArgumentTest((str: "b", i: 2));
    ValueTupleArgumentTest(new ValueTuple<string, int>("c", 3));
    ValueTupleArgumentTest(ValueTuple.Create("d", 4));
    // これはダメ
    // ValueTupleArgumentTest(Tuple.Create("d", 4));

    // 引数はValueTuple<string,int>型
    private static void ValueTupleArgumentTest((string str, int i) key)
    {
        Console.WriteLine(key.str);
        Console.WriteLine(key.i);
    }
```

---

## ASP.NetのWeb APIのパラメーターでタプルを渡す

コードと日付だけのパラメーターのためにクラスを用意するのが面倒で、タプルとか匿名型を渡せないか調べた。  
匿名型は無理だったが、タプルは行けたのでまとめる。  
いいかどうかはしらん。  

``` C#
    /// <summary>
    /// フロント側
    /// </summary>
    public async Task<HogeClass> GetHogeClass(string code, DateTime businessDate)
    {
        return await _Accessor.GetResultAsync<HogeClass>(PATH + "/get/basic_info", (code, businessDate));
    }

    /// <summary>
    /// API側
    /// </summary>
    [HttpPost("get/basic_info")]
    public HogeClass GetHogeClass([FromBody] (string code, DateTime businessDate) key)
    {
        return _HogeModel_.GetHogeClass(key.code, key.businessDate);
    }
```

---

## ValueTupleのnull判定

ValueTupleは構造体なのでnullにならない。  
この状態においてnullを判定するためにはdefault句を使う。  
ValueTupleはnull許容型にできるので、その場合はnullとして判定できるが、要素にアクセスするために毎回`tuple.value.要素`としなければならない。  

因みにValueTupleの初期状態は各メンバーの初期値が入る。  
int,stringなら0とnullがdefaultとなるので、それをdefault句で判定する感じだろか。  

``` C#
    var tupleList = new List<(int a, int b, int c)>()
    {
        (1, 1, 2),
        (1, 2, 3),
        (2, 2, 4)
    };
    var result = tupleList.FirstOrDefault(f => f.a == 4);

    // タプルのnull判定は Equalsメソッド + default
    if (result.Equals(default(ValueTuple<int,int,int>)))
    {
        Console.WriteLine("Missing!"); 
    }

    // default構文は省略可能(.NetFramework4.8でも可能であることを確認)
    if (result.Equals(default()))
    {
        Console.WriteLine("Missing!"); 
    }
```

``` C# : null許容型
    // null許容型が可能
    var tupleList = new List<(int a, int b, int c)?>()
    {
        (1, 1, 2),
        (1, 2, 3),
        (2, 2, 4)
    };
    // null許容型にした場合 ?. のnull合体演算子でなければエラーになる
    // この時の結果はnullとなる。
    var result = tupleList.FirstOrDefault(f => f?.a == 4);

    // nullなので、普通に判定可能
    if (result == null)
    {
        Console.WriteLine("Missing!");
    }
    // こちらでも判定可能。null.Equalsに相当するものだと思われるがエラーにならない。謎。
    if (result.Equals(default))
    {
        Console.WriteLine("Missing!");
    }

    ただし、null許容型にした場合、要素にアクセスするときは
```

<https://twitter.com/kyubuns/status/1379265780240457729>  
[How to null check c# 7 tuple in LINQ query?](https://stackoverflow.com/questions/44307657/how-to-null-check-c-sharp-7-tuple-in-linq-query)  
