# C#数値計算

---

## decimalに1をかけて意味があるのか？

2021/11/13 Sat  

おもむろに1を掛けているコードを発見した。  
意味があるようには見えなかったので、聞いてみたが、どうやら数量を掛けているつもりだったらしい。  
しかし、小数点があったら四捨五入されるなんて言っていたが、本当だろうかと確かめてみた。  
されなかった。  
やっぱり意味がなかった。  

``` C#
    decimal? test = (decimal?)Math.PI;
    test = test * 1;
    test = (decimal?)2.5252525252;
    test = test * 1;
```

---

## 演算の優先順位の確認

decimal?型にnullが入って来た場合、null合体演算子と反転処理は同居させてもエラーにならないか気になったので調査。  
`-1 × null` でエラーになりそうに見える。  
なのでかっこでくくって演算の優先順位を決めてあげたほうがよさそうに見えた。  
→`-1 × (aa ?? decimal.zero)`  

結果はかっこで括らなくても「0」と表示されたので、内部的にちゃんと評価されている模様。  

後日判明したが、そもそもnullとの演算は全てnullになるので、`-1 * null`がエラーになるという視点はちょっと残念だった。  

``` C#
    decimal? aa = null;
    // -(aa ?? decimal.zero)と括って上げたほうがよさそうに見える。
    var aac = -aa ?? decimal.Zero;
    // 「0」と表示されたので内部的に評価されている模様。
    System.Console.WriteLine(aac);

    // 後日これを実行してみたらaacはnullになったので、
    // -aa ?? decimal.Zero は -1 * (aa ?? decimal.zero) ではなく (null ?? decimal.zero)の状態になっていたことが分かった。
    var aac = -aa;

    // これがnullになるので、そもそもnullとの演算は全てnullになる模様。
    var aac = -1 * aa;
```

タプルで先頭の要素に入れた値を後の要素で使えないか気になったので調査。  
ダメだった。  
同時に値を入れるので、先頭とか関係ない模様。  

``` C#
    // Main.cs(10,20): error CS0103: The name `Disp' does not exist in the current context
    var www = (
        Disp: aa,
        Calc: -Disp
    );    
    System.Console.WriteLine(www.Calc);
```
