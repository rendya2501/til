# SelectMany

selectだと配列の中の配列があったときに、`IEnumerable<IEnumerable<array>>`ってなっちゃうから、そういう場合はSelectManyってのを使って余計な入れ子をなくそうねってサンプル。  
実際に実務で使うまでは便利さがわからなかったけど、使ってみて完全に理解した。  

■ **Foreachで頑張った場合**

こちらの場合、変数を複数用意しないといけないのでまとまりがない。  
最も単純でわかりやすいが、野暮ったい感じは否めない。  

``` cs
var list = this.GetList(condition);
List<Slip> SlipList = new List<Slip>();
foreach (var item in list)
{
    var slipList = SlipModel.GetList(new Condition(){ID = item.ID});
    foreach (var slip in slipList)
    {
        slip.SettlementID = slip.ID;
        slip.UpdateDateTime = DateTime.Now;
        SlipList.Add(slip);
    }
}
```

■ **SelectManyの場合**

変数なんて用意する必要はない。とてもすっきりしてまとまっている。Linq最高。  
Selectだと`IEnumerable<IEnumerable<class>>`になるけど、  
SelectManyすることで`IEnumerable<class>`にすることができる。  
平坦化ってやつ。  

``` cs
var SlipList = this.GetList(condition)
    .SelectMany(item => 
        Model.GetList(new Condition(){ID = item.SettelementID})
            .Select(slip =>
            {
                slip.SettlementID = slip.ID;
                slip.UpdateDateTime = DateTime.Now;
                return slip;
            })
    );
```

<https://www.urablog.xyz/entry/2018/05/28/070000>  
