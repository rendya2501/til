# XAML_Column

---

## XAMLでBoolを反転するには？

どう頑張っても、普通にやるならConverterが必要らしい。  
が、こんなことのためにあんな面倒くさいConverterを噛まさなければいけないのは絶対にいやだ。  

[bool を反転して Binding したかっただけなんだ](https://usagi.hatenablog.jp/entry/2018/12/05/211311)  

``` XML
<!-- なぜこれがデフォルトで出来ないのか -->
<CheckBox IsChecked="{Binding Awabi}"/>
<CheckBox IsEnabled="{Binding !Awabi}"/>
```

世の中には便利なライブラリがあるらしい。  
[Alex141/CalcBinding](https://github.com/Alex141/CalcBinding)  
Manage NuGet Packages から CalcBindingをインストールしてWindowに参照を追加することでほぼ理想的な書き方が実現できる。  

``` XML
<Window 
    xmlns:c="clr-namespace:CalcBinding;assembly=CalcBinding">

    <!-- かなり理想郷に近いぞ！！！ -->
    <CheckBox IsChecked="{Binding Awabi}"/>
    <CheckBox IsEnabled="{c:Binding !Awabi}"/>
</Window>
```

---

## 行番号をつけたい

DataGridにはAlternationIndexなるプロパティがあるのでいいけど、FlexGridのはないのでおとなしくCellFactory使うしかないみたい。  
AlternationIndexで見事解決かと思ったが、Indexなので0から始まる。  
バインドするついでに+1くらいできないかなと思ったが、できない模様。  
それくらい何とかしてくれと思うのだが・・・。  
素直にConverterを使えと行ってくる。  
はぁ  

wpf binding add two values

``` xml
<DataGrid
    AlternationCount="{Binding Items.Count, RelativeSource={RelativeSource Self}}">
    <DataGrid.Columns>
        <DataGridTextColumn Width="10" Binding="{Binding AlternationIndex, RelativeSource={RelativeSource AncestorType=DataGridRow}, StringFormat=その1{0}をつかう}" />
    </DataGrid.Columns>
</DataGrid>
```

[Simple way to display row numbers on WPF DataGrid](https://stackoverflow.com/questions/4661998/simple-way-to-display-row-numbers-on-wpf-datagrid)  
[DataGridに行番号を表示しよう](http://tawamuredays.blog.fc2.com/blog-entry-75.html)  
