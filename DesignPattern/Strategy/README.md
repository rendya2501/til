# Strategy

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

---

[C#でGoFのデザインパターン~Strategyパターン編~](https://tech-blog.cloud-config.jp/2019-10-30-strategypattern/)  
