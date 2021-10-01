# TypeScriptメモ

## APIのメッセージを表示する方法(Response型のメッセージを取得する方法)

APIからのメッセージは_bodyプロパティの中にあるみたいが、素直にアクセスしようとすると存在しないというエラーになる。  
実行したときは存在するけど、コーディングの段階では存在しないみたいなパターン。  
これがJavascriptの仕様なのかTypescriptの仕様なのかは、わからないが、それはまた別でまとめたい。  
取り出すときはres.json()でなぜか取り出せる。  
そういうものなのだろうか。これも後でまとめる。  

``` ts
    // Service
    public maintenance(param: any): Promise<Response> {
        return this.apiService.put('system/web-cooperation/plan-cooperation/maintenance/', JSON.stringify(param));
    }
    
    // ViewModel
    this.goraMaintenanceService
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

RESTとGraphQLについて
<https://www.utakata.work/entry/2019/12/02/000000>

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
