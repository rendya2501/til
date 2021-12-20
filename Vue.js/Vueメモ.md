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

---

## vue.jsのcreatedとmountedの違いを目で見て理解

[vue.jsのcreatedとmountedの違いを目で見て理解](https://reffect.co.jp/vue/vue-js-created-mounted-diffrence)  

---

## CORSエラーをaxios proxyで回避した方法

asp net core cors
ASP.Net側もいじって見たけど、「vue axios cors」で調べたらproxyを使えばいいってあったのでやってみた。

[Vue.jsとAPIサーバとのaxiosでCORSに引っかかった時のProxyを使った回避方法](https://qiita.com/Ryoma0413/items/c41d10d2e6e2a420c3cf)  
[Vue 3 + TypeScript + axiosでAPI接続](https://qiita.com/Esfahan/items/1b41b64d0a605732a0dd)  

CORS(Cross-Origin Resource Sharing)  
オリジン間リソース共有  
[なんとなく CORS がわかる...はもう終わりにする。](https://qiita.com/att55/items/2154a8aad8bf1409db2b)  
[CORSエラーが出てしまったらヘッダー情報を追加しよう](https://qiita.com/mtoutside/items/cee708841cad7e02f85c)  
[CORSの仕組みをGIFアニメで分かりやすく解説](https://coliss.com/articles/build-websites/operation/work/cs-visualized-cors.html)  

[corsに悩まされるな。axios でcorsを攻略する](https://qiita.com/inatatsu_csg/items/15f63be00096ec21535e)  
[.NET Core3.1でCORSの設定をする](https://qiita.com/SuyamaDaichi/items/2769c962097aacd5835d)  

---

## aa

[Vue.jsで処理中はボタンを無効にする](https://qiita.com/reflet/items/8337b17fb727364328d1)  
