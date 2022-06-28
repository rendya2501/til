
# INotifyPropertyChanged

[WPF4.5入門 その60「データバインディングを前提としたプログラミングモデル」](https://blog.okazuki.jp/entry/2014/12/23/180413)  
>INotifyPropertyChangedインターフェースはPropertyChangedイベントのみをもつシンプルなインターフェースです。  
>このイベントを通じてModelからViewModel、ViewModelからViewへの変更通知が行われます。  

[世界で一番短いサンプルで覚えるMVVM入門](https://resanaplaza.com/%E4%B8%96%E7%95%8C%E3%81%A7%E4%B8%80%E7%95%AA%E7%9F%AD%E3%81%84%E3%82%B5%E3%83%B3%E3%83%97%E3%83%AB%E3%81%A7%E8%A6%9A%E3%81%88%E3%82%8Bmvvm%E5%85%A5%E9%96%80/)  

``` C# : INotifyPropertyChangedの最小実装
    // 1. INotifyPropertyChangedの継承
    internal class ViewModel : INotifyPropertyChanged
    {
        // 2. PropertyChangedEventHandlerイベントハンドラの記述
        /// <summary>
        /// INotifyPropertyChangedインターフェース実装イベント
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// カウント数
        /// </summary>
        public int Count
        {
            get { return _Count; }
            set
            {
                _Count = value;
                // 3. プロパティの set 内でのPropertyChangedEventHandlerイベントハンドラの呼び出し
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
            }
        }
        private int _Count;
    }
```

---

## ヘルパークラス

INotifyPropertyChangedインターフェースの実装をすべてのプロパティに実装するのは負荷が高いため、一般的に以下のようなヘルパークラスが作成されます。  

→  
これが実務でも見るViewModelBaseに記述されてるあれになるわけだ。  

``` C# : INotifyPropertyChangedのヘルパークラス(BindableBase)
using System.ComponentModel;
using System.Runtime.CompilerServices;

    /// <summary>
    /// INotifyPropertyChangedのヘルパークラス
    /// </summary>
    public class BindableBase : INotifyPropertyChanged
    {
        /// <summary>
        /// INotifyPropertyChangedインターフェース実装イベント
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 値のセットと通知を行う
        /// </summary>
        protected virtual bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            // 同じ値なら処理しない
            if (Equals(field, value))
            {
                return false;
            }
            // 値を反映
            field = value;
            // プロパティ発火
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            // 正常に終了したことを通知
            return true;
        }
    }
```

``` C# : BindableBaseの実装例
    public class ViewModel : BindableBase
    {

        /// <summary>
        /// カウント数
        /// </summary>
        public int Count
        {
            get { return _Count; }
            set { SetProperty(ref _Count, value); }
        }
        private int _Count;
    }
```
