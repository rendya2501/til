# TypeScriptメモ

## APIのメッセージを表示する方法(Response型のメッセージを取得する方法)

APIからのメッセージは_bodyプロパティの中にあるみたいが、素直にアクセスしようとすると存在しないというエラーになる。  
実行したときは存在するけど、コーディングの段階では存在しないみたいなパターン。  
これがJavascriptの仕様なのかTypescriptの仕様なのかは、わからないが、それはまた別でまとめたい。  
取り出すときはres.json()でなぜか取り出せる。  
そういうものなのだろうか。これも後でまとめる。  

``` ts : Service
    public maintenance(param: any): Promise<Response> {
        return this.apiService.put('system/web-cooperation/plan-cooperation/maintenance/', JSON.stringify(param));
    }
```

``` ts : ViewModel
    this.MaintenanceService
        .maintenance(param)
        .then(
            res => {
                if (res.json() !== null && res.json() !== '') {
                    this.confirmComponent.dialogOk(res.json(), '確認', null)
                }
                this.notify.success('メンテナンスしました。');
                // 成功したら一覧を更新する
                this.getPlanList();
            },
            error => this.notify.error('失敗しました。\n : ' + error.json());
        )
        .then(() => this.isLoading = false);
```

どうでもいいけどこの人すごい。  
1990年生まれ。京大卒。  

---

## 全角の長さを取得する

```ts
    /**
     * 全角の長さを取得する
     * 「あ」   = 全角1文字
     * 「あa」  = 全角2文字
     * 「あaa」 = 全角2文字
     * 「あaab」= 全角3文字
     * @param str 判定文字列
     */
    private getZenkakuLength(str: string): number {
        // 半角0.5 は繰り上げる
        return Math.ceil(this.zenkakuCount(str));
    }

    /**
     * 全角数を判定する処理
     * 全角:1
     * 半角:0.5
     * @param str 判定する文字列
     */
    private zenkakuCount(str: string) {
        let length = 0;
        for (let i = 0; i < str.length; i++) {
            if (str[i].match(/[ -~]/)) {
                length += 0.5;
            } else {
                length += 1;
            }
        }
        return length;
    }
```

---

## Promiseで意図的にrejectをreturnする方法

例えば、メニュー画像を取得したい場合、メニュー画像を取得するためのフラグ制御、リクエスト生成、通信を1つのメソッドとしてまとめ、それ以降の処理は呼び出し元に戻ってPromise的な制御ができれば、意味合い的にも処理のまとまり的にも違和感がないように思えたので、どうやったらそれを実現できるか調べたが、意外とそういう例を見つけることができなかった。  
特に通信や意図した結果でない場合に意図的にrejectを発生させる方法がわからず、そちらも例題が全然なかった。  

結果的にどちらも達成することができたので、その例をここに載せる。  
参考文献等は、ts promiseで調べれば腐るほど出てくるので割愛する。  

``` ts : 例2
export default class Hoge extends HogeBase {
  /**
   * メニュー情報をロードします。
   */
  private async hoge(): Promise<void> {
    // メニュー取得リクエスト生成
    const request = new MenuRequest(this.code);
    // メニュー情報取得
    await this.Service
      .getHoge(request, this.token)
      .then(
        resolve => {
          this.setMenu(resolve);
          return Promise.resolve(reject.data);
        },
        reject => {
          console.error(reject);
          return Promise.reject(reject.data);
        }
      );
  }
}
```

``` ts : 例2
export default class MenuList extends ViewBase {
  /**
   * ライフサイクルフックmounted
   */
  async mounted(): Promise<void> {
    // メニュー情報をロードします。
    await this.loadMenu()
      // ロードに成功した場合、操作可能にする
      .then(() => (this.isTotalEnable = true))
      // ロードでrejectした場合、もしくは成功後のthenの処理でエラーが発生した場合
      .catch(error => {
        if (typeof error == 'string') {
          this.toast.error(error);
        } else {
          console.error(error);
          this.toast.error('エラーが発生しました。スタッフをお呼びください。');
        }
      })
      .finally(() => (this.isBusy = false));
  }

  /**
   * メニュー情報をロードします。
   */
  private async loadMenu(): Promise<void> {
    // メニュー取得済みなら再取得しない。
    if (this.menu) {
      return Promise.resolve();
    }
    // メニュー取得リクエスト生成
    const request = new MenuRequest(this.patternCD);
    // メニュー情報取得
    await this.selfOrderService
      .getMenu(request, this.token)
      .then(
        resolve => this.setMenu(resolve),
        reject => Promise.reject(reject.data)
      );
  }
}
```

---

## TypeScriptのオブジェクトの初期化

C#みたいに `new Object(){a = huga,b = hoge}` ってできないか調べた。  
[TypeScriptのclassをオブジェクトで初期化する](https://qiita.com/Tsuyoshi84/items/e74109e2ccc0f4e625aa)  

---

## TypeScriptのオーバーロード

[TypeScript: オーバーロードメソッドを定義する方法](https://qiita.com/suin/items/7d6837a0342b36891099)  

---

## typescript json to class

[TypeScriptのリフレクションでJSONの型変換を自動化する](https://qiita.com/bitrinjani/items/d60bdac10e5ced126d1a)  

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

## TypeScript Enum

[「なぜ enum の利用が推奨されないのか？」をまとめてみた](https://qiita.com/saba_can00/items/696baa5337eb10c37342)  

---

## TypeScriptで "Object is possibly null" と怒られたときにすること

<https://qiita.com/fufufukakaka/items/5d4a2f2272b8f1a4a16f>  

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
