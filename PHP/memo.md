# PHPメモ

---

## 変数

・型の宣言は不要  
・実行時に判定される  
・最初の文字列を格納した変数に後から数字をセットしても問題ない→型の相互変換

---

## foreach

・`foreach(引数 as 要素名)`  
・配列とオブジェクトだけ使用可能。それ以外はエラーになる。  
・PHPの文字列はC#と違い、Char型の配列という扱いではないのでエラーになる。  
　またPHPにCHAR型は存在しない。String型のみである。  

``` PHP
<?php
    $str = "aaaa";
    foreach ($str as $a){
        echo $a;
    }
    // PHP Warning:  foreach() argument must be of type array|object, string given in /workspace/Main.php on line 4

    $null = null;
    foreach ($null as $a){
        echo $a;
    }
    // PHP Warning:  foreach() argument must be of type array|object, null given in /workspace/Main.php on line 14

    $num = 1;
    foreach ($num as $a){
        echo $a;
    }
    // PHP Warning:  foreach() argument must be of type array|object, int given in /workspace/Main.php on line 19
?>
```

### invalid argument supplied for foreach()

[Warning: Invalid argument supplied for foreach() とでたら。。。](https://hacknote.jp/archives/19783/)  

萬君に、何をしたらforeachでこのようなエラーが出るのか答えられなかったのでまとめ。  
PaizaIOで確かめてもこのエラーにならなかった。

``` php
// $itemがnullだとエラー。
// $item = []だとエラーにならず、処理されずに終わる。
foreach($item as $value)
```

---

## PHPのプロセスを止める方法

<https://flashbuilder-job.com/php/635.html>

`ps aux | grep php`  
これでプロセスID（左から２つ目の値）を調べる。  

ルートユーザー、またはルートになれるユーザーでコマンド実行  
`sudo kill -9 (プロセスID)`  

---

## コールバック

[いい感じのコールバックサンプル]<https://qiita.com/dublog/items/0eb8bcea2fc452c0b4b2>

```php
const API_URI = "API_URI";
const ServiceURL = "https://";

function add()
{
    $params = ['1,2,3,4,5'];
    $try = function ($token) use ($params) {
        return [
            'PATCH',
            ServiceURL . API_URI,
            [
                'Authorization' => 'Bearer '.$token,
                'Content-Type' => 'application/json'
            ],
            json_encode($params)
        ];
    };
    retry($try);
}

// 共通のリトライ処理を別メソッドとして用意しておき、それぞれの処理を噛ませる。
function retry(callable $try)
{
    $token = "tekitou_token";

    $result = $try($token);

    print_r($result);
}

add();
```

---

## エルビス演算子の使いどころさん

```php
// statusCodeが500の時エラーメッセージを取得する
$status = 500;
$message = 'err';
$err = null;

// falseの時だけ実行される。200 === 500でfalse。
$status === 200 ?: $err .= $message;

print $err ?? 'naiyo';
```

---

## cURL error 56: TCP connection reset by peer

<https://deep-blog.jp/engineer/5443/>  
後でちゃんとした場所に書く。  
内容的にはネットワークの部類に入るのだろうけど、GORAのエラーで発生したのでここに書く。  
参考サイト的にはpeerとは相手のことを指すので、この場合、接続相手から接続を切られてしまったことを意味するとか。  
GORAで発生した現象だと、管理サイトにはプランが出ていたので、リクエストは受け付けられた模様。  
その後の切断処理がうまくいかなかったと思われる。  
ここはプログラムでどうにかできる問題では無い。  
完全にインフラ側の問題。  
GORAからの回答を待つしかない。  
応用で勉強したので状況がわかる。初めての問題だったので備忘録に残す。  

---

## nullと文字列結合演算子

<https://tomcky.hatenadiary.jp/entry/20180810/1533908140>

nullを文字列結合するとNULLは常に空文字列に変換されるので、is_nullに引っかからなくなる、びっくりphp言語仕様。  
この場合はempry()を使うといいらしい。  
エラーメッセージだけ取得する関数作って、それを直接、エラー変数にぶち込んだらis_null利かなくなって嵌ったので残す。  

``` php
function getNull() {
    return null;
}
$tmp = getNull();
print is_null($tmp) ? 1 : 0; // 1

$ee = null;
$ee .= $tmp;
print is_null($ee) ? 1 : 0; // 0

$ee = null;
$ee .= null;
print is_null($ee) ? 1 : 0; // 0
```
