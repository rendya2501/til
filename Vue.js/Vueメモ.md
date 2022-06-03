# Vueメモ

Vue
[Visual Studio CodeでVue.jsアプリケーションの開発環境を構築する](https://qiita.com/rubytomato@github/items/b35b819671e7cbb3dff7)  
[はじめてのvue-property-decorator (nuxtにも対応）](https://qiita.com/simochee/items/e5b77af4aa36bd0f32e5)  

Vuex
[【Vuex】stateの変更を監視する3つの方法](https://kawadev.net/vuex-watch/)  
[Vuexのstate/getters/mutations/actionsについて調べた](https://shkn.hatenablog.com/entry/2019/05/29/020223)  
[Vuex の使い方を勉強してみた](https://qiita.com/nmgw1119/items/27ed23e433ec54a9c950)  

QRコード関連  
[JavaScript で QRコードのデコード：jsQR で簡素な処理（日本語の扱いでハマった部分についても記載）](https://qiita.com/youtoy/items/d94a5bf835d3f4007c81)  
[requestAnimationFrameの仕組みと使い方！うまく使ってパフォーマンスを改善しよう！](https://leap-in.com/ja/use-requestanimtionframe/)  
[jsQRであっさりQRコードリーダ/メーカ](https://zenn.dev/sdkfz181tiger/articles/096dfb74d485db)  

TypeScript
[TypeScriptの型入門](https://qiita.com/uhyo/items/e2fdef2d3236b9bfe74a)  
[TypeScriptを雰囲気で使っていたので勉強し直したら知らなかったこと](https://zenn.dev/nrikiji/articles/45721c42b48124)  
[JavaScriptの数値チェックについてハマったのでメモ](https://qiita.com/taku-0728/items/329e0bee1c49b7ce7cd1)  
[[JavaScript] 配列の存在チェック（空判定）は if (array.length) {...} でいいよって話](https://qiita.com/kozzzz/items/42108bc96b3a0e7c53d5)  
[[JavaScript] null とか undefined とか 0 とか 空文字('') とか false とかの判定について](https://qiita.com/phi/items/723aa59851b0716a87e3)

見た目
[スマホで100vhを使うと、アドレスバーの高さ分はみでてしまう](https://877.design/smartphone-height/)  
結局min-height: 92vh;で画面サイズをそもそも小さくして対応した。  
const vh = window.innerHeight;  
// const vh = document.documentElement.clientHeight;  
ここら辺の資料も膨大に出てきた。  
[【CSS】100vhは「一画面」ではないという罠](https://jsnotice.com/posts/2019-03-30/#%E3%82%A2%E3%83%89%E3%83%AC%E3%82%B9%E3%83%90%E3%83%BC%E3%81%AE%E5%88%86%E6%8A%BC%E3%81%97%E5%87%BA%E3%81%95%E3%82%8C%E3%82%8B)
[【CSS】vhは一画面でない...ならばwindow.outerHeightを試してみよう+実用例](https://jsnotice.com/posts/2019-04-03/)  

[Javascript/jQuery でブラウザ高さを取得する方法](https://www-creators.com/archives/6036)  

[JavaScriptでスクロール禁止にする方法を解説！](https://qumeru.com/magazine/277)  

[iOS Safari でダブルタップによるズームを防ぐには touch-action: manipulation が一番簡単](https://neos21.net/blog/2018/12/22-01.html)  
[[React] 画面タッチによるピンチズーム操作を無効にする](https://dev.classmethod.jp/articles/react-disable-pinch-zoom-by-screen-touch/)  
[JavaScriptのtouchstartを止めるとclickイベントが発火しない](https://qiita.com/krmbn0576/items/fb8f1c90128b242a83bc)  
[[javascript] ダブルタップでブラウザが拡大するのを防ぐ](https://qiita.com/ELIXIR/items/1d007313c09489738e26)  

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

## Linq

[TypeScript vs. C#: LINQ](https://decembersoft.com/posts/typescript-vs-csharp-linq/#firstordefault)  
[はじめてのvue-property-decorator (nuxtにも対応）](https://qiita.com/otagaisama-1/items/a9eec24acabb35cc4b1c)  

Sum  
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

---

## Vue Enable

``` html
<b-button
  class="btn"
  block
  variant="outline-primary"
  @click="callStaff"
  v-bind:disabled="isWaiting || this.tableNo == null"
  v-text="'スタッフ呼出'"
/>
```

---

## Vue @Propデコレータの書き方

[Vue.js to TypeScriptの書き方一覧](https://qiita.com/ryo2132/items/4d43209ea89ad1297426)  

``` Vue
<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator';

@Component
export default class Post extends Vue {
  @Prop({ default: '' })
  contents!: string;

  @Prop({ default: 0 })
  postNumber!: number | string;

  @Prop({ default: false })
  publish!: boolean;

  @Prop
  option?: {
    new: boolean,
    important: boolean,
    sortNumber: number
  };
}
</script>
```

---

## npm run build 出来ない問題

物凄く単純な事だった。  
cdコマンドで移動先の名前が完全に合っていないとダメらしい。  
これで30分くらい嵌った。

移動先ディレクトリが`SelfOrder`のときに`cd selfOrder`で問題なく移動できるが、そこでbuildするとエラーが発生する。  
`cd SelfOrder`と大文字小文字をしっかり合わせて移動すると問題なかった。  
大文字小文字の違いってそういう？  
逆にわからなんってなった。  

因みに発生したエラーはこれ。  
[there are multiple modules with names that only differ in casing](https://minokuba.hatenablog.com/entry/20180310/1520679200)  
>無造作にnpm updateしたら突如出るようになったエラー。  
>どうやらWindowsの場合に、importするモジュール名の大文字・小文字が異なるとエラーになるらしい。  
>
>node_modulesを見たら、index.jsに定義していたモジュールの大文字小文字が異なったので直したら解消した。  
>早めに、仮想Linux環境に移行したいな・・・  

---

## ASP.NETのAuthorize(AuthenticationSchemesとRoles)の認証を通す

SelfOrderControlerにある以下の認証を通すために1日無駄にしたのでメモする。  
`[Authorize(AuthenticationSchemes = OtherSystemCustomerAuthenticationOptions.DEFAULT_SCHEME, Roles = nameof(WebCoopClsType.RoundNaviWeb))]`  

結論からいうとヘッダーに以下の指定をすればよかった。  

``` txt
coop-cls-type-name : RoundNaviWeb
encrypted-coop-customer-code : 暗号化されたWeb会員CD
```

ASP側のコードをよくよく見てみれば、ヘッダーから`coop-cls-type-name`と`encrypted-coop-customer-code`の値を取得するような処理があったので、  
もしやと思ってヘッダーを追加してみたら行けた。  
この程度のことなら一言、言ってくれればよかったのに・・・。

<https://qiita.com/okazuki/items/f66976c8cd71ea99c385>  

---

## TypeScript Const Class

[Typescriptで定数クラスを作成する](https://dev.appswingby.com/typescript/typescript%E3%81%A7%E5%AE%9A%E6%95%B0%E3%82%AF%E3%83%A9%E3%82%B9%E3%82%92%E4%BD%9C%E6%88%90%E3%81%99%E3%82%8B/)  
[TypeScript(Angular)で定数クラス](https://www.l08084.com/entry/2018/02/16/180015)  

[[Vue.js] template内で定数を簡単に使用したい](https://ohmyenter.com/use-constants-in-vue-template/)  
[Vue.jsでグローバルな定数をコンポーネントで使いまわせるようにしたい](https://qiita.com/amagurix/items/0f19d04b7771a71b5eaf)  

``` ts
export class SystemConst {
  /** アプリケーション名 */
  static readonly APPLICATION_NAME = 'ほげアプリ';

  /** サーバー */
  static readonly Server = class {
      /** IPアドレス */
      static readonly IP = '192.168.1.1';
      /** サブネットマスク */
      static readonly Mask = '255.255.255.0';
  }
}

/** システム定数クラス */
export namespace SystemConst {
  /** アプリケーション名 */
  export const APPLICATION_NAME = 'ほげアプリ';

  /** サーバー */
  export namespace Server {
      /** IPアドレス */
      export const IP = '192.168.1.1';
      /** サブネットマスク */
      export const Mask = '255.255.255.0';
  }
}
```

[TypescriptでInner Classを定義する方法](https://anton0825.hatenablog.com/entry/2015/11/14/000000)  
[TypeScriptでネストされたクラスを作成できますか？](https://www.webdevqa.jp.net/ja/javascript/typescript%E3%81%A7%E3%83%8D%E3%82%B9%E3%83%88%E3%81%95%E3%82%8C%E3%81%9F%E3%82%AF%E3%83%A9%E3%82%B9%E3%82%92%E4%BD%9C%E6%88%90%E3%81%A7%E3%81%8D%E3%81%BE%E3%81%99%E3%81%8B%EF%BC%9F/1055625294/)  

---

## GetとSetを同時に行うメソッドの名前

「load」がいいのでは？という話。  
メソッド名は永遠の課題だ。  

[メソッド名等の名前付け (個人的)](https://knooto.info/naming-methods/#%E7%B5%84%E3%81%BF%E5%90%88%E3%82%8F%E3%81%9B)  
[うまくメソッド名を付けるための参考情報](https://qiita.com/KeithYokoma/items/2193cf79ba76563e3db6)  

---

## Promise then,catch

``` ts
  /**
   * ライフサイクルフックmounted
   */
  async mounted(): Promise<void> {
    try {
      await super.mounted();
    } catch (error) {
      console.error(error);
      this.toast.error('エラーが発生しました。スタッフをお呼びください。');
      return;
    }
    // プレーヤー一覧をロード
    await this.loadFramePlayerList()
      .then(() => {
        // 代表者の会計Noが状態管理に登録されていれば操作可能にする。
        this.isTotalEnable = Boolean(this.encryptRepreAccountNo);
      })
      .catch(error => {
        this.toast.error(error);
      });
  }

  /**
   * プレーヤー一覧をロードします。
   */
  private async loadFramePlayerList(): Promise<void> {
    // プレーヤー一覧取得リクエストの生成
    const framePlayerListRequest = new FramePlayerListRequest(
      this.encryptWebMemberCD
    );
    // プレーヤー一覧の取得
    await this.selfOrderService
      .getFramePlayerList(framePlayerListRequest, this.encryptWebMemberCD)
      .then(
        resolve => {
          // 画面に反映させる
          this.playerList = resolve.PlayerList.map(m => {
            return {
              EncryptedAccountNo: m.EncryptedAccountNo,
              AccountNo: m.AccountNo,
              Name: m.Name,
              Kana: m.Kana,
              // 最初から選択された状態にする。
              // 項目はなんでもよかったがとりあえずEncryptedAccountNoにしておいた。
              IsSelected: Boolean(m.EncryptedAccountNo),
              // 代表者は絶対にチェックを外せないようにする。
              IsDisable: m.EncryptedAccountNo == resolve.EncryptedAccountNo
            };
          });
          // マルチモード∧プレーヤーが2人以上の場合にチェックボックスを表示する
          this.isMultiSelect =
            resolve.IsMultiSelect && this.playerList.length >= 2;
          // 暗号化された代表者の会計Noを登録する。
          this.setEncryptRepreAccountNo(resolve.EncryptedAccountNo);
        },
        reject => {
          console.error(reject);
          return Promise.reject(reject.data.message);
        }
      );
  }
```

---

## Object.assign({}, this.$route.query)

---

## objectの比較方法

[JavaScriptでのObject比較方法](https://www.deep-rain.com/programming/javascript/755)  

安直にやるなら以下の方法で行けるらしい。

``` js
const a = {"a":"a"};
const b = {"a":"a"};

const aJSON = JSON.stringify(a);
const bJSON = JSON.stringify(b);

console.log(aJSON === bJSON);  // -> true
```

---

## { [key: string]: string }

[TypeScriptのIndex Signature"{[key:string]:string}"で特定の文字だけのIndexを扱う](https://blog.mitsuruog.info/2019/03/typescript-limited-set-of-index-signature)  

javascriptでよく見る記述。  
jsonを作る時に便利っぽい。
後でまとめたい。  

``` ts
  protected get tableParameter(): { [key: string]: string } {
    const params: { [key: string]: string } = {};
    params.ReferrerName = 'RoundNaviWeb';
    params.WebMemberCD = 'test2';
    return params;
  }
```

---

## クエリパラメータを使いまわす方法

hold query params Vue

<https://flutterq.com/how-to-set-url-query-params-in-vue-with-vue-router/>  
<https://stackoverflow.com/questions/45091380/vue-router-keep-query-parameter-and-use-same-view-for-children>  
[Vue Router - How to pass along query params between routes](https://vuejscode.com/vue-router-how-to-pass-along-query-params-between-routes)  

[VueにVue-Routerを使ってURLクエリパラメータを設定する方法](https://www.webdevqa.jp.net/ja/javascript/vue%E3%81%ABvuerouter%E3%82%92%E4%BD%BF%E3%81%A3%E3%81%A6url%E3%82%AF%E3%82%A8%E3%83%AA%E3%83%91%E3%83%A9%E3%83%A1%E3%83%BC%E3%82%BF%E3%82%92%E8%A8%AD%E5%AE%9A%E3%81%99%E3%82%8B%E6%96%B9%E6%B3%95/827650744/)  

[Vue Routerのナビゲーションガードについて](https://qiita.com/yoshiblog-space/items/a4eb02d1d05ba1fbf9b5)  
[Vue-Routerのナビゲーションガードを使ってみる](https://qiita.com/SatohJohn/items/cd7067ac64d8e45da4dd)  

``` ts
// with query, resulting in /register?plan=private
router.Push({ path: 'register', query: { plan: 'private' }})

const routes: Array<RouteConfig> = [
  {
    path: '/',
    name: 'ScanQRCode',
    component: ScanQRCode,
    beforeEnter: (to, from, next) => {
      //to.query = Object.assign({}, from.query);
      to.query.ReferrerName = 'RoundNaviWeb';
      to.query.WebMemberCD = 'test2';
      next();
    }
  },


// グローバルビフォーガード
router.beforeEach((to, from, next) => {
        console.log(from.path);
      console.log(to.path);
  if (from.path == to.path) {
    if (from.path == '/') {
      next();
    } else {
      next(false);
    }
  } else {
    if (JSON.stringify(from.query) != JSON.stringify(to.query)) {
      next({
        path: to.path,
        query: from.query
      });
    } else {
      next(false);
    }
  }

  // if (JSON.stringify(from.query) != JSON.stringify(to.query)) {
  //   if (from.path == to.path) {
  //     console.log('b');
  //     next(false);
  //   } else {
  //     console.log('a');
  //     console.log(from.path);
  //     console.log(to.path);
  //     console.log(JSON.stringify(from.query));
  //     console.log(JSON.stringify(to.query));
  //     next({
  //       path: to.path,
  //       query: from.query
  //     });
  //   }
  // } else {
  //   console.log('c');
  //   console.log(from.path);
  //   console.log(to.path);
  //   console.log(JSON.stringify(from.query));
  //   console.log(JSON.stringify(to.query));
  //   console.log(JSON.stringify(from.query) != JSON.stringify(to.query));
  //   // next();
  //   if (JSON.stringify(from.query) == JSON.stringify(to.query)) {
  //     next(false);
  //   } else {
  //     next();
  //   }
  // }
});

function hasQueryParams(route: Route) {
  return !!Object.keys(route.query).length;
}
```

``` ts : 最も安直にクエリパラメータを引き回す方法
  /**
   * QRコード読み込み画面に遷移します。
   */
  protected moveToScanQR(): void {
    this.$router.push({
      path: '/',
      query: Object.assign({}, this.$route.query)
    });
  }
  /**
   * 来場者確認画面に遷移します。
   */
  protected moveToConfirmTable(): void {
    this.$router.push({
      path: 'ConfirmTable',
      query: Object.assign({}, this.$route.query)
    });
  }
```

---

## bootstrap-vueのボタン、無効を即反映させる

`:variant`の指定が`outline-primary`だけでは駄目だった。  
それについで、`bg-white text-primary`も指定する必要があった。  
逆に言えばこれだけだったのだが、全然答えが見つからなかった。  

``` html
  <b-button
    block
    class="btn-request"
    :variant="
      item.isSelected == true
        ? 'primary'
        : 'outline-primary bg-white text-primary'
    "
    v-bind:disabled="!isTotalEnable"
    :pressed.sync="item.isSelected"
  >
```

---

## ダブルタップの拡大

[スマホでダブルタップしたときに拡大しないようにするCSS](https://www.memory-lovers.blog/entry/2020/01/27/170000)  
[iOS10のSafariでuser-scalable=no が効かなくズームがされる問題への対策](https://qiita.com/GrDolphium/items/d74e5758a36478fbc039)  

`touch-action`と`user-scalable=no`の2つが手っ取り早いみたい。  
`user-scalable`のほうはあまりよろしくないみたいなので、`touch-action`を使ったら一発で解決したのでこれでいいかなって思った。  

---

## コンポーネント間のデータの受け渡し

[vue.js component間のデータの受け渡し](https://qiita.com/catkk/items/bd474d75b42aae2b92e9)  

主に「path,query」の組み合わせと「name,param」の組み合わせがあるらしい。  
paramを使う場合、nameでなければ駄目。queryは使えるらしい。  
基本的にnameが万能そうであるが、履歴の

---

## npm WARN old lockfile

[cdkでよくあると思う npm WARN をとにかく解消してみた](https://dev.classmethod.jp/articles/fix-npm-warn-on-cdk/)  
>lockfileが古い状況です。更新すると表示されなくなります。  
>`npm i --package-lock-only`  

[npmにpackage-lock.jsonを生成させる方法はありますか？](https://www.webdevqa.jp.net/ja/node.js/npm%E3%81%ABpackagelockjson%E3%82%92%E7%94%9F%E6%88%90%E3%81%95%E3%81%9B%E3%82%8B%E6%96%B9%E6%B3%95%E3%81%AF%E3%81%82%E3%82%8A%E3%81%BE%E3%81%99%E3%81%8B%EF%BC%9F/833855653/)  

---

## URLのエスケープ

`ZAOtlo+HxBqV0Nhbgq2GYA==` がURLだと `ZAOtlo%20HxBqV0Nhbgq2GYA%3D%3D` となり、それを変換すると `ZAOtlo HxBqV0Nhbgq2GYA==` なる問題が発生。  
「+」はASCIIだと `%2B` になるはずだが、 `%20` : `SPACE` になってしまう。  

URLに乗せる際、エスケープしていないのが問題だった。  
PHPのurlencode関数を暗号化された文字列に噛ませたら解決した。  

[urlencode](https://www.php.net/manual/ja/function.urlencode.php)  
[ASCII文字とURLエンコードの対応表](https://www.seil.jp/doc/index.html#tool/url-encode.html)  
[URLエンコード 【URL encoding】 パーセントエンコーディング](https://e-words.jp/w/URL%E3%82%A8%E3%83%B3%E3%82%B3%E3%83%BC%E3%83%89.html)  

---

## b-row,b-colで頑張った

``` html
<b-container fluid class="bv-example-row">
  <b-row>
    <b-col>
      <b-row>
        <b-col>Level 2: cols="8" sm="6"</b-col>
      </b-row>
      <b-row>
        <b-col>Level 2: cols="4" sm="6"</b-col>
      </b-row>
    </b-col>
    <b-col align-self="center">
      プラン毎に設定1
    </b-col>
  </b-row>
</b-container>
```

---

## bootstrap-vueのb-row,b-colのmarginとpaddingを0にする

marginとpaddinigで痛い目にあったのでまとめる。  
デフォルト状態だと、勝手にmarginとpaddingが設定されるらしく、それが悪さしてたので、classでさっくり指定できる方法がないか探したらあった。  

`class="p-0 m-0"`でmarginとpaddingを0に設定できた。  

``` html
  <b-row class="p-0 m-0">
    <b-col class="p-0 m-0">
```

[bootstrapvueの使い方全覚書(b-container,b-col,b-table/b-form-radio)](https://ebookbrain.net/bootstrap-vue/#b-colpadding)  

---

## html 1行 左寄せ 右寄せ

[一行の中で左寄せと右寄せに分けて表示する方法](https://blog.goo.ne.jp/nobutoshi3/e/5f1f53bce3f4a835a2e27f7089616091)  

実際にはこんな感じで実現できた。

``` html
  <b-td
    :variant="item.SettlementFlag ? 'danger' : 'primary'"
    style="margin-right: 20px;border: 2px solid #ffffff;"
  >
    <span
      class="text-left float-left"
      v-text="!item.Name ? '　' : item.Name + ' 様'"
    />
    <span
      class="text-right float-right"
      v-if="item.SettlementFlag"
      v-text="'精算済'"
    />
  </b-td>
```

---

## 動的にテーブルを結合する方法

どうでもいいけどすんげーことしてるやつ見つけた。

[HTMLのテーブルのセルを動的結合するvueコンポーネントを作る](https://note.com/moli9ma/n/n27bab66d27e6)  

---

## bootstrap 文字色を変更する

classでvariantの文字列使えばおｋ。

``` html
            <p
              class="p-0 m-0 text-left text-info"
              v-text="'※精算済みの方には伝票は付けられません。'"
              v-if="playerList.some(s => s.SettlementFlag)"
            />
```

---

## ブラウザバックの無効化

safariだとうまく行かなかった。他の方法で対応した。  
ブラウザにも色々あるんだなぁって思った。  

[HTML5 history apiで戻るボタン無効化](https://qiita.com/maruo327/items/a0502c8b508fed4ec169)  
[JavaScriptだけでブラウザの「戻る」ボタンを無効化する方法](https://hidari-lab.hatenablog.com/entry/2015/03/02/212818)  
[ブラウザの「戻る」ボタンでキャッシュが表示されてしまう際の対策](https://qiita.com/shibe23/items/79f950061457ff1a7687)  
[History APIとスマホブラウザでのハマりどころ](https://qiita.com/nenokido2000/items/2dd8da77d06837c8a510)  

``` ts
finally {
      // 戻る禁止
      if (history && history.pushState && history.state !== undefined) {
        history.pushState(null, '', null);
        window.addEventListener(
          'popstate',
          () => history.pushState(null, '', null),
          false
        );
      }
    }
```

---

## 一定時間後に実行する

javascriptに書くべきだがとりあえず。
setTimeout関数使う。

``` js
// とりあえず0.1秒無効にする。
setTimeout(() => (this.isTotalEnable = true), 100);
```

---

## デッドロック

同時保存のデッドロックが解決できなくて苦しんだ。  
こちらではなくSQL Server側の記事に移したのでそちらを参照されたし。  
