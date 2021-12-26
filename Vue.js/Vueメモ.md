# Vueメモ

[Visual Studio CodeでVue.jsアプリケーションの開発環境を構築する](https://qiita.com/rubytomato@github/items/b35b819671e7cbb3dff7)  

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

``` html
<button 
  v-bind:disabled="isProcessing"
  @click="submit">何かしら処理する
</button>
```

## TypeScriptのオブジェクトの初期化

C#みたいに `new Object(){a = huga,b = hoge}` ってできないか調べた。  
[TypeScriptのclassをオブジェクトで初期化する](https://qiita.com/Tsuyoshi84/items/e74109e2ccc0f4e625aa)  

## TypeScriptのオーバーロード

[TypeScript: オーバーロードメソッドを定義する方法](https://qiita.com/suin/items/7d6837a0342b36891099)  

## typescript json to class

[TypeScriptのリフレクションでJSONの型変換を自動化する](https://qiita.com/bitrinjani/items/d60bdac10e5ced126d1a)  

## Veturの設定

Vetur.config.jsがないことが最大の原因だった。
後は設定内容を正しく設定してあげれば機能する。

[vscodeのサブディレクトリのプロジェクトでveturが機能しないときの対処法](https://zukucode.com/2021/02/vscode-vetur-subdirectory.html)  
[VSCODE上でVeturの警告が出ていたので解決した](https://snyt45.com/posts/20210531/vscode-vetur/)  

[.Vetur.config.js]ではない。[Vetur.config.js]です。
先頭に[.]はつかない。
rootフォルダ直下に置くので、今回ならWebContentに配置する。

``` js : うまくいった内容
// vetur.config.js
/** @type {import('vls').VeturConfig} */
module.exports = {
    // **optional** default: `{}`
    // override vscode settings
    // Notice: It only affects the settings used by Vetur.
    settings: {
      "vetur.useWorkspaceDependencies": true,
      "vetur.experimental.templateInterpolationService": true
    },
    // **optional** default: `[{ root: './' }]`
    // support monorepos
    projects: [
      {
        // **required**
        // Where is your project?
        // It is relative to `vetur.config.js`.
        root: './SelfOrder', // ←ここをvueプロジェクトのフォルダに合わせる
        // **optional** default: `'package.json'`
        // Where is `package.json` in the project?
        // We use it to determine the version of vue.
        // It is relative to root property.
        package: './package.json',
        // **optional**
        // Where is TypeScript config file in the project?
        // It is relative to root property.
        tsconfig: './tsconfig.json',
        // **optional** default: `'./.vscode/vetur/snippets'`
        // Where is vetur custom snippets folders?
        snippetFolder: './.vscode/vetur/snippets',
        // **optional** default: `[]`
        // Register globally Vue component glob.
        // If you set it, you can get completion by that components.
        // It is relative to root property.
        // Notice: It won't actually do it. You need to use `require.context` or `Vue.component`
        globalComponents: []
      }
    ]
  }
```

---

## byte配列を画像に変換する

javascript byte array to image  
[How to convert a byte array into an image?](https://www.titanwolf.org/Network/q/2fbc7635-d37c-4f51-a627-d261be5a79a6/y)  

---

## 宣言の仕方

``` ts
  private selectedItem?: {
    category: string;
    idx: string;
    name: string;
    price: number;
  } | null = { category: '', idx: '', name: '', price: 0 };
```

---

## Vueの色々

<https://qiita.com/i-ryo/items/baa50cf0a6647fe8bd2e>  
<https://zenn.dev/koduki/articles/0fe6cc5ada58e5600f75>  

---

## Vueのパラメータの受け渡し方法

[【Vue】Vue Router ～ パラメータの受け渡し ～](https://dk521123.hatenablog.com/entry/2021/03/02/001653)  

---

## とりあえず入門

[簡単な例で始めるVue3でTypeScript入門](https://reffect.co.jp/vue/vue3-typescript)  

---

## TypeScript Enum

[「なぜ enum の利用が推奨されないのか？」をまとめてみた](https://qiita.com/saba_can00/items/696baa5337eb10c37342)  

---

## 役立ちそうな情報

[How to to use `v-for` with Bootstrap-Vue's `b-col` and `b-row`?](https://stackoverflow.com/questions/63960450/how-to-to-use-v-for-with-bootstrap-vues-b-col-and-b-row)  

---

## Gitの変更の色が反映されない問題

参照先の違いだった。  

`C:\Develop\s.ito\RN3.WebContent`ならOK。  
`C:\VSSTEMP\s.ito\RN3.WebContent`だとNG。  
そういうあれなのか？なんかそういう設定あるのだろうか。  

まぁ、何はともあれ、分からなかったことが分かってよかった。  
この線で調べれば何か文献が出てくるだろう。  

---

## TypeScriptで "Object is possibly null" と怒られたときにすること

<https://qiita.com/fufufukakaka/items/5d4a2f2272b8f1a4a16f>  

---

## Vueのテンプレート構文はJavaScriptしかサポートしてない

ということなので、三項演算子はいいけど、null合体演算子`?.`を使うと構文エラーになる。  

オプショナルチェイニング演算子 (?.) との関係  
Null 合体演算子は、null と undefined を特定の値として扱いますが、オプショナルチェイニング演算子 (?.)  
<https://developer.mozilla.org/ja/docs/Web/JavaScript/Reference/Operators/Nullish_coalescing_operator>  

``` html
  <label class="lbl-account-total">
    ○小計&nbsp;{{ this.total ? this.total.price.toLocaleString() : '' }}円
    ×小計&nbsp;{{ this.total?.price.toLocaleString() ?? '' }}円
  </label>
```

---

## Vue $refs

[[Vue.js] $refsでコンポーネント内の子要素を触る](https://qiita.com/1994spagetian/items/5f372fc68122ec207c78)  

---

## Vue @Click 三項演算子

[If statement inside Vue click to change function to be run?](https://stackoverflow.com/questions/43698274/if-statement-inside-vue-click-to-change-function-to-be-run)  

``` txt
@click="isAlone ? makeOrder() : confirmOrder()"
```

thisがないと行けた。

---

## localStorageの強制クリア

[Clearing localStorage in javascript?](https://stackoverflow.com/questions/7667958/clearing-localstorage-in-javascript)  

消さないといけないので調べたら物凄く単純だった。
というか、ローカルストレージって本当に単純にキーバリューで管理してるだけなんだな。

---

## 端末情報を取得できる限り取得したい

[使用してるブラウザを判定したい](https://qiita.com/sakuraya/items/33f93e19438d0694a91d)  

``` js

    var userAgent = window.navigator.userAgent.toLowerCase();

    if (userAgent.indexOf('iphone') != -1) {
      console.log('iPhoneをお使いですね');
    } else if (userAgent.indexOf('ipad') != -1) {
      console.log('iPadをお使いですね');
    } else if (userAgent.indexOf('android') != -1) {
      if (userAgent.indexOf('mobile') != -1) {
        console.log('androidのスマホをお使いですね');
      } else {
        console.log('androidのタブレットをお使いですね');
      }
      //OSの種類
      console.log(navigator.platform);
    }

```

---

## TypeScript 初期化方法

[TypeScriptのclassをオブジェクトで初期化する](https://qiita.com/Tsuyoshi84/items/e74109e2ccc0f4e625aa)  

``` ts
class Person {
  name?: string;
  age?: number;

  constructor(init?: Partial<Person>) {
    Object.assign(this, init);
  }
}
```

---

## Linq.Sum

[[TypeScript]配列の要素の合計値を計算する](https://codelab.website/typescript-reduce/)  

``` ts
const data = [
  {num: 1}, {num: 2}, {num: 3}, {num: 4}, {num: 5},
  {num: 6}, {num: 7}, {num: 8}, {num: 9}, {num: 10},
];

const result = data.reduce(function(a, x){return a + x.num;}, 0);

console.log(result);

```

<https://decembersoft.com/posts/typescript-vs-csharp-linq/>  
