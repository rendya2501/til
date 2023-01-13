# ObservableCollection

---

## 概要

>項目が追加または削除されたとき、あるいはリスト全体が更新されたときに通知を行う動的なデータ コレクションを表します。  
[マイクロソフト公式](https://docs.microsoft.com/ja-jp/dotnet/api/system.collections.objectmodel.observablecollection-1?view=netcore-3.1)  

<!--  -->
>コレクションのアイテムに対する、追加、削除、変更、移動操作があった場合、またはリスト全体が更新されたとき、CollectionChanged イベントを発生させることができるコレクションです。  
>「Observable」という名前がついていますが、IObservable\<T> や IObserver\<T> とは直接の関連はありません。  
>むしろ、INotifyPropertyChanged に近いイメージです。  
>ObservableCollection\<T> は INotifyPropertyChanged も実装していますが、そのイベントを直接購読することはできないようになっています。
[意外と知らない！？ C#の便利なコレクション！](https://qiita.com/hiki_neet_p/items/75bf39838ce580cca92d)  

→  
追加や削除した時にイベントを発生させるので、追加、削除した時に何かやりたい時はObservableCollectionを使う必要がある。  

---

## 実装

``` C# : 実装例
public class ViewModel
{
    /// <summary>
    /// コレクション本体
    /// </summary>
    public ObservableCollection<Person> People { get; set; } 
        = new ObservableCollection<Person>(Enumerable.Range(1, 10).Select(x => new Person { Name = "tanaka" + x, Age = x }));
    /// <summary>
    /// コレクションに要素を追加する
    /// </summary>
    public DelegateCommand AddItem 
        => new DelegateCommand(() => People.Add(new Person { Name = "追加したtanaka", Age = People.Count + 1 }));
    /// <summary>
    /// コレクションの要素を削除する。
    /// </summary>
    public DelegateCommand RemoveItem 
        => new DelegateCommand(() => People.RemoveAt(People.Count - 1 ));

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public ViewModel()
    {
        // ObservableCollectionにイベント登録
        People.CollectionChanged += People_CollectionChanged;
    }

    /// <summary>
    /// ObservableCollectionのAddやRemoveされた時実行される処理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void People_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        Console.WriteLine("Action for this event: {0}", e.Action);

        switch (e.Action)
        {
            // They added something. 
            case NotifyCollectionChangedAction.Add:
                // Now show the NEW items that were inserted.
                Console.WriteLine("Here are the NEW items:");
                foreach (Person p in e.NewItems)
                {
                    Console.WriteLine(p.ToString());
                }
                break;
            // They removed something. 
            case NotifyCollectionChangedAction.Remove:
                Console.WriteLine("Here are the OLD items:");
                foreach (Person p in e.OldItems)
                {
                    Console.WriteLine(p.ToString());
                }
                break;
            case NotifyCollectionChangedAction.Replace:
            case NotifyCollectionChangedAction.Move:
            case NotifyCollectionChangedAction.Reset:
            default:
                break;
        }

        Console.WriteLine();
    }
}
```

``` XML
<Window>
    <Window.DataContext>
        <local:ViewModel />
    </Window.DataContext>
    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto" />
            <RowDefinition Height="*" />
        </Grid.RowDefinitions>
        <Menu Grid.Row="0">
            <MenuItem Command="{Binding AddItem}" Header="追加" />
            <MenuItem Command="{Binding RemoveItem}" Header="削除" />
        </Menu>
        <ListBox Grid.Row="1" ItemsSource="{Binding People}">
            <ListBox.ItemTemplate>
                <DataTemplate>
                    <StackPanel Orientation="Horizontal">
                        <TextBox Text="{Binding Name}" />
                        <TextBox Text="{Binding Age}" />
                    </StackPanel>
                </DataTemplate>
            </ListBox.ItemTemplate>
        </ListBox>
    </Grid>
</Window>
```
