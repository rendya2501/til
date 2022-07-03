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

``` ts
  /**
   * メニュー情報をロードします。
   */
  private async hoge(): Promise<void> {
    // メニュー取得リクエスト生成
    const request = new MenuRequest(this.patternCD);
    // メニュー情報取得
    await this.Service
      .getHoge(request, this.encryptToken)
      .then(
        resolve => this.setMenu(resolve),
        reject => Promise.reject(reject.data)
      );

    await this.Service
      .getHoge(request, this.encryptToken)
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
```

``` ts : 例1
export default class MenuList extends ViewBase {
  /**
   * ライフサイクルフックmounted
   */
  async mounted(): Promise<void> {
    // メニュー情報をロードします。
    await this.loadMenu()
      // ロードに成功した場合
      .then(() => {
        // メニュー分類をセット(上のバー)
        this.setMenuClass();
        const setMenuListTimeout = setTimeout(
          () => {
            this.setSelectedMenuList(null);
            clearInterval(setMenuListInterval);
          },
          3000
        );
        const setMenuListInterval = setInterval(() => {
          if (
            this.menu != null &&
            this.menu.MenuClassList != null &&
            this.menu.MenuClassList.length > 0
          ) {
            this.setSelectedMenuList(this.menu.MenuClassList[0].MenuClassCD);
            clearTimeout(setMenuListTimeout);
            clearInterval(setMenuListInterval);
          }
        }, 100);
      })
      // 操作可能にする
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
      .getMenu(request, this.encryptWebMemberCD)
      .then(
        resolve => this.setMenu(resolve),
        reject => Promise.reject(reject.data)
      );
  }
}
```

``` ts : 例2
export default class MenuDetail extends ViewBase {
  /**
   * ライフサイクルフックmounted
   */
  async mounted(): Promise<void> {
    try {
      await super.mounted();
      // 必要な情報が存在しない場合、処理しない。
      if (!this.encryptRepreAccountNo) {
        this.toast.error('セッションが切れました。');
        return;
      }
      if (!this.selectedMenu) {
        this.toast.error('商品が選択されていません。');
        return;
      }
    } catch (error) {
      console.error(error);
      this.toast.error('エラーが発生しました。スタッフをお呼びください。');
      return;
    }
    // 商品画像をロード
    // 画像でエラーになっても動作に影響はないので続ける。
    await this.loadImage().catch(error => this.toast.error(error));
    // 選択した商品情報を画面項目にセットする
    this.setParam();

    // 操作可能にする
    this.isTotalEnable = true;
  }

  /**
   * 商品の画像をロードします。
   */
  private async loadImage(): Promise<void> {
    this.isLoadingImage = true;
    await this.selfOrderService
      .getMenuImage(
        {
          PatternCD: this.selectedMenu?.PatternCD,
          MenuLargeClassCD: this.selectedMenu?.MenuLargeClassCD,
          MenuClassCD: this.selectedMenu?.MenuClassCD,
          MenuCD: this.selectedMenu?.MenuCD
        },
        this.encryptWebMemberCD
      )
      .then(
        resolve => (this.menuImage = resolve.toString()),
        reject => {
          console.error(reject);
          return Promise.reject(reject.data);
        }
      )
      .finally(() => (this.isLoadingImage = false));
  }
}
```
