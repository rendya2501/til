# Repeat

## 指定の要素数で初期化されたListを作成する方法

検索処理が複雑だったので、1件の時はいつも通りにするけど、2件以上あったら、適当に2件作って、
フロント側で判断してごにょごにょするって感じで作った。  

``` C#
// List<Product> Count() = 2にする
Enumerable.Repeat(new Product(), 2).ToList();
```

[C#の配列を同じ値で初期化する](https://www.paveway.info/entry/2019/07/**15_csharp_initarray**)  
