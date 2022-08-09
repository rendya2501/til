# PHP_関数

---

## コールバック

phpのコールバックは3種類ある。  

- 可変関数を使う方法  
- call_user_func関数を使う方法  
- 無名関数を使う方法  

無名関数がおすすめ。  
call_user_func関数は文字列で指定するのでなんかいやだ。  
関数名を文字列に変換するしてくれるnameofみたいなのがあればいいのだが、軽く調べてみたけど、なさそうですね。  
現在実行している関数名を取得する方法はあるけど、クラスの中のこの関数を文字列に変換するみたいな処理はないっぽい。  

[いい感じのコールバックサンプル](https://qiita.com/dublog/items/0eb8bcea2fc452c0b4b2)  
[PHPでコールバック関数を利用する](https://qiita.com/tricogimmick/items/23fb5958b6ea914bbfb5)  
とりあえず、困ったらここ見ればいいんじゃないかな。  

``` PHP : 無名関数を使ったAPI実行サンプル
function callGetAPI()
{
    // クエリパラメータ
    $params = 'linkage_ids=';
    // リクエスト生成コールバック生成
    $createRequest = function ($token) use ($params) {
        return new Request(
            'GET',
            SERVICE_URL . API_URI . '?' . $params,
            ['Authorization' => 'Bearer ' . $token]
        );
    };
    // API実行
    return commonAPIAction($createRequest);
}

// 引数にcallableを指定すると、引数がコールバック関数であることを明示できるみたい
// タイプヒンティングっていうらしいみたい。
function commonAPIAction(callable $createRequest)
{
    // ログインAPIを実行してトークンを取得
    $token = callLoginAPI();
    // リクエスト生成
    $request = $createRequest($token);
    // クライアント生成
    $client = new Client(['http_errors' => false, 'function' => 'functionfunction']);
    // API実行
    $response = executeAPI($client, $request);
    return $response;
}
```
