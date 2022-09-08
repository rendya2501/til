# ICommand

---

## 概要

INotifyPropertyChangedをやったなら、ボタンを押したときの実装もするだろう。  
というわけで、原初のICommandを実装してみたが、クソ面倒くさかった。  
これをプレーンのままで実装するなんて考えられない。  
というわけで、Prismを使った実例までをまとめた。  

---

## Commandクラスに実装すべき内容

ICommandを実装することで強制される実装が3つもある。  
ボタンを押した時の処理を記述したいだけなのに、クラスを定義しないといけない。  
つまり、1つのコマンドを実装するためには、1つのクラスとICommandの実装が必要になるというわけ。  

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

[MVVM:とにかく適当なICommandを実装したい時のサンプル](https://running-cs.hatenablog.com/entry/2016/09/03/211015)  

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

まったくおすすめしないが、ライブラリ使わないとここまで大変というサンプル  
[WPFのMVVMでコマンドをバインディングする利点](https://takamints.hatenablog.jp/entry/why-using-commands-in-wpf-mvvm)  

---

## Prism

Prismはマイクロソフトが開発しているサポートライブラリ。  
nugetからインストールすることができる。  
マイクロソフト公式なのでサードパーティー製とかあまり気にせずに入れることができる。  

VisualStudioが新しいためかは知らないが、DelegateCommandって入力して[Ctrl + .]でおすすめを表示させると、Prismをインストールしてusingまで通してくれる選択肢が出てくる。  
簡単にインストールできてしかも軽いので、コマンドの実装をするなら使わない手はない。  
nugetから入れようとすると、まず開くので重くて、調べるので重くて、インストールがだるいという3重苦だが、ここまで軽くて簡単に入れられるならマストで入れるべきだ。  

Prismでの実装ならたった1行で済む。控えめに言って神。  

``` C# : Prismでの実装
using Prism.Commands;

    internal class ViewModel : INotifyPropertyChanged
    {
        public DelegateCommand DelegateCountDownCommand => new DelegateCommand(() => Count++);
    }
```

CanExecuteとかFunc\<T>とか実装したいならこのサイトを参考に実装すればいいと思うけど、ならDelegateCommand使えという話。  
[XAMLからViewModelのメソッドにバインドする～RelayCommand～](https://sourcechord.hatenablog.com/entry/2014/01/13/200039)  