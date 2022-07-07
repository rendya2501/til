# Grid

---

## Gridの行列の幅や高さの定義

[C#WPFの道#3！Gridの使い方をわかりやすく解説！](https://anderson02.com/cs/wpf/wpf-3/)  
GridのWidthやHeightに[*]や[Auto]を指定した時の動作が曖昧だったのでまとめ。  

### 数値

絶対値でピクセルを指定します。  

### *

均等な比率を割り当てます。  

例)  
3列宣言していて、すべての列の定義が「＊」の場合は、均等に3列の幅が作られます。  
「２＊」などと、数字を＊の前に記述すると、その列のみ2倍などの指定した値の倍率で確保されます。  

### Auto

設置したコントロールの幅で動的に変動します。  
おそらく、内部のコントロールが動的に変化する場合、Gridの大きさ同じように変化すると思われる。  

---

## 左右に分けて配置するテク

結構需要はあるのだが、毎回忘れるのでメモすることにした。  

``` XML
    <Grid Grid.Row="1">
        <!-- 左のまとまり -->
        <StackPanel HorizontalAlignment="Left" Orientation="Horizontal">
            <!-- 内容 -->
        </StackPanel>
        
        <!-- 右のまとまり -->
        <StackPanel HorizontalAlignment="Right" Orientation="Horizontal">
            <!-- 内容 -->
        </StackPanel>
    </Grid>
```
