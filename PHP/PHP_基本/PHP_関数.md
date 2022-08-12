# PHP_関数

---

## 関数の基本

- `function`で宣言する。  

シンプルな関数の定義

``` php
function 関数名 () {
    // 処理
}
```

PHP 7.0 から引数や戻り値の型宣言が可能。  

``` php
function 関数名 (引数の型 $引数): 関数の戻り値 {
    // 処理
    return $戻り値;
}
```

---

## デフォルト引数

PHP 7.0 から使用可能。  

``` php
function hoge(string $str = null) {
    var_dump($str);
}
```

---

## null許容引数

型名の前に ? を追加することで NULL 許容型として宣言できる。  
PHP 7.1 から使用可能。  

``` php
function hoge(?string $str) {
    var_dump($str);
}
```

[【PHP】引数の二種類の NULL 許容型の宣言方法とその違い](https://qiita.com/ymm1x/items/e53ded283080ca3a42b4)

---

## void

PHP 7.1からvoid宣言が可能。  
戻り値がないことを明記できるようになった。  

``` php
function hoge(): void {
}
```

あくまで戻り値がないことを宣言するだけ。  
何も返さないreturnはエラーにならず、自動的にvoidになる。  
voidを変数受けしてもエラーにはならない。中身はNULLになる。  
戻り値の型がない or return だけならvoid宣言した時と同じ状態になる模様。  

``` php
function hoge_void(string $str): void{
}

function hoge_return(string $str){
    return;
}

function hoge(string $str){
}

$hoge_void = hoge_void("aaa");
$hoge_return = hoge_return("aaa");
$hoge = hoge("aaa");

var_dump(hoge_void("aaa"));   // NULL
var_dump(hoge_return("aaa")); // NULL
var_dump(hoge("aaa"));        // NULL
var_dump($hoge_void);   // NULL
var_dump($hoge_return); // NULL
var_dump($hoge);        // NULL
```

void宣言しているのに、戻り値を返すとエラーになる。

``` php
function hoge_void(): void{
    // PHP Fatal error:  A void function must not return a value in /workspace/Main.php on line 21
    return "hoge";
}
var_dump(hoge_void());
```

---

## 型宣言 (タイプヒンティング)

関数の宣言時、関数が受け取る引数の型を指定して、指定型以外のデータが渡された場合にエラーが発生する機能。  

指定できる型一覧

- 文字列型  
- 整数型  
- 浮動小数点数型  
- 配列型  
- 論理型  
- Callable（コールバック）  
- クラス  
- インターフェース  

[PHP の関数 ( function ) について 【初心者向け】](https://wepicks.net/phpref-functions_userdefined/)  

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
[(PHP) コールバック関数とは？使い方を分かりやすく解説](https://hara-chan.com/it/programming/php-callback/)  

### 無名関数を使ったAPI実行サンプル

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

// 引数にcallableを指定すると、引数がコールバック関数であることを明示できる(タイプヒンティング)
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
