using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ObserverObservableSample;

public class Sample4
{
    /// <summary>
    /// サンプル第4弾
    /// https://qiita.com/nomok_/items/39b5d7c61810f274768d
    /// </summary>
    /// <param name="args"></param>
    static void Main(string[] args)
    {
        // ①
        //var myObservable = new MyObservable();
        //myObservable.Subscribe(new MyObserver());

        //myObservable.Notify("Hello");
        //myObservable.Notify("World");


        // ②
        // IObservable<T>
        var subject = new Subject<string>();

        // IObserverをわざわざ定義しなくてもラムダ式でOK
        var d = subject.Subscribe(str => Console.WriteLine(str));

        // subject(Observable)から変更通知
        subject.OnNext("Hello");
        subject.OnNext("World");

        // Subscribeで返されるIDisposableのDisposeを呼ぶと、Observableへの登録を解除できる
        d.Dispose();

        // OnNextをしてもWriteLineされない
        subject.OnNext("Rx完全に理解した");


        var subject2 = new Subject<int>();
        // subjectから発行された値が3の場合のみ、SubscribeしているIObserver<T>に通知する
        subject2.Where(w => w == 3).Subscribe(s => Console.WriteLine("OnNext : {0}", s));
        subject2.OnNext(1);
        subject2.OnNext(2);
        subject2.OnNext(3);

        // ObservableのRangeメソッド
        // 第一引数に指定した値から1ずつ増やした値を第2引数で指定した数だけ返す
        Observable.Range(1, 3)
            .Select(s => s * s) //値を2乗する
            .Subscribe(
                s => Console.WriteLine("OnNext : {0}", s),
                () => Console.WriteLine("Completed"));
    }
}

class MyObservable : IObservable<string>
{
    private ICollection<IObserver<string>> _collection = new List<IObserver<string>>();

    public IDisposable Subscribe(IObserver<string> observer)
    {
        this._collection.Add(observer);
        // 一度追加したobserverの削除には対応しない
        return null;
    }

    // 自分をSubscribeしてるObserverに変更を通知する
    public void Notify(string value)
    {
        foreach (var observer in this._collection)
        {
            observer.OnNext(value);
        }
    }
}

class MyObserver : IObserver<string>
{
    public void OnCompleted()
    {
        Console.WriteLine("OnCompleted");
    }

    public void OnError(Exception error)
    {
        Console.WriteLine("OnError");
    }
    // 変更通知を受け取るメソッド
    public void OnNext(string value)
    {
        Console.WriteLine("OnNext : {0}", value);
    }
}
