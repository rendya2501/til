
# インデクサ(indexer)

<https://ufcpp.net/study/csharp/oo_indexer.html#definition>  

クラスや構造体のインスタンスに配列と同様の添字を指定してアクセスするための構文。  
ユーザー定義型が配列型と同様に`[]`を用いた要素の読み書きが行えるようにしたもの。  
インデックスでアクセスするのでインデクサーだって。  
インデクサー自体はプロパティの拡張でしかない。  

```C#
アクセスレベル 戻り値の型 this[添字の型 添字]
{
  set
  {
    // setアクセサ
    //  ここに値の変更時の処理を書く。
    //  value という名前の変数に代入された値が格納される。
    //  添字が使える以外はプロパティと同じ。
  }
  get
  {
    // getアクセサ
    //  ここに値の取得時の処理を書く。
    //  メソッドの場合と同様に、値はreturnキーワードを用いて返す。
    //  こっちも添字が使える以外はプロパティと同じ。
  }
}
```

```C#
using System;

/// <summary>
/// 添字の下限と上限を指定できる配列。
/// </summary>
class BoundArray
{
    int[] array;
    int lower;   // 配列添字の下限

    public BoundArray(int lower, int upper)
    {
        this.lower = lower;
        array = new int[upper - lower + 1];
    }
    /// <summary>
    /// インデクサー
    /// </summary>
    public int this[int i]
    {
        set { this.array[i - lower] = value; }
        get { return this.array[i - lower]; }
    }
}

class Program
{
    static void Main()
    {
        BoundArray a = new BoundArray(1, 9);

        for (int i = 1; i <= 9; ++i)
            a[i] = i;

        for (int i = 1; i <= 9; ++i)
            Console.Write("a[{0}] = {1}\n", i, a[i]);
    }
}
```
