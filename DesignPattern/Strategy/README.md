# Strategy

## 概要

振る舞いやアルゴリズムをオブジェクト指向の原則に従ってカプセル化する。
アルゴリズムを簡単に切り替えたり、新しいアルゴリズムを追加できるようにするために使用される。  

ストラテジーパターンは、アルゴリズムや振る舞いが頻繁に変更される場合や、複数のアルゴリズムが異なるシナリオで使用される場合に有効となる。  

---

## 主要な概念

1. **ストラテジー（Strategy）**  
   インターフェースまたは抽象クラス。  
   アルゴリズムの構造を定義する。  

2. **コンクリートストラテジー（Concrete Strategy）**  
   ストラテジーインターフェースを実装した具体的なアルゴリズム。  
   これらのクラスは、アルゴリズムの異なるバリエーションを表す。  

3. **コンテキスト（Context）**  
   ストラテジーを使用するクライアント。  
   コンテキストは、ストラテジーインターフェースを使用してアルゴリズムを実行するが、コンクリートストラテジーに直接依存しない。  

---

## メリット

1. **アルゴリズムの切り替えが容易**  
   アルゴリズムの切り替えが必要な場合、コンテキストに別のストラテジーを渡すだけで実現できます。  

2. **コードの維持性と拡張性が向上**  
   新しいアルゴリズムを追加する際に、既存のコードを修正せずに新しいストラテジークラスを追加するだけで済みます。  

3. **単一責任の原則（SRP）**  
   各ストラテジーは、それぞれのアルゴリズムに対して単一の責任を持ちます。  
   これにより、コードがきれいで読みやすくなります。  

4. **オープン/クローズドの原則（OCP）**  
   コードの変更に対して閉じられており、拡張に対して開いています。  
   新しいアルゴリズムを追加する場合、既存のコードを変更せず、新しいコンクリートストラテジーを追加するだけで済みます。  

---

## ステートパターンとの違い

ステートパターンとストラテジーパターンは確かに似ているがそれぞれ異なる目的で使用される。  
主な両者の違いは以下の通りとなる。  

1. 目的：  
   - ストラテジーパターン  
     アルゴリズムや振る舞いをカプセル化し、クライアントが簡単に切り替えることができる柔軟な構造を提供する。  
     同じ問題を解決するための異なるアルゴリズムや手法が存在する場合に適している。  
   - ステートパターン  
     オブジェクトの内部状態に応じて、その振る舞いを変更する。  
     オブジェクトが異なる状態で異なる振る舞いを持つべき場合に適している。  

2. 構造：  
   - ストラテジーパターン  
     コンテキストクラスは、ストラテジーインターフェースを使用してアルゴリズムを実行するが、コンクリートストラテジーに直接依存しない。  
   クライアントは必要に応じてストラテジーを変更できる。  
   - ステートパターン  
     コンテキストクラスは、状態インターフェースを使用して振る舞いを実行しするが、コンクリート状態に直接依存しない。  
     コンテキストは通常、状態の変更を監視し、必要に応じて状態オブジェクトを切り替える。  

3. クライアントとの関係：  
   - ストラテジーパターン  
     クライアントは、アルゴリズムや振る舞いを選択し、それをコンテキストに渡すことができる。  
     クライアントがアルゴリズムを選択し、コンテキストにインジェクトすることが一般的です。  
   - ステートパターン  
     クライアントは、コンテキストを通じてオブジェクトの振る舞いにアクセスしますが、通常は状態の変更を直接管理しない。  
     状態の変更は、通常コンテキスト内で管理されます。  

要約すると、ストラテジーパターンは異なるアルゴリズムや振る舞いをカプセル化し、クライアントが簡単に切り替えられるようにするのに対して、ステートパターンはオブジェクトの状態に応じて振る舞いを変更することに焦点を当てている。  
ステートパターンでは、オブジェクトの状態が変わると、その振る舞いも自然に変わる。  
このため、ステートパターンは、オブジェクトが複数の状態を持ち、それぞれの状態で異なる振る舞いを実行する必要がある場合に適している。  

両者は構造的に似ているが、使用目的や応用分野が異なる。  
アルゴリズムや手法を切り替える柔軟性が必要な場合はストラテジーパターンを、オブジェクトの状態に応じて振る舞いを変更することが重要な場合はステートパターンを選択することが適切となるs。  

---

## サンプル

``` cs
/// ストラテジー（Strategy）
/// アルゴリズムの構造を定義する
public interface ISortStrategy
{
    void Sort(List<int> list);
}


/// コンクリートストラテジー（Concrete Strategy）
/// ストラテジーインターフェースを実装した具体的なアルゴリズム。
public class BubbleSortStrategy : ISortStrategy
{
    public void Sort(List<int> list)
    {
        // Bubble sort algorithm implementation
    }
}

public class QuickSortStrategy : ISortStrategy
{
    public void Sort(List<int> list)
    {
        // Quick sort algorithm implementation
    }
}

public class MergeSortStrategy : ISortStrategy
{
    public void Sort(List<int> list)
    {
        // Merge sort algorithm implementation
    }
}


/// コンテキスト（Context）
/// ストラテジーを使用するクライアント。
public class SortContext
{
    private ISortStrategy _sortStrategy;

    public SortContext(ISortStrategy sortStrategy)
    {
        _sortStrategy = sortStrategy;
    }

    public void SetSortStrategy(ISortStrategy sortStrategy)
    {
        _sortStrategy = sortStrategy;
    }

    public void ExecuteSort(List<int> list)
    {
        _sortStrategy.Sort(list);
    }
}


/// 実行例
/// さまざまなソートアルゴリズムを切り替える。
public static void Main(string[] args)
{
    var myList = new List<int> { 5, 2, 8, 1, 4 };

    var sortContext = new SortContext(new BubbleSortStrategy());
    sortContext.ExecuteSort(myList); // Sorts myList using Bubble Sort

    sortContext.SetSortStrategy(new QuickSortStrategy());
    sortContext.ExecuteSort(myList); // Sorts myList using Quick Sort

    sortContext.SetSortStrategy(new MergeSortStrategy());
    sortContext.ExecuteSort(myList); // Sorts myList using Merge Sort
}
```

---

## 実践例_テーブルの接頭字の切り替え

enumの状態によって参照するテーブルのPrefixを切り替えたい。  
Front系テーブル、Reservation系テーブルのPrefixの状態をそれぞれのクラスの固定として定義して、それを切り替えて管理したい。  

使用時は `var query = $"SELECT * FROM {Prefix.Front}_TableA";` のように、Prefix.Frontだけ指定すれば、後は"TFr"か"TPa"かは意識しないで済むようにしたい。  

``` cs
// Prefixのインスタンス化
var prefix = new Prefix(DateStatus.InPast);
// クエリの構築
var query = $"SELECT * FROM {prefix.Front}_FrontTable";


// 当日・未来と過去の状態を定義
public enum DateStatus
{
    InPast,
    InPresentOrFuture
}

// Strategyインターフェース
public interface IPrefixStrategy
{
    string Front { get; }
    string Reservation { get; }
}

// 過去状態のStrategyを定義
public class PastPrefixStrategy : IPrefixStrategy
{
    public string Front { get; } = "TPa";
    public string Reservation { get; } = "TPa";
}

// 当日・未来状態のStrategyを定義
public class PresentOrFuturePrefixStrategy : IPrefixStrategy
{
    public string Front { get; } = "TFr";
    public string Reservation { get; } = "TRe";
}

// Contextクラスの定義
public class Prefix
{
    private readonly IPrefixStrategy strategy;

    public string Front => strategy.Front;
    public string Reservation => strategy.Reservation;

    public Prefix(DateStatus status)
    {
        strategy = status == DateStatus.InPast
            ? new PastPrefixStrategy()
            : new PresentOrFuturePrefixStrategy();
    }
}
```

コード内の各要素がストラテジーパターンの概念に対応している。  

1. **IPrefixStrategy**
   ストラテジーインターフェース。  
   様々なプレフィックス（接頭辞）を取得するための構造を定義する。  

2. **PastPrefixStrategyとPresentOrFuturePrefixStrategy**
   コンクリートストラテジー。  
   それぞれの状態（過去、現在・未来）に対応する接頭辞を提供する具体的なアルゴリズムを実装する。  

3. **Prefix**
   コンテキストクラス。  
   IPrefixStrategyインターフェースを使用して、適切な接頭辞を取得する。  
   コンストラクタ内でDateStatusに基づいてストラテジーが選択される。  

DateStatusに基づいて適切なプレフィックスを取得するために、ストラテジーパターンを使用している。  
Prefixクラスはコンテキストクラスであり、状況に応じて適切なストラテジーを選択する。  
この実装により、プレフィックス取得方法を柔軟に変更できるため、コードの拡張性とメンテナンス性が向上する。  

### ストラテジーパターンの主な目的

こちらの例では、外部から実装をインジェクションしていない。  

それでもストラテジーパターンといえるのか？ということになるが、そもそもストラテジーパターンの主な目的はアルゴリズムや振る舞いをカプセル化し、それらを簡単に切り替えることができる柔軟な構造を提供することにある。  
外部から実装をインジェクトするかどうかは、実際の要件や設計上の選択による。  

外部から実装をインジェクトすることで、より柔軟なコードを書くことができるが、場合によっては内部で条件に応じてストラテジーを選択するだけでも十分な場合がある。  
前述のコード例のように、コンテキスト内でストラテジーを選択することでも、ストラテジーパターンの基本的な目的を達成できる。  

ただし、外部からストラテジーをインジェクトできるように設計することで、コードの再利用性やテストの容易さが向上することがある。  
どちらのアプローチを採用するかは、アプリケーションの要件や設計上の選択による。  

---

[C#でGoFのデザインパターン~Strategyパターン編~](https://tech-blog.cloud-config.jp/2019-10-30-strategypattern/)  
