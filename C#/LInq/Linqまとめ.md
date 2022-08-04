# Linqのあれこれ

---

## 特定の月のすべての日付を取得する方法

<https://www.it-mure.jp.net/ja/c%23/c%EF%BC%83%E3%81%A7%E7%89%B9%E5%AE%9A%E3%81%AE%E6%9C%88%E3%81%AE%E3%81%99%E3%81%B9%E3%81%A6%E3%81%AE%E6%97%A5%E4%BB%98%E3%82%92%E5%8F%96%E5%BE%97%E3%81%99%E3%82%8B%E6%96%B9%E6%B3%95/971126391/>

``` C#
public static List<DateTime> GetDates(int year, int month)
{
   return Enumerable.Range(1, DateTime.DaysInMonth(year, month))  // Days: 1, 2 ... 31 etc.
        .Select(day => new DateTime(year, month, day)) // Map each day to a date
        .ToList(); // Load dates into a list
}

// 一応forバージョンも。最も直観的でわかりやすいのでは？
public static List<DateTime> GetDates(int year, int month)
{
   var dates = new List<DateTime>();
   for (var date = new DateTime(year, month, 1); date.Month == month; date = date.AddDays(1))
   {
      dates.Add(date);
   }
   return dates;
}
```

---

## IN と NOT IN

<https://www.kazukiio.com/entry/2019/01/29/000919>

似たようなのに差集合を取得するやつも作ったけど、片方にあるやつを抜き出すだけならINで十分だった。  

``` C#
// テストデータ
var shops = new[]
    {
        new Shop() {ShopId = 1, ShopName = "shop1"},
        new Shop() {ShopId = 2, ShopName = "shop2"},
        new Shop() {ShopId = 3, ShopName = "shop3"},
        new Shop() {ShopId = 4, ShopName = "shop4"},
        new Shop() {ShopId = 5, ShopName = "shop5"},
        new Shop() {ShopId = 6, ShopName = "shop6"},
    };
var inCause = new int[] { 1, 3, 5 };

// IN
var results = shops.Where(x => inCause.Contains(x.ShopId));
// NOT IN
var results = shops.Where(x => !inCause.Contains(x.ShopId));
```

## Contains

[配列やコレクション内に指定された要素があるか調べる](https://dobon.net/vb/dotnet/programing/arraycontains.html)  
LinqのIN,NOT INでしれっとContainsを使っているけど、どういうメソッドなのか、実は知らないのでしらべた。  

INとNOT INでは、Whereで1レコード引っ張って、それをそのコレクションのContainsで回すって流れるなるわけか。  
で、あればTrueが返却されるので、そのレコードは取得され、それを繰り返していくわけだ。  

``` C#
// 指定した値と同じ要素がコレクション内に存在するかを調べるメソッド
// 存在すればtrueを、しなければfalseを返します。
// 順次検索のため、O(n)操作です。
// IList<T> or LINQ（Enumerable.Contains<TSource>メソッド）のメソッド

//検索元のArrayListを作成する
System.Collections.ArrayList al = new System.Collections.ArrayList();
al.Add("b");
al.Add("aaaaa");
al.Add("cc");

//al内に"cc"があるか調べる
//あるので、"true"を返す
bool b1 = al.Contains("cc");
//al内に"a"があるか調べる
//ないので、"false"を返す
bool b2 = al.Contains("a");
```

---

## 差集合を取得

<https://www.urablog.xyz/entry/2018/07/04/070000>  

Exceptメソッド  

``` C#
    // 人物データ
    var dataA = new List<(int ID, string Name)>()
    {
       (ID = 0, Name = "正一郎"),
       (ID = 5, Name = "清次郎"),
       (ID = 3, Name = "誠三郎"),
       (ID = 9, Name = "征史郎"),
    };
    var dataB = new List<(int ID, string Name)>()
    {
       (ID = 5, Name = "清次郎"),
       (ID = 3, Name = "誠三郎"),
       (ID = 2, Name = "征史郎"),
    };

    var results  = dataA.Select(s => s.ID).Except(dataB.Select(s => s.ID));

    // System.Console.WriteLine( "dataA  :{0}", dataA.Text() );
    // System.Console.WriteLine( "dataB  :{0}", dataB.Text() );
    foreach (var item in results){
        System.Console.WriteLine( "results:{0}", item );    
    }
---

## x.Items!=null && x.Items.Any()のショートカット

<https://stackoverflow.com/questions/28903952/a-shortcut-for-c-sharp-null-and-any-checks/28904185>  

``` C#
// この2つの比較は
if(x.Items!=null && x.Items.Any())
// このように1つにすることができる
if(x.Items?.Any() == true)

// ちなみにnullまたは1件もない場合をはじきたい場合はこう書かないといけない。
// null == false は falseになるので、if文の中に入らないので、nullだったらfalseに変換してあげる。
if(x.Items?.Any() ?? false == false)
// こっちでもよかった。単純なのに奥が深い。
// そりゃそうだよな。nullはtrueでもfalseでもない。
// わざわざnullをfalseにしてfalseと比較するのではなく、trueではないってやればいいだけの話だった。
if(x.Items?.Any() != true)
```

---

## 配列の全ての要素が同じであるか判定したい

<https://teratail.com/questions/140064>

``` C#
if(rawA.Distinct().Count()==1)
```

---

## 重複を確認したい

[【C#】List中の重複する要素を抽出する方法](https://qiita.com/nkojima/items/c927255b8d621d714f0a)  
[C#, LINQのGroupByで重複した要素と重複した要素が何個あるかを得る](https://hayaup.hatenablog.com/entry/2020/04/16/012501)  

商品台帳の修正中にフロント側で重複のチェックをしたほうがだろうというバグを見つけたので、  
Linqで重複チェックくらいできるだろう、ということで探したのでまとめ。  
Linq万歳。  

``` C#
// GroupByしてWhereとやると、まとめた要素がレコードごとにループされるっぽいので、Countってやると、そのまとめた件数が何件あるか取れる。
// なので、1レコードが2件以上あるということは、どこかしら重複があると見なせるのでAnyで取得する。
if (Data.ProductUnitPriceList
        .GroupBy(g => new { g.PrivilegeTypeCD, g.SeasonCD, g.FeeTypeCD, g.UseTimeZoneCD })
        .Any(a => a.Count() > 1)
)
if (Data.ProductJanList.GroupBy(g => g.JanCD).Any(w => w.Count() > 1))
```

---

## 指定の要素数で初期化されたListを作成する方法

[C#の配列を同じ値で初期化する](https://www.paveway.info/entry/2019/07/**15_csharp_initarray**)  

検索処理が複雑だったので、1件の時はいつも通りにするけど、2件以上あったら、適当に2件作って、
フロント側で判断してごにょごにょするって感じで作った。  

``` C#
    // List<Product> Count() = 2にする
    Enumerable.Repeat(new Product(), 2).ToList();
```

---

## LinqのCast

[【LINQ】Cast\<T>は使わない方が良い？OfType\<T>を使おう](https://threeshark3.com/castistrap/)  

Castって失敗した時の挙動ってどうなのか知らなかったので調べた。  
→  
失敗すると例外を吐く模様。  

それはそうと、CastはFirstと同じくらい罠らしく、CastではなくOfTypeを使うべきって記事を見つけたのでまとめる。  

OfType\<T>
キャストできなかった時 : 要素はフィルタリングされる
要素がnullだった時 : 要素はフィルタリングされる

Cast\<T>
キャストできなかった時 : 例外になる
要素がnullだった時 : 例外になる or 要素はフィルタリングされない

OfType\<T>は挙動の一貫性があり、予想外の値が来た時も安定してフィルタリングしてくれますね。  
以上の点から、Cast\<T>を使う必要はほぼなく、OfType\<T>を使うことを強くお勧めします。  

``` C#
    object[] hogeArray = new object[] { 3, "apple", 42 };
    foreach (var item in hogeArray.OfType<int>())
    {
        // キャストできたものだけforeachの中にくるので、
        // 3 と 42 だけが出力される
        Console.WriteLine(item);
    }
    // 2個目の要素を列挙しようとした時に例外発生
    foreach (var item in hogeArray.Cast<int>())
    {
        // 3 はintにキャストできるので出力されるが、
        // "apple"をキャストしようとしたタイミングで例外発生!
        Console.WriteLine(item);
    }
```

``` C#
    // こうやるのと
    if (args?.AddedItems?.Count > 0
        && args.AddedItems.OfType<Hoge>() is IEnumerable<Hoge> addItems)
    {
        HogeList.AddRange(addItems);
    }
    // こうやるのとだったら、こっちのほうがいいのでは
    foreach (var addItem in args.AddedItems.OfType<Hoge>())
    {
        HogeList.Add(addItem);
    }
```

---

## 単純にコード一覧が欲しい時にイミディエイトで出力する処理

まぁ、これといったあれはないけど、意外と迷ったのでメモしておく。  
必要なくなったら消してもいいでしょう。  

``` C#
string.Join(",", TestList.SelectMany(s2 => s2.SlipList.Select(s3 => s3.SubjectCD)).Distinct())
// "20,120,900,131,800,900,20,120,900,110"
```

---

## 遅延実行とIQueryable

[LINQの遅延実行&即時実行とforeach+遅延実行の問題【C#】【LINQ】](https://kan-kikuchi.hatenablog.com/entry/LINQ_Delay)  

IQueryableのヒントになるかどうか知らないが、このサイトのある一文がほぼ全てな気がした。  
`whereは呼び出された時点で処理をして、その結果を返しているわけではなく、どういう処理をするかの命令(クエリ)を返している`  
つまり`Whereは配列を作ってるわけではなくクエリを作っている`  

IQueryableは外部実行が何たらかんたらって言ってたから、つまりそういうことなのかなーと思ったり。  
とりあえず、即時実行の動作確認。  

``` C# : 遅延実行サンプル
    var aa = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
    var res1 = aa.Where(w => w > 7);
    var res2 = aa.Where(w => w > 7).ToList();
    
    // 8,9
    foreach (var item in res1) Console.WriteLine(item);
    // 8,9
    foreach (var item in res2) Console.WriteLine(item);

    aa.Add(10);
    // 実際に値が必要になった時に処理を実行(遅延実行)
    // 8,9,10 
    foreach (var item in res1) Console.WriteLine(item);
    // 8,9
    foreach (var item in res2) Console.WriteLine(item);
```

[2種類のLINQ](https://csharptan.wordpress.com/2011/12/09/2%E7%A8%AE%E9%A1%9E%E3%81%AElinq/)  
[結局IEnumerable\<T>とIQueryable\<T>はどう違うの？](https://qiita.com/momotaro98/items/7be27447f5f4a5c8bac9)  
[What is the difference between IQueryable\<T> and IEnumerable\<T>?](https://stackoverflow.com/questions/252785/what-is-the-difference-between-iqueryablet-and-ienumerablet)  
[IEnumerable vs IQueryable](https://samueleresca.net/the-difference-between-iqueryable-and-ienumerable/)  

``` txt
IQueryable<T>は外部リソースを扱うLinqプロバイダのために提供されているもの。
外部クエリ方式: クエリを投げて、外部のサーバー上で処理してもらって、結果だけ受け取る。
```

なんか頻繁に外部がどうだの言ってるけど、んなわけなくね？  
データベースなんてまったく関係ないでしょ。  
正直、こいつらが何言ってるかわからないんだけど。  

``` txt
IQueryable<T>はLinqメソッド実行時はあくまでクエリが構築されるだけのようです。
foreachなどの評価で初めてクエリが外部ソースに発行され結果が取得されるので、IQueryable<T>は遅延"読み込み"(lazy loading)の特徴があると説明されます。
```

「クエリを構築するだけ。」「問い合わせ可能」って認識程度でいいのかもねぇ。  
本質は「即時実行されない」「評価が必要になった時に実行される」で充分だと思う。  

---

## 連番を指定した数の塊に分解する方法

間隔3と指定した場合、
{
    {0,1,2},
    {3,4,5},
    {6,7,8}
}

間隔5を指定した場合、
{
    {0,1,2,3,4},
    {5,6,7,8,9},
    {10,11,12,13,14},
}

年齢を範囲で区切る処理で必要になったのでまとめ。  
みたいな一連のリストを生成するスマートな処理はないものか探した。

最後に考えたのはForで愚直にやる方法。  
スマートじゃないし、直観的でもないけど、十分動く。でもって速度的には一番早い。  

``` C#
var kankaku = 5;
var from = 0;
// 間隔分インクリメントされる。
for (var to = kankaku - 1; to < 150; to += kankaku)
{
    // ここの間は0,1,2,3,4  5,6,7,8,9 になってる。

    // fromは6,10,16みたいな感じで常にto + 1の値にする。
    from = to + 1;
}
```

探したら使えそうなのがたくさんあった。  

[【C#】LINQ でコレクションをN個ずつの要素に分割する](https://qiita.com/Nossa/items/db9bff2390291432d138)  
[連続する数値でグループ分けする](https://noriok.hatenadiary.jp/entry/2015/06/14/122043)  
[LINQでn個ずつグルーピング](https://ichiroku11.hatenablog.jp/entry/2015/04/16/230309)  
[[C#][VB] LINQでコレクションをチャンク(N個ずつ)に分割](https://webbibouroku.com/Blog/Article/chunk-linq)  

1から100までの連番を生成して、それを指定した要素数の塊にわけて、ループして最小値と最大値を取れば実現できるという寸法。  

``` C#
    // N 個ずつの N
    var chunkSize = 5;
    var chunks = Enumerable.Range(0, 100)
        .Select((v, i) => (v, i))
        .GroupBy(x => x.i / chunkSize)
        .Select(g => g.Select(x => x.v));
    // 動作確認
    foreach (var chunk in chunks)
    {
        foreach (var item in chunk)
            Console.Write($"{item} ");
        Console.WriteLine();
    }

    // fromtoとnullのバージョン
    List<(int, int)?> chunks2 = Enumerable.Range(0, 100)
        .Select((v, i) => (v, i))
        .GroupBy(x => x.i / 5)
        .Select(g => ((int, int)?)(g.Min(m => m.v), g.Max(m => m.v)))
        .ToList();
    chunks2.Insert(0, null);
```

---

## firstordefaultした奴は変更したら元にも反映される

FirstOrDefaultしたものがプリミティブなら、Listは変更されないが、参照型の場合、普通に考えたら変更されるわな。  
だけど、改めて考えてみたらどうなるんだろうってなったのでまとめ。  

---

## Union

[要素が重複しないようにして、複数の配列（またはコレクション）をマージする（和集合を取得する）](https://dobon.net/vb/dotnet/programing/arrayunion.html)  

AリストとBリストの番号を統合して重複を排除した合計が1より大きいか？みたいな判定する時に使ったのでメモ。  

``` C#
if (TestList1.Select(s => s.TestNo).Union(TestList2.Select(s => s.TestNo)).Distinct().Count(w => !string.IsNullOrEmpty(w)) > 1)
```

---

## AddとUnion

LinqのAddはvoidなので、チェーンして書くことができない。  
だけど、Unionを工夫して使うと全部繋げて書けるよっていう例  
速度は保証できないので、完全に好みであるが、備忘録として残しておく  

``` C#
    // 本来のパターン
    // 途中でAddさせたいならいったんチェーンを切らないといけないし、ToList化もしないといけない。
    var framePlayerList = TestModel
        .GetList(condition)
        .Where(w => w.TestNo != targetPlayer.TestNo)
        .ToList();
    // チェックインするプレーヤーを追加する
    framePlayerList.Add(targetPlayer);
    // 今回のチェックインでその枠に存在するプレーヤー全員がチェックイン済みになるなら組確定とする。
    reservationFrame.ConfirmFlag = framePlayerList.All(a => a.CheckinFlag == true);


    // UNIONで疑似的にADDしたパターン
    // 繋げるべきデータをUnion内で作ってしまえば、データの追加が可能というわけ
    reservationFrame.ConfirmFlag = TestModel
        .GetList(condition)
        .Where(w => w.TestNo != targetPlayer.TestNo)
        // そのプレーヤーNoを除外した後、Listを作成してUNIONすることで疑似的なADDが可能というわけ
        .Union(new List<TestClass>() { targetPlayer })
        .All(a => a.CheckinFlag == true);


    // ごめん。このサンプルだったらこれで済んだわ。
    // 比較したい要素はフラグだけだから、そのプレーヤーが既に存在するなら、
    // そのフラグだけ最新のプレーヤー情報に書き換えればいいだけだった。
    var framePlayerList = TestModel
        .GetList(condition)
        .All(a => {
            if (a.TestNo == targetPlayer.TestNo) {
                a.CheckinFlag = targetPlayer.CheckinFlag;
            }
            return a.CheckinFlag == true;
        });

    var tupleList = new List<(int Index, bool flag)>
        {
            (1, false),
            (2, true),
            (3, true),
        }
        .All(a => {
            if (a.Index == 1)
            {
                a.flag=true;
            }
            return a.flag == true;
        });
```

``` C#
        static void Benchmark()
        {
            var stopWatch = new System.Diagnostics.Stopwatch();
            var aa = new List<int>();

            // 処理1 約20~30万tick
            stopWatch.Start();
            for (int i = 0; i < 10000000; i++)
            {
                aa.Add(i);
            }
            stopWatch.Stop();
            stopWatch.Reset();

            aa.Clear();

            // 処理2 約100万tick
            stopWatch.Start();
            aa = aa.Union(Enumerable.Range(0, 10000000)).ToList();
            stopWatch.Stop();
            stopWatch.Reset();
        }
```

---
