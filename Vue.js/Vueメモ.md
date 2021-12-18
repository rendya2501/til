# Vueメモ

## テーブルを作る

[Vue.jsのv-forでTableを表示させてみた](https://neco913.kirara.st/post-12827/)  

[Gridの構築の仕方はここが参考になった](https://www.itra.co.jp/webmedia/what-is-inline-block.html)  
Borderをある程度の太さにして色を白くすればよかった。  
Borderをはる場所はtd。  

``` html
    <b-table-simple hover small caption-top responsive>
    <b-tbody>
        <b-tr v-for="item in items" :key="item.accountNo">
        <b-td
            variant="primary"
            style="margin-right: 20px;border: 2px solid #ffffff;"
            v-text="item.accountNo"
        />
        <b-td
            class="text-left"
            variant="primary"
            style="margin-right: 20px;border: 2px solid #ffffff;"
            v-text="item.name + '様'"
        />
        </b-tr>
    </b-tbody>
    </b-table-simple>
```

``` ts
  private items = [
    { accountNo: 111, name: 'カトウ　タカシ' },
    { accountNo: 333, name: 'キムラ　ユウイチ' },
    { accountNo: 555, name: 'クドウ　ケイ' },
    { accountNo: 777, name: 'サイトウ　ユズル' }
  ];
```

## バインドと文字列の連結方法

[Vueの基本構文をまとめてみた](https://qiita.com/_masa_u/items/7a940f1aea8be4eef4fe)  

``` html
v-text="item.name + '様'"
```
