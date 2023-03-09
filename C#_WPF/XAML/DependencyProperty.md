# DependencyProperty (依存関係プロパティ)

## 概要

DependencyProperty→バインディングができるようにする。  
コントロールにそういう機能がある。  

バインディングをしたときに、バインド間で通知をしてくれる。  
この処理は.Net側の処理なので、その先で何をやっているかはわからない。  

[【WPF】依存関係プロパティでユーザーコントロールをバインド対応する！](https://resanaplaza.com/%E3%80%90wpf%E3%80%91%E4%BE%9D%E5%AD%98%E9%96%A2%E4%BF%82%E3%83%97%E3%83%AD%E3%83%91%E3%83%86%E3%82%A3%E3%81%A7%E3%83%A6%E3%83%BC%E3%82%B6%E3%83%BC%E3%82%B3%E3%83%B3%E3%83%88%E3%83%AD%E3%83%BC/)

---

## 依存プロパティをバインドする方法

[Binding to custom dependency property](https://stackoverflow.com/questions/13956767/binding-to-custom-dependency-property-again)  

マルチセレクトコンボボックス開発の時に直面。  
全選択コンボボックスのcontextを設定できたほうが便利だと思って依存プロパティを作ったはいいものの、素直にバインドできなかったのでまとめ。  
ただバインドさせるだけでは警告が出てうまく動作しない。

結論としてはRelativeSourceで自分自身のコントロールの親を指定する必要がある。  
Windowに乗っているならAncestorTypeはWindowだし、UsercontrolならAncestorTypeはUserControl。  

``` XML
<!-- SelectAllContentが依存プロパティとして外部公開し、実行時に反映する値 -->
<CheckBox
    Margin="5,1,0,1"
    HorizontalAlignment="Left"
    VerticalContentAlignment="Center"
    Content="{Binding SelectAllContent, RelativeSource={RelativeSource FindAncestor, AncestorType=UserControl}}"
    IsChecked="{x:Null}"
    IsThreeState="True" />
```

``` C#
    public static readonly DependencyProperty SelectAllContentProperty = DependencyProperty.Register(
        "SelectAllContent",
        typeof(string),
        typeof(MultiSelectComboBox1),
        new UIPropertyMetadata("Select All")
    );
    public string SelectAllContent
    {
        get { return (string)GetValue(SelectAllContentProperty); }
        set { SetValue(SelectAllContentProperty, value); }
    }
```

---

## DependencyProperyのSetterの値がNullになってしまう問題

[Why does my Dependency Property send null to my view model?](https://stackoverflow.com/questions/38958177/why-does-my-dependency-property-send-null-to-my-view-model)  
[MVVMのDataGridまたはListBoxからSelectedItemsにバインド](https://www.webdevqa.jp.net/ja/c%23/mvvm%E3%81%AEdatagrid%E3%81%BE%E3%81%9F%E3%81%AFlistbox%E3%81%8B%E3%82%89selecteditems%E3%81%AB%E3%83%90%E3%82%A4%E3%83%B3%E3%83%89/942024865/amp/)  

MultiSelectComboBoxのSelectedItemsの2WayBindingを実装している時に出くわした問題。  
コントロール側はIList,ViewModel側は`IEnumerable<T>`で実装していたのだが、コントロール側のSetterまでは値が入っているのに、ViewModelのSetterにはNullが入ってしまう現象に遭遇した。  
コントロール側の型とViewModel側の型を合わせれば値は届くが、2Wayにしたい以上、コントロールはIList,ViewModelはIEnumerableで受け取りたい願望がある。  
その線でいろいろ探してみたが、どうやら無理らしい。  

XAMLはジェネリックのバインドをサポートしてないっぽい。  
なので非ジェネリックIListとIListを同じ意味で使用はできない模様。  
どのサンプルでもObservableCollectionを使っていたり、イベントやビヘイビアで実現している。  
現状では、ジェネリックのバインディングは実現できない模様。  
実質的には疑似的な2wayどまりで、ViewModelからの通知はOneWayになってしまうだろう。  

対処法  
1.コントロールの型とViewModelの型を合わせる→IListで受け取ってOfTypeで変換して使う。  
2.ObservableCollectionのCollectionChangedイベントを観測する。  
3.イベントやビヘイビアで観測する。  

いろいろな実現方法もまとめておく  
[ListBoxやDataGridなどのItemsControlでSelectedItemsやIsSelectedをBindingする](https://qiita.com/mkuwan/items/7372b4b602fdabc3358c)  
