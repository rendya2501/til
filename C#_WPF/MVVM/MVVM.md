# MVVMパターン

>プログラムの構成をView(見た目),ViewModel(見た目とデータの制御),Model(データの制御)に分けた物。  
基本的に、View,ViewModel間のやり取りはプロパティのBindingを行う。  
プロパティ変更通知はINotifyPropertyChangedインターフェースを使用するが、Prismは便利なサポート  
[WPF4.5入門 その60「データバインディングを前提としたプログラミングモデル」 - かずきのBlog@hatena](https://blog.okazuki.jp/entry/2014/12/23/180413)  
