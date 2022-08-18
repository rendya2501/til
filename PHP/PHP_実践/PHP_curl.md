# PHP_curl

---

## cURLの基本的な使用方法

外部のサイトにアクセスするのに最低限必要な手順。  

1. `curl_init関数` : cURLによるセッションを初期化する。  
2. `curl_setopt関数` : 各種オプションやアクセスする外部サイトのURLを指定する。  
3. `curl_exec関数` : 外部サイトと通信する。  
4. `curl_close関数` : cURLによるセッションを閉じる。  

``` php : cURLを利用した簡単な例
// 1. cURL セッションを初期化する
$ch = curl_init();
// 2. 転送用オプション設定
// アクセスする外部サイトのURLを設定
curl_setopt($ch, CURLOPT_URL, "https://httpbin.org/");
// curl_exec() の返り値を 文字列で返す。
// TRUE ではない場合、データを直接出力する。
curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
// 3. 指定した URL に data 送信
$http_str = curl_exec($ch);
// 4. システムリソースを解放
curl_close($ch);
```

- `CURLOPT_URL`  
- `CURLOPT_RETURNTRANSFER`  

外部サイトにアクセスし、その情報をPHPのプログラム内で利用する場合は、この2つのオプションは必須。  

---

## curl_POST

``` php
curl_setopt($ch, CURLOPT_POST, TRUE);
curl_setopt($ch, CURLOPT_POSTFIELDS, $json_params);
```

この2行でPOST送信が可能になる。  

`CURLOPT_POST`は言わずもがな。  
`CURLOPT_POSTFIELDS`で送信する変数と値を設定する。  

postの指定は`curl_setopt($ch, CURLOPT_CUSTOMREQUEST, 'POST');`でもできるが、定数として`CURLOPT_POST`が用意されているので別に使う必要はないだろう。  

``` php
function POST($url,$header,$param){
    $ch = curl_init();
    curl_setopt($ch, CURLOPT_URL, $url);
    curl_setopt($ch, CURLOPT_POST, TRUE);
    curl_setopt($ch, CURLOPT_POSTFIELDS, $param);
    curl_setopt($ch, CURLOPT_RETURNTRANSFER, TRUE);
    curl_setopt($ch, CURLOPT_HTTPHEADER, $header);

    if(! $result = curl_exec($ch) ){
        echo 'curl_errno：' . curl_errno($ch) . "\n";
        echo 'curl_error：' . curl_error($ch) . "\n";
        echo '▼ curl_getinfo' . "\n";
        foreach(curl_getinfo($ch) as $key => $val){
            echo '  ■ ' . $key . '：' . (is_array($val) ? print_r($val) : $val) . "\n";
        }
    }

    curl_close($ch);
    return $result;
}

$url = 'https://httpbin.org/post';
$header = [
    'Content-Type: application/json',
    'Accept-Charset: UTF-8'
];
$param = [
    'user_id'  => 'test_id',
    'password' => 'test_password'
];

echo POST($url,$header,json_encode($param));
```

---

## curl_GET

URLに続けて変数と値を記述して、curl_exec関数で外部サイトにアクセスするだけで、変数と値を送信可能。  
`CURLOPT_HTTPGET`オプションで明示的にGET送信を指定することも可能。  

``` php : cURLによるGET送信の例
function GET($url,$header){
    $ch = curl_init();
    curl_setopt($ch, CURLOPT_URL, $url);
    curl_setopt($ch, CURLOPT_HTTPGET, TRUE);
    curl_setopt($ch, CURLOPT_RETURNTRANSFER, TRUE);
    curl_setopt($ch, CURLOPT_HTTPHEADER, $header);

    if(! $result = curl_exec($ch) ){
        echo 'curl_errno：' . curl_errno($ch) . "\n";
        echo 'curl_error：' . curl_error($ch) . "\n";
        echo '▼ curl_getinfo' . "\n";
        foreach(curl_getinfo($ch) as $key => $val){
            echo '  ■ ' . $key . '：' . (is_array($val) ? print_r($val) : $val) . "\n";
        }
    }

    curl_close($ch);
    return $result;
}

$url = 'https://httpbin.org/get';
$query = '?id=test';
$header = [
    'Accept-Charset: UTF-8'
];

echo GET($url.$query,$header);
```

---

## 参考リンク

[PHPのcURLならPOST送信もらくらく！例を使って詳しく解説します](https://webukatu.com/wordpress/blog/18538/)  
[【PHP】cURL関数でPOSTする方法（送信＆受け取る（API））](https://qiita.com/_naka_no_mura_/items/da20c7305e739953dd70)  
[php curl 使って クリックなしで POST 送信](https://hapicode.com/php/curl-post.html#function)  
