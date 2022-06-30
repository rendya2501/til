# PHP基礎・基本

## 変数

・型の宣言は不要  
・実行時に判定される  
・最初の文字列を格納した変数に後から数字をセットしても問題ない→型の相互変換  

---

## PHP6

どうでもいい事まとめたくなった。  
PHPの参考書を読んでいた時にコラムに書いてあった奴。  
結局何だっけ？ってのをまとめておく。  

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

## foreach

・`foreach(引数 as 要素名)`  
・配列とオブジェクトだけ使用可能。それ以外はエラーになる。  
・PHPの文字列はC#と違い、Char型の配列という扱いではないのでエラーになる。  
　またPHPにCHAR型は存在しない。String型のみである  

### foreachで作った変数はforeach抜けた後も有効

割と罠。  
公式のリファレンスにも書いてありました。  
unsetはマナー的にあってもいいのかもしれないですね。  

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

supplied : 供給  
違法な引数をforeachに供給した  

Yr君に、何をしたらforeachでこのようなエラーが出るのか答えられなかったのでまとめ。  
PaizaIOで確かめてもこのエラーにならなかった。  
結局、foreachにnullを渡せばこのエラーになるのはわかったけど、なぜエラー内容が違うのかわからないままだ。  

``` php
// $itemがnullだとエラー。
// $item = []だとエラーにならず、処理されずに終わる。
foreach($item as $value)

// 関数に渡したらエラーになるかと思ったけど、ならなかった。最後まで謎だった。
function aa($str) {
    foreach ($str as $a){
        echo $a;
    }
}
$str = null;
aa($str);
```

---

## コールバック

phpのコールバックは3種類ある。  

可変関数なるものを使う方法とcall_user_func関数なるものを使う方法と無名関数を使う方法だ。  
GORAでは無名関数を使う方法でうまくいった。  
call_user_func関数は文字列で指定するのでなんかいやだ。  
できれば、関数名を文字列に変換するしてくれるnameofみたいなのがあればいいのだが、あるのだろうか。  
次から次へと気になるものがでてくるな。  

軽く調べてみたけど、なさそうですね。  
現在実行している関数名を取得する方法はあるけど、クラスの中のこの関数を文字列に変換するみたいな処理はないっぽい。  
まぁ、優先は無名関数で次にcall_user_func使えばいいと思うよ。  

[いい感じのコールバックサンプル](https://qiita.com/dublog/items/0eb8bcea2fc452c0b4b2)  
[PHPでコールバック関数を利用する](https://qiita.com/tricogimmick/items/23fb5958b6ea914bbfb5)  
とりあえず、困ったらここ見ればいいんじゃないかな。  

``` PHP : うまくいった無名関数を使ったAPI実行サンプル
function callGetAPI()
{
    // パラメータ生成
    $params = 'linkage_plan_ids=';
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
後でちゃんとした場所に書く。  
内容的にはネットワークの部類に入るのだろうけど、GORAのエラーで発生したのでここに書く。  
参考サイト的にはpeerとは相手のことを指すので、この場合、接続相手から接続を切られてしまったことを意味するとか。  
実務で発生した現象だと、管理サイトにはプランが出ていたので、リクエストは受け付けられた模様。  
その後の切断処理がうまくいかなかったと思われる。  
ここはプログラムでどうにかできる問題では無い。  
完全にインフラ側の問題。  
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

---

## PHPのオーバーロード

結論からいうと、PHPにオーバーロードはない。  
前にも調べた記憶があるので、この際に「ない」ということをまとめる。  

実現の仕方はあるらしいが、そうしたい場合は改めて調べてくれ。  
時間がなさすぎる。  

[PHPでのオーバーロード（引数ちがいの同名関数）](https://qiita.com/yasumodev/items/cf3da2a2f5547358e780)  

---

## エルビス演算子

phpにはfalseの時だけ実行するエルビス演算子なるものが存在する。  
falseの時だけ実行してほしい処理って割とあるので、C#にもあってほしいと願うのだがどうなるかね。  

```php : エルビス演算子の使いどころさん
// statusCodeが500の時エラーメッセージを取得する
$status = 500;
$message = 'err';
$err = null;

// falseの時だけ実行される。200 === 500でfalse。
$status === 200 ?: $err .= $message;

print $err ?? 'naiyo';
```

---

## 三項演算子

phpの三項演算子は癖が強く、なんでそうなるの？って動作をしたのでまとめたのだが、だいぶ前にまとめた内容なので、また綺麗にまとめたい。  
まぁ、優先度は低いし、次にGORAの修正をやる時にでも。  

``` php
<?php
// Your code here!
    const DELETE_TYPE_LOGICAL = 3;
    const DELETE_TYPE_PHYSICAL = 1;
    const DELETE_TYPE_LOGICAL_WITH_HAS_ACHIEVE = 0;
    const DELETE_TYPE_LOGICAL_WITH_NO_ACHIEVE = 2;
    
    $delType = DELETE_TYPE_PHYSICAL;
    $landingAchieveFlag = true;
    
    $extendDelType = $delType === DELETE_TYPE_PHYSICAL
        // 物理削除はそのまま
        ? DELETE_TYPE_PHYSICAL
        : $landingAchieveFlag
            // 論理削除+実績あり
            ? DELETE_TYPE_LOGICAL_WITH_HAS_ACHIEVE
            // 論理削除+実績なし
            : DELETE_TYPE_LOGICAL_WITH_NO_ACHIEVE;
            
    $extendDelType2 = (function ($delType, $landingAchieveFlag) {
        if ($delType === DELETE_TYPE_PHYSICAL) {
            return DELETE_TYPE_PHYSICAL;
        } else {
            if ($landingAchieveFlag) {
                return DELETE_TYPE_LOGICAL_WITH_HAS_ACHIEVE;
            } else {
                return DELETE_TYPE_LOGICAL_WITH_NO_ACHIEVE;
            }
        }
    })($delType,$landingAchieveFlag);
    
    var_dump($extendDelType);
    var_dump($extendDelType2);
?>
```
