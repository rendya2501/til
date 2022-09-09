# Grid

---

## Gridの行列の幅や高さの定義

GridのWidthやHeightに[*]や[Auto]を指定した時の動作が曖昧だったのでまとめ。  
[C#WPFの道#3！Gridの使い方をわかりやすく解説！](https://anderson02.com/cs/wpf/wpf-3/)  

``` xml : 列定義
    <Grid.ColumnDefinitions>
        <ColumnDefinition Width="123"/>
        <ColumnDefinition Width="Auto"/>
        <ColumnDefinition Width="*"/>
        <ColumnDefinition Width="2*"/>
    </Grid.ColumnDefinitions>
```

### 数値

絶対値でピクセルを指定。  

### *

均等な比率を割り当てる。  

例)  
3列宣言していて、すべての列の定義が「＊」の場合は、均等に3列の幅が作られます。  
「2＊」などと、数字を＊の前に記述すると、その列のみ2倍などの指定した値の倍率で確保されます。  

### Auto

設置したコントロールの幅で動的に変動する。  

おそらく、内部のコントロールが動的に変化する場合、Gridの大きさ同じように変化すると思われる。  
