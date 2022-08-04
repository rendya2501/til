# OrderBy,ThenBy

OrderByの後にOrderByすると、さっきまでソートした結果を無視して新しくソートしなおしてしまう。  
だからThenByでどんどんソートした結果を保持したままソートする。  

まとめ  

- OrderByした結果に対してThenByを行う。  
- ThenByした結果にOrderByをすると、今までの結果は考慮されず一からソートし直す。  

- 匿名型は指定不可。  
- タプル、バリュータプルは指定可能。  
- タプルを指定した場合、Orderby,ThenByの順にソートしたのと同じ動作になる模様。  

- タプルでも、内部の型が複数になるとエラーになる。  
- OrderBy中でタプルは.NetFramework4.8(C# 7.3)でも行けた  

---

## OrderByの要素を切り替えたい

`list.ThenBy(o => true ? o.ID : o.name);`この構文がエラーになるのだが、分からないかと聞かれたのでまとめ。  
よく考えたら、OrderByって複数のキーを指定できたっけ？みたいなこともあいまいだったので復習も兼ねる。  

[型引数を使い方から推論することはできません。型引数を明示的に指定してください。]
ということらしいので、コーディング段階で型推論できないという事であれば、dynamicにして実行時に対応するようにすればいけるのではということでやったらできた。  
dynamicにキャストするだけで済む話だったが、なんでこんなことがどこにも書いてないのだろうか。  

### 完成形

``` C# : 完成形
    var list= new List<(int id, string name)>
    {
        (1, "E"),
        (2, "D"),
        (3, "C"),
        (4, "B"),
        (5, "A"),
    };
    // 完成形
    var type1 = list.OrderBy(o => false ? (dynamic)o.id : o.name).ToList();
    // タプルでも可能
    var type2 = list.OrderBy(o => false ? (dynamic)(o.id, o.name) : (o.name, o.id)).ToList();
```

### 完成形に至るまでの過程

``` C# : 完成形に至るまでの過程
    // 過程1
    Func<(int id, string name), dynamic> sortIdFunc = (p) => { return p.id; };
    Func<(int id, string name), dynamic> sortNameFunc = (p) => { return p.name; };
    var process1 = list.OrderBy(false ? sortIdFunc : sortNameFunc).ToList();
    // https://10.hateblo.jp/entry/2014/05/30/154157
    // このサイトで下の案はいけるってことなので、ならなんでもOKなdynamicにしてそもそもソートされるのか試したらできた。
    // Func<Person,int> f=(p)=>{return p.id;}
    // Func<Person,string> f2=(p)=>{return p.Name;}
    // query = query.OrderBy(model.Order==0?f:f2);

    // 過程2
    // dynamicでいけるなら、後は単純に三項演算子の形に落とし込んでいくだけ。これも問題なかった。
    // 最終的に完成形となる。
    Func<(int id, string name), dynamic> sortFunc = p => (false ? (dynamic)p.id : p.name);
    var process2 = list.OrderBy(sortFunc).ToList();

```

[OrderByのセレクタを外出ししたい](https://10.hateblo.jp/entry/2014/05/30/154157)  
[Create an OrderBy Expression for LINQ/Lambda](https://stackoverflow.com/questions/5766247/create-an-orderby-expression-for-linq-lambda)  

同じ要望を実現したいと考えている人で、一番参考になった。  
紹介されている`Expression<Func<Person,dynamic>>`を素直に実装してみたが、これではダメだった。  
しかし、ここまで近しい資料は他になかったので、この考え方をベースにいろいろやったらいけたという感じである。  

``` C#
    // これがダサい。
    if (model.OrderDirection==0){
        query = model.Order==0? query.OrderBy(p=>p.Name) : query.OrderBy(p=>p.id);
    }else{
        query = model.Order==0? query.OrderByDescending(p=>p.Name) : query.OrderByDescending(p=>p.id);
    }

    // これが通れば、もっといろいろできるはずだったが、受け取る型がどうのこうのというエラーになってしまった。
    Expression<Func<Person,dynamic>> f;
    if (model.Order==0){
        f = p=> new{p.id};
    }else{ 
        f = p=> new{p.Name};
    }
    query = query.OrderBy(f);
```

[Lambda Expressions Are Wonderful](https://www.c-sharpcorner.com/UploadFile/afenster/lambda-expressions-are-wonderful/)  

クラスのメソッドとして定義する案っぽいけど、正直微妙だ。  
やることはないだろう。  

``` C#
    List<Employee> employees = new List<Employee>();
    new Employee().Sort(ref employees, e => e.LastName, SortOrder.Ascending);

    public class Employee
    {
        public string FirstName { set; get; }
        public string LastName { set; get; }
        public decimal Salary { set; get; }
        public bool IsManager { set; get; }

        public void Sort<TKey>(
             ref List<Employee> list,
             Func<Employee, TKey> sortKey,
             SortOrder sortOrder = SortOrder.Ascending)
        {
            if (sortOrder == SortOrder.Ascending)
                list = list.OrderBy(sortKey).ToList();
            else
                list = list.OrderByDescending(sortKey).ToList();
        }
    }
    public enum SortOrder { Ascending, Decending }
```

---

## OrderByでタプルや匿名型の動作検証

``` C#
    var list4 = new List<(int id, string name)>
    {
        (1, "A"),
        (2, "B"),
        (3, "C"),
        (4, "D"),
        (5, "E"),
    };
    
    // IDがint,nameがstringの場合、以下のコードはコーディングの段階でエラーになる。
    // 実行する時にならないと型が確定しないからだと思われる。
    // CS0411
    _ = list4.OrderBy(o => true ? o.ID : o.name);
    // これもダメ
    _ = list4.OrderBy(o => true ? (o.id, o.name) : (o.name, o.id));
    // 当然ながら匿名型はだめ。
    var aaa = list4.OrderBy(o => new { o.name, o.id } ).ToList();

    // 素直にOrderBy,ThenByを使う必要がある。
    list4.OrderBy(o => o.id ).ThenBy(o => o.name);
    list4.OrderBy(o => o.name).ThenBy(o => o.id);
```

``` C#
    var list = new List<(int Data1, int Data2)>
    {
        (11, 20),
        (12, 20),
        (10, 22),
        (10, 21),
        (10, 20),
    };

    // 匿名型は不可
    // コーディング段階でエラーにならないが実行するとエラーになる。
    // System.InvalidOperationException: 'Failed to compare two elements in the array.'
    var aa = order_list3.OrderBy(o => new { o.Data1, o.Data2 });
    
    // タプルはいける
    // data1=10, data2=20
    // data1=10, data2=21
    // data1=10, data2=22
    // data1=11, data2=20
    // data1=12, data2=20
    var a2 = order_list3.OrderBy(o => (o.Data1, o.Data2));
    var order_list3 = list.OrderBy(d => d.Data1).ThenBy(d => d.Data2);

    // data1=10, data2=20
    // data1=11, data2=20
    // data1=12, data2=20
    // data1=10, data2=21
    // data1=10, data2=22
    var a2 = order_list3.OrderBy(o => (o.Data2, o.Data1));
    var order_list33 = list.OrderBy(d => d.Data2).ThenBy(d => d.Data1);

    // タプル内の型が全て同じ場合、こういう芸当も可能
    _ = list
        .OrderBy(o => boolean ? (o.Data1, o.Data2) : (o.Data2, o.Data1))
```

``` C#
    var list2 = new List<(int Large, int Middle, int small)>
    {
        (1, 1, 1),(1, 1, 2),(1, 2, 3),(1, 2, 4),(1, 3, 5),(1, 3, 6),
        (2, 1, 7),(2, 1, 8),(2, 2, 9),(2, 2, 10),(2, 3, 11),(2, 3, 12),
        (3, 1, 13),(3, 1, 14),(3, 2, 15),(3, 2, 16),(3, 3, 17),(3, 3, 18),
    };

    // タプルの中でさらに入れ子のタプルを定義し、三項演算子によって分岐させることも可能。  
    var a2222 = list2
        .OrderBy(o => (o.Middle, true ? (o.Large, o.small) : (o.small, o.Large)))
        .ToList();
    // middle → large → small
    // [0]: (1, 1, 1)
    // [1]: (1, 1, 2)
    // [2]: (2, 1, 7)
    // [3]: (2, 1, 8)
    // [4]: (3, 1, 13)
    // [5]: (3, 1, 14)
    // [6]: (1, 2, 3)
    // [7]: (1, 2, 4)
    // [8]: (2, 2, 9)
    // [9]: (2, 2, 10)
    // [10]: (3, 2, 15)
    // [11]: (3, 2, 16)
    // [12]: (1, 3, 5)
    // [13]: (1, 3, 6)
    // [14]: (2, 3, 11)
    // [15]: (2, 3, 12)
    // [16]: (3, 3, 17)
    // [17]: (3, 3, 18)
    
    // Middle → small → Large
    // [0]: (1, 1, 1)
    // [1]: (1, 1, 2)
    // [2]: (2, 1, 7)
    // [3]: (2, 1, 8)
    // [4]: (3, 1, 13)
    // [5]: (3, 1, 14)
    // [6]: (1, 2, 3)
    // [7]: (1, 2, 4)
    // [8]: (2, 2, 9)
    // [9]: (2, 2, 10)
    // [10]: (3, 2, 15)
    // [11]: (3, 2, 16)
    // [12]: (1, 3, 5)
    // [13]: (1, 3, 6)
    // [14]: (2, 3, 11)
    // [15]: (2, 3, 12)
    // [16]: (3, 3, 17)
    // [17]: (3, 3, 18)
```

---

## 特定の値を先頭にして、後はそのままの順番にするやり方

[C# LINQで特定の値を先頭にして並び替え](https://teratail.com/questions/120228)  

100,546を割り勘→100をチェックアウトで呼び出す→割り勘起動→546も引っ張られて表示される。  
この時、もう一つ割り勘を開いて、546と入力すると、割り勘で排他を取っているはずなのに、「チェックアウトで精算中です」って言われてしまう。  
原因は100,546の順で排他を取るので、100はチェックアウトで排他中なので、546と打ってもそうなってしまうから。  
というわけで、排他を取る順番を指定した会計Noを先頭にして、後はそのままってやりたかったわけです。  
そしたらドンピシャなモノがありました。  
おかげで無事解決しました。  

``` C#
var result = Enumerable
    // 10,11,12,13,14
    .Range(10, 5)
    // A,B,C,D,E
    .Select(a => a.ToString("X"))
    // C,A,B,D,E
    .OrderBy(a => a == "C" ? 0 : 1);

    // サンプルではThenByがあるが、なくても想定した動作になる。
    // .ThenBy(a => a);

    // ThenBy(a => a)と書くと、場合によっては「failed to compare two elements in the array」というエラーになってしまう。
    // なくてもうまく行くならなくてもいいかも。
    // 実際に使ったときは順番に影響もなかった。
```
