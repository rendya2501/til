# PHP_演算子

---

## 演算子早見表

``` txt
-> : アロー演算子
=> : ダブルアロー演算子
:: : スコープ定義演算子
:? : エルビス演算子
```

---

## アロー演算子 [->]

クラスのメソッドやプロパティにアクセスするための演算子。  
左辺から右辺を取り出す。  

---

## ダブルアロー演算子 [=>]

配列や連想配列において、キーに対してバリューを指定する際に使用する演算子。  

``` php
// 配列
$names = array(0=>"山田", 1=>"田中", 2=>"髙橋");
// 連想配列
$person = array("name"=>"山田", "age"=>22, "gender"=>"男性");
```

---

## スコープ定義演算子 [::]

クラスの定数や静的なメソッドにアクセスする演算子。  

---

## エルビス演算子 [:?]

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

**PHPの三項演算子は右から順に評価される**。  

下記例では`$a`がTrueなので「あ」が出力されるはず。  
しかし、「い」が出力されてしまう。  
PHP8.0になってからなのか知らないが、ネストした演算を括弧で囲まない場合、エラーになる模様。  
PHP7時代では普通にできていた気がする。  

``` php
$a = true; $b = true;
// PHP 7時代は普通に実行されていたが今はエラーになる
// PHP Fatal error:  Unparenthesized `a ? b : c ? d : e` is not supported. Use either `(a ? b : c) ? d : e` or `a ? b : (c ? d : e)` in /workspace/Main.php on line 5
echo $a 
    ? 'あ' 
    : $b 
        ? 'い' 
        : 'う'; 
// い
```

ネスト部分を括弧で囲むと意図した通りに実行される。  

``` php
$a = true; $b = true;

echo $a 
    ? 'あ' 
    : ( $b 
        ? 'い' 
        : 'う' ); 
#=> あ
```

PHPでは参考演算子で関数の実行が可能。  
C#では不可能。  

``` php
function foo() { echo "foo\n";}
function bar() { echo "bar\n";}

$int = 1;
$int == 1 ? foo() : bar();
// foo

$int++;
$int == 1 ? foo() : bar();
// bar
```

---

## 参考

[【PHP】アロー演算子→とスコープ定義演算子::](https://qiita.com/sho91/items/c6503e7d344ca29aa03f)  
