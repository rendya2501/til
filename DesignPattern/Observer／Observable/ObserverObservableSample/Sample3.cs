using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ObserverObservableSample;

public class Sample3
{
    static void Main(string[] args)
    {
        MyObserver observer = new MyObserver();
        MyObservable observable = new MyObservable();
        // 監視人を割り当てる
        IDisposable disporser = observable.Subscribe(observer);

        Console.WriteLine("------------作業開始-----------");
        // 監視人がいるのでObserverが反応する
        observable.Execute();
        //監視から外す
        disporser.Dispose();
        //監視対象が実行しても監視していないからObserverは反応しない
        Console.WriteLine("------------作業開始-----------");
        observable.Execute();
    }
}

/// <summary>
/// 労働者(監視される人)
/// </summary>
public class MyObservable : IObservable<int>
{
    /// <summary>
    /// 自分を監視する人
    /// </summary>
    public MyObserver myObserver;

    /// <summary>
    /// 監視される仕事を実行
    /// </summary>
    public void Execute()
    {
        Console.WriteLine("労働者「作業1をしました」");
        Task.Delay(300).Wait();
        // 監視人に通知
        myObserver?.OnNext(1);

        Console.WriteLine("労働者「作業2をしました」");
        Task.Delay(300).Wait();
        // 監視人に通知
        myObserver?.OnNext(2);

        Console.WriteLine("労働者「作業3をしました」");
        Task.Delay(300).Wait();
        //監視人に通知
        myObserver?.OnNext(3);

        Console.WriteLine("労働者「すべての作業おわりました」");
        Task.Delay(300).Wait();
        //監視人に完了通知
        myObserver?.OnCompleted();
    }

    /// <summary>
    /// Observerの登録
    /// 監視人を割り当てて監視開始
    /// </summary>
    /// <param name="observer"></param>
    /// <returns></returns>
    public IDisposable Subscribe(IObserver<int> observer)
    {
        myObserver = (MyObserver)observer;
        return new MyObserverDisposer(this);
    }
}


/// <summary>
/// 監視人(監視する人)
/// </summary>
public class MyObserver : IObserver<int>
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public MyObserver() { }

    //監視対象が処理を完了したとき
    public void OnCompleted()
    {
        Console.WriteLine("監視人「すべての作業終了を確認した」");
        Task.Delay(300).Wait();
    }
    //監視対象がエラーを出したとき
    public void OnError(Exception error)
    {
        Console.WriteLine("監視人「エラー発生を確認した」{0}", error);
        Task.Delay(300).Wait();
    }
    //監視対象から通知が来た時
    public void OnNext(int value)
    {
        Console.WriteLine("監視人「作業{0}を確認した」", value);
        Task.Delay(300).Wait();
    }
}

/// <summary>
/// 監視人の監視を解除する人
/// </summary>
public class MyObserverDisposer : IDisposable
{
    public MyObservable observable;
    public MyObserverDisposer(MyObservable observable)
    {
        this.observable = observable;
    }
    public void Dispose()
    {
        observable.myObserver = null;
        Console.WriteLine("削除人「監視を削除したぞ」");
        Task.Delay(300).Wait();
    }
}

