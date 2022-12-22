using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ObserverObservableSample;

public class Sample1
{
    static void Main(string[] args)
    {
        // 乱数生成インスタンスを生成
        NumberGenerator generator = new RandomNumberGenerator();
        // 観察者を生成
        IObserver observer1 = new DigitObserver();
        IObserver observer2 = new GraphObserver();
        // 観察者を登録
        generator.AddObserver(observer1);
        generator.AddObserver(observer2);
        // 数の生成の実行
        generator.Execute();
    }
}

/// <summary>
/// 数を生成する抽象クラス
/// </summary>
/// <remarks>
/// 被験者
/// </remarks>
public abstract class NumberGenerator
{
    /// <summary>
    /// Observerたちを保持
    /// </summary>
    private readonly List<IObserver> _Observers = new List<IObserver>();

    /// <summary>
    /// Observerを追加
    /// </summary>
    /// <param name="observer"></param>
    public void AddObserver(IObserver observer)
    {
        this._Observers.Add(observer);
    }
    /// <summary>
    /// Observerを削除
    /// </summary>
    /// <param name="observer"></param>
    public void DeleteObserver(IObserver observer)
    {
        this._Observers.Remove(observer);
    }
    /// <summary>
    /// Observerへ通知
    /// </summary>
    public void NotifyObservers()
    {
        foreach (var item in this._Observers)
        {
            item.Update(this);
        }
    }
    /// <summary>
    /// 数を取得する
    /// </summary>
    /// <returns></returns>
    public abstract int GetNumber();
    /// <summary>
    /// 数を生成する
    /// </summary>
    public abstract void Execute();
}

/// <summary>
/// 観察者を表すインターフェース
/// </summary>
public interface IObserver
{
    /// <summary>
    /// Updateメソッド
    /// </summary>
    /// <param name="generator"></param>
    /// <remarks>
    /// 私の内容が更新されました。表示のほうも更新してください。
    /// とObserverに伝えるためのもの。
    /// </remarks>
    void Update(NumberGenerator generator);
}

/// <summary>
/// 乱数生成クラス
/// </summary>
/// <remarks>
/// NumberGeneratorのサブクラス
/// 具体的な被験者。観察対象。
/// </remarks>
public class RandomNumberGenerator : NumberGenerator
{
    /// <summary>
    /// 乱数発生器
    /// </summary>
    private readonly Random _random = new Random();
    /// <summary>
    /// 現在の数
    /// </summary>
    private int _number;

    /// <summary>
    /// 数の生成
    /// </summary>
    public override void Execute()
    {
        for (int i = 0; i < 20; i++)
        {
            this._number = this._random.Next(50);
            NotifyObservers();
        }
    }
    /// <summary>
    /// 数の取得
    /// </summary>
    /// <returns></returns>
    public override int GetNumber()
    {
        return _number;
    }
}

/// <summary>
/// 観測した数を数字で表示するためのクラス
/// </summary>
/// <remarks>
/// 具体的な観察者
/// </remarks>
public class DigitObserver : IObserver
{
    public void Update(NumberGenerator generator)
    {
        Console.WriteLine("DigitObserver:" + generator.GetNumber());
        try
        {
            Task.Delay(300).Wait();
        }
        catch (Exception ex)
        {

        }
    }
}

/// <summary>
/// 観測した数を軌道で表示するクラス
/// </summary>
/// <remarks>
/// 具体的な観察者
/// </remarks>
public class GraphObserver : IObserver
{
    public void Update(NumberGenerator generator)
    {
        var count = generator.GetNumber();
        for (int i = 0; i < count; i++)
        {
            Console.Write("*");
        }
        Console.WriteLine("");
        try
        {
            Task.Delay(300).Wait();
        }
        catch (Exception ex)
        {

        }
    }
}

