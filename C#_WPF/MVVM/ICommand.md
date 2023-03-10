# ICommand

## 概要

INotifyPropertyChangedをやったなら、ボタンを押したときの実装もするだろう。  
というわけで、原初のICommandを実装してみたが、クソ面倒くさかった。  
これをプレーンのままで実装するなんて考えられない。  
というわけで、Prismを使った実例までをまとめた。  

---

## Commandクラスに実装すべき内容

ICommandを実装することで強制される実装が3つもある。  
ボタンを押した時の処理を記述したいだけなのに、クラスを定義しないといけない。  
つまり、1つのコマンドを実装するためには、対となるICommand実装クラスが必要となる。  

``` C# : Commandクラスに実装すべき内容
public class CountDownCommand : ICommand
{
    public event EventHandler CanExecuteChanged
    {
    }
 
    public bool CanExecute(object parameter)
    {
    }
 
    public void Execute(object parameter)
    {
    }
 }
```

---

## IComandの最小実装

``` C# : IComandの最小実装
public class ViewModel : INotifyPropertyChanged
{
    // ICommand で宣言すること
    public ICommand PlaneCountDownCommand => new RelayCommand(() => Count++);
}

/// <summary>
/// MVVMのViewModelから呼び出されるコマンドの定義
/// コマンド1つにつき1つのクラスを定義する
/// </summary>
public class CountDownCommand : ICommand
{
    /// <summary>
    /// Command実行時に実行するアクション
    /// 引数を受け取りたい場合はこのActionをAction<object>などにする
    /// </summary>
    private Action _action;

    /// <summary>
    /// コンストラクタ
    /// Actionを登録
    /// </summary>
    /// <param name="action"></param>
    public CountDownCommand(Action action) => _action = action;

    #region ICommandインターフェースの必須実装
    /// <summary>
    /// コマンドのルールとして必ず実装しておくイベントハンドラ
    /// 通常、このメソッドを丸ごとコピーすればOK
    /// RaiseCanExecuteChanged が呼び出されたときに生成されます。
    /// </summary>
    public event EventHandler? CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }

    /// <summary>
    /// コマンドの有効／無効を判定するメソッド
    /// コマンドのルールとして必ず実装しておくメソッド
    /// 有効／無効を制御する必要が無ければ、無条件にTrueを返しておく
    /// </summary>
    /// <param name="parameter"></param>
    /// <returns></returns>
    public bool CanExecute(object? parameter)
    {
        //とりあえずActionがあれば実行可能
        return _action != null;
    }

    /// <summary>
    /// コマンドの動作を定義するメソッド
    /// コマンドのルールとして必ず実装しておくメソッド
    /// </summary>
    /// <param name="parameter"></param>
    public void Execute(object? parameter)
    {
        //今回は引数を使わずActionを実行
        _action?.Invoke();
    }
    #endregion
}
```

[MVVM:とにかく適当なICommandを実装したい時のサンプル](https://running-cs.hatenablog.com/entry/2016/09/03/211015)  

---

## PrismでのCommand実装

Prismでの実装ならたった1行で済む。控えめに言って神。  

``` cs
using Prism.Commands;

internal class ViewModel : INotifyPropertyChanged
{
    public DelegateCommand DelegateCountDownCommand => new DelegateCommand(() => Count++);
}
```

VisualStudioが新しいためかは知らないが、DelegateCommandって入力して[Ctrl + .]でおすすめを表示させると、Prismをインストールしてusingまで通してくれる選択肢が出てくる。  
簡単にインストールできてしかも軽いので、コマンドの実装をするなら使わない手はない。  

nugetから入れようとすると、開く時点で重く、調べるのでも重く、インストールがだるいという3重苦だが、ここまで軽くて簡単に入れられるならマストで入れるべきだ。  

CanExecuteとかFunc\<T>とか実装したいならこのサイトを参考に実装すればいいと思うけど、ならDelegateCommand使えという話。  
[XAMLからViewModelのメソッドにバインドする～RelayCommand～](https://sourcechord.hatenablog.com/entry/2014/01/13/200039)  

---

## ヘルパークラス

``` cs
using System;
using System.Windows.Input;

namespace WPF_QuickStart.Common;

public class RelayCommand : ICommand
{
    private readonly Action _execute;
    private readonly Func<bool> _canExecute;

    /// <summary>
    /// RaiseCanExecuteChanged が呼び出されたときに生成されます。
    /// </summary>
    public event EventHandler CanExecuteChanged;

    /// <summary>
    /// 常に実行可能な新しいコマンドを作成します。
    /// </summary>
    /// <param name="execute">実行ロジック。</param>
    public RelayCommand(Action execute)
        : this(execute, null)
    {
    }

    /// <summary>
    /// 新しいコマンドを作成します。
    /// </summary>
    /// <param name="execute">実行ロジック。</param>
    /// <param name="canExecute">実行ステータス ロジック。</param>
    public RelayCommand(Action execute, Func<bool> canExecute)
    {
        if (execute == null)
            throw new ArgumentNullException("execute");
        _execute = execute;
        _canExecute = canExecute;
    }


    /// <summary>
    /// 現在の状態でこの <see cref="RelayCommand"/> が実行できるかどうかを判定します。
    /// </summary>
    /// <param name="parameter">
    /// コマンドによって使用されるデータ。コマンドが、データの引き渡しを必要としない場合、このオブジェクトを null に設定できます。
    /// </param>
    /// <returns>このコマンドが実行可能な場合は true、それ以外の場合は false。</returns>
    public bool CanExecute(object parameter)
    {
        return _canExecute == null ? true : _canExecute();
    }

    /// <summary>
    /// 現在のコマンド ターゲットに対して <see cref="RelayCommand"/> を実行します。
    /// </summary>
    /// <param name="parameter">
    /// コマンドによって使用されるデータ。コマンドが、データの引き渡しを必要としない場合、このオブジェクトを null に設定できます。
    /// </param>
    public void Execute(object parameter)
    {
        _execute();
    }

    /// <summary>
    /// <see cref="CanExecuteChanged"/> イベントを発生させるために使用されるメソッド
    /// <see cref="CanExecute"/> の戻り値を表すために
    /// メソッドが変更されました。
    /// </summary>
    public void RaiseCanExcuteChanged()
    {
        var handler = CanExecuteChanged;
        if (handler != null)
        {
            handler(this, EventArgs.Empty);
        }
    }
}
```

[第6回　「コマンド」と「MVVMパターン」を理解する：連載：WPF入門（3/3 ページ） - ＠IT](https://atmarkit.itmedia.co.jp/ait/articles/1011/09/news102_3.html)  
[XAMLからViewModelのメソッドにバインドする～RelayCommand～ - SourceChord](https://sourcechord.hatenablog.com/entry/2014/01/13/200039)  

---

まったくおすすめしないが、ライブラリ使わないとここまで大変というサンプル  
[WPFのMVVMでコマンドをバインディングする利点](https://takamints.hatenablog.jp/entry/why-using-commands-in-wpf-mvvm)  
