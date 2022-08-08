# PHP基礎・基本

---

## PHP

Hypertext Preprocessor

---

## 書き始め

`<?php`で書き始め`?>`で閉じる。  
`?>`は省略可能。  

``` php
<?php

?>
```

---

## PHP6

PHPの参考書を読んでいた時にコラムに書いてあった奴。  
なぜPHP6はないのか。

要約すると、文字コードのUTF-16対応が6のメインだったけど、出来たものがメモリを食い過ぎて使い物にならなくなったのでゴミを化したらしい。  

[PHPは何故バージョン6が存在しないのか？](https://wordpress.ideacompo.com/?p=16906)  
よくまとまっているので全文引用します。  

``` txt : PHP6は開発失敗作
今から10年以上まえになりますが、2010年に、PHPがバージョン5.3だった時に、PHPバージョン6の開発が精力的に行われていたそうです。
目玉機能のひとつとして、unicode対応というのがあり、UTF16の正式対応が決まっていたそうです。
そうした文字エンコード対応を重要視した開発をおこなっていた時に、メモリの使用量が想定したよりも膨らみすぎて、正直使い物にならなくなってしまったようです。
mbstringをプラグインインストールからするのがめんどくさいので、こういう点も対応してもらいたいんですが、文字コードはインターネットでの黒歴史が多いジャンルですからね。SHIFT-JISとか、EUCとか・・・
そのため、5.3から6にメジャーバージョンアップするソースが、全てゴミと化してphpのバージョン事態も5.3にロールバックしてしまったという痛ましい事件が起きたらしいです。
その後、色々な機能追加によるバージョンアップする時に、開発員たちは、封印したバージョンとして6をすっ飛ばして、7を選択したのだそうです。
```

因みに独習PHPにはこのように紹介されていたので乗せておきます。

``` txt : 独習コラム
PHP5の後、当初はPHP6のリリースが予定されており、実行エンジンの内部処理がUTF-16で全て置き換えられる予定でした。
しかし、予想以上に大規模な変更になったことから開発は滞り、様々な問題も生じた事から、2010年に結局、開発は断念されることになりました。
その後、PHP6で予定されていた機能の多くは、PHP5のマイナーバージョンに取りこまれ、PHP6は永久欠番になったのです。
```

---

## 変数

・型の宣言は不要  
・実行時に判定される  
・最初の文字列を格納した変数に後から数字をセットしても問題ない→型の相互変換  

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

---

## cURL error 56: TCP connection reset by peer

<https://deep-blog.jp/engineer/5443/>  

内容的にはネットワークの部類に入るのだろうけど、実務でphpで通信処理を書いている時に発生したのでここに書く。  
参考サイト的にはpeerとは相手のことを指すので、この場合、接続相手から接続を切られてしまったことを意味するとか。  
実務で発生した現象だと、管理サイトにはプランが出ていたので、リクエストは受け付けられた模様。  
その後の切断処理がうまくいかなかったと思われる。  
ここはプログラムでどうにかできる問題では無い。  
完全にインフラ側の問題。  

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

---

## 最後の文字を消す

[PHP で文字列から最後の文字を削除する](https://www.delftstack.com/ja/howto/php/php-remove-last-character-from-string/)  
別にまとめる程のことでもないかもしれないが、色々方法があるので、一番手っ取り早い奴だけまとめておく  

``` php
// 対象文字列、スタート位置、長さ
// substr($string, $start, $length)
$mystring = "This is a PHP program.";
echo substr($mystring, 0, -1);
// 出力：This is a PHP program
```

---

## PHPのプロセスを止める方法

<https://flashbuilder-job.com/php/635.html>

`ps aux | grep php`  
これでプロセスID（左から２つ目の値）を調べる。  

ルートユーザー、またはルートになれるユーザーでコマンド実行  
`sudo kill -9 (プロセスID)`  
