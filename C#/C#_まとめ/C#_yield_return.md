# yield return

yield return は、イテレーターブロック内で使用されるキーワードです。  
イテレーターは、コレクションやリストの要素を順番にアクセスするためのシンプルなインターフェースを提供します。  
IEnumerable\<T> インターフェースを実装することで、独自のイテレーターを作成できます。  

イテレーターブロック内で yield return を使用すると、戻り値の生成と状態の保持が自動的に行われます。  
イテレーターは、yield return で指定された値を次々と返します。  
これにより、大量のデータをメモリに格納することなく、必要に応じてデータを生成することができます。  

---

## 使用例

``` cs
public static void Execute()
{
    // 1. GenerateNumbersを実行
    foreach (int number in GenerateNumbers(10))
    {
        // 4. numberが1を受け取り、それを表示する
        // 7. numberが2を受け取り、それを表示する
        Console.WriteLine(number);
    // 5. コードブロックの最後でGenerateNumbersに制御が戻る
    // 8. コードブロックの最後でGenerateNumbersに制御が戻る
    }
    // 11. GenerateNumbersの処理終了とともにforeachブロックを抜ける
}

// 2. count = 10 でGenerateNumbersが実行される
static IEnumerable<int> GenerateNumbers(int count)
{
    for (int i = 1; i <= count; i++)
    {
        // 3. 1が返却される
        // 6. 2が返却される
        yield return i;
    // 5. 制御が戻ってくるので次のループを実行する
    // 9. 制御が戻ってくるので次のループを実行する 以下同じ
    }
    // 10. ループ終了でGenerateNumbersの実行が終わる
}
```

---

## 再帰での使用例

再帰の例として、二分木の各ノードを深さ優先探索 (DFS: Depth-First Search) で巡回するコードを示します。  
二分木のノードは以下のようなクラスで表現します。  

``` cs
public class TreeNode
{
    public int Value;
    public TreeNode Left;
    public TreeNode Right;

    public TreeNode(int value, TreeNode left = null, TreeNode right = null)
    {
        Value = value;
        Left = left;
        Right = right;
    }
}
```

そして、深さ優先探索で二分木を巡回するためのイテレーターを作成します。  
yield return を使って再帰的にノードの値を返します。  

``` cs
public static IEnumerable<int> DepthFirstTraversal(TreeNode node)
{
    if (node == null)
    {
        yield break;
    }

    yield return node.Value;

    foreach (int value in DepthFirstTraversal(node.Left))
    {
        yield return value;
    }

    foreach (int value in DepthFirstTraversal(node.Right))
    {
        yield return value;
    }
}
```

このイテレーターは、与えられた二分木のノードを深さ優先探索で巡回し、ノードの値を順に返します。  
まず、与えられたノードの値を返し、次に左の子ノードを再帰的に巡回し、最後に右の子ノードを再帰的に巡回します。  
再帰の各ステップで yield return を使って値を返すことで、イテレーターの動作がシームレスになります。  

`DepthFirstTraversal`メソッドを使って二分木を巡回するサンプルコードを示します。  

``` cs
class Program
{
    static void Main()
    {
        var tree = new TreeNode(1,
            new TreeNode(2,
                new TreeNode(4),
                new TreeNode(5)),
            new TreeNode(3,
                new TreeNode(6),
                new TreeNode(7)));

        foreach (int value in DepthFirstTraversal(tree))
        {
            Console.WriteLine(value);
        }
    }
}
```

このコードは、以下の二分木を作成し、深さ優先探索で巡回します。

``` txt
    1
   / \
  2   3
 / \ / \
4  5 6  7
```

実行結果は次のようになります。

``` txt
1
2
4
5
3
6
7
```

以下、全プログラム。  

``` cs
public class DepthFirstSearch
{
    public static void Execute()
    {
        var tree = new TreeNode(1,
            new TreeNode(2,
                new TreeNode(4),
                new TreeNode(5)),
            new TreeNode(3,
                new TreeNode(6),
                new TreeNode(7)));

        foreach (int value in DepthFirstTraversal(tree))
        {
            Console.WriteLine(value);
        }
    }

    /// <summary>
    /// 二分木を巡回するためのイテレーター
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public static IEnumerable<int> DepthFirstTraversal(TreeNode node)
    {
        if (node == null)
        {
            yield break;
        }

        yield return node.Value;

        foreach (int value in DepthFirstTraversal(node.Left))
        {
            yield return value;
        }

        foreach (int value in DepthFirstTraversal(node.Right))
        {
            yield return value;
        }
    }

    /// <summary>
    /// 二分木ノード
    /// </summary>
    public class TreeNode
    {
        public int Value;
        public TreeNode Left;
        public TreeNode Right;

        public TreeNode(int value, TreeNode left = null, TreeNode right = null)
        {
            Value = value;
            Left = left;
            Right = right;
        }
    }
}
```

---

## yield return の戻り値

yield return を使用する場合、戻り値は IEnumerable または IEnumerable\<T> のいずれかでなければなりません。  
yield return を使うと、イテレーターブロックを実装することができ、このイテレーターブロック内で yield return を使って要素を一つずつ返すことができます。  

イテレーターブロックは、IEnumerable または IEnumerable\<T> を返すメソッド、プロパティ、インデクサに使用することができます。  
イテレーターブロック内では、yield return を使ってシーケンスの次の要素を返すことができ、また yield break を使ってシーケンスの終了を示すことができます。  

``` cs
public IEnumerable<int> GetEvenNumbers(int count)
{
    int current = 0;
    for (int i = 0; i < count; i++)
    {
        yield return current;
        current += 2;
    }
}
```

この例では、GetEvenNumbers メソッドが IEnumerable\<int> を返しています。  
このメソッドは、指定された個数の偶数を生成し、yield return を使って一つずつ返します。  
yield return を使用することで、イテレーターブロックの状態が自動的に保持され、シーケンスの次の要素が必要になるまで処理が中断されます。  

このように、yield return を使用する場合、戻り値は IEnumerable または IEnumerable\<T> でなければなりません。  
これにより、独自のシーケンスを効率的に生成することができます。  
