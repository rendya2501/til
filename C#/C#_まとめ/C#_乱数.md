# 乱数まとめ

---

## 短時間でRandomクラスを複数インスタンス化すると乱数が同値になる

[C#で乱数を作成する & 毎回異なるシードを指定する方法](https://takap-tech.com/entry/2019/05/09/222104)  

短時間で複数のRandomクラスのインスタンスを作成して乱数を生成すると同じ値になってしまいます。
そのような場合、複数のインスタンスを作成するのではなく同じインスタンスを使用してNextメソッドを呼び出せば回避できますが、事情でそうできない場合もあります。  

``` C# : 回避案(1) Randomクラスのインスタンスを全体で使いまわす
public static class MyRandom
{
    private static Random random;
    public static int Next()
    {
        if (random == null) random = new Random();
        return random.Next();
    }
    public static int Next(int maxValue)
    {
        if (random == null) random = new Random();
        return random.Next(maxValue);
    }
    public static int Next(int minValue, int maxValue)
    {
        if (random == null) random = new Random();
        return random.Next(minValue, maxValue);
    }
}
```

``` C# : 回避案(2) ミリ秒以下の呼び出しでも異なるシード値を指定する
// ユニークなSEED値を持つRandomオブジェクトを生成するためのクラス
public static class MyRandom
{
    // 乱数のSeed値に乱数を使用する
    private static Random random = new Random();
    public static Random Create() => new Random(random.Next());
}

// 使い方
var list = new List<Random>();
for(int i = 0; i < 100; i++)
{
    // list.Add(new Random()); // こうすると毎回同じになってしまうので
    list.Add(MyRandom.Create()); // こう変更すると毎回、違くなる
}
foreach (var r in list)
{
    Console.WriteLine(r.Next()); // 必ず違う値になる
    // > 314216147
    // > 1401494015
    // > 370983633
    // ....
}
```
