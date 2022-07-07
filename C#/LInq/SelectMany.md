# SelectMany

<https://www.urablog.xyz/entry/2018/05/28/070000>  

selectだと配列の中の配列があったときに、`IEnumerable<IEnumerable<array>>`ってなっちゃうから、  
そういう場合はSelectManyってのを使って余計な入れ子をなくそうねってサンプル。  
実際に実務で使うまでは便利さがわからなかったけど、使ってみて完全に理解した。  

``` C# : Foreachで頑張った場合
    // こちらの場合、変数を複数用意しないといけないのでまとまりがない。
    // 最も単純でわかりやすいが、野暮ったい感じは否めない。
    var SettlementSettingPlayerViewList = GetSettlementSettingPlayerViewList(data.SettlementSetting);
    List<TFr_Slip> SlipList = new List<TFr_Slip>();
    foreach (var settingPlayer in SettlementSettingPlayerViewList)
    {
        var slipList = _TFr_SlipModel.GetList(
            new PlayerCondition()
            {
                PlayerNo = settingPlayer.SettelementPlayerNo
            }
        );
        foreach (var slip in slipList)
        {
            slip.SettlementPlayerNo = slip.PlayerNo;
            slip.UpdateDateTime = DateTime.Now;
            SlipList.Add(slip);
        }
    }
```

``` C# : SelectManyの場合
    // 
    // 変数なんて用意する必要はない。とてもすっきりしてまとまっている。Linq最高。
    // SelectだとIEnumerable<IEnumerable<TFr_Slip>>になるけど、
    // SelectManyすることでIEnumerable<TFr_Slip>にすることができる。平坦化ってやつ。
    var SlipList = GetSettlementSettingPlayerViewList(data.SettlementSetting)
        .SelectMany(settingPlayer => 
            _TFr_SlipModel
                .GetList(new PlayerCondition(){PlayerNo = settingPlayer.SettelementPlayerNo})
                .Select(slip =>
                {
                    slip.SettlementPlayerNo = slip.PlayerNo;
                    slip.UpdateDateTime = DateTime.Now;
                    return slip;
                })
        );
```
