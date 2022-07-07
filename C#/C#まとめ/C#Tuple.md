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

事業者コードと日付だけのパラメーターのためにクラスを用意するのが面倒で、タプルとか匿名型を渡せないか調べた。  
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
