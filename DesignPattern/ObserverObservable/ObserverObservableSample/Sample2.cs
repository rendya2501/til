using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ObserverObservableSample;

public class Sample2
{
    public static void Execute()
    {
        Observer<string> source = null;
        var root = new Observable<string>(o => source = o);

        //var second = new Observable<string>(o =>
        //{
        //    root.Subscribe(new Observer<string>(s =>
        //    {
        //        Console.WriteLine($"'{s}'が流れてきました。2倍にします。");
        //        o.OnNext(s + s);
        //    }));
        //});

        //var third = new Observable<string>(o =>
        //{
        //    second.Subscribe(new Observer<string>(s =>
        //    {
        //        if(s == "ふがふが")
        //        {
        //            Console.WriteLine("'ふがふが'は先に流しません");
        //        }
        //        else
        //        {
        //            o.OnNext(s);
        //        }
        //    }));
        //});

        //third.Subscribe(new Observer<string>(s =>
        //{
        //    Console.WriteLine($"値:{s}");
        //}));

        root
            .Select(s => s + s)
            .Where(s => s != "ふがふが")
            .Subscribe(s => Console.WriteLine($"値:{s}"));

        Console.WriteLine("値を送信します");
        source.OnNext("てすと");
        Console.WriteLine("値を送信します(2回目)");
        source.OnNext("ふが");
        Console.WriteLine("値を送信します(3回目)");
        source.OnNext("ほげほげ");



        var timer = new Stopwatch();
        timer.Start();

        int? vs = null;

        var j = 100000000;
        for (var i = 1; i <= 100000000; i++)
        {
            if (j >= i)
            {
                vs = i;
            }
            //else
            //{
            //    vs = null;
            //}
        }
        timer.Stop();
        Console.WriteLine(timer.ElapsedMilliseconds);
    }
}


class Observer<T>
{
    private readonly Action<T> onNext;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="onNext"></param>
    public Observer(Action<T> onNext) => this.onNext = onNext;
    public void OnNext(T v) => this.onNext.Invoke(v);
}

class Observable<T>
{
    private readonly Action<Observer<T>> subscribe;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="subscribe"></param>
    public Observable(Action<Observer<T>> subscribe) => this.subscribe = subscribe;
    public void Subscribe(Observer<T> observer) => this.subscribe(observer);
}

static class SubscribeExtention
{
    public static void Subscribe<T>(this Observable<T> observable, Action<T> subscribe)
    {
        observable.Subscribe(new Observer<T>(v => subscribe(v)));
    }

    public static Observable<U> Select<T, U>(this Observable<T> observable, Func<T, U> select)
    {
        return new Observable<U>(o =>
        {
            observable.Subscribe(v =>
            {
                o.OnNext(select(v));
            });
        });
    }

    public static Observable<T> Where<T>(this Observable<T> observable, Func<T, bool> where)
    {
        return new Observable<T>(o =>
        {
            observable.Subscribe(v =>
            {
                if (where(v))
                {
                    o.OnNext(v);
                }
            });
        });
    }
}

