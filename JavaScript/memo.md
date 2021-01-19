# js色々

## javascriptにおけるIsNullOrEmpty

直接判定してくれる関数は存在しないが判定する方法はいくらでもある模様。  

[Stack雑談](https://stackoverflow.com/questions/154059/how-can-i-check-for-an-empty-undefined-null-string-in-javascript)  
[JavascriptにIsNullOrEmptyは必要ない](http://jmqys.hatenablog.com/entry/2015/06/21/223005)

```js
// 値があるかどうかを確認したいだけの場合
if (strValue) {
    //do something
}

// nullを超える空の文字列を具体的にチェックする必要がある場合、
// [""]は、[===]演算子を使用してチェックするのが最善の策だと思います。
//（実際には、比較対象の文字列であることがわかります）。  
if (strValue === "") {
    //...
}

// 型キャストを使用する  
if (Boolean(strValue)) {
    // Code here
}

// str.lengthではstrがnullの場合機能しない。
// !!str.trim()も同様。
```

---

## 配列をカンマ区切りの文字列に変換する方法

地味によく使う機能だから覚えておきたい。  

```js
let badPlansName = this.gdoData.GDOGotPlanMasterInfoList
    .filter(f => f.hoge > 0)
    .map(m => m.PlanMasterName)
    .join(',');
```
